@echo off
REM Uninstalls the PowerShell command snap-in.

setlocal
set LOCALDIR=%~dp0
set FRAMEWORKDIR=%SYSTEMROOT%\Microsoft.Net\Framework\v2.0.50727\
set PSCOMMANDS=%LOCALDIR%Gallio.PowerShellCommands\bin\Gallio.PowerShellCommands.dll

echo Uninstalling the PowerShell command snap-in.
echo.
%FRAMEWORKDIR%installutil /uninstall %PSCOMMANDS% /LogFile= >nul
if errorlevel 1 (
	echo Failed!
	exit /b 1
)


