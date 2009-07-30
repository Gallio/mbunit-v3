@if defined ECHO (echo %ECHO%) else (echo off)
REM
REM Copies build tools into bin folder where they will be checked in.
REM

setlocal

set BASE_DIR=%~dp0
set BIN_DIR=%~dp0..\..\bin

echo.
echo Compiling tools.
echo.

"%SYSTEMROOT%\Microsoft.NET\Framework\v3.5\msbuild.exe" "%BASE_DIR%\Gallio.BuildTools.sln" /p:Configuration=Release
if errorlevel 1 (
    echo Build failed.
    exit /b 1
)

echo.
echo Copying compiled files to bin folder.
echo.

copy "%BASE_DIR%\Gallio.BuildTools.Tasks\bin\Gallio.BuildTools.Tasks.dll" "%BIN_DIR%"
if errorlevel 1 goto :COPY_FAILED
copy "%BASE_DIR%\Gallio.BuildTools.Tasks\bin\Gallio.BuildTools.XsdGen.exe" "%BIN_DIR%"
if errorlevel 1 goto :COPY_FAILED
copy "%BASE_DIR%\Gallio.BuildTools.Tasks\bin\Gallio.BuildTools.XsdGen.exe.config" "%BIN_DIR%"
if errorlevel 1 goto :COPY_FAILED

echo Done.
echo.

exit /b 0

:COPY_FAILED
echo Copy failed.
exit /b 1

