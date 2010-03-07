@if defined ECHO (echo %ECHO%) else (echo off)
REM
REM Builds everything and drops it into the Build folder.
REM

setlocal

set ROOT_DIR=%~dp0
set MSBUILD_ARGS=%*

if not defined MSBUILD set MSBUILD=%SystemRoot%\Microsoft.Net\Framework\v4.0.30128\MSBuild.exe
if not defined MSBUILD set MSBUILD=%SystemRoot%\Microsoft.Net\Framework\v3.5\MSBuild.exe
if not defined MSBUILD (
  echo Could not find path to MSBuild.exe.
  exit /b 1
)

"%MSBUILD%" "%ROOT_DIR%\bin\Build.msbuild" /nologo /clp:NoSummary %MSBUILD_ARGS%

if errorlevel 1 (
  echo Failed!
  exit /b 1
)

exit /b 0
