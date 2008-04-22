@echo off
REM Installs a reference to the Gallio test runner for MSTest for local debugging.

setlocal
set LOCALDIR=%~dp0
set SRCDIR=%LOCALDIR%..\..\
set ROOTDIR=%SRCDIR%..\
set LIBSDIR=%ROOTDIR%libs\

set REG=%LIBSDIR%Tools\reg.exe

set MSTESTRUNNER_DLL=%LOCALDIR%Gallio.MSTestRunner\bin\Gallio.MSTestRunner.dll

echo Installing the locally compiled MSTest extensions for running Gallio tests.
echo.

:VS80_RETRY_PROMPT
set /P Answer=Support MSTest for VS 2005?  (Y/N)
if /I "%Answer%"=="Y" call :INSTALL "8.0" & goto :VS80_DONE_PROMPT
if /I not "%Answer%"=="N" goto :VS80_RETRY_PROMPT
:VS80_DONE_PROMPT

:VS90_RETRY_PROMPT
set /P Answer=Support MSTest for VS 2008?  (Y/N)
if /I "%Answer%"=="Y" call :INSTALL "9.0" & goto :VS90_DONE_PROMPT
if /I not "%Answer%"=="N" goto :VS90_RETRY_PROMPT
:VS90_DONE_PROMPT

exit /b 0

:INSTALL
set VS_VERSION=%~1

set VS_ROOT_KEY=HKLM\Software\Microsoft\VisualStudio\%VS_VERSION%
set VS_INSTALL_DIR=
for /F "tokens=1,2*" %%V in ('"%REG%" query %VS_ROOT_KEY% /V InstallDir') do (
    if "%%V"=="InstallDir" set VS_INSTALL_DIR=%%X
)

if not exist "%VS_INSTALL_DIR%" (
    echo Visual Studio version %VS_VERSION% was not found.  Skipped.
    goto :EOF
)

copy "%MSTESTRUNNER_DLL%" "%VS_INSTALL_DIR%\PrivateAssemblies" /Y >nul
call :PATCH_CONFIG "%SRCDIR%\Gallio\Gallio\bin" "%MSTESTRUNNER_DLL%.config" "%VS_INSTALL_DIR%\PrivateAssemblies\Gallio.MSTestRunner.dll.config"

set VS_TEST_TYPE_KEY=%VS_ROOT_KEY%\EnterpriseTools\QualityTools\TestTypes\{F3589083-259C-4054-87F7-75CDAD4B08E5}

"%REG%" ADD %VS_TEST_TYPE_KEY% /V TipProvider /D "Gallio.MSTestRunner.GallioTip, Gallio.MSTestRunner" /F >nul
goto :EOF


:PATCH_CONFIG
set INSTALLATION_PATH=%~dpnx1
set SOURCE_FILE=%~dpnx2
set DEST_FILE=%~dpnx3

for /F "tokens=*" %%V in ('echo %INSTALLATION_PATH%^| sed s/\\/\\\\\\\\/g') do set ESCAPED_INSTALLATION_PATH=%%V
sed "s/<!--PLACEHOLDER-->/%ESCAPED_INSTALLATION_PATH%/" < "%SOURCE_FILE%" > "%DEST_FILE%"

goto :EOF

