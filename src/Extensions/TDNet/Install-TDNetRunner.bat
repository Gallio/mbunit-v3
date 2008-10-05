@echo off & if not "%ECHO%"=="" echo %ECHO%
REM Installs a reference to the Gallio test runner for TestDriven.Net for local debugging.

setlocal
set LOCALDIR=%~dp0
set SRCDIR=%LOCALDIR%..\..\
set ROOTDIR=%SRCDIR%..\
set BINDIR=%ROOTDIR%bin\

set REG=%BINDIR%reg.exe
set ICARUS_EXE=%SRCDIR%\Extensions\Icarus\bin\Gallio.Icarus.exe

REM Point at copy in Tests so that the runner can run its own tests.
REM Otherwise it gets confused by the presence of two runner dlls, one
REM loaded using LoadFrom and the other accessible in the AppDomain.
set TDNETRUNNER_DLL=%LOCALDIR%\Gallio.TDNetRunner\bin\Gallio.TDNetRunner.dll
set TDKEY=HKLM\Software\MutantDesign\TestDriven.Net\TestRunners

call "%SRCDIR%\RegisterAssembliesForDebugging.bat"

echo Installing the locally compiled Gallio test runner for TestDriven.Net.
echo.

"%REG%" ADD %TDKEY%\Gallio_Icarus /VE /D 10 /F >nul
"%REG%" ADD %TDKEY%\Gallio_Icarus /V Application /D "%ICARUS_EXE%" /F >nul

call :AddRunner CSUnit csUnit
call :AddRunner MbUnit MbUnit
call :AddRunner MbUnit2 MbUnit.Framework
call :AddRunner MSTest Microsoft.VisualStudio.QualityTools.UnitTestFramework
call :AddRunner NUnit nunit.framework
call :AddRunner Xunit xunit
exit /b 0

:AddRunner
set NAME=%~1
set FRAMEWORK=%~2
set KEY=%TDKEY%\Gallio_%NAME%

set PRIORITY=20
:RETRY_PROMPT
set /P Answer=Use Gallio TestDriven.Net runner for %NAME%?  (Y/N)
if /I "%Answer%"=="Y" set PRIORITY=1 & goto :DONE_PROMPT
if /I not "%Answer%"=="N" goto :RETRY_PROMPT
:DONE_PROMPT

"%REG%" ADD %KEY% /VE /D %PRIORITY% /F >nul
"%REG%" ADD %KEY% /V AssemblyPath /D %TDNETRUNNER_DLL% /F >nul
"%REG%" ADD %KEY% /V TargetFrameworkAssemblyName /D %FRAMEWORK% /F >nul
"%REG%" ADD %KEY% /V TypeName /D Gallio.TDNetRunner.GallioTestRunner /F >nul
"%REG%" ADD %KEY% /V ResidentTypeName /D Gallio.TDNetRunner.GallioResidentTestRunner /F >nul
goto :EOF

