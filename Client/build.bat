@echo off

ECHO Enter name for build
set /p BUILDNAME=Name: 

set PROJECT=-projectPath
set PROJECT_PATH="E:\Dev\Gridia\Client"
set WIN_PATH="build\%BUILDNAME%\win32\client.exe"
set OSX_PATH="build\%BUILDNAME%\osx\client.app"
@REM With Unity 4 we now have Linux
set LINUX_PATH="build\%BUILDNAME%\linux\client.app"
set LINUX64_PATH="build\%BUILDNAME%\linux64\client.app"
@REM Common options
set BATCH=-batchmode
set QUIT=-quit
@REM Builds:
set WIN=-buildWindowsPlayer %WIN_PATH%
set OSX=-buildOSXPlayer %OSX_PATH%
set LINUX=-buildLinux32Player %LINUX_PATH%
set LINUX64=-buildLinux64Player %LINUX64_PATH%
@REM Win32 build
echo Running Win Build for: %PROJECT_PATH%
echo "%PROGRAMFILES%\Unity\Editor\Unity.exe" %BATCH% %QUIT% %PROJECT% %PROJECT_PATH% %WIN%
"%ProgramFiles(x86)%\Unity\Editor\Unity.exe" %BATCH% %QUIT% %PROJECT% %PROJECT_PATH% %WIN%
@REM OSX build
echo Running OSX Build for: %PROJECT_PATH%
echo "%PROGRAMFILES%\Unity\Editor\Unity.exe" %BATCH% %QUIT% %PROJECT% %PROJECT_PATH% %OSX%
"%ProgramFiles(x86)%\Unity\Editor\Unity.exe" %BATCH% %QUIT% %PROJECT% %PROJECT_PATH% %OSX%
@REM Linux build
echo Running Linux Build for: %PROJECT_PATH%
echo "%PROGRAMFILES%\Unity\Editor\Unity.exe" %BATCH% %QUIT% %PROJECT% %PROJECT_PATH% %LINUX%
"%ProgramFiles(x86)%\Unity\Editor\Unity.exe" %BATCH% %QUIT% %PROJECT% %PROJECT_PATH% %LINUX%
@REM Linux 64-bit build
echo Running Linux Build for: %PROJECT_PATH%
echo "%PROGRAMFILES%\Unity\Editor\Unity.exe" %BATCH% %QUIT% %PROJECT% %PROJECT_PATH% %LINUX64%
"%ProgramFiles(x86)%\Unity\Editor\Unity.exe" %BATCH% %QUIT% %PROJECT% %PROJECT_PATH% %LINUX64%
PAUSE