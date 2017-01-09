@echo off
rem NuGet Package Script
rem ====================
rem Creates a release build then generates the NuGet package into the output directory.
rem Remember to update the version number and release notes in the "nuspec" file before each release.

echo.
echo Initializing Visual Studio tools...
call "%vs140comntools%\vsvars32.bat"
if %errorlevel% neq 0 goto error

echo.
echo Creating Release build
msbuild "%~dp0Emlid.WindowsIot.Hardware.csproj" /p:Configuration=Release
if %errorlevel% neq 0 goto error

echo.
echo Creating NuGet Package
"%~dp0..\..\Tools\NuGet\nuget.exe" pack "%~dp0Emlid.WindowsIot.Hardware.csproj" -Prop Configuration=Release -OutputDirectory "%~dp0bin\ARM\Release"
if %errorlevel% neq 0 goto error

echo.
echo Packaging successful.
exit /b 0

:error
set result=%errorlevel%
echo.
echo Error %result%
exit /b %result%
