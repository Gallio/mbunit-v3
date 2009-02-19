' Runs a program in the background.
command = Replace(WScript.Arguments(0), "'", """")
WScript.CreateObject("WScript.Shell").Run command, 7, false
