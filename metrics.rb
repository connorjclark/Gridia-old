def doMetrics(dir, extensions)
  result = { loc: 0, dir: dir, extensions: extensions }
  extensions.each { |ext|
    Dir.glob("#{dir}/**/*.#{ext}") { |file_path|
      File.open(file_path) { |file|
        result[:loc] += file.readlines.size
      }
    }
  }
  result
end

server_metrics =  doMetrics 'Server', ['java']
client_metrics =  doMetrics 'Client', ['cs']
test_world_metrics =  doMetrics 'Server/GridiaServer/TestWorld/clientdata/content', ['*']

puts server_metrics
puts client_metrics
puts server_metrics[:loc] + client_metrics[:loc]
puts test_world_metrics
