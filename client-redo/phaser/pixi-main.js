"use strict";

var stage;
var view = {x: 0, y: 0};
var numFloorSheets = 6;
var numTemplateSheets = 0;
var numItemSheets = 27;
var numPlayerSheets = 1;
var sharedChunkData;
var itemsConfig;
var music;
var tileSize = 32;
var chunkSize = 20;

var TileIndexer = (function() {
  return {
    getFloor: function(floorType) {
      // 1 is water. don't want to implement water templating yet, so default to 5
      if (floorType === 1) {
        floorType = 5;
      }

      var tileSheetIndex = (floorType / 100) | 0;
      var baseTexture = PIXI.utils.TextureCache["assets/floors/floors" + tileSheetIndex + ".png"];
      floorType = floorType % 100;
      var rect = new PIXI.Rectangle((floorType%10)*tileSize, ((floorType/10)|0)*tileSize, tileSize, tileSize);
      
      return new PIXI.Texture(baseTexture, rect);
    },
    getItem: function(itemType) {
      var index = 0;

      if (itemsConfig[itemType] && itemsConfig[itemType].animations) {
        index = itemsConfig[itemType].animations[0] || 0;
      }

      var tileSheetIndex = (index / 100) | 0;
      var baseTexture = PIXI.utils.TextureCache["assets/items/items" + tileSheetIndex + ".png"];
      index = index % 100;
      var rect = new PIXI.Rectangle((index%10)*tileSize, ((index/10)|0)*tileSize, tileSize, tileSize);

      return new PIXI.Texture(baseTexture, rect);
    }
  };
})();

function createFloor(x, y, floor) {
  var sprite = new PIXI.Sprite(TileIndexer.getFloor(floor));
  sprite.x = x * tileSize;
  sprite.y = y * tileSize;
  return sprite;
}

function createItem(x, y, item) {
  var sprite = new PIXI.Sprite(TileIndexer.getItem(item));
  sprite.x = x * tileSize;
  sprite.y = y * tileSize;
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

    var floors = [];
    var items = [];

    var url = 'assets/maps/demo-city/' + x + ',' + y + ',0.json';
    PIXI.loader.add(url).load(function() {
      var data = PIXI.loader.resources[url].data;

      for (var i = 0; i < chunkSize; i++) {
        for (var j = 0; j < chunkSize; j++) {
          var floor = createFloor(x*chunkSize + i, y*chunkSize + j, data[i][j].floor);
          floors.push(floor);
          addToParticleContainers(floorParticleContainers, floor, floorLayer);

          if (data[i][j].item && data[i][j].item.type) {
            var item = createItem(x*chunkSize + i, y*chunkSize + j, data[i][j].item.type);
            items.push(item);
            addToParticleContainers(itemParticleContainers, item, itemLayer);
          }
        }
      }

      delete PIXI.loader.resources[url];
    });

    return {
      floors: floors,
      items: items,
      x: x,
      y: y
    }
  }

  return {
    getChunk: function(x, y) {
      var chunk = chunks[x + ',' + y];

      if (!chunk) {
        chunk = chunks[x + ',' + y] = loadChunk(x, y);
      }

      return chunk;
    },
    cull: function(minX, maxX, minY, maxY) {
      $.each(chunks, function(key, chunk) {
        var inViewport = chunk.x >= minX && chunk.x <= maxX && chunk.y >= minY && chunk.y <= maxY;
        
        if (!inViewport) {
          console.log('culling', chunk.x, chunk.y);

          $.each(chunk.floors, function() {
            removeFloor(this);
            this.destroy();
          });
          $.each(chunk.items, function() {
            removeItem(this);
            this.destroy();
          });

          delete chunks[key];
        }
      });
    },
    chunks: chunks,
    floorLayer: floorLayer,
    itemLayer: itemLayer,
    floorParticleContainers: floorParticleContainers,
    itemParticleContainers: itemParticleContainers
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
      ChunkManager.getChunk(x, y); // just to load it.
    }
  }

  ChunkManager.cull(minChunkX-1, maxChunkX+1, minChunkY-1, maxChunkY+1);
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

    stage.addChild(ChunkManager.floorLayer);
    stage.addChild(ChunkManager.itemLayer);

    requestAnimationFrame(update);
  }

  function update() {
    requestAnimationFrame(update);

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
