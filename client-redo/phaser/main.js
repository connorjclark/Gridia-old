"use strict";

var game = new Phaser.Game($(window).width(), $(window).height(), Phaser.CANVAS, 'phaser-example', { preload: preload, create: create, update: update, render: render });
var view = {x: 0, y: 0};
var cursors;
var tileSize = 32;
var chunkSize = 20;
var numFloorSheets = 6;
var numItemSheets = 27;
var numPlayerSheets = 1;
var sharedChunkData;
var itemsConfig;
var music;

var TileIndexer = (function() {
  var numFloors = numFloorSheets * 100;
  var numItems = numItemSheets * 100;

  return {
    getFloor: function(floorType) {
      // 1 is water. don't want to implement water templating yet, so default to 5
      if (floorType === 1) {
        return 5;
      }

      return floorType;
    },
    getItem: function(itemType) {
      var index = 0;

      if (itemsConfig[itemType] && itemsConfig[itemType].animations) {
        index = itemsConfig[itemType].animations[0] || 0;
      }

      return index + numFloors;
    },
    getPlayer: function(index) { return index + numFloors + numItems; }
  };
})();

var ChunkManager = (function() {
  var chunks = {};

  // TileMap.putTile does a lot of extra work that I do not need.
  function quickPutTile(map, index, x, y, layer) {
    layer = map.getLayer(layer);
    map.layers[layer].data[y][x] = new Phaser.Tile(map.layers[layer], index, x, y, map.tileWidth, map.tileHeight);
    map.layers[layer].dirty = true;
  }

  function loadChunk(x, y) {
    var map = game.add.tilemap();

    map.tilesets = sharedChunkData.tilesets;
    map.tiles = sharedChunkData.tiles;

    var floorLayer = map.create('floor-layer', chunkSize, chunkSize, tileSize, tileSize);
    var itemLayer = map.createBlankLayer('item-layer', chunkSize, chunkSize, tileSize, tileSize);
    var playerLayer = map.createBlankLayer('player-layer', chunkSize, chunkSize, tileSize, tileSize);

    floorLayer.fixedToCamera = false;
    itemLayer.fixedToCamera = false;
    playerLayer.fixedToCamera = false;

    game.load.json(x + ',' + y, 'assets/maps/demo-city/' + x + ',' + y + ',0.json', true);
    game.load.onLoadComplete.add(function(name, a) {
      if (!map.game) return; // necessary because this chunk could have been culled...somehow....

      var data = game.cache.getJSON(x + ',' + y);
      for (var i = 0; i < chunkSize; i++) {
        for (var j = 0; j < chunkSize; j++) {
          quickPutTile(map, TileIndexer.getFloor(data[i][j].floor), i, j, floorLayer);
          if (data[i][j].item && data[i][j].item.type) {
            quickPutTile(map, TileIndexer.getItem(data[i][j].item.type), i, j, itemLayer);
          }
        }
      }
    }, game);
    game.load.start();

    return {
      map: map,
      floorLayer: floorLayer,
      itemLayer: itemLayer,
      playerLayer: playerLayer,
      x: x,
      y: y
    }
  }

  function updateChunkPosition(chunk) {
    $.each([chunk.floorLayer, chunk.itemLayer, chunk.playerLayer], function() {
      this.position.set(chunk.x * tileSize * chunkSize - view.x, chunk.y * tileSize * chunkSize - view.y);
    });
  }

  return {
    getChunk: function(x, y) {
      var chunk = chunks[x + ',' + y];

      if (!chunk) {
        chunk = chunks[x + ',' + y] = loadChunk(x, y);
      }

      return chunk;
    },
    updateChunkPositions: function() {
      $.each(chunks, function() {
        updateChunkPosition(this);
      });
    },
    cull: function(minX, maxX, minY, maxY) {
      $.each(chunks, function(key, chunk) {
        var inViewport = chunk.x >= minX && chunk.x <= maxX && chunk.y >= minY && chunk.y <= maxY;
        
        if (!inViewport) {
          chunk.map.destroy();
          delete chunks[key];
        }
      });
    },
    chunks: chunks
  };
})();

function preload() {
  function loadTileSheet(type, i) {
    game.load.image(type + i, 'assets/' + type + '/' + type + i + '.png');
  }

  for (var i = 0; i < numFloorSheets; i++) loadTileSheet('floors', i);
  for (var i = 0; i < numItemSheets; i++) loadTileSheet('items', i);
  for (var i = 0; i < numPlayerSheets; i++) loadTileSheet('players', i);

  game.load.json('items', 'assets/content/items.json');

  game.load.audio('music', 'assets/sound/music/scythuz/Spring Breeze.ogg');

  game.time.advancedTiming = true;
}

function create() {
  itemsConfig = game.cache.getJSON('items');
  game.stage.backgroundColor = '#2d2d2d';
  sharedChunkData = createSharedChunkData();
  cursors = game.input.keyboard.createCursorKeys();
  updateChunks();
  music = game.add.audio('music');
  music.play();
}

function createSharedChunkData() {
  var map = game.add.tilemap();
  var gid = 0;

  function loadTileSheet(type, i) {
    map.addTilesetImage(type + i, type + i, tileSize, tileSize, 0, 0, gid);
    gid += 100;
  }

  for (var i = 0; i < numFloorSheets; i++) loadTileSheet('floors', i);
  for (var i = 0; i < numItemSheets; i++) loadTileSheet('items', i);
  for (var i = 0; i < numPlayerSheets; i++) loadTileSheet('players', i);

  return {tilesets: map.tilesets, tiles: map.tiles};
}

function update() {
  var posChanged = false;
  var speed = 4;

  if (cursors.left.isDown) {
    view.x -= speed;
    posChanged = true;
  } else if (cursors.right.isDown) {
    view.x += speed;
    posChanged = true;
  }

  if (cursors.up.isDown) {
    view.y -= speed;
    posChanged = true;
  } else if (cursors.down.isDown) {
    view.y += speed;
    posChanged = true;
  }

  if (posChanged) {
    updateChunks();
  }
}

function updateChunks() {
  var minChunkX = (view.x / tileSize / chunkSize) | 0;
  var maxChunkX = ((view.x + game.width) / tileSize / chunkSize) | 0;
  var minChunkY = (view.y / tileSize / chunkSize) | 0;
  var maxChunkY = ((view.y + game.height) / tileSize / chunkSize) | 0;

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
  ChunkManager.updateChunkPositions();
}

function render() {
  game.debug.text(game.time.fps || '--', 2, 14, "#00ff00");
}
