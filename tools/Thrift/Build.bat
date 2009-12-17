@if defined ECHO (echo %ECHO%) else (echo off)
REM
REM Builds Thrift.
REM Requires Cygwin to be installed.
REM

if not defined CYGWIN (
    echo This script required Cygwin in order to run.
	echo Please install Cygwin then set the CYGWIN environment variable
	echo to point to the path of the Cygwin installation directory.
	echo eg. set CYGWIN=c:\cygwin
	echo.
	echo Be sure to have the following packages installed:
	echo   - autoconf
	echo   - automake
	echo   - bison
	echo   - boost
	echo   - boost-devel
	echo   - flex
	echo   - gcc-core
	echo   - gcc-g++
	echo   - gcc-mingw-core
	echo   - gcc-mingw-g++
	echo   - make
	echo.
	exit /b 1
)

"%CYGWIN%\bin\bash" --login "%~dp0Build.sh"

if errorlevel 1 (
    echo Failed!
	exit /b 1
)

exit /b 0
