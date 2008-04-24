Var VS2005UpdateRequired
Var VS2008UpdateRequired

!macro UpdateVS2005IfNeeded
	StrCmp $VS2005UpdateRequired "0" SkipUpdateVS2005
	DetailPrint "Updating Visual Studio 2005 configuration."
	ClearErrors
	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\8.0" "InstallDir"
	IfErrors SkipUpdateVS2005
	ExecWait '"$0\devenv.exe" /setup'
	SkipUpdateVS2005:
!macroend

!macro UpdateVS2008IfNeeded
	StrCmp $VS2008UpdateRequired "0" SkipUpdateVS2008
	DetailPrint "Updating Visual Studio 2008 configuration."
	ClearErrors
	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\9.0" "InstallDir"
	IfErrors SkipUpdateVS2008
	ExecWait '"$0\devenv.exe" /setup'
	SkipUpdateVS2008:
!macroend
