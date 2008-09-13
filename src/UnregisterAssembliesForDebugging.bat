@echo off
REM
REM This script unregisters Gallio assemblies that were previously
REM registered for debugging.
REM

setlocal
set SRC_DIR=%~dp0
set BIN_DIR=%~dp0\..\bin
set GACUTIL=%BIN_DIR%\gacutil.exe
set REG=%BIN_DIR%\reg.exe

"%GACUTIL%" /u Gallio.Loader
"%GACUTIL%" /u Gallio.VisualStudio.Tip.Proxy

"%REG%" DELETE HKLM\Software\Gallio.org\Gallio\3.0 /F /V DevelopmentRuntimePath
