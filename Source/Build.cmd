@echo off
setlocal
echo Build
echo =======
echo.
echo Performs a build of one configuration then copies the output
echo to a solution local build directory, ready for release.
echo.

rem * Check syntax
if "%~1" == "" (
	echo Configuration name parameter is required.
	endlocal
	exit /b 1
)
set ConfigurationName=%~1

echo.
echo %ConfigurationName% Build...

echo.
echo Initializing Visual Studio environment...
call "%~dp0Dependencies\Variables.cmd"
if %errorlevel% neq 0 goto Error

echo.
echo Delete old files...
if exist "%~dp0Temp\Build\%ConfigurationName%" (
	rmdir "%~dp0Temp\Build\%ConfigurationName%" /s /q
	if %errorlevel% gtr 1 goto Error
)

echo.
echo Compiling %ConfigurationName% build...
msbuild "%~dp0Navio SDK for Windows IoT.sln" /p:Configuration=%ConfigurationName%
if %errorlevel% neq 0 goto Error

echo.
echo Copying components...
robocopy "%~dp0Framework\Emlid.WindowsIot.Hardware\bin\ARM\%ConfigurationName%" "%~dp0Temp\Build\%ConfigurationName%\Components" *.winmd *.dll *.pdb *.xml /xf *CodeAnalysisLog.xml
if %errorlevel% gtr 7 goto Error

echo.
echo Copying Windows packages...
robocopy "%~dp0Samples\CPP\Navio RC Input\AppPackages" "%~dp0Temp\Build\%ConfigurationName%\Packages\AppX" /s
if %errorlevel% gtr 7 goto Error
robocopy "%~dp0Samples\CS\Navio Barometer\AppPackages" "%~dp0Temp\Build\%ConfigurationName%\Packages\AppX" /s
if %errorlevel% gtr 7 goto Error
robocopy "%~dp0Samples\CS\Navio FRAM\AppPackages" "%~dp0Temp\Build\%ConfigurationName%\Packages\AppX" /s
if %errorlevel% gtr 7 goto Error
robocopy "%~dp0Samples\CS\Navio LED\AppPackages" "%~dp0Temp\Build\%ConfigurationName%\Packages\AppX" /s
if %errorlevel% gtr 7 goto Error
robocopy "%~dp0Samples\CS\Navio RC Input\AppPackages" "%~dp0Temp\Build\%ConfigurationName%\Packages\AppX" /s
if %errorlevel% gtr 7 goto Error
robocopy "%~dp0Tools\Navio 2 RCIO Terminal\AppPackages" "%~dp0Temp\Build\%ConfigurationName%\Packages\AppX" /s
if %errorlevel% gtr 7 goto Error
robocopy "%~dp0Tools\Navio Hardware Test\AppPackages" "%~dp0Temp\Build\%ConfigurationName%\Packages\AppX" /s
if %errorlevel% gtr 7 goto Error

echo.
echo Copying NuGet packages...
robocopy "%~dp0Framework\Emlid.WindowsIot.Hardware\bin\ARM\%ConfigurationName%\NuGet" "%~dp0Temp\Build\%ConfigurationName%\Packages\NuGet" /s
if %errorlevel% gtr 7 goto Error

echo.
echo Copying documentation...
robocopy "%~dp0Documentation" "%~dp0Temp\Build\%ConfigurationName%\Documentation" /s
if %errorlevel% gtr 7 goto Error
copy "%~dp0Dependencies\Code for .NET\Documentation\Release Notes.md" "%~dp0Temp\Build\%ConfigurationName%\Documentation\Dependencies\Code for .NET Release Notes.md"
if %errorlevel% neq 0 goto Error
copy "%~dp0Dependencies\Code for PowerShell\Documentation\Release Notes.md" "%~dp0Temp\Build\%ConfigurationName%\Documentation\Dependencies\Code for PowerShell Release Notes.md"
if %errorlevel% neq 0 goto Error
copy "%~dp0Dependencies\Code for Windows\Documentation\Release Notes.md" "%~dp0Temp\Build\%ConfigurationName%\Documentation\Dependencies\Code for Windows Release Notes.md"
if %errorlevel% neq 0 goto Error

echo.
echo Copying version references...
md "%~dp0Temp\Build\%ConfigurationName%\Version"
if %errorlevel% neq 0 goto Error
copy "%~dp0Version.txt" "%~dp0Temp\Build\%ConfigurationName%\Version\Emlid.WindowsIot.Version.txt"
if %errorlevel% neq 0 goto Error
robocopy "%~dp0Dependencies\Code for .NET\Version" "%~dp0Temp\Build\%ConfigurationName%\Version" *Version.txt /xn
if %errorlevel% gtr 7 goto Error
robocopy "%~dp0Dependencies\Code for PowerShell\Version" "%~dp0Temp\Build\%ConfigurationName%\Version" *Version.txt /xn
if %errorlevel% gtr 7 goto Error
robocopy "%~dp0Dependencies\Code for Windows\Version" "%~dp0Temp\Build\%ConfigurationName%\Version" *Version.txt /xn
if %errorlevel% gtr 7 goto Error

echo.
echo %ConfigurationName% build successful.
endlocal
exit /b 0

:Error
echo Error %errorlevel%!
endlocal
exit /b 1