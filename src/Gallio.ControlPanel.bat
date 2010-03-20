@echo off
REM Runs Gallio.ControlPanel from within the source tree.
REM Can be run with Mono if the MONO variable is set to the Mono VM path (mono.exe).
%MONO% "%~dp0Extensions\ControlPanel\Gallio.ControlPanel\bin\Gallio.ControlPanel.exe" "/pd:%~dp0." %*
