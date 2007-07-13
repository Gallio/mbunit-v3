@echo off
REM Installs registry settings in Visual Studio to make working
REM with the MbUnit projects a little nicer.

setlocal
set ROOT=%~dp0

REM Make Visual Studio stop prompting about our custom targets.
echo Adding SafeImports registry keys to prevent Visual Studio from
echo prompting about the custom MSBuild imports in our project files.
"%ROOT%libs\Tools\reg.exe" ADD HKLM\Software\Microsoft\VisualStudio\8.0\MSBuild\SafeImports /v MbUnit.Gallio.ProjectReferences /d "%ROOT%src\MbUnit.Gallio.ProjectReferences.targets" /f
"%ROOT%libs\Tools\reg.exe" ADD HKLM\Software\Microsoft\VisualStudio\8.0\MSBuild\SafeImports /v MbUnit.Gallio.ILMerge /d "%ROOT%src\MbUnit.Gallio.ILMerge.targets" /f

endlocal
