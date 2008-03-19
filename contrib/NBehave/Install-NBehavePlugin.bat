@echo off
REM Installs a reference to the Gallio test runner for TestDriven.Net for local debugging.

setlocal
set LOCALDIR=%~dp0
set ROOTDIR=%LOCALDIR%..\..\
set LIBSDIR=%ROOTDIR%libs\

set NBEHAVEPLUGINDIR=%LOCALDIR%NBehave

set GALLIOKEY=HKLM\Software\Gallio
set GALLIOBINDIR=%ROOTDIR%build\target\bin\

set REG=%LIBSDIR%Tools\reg.exe
set ICARUS_EXE=%GALLIOBINDIR%Gallio.Icarus.exe

set TDNETRUNNER_DLL=%GALLIOBINDIR%Gallio.TDNetRunner.dll
set TDKEY=HKLM\Software\MutantDesign\TestDriven.Net\TestRunners

echo Registering the NBehave plugin directory.
echo.
%REG% ADD %GALLIOKEY%\AdditionalPluginDirectories /V NBehave /D %NBEHAVEPLUGINDIR% /F >nul

echo Installing the locally compiled Gallio test runner for TestDriven.Net.
echo.

call :AddRunner NBehave NBehave
exit /b 0

:AddRunner
set NAME=%~1
set FRAMEWORK=%~2
set KEY=%TDKEY%\Gallio_%NAME%

set PRIORITY=20
:RETRY_PROMPT
set /P Answer=Use Gallio TestDriven.Net runner for %NAME%?  (Y/N)
if /I "%Answer%"=="Y" set PRIORITY=5 & goto :DONE_PROMPT
if /I not "%Answer%"=="N" goto :RETRY_PROMPT
:DONE_PROMPT

%REG% ADD %KEY% /VE /D %PRIORITY% /F >nul
%REG% ADD %KEY% /V Application /D %ICARUS_EXE% /F >nul
%REG% ADD %KEY% /V AssemblyPath /D %TDNETRUNNER_DLL% /F >nul
%REG% ADD %KEY% /V TargetFrameworkAssemblyName /D %FRAMEWORK% /F >nul
%REG% ADD %KEY% /V TypeName /D Gallio.TDNetRunner.GallioTestRunner /F >nul
goto :EOF

