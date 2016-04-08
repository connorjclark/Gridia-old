def run_command(command)
  fixed_command = command
    .gsub(%r"([^ /])/", '\1\\')
    .gsub('//', '/')
  puts fixed_command
  `#{fixed_command}`
end

def for_each_platform(&proc)
  Dir.glob("#{$build_dir}/**").select { |f|
    File.directory? f and not f.split('/').last.include? 'server-standalone'
  }.each { |f|
    proc.call(f)
  }
end

print "Build name?\n> "
build_name = $stdin.gets.chomp

base_dir = Dir.pwd
$build_dir = build_dir = "#{base_dir}/builds/#{build_name}"
server_dir = "#{build_dir}/gridia-#{build_name}-server-standalone"
win32 = "-buildWindowsPlayer #{build_dir}/gridia-#{build_name}-win32/client.exe"
win64 = "-buildWindows64Player #{build_dir}/gridia-#{build_name}-win64/client.exe"
osx = "-buildOSXPlayer #{build_dir}/gridia-#{build_name}-osx/client.app"
linux32 = "-buildLinux32Player #{build_dir}/gridia-#{build_name}-linux32/client.app"
linux64 = "-buildLinux64Player #{build_dir}/gridia-#{build_name}-linux64/client.app"
wp = "-buildWebPlayer #{build_dir}/gridia-#{build_name}-wp"

puts 'building clients for each platform'
`unity -batchmode -quit -projectPath #{base_dir}/Client #{win32} #{win64} #{osx} #{linux32} #{linux64} #{wp}`

puts 'running maven'
`cd server/ & mvn clean compile assembly:single`

puts 'making server-standalone'
run_command "echo f | xcopy server/target/server.jar #{server_dir}/server.jar"
run_command "echo f | xcopy server/splash.txt #{server_dir}/splash.txt"
run_command "xcopy server/worlds/demo-world/clientdata #{server_dir}/worlds/demo-world/clientdata //E //i"
run_command "xcopy server/worlds/demo-world/scripts #{server_dir}/worlds/demo-world/scripts //E //i"
run_command "xcopy server/worlds/demo-world/maps/demo-city #{server_dir}/worlds/demo-world/maps/demo-city //E //i"

puts 'copying server to all platforms but webplayer'
for_each_platform do |f|
  run_command "xcopy #{server_dir} #{f} //E //i" unless f.include? "gridia-#{build_name}-wp"
end

puts 'copying instructions.odt'
for_each_platform do |f|
  run_command "copy #{base_dir}/instructions.odt #{f}"
end

# puts 'zipping standalone server'
# run_command "7z a #{server_dir}.zip #{server_dir}"

# puts 'zipping all platforms'
# for_each_platform do |f|
#   run_command "7z a #{f}.zip #{f}"
# end

# puts 'cleaning up'
# run_command "rd #{server_dir} //Q //S"
# for_each_platform do |f|
#   run_command "rd #{f} //Q //S"
# end
