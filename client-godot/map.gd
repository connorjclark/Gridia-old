extends Node2D

var TileSetLoader = preload("tile_set_loader.gd").new()

var tile_width = 32
var layers = []

func add_tilemap_layer(tileset_dir):
	var tilemap = TileMap.new()
	
	var tile_set = TileSetLoader.load_tileset(tileset_dir)
	
	tilemap.set_tileset(tile_set)
	#tilemap._update_dirty_quadrants()
	
	tilemap.set_cell_size(Vector2(tile_width, tile_width))
	#tilemap.set_mode(tilemap.MODE_ISOMETRIC)
	#tilemap.set_half_offset(tilemap.HALF_OFFSET_X)
	tilemap.set_collision_layer(1)
	tilemap.set_collision_mask(1)
	#tilemap.set_collision_use_kinematic(true)
	
	add_child(tilemap)
	tilemap.add_child(StaticBody2D.new())
	layers.push_back(tilemap)

func init():
	add_tilemap_layer("floors")
	add_tilemap_layer("items")
	#add_tilemap_layer("players")

func set_floor(x, y, tile):
	layers[0].set_cell(x, y, tile)

func set_item(x, y, tile):
	layers[1].set_cell(x, y, tile)

func set_creature(x, y, tile):
	layers[1].set_cell(x, y, tile)
