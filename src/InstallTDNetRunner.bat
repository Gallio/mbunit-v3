@echo off & if not "%ECHO%"=="" echo %ECHO%
REM Installs the Gallio test runner for TestDriven.Net from the source tree.
call "%~dp0RegisterAssembliesForDebugging.bat"
call "%~dp0Gallio.Utility Setup /install"
