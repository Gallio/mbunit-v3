@echo off
REM Runs all of the integration tests from the command-line.
REM We assume MSBuild is in the PATH
cls

call :RUN PassingTests
call :RUN FailingTests
call :RUN FailingTestsWithIgnoreFailures
call :RUN NoAssemblies
call :RUN NoTests
call :RUN NoFilter
exit /b 0

:RUN
MSBuild Integration.proj /t:%1 /p:GallioPath="%~dp0..\bin"
echo Exit code: %ERRORLEVEL%
echo ============================================================
goto :EOF
