@echo off
REM Builds everything and drops it in the Build folder.
%SystemRoot%\Microsoft.NET\Framework\v2.0.50727\msbuild.exe "%~dp0Build.msbuild" /v:minimal %*
