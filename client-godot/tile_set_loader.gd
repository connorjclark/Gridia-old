var tile_width = 32
var spritesheet_size = 10 # size in tiles

var shaders = {}

# simple shader that varies the frame in a tilesheet
# for frames.size() == 2, shader would look like:
#
#		uniform vec2 v0;
#		uniform vec2 v1;
#		
#		vec2 offset;
#		float time = mod(TIME, 1.0);
#		
#		if (time < 0.5) {
#			offset = v0;
#		} else {
#			offset = v1;
#		}
#	
#		UV = UV + offset;
#
func get_shader(item):
	var frames = item.animations
	
	if shaders.has(frames.size()):
		return shaders[frames.size()]
	
	var shader = CanvasItemShader.new()
	
	var vcode = ""
	
	for i in range(frames.size()):
		vcode += "uniform vec2 v%s;\n" % i
	
	vcode += "vec2 offset;\n"
	vcode += "float time = mod(TIME, 1.0);\n"
	vcode += make_shader_if_else_helper(0, frames.size())
	vcode += "UV = UV + offset;"
	
	shader.set_code(vcode, "", "")
	
	shaders[frames.size()] = shader
	return shader

func make_shader_if_else_helper(i, num_frames):
	var first = i == 0
	var last = i == num_frames - 1
	var tcond = 1.0 * (i + 1) / num_frames
	
	var code = ""
	
	if last:
		code += "offset = v%s;\n" % i
	else:
		code += """
			if (time < %s) {
				offset = v%s;
			} else {
				%s
			}
		""" % [tcond, i, make_shader_if_else_helper(i + 1, num_frames)]
	
	return code

func get_material_for_item(item):
	var material = CanvasItemMaterial.new()
	
	material.set_shader(get_shader(item))
	
	var origin_x = item.animations[0] % spritesheet_size
	var origin_y = (item.animations[0] / spritesheet_size) % spritesheet_size
	
	for i in range(item.animations.size()):
		var x = item.animations[i] % spritesheet_size
		var y = (item.animations[i] / spritesheet_size) % spritesheet_size
		
		x = (x - origin_x) / 10.0
		y = (y - origin_y) / 10.0
		
		material.set_shader_param("v%s" % i, Vector2(x, y))
	
	return material

func load_tileset(content_type):
	var tileset = TileSet.new()
	
	var tile_index = 0
	
	var textures = []
	while File.new().file_exists("res://%s/%s%s.png" % [content_type, content_type, textures.size()]):
		var texture = load("res://%s/%s%s.png" % [content_type, content_type, textures.size()])
		textures.push_back(texture)
	
	if content_type == "items":
		var items = []
		var content_file = File.new()
		
		if content_file.open("res://content/%s.json" % content_type, content_file.READ) == 0:
			var json = content_file.get_as_text()
			items = str2var(json)
		
		for item in items:
			var texture = null
			var frame = 0
			if item != null && item.has('animations'):
				frame = item.animations[0]
				texture = textures[floor(frame / (spritesheet_size * spritesheet_size))]
			
			var x = frame % spritesheet_size
			var y = (frame / spritesheet_size) % spritesheet_size
			
			tileset.create_tile(tile_index)
			tileset.tile_set_name(tile_index, str(tile_index))
			tileset.tile_set_texture(tile_index, texture)
			tileset.tile_set_region(tile_index, Rect2(x * tile_width, y * tile_width, tile_width, tile_width))
			
			var shape = CapsuleShape2D.new()
			shape.set_radius(tile_width / 2 * 0.9)
			shape.set_height(tile_width / 4 * 0.8)
			#shape.set_extents(Vector2(tile_width / 2, tile_width / 2))
			tileset.tile_set_shape(tile_index, shape)
			tileset.tile_set_shape_offset(tile_index, Vector2(tile_width / 2, tile_width / 2))
			
			if item != null && item.has('animations') && item.animations.size() > 1:
				tileset.tile_set_material(tile_index, get_material_for_item(item))
			
			tile_index += 1
	else:
		for texture in textures:
			for y in range(spritesheet_size):
				for x in range(spritesheet_size):
					tileset.create_tile(tile_index)
					tileset.tile_set_name(tile_index, str(tile_index))
					tileset.tile_set_texture(tile_index, texture)
					tileset.tile_set_region(tile_index, Rect2(x * tile_width, y * tile_width, tile_width, tile_width))
					tile_index += 1
	
	return tileset