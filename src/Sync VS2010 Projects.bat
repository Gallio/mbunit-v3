@echo off

powershell set-executionpolicy unrestricted

powershell "& './Compare VS2010 Projects.ps1' -sync %*"

powershell set-executionpolicy restricted