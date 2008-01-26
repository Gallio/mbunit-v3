@echo off
REM Runs Gallio.Echo from the Visual Studio source folders with references
REM to the various plugins.
REM Can be used to test Echo with Mono if the MONO variable is set to
REM point to the Mono VM (mono.exe).
%MONO% %~dp0\Gallio.Echo\bin\Gallio.Echo.exe /pd:%~dp0..\.. %*
