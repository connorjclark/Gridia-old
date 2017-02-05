extends Node

onready var asset_loader = get_node("AssetLoader")
onready var player = get_node("Player")
onready var map = get_node("Map")

func _ready():
	asset_loader.connect("resources_updated", self, "on_resources_updated")
	set_fixed_process(true)

func on_resources_updated(success):
	if success:
		map.init()
		
		var size = 30

		for x in range(size):
			for y in range(size):
				map.set_floor(x, y, 21)
				
				var edge = x == 0 || y == 0 || x == size - 1 || y == size - 1
				
				var path = y % 2 == 0 && x > 3 && x < size - 1 - 3
				
				if edge || path:
					map.set_item(x, y, 10)

var direction = Vector2()

func _fixed_process(delta):
	var speed = 5 * 32 * delta
	var dx = 0
	var dy = 0
	
	if (Input.is_action_pressed("move_left")):
		direction.x = -1
	elif (Input.is_action_pressed("move_right")):
		direction.x = 1
	else:
		direction.x = 0
	
	if (Input.is_action_pressed("move_up")):
		direction.y = -1
	elif (Input.is_action_pressed("move_down")):
		direction.y = 1
	else:
		direction.y = 0
	
	direction = direction.normalized()

	var motion = direction * speed
	motion = player.move(motion) 

	if (player.is_colliding()):
		var n = player.get_collision_normal()
		motion = n.slide(motion) 
		direction = n.slide(direction)
		player.move(motion)
