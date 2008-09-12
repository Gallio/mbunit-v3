@echo off
REM
REM This script registers certain Gallio assemblies for debugging purposes.
REM It's a good idea to run this script before attempting to debug projects
REM like the Visual Studio integration that depend on the presence of the
REM Gallio.Loader in the GAC.
REM

setlocal
set SRC_DIR=%~dp0
set BIN_DIR=%~dp0\..\bin
set GACUTIL=%BIN_DIR%\gacutil.exe
set REG=%BIN_DIR%\reg.exe

set RUNTIME_PATH=%SRC_DIR%\Gallio\Gallio\bin
set GALLIO_LOADER_DLL=%SRC_DIR%\Gallio\Gallio.Loader\bin\Gallio.Loader.dll
set GALLIO_VISUALSTUDIO_TIP_PROXY_DLL=%SRC_DIR%\Extensions\VisualStudio\Gallio.VisualStudio.Tip.Proxy\bin\Gallio.VisualStudio.Tip.Proxy.dll

if exist "%GALLIO_LOADER_DLL%" %GACUTIL% /i "%GALLIO_LOADER_DLL%" /f
if exist "%GALLIO_VISUALSTUDIO_TIP_PROXY_DLL%" %GACUTIL% /i "%GALLIO_VISUALSTUDIO_TIP_PROXY_DLL%" /f

"%REG%" ADD HKLM\Software\Gallio.org\Gallio\3.0 /F /V DevelopmentRuntimePath /D "%RUNTIME_PATH%
