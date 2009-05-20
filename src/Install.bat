@if not defined ECHO (echo off) else (echo %ECHO%)
REM
REM This script installs or uninstalls components.
REM

setlocal
set SRC_DIR=%~dp0
set BIN_DIR=%~dp0..\bin
set GACUTIL=%BIN_DIR%\gacutil.exe
set GACUTIL40=%BIN_DIR%\gacutil40.exe
set REG=%BIN_DIR%\reg.exe

echo.
echo This script installs or uninstalls components built in the source tree
echo for local debugging purposes.
echo.
echo Choose the component to install or uninstall:
echo   1^) Install Gallio.Loader into GAC.
echo   2^) Install TestDriven.Net runner registry keys.  ^(implies 1^)
echo   3^) Install Visual Studio 2008 addin.  ^(implies 1^)
echo   4^) Install Visual Studio 2010 addin.  ^(implies 1^)
echo.
echo   0^) Uninstall all components.
echo.

:PROMPT
set ANSWER=%~1
if not defined ANSWER set /P ANSWER=Choice? 
echo.

if "%ANSWER%"=="1" call :INSTALL_LOADER & goto :OK
if "%ANSWER%"=="2" call :INSTALL_TDNETRUNNER & goto :OK
if "%ANSWER%"=="3" call :INSTALL_VISUALSTUDIO_ADDIN 9 & goto :OK
if "%ANSWER%"=="4" call :INSTALL_VISUALSTUDIO_ADDIN 10 & goto :OK

if "%ANSWER%"=="0" call :UNINSTALL_ALL & goto :OK
goto :PROMPT

:OK
exit /b 0

REM Install Gallio.Loader into the GAC.
:INSTALL_LOADER
echo ** Install Gallio.Loader **
call :SET_LOADER_VARS

echo Installing Gallio.Loader assembly into GAC.
"%GACUTIL%" /i "%LOADER_DLL%" /f
echo.
exit /b 0


REM Uninstalls Gallio.Loader from the GAC.
:UNINSTALL_LOADER
echo ** Uninstall Gallio.Loader **
call :SET_LOADER_VARS

echo Uninstalling Gallio.Loader assembly from GAC.
"%GACUTIL%" /u "%LOADER_DLL%" 2>nul >nul
echo.
exit /b 0


REM Helper: Set Loader vars
:SET_LOADER_VARS
set LOADER_DLL=%SRC_DIR%\Gallio\Gallio.Loader\bin\Gallio.Loader.dll
exit /b 0


REM Install TestDriven.Net runner.
:INSTALL_TDNETRUNNER
call :INSTALL_LOADER

echo ** Install TDNet Runner **
call "%SRC_DIR%Gallio.Utility.bat" Setup /install /v:verbose
echo.
exit /b 0


REM Uninstall TestDriven.Net runner.
:UNINSTALL_TDNETRUNNER
echo ** Uninstall TDNet Runner **
call "%SRC_DIR%Gallio.Utility.bat" Setup /uninstall /v:verbose
echo.
exit /b 0


REM Install Visual Studio addin.
:INSTALL_VISUALSTUDIO_ADDIN
call :INSTALL_LOADER

echo ** Install Visual Studio v%~1.0 Addin **
call :SET_VISUALSTUDIO_ADDIN_VARS "%~1"
if errorlevel 1 exit /b 1

echo Adding registry keys.

REM Register Shell
"%REG%" ADD "%VS_PRODUCT_KEY%" /V Package /D "{9e600ffc-344d-4e6f-89c0-ded6afb42459}" /F >nul
"%REG%" ADD "%VS_PRODUCT_KEY%" /V UseInterface /T REG_DWORD /D "1" /F >nul

"%REG%" ADD %VS_PACKAGE_KEY% /VE /D "Gallio Shell Package" /F >nul
"%REG%" ADD %VS_PACKAGE_KEY% /V InprocServer32 /D "%SystemRoot%\system32\mscoree.dll" /F >nul
"%REG%" ADD %VS_PACKAGE_KEY% /V Class /D "Gallio.VisualStudio.Shell.ShellPackage" /F >nul
"%REG%" ADD %VS_PACKAGE_KEY% /V CodeBase /D "%SHELL_BIN_DIR%\Gallio.VisualStudio.Shell.dll" /F >nul
"%REG%" ADD %VS_PACKAGE_KEY% /V ID /T REG_DWORD /D 1 /F >nul
"%REG%" ADD %VS_PACKAGE_KEY% /V MinEdition /D "Standard" /F >nul
"%REG%" ADD %VS_PACKAGE_KEY% /V ProductVersion /D "3.0" /F >nul
"%REG%" ADD %VS_PACKAGE_KEY% /V ProductName /D "Gallio" /F >nul
"%REG%" ADD %VS_PACKAGE_KEY% /V CompanyName /D "Gallio Project" /F >nul

"%REG%" ADD "%VS_ROOT_KEY%\AutoLoadPackages\{f1536ef8-92ec-443c-9ed7-fdadf150da82}" /V "{9e600ffc-344d-4e6f-89c0-ded6afb42459}" /T REG_DWORD /D "0" /F >nul

REM Register AddIn
"%REG%" ADD %VS_ROOT_KEY%\AutomationOptions\LookInFolders /V "%SHELL_BIN_DIR%" /D "Gallio" /F >nul

REM Register TIP
"%REG%" ADD %VS_TEST_TYPE_KEY% /V NameId /D "#100" /F >nul
"%REG%" ADD %VS_TEST_TYPE_KEY% /V TipProvider /D "Gallio.VisualStudio.Tip.GallioTipProxy, Gallio.VisualStudio.Tip%VS_VERSION%0.Proxy, Version=0.0.0.0, Culture=neutral, PublicKeyToken=eb9cfa67ee6ab36e" /F >nul
"%REG%" ADD %VS_TEST_TYPE_KEY% /V ServiceType /D "Gallio.VisualStudio.Tip.SGallioTestService, Gallio.VisualStudio.Tip%VS_VERSION%0.Proxy, Version=0.0.0.0, Culture=neutral, PublicKeyToken=eb9cfa67ee6ab36e" /F >nul

"%REG%" ADD %VS_TEST_TYPE_KEY%\Extensions /V .dll /T REG_DWORD /D "101" /F >nul
"%REG%" ADD %VS_TEST_TYPE_KEY%\Extensions /V .exe /T REG_DWORD /D "101" /F >nul

echo Installing Gallio.VisualStudio.Tip.Proxy proxy assembly into GAC.
if %VS_VERSION%==9 ("%GACUTIL%" /i "%PROXY_DLL%" /f) else ("%GACUTIL40%" /i "%PROXY_DLL%" /f)

call :RUN_VISUALSTUDIO_ADDIN_SETUP
echo.
exit /b 0


REM Uninstall Visual Studio addin.
:UNINSTALL_VISUALSTUDIO_ADDIN
echo ** Uninstall Visual Studio v%~1.0 Addin **
call :SET_VISUALSTUDIO_ADDIN_VARS "%~1"
if errorlevel 1 exit /b 1

echo Deleting registry keys.
"%REG%" DELETE "%VS_TEST_TYPE_KEY%" /F 2>nul >nul
"%REG%" DELETE "%VS_PRODUCT_KEY%" /F 2>nul >nul
"%REG%" DELETE "%VS_PACKAGE_KEY%" /F 2>nul >nul
"%REG%" DELETE "%VS_ROOT_KEY%\AutoLoadPackages\{f1536ef8-92ec-443c-9ed7-fdadf150da82}" /V "{9e600ffc-344d-4e6f-89c0-ded6afb42459}" /F 2>nul >nul
"%REG%" DELETE "%VS_ROOT_KEY%\AutomationOptions\LookInFolders" /V "%SHELL_BIN_DIR%" /F 2>nul >nul

echo Uninstalling Gallio.VisualStudio.Tip.Proxy proxy assembly from GAC.
if %VS_VERSION%==9 ("%GACUTIL%" /u "%PROXY_DLL%" 2>nul >nul) else ("%GACUTIL40%" /u "%PROXY_DLL%" 2>nul >nul)

call :RUN_VISUALSTUDIO_ADDIN_SETUP
echo.
exit /b 0

REM Helper: Run DEVENV.exe / setup
:RUN_VISUALSTUDIO_ADDIN_SETUP
echo Running devenv.exe /setup.
"%VS_INSTALL_DIR%\devenv.exe" /setup /nosetupvstemplates
exit /b 0


REM Helper: Set Visual Studio addin variables.
:SET_VISUALSTUDIO_ADDIN_VARS
set VS_VERSION=%~1
set VS_ROOT_KEY=HKLM\Software\Microsoft\VisualStudio\%VS_VERSION%.0
set VS_INSTALL_DIR=
for /F "tokens=1,2*" %%V in ('"%REG%" query %VS_ROOT_KEY% /V InstallDir') do (
    if "%%V"=="InstallDir" set VS_INSTALL_DIR=%%X
)
if not exist "%VS_INSTALL_DIR%" (
    echo Visual Studio version %VS_VERSION% was not found!
    exit /b 1
)

set VS_PRIVATE_ASSEMBLIES_DIR=%VS_INSTALL_DIR%\PrivateAssemblies

set VS_TEST_TYPE_KEY=%VS_ROOT_KEY%\EnterpriseTools\QualityTools\TestTypes\{F3589083-259C-4054-87F7-75CDAD4B08E5}
set VS_PRODUCT_KEY=%VS_ROOT_KEY%\InstalledProducts\Gallio
set VS_PACKAGE_KEY=%VS_ROOT_KEY%\Packages\{9e600ffc-344d-4e6f-89c0-ded6afb42459}

set SHELL_BIN_DIR=%SRC_DIR%\Extensions\VisualStudio\Gallio.VisualStudio.Shell\bin
set PROXY_DLL=%SRC_DIR%\Extensions\VisualStudio\Gallio.VisualStudio.Tip.Proxy\bin\v%VS_VERSION%.0\Gallio.VisualStudio.Tip%VS_VERSION%0.Proxy.dll
exit /b 0


REM Uninstall all.
:UNINSTALL_ALL
call :UNINSTALL_LOADER
call :UNINSTALL_TDNETRUNNER
call :UNINSTALL_VISUALSTUDIO_ADDIN 9
call :UNINSTALL_VISUALSTUDIO_ADDIN 10
exit /b 0
