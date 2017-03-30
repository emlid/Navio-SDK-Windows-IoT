@echo off
setlocal

echo NuGet Package Script
echo ====================
echo Creates a release build then generates the NuGet package into the output directory.
echo Remember to update the version number and release notes in the "nuspec" file before each release.
echo.

echo Initializing environment...
call "%~dp0..\Scripts\VSWhereDev.cmd"
if %errorlevel% neq 0 goto error

echo.
echo Creating release build...
msbuild "%~dp0..\..\Framework\Emlid.WindowsIot.Hardware\Emlid.WindowsIot.Hardware.csproj" /p:Configuration=Release /p:Platform=ARM
if %errorlevel% neq 0 goto error

echo.
echo Creating NuGet Package...
pushd "%~dp0..\..\Framework\Emlid.WindowsIot.Hardware"
"%~dp0nuget.exe" pack "Emlid.WindowsIot.Hardware.nuspec" -Properties Configuration=Release;Platform=ARM -BasePath "bin\ARM\Release" -OutputDirectory "%~dp0Release" -ForceEnglishOutput
if %errorlevel% neq 0 goto error
popd

echo.
echo Packaging successful.
endlocal
exit /b 0

:error
echo Error %errorlevel%
endlocal
exit /b 1
