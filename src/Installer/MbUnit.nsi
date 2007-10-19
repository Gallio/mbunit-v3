; Define your application name
!define APPNAME "MbUnit Gallio"
!define APPNAMEANDVERSION "MbUnit Gallio Alpha 1"
!define LIBSDIR "..\..\libs"
!define BUILDDIR "..\..\build"

; Main Install settings
Name "${APPNAMEANDVERSION}"
InstallDir "$PROGRAMFILES\MbUnit Gallio"
OutFile "${BUILDDIR}\MbUnit-Setup.exe"

BrandingText "mbunit.com"

InstType "Full"
InstType "Typical"

; Constants
!define SF_SELECTED 1
!define SF_RO 16

!define /math SF_SELECTED_MASK 255 - ${SF_SELECTED}
!define /math SF_RO_MASK 255 - ${SF_RO}

; Settings
!define MUI_COMPONENTSPAGE_SMALLDESC ; Put description on bottom.
!define MUI_ABORTWARNING

; Modern interface settings
!include "MUI.nsh"

; Pages
!insertmacro MUI_PAGE_WELCOME
Page custom AddRemovePageEnter AddRemovePageLeave
!insertmacro MUI_PAGE_LICENSE "${BUILDDIR}\bin\MbUnit License.txt"
Page custom UserSelectionPageEnter UserSelectionPageLeave
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

; Set languages (first is default language)
!insertmacro MUI_LANGUAGE "English"
!insertmacro MUI_RESERVEFILE_LANGDLL

; Store installation options in the Reserve data block for
; startup efficiency.
ReserveFile "AddRemovePage.ini"
ReserveFile "UserSelectionPage.ini"
!insertmacro MUI_RESERVEFILE_INSTALLOPTIONS
Var INI_VALUE

; Detect whether any components are missing
!tempfile DETECT_TEMP
!system 'if not exist "${BUILDDIR}\docs\chm\MbUnit.chm" echo !define MISSING_CHM_HELP >> "${DETECT_TEMP}"'
!system 'if not exist "${BUILDDIR}\docs\vs2005\MbUnit.HxS" echo !define MISSING_VS2005_HELP >> "${DETECT_TEMP}"'
!include "${DETECT_TEMP}"
!delfile "${DETECT_TEMP}"

; Emit warnings if any components are missing
!ifdef MISSING_CHM_HELP
	!warning "Missing MbUnit.chm file."
!endif

!ifdef MISSING_VS2005_HELP
	!warning "Missing VS2005 help collection."
!endif

; Define sections
Section "!MbUnit Gallio" GallioSection
	; Set Section properties
	SectionIn RO
	SetOverwrite on
	
	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR"
	File "${BUILDDIR}\bin\ASL - Apache Software Foundation License.txt"
	File "${BUILDDIR}\bin\MbUnit License.txt"
	File "MbUnit Website.url"
	File "MbUnit Online Documentation.url"

	SetOutPath "$INSTDIR\bin"
	File "${BUILDDIR}\bin\Castle.Core.dll"
	File "${BUILDDIR}\bin\Castle.DynamicProxy2.dll"
	File "${BUILDDIR}\bin\Castle.MicroKernel.dll"
	File "${BUILDDIR}\bin\Castle.Windsor.dll"
	File "${BUILDDIR}\bin\MbUnit.Gallio.dll"
	File "${BUILDDIR}\bin\MbUnit.Gallio.plugin"
	File "${BUILDDIR}\bin\MbUnit.Gallio.xml"

	SetOutPath "$INSTDIR\bin\Reports"
	File /r "${BUILDDIR}\bin\Reports\*"

	; Create Shortcuts
	CreateDirectory "$SMPROGRAMS\MbUnit Gallio"
	
	CreateShortCut "$SMPROGRAMS\${APPNAME}\Uninstall.lnk" "$INSTDIR\uninstall.exe"
	CreateShortCut "$SMPROGRAMS\${APPNAME}\MbUnit Online Documentation.lnk" "$INSTDIR\MbUnit Online Documentation.url"
	CreateShortCut "$SMPROGRAMS\${APPNAME}\MbUnit Website.lnk" "$INSTDIR\MbUnit.url"
SectionEnd

SectionGroup "Plugins"
Section "MbUnit v2 Plugin" MbUnit2PluginSection
	; Set Section properties
	SetOverwrite on
	
	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\bin\MbUnit2"
	File /r "${BUILDDIR}\bin\MbUnit2\*"
SectionEnd

Section "NUnit Plugin" NUnitPluginSection
	; Set Section properties
	SetOverwrite on
	
	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\bin\NUnit"
	File /r "${BUILDDIR}\bin\NUnit\*"
SectionEnd

Section "xUnit.Net Plugin" XunitPluginSection
	; Set Section properties
	SetOverwrite on
	
	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\bin\Xunit"
	File /r "${BUILDDIR}\bin\Xunit\*"
SectionEnd
SectionGroupEnd

SectionGroup "Runners"
Section "Echo (Console Test Runner)" EchoSection
	; Set Section properties
	SetOverwrite on
	
	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\bin"
	File "${BUILDDIR}\bin\MbUnit.Echo.exe"
	File "${BUILDDIR}\bin\MbUnit.Echo.exe.config"
SectionEnd

Section "Icarus (GUI Test Runner)" IcarusSection
	; Set Section properties
	SetOverwrite on
	
	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\bin"
	File "${BUILDDIR}\bin\ICSharpCode.TextEditor.dll"
	File "${BUILDDIR}\bin\MbUnit.Icarus.exe"
	File "${BUILDDIR}\bin\MbUnit.Icarus.exe.config"
	File "${BUILDDIR}\bin\MbUnit.Icarus.Core.dll"
	File "${BUILDDIR}\bin\ZedGraph.dll"

	CreateDirectory "$SMPROGRAMS\MbUnit Gallio"
	CreateShortCut "$SMPROGRAMS\${APPNAME}\MbUnit Icarus GUI.lnk" "$INSTDIR\bin\MbUnit.Icarus.exe"
SectionEnd

Section "MSBuild Tasks" MSBuildTasksSection
	; Set Section properties
	SetOverwrite on
	
	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\bin"
	File "${BUILDDIR}\bin\MbUnit.Tasks.MSBuild.dll"
	File "${BUILDDIR}\bin\MbUnit.Tasks.MSBuild.xml"
SectionEnd

Section "NAnt Tasks" NAntTasksSection
	; Set Section properties
	SetOverwrite on
	
	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\bin"
	File "${BUILDDIR}\bin\MbUnit.Tasks.NAnt.dll"
	File "${BUILDDIR}\bin\MbUnit.Tasks.NAnt.xml"
SectionEnd

Section "TestDriven.Net AddIn for Gallio" TDNetAddInSection
	; Set Section properties
	SetOverwrite on
	
	; Registry Keys
	WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio" "" "10"
	WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio" "AssemblyPath" "$PROGRAMFILES\MbUnit Gallio\bin\MbUnit.AddIn.TDNet.dll"
	WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio" "TypeName" "MbUnit.AddIn.TDNet.MbUnitTestRunner"
	WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio" "TargetFrameworkAssemblyName" "MbUnit.Gallio"

	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\bin"
	File "${BUILDDIR}\bin\MbUnit.AddIn.TDNet.dll"
SectionEnd

Section "TestDriven.Net AddIn for All Supported Frameworks" TDNetAddInOtherFrameworksSection
	; Set Section properties
	SetOverwrite on
	
	; Registry Keys
        SectionGetFlags ${MbUnit2PluginSection} $0
        IntOp $0 $0 & ${SF_SELECTED}
	IntCmp $0 0 NoMbUnit2
		WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_MbUnit2" "" "5"
		WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_MbUnit2" "AssemblyPath" "$PROGRAMFILES\MbUnit Gallio\bin\MbUnit.AddIn.TDNet.dll"
		WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_MbUnit2" "TypeName" "MbUnit.AddIn.TDNet.MbUnitTestRunner"
		WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_MbUnit2" "TargetFrameworkAssemblyName" "MbUnit.Framework"
	NoMbUnit2:

        SectionGetFlags ${NUnitPluginSection} $0
        IntOp $0 $0 & ${SF_SELECTED}
	IntCmp $0 0 NoNUnit
		WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_NUnit" "" "5"
		WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_NUnit" "AssemblyPath" "$PROGRAMFILES\MbUnit Gallio\bin\MbUnit.AddIn.TDNet.dll"
		WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_NUnit" "TypeName" "MbUnit.AddIn.TDNet.MbUnitTestRunner"
		WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_NUnit" "TargetFrameworkAssemblyName" "nunit.framework"
	NoNUnit:

        SectionGetFlags ${XunitPluginSection} $0
        IntOp $0 $0 & ${SF_SELECTED}
	IntCmp $0 0 NoXunit
		WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_Xunit" "" "5"
		WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_Xunit" "AssemblyPath" "$PROGRAMFILES\MbUnit Gallio\bin\MbUnit.AddIn.TDNet.dll"
		WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_Xunit" "TypeName" "MbUnit.AddIn.TDNet.MbUnitTestRunner"
		WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_Xunit" "TargetFrameworkAssemblyName" "xunit"
	NoXunit:
SectionEnd
SectionGroupEnd

!ifndef MISSING_CHM_HELP | MISSING_VS2005_HELP
SectionGroup "Documentation"
!ifndef MISSING_CHM_HELP
Section "Standalone Help Docs" CHMHelpSection
	; Set Section properties
	SetOverwrite on
	
	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR"
	File "MbUnit Offline Documentation.lnk"

	SetOutPath "$INSTDIR\docs"
	File "${BUILDDIR}\docs\chm\MbUnit.chm"

	; Create Shortcuts
	CreateShortCut "$SMPROGRAMS\${APPNAME}\MbUnit Offline Documentation.lnk" "$INSTDIR\MbUnit Offline Documentation.lnk"
SectionEnd
!endif

!ifndef MISSING_VS2005_HELP
Section "Visual Studio 2005 Help Docs" VS2005HelpSection
	; Set Section properties
	SetOverwrite on

	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\docs\vs2005"
	File "${BUILDDIR}\docs\vs2005\MbUnit.Hx?"
	File "${BUILDDIR}\docs\vs2005\MbUnitCollection.*"

	SetOutPath "$INSTDIR\utils"
	File "${LIBSDIR}\H2Reg\H2Reg.exe"
	File "${LIBSDIR}\H2Reg\H2Reg.ini"

	; Merge the collection
	ExecWait '"$INSTDIR\utils\H2Reg.exe" -r CmdFile="$INSTDIR\docs\vs2005\MbUnitCollection.h2reg.ini"'
SectionEnd
!endif
SectionGroupEnd
!endif

Section -FinishSection
	WriteRegStr SHCTX "Software\${APPNAME}" "" "$INSTDIR"
	WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayName" "${APPNAME}"
	WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "UninstallString" "$INSTDIR\uninstall.exe"
	WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "InstallLocation" "$INSTDIR"
	WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "HelpLink" "http://www.mbunit.com/"
	WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "URLUpdateInfo" "http://www.mbunit.com/"
	WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "URLInfoAbout" "http://www.mbunit.com/"
	WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "URLInfoAbout" "http://www.mbunit.com/"
	WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "NoModify" "1"
	WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "NoRepair" "1"
	WriteUninstaller "$INSTDIR\uninstall.exe"
SectionEnd

;Uninstall section
Section Uninstall
	; Uninstall from TD.Net
	DeleteRegKey SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio"
	DeleteRegKey SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_MbUnit2"
	DeleteRegKey SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_NUnit"
	DeleteRegKey SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_Xunit"

	; Uninstall the help collection
	IfFileExists "$INSTDIR\docs\vs2005\MbUnitCollection.h2reg.ini" 0 +2
		ExecWait '"$INSTDIR\utils\H2Reg.exe" -u CmdFile="$INSTDIR\docs\vs2005\MbUnitCollection.h2reg.ini"'

	; Delete Shortcuts
	Delete "$SMPROGRAMS\${APPNAME}\Uninstall.lnk"
	Delete "$SMPROGRAMS\${APPNAME}\MbUnit Icarus GUI.lnk"
	Delete "$SMPROGRAMS\${APPNAME}\MbUnit Website.lnk"
	Delete "$SMPROGRAMS\${APPNAME}\MbUnit Online Documentation.lnk"
	Delete "$SMPROGRAMS\${APPNAME}\MbUnit Offline Documentation.lnk" 
	RMDir "$SMPROGRAMS\MbUnit Gallio"

	; Remove from registry...
	DeleteRegKey SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"
	DeleteRegKey SHCTX "SOFTWARE\${APPNAME}"

	; Delete self
	Delete "$INSTDIR\uninstall.exe"

	; Remove all remaining contents
	RMDir /r "$INSTDIR"
SectionEnd

; Component descriptions
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
	!insertmacro MUI_DESCRIPTION_TEXT ${GallioSection} "Installs the MbUnit Gallio framework components, test runners and documentation."

	!insertmacro MUI_DESCRIPTION_TEXT ${MbUnit2PluginSection} "Installs the MbUnit v2 plugin.  Enables Gallio to run MbUnit v2 tests."
	!insertmacro MUI_DESCRIPTION_TEXT ${NUnitPluginSection} "Installs the NUnit plugin.  Enables Gallio to run NUnit tests."
	!insertmacro MUI_DESCRIPTION_TEXT ${XunitPluginSection} "Installs the Xunit plugin.  Enables Gallio to run xUnit.Net tests."

	!insertmacro MUI_DESCRIPTION_TEXT ${EchoSection} "Installs the command-line test runner for MbUnit Gallio."
	!insertmacro MUI_DESCRIPTION_TEXT ${IcarusSection} "Installs the GUI-based test runner for MbUnit Gallio."
	!insertmacro MUI_DESCRIPTION_TEXT ${MSBuildTasksSection} "Installs the MSBuild tasks for MbUnit Gallio."
	!insertmacro MUI_DESCRIPTION_TEXT ${NAntTasksSection} "Installs the NAnt tasks for MbUnit Gallio."
	!insertmacro MUI_DESCRIPTION_TEXT ${TDNetAddInSection} "Installs the TestDriven.Net add-in for MbUnit Gallio."
	!insertmacro MUI_DESCRIPTION_TEXT ${TDNetAddInOtherFrameworksSection} "Enables the TestDriven.Net add-in for Gallio to also run tests for all other supported frameworks."

	!ifndef MISSING_CHM_HELP
		!insertmacro MUI_DESCRIPTION_TEXT ${CHMHelpSection} "Installs the MbUnit standalone help documentation CHM file."
	!endif

	!ifndef MISSING_VS2005_HELP
		!insertmacro MUI_DESCRIPTION_TEXT ${VS2005HelpSection} "Installs the MbUnit integrated help documentation for Visual Studio 2005 access with F1."
	!endif
!insertmacro MUI_FUNCTION_DESCRIPTION_END

; Initialization code
Function .onInit
	; Extract install option pages.
	!insertmacro MUI_INSTALLOPTIONS_EXTRACT "AddRemovePage.ini"
	!insertmacro MUI_INSTALLOPTIONS_EXTRACT "UserSelectionPage.ini"

	; Set installation types.
	SectionSetInstTypes ${GallioSection} 3
	SectionSetInstTypes ${MbUnit2PluginSection} 1
	SectionSetInstTypes ${NUnitPluginSection} 1
	SectionSetInstTypes ${XunitPluginSection} 1
	SectionSetInstTypes ${EchoSection} 3
	SectionSetInstTypes ${IcarusSection} 3
	SectionSetInstTypes ${MSBuildTasksSection} 3
	SectionSetInstTypes ${NAntTasksSection} 3
	SectionSetInstTypes ${TDNetAddInSection} 3
	SectionSetInstTypes ${TDNetAddInOtherFrameworksSection} 1
	!ifndef MISSING_CHM_HELP
		SectionSetInstTypes ${CHMHelpSection} 3
	!endif
	!ifndef MISSING_VS2005_HELP
		SectionSetInstTypes ${VS2005HelpSection} 3
	!endif

	SetCurInstType 0

	; Disable VS2005 help section if not installed.
	!ifndef MISSING_VS2005_HELP
	; Check if VS 2005 Standard or above is installed.
	; Specifically exclude Express editions.
        ClearErrors
	ReadRegDWORD $0 HKLM "Software\Microsoft\DevDiv\VS\Servicing\8.0" "SP"
	IfErrors 0 +3
		SectionSetFlags ${VS2005HelpSection} ${SF_RO}
		SectionSetText ${VS2005HelpSection} "The MbUnit integrated help documentation for Visual Studio 2005 requires Visual Studio 2005 Standard Edition or higher to be installed.  Visual Studio 2005 Express Edition is not supported."
	!endif
FunctionEnd

; Uninstaller initialization code.
Function un.onInit
        ClearErrors
	ReadRegStr $0 HKCU "Software\${APPNAME}" ""
	IfErrors NotInstalledForUser
		SetShellVarContext current
		StrCpy $INSTDIR $0
		Goto Installed
	NotInstalledForUser:

        ClearErrors
	ReadRegStr $0 HKLM "Software\${APPNAME}" ""
	IfErrors NotInstalledForSystem
		SetShellVarContext all
		StrCpy $INSTDIR $0
		Goto Installed
	NotInstalledForSystem:

	MessageBox MB_OK "MbUnit does not appear to be installed!  Abandoning uninstallation."
	Abort

	Installed:	
FunctionEnd

; Selection change code
Function .onSelChange
	; Only allow selection of the TDNet addin for other frameworks
	; when at least one of them is selected and the TDNet addin is
	; being installed
        SectionGetFlags ${MbUnit2PluginSection} $0

        SectionGetFlags ${NUnitPluginSection} $1
	IntOp $0 $0 | $1

        SectionGetFlags ${XunitPluginSection} $1
	IntOp $0 $0 | $1

	SectionGetFlags ${TDNetAddInSection} $1
	IntOp $0 $0 & $1
	IntOp $0 $0 & ${SF_SELECTED}

	SectionGetFlags ${TDNetAddInOtherFrameworksSection} $1
	IntCmp $0 0 Disable
		IntOp $1 $1 & ${SF_RO_MASK}
		Goto Done
	Disable:
		IntOp $1 $1 & ${SF_SELECTED_MASK}
		IntOp $1 $1 | ${SF_RO}
	Done:
	
	SectionSetFlags ${TDNetAddInOtherFrameworksSection} $1
FunctionEnd

; Add-remove page.
Var OLD_INSTALL_DIR
Function AddRemovePageEnter
        ClearErrors
	ReadRegStr $OLD_INSTALL_DIR HKCU "Software\${APPNAME}" ""
	IfErrors 0 AlreadyInstalled
	ReadRegStr $OLD_INSTALL_DIR HKLM "Software\${APPNAME}" ""
	IfErrors 0 AlreadyInstalled
	Return

	AlreadyInstalled:
	!insertmacro MUI_HEADER_TEXT "Installation Type" "Please select whether to upgrade or remove the currently installed version."
	!insertmacro MUI_INSTALLOPTIONS_DISPLAY "AddRemovePage.ini"
FunctionEnd

Function AddRemovePageLeave
	!insertmacro MUI_INSTALLOPTIONS_READ $INI_VALUE "AddRemovePage.ini" "Field 2" "State"

	MessageBox MB_OKCANCEL "Uninstall the current version?" IDOK Uninstall
	Abort

	Uninstall:
	ExecWait '"$OLD_INSTALL_DIR\uninstall.exe" /S' $0
	DetailPrint "Uninstaller returned $0"

	IntCmp $INI_VALUE 1 Upgrade
	Quit

	Upgrade:
FunctionEnd

; User-selection page.
Function UserSelectionPageEnter
	!insertmacro MUI_HEADER_TEXT "Installation Options" "Please select for which users to install MbUnit."
	!insertmacro MUI_INSTALLOPTIONS_DISPLAY "UserSelectionPage.ini"
FunctionEnd

Function UserSelectionPageLeave
	!insertmacro MUI_INSTALLOPTIONS_READ $INI_VALUE "UserSelectionPage.ini" "Field 2" "State"
	IntCmp $INI_VALUE 0 CurrentUserOnly
		SetShellVarContext all
		Goto Done
	CurrentUserOnly:
		SetShellVarContext current
	Done:
FunctionEnd
