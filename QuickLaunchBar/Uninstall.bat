@echo off

"%~dp0tools\RegAsm.exe" /nologo /unregister "%~dp0QuickLaunchBar.dll"

taskkill /f /im "explorer.exe"
start explorer.exe

:: explorer "D:\Program\Visual C#\C#_WIN_DeskTop\DeskBandTest\Test"

pause
