func dir_files(path):
	var result = []
	var dir = Directory.new()

	if dir.open(path) == OK:
		dir.list_dir_begin()
		var file_name = dir.get_next()
		while file_name != "":
			if file_name != ".." && file_name != ".":
				if !dir.current_is_dir():
					result.push_back(file_name)

			file_name = dir.get_next()
	
	return result