print "Build name?\n> "
build_name = $stdin.gets.chomp

base_dir = "#{Dir.pwd}/builds/#{build_name}"
win32 = "-buildWindowsPlayer #{base_dir}/win32/client.exe"
win64 = "-buildWindows64Player #{base_dir}/win64/client.exe"
osx = "-buildOSXPlayer #{base_dir}/osx/client.app"
linux32 = "-buildLinux32Player #{base_dir}/linux32/client.app"
linux64 = "-buildLinux64Player #{base_dir}/linux64/client.app"

unity = '"%PROGRAMFILES%/Unity/Editor/Unity.exe"'
`#{unity} -batchmode -quit -projectPath E:/Dev/Gridia/Client #{win32} #{win64} #{osx} #{linux32} #{linux64}`
