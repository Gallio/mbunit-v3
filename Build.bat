@if defined ECHO (echo %ECHO%) else (echo off)
REM
REM Builds everything and drops it into the Build folder.
REM

setlocal

set ROOT_DIR=%~dp0
set MSBUILD_ARGS=%*

if not defined MSBUILD_ARGS (
  echo Must specify a module to build such as "packages\Bundle Package.module".
  exit /b 1
)

if not defined MSBUILD set MSBUILD=%SystemRoot%\Microsoft.Net\Framework\v4.0.30128\MSBuild.exe
if not defined MSBUILD set MSBUILD=%SystemRoot%\Microsoft.Net\Framework\v3.5\MSBuild.exe
if not defined MSBUILD (
  echo Could not find path to MSBuild.exe.
  exit /b 1
)

call :SANITIZE ROOT_DIR

"%MSBUILD%" /nologo /clp:NoSummary /p:"RootDir=%ROOT_DIR%" %MSBUILD_ARGS%

if errorlevel 1 (
  echo Failed!
  exit /b 1
)

exit /b 0

REM Sanitizes a path by converting it to a full path without a trailing backslash.
REM We do this because the .Net command parser handles sequences like '\"' by taking the
REM quote literally.  This causes problems when specifying MSBuild property values
REM unless we remove or double-up the trailing backslash.
:SANITIZE
if not defined %~1 exit /b 0
for /F "tokens=*" %%V in ('echo %%%~1%%') do set SANITIZE_TEMP=%%~dpnxV
if "%SANITIZE_TEMP:~-1%"=="\" set SANITIZE_TEMP=%SANITIZE_TEMP:~0,-1%
set %~1=%SANITIZE_TEMP%
set SANITIZE_TEMP=
exit /b 0
