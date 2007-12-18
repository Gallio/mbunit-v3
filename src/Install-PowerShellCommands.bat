@echo off
REM Installs the PowerShell command snap-in.

setlocal
set SRCDIR=%~dp0
set PSCOMMANDS=%SRCDIR%\Gallio\Runners\Gallio.PowerShellCommands\bin\Gallio.PowerShellCommands.dll

echo Installing the PowerShell command snap-in.
echo.
%SYSTEMROOT%\Microsoft.Net\Framework\v2.0.50727\installutil %PSCOMMANDS% /LogFile= >nul
if errorlevel 1 (
	echo Failed!
	exit /b 1
)

echo To use the commands in PowerShell you must first add the snap-in:
echo   ^> Add-PSSnapIn Gallio
echo   ^> Run-Gallio MyTests.dll
echo.

