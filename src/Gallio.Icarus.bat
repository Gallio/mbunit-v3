@echo off
REM Runs Gallio.Icarus from within the source tree.
REM Can be run with Mono if the MONO variable is set to the Mono VM path (mono.exe).
%MONO% "%~dp0Extensions\Icarus\Gallio.Icarus\bin\Gallio.Icarus.exe" %*
