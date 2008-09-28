@echo off & if not "%ECHO%"=="" echo %ECHO%
REM Installs the Gallio Navigator components for local debugging.

setlocal
set LOCALDIR=%~dp0
set SRCDIR=%LOCALDIR%..\..\
set ROOTDIR=%SRCDIR%..\
set BINDIR=%ROOTDIR%bin\

set ASM=%LOCALDIR%Gallio.Navigator\bin\Gallio.Navigator.exe

"%SYSTEMROOT%\Microsoft.Net\Framework\v2.0.50727\RegAsm.exe" /unregister "%ASM%"

exit /b 0

