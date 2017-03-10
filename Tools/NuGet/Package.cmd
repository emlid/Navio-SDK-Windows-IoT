@echo off
setlocal

rem NuGet Package Script
rem ====================
rem Creates a release build then generates the NuGet package into the output directory.
rem Remember to update the version number and release notes in the "nuspec" file before each release.

rem * Initialize environment
echo Initializing Visual Studio tools...
pushd "%~dp0"
call "%VS150ComnTools%VsDevCmd.bat"
if %errorlevel% neq 0 goto error
popd

echo.
echo Creating release build...
msbuild "%~dp0..\..\Framework\Emlid.WindowsIot.Hardware\Emlid.WindowsIot.Hardware.csproj" /p:Configuration=Release /p:Platform=ARM
if %errorlevel% neq 0 goto error

echo.
echo Creating NuGet Package...
"%~dp0nuget.exe" pack "%~dp0..\..\Framework\Emlid.WindowsIot.Hardware\Emlid.WindowsIot.Hardware.csproj" -Prop Configuration=Release -OutputDirectory "%~dp0Release" -ForceEnglishOutput
if %errorlevel% neq 0 goto error

echo.
echo Packaging successful.
endlocal
exit /b 0

:error
echo Error %errorlevel%
endlocal
exit /b 1
