@echo off
rem ==========================================================================
rem Visual Studio Environment Variables
rem --------------------------------------------------------------------------
rem Initializes Visual Studio command line environment variables, to use
rem the most recent version of Visual Studio installed. Since 2017 the VSWhere
rem tool and VSDevCmd.bat replace the VS#COMNTOOLS environment variable and
rem VSVars32.bat. The location of Visual Studio is no longer fixed, due to
rem the necessity for support of parallel versions and editions.
rem --------------------------------------------------------------------------

rem Do nothing when already setup.
if "%VSCMD_VER%" neq "" goto Done

rem Use the new VSWhere when present.
if not exist "%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\VSWhere.exe" goto OldMethod

rem * Call VSWhere to locate Visual Studio.
for /f "usebackq tokens=1* delims=" %%i in (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\VSWhere.exe" -version 15.0^ -property installationPath`) do set VSInstallDir=%%~i\
if %errorlevel% neq 0 goto Error

rem Use old method when no 2017 or later installations found.
if "%VSInstallDir%" neq "" goto NewMethod

rem ---------------------------------------------------------------------------
rem Call old setup script
rem ---------------------------------------------------------------------------

:OldMethod
echo * Calling VSVars32.bat to initialize environment...
call "%VS140ComnTools%VSVars32.bat"
if %errorlevel% neq 0 goto Error

goto Done

rem ---------------------------------------------------------------------------
rem Call new setup script
rem ---------------------------------------------------------------------------

:NewMethod
echo * Calling VSDevCmd.bat to initialize environment...

rem * Fix current directory lost after running VSDevCmd.bat (fixed in latest builds of VS 2017 15.6)
set VSCMD_START_DIR=%CD%
rem * Fix DevEnvDir not always set.
set DevEnvDir=%VSInstallDir%Common7\IDE\

rem * Call VSDevCmd.bat from detected VS location
call "%VSInstallDir%Common7\Tools\VSDevCmd.bat"
if %errorlevel% neq 0 goto Error

rem ---------------------------------------------------------------------------
rem Success
rem ---------------------------------------------------------------------------

:Done
exit /b 0

rem ---------------------------------------------------------------------------
rem Error
rem ---------------------------------------------------------------------------

:Error
echo Error %errorlevel%!
exit /b 1
