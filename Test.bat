@echo off
REM Copies files that were previously built (either by the build scrits
REM or by Visual Studio) to the build target folder then runs the tests.
%~dp0\Build.bat /target:RecursiveAfterBuild;CopyFiles;Test %*
