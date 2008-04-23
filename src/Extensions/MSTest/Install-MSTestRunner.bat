@echo off & if not "%ECHO%"=="" echo %ECHO%
REM Installs a reference to the Gallio test runner for MSTest for local debugging.

setlocal
set LOCALDIR=%~dp0
set SRCDIR=%LOCALDIR%..\..\
set ROOTDIR=%SRCDIR%..\
set LIBSDIR=%ROOTDIR%libs\

set REG=%LIBSDIR%Tools\reg.exe

set MSTESTRUNNER_BIN_DIR=%LOCALDIR%Gallio.MSTestRunner\bin

echo Installing the locally compiled MSTest extensions for running Gallio tests.
echo.

call :SETVARS "9.0"
if not defined VS_INSTALL_DIR goto :VS90_DONE_PROMPT
:VS90_RETRY_PROMPT
set /P Answer=Support MSTest for VS 2008?  (Y/N)
if /I "%Answer%"=="Y" call :INSTALL & goto :VS90_DONE_PROMPT
if /I not "%Answer%"=="N" goto :VS90_RETRY_PROMPT
call :UNINSTALL
:VS90_DONE_PROMPT

exit /b 0


:INSTALL
call :UNINSTALL

copy "%MSTESTRUNNER_BIN_DIR%\Gallio*.dll" "%VS_PRIVATE_ASSEMBLIES_DIR%" /Y >nul
copy "%MSTESTRUNNER_BIN_DIR%\Castle*.dll" "%VS_PRIVATE_ASSEMBLIES_DIR%" /Y >nul
call :PATCH_CONFIG "%SRCDIR%\Gallio\Gallio\bin" "%MSTESTRUNNER_BIN_DIR%\Gallio.MSTestRunner.dll.config" "%VS_PRIVATE_ASSEMBLIES_DIR%\Gallio.MSTestRunner.dll.config"

"%REG%" ADD %VS_TEST_TYPE_KEY% /V NameId /D "#100" /F >nul
"%REG%" ADD %VS_TEST_TYPE_KEY% /V SatelliteBasePath /D "%VS_PRIVATE_ASSEMBLIES_DIR%" /F >nul
"%REG%" ADD %VS_TEST_TYPE_KEY% /V SatelliteDllName /D "Gallio.MSTestRunner.dll" /F >nul
"%REG%" ADD %VS_TEST_TYPE_KEY% /V TipProvider /D "Gallio.MSTestRunner.GallioTip, Gallio.MSTestRunner" /F >nul
"%REG%" ADD %VS_TEST_TYPE_KEY% /V ServiceType /D "Gallio.MSTestRunner.SGallioTestService, Gallio.MSTestRunner" /F >nul
REM "%REG%" ADD %VS_TEST_TYPE_KEY% /V VsEditor /D "{00000000-0000-0000-0000-000000000000}" /F >nul

"%REG%" ADD %VS_TEST_TYPE_KEY%\Extensions /V .dll /T REG_DWORD /D "101" /F >nul
"%REG%" ADD %VS_TEST_TYPE_KEY%\Extensions /V .exe /T REG_DWORD /D "101" /F >nul
"%REG%" ADD %VS_TEST_TYPE_KEY%\Extensions /V .gallio /T REG_DWORD /D "101" /F >nul

"%REG%" ADD "%VS_PRODUCT_KEY%" /V Package /D "{9e600ffc-344d-4e6f-89c0-ded6afb42459}" /F >nul
"%REG%" ADD "%VS_PRODUCT_KEY%" /V UseInterface /T REG_DWORD /D "1" /F >nul

"%REG%" ADD %VS_PACKAGE_KEY% /VE /D "Gallio.MSTestRunner.GallioPackage, Gallio.MSTestRunner" /F >nul
"%REG%" ADD %VS_PACKAGE_KEY% /V InprocServer32 /D "%SystemRoot%\system32\mscoree.dll" /F >nul
"%REG%" ADD %VS_PACKAGE_KEY% /V Class /D "Gallio.MSTestRunner.GallioPackage" /F >nul
"%REG%" ADD %VS_PACKAGE_KEY% /V Assembly /D "Gallio.MSTestRunner" /F >nul
"%REG%" ADD %VS_PACKAGE_KEY% /V ID /T REG_DWORD /D 1 /F >nul
"%REG%" ADD %VS_PACKAGE_KEY% /V MinEdition /D "Standard" /F >nul
"%REG%" ADD %VS_PACKAGE_KEY% /V ProductVersion /D "3.0" /F >nul
"%REG%" ADD %VS_PACKAGE_KEY% /V ProductName /D "Gallio.MSTestRunner" /F >nul
"%REG%" ADD %VS_PACKAGE_KEY% /V CompanyName /D "Gallio Project" /F >nul

"%VS_INSTALL_DIR%\devenv.exe" /setup /nosetupvstemplates
goto :EOF


:UNINSTALL
del "%VS_PRIVATE_ASSEMBLIES_DIR%\Gallio*.*" 2>nul
del "%VS_PRIVATE_ASSEMBLIES_DIR%\Castle*.*" 2>nul

"%REG%" DELETE "%VS_TEST_TYPE_KEY%" /F 2>nul >nul
"%REG%" DELETE "%VS_PRODUCT_KEY%" /F 2>nul >nul
"%REG%" DELETE "%VS_PACKAGE_KEY%" /F 2>nul >nul
goto :EOF


:SETVARS
set VS_VERSION=%~1

set VS_ROOT_KEY=HKLM\Software\Microsoft\VisualStudio\%VS_VERSION%
set VS_INSTALL_DIR=
for /F "tokens=1,2*" %%V in ('"%REG%" query %VS_ROOT_KEY% /V InstallDir') do (
    if "%%V"=="InstallDir" set VS_INSTALL_DIR=%%X
)

if not exist "%VS_INSTALL_DIR%" (
    echo Visual Studio version %VS_VERSION% was not found.  Skipped.
    set VS_INSTALL_DIR=
    goto :EOF
)

set VS_PRIVATE_ASSEMBLIES_DIR=%VS_INSTALL_DIR%\PrivateAssemblies
set VS_TEST_TYPE_KEY=%VS_ROOT_KEY%\EnterpriseTools\QualityTools\TestTypes\{F3589083-259C-4054-87F7-75CDAD4B08E5}
set VS_PRODUCT_KEY=%VS_ROOT_KEY%\InstalledProducts\Gallio.MSTestRunner
set VS_PACKAGE_KEY=%VS_ROOT_KEY%\Packages\{9e600ffc-344d-4e6f-89c0-ded6afb42459}
goto :EOF

:PATCH_CONFIG
set INSTALLATION_PATH=%~dpnx1
set SOURCE_FILE=%~dpnx2
set DEST_FILE=%~dpnx3

for /F "tokens=*" %%V in ('echo %INSTALLATION_PATH%^| sed s/\\/\\\\\\\\/g') do set ESCAPED_INSTALLATION_PATH=%%V
sed "s/<!--PLACEHOLDER-->/%ESCAPED_INSTALLATION_PATH%/" < "%SOURCE_FILE%" > "%DEST_FILE%"

goto :EOF

