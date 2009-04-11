@echo off
REM Runs all of the integration tests from the command-line.
REM We assume NAnt is in the PATH
cls

call :RUN PassingTests
call :RUN FailingTests
call :RUN FailingTestsWithIgnoreFailures
call :RUN NoAssemblies
call :RUN NoTests
call :RUN NoFilter
call :RUN UnhandledException
call :RUN Extensions
call :RUN Verbosity
exit /b 0

:RUN
"..\..\libs\NAnt" /f:Integration.build %1 /D:GallioPath="%~dp0..\bin"
echo Exit code: %ERRORLEVEL%
echo ============================================================
goto :EOF
