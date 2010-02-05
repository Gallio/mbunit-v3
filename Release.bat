@echo off
REM Builds everything for release and drops it in the Build folder.
REM Includes building of documentation and other expensive activities.
%~dp0\Build.bat /clean /build /image /test /dist %*
