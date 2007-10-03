@echo off
REM Runs MbUnit.Echo from the Visual Studio source folders with references
REM to the various plugins.
REM Can be used to test Echo with Mono if the MONO variable is set to
REM point to the Mono VM (mono.exe).
%MONO% %~dp0\Runners\MbUnit.Echo\bin\MbUnit.Echo.exe /pd:%~dp0\Plugins\MbUnit.Plugin.MbUnit2Adapter\bin /pd:%~dp0\Plugins\MbUnit.Plugin.NUnitAdapter\bin /pd:%~dp0\Plugins\MbUnit.Plugin.XunitAdapter\bin %*
