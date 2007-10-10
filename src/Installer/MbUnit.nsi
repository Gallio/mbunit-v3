; Script generated with the Venis Install Wizard

; Define your application name
!define APPNAME "MbUnit Gallio"
!define APPNAMEANDVERSION "MbUnit Gallio Alpha 1"
!define LIBSDIR "..\..\libs"
!define BUILDDIR "..\..\build"

; Main Install settings
Name "${APPNAMEANDVERSION}"
InstallDir "$PROGRAMFILES\MbUnit Gallio"
InstallDirRegKey HKLM "Software\${APPNAME}" ""
OutFile "${BUILDDIR}\MbUnit-Setup.exe"

; Modern interface settings
!include "MUI.nsh"

!define MUI_ABORTWARNING

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "${BUILDDIR}\bin\MbUnit License.txt"
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

; Set languages (first is default language)
!insertmacro MUI_LANGUAGE "English"
!insertmacro MUI_RESERVEFILE_LANGDLL

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
	SetShellVarContext all
	
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
	File "${BUILDDIR}\bin\ICSharpCode.TextEditor.dll"
	File "${BUILDDIR}\bin\MbUnit.Echo.exe"
	File "${BUILDDIR}\bin\MbUnit.Echo.exe.config"
	File "${BUILDDIR}\bin\MbUnit.Icarus.exe"
	File "${BUILDDIR}\bin\MbUnit.Icarus.exe.config"
	File "${BUILDDIR}\bin\MbUnit.Icarus.Core.dll"
	File "${BUILDDIR}\bin\MbUnit.Gallio.dll"
	File "${BUILDDIR}\bin\MbUnit.Gallio.plugin"
	File "${BUILDDIR}\bin\MbUnit.Gallio.xml"
	File "${BUILDDIR}\bin\MbUnit.Tasks.MSBuild.dll"
	File "${BUILDDIR}\bin\MbUnit.Tasks.MSBuild.xml"
	File "${BUILDDIR}\bin\MbUnit.Tasks.NAnt.dll"
	File "${BUILDDIR}\bin\MbUnit.Tasks.NAnt.xml"
	File "${BUILDDIR}\bin\ZedGraph.dll"

	SetOutPath "$INSTDIR\bin\Reports"
	File /r "${BUILDDIR}\bin\Reports\*"

	; Create Shortcuts
	CreateDirectory "$SMPROGRAMS\MbUnit Gallio"
	
	CreateShortCut "$SMPROGRAMS\${APPNAME}\Uninstall.lnk" "$INSTDIR\uninstall.exe"
	CreateShortCut "$SMPROGRAMS\${APPNAME}\MbUnit Icarus GUI.lnk" "$INSTDIR\bin\MbUnit.Icarus.exe"
	CreateShortCut "$SMPROGRAMS\${APPNAME}\MbUnit Online Documentation.lnk" "$INSTDIR\MbUnit Online Documentation.url"
	CreateShortCut "$SMPROGRAMS\${APPNAME}\MbUnit Website.lnk" "$INSTDIR\MbUnit.url"
SectionEnd

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

Section "Xunit Plugin" XunitPluginSection
	; Set Section properties
	SetOverwrite on
	
	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\bin\Xunit"
	File /r "${BUILDDIR}\bin\Xunit\*"
SectionEnd

Section "TestDriven.Net AddIn" TDNetAddInSection
	; Set Section properties
	SetOverwrite on
	
	; Registry Keys
	WriteRegStr HKLM "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio" "" "15"
	WriteRegStr HKLM "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio" "AssemblyPath" "$PROGRAMFILES\MbUnit Gallio\bin\MbUnit.AddIn.TDNet.dll"
	WriteRegStr HKLM "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio" "TypeName" "MbUnit.AddIn.TDNet.MbUnitTestRunner"
	WriteRegStr HKLM "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio" "TargetFrameworkAssemblyName" "MbUnit.Gallio"

	WriteRegStr HKLM "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_MbUnit2" "" "20"
	WriteRegStr HKLM "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_MbUnit2" "AssemblyPath" "$PROGRAMFILES\MbUnit Gallio\bin\MbUnit.AddIn.TDNet.dll"
	WriteRegStr HKLM "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_MbUnit2" "TypeName" "MbUnit.AddIn.TDNet.MbUnitTestRunner"
	WriteRegStr HKLM "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_MbUnit2" "TargetFrameworkAssemblyName" "MbUnit.Framework"

	WriteRegStr HKLM "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_NUnit" "" "20"
	WriteRegStr HKLM "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_NUnit" "AssemblyPath" "$PROGRAMFILES\MbUnit Gallio\bin\MbUnit.AddIn.TDNet.dll"
	WriteRegStr HKLM "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_NUnit" "TypeName" "MbUnit.AddIn.TDNet.MbUnitTestRunner"
	WriteRegStr HKLM "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_NUnit" "TargetFrameworkAssemblyName" "nunit.framework"

	WriteRegStr HKLM "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_Xunit" "" "20"
	WriteRegStr HKLM "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_Xunit" "AssemblyPath" "$PROGRAMFILES\MbUnit Gallio\bin\MbUnit.AddIn.TDNet.dll"
	WriteRegStr HKLM "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_Xunit" "TypeName" "MbUnit.AddIn.TDNet.MbUnitTestRunner"
	WriteRegStr HKLM "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_Xunit" "TargetFrameworkAssemblyName" "xunit"

	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\bin"
	File "${BUILDDIR}\bin\MbUnit.AddIn.TDNet.dll"
SectionEnd

!ifndef MISSING_CHM_HELP
Section "Standalone Help Docs" CHMHelpSection
	; Set Section properties
	SetOverwrite on
	SetShellVarContext all
	
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

Section -FinishSection
	WriteRegStr HKLM "Software\${APPNAME}" "" "$INSTDIR"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayName" "${APPNAME}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "UninstallString" "$INSTDIR\uninstall.exe"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "InstallLocation" "$INSTDIR"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "HelpLink" "http://www.mbunit.com/"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "URLUpdateInfo" "http://www.mbunit.com/"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "URLInfoAbout" "http://www.mbunit.com/"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "URLInfoAbout" "http://www.mbunit.com/"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "NoModify" "1"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "NoRepair" "1"
	WriteUninstaller "$INSTDIR\uninstall.exe"
SectionEnd

; Modern install component descriptions
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
	!insertmacro MUI_DESCRIPTION_TEXT ${GallioSection} "Installs the MbUnit Gallio framework components, test runners and documentation."
	!insertmacro MUI_DESCRIPTION_TEXT ${MbUnit2PluginSection} "Installs the MbUnit v2 plugin.  Enables Gallio to run MbUnit v2 tests."
	!insertmacro MUI_DESCRIPTION_TEXT ${NUnitPluginSection} "Installs the NUnit plugin.  Enables Gallio to run NUnit tests."
	!insertmacro MUI_DESCRIPTION_TEXT ${XunitPluginSection} "Installs the Xunit plugin.  Enables Gallio to run Xunit tests."
	!insertmacro MUI_DESCRIPTION_TEXT ${TDNetAddInSection} "Installs the TestDriven.Net add-in for MbUnit Gallio."

	!ifndef MISSING_CHM_HELP
		!insertmacro MUI_DESCRIPTION_TEXT ${CHMHelpSection} "Installs the MbUnit standalone help documentation CHM file."
	!endif

	!ifndef MISSING_VS2005_HELP
		!insertmacro MUI_DESCRIPTION_TEXT ${VS2005HelpSection} "Installs the MbUnit integrated help documentation for Visual Studio 2005.  Enables access to the MbUnit help documentation using F1 help."
	!endif
!insertmacro MUI_FUNCTION_DESCRIPTION_END

; Initialization code
!define SF_RO 16
Function .onInit
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


;Uninstall section
Section Uninstall
	SetShellVarContext all

	; Uninstall from TD.Net
	DeleteRegKey HKLM "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio"
	DeleteRegKey HKLM "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_MbUnit"
	DeleteRegKey HKLM "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_NUnit"
	DeleteRegKey HKLM "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_Xunit"

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
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"
	DeleteRegKey HKLM "SOFTWARE\${APPNAME}"

	; Delete self
	Delete "$INSTDIR\uninstall.exe"

	; Remove all remaining contents
	RMDir /r "$INSTDIR"
SectionEnd

BrandingText "mbunit.com"

; eof
