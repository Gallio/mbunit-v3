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
	File "${BUILDDIR}\bin\MbUnit.Gallio.Core.dll"
	File "${BUILDDIR}\bin\MbUnit.Gallio.Core.plugin"
	File "${BUILDDIR}\bin\MbUnit.Gallio.Core.xml"
	File "${BUILDDIR}\bin\MbUnit.Gallio.Framework.dll"
	File "${BUILDDIR}\bin\MbUnit.Gallio.Framework.xml"
	File "${BUILDDIR}\bin\MbUnit.Tasks.MSBuild.dll"
	File "${BUILDDIR}\bin\MbUnit.Tasks.MSBuild.xml"
	File "${BUILDDIR}\bin\MbUnit.Tasks.NAnt.dll"
	File "${BUILDDIR}\bin\MbUnit.Tasks.NAnt.xml"
	File "${BUILDDIR}\bin\ZedGraph.dll"

	; Create Shortcuts
	CreateDirectory "$SMPROGRAMS\MbUnit Gallio"
	
	CreateShortCut "$SMPROGRAMS\${APPNAME}\Uninstall.lnk" "$INSTDIR\uninstall.exe"
	CreateShortCut "$SMPROGRAMS\${APPNAME}\MbUnit Online Documentation.lnk" "$INSTDIR\MbUnit Online Documentation.url"
	CreateShortCut "$SMPROGRAMS\${APPNAME}\MbUnit Website.lnk" "$INSTDIR\MbUnit.url"
SectionEnd

Section "MbUnit v2 Plugin" MbUnit2PluginSection
	; Set Section properties
	SetOverwrite on
	
	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\bin\MbUnit2"
	File "${BUILDDIR}\bin\MbUnit2\MbUnit.Framework.2.0.dll"
	File "${BUILDDIR}\bin\MbUnit2\MbUnit.Framework.dll"
	File "${BUILDDIR}\bin\MbUnit2\MbUnit.Plugin.MbUnit2Adapter.dll"
	File "${BUILDDIR}\bin\MbUnit2\MbUnit.Plugin.MbUnit2Adapter.plugin"
	File "${BUILDDIR}\bin\MbUnit2\QuickGraph.Algorithms.dll"
	File "${BUILDDIR}\bin\MbUnit2\QuickGraph.dll"
	File "${BUILDDIR}\bin\MbUnit2\Refly.dll"
	File "${BUILDDIR}\bin\MbUnit2\TestFu.dll"
SectionEnd

Section "NUnit Plugin" NUnitPluginSection
	; Set Section properties
	SetOverwrite on
	
	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\bin\NUnit"
	File "${BUILDDIR}\bin\NUnit\license.txt"
	File "${BUILDDIR}\bin\NUnit\MbUnit.Plugin.NUnitAdapter.dll"
	File "${BUILDDIR}\bin\NUnit\MbUnit.Plugin.NUnitAdapter.plugin"
	File "${BUILDDIR}\bin\NUnit\nunit.core.dll"
	File "${BUILDDIR}\bin\NUnit\nunit.core.extensions.dll"
	File "${BUILDDIR}\bin\NUnit\nunit.core.interfaces.dll"
	File "${BUILDDIR}\bin\NUnit\nunit.framework.dll"
	File "${BUILDDIR}\bin\NUnit\nunit.framework.extensions.dll"
SectionEnd

Section "TestDriven.Net AddIn" TDNetAddInSection
	; Set Section properties
	SetOverwrite on
	
	; Registry Keys
	WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio" "" "15"
	WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio" "AssemblyPath" "$PROGRAMFILES\MbUnit Gallio\MbUnit.AddIn.TDNet.dll"
	WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio" "TypeName" "MbUnit.AddIn.TDNet.MbUnitTestRunner"
	WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio" "TargetFrameworkAssemblyName" "MbUnit.Gallio.Framework"

	WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_MbUnit2" "" "20"
	WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_MbUnit2" "AssemblyPath" "$PROGRAMFILES\MbUnit Gallio\MbUnit.AddIn.TDNet.dll"
	WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_MbUnit2" "TypeName" "MbUnit.AddIn.TDNet.MbUnitTestRunner"
	WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_MbUnit2" "TargetFrameworkAssemblyName" "MbUnit.Framework"

	WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_NUnit" "" "20"
	WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_NUnit" "AssemblyPath" "$PROGRAMFILES\MbUnit Gallio\MbUnit.AddIn.TDNet.dll"
	WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_NUnit" "TypeName" "MbUnit.AddIn.TDNet.MbUnitTestRunner"
	WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_NUnit" "TargetFrameworkAssemblyName" "nunit.framework"

	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\bin"
	File "${BUILDDIR}\bin\MbUnit.AddIn.TDNet.dll"
SectionEnd

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
	; Remove from registry...
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"
	DeleteRegKey HKLM "SOFTWARE\${APPNAME}"

	; Install the help collection
	IfFileExists "$INSTDIR\docs\vs2005\MbUnitCollection.h2reg.ini" 0 +2
		ExecWait '"$INSTDIR\utils\H2Reg.exe" -u CmdFile="$INSTDIR\docs\vs2005\MbUnitCollection.h2reg.ini"'

	; Delete self
	Delete "$INSTDIR\uninstall.exe"

	; Delete Shortcuts
	Delete "$SMPROGRAMS\${APPNAME}\Uninstall.lnk"
	Delete "$SMPROGRAMS\${APPNAME}\MbUnit Website.lnk"
	Delete "$SMPROGRAMS\${APPNAME}\MbUnit Online Documentation.lnk"
	Delete "$SMPROGRAMS\${APPNAME}\MbUnit Offline Documentation.lnk" 

	; Clean up MbUnit Gallio
	Delete "$INSTDIR\ASL - Apache Software Foundation License.txt"
	Delete "$INSTDIR\MbUnit License.txt"
	Delete "$INSTDIR\MbUnit Website.url"
	Delete "$INSTDIR\MbUnit Online Documentation.url"
	Delete "$INSTDIR\MbUnit Offline Documentation.lnk"

	Delete "$INSTDIR\bin\Castle.Core.dll"
	Delete "$INSTDIR\bin\Castle.DynamicProxy2.dll"
	Delete "$INSTDIR\bin\Castle.MicroKernel.dll"
	Delete "$INSTDIR\bin\Castle.Windsor.dll"
	Delete "$INSTDIR\bin\ICSharpCode.TextEditor.dll"
	Delete "$INSTDIR\bin\MbUnit.AddIn.TDNet.dll"
	Delete "$INSTDIR\bin\MbUnit.Echo.exe"
	Delete "$INSTDIR\bin\MbUnit.Echo.exe.config"
	Delete "$INSTDIR\bin\MbUnit.Gallio.Core.dll"
	Delete "$INSTDIR\bin\MbUnit.Gallio.Core.plugin"
	Delete "$INSTDIR\bin\MbUnit.Gallio.Core.xml"
	Delete "$INSTDIR\bin\MbUnit.Gallio.Framework.dll"
	Delete "$INSTDIR\bin\MbUnit.Gallio.Framework.xml"
	Delete "$INSTDIR\bin\MbUnit.Tasks.MSBuild.dll"
	Delete "$INSTDIR\bin\MbUnit.Tasks.MSBuild.xml"
	Delete "$INSTDIR\bin\MbUnit.Tasks.NAnt.dll"
	Delete "$INSTDIR\bin\MbUnit.Tasks.NAnt.xml"
	Delete "$INSTDIR\bin\ZedGraph.dll"

	Delete "$INSTDIR\bin\MbUnit2\MbUnit.Framework.2.0.dll"
	Delete "$INSTDIR\bin\MbUnit2\MbUnit.Framework.dll"
	Delete "$INSTDIR\bin\MbUnit2\MbUnit.Plugin.MbUnit2Adapter.dll"
	Delete "$INSTDIR\bin\MbUnit2\MbUnit.Plugin.MbUnit2Adapter.plugin"
	Delete "$INSTDIR\bin\MbUnit2\QuickGraph.Algorithms.dll"
	Delete "$INSTDIR\bin\MbUnit2\QuickGraph.dll"
	Delete "$INSTDIR\bin\MbUnit2\Refly.dll"
	Delete "$INSTDIR\bin\MbUnit2\TestFu.dll"

	Delete "$INSTDIR\bin\NUnit\license.txt"
	Delete "$INSTDIR\bin\NUnit\MbUnit.Plugin.NUnitAdapter.dll"
	Delete "$INSTDIR\bin\NUnit\MbUnit.Plugin.NUnitAdapter.plugin"
	Delete "$INSTDIR\bin\NUnit\nunit.core.dll"
	Delete "$INSTDIR\bin\NUnit\nunit.core.extensions.dll"
	Delete "$INSTDIR\bin\NUnit\nunit.core.interfaces.dll"
	Delete "$INSTDIR\bin\NUnit\nunit.framework.dll"
	Delete "$INSTDIR\bin\NUnit\nunit.framework.extensions.dll"

	Delete "$INSTDIR\docs\MbUnit.chm"
	Delete "$INSTDIR\docs\vs2005\MbUnit.HxS"
	Delete "$INSTDIR\docs\vs2005\MbUnitCollection.*"

	Delete "$INSTDIR\utils\H2Reg.exe"
	Delete "$INSTDIR\utils\H2Reg.ini"
	Delete "$INSTDIR\utils\H2Reg_Log.txt"
	
	; Remove remaining directories
	RMDir "$SMPROGRAMS\MbUnit Gallio"
	RMDir "$INSTDIR\bin\MbUnit2"
	RMDir "$INSTDIR\bin\NUnit"
	RMDir "$INSTDIR\bin"
	RMDir "$INSTDIR\docs\vs2005"
	RMDir "$INSTDIR\docs"
	RMDir "$INSTDIR\utils"
	RMDir "$INSTDIR"

	; Remove registry keys
	DeleteRegKey SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio"
	DeleteRegKey SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_MbUnit"
	DeleteRegKey SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnit.Gallio_NUnit"
SectionEnd

BrandingText "mbunit.com"

; eof
