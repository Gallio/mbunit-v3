cls
@echo off

' We asume MSBuild is in the PATH

MSBuild NoTests.xml /p:"ExpectedMbUnitExitCode=16"
echo ============================================================

MSBuild PassingTests.xml /p:"ExpectedMbUnitExitCode=0"
echo ============================================================

MSBuild FailingTests-FailuresIgnored.xml /p:"ExpectedMbUnitExitCode=1"
echo ============================================================

MSBuild FailingTests.xml /p:"ExpectedMbUnitExitCode=1"
echo ============================================================

MSBuild NoFilter.xml /p:"ExpectedMbUnitExitCode=1"