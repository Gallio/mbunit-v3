@if defined ECHO (echo %ECHO%) else (echo off)
REM
REM Builds everything and drops it into the Build folder.
REM You may specify additional arguments for MetaBuild also.
REM
"%~dp0tools\MetaBuild\bin\MetaBuild.bat" /rootdir "%~dp0" /metabuildconfig "%~dp0bin\MetaBuild.config.custom" %*
