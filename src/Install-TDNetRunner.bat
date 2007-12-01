@echo off
REM Installs a reference to the Gallio test runner for TestDriven.Net for local debugging.

setlocal
set SRCDIR=%~dp0
set REG=%SRCDIR%..\libs\Tools\reg.exe
set ICARUS_EXE=%SRCDIR%Gallio\Runners\Gallio.Icarus\bin\Gallio.Icarus.exe
set TDNETRUNNER_DLL=%SRCDIR%Gallio\Runners\Gallio.TDNetRunner\bin\Gallio.TDNetRunner.dll
set TDKEY=HKLM\Software\MutantDesign\TestDriven.Net\TestRunners

echo Installing the locally compiled Gallio test runner for TestDriven.Net.
echo.

set MbUnit2AddInPriority=20
:MBUNIT2_RETRY_PROMPT
set /P Answer=Use Gallio for MbUnit v2?  (Y/N)
if /I "%Answer%"=="Y" set MbUnit2AddInPriority=5 & goto :MBUNIT2_DONE_PROMPT
if /I not "%Answer%"=="N" goto :MBUNIT2_RETRY_PROMPT
:MBUNIT2_DONE_PROMPT

set NUnitAddInPriority=20
:NUNIT_RETRY_PROMPT
set /P Answer=Use Gallio for NUnit?  (Y/N)
if /I "%Answer%"=="Y" set NUnitAddInPriority=5 & goto :NUNIT_DONE_PROMPT
if /I not "%Answer%"=="N" goto :NUNIT_RETRY_PROMPT
:NUNIT_DONE_PROMPT

set XunitAddInPriority=20
:XUNIT_RETRY_PROMPT
set /P Answer=Use Gallio for Xunit?  (Y/N)
if /I "%Answer%"=="Y" set XunitAddInPriority=5 & goto :XUNIT_DONE_PROMPT
if /I not "%Answer%"=="N" goto :XUNIT_RETRY_PROMPT
:XUNIT_DONE_PROMPT

call :AddRunner Gallio_MbUnit 10 MbUnit
call :AddRunner Gallio_MbUnit2 %MbUnit2AddInPriority% MbUnit.Framework
call :AddRunner Gallio_NUnit %NUnitAddInPriority% nunit.framework
call :AddRunner Gallio_Xunit %XunitAddInPriority% xunit
exit /b 0

:AddRunner
set NAME=%~1
set PRIORITY=%~2
set FRAMEWORK=%~3

%REG% ADD %TDKEY%\%NAME% /VE /D %PRIORITY% /F >nul
%REG% ADD %TDKEY%\%NAME% /V Application /D %ICARUS_EXE% /F >nul
%REG% ADD %TDKEY%\%NAME% /V AssemblyPath /D %TDNETRUNNER_DLL% /F >nul
%REG% ADD %TDKEY%\%NAME% /V TargetFrameworkAssemblyName /D %FRAMEWORK% /F >nul
%REG% ADD %TDKEY%\%NAME% /V TypeName /D Gallio.TDNetRunner.GallioTestRunner /F >nul
goto :EOF

