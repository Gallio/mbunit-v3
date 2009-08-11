@if defined ECHO (echo %ECHO%) else (echo off)
REM
REM Copies build tools into bin folder where they will be checked in.
REM

setlocal

set BASE_DIR=%~dp0
set KEY_FILE=%~dp0..\..\src\Key.snk
set LIBS_DIR=%~dp0..\..\src\Gallio\libs

echo.
echo Compiling tools.
echo.

"%SYSTEMROOT%\Microsoft.NET\Framework\v3.5\msbuild.exe" "%BASE_DIR%\Gallio.ReflectionShim.Generator.sln" /p:Configuration=Release
if errorlevel 1 (
    echo Build failed.
    exit /b 1
)

echo.
echo Generating the shim in the Gallio libs folder.
echo.

"%BASE_DIR%\bin\Gallio.ReflectionShim.Generator.exe" "%KEY_FILE%" "%LIBS_DIR%"
if errorlevel 1 goto :GENERATOR_FAILED

echo Done.
echo.

exit /b 0

:GENERATOR_FAILED
echo Generator failed.
exit /b 1

