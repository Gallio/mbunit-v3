cls
@echo off

REM  We asume nant is in the PATH

nant /f:NoTests.build /D:ExpectedMbUnitExitCode=16
echo ============================================================

nant /f:PassingTests.build /D:ExpectedMbUnitExitCode=0
echo ============================================================

nant /f:FailingTests-FailuresIgnored.build /D:ExpectedMbUnitExitCode=1
echo ============================================================

nant /f:FailingTests.build /D:ExpectedMbUnitExitCode=1
echo ============================================================

nant /f:NoFilter.build /D:ExpectedMbUnitExitCode=1