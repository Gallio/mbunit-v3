!include WinMessages.nsh

!define SYSTEMPATHREGKEY "SYSTEM\CurrentControlSet\Control\Session Manager\Environment"
!define USERPATHREGKEY "Environment"

; Gets the user or system path
;   OUT     - the variable in which to store the result
;   CONTEXT - "all" or "current"
!macro GetPath OUT CONTEXT
	StrCpy "${OUT}" ""
	StrCmp "${CONTEXT}" "all" +3
		ReadRegStr "${OUT}" HKCU "${USERPATHREGKEY}" "PATH"
		Goto +2
		ReadRegStr "${OUT}" HKLM "${SYSTEMPATHREGKEY}" "PATH"
!macroend

; Sets the user or system path
;   CONTEXT - "all" or "current"
;   VALUE   - the value to store
!macro SetPath CONTEXT VALUE
	StrCmp "${CONTEXT}" "all" +3
		WriteRegStr HKCU "${USERPATHREGKEY}" "PATH" "${VALUE}"
		Goto +2
		WriteRegStr HKLM "${SYSTEMPATHREGKEY}" "PATH" "${VALUE}"

	SendMessage ${HWND_BROADCAST} ${WM_WININICHANGE} 0 "STR:Environment" /TIMEOUT=5000
!macroend

; Adds a path segment to the user or system path
;   CONTEXT - "all" or "current"
;   VALUE   - the value to add
!define AddPath '!insertmacro AddPath'

!macro AddPath CONTEXT VALUE
	Push "${CONTEXT}"
	Push "${VALUE}"
	Call AddPath
!macroend

Function AddPath
	Exch $0 ; value
	Exch 1
	Exch $1 ; context
	Push $2 ; temp

	ClearErrors
	!insertmacro GetPath $2 $1
	IfErrors AddPathFailed
		StrCpy $2 "$2;$0"
		!insertmacro SetPath $1 $2
		IfErrors AddPathFailed AddPathDone

	AddPathFailed:
		MessageBox MB_OK "Could not add '$0' to the path due to an error.  You may need to add it to the path manually once the installation has completed."

	AddPathDone:

	Pop $2
	Pop $1
	Pop $0
FunctionEnd

; Removes a path segment from the user or system path
;   CONTEXT - "all" or "current"
;   VALUE   - the value to add
!define un.RemovePath '!insertmacro un.RemovePath'

!macro un.RemovePath CONTEXT VALUE
	Push "${CONTEXT}"
	Push "${VALUE}"
	Call un.RemovePath
!macroend

Function un.RemovePath
	Exch $0 ; value
	Exch 1
	Exch $1 ; context
	Push $2 ; temp

	ClearErrors
	!insertmacro GetPath $2 $1
	IfErrors RemovePathFailed
		${un.StrReplace} $2 ";$0" "" $2  ; With leading semi-colon
		${un.StrReplace} $2 "$0" "" $2   ; Without leading semi-colon
		!insertmacro SetPath $1 $2
		IfErrors RemovePathFailed RemovePathDone

	RemovePathFailed:
		MessageBox MB_OK "Could not remove '$0' from the path due to an error.  You may need to remove it from the path manually once the uninstallation has completed."

	RemovePathDone:

	Pop $2
	Pop $1
	Pop $0
FunctionEnd

