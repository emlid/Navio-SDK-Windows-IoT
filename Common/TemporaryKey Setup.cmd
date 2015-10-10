@echo off
call "%vs140comntools%vsvars32.bat"

echo Install Visual Studio Temporary Key
echo ===================================
echo This solution uses temporary keys for development and testing.
echo Of course any official submission to the store or production release would
echo require a complete re-sign with secret protected keys.
echo.
echo ***************************************************************************
echo *** ACTION: When prompted for a password, just hit enter (no password)! ***
echo ***************************************************************************
echo.
sn -i "%~dp0TemporaryKey.pfx" VS_KEY_3E3638EEF5FAA130
if %errorlevel% neq 0 pause

exit /b 0