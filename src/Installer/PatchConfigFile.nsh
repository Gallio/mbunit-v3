
; Patches the installation directory in a configuration file
;   OLDNAME - the old file name
;   NEWNAME - the new file name
!define PatchConfigFile '!insertmacro PatchConfigFile'

!macro PatchConfigFile OLDNAME NEWNAME
	Push "${OLDNAME}"
	Push "${NEWNAME}"
	Call PatchConfigFile
!macroend

Function PatchConfigFile
	Exch $0 ; new name
	Exch 1
	Exch $1 ; old name
	Exch 2
	Push $2
	Push $3
	Push $4
	Push $5

	PatchRetry:
	FileOpen $3 $1 "r"
	IfErrors PatchError
	FileOpen $4 $0 "w"
	IfErrors PatchErrorCloseSource
	
	PatchLoop:
	FileRead $3 $5
	IfErrors PatchDone
	${StrReplace} $5 "<!--PLACEHOLDER-->" "$INSTDIR\bin" $5
	FileWrite $4 $5
	GoTo PatchLoop

	PatchDone:
	ClearErrors
	FileClose $4
	FileClose $3
	Delete $1
	IfErrors PatchError PatchExit

	PatchErrorCloseSource:
	FileClose $3
	PatchError:
	ClearErrors
	MessageBox MB_ABORTRETRYIGNORE "Error patching ReSharper plug-in configuration file." IDRETRY PatchRetry IDIGNORE PatchExit
	Abort

	PatchExit:
	Pop $5
	Pop $4
	Pop $3
	Pop $2
	Pop $1
	Pop $0
FunctionEnd

