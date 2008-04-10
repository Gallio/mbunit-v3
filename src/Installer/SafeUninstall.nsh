Function un.SafeDelete
	Exch $0 ; file

	DeleteRetry:

	ClearErrors
	Delete "$0"
	IfErrors +1 DeleteDone
		MessageBox MB_ABORTRETRYIGNORE "Unable to delete file '$0'.  It may be in use by some other application." IDRETRY DeleteRetry IDIGNORE DeleteDone
		Abort		

	DeleteDone:

	Pop $0
FunctionEnd

!macro un.SafeDelete FILE
	Push "${FILE}"
	Call un.SafeDelete
!macroend

!define un.SafeDelete '!insertmacro un.SafeDelete'


Function un.SafeRMDir
	Exch $0 ; dir

	DeleteRetry:

	ClearErrors
	RMDir /r "$0"
	IfErrors +1 DeleteDone
		MessageBox MB_ABORTRETRYIGNORE "Unable to delete directory '$0'.  It may be in use by some other application." IDRETRY DeleteRetry IDIGNORE DeleteDone
		Abort		

	DeleteDone:

	Pop $0
FunctionEnd

!macro un.SafeRMDir DIR
	Push "${DIR}"
	Call un.SafeRMDir
!macroend

!define un.SafeRMDir '!insertmacro un.SafeRMDir'

