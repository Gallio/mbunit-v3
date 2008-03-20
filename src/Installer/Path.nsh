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
!macro AddPath CONTEXT VALUE
	Push $0
	Push "${VALUE}"
	!insertmacro GetPath $0 "${CONTEXT}"
	Exch $1
	StrCpy $0 "$0;$1"
	!insertmacro SetPath "${CONTEXT}" "$0"
	Pop $1
	Pop $0
!macroend

; Removes a path segment from the user or system path
;   CONTEXT - "all" or "current"
;   VALUE   - the value to add
!macro RemovePath CONTEXT VALUE
	Push $0
	Push "${VALUE}"
	!insertmacro GetPath $0 "${CONTEXT}"
	Exch $1
	${un.StrReplace} $0 ";$1" "" $0  ; With leading semi-colon
	${un.StrReplace} $0 "$1" "" $0   ; Without leading semi-colon
	!insertmacro SetPath "${CONTEXT}" $0
	Pop $1
	Pop $0
!macroend

