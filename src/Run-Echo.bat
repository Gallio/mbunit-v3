@echo off
REM Runs Gallio.Echo from the Visual Studio source folders with references
REM to the various plugins.
REM Can be used to test Echo with Mono if the MONO variable is set to
REM point to the Mono VM (mono.exe).
%MONO% %~dp0\Runners\Gallio.Echo\bin\Gallio.Echo.exe /pd:%~dp0\Gallio\Gallio /pd:%~dp0\MbUnit\MbUnit /pd:%~dp0\Plugins\Gallio.Plugin.Reports /pd:%~dp0\Plugins\Gallio.Plugin.MbUnit2Adapter /pd:%~dp0\Plugins\Gallio.Plugin.NUnitAdapter /pd:%~dp0\Plugins\Gallio.Plugin.XunitAdapter %*
