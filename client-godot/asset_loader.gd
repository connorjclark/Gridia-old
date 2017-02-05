extends Node

signal resources_updated

# need request_completed signal to include the url used
class MyHTTPRequest extends HTTPRequest:
	var url
	
	func request(url, custom_headers=StringArray([]), ssl_validate_domain=true, method=0, request_data=""):
		self.url = url
		return .request(url, custom_headers, ssl_validate_domain, method, request_data)
	
	func _request_done(p_status, p_code,  headers,  p_data):
		cancel_request()
		emit_signal("request_completed", url, p_status, p_code, headers, p_data)


var num_outgoing_requests = 0
var queued_requests = []
var success = true
var root_url = "http://localhost:8080"
var last_modified_file_name = "res://last-modified.json"
var max_concurrent_requests = 20
var last_modified_times = {}


func _ready():
	load_last_modified_times()
	queue_request(root_url)
	set_process(true)


func _process(delta):
	if queued_requests.size() > 0 && num_outgoing_requests < max_concurrent_requests:
		make_next_request()
	
	if queued_requests.size() == 0 && num_outgoing_requests == 0:
		on_done()
	
	get_node("Label").set_text("%s files left" % (queued_requests.size() + num_outgoing_requests))


func load_last_modified_times():
	var file = File.new()
	
	if file.open(last_modified_file_name, file.READ) == 0:
		var json = file.get_as_text()
		last_modified_times.parse_json(json)


func save_last_modified_times():
	var file = File.new()
	file.open(last_modified_file_name, file.WRITE)
	file.store_string(last_modified_times.to_json())


func make_next_request():
	var request = queued_requests[-1]
	queued_requests.pop_back()
	var url = request[0]
	var req = request[1]
	var headers = request[2]
	
	num_outgoing_requests += 1
	req.request(url, headers, false)


func queue_request(url, res_path = ""):
	var is_dir_request = res_path == ""
	var method = ""
	var headers = []
	
	if is_dir_request:
		method = "on_dir_request_completed"
	else:
		method = "on_request_completed"
	
	var request = MyHTTPRequest.new()
	request.connect("request_completed", self, method)
	add_child(request)
	
	if !is_dir_request && File.new().file_exists(res_path) && last_modified_times.has(res_path):
		var cache_header = "If-Modified-Since: %s" % last_modified_times[res_path]
		
		headers.push_back(cache_header)
		headers.push_back("Cache-Control: max-age=0")
	
	if !is_dir_request:
		request.set_download_file(res_path)
	
	queued_requests.push_back([url, request, headers])


# simple href scraper
func find_all_hrefs(html):
	var result = []
	var index = html.find("BODY")
	var href_attr = "HREF=\""
	
	while index != -1:
		index = html.find(href_attr, index)
		
		if index != -1:
			var start = index + href_attr.length()
			index = html.find("\">", index)
			
			if index != -1:
				var href = html.substr(start, index - start)
				
				if href.find("/../") == -1:
					result.append(href)
			
	
	return result


func on_dir_request_completed(url, result, response_code, headers, body):
	num_outgoing_requests -= 1
	success = success && result == 0
	
	for href in find_all_hrefs(body.get_string_from_ascii()):
		var is_dir = href.ends_with("/")
		var res_path = ""
		var url = "%s%s" % [root_url, href]
		
		if is_dir:
			Directory.new().make_dir("res:/%s" % href)
		else:
			res_path = "res:/%s" % href.replace("%20", " ")
		
		queue_request(url, res_path)


func find_last_modified_header(headers):
	for header in headers:
		if header.begins_with("Last-Modified:"):
			return header.replace("Last-Modified:", "")
	
	return ""


func on_request_completed(url, result, response_code, headers, body):
	num_outgoing_requests -= 1
	success = success && result == 0
	
	if response_code == 200:
		var resource_path = url.replace(root_url, "res:/").replace("%20", " ")
		last_modified_times[resource_path] = find_last_modified_header(headers)

var once = false

func on_done():
	set_process(false)
	save_last_modified_times()
	emit_signal("resources_updated", success)
	queue_free()