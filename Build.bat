@echo off
REM Builds everything and drops it in the Build folder.
%SystemRoot%\Microsoft.NET\Framework\v3.5\msbuild.exe "%~dp0Build.msbuild" /v:minimal %*
