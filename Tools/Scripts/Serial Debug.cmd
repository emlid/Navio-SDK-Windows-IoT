@echo off
rem TODO: Detect processor architecture (also from WoW64) to select correct debugger path also on 32bit machines
start "Windows Debug" "C:\Program Files (x86)\Windows Kits\10\Debuggers\x64\windbg.exe" -k com:port=3,baud=912600