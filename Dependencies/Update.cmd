@echo off
setlocal

echo * Preparing temporary directory.
set TempPath=%TEMP%\CppWinRT
if exist "%TempPath%" rmdir "%TempPath%" /s /q
if %errorlevel% neq 0 goto error

echo * Downloading latest C++/WinRT source from GitHub.
git clone https://github.com/Microsoft/cppwinrt "%TempPath%"
if %errorlevel% neq 0 goto error

echo * Copying current SDK headers to dependencies.
robocopy "%TempPath%\10.0.15063.0\winrt" "%~dp0CppWinRT\winrt" /s /purge
if %errorlevel% gtr 7 goto error

echo * Clean-up.
rmdir "%TempPath%" /s /q

echo * Successful.
endlocal
exit /b 0

:error
echo Error %errorlevel%
endlocal
exit /b 1

