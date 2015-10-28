"use strict";

var stage;
var previousGlobalTick;
var globalTick;
var view = {x: 1550, y: 5440};
var numFloorSheets = 6;
var numTemplateSheets = 1;
var numItemSheets = 27;
var numPlayerSheets = 1;
var sharedChunkData;
var itemsConfig;
var music;
var tileSize = 32;
var chunkSize = 20;

var TileIndexer = (function() {
  return {
    getFloor: function(floorType, x, y) {
      var baseTexture, index;

      // water tiling
      if (floorType === 1) {
        index = useTemplate(0, 1, x, y);
        var tileSheetIndex = (index / 100) | 0;
        baseTexture = PIXI.utils.TextureCache["assets/templates/templates" + tileSheetIndex + ".png"];
        index = index % 100;
      } else {
        var tileSheetIndex = (floorType / 100) | 0;
        baseTexture = PIXI.utils.TextureCache["assets/floors/floors" + tileSheetIndex + ".png"];
        index = floorType % 100;
      }

      var rect = new PIXI.Rectangle((index%10)*tileSize, ((index/10)|0)*tileSize, tileSize, tileSize);
      
      return new PIXI.Texture(baseTexture, rect);
    },
    getItem: function(itemType, tick) {
      var index = 0;
      var width = tileSize;
      var height = tileSize;

      if (itemsConfig[itemType] && itemsConfig[itemType].animations) {
        index = itemsConfig[itemType].animations[tick % itemsConfig[itemType].animations.length] || 0;
        width *= itemsConfig[itemType].imageWidth || 1;
        height *= itemsConfig[itemType].imageHeight || 1;
      }

      var tileSheetIndex = (index / 100) | 0;
      var baseTexture = PIXI.utils.TextureCache["assets/items/items" + tileSheetIndex + ".png"];
      index = index % 100;
      var rect = new PIXI.Rectangle((index%10)*tileSize, ((index/10)|0)*tileSize, width, height);

      return new PIXI.Texture(baseTexture, rect);
    }
  };
})();

function createFloor(x, y, floor) {
  var sprite = new PIXI.Sprite(TileIndexer.getFloor(floor, x, y));
  sprite.x = x * tileSize;
  sprite.y = y * tileSize;
  return sprite;
}

function createItem(x, y, item) {
  var sprite = new PIXI.Sprite(TileIndexer.getItem(item, globalTick));
  sprite.x = x * tileSize;
  sprite.y = y * tileSize - sprite.height + tileSize; // offset for items taller than one tile
  return sprite;
}

var ChunkManager = (function() {
  var chunks = {};

  // particle container draw sprites very quickly
  // but all children must have the same base texture
  // so, one particle container per tilesheet
  var floorParticleContainers = [];
  var itemParticleContainers = [];

  var floorLayer = new PIXI.Container();
  var itemLayer = new PIXI.Container();
  
  function addToParticleContainers(particleContainers, sprite, layer) {
    if (!particleContainers[sprite.texture.baseTexture.imageUrl]) {
      particleContainers[sprite.texture.baseTexture.imageUrl] = new PIXI.ParticleContainer();
      layer.addChild(particleContainers[sprite.texture.baseTexture.imageUrl]);
    }
    particleContainers[sprite.texture.baseTexture.imageUrl].addChild(sprite);
  }

  function removeFloor(floor) {
    floorParticleContainers[floor.texture.baseTexture.imageUrl].removeChild(floor);
  }

  function removeItem(item) {
    itemParticleContainers[item.texture.baseTexture.imageUrl].removeChild(item);
  }

  function loadChunk(x, y) {
    console.log('loading', x, y);

    var chunk = {
      floors: [],
      items: [],
      x: x,
      y: y,
      data: [],
      redrawFloor: function(x, y) {
        var floor = this.floors[x % chunkSize][y % chunkSize];

        // need to remove from current particle container
        floorParticleContainers[floor.texture.baseTexture.imageUrl].removeChild(floor);

        // update data
        floor.texture = TileIndexer.getFloor(this.data[x][y].floor, x, y);

        // place in correct particle container
        if (!floorParticleContainers[floor.texture.baseTexture.imageUrl]) {
          floorParticleContainers[floor.texture.baseTexture.imageUrl] = new PIXI.ParticleContainer();
          floorLayer.addChild(floorParticleContainers[floor.texture.baseTexture.imageUrl]);
        }
        floorParticleContainers[floor.texture.baseTexture.imageUrl].addChild(floor);
      },
      redrawItem: function(x, y) {
        var item = this.items[x % chunkSize][y % chunkSize];

        var previousBaseTexture = item.texture.baseTexture;
        item.texture = TileIndexer.getItem(this.data[x][y].item.type, globalTick);
        var newBaseTexture = item.texture.baseTexture;

        // TODO for some reason it's necessary to remove and add the sprite, always.
        if (previousBaseTexture !== newBaseTexture || true) {
          // need to remove from previous particle container
          itemParticleContainers[previousBaseTexture.imageUrl].removeChild(item);

          // place in correct particle container
          if (!itemParticleContainers[newBaseTexture.imageUrl]) {
            itemParticleContainers[newBaseTexture.imageUrl] = new PIXI.ParticleContainer();
            itemLayer.addChild(itemParticleContainers[newBaseTexture.imageUrl]);
          }
          itemParticleContainers[newBaseTexture.imageUrl].addChild(item);
        }
      },
      redrawAnimations: function() {
        if (!this.data.length) return;

        for (var i = 0; i < chunkSize; i++) {
          for (var j = 0; j < chunkSize; j++) {
            if (itemsConfig[this.data[i][j].item.type].animations.length > 1) {
              this.redrawItem(i, j);
            }
          }
        }
      }
    };

    chunks[x + ',' + y] = chunk;

    var url = 'assets/maps/demo-city/' + x + ',' + y + ',0.json';
    PIXI.loader.add(url).load(function() {
      chunk.data = PIXI.loader.resources[url].data;

      for (var i = 0; i < chunkSize; i++) {
        chunk.floors[i] = [];
        chunk.items[i] = [];
        for (var j = 0; j < chunkSize; j++) {
          var floor = createFloor(x*chunkSize + i, y*chunkSize + j, chunk.data[i][j].floor);
          chunk.floors[i][j] = floor;
          addToParticleContainers(floorParticleContainers, floor, floorLayer);

          var item = createItem(x*chunkSize + i, y*chunkSize + j, chunk.data[i][j].item.type);
          chunk.items[i][j] = item;
          addToParticleContainers(itemParticleContainers, item, itemLayer);
        }
      }

      delete PIXI.loader.resources[url];
    });

    return chunk;
  }

  return {
    getChunk: function(x, y) {
      var chunk = chunks[x + ',' + y];

      if (!chunk) {
        chunk = loadChunk(x, y);
      }

      return chunk;
    },
    cull: function(minX, maxX, minY, maxY) {
      $.each(chunks, function(key, chunk) {
        var inViewport = chunk.x >= minX && chunk.x <= maxX && chunk.y >= minY && chunk.y <= maxY;
        
        if (!inViewport) {
          console.log('culling', chunk.x, chunk.y);

          $.each(chunk.floors, function() {
            $.each(this, function() {
              removeFloor(this);
              this.destroy();
            });
          });
          $.each(chunk.items, function() {
            $.each(this, function() {
              removeItem(this);
              this.destroy();
            });
          });

          delete chunks[key];
        }
      });
    },
    chunks: chunks,
    floorLayer: floorLayer,
    itemLayer: itemLayer
  };
})();

function updateChunks() {
  var minChunkX = (view.x / tileSize / chunkSize) | 0;
  var maxChunkX = ((view.x + window.innerWidth) / tileSize / chunkSize) | 0;
  var minChunkY = (view.y / tileSize / chunkSize) | 0;
  var maxChunkY = ((view.y + window.innerHeight) / tileSize / chunkSize) | 0;

  // console.log(minChunkX, maxChunkX, minChunkY, maxChunkY);

  // hard code size of map
  minChunkX = Math.max(minChunkX, 0);
  minChunkY = Math.max(minChunkY, 0);
  maxChunkX = Math.min(maxChunkX, 14);
  maxChunkY = Math.min(maxChunkY, 14);

  for (var x = minChunkX; x <= maxChunkX; x++) {
    for (var y = minChunkY; y <= maxChunkY; y++) {
      var chunk = ChunkManager.getChunk(x, y); // this will load it if necessary
      if (previousGlobalTick !== globalTick) {
        chunk.redrawAnimations();
      }
    }
  }

  ChunkManager.cull(minChunkX-1, maxChunkX+1, minChunkY-1, maxChunkY+1);
}

function getTile(x, y) {
  if (x < 0 || y < 0) return {floor:1, item:{type:0}};
  var chunkX = (x / chunkSize) | 0;
  var chunkY = (y / chunkSize) | 0;
  var chunk = ChunkManager.chunks[chunkX + ',' + chunkY];
  return chunk && chunk.data.length && chunk.data[0].length ? chunk.data[x % chunkSize][y % chunkSize] : {floor:1, item:{type:0}};
}

function getFloor(x, y) {
  return getTile(x, y).floor;
}

function getItem(x, y) {
  return getTile(x, y).item;
}

function keyboard(keyCode) {
  var key = {};
  key.code = keyCode;
  key.isDown = false;
  key.isUp = true;
  key.press = undefined;
  key.release = undefined;
  //The `downHandler`
  key.downHandler = function(event) {
    if (event.keyCode === key.code) {
      if (key.isUp && key.press) key.press();
      key.isDown = true;
      key.isUp = false;
    }
    event.preventDefault();
  };

  //The `upHandler`
  key.upHandler = function(event) {
    if (event.keyCode === key.code) {
      if (key.isDown && key.release) key.release();
      key.isDown = false;
      key.isUp = true;
    }
    event.preventDefault();
  };

  //Attach event listeners
  window.addEventListener(
    "keydown", key.downHandler.bind(key), false
  );
  window.addEventListener(
    "keyup", key.upHandler.bind(key), false
  );
  return key;
}

$(function() {
  var renderer = PIXI.autoDetectRenderer(256, 256);

  document.body.appendChild(renderer.view);

  renderer.view.style.position = "absolute"
  renderer.view.style.display = "block";
  renderer.autoResize = true;
  renderer.resize(window.innerWidth, window.innerHeight);

  stage = new PIXI.Container();

  function loadTileSheet(type, i) {
    PIXI.loader.add(type + i, 'assets/' + type + '/' + type + i + '.png');
  }

  for (var i = 0; i < numFloorSheets; i++) loadTileSheet('floors', i);
  for (var i = 0; i < numTemplateSheets; i++) loadTileSheet('templates', i);
  for (var i = 0; i < numItemSheets; i++) loadTileSheet('items', i);
  for (var i = 0; i < numPlayerSheets; i++) loadTileSheet('players', i);

  PIXI.loader.add("assets/content/items.json");
  
  PIXI.loader.load(setup);

  var left = keyboard(37),
      up = keyboard(38),
      right = keyboard(39),
      down = keyboard(40);

  function setup() {
    itemsConfig = PIXI.loader.resources["assets/content/items.json"].data;

    // hack for displating a blank image on 0 itemType. items.json is set to show a '?'
    itemsConfig[0].animations = [1];

    stage.addChild(ChunkManager.floorLayer);
    stage.addChild(ChunkManager.itemLayer);

    requestAnimationFrame(update);
  }

  function update() {
    requestAnimationFrame(update);

    previousGlobalTick = globalTick;
    globalTick = Math.floor(Date.now() / 1000 * 5);

    if (right.isDown) {
      view.x += 4;
    }
    if (left.isDown) {
      view.x -= 4;
    }
    if (down.isDown) {
      view.y += 4;
    }
    if (up.isDown) {
      view.y -= 4;
    }

    ChunkManager.floorLayer.x = ChunkManager.itemLayer.x = -view.x;
    ChunkManager.floorLayer.y = ChunkManager.itemLayer.y = -view.y;
    
    updateChunks();

    renderer.render(stage);
  }
});

/* crazy code for water templating */

function useTemplate(templateId, typeToMatch, x, y, z) {
  var xl = x - 1;
  var xr = x + 1;
  var yu = y - 1;
  var yd = y + 1;

  var above = getFloor(x, yu, z) === typeToMatch;
  var below = getFloor(x, yd, z) === typeToMatch;
  var left = getFloor(xl, y, z) === typeToMatch;
  var right = getFloor(xr, y, z) === typeToMatch;

  var offset = templateId * 50;
  var v = (above ? 1 : 0) + (below ? 2 : 0) + (left ? 4 : 0) + (right ? 8 : 0);

  // this is where the complicated crap kicks in
  // i'd really like to replace this.
  // :'(
  // this is mostly guess work. I think. I wrote this code years ago. I know it works,
  // so I just copy and pasted. Shame on me.

  var upleft = getFloor(xl, yu, z) === typeToMatch;
  var upright = getFloor(xr, yu, z) === typeToMatch;
  var downleft = getFloor(xl, yd, z) === typeToMatch;
  var downright = getFloor(xr, yd, z) === typeToMatch;

  if (v == 15)
  {
      if (!upleft)
      {
          v++;
      }
      if (!upright)
      {
          v += 2;
      }
      if (!downleft)
      {
          v += 4;
      }
      if (!downright)
      {
          v += 8;
      }
  }
  else if (v == 5)
  {
      if (!upleft)
      {
          v = 31;
      }
  }
  else if (v == 6)
  {
      if (!downleft)
      {
          v = 32;
      }
  }
  else if (v == 9)
  {
      if (!upright)
      {
          v = 33;
      }
  }
  else if (v == 10)
  {
      if (!downright)
      {
          v = 34;
      }
  }
  else if (v == 7)
  {
      if (!downleft || !upleft)
      {
          v = 34;
          if (!downleft)
          {
              v++;
          }
          if (!upleft)
          {
              v += 2;
          }
      }
  }
  else if (v == 11)
  {
      if (!downright || !upright)
      {
          v = 37;
          if (!downright)
          {
              v++;
          }
          if (!upright)
          {
              v += 2;
          }
      }
  }
  else if (v == 13)
  {
      if (!upright || !upleft)
      {
          v = 40;
          if (!upright)
          {
              v++;
          }
          if (!upleft)
          {
              v += 2;
          }
      }
  }
  else if (v == 14)
  {
      if (!downright || !downleft)
      {
          v = 43;
          if (!downright)
          {
              v++;
          }
          if (!downleft)
          {
              v += 2;
          }
      }
  }

  return v + offset;
}