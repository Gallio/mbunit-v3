; Check arguments.
!ifndef VERSION
	!error "The /DVersion=x.y.z.w argument must be specified."
!endif
!ifndef ROOTDIR
	!error "The /DRootDir=... argument must be specified."
!endif

; Define your application name
!define APPNAME "Gallio"
!define APPNAMEANDVERSION "Gallio v${VERSION}"
!define LIBSDIR "${ROOTDIR}libs"
!define BUILDDIR "${ROOTDIR}\build"
!define TARGETDIR "${BUILDDIR}\target"
!define RELEASEDIR "${BUILDDIR}\release"

!include "StrRep.nsh"

; Main Install settings
Name "${APPNAMEANDVERSION}"
InstallDir "$PROGRAMFILES\Gallio"
OutFile "${RELEASEDIR}\GallioBundle-${VERSION}-Setup.exe"

BrandingText "gallio.org"

InstType "Full"
InstType "Typical"

; Constants
!define SF_SELECTED_MASK 254
!define SF_RO_MASK 239

; Modern interface settings
!define MUI_COMPONENTSPAGE_SMALLDESC ; Put description on bottom.
!define MUI_ABORTWARNING
!include "MUI.nsh"

; Installer pages
!insertmacro MUI_PAGE_WELCOME
Page custom AddRemovePageEnter AddRemovePageLeave
!insertmacro MUI_PAGE_LICENSE "${TARGETDIR}\Gallio License.txt"
Page custom UserSelectionPageEnter UserSelectionPageLeave
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

; Uninstaller pages
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

; Stores "all" if installing for all users, else "current"
Var UserContext

; Detect whether any components are missing
!tempfile DETECT_TEMP
!system 'if not exist "${TARGETDIR}\docs\Gallio.chm" echo !define MISSING_CHM_HELP >> "${DETECT_TEMP}"'
!system 'if not exist "${TARGETDIR}\docs\vs2005\Gallio.HxS" echo !define MISSING_VS2005_HELP >> "${DETECT_TEMP}"'
!include "${DETECT_TEMP}"
!delfile "${DETECT_TEMP}"

; Emit warnings if any components are missing
!ifdef MISSING_CHM_HELP
	!warning "Missing CHM file."
!endif

!ifdef MISSING_VS2005_HELP
	!warning "Missing VS2005 help collection."
!endif

; Define sections
Section "!Gallio" GallioSection
	; Set Section properties
	SectionIn RO
	SetOverwrite on
	
	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR"
	File "${TARGETDIR}\ASL - Apache Software Foundation License.txt"
	File "${TARGETDIR}\Gallio License.txt"
	File "${TARGETDIR}\Release Notes.txt"
	File "${TARGETDIR}\Gallio Website.url"

	SetOutPath "$INSTDIR\bin"
	File "${TARGETDIR}\bin\Castle.Core.dll"
	File "${TARGETDIR}\bin\Castle.DynamicProxy2.dll"
	File "${TARGETDIR}\bin\Castle.MicroKernel.dll"
	File "${TARGETDIR}\bin\Castle.Windsor.dll"
	File "${TARGETDIR}\bin\Gallio.dll"
	File "${TARGETDIR}\bin\Gallio.pdb"
	File "${TARGETDIR}\bin\Gallio.XmlSerializers.dll"
	File "${TARGETDIR}\bin\Gallio.plugin"
	File "${TARGETDIR}\bin\Gallio.xml"
	File "${TARGETDIR}\bin\Gallio.Host.exe"

	SetOutPath "$INSTDIR\bin\Reports"
	File /r "${TARGETDIR}\bin\Reports\*"

	; Create Shortcuts
	SetOutPath "$SMPROGRAMS\${APPNAME}"
	CreateDirectory "$SMPROGRAMS\${APPNAME}"
	CreateShortCut "$SMPROGRAMS\${APPNAME}\Uninstall.lnk" "$INSTDIR\uninstall.exe"
	File "${TARGETDIR}\Gallio Website.url"
SectionEnd

SectionGroup "!MbUnit v3"
Section "!MbUnit v3 Framework" MbUnitSection
	; Set Section properties
	SetOverwrite on
	
	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR"
	File "${TARGETDIR}\MbUnit Website.url"

	SetOutPath "$INSTDIR\bin"
	File "${TARGETDIR}\bin\MbUnit.dll"
	File "${TARGETDIR}\bin\MbUnit.pdb"
	File "${TARGETDIR}\bin\MbUnit.plugin"
	File "${TARGETDIR}\bin\MbUnit.xml"

	; Create Shortcuts
	SetOutPath "$SMPROGRAMS\${APPNAME}"
	File "${TARGETDIR}\MbUnit Website.url"
SectionEnd

Section "MbUnit v3 Pex Package" MbUnitPexSection
	; Set Section properties
	SetOverwrite on
	
	SetOutPath "$INSTDIR\bin"
	File "${TARGETDIR}\bin\MbUnit.Pex.dll"
SectionEnd

Section "MbUnit v3 Visual Studio 2005 Templates" MbUnitVS2005TemplatesSection
	; Set Section properties
	SetOverwrite on

	ClearErrors
	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\8.0" "InstallDir"
	IfErrors SkipMbUnitVS2005Templates

        ; C# Item Templates
	SetOutPath "$0\ItemTemplates\CSharp\Test"
	File "${TARGETDIR}\extras\Templates\VS2005\ItemTemplates\CSharp\Test\MbUnit.TestFixtureTemplate.CSharp.zip"

	; C# Project Templates
	SetOutPath "$0\ProjectTemplates\CSharp\Test"
	File "${TARGETDIR}\extras\Templates\VS2005\ProjectTemplates\CSharp\Test\MbUnit.TestProjectTemplate.CSharp.zip"

        ; VB Item Templates
	SetOutPath "$0\ItemTemplates\VisualBasic\Test"
	File "${TARGETDIR}\extras\Templates\VS2005\ItemTemplates\VisualBasic\Test\MbUnit.TestFixtureTemplate.VisualBasic.zip"

	; VB Project Templates
	SetOutPath "$0\ProjectTemplates\VisualBasic\Test"
	File "${TARGETDIR}\extras\Templates\VS2005\ProjectTemplates\VisualBasic\Test\MbUnit.TestProjectTemplate.VisualBasic.zip"

	; Run DevEnv /setup to register the templates.
	ExecWait '"$0\devenv.exe" /setup'

	SkipMbUnitVS2005Templates:
SectionEnd

Section "MbUnit v3 Visual Studio 2008 Templates" MbUnitVS2008TemplatesSection
	; Set Section properties
	SetOverwrite on

	ClearErrors
	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\9.0" "InstallDir"
	IfErrors SkipMbUnitVS2008Templates

        ; C# Item Templates
	SetOutPath "$0\ItemTemplates\CSharp\Test"
	File "${TARGETDIR}\extras\Templates\VS2008\ItemTemplates\CSharp\Test\MbUnit.TestFixtureTemplate.CSharp.zip"

	; C# Project Templates
	SetOutPath "$0\ProjectTemplates\CSharp\Test"
	File "${TARGETDIR}\extras\Templates\VS2008\ProjectTemplates\CSharp\Test\MbUnit.TestProjectTemplate.CSharp.zip"
	File "${TARGETDIR}\extras\Templates\VS2008\ProjectTemplates\CSharp\Test\MbUnit.MvcWebApplicationTestProjectTemplate.CSharp.zip"

	WriteRegStr HKLM "SOFTWARE\Microsoft\VisualStudio\9.0\MVC\TestProjectTemplates\MbUnit\C#" "Path" "CSharp\Test"
	WriteRegStr HKLM "SOFTWARE\Microsoft\VisualStudio\9.0\MVC\TestProjectTemplates\MbUnit\C#" "Template" "MbUnit.MvcWebApplicationTestProjectTemplate.CSharp.zip"
	WriteRegStr HKLM "SOFTWARE\Microsoft\VisualStudio\9.0\MVC\TestProjectTemplates\MbUnit\C#" "TestFrameworkName" "MbUnit v3"
	WriteRegStr HKLM "SOFTWARE\Microsoft\VisualStudio\9.0\MVC\TestProjectTemplates\MbUnit\C#" "AdditionalInfo" "http://www.mbunit.com/"

        ; VB Item Templates
	SetOutPath "$0\ItemTemplates\VisualBasic\Test"
	File "${TARGETDIR}\extras\Templates\VS2008\ItemTemplates\VisualBasic\Test\MbUnit.TestFixtureTemplate.VisualBasic.zip"

	; VB Project Templates
	SetOutPath "$0\ProjectTemplates\VisualBasic\Test"
	File "${TARGETDIR}\extras\Templates\VS2008\ProjectTemplates\VisualBasic\Test\MbUnit.TestProjectTemplate.VisualBasic.zip"
	File "${TARGETDIR}\extras\Templates\VS2008\ProjectTemplates\VisualBasic\Test\MbUnit.MvcWebApplicationTestProjectTemplate.VisualBasic.zip"

	WriteRegStr HKLM "SOFTWARE\Microsoft\VisualStudio\9.0\MVC\TestProjectTemplates\MbUnit\VB" "Path" "VisualBasic\Test"
	WriteRegStr HKLM "SOFTWARE\Microsoft\VisualStudio\9.0\MVC\TestProjectTemplates\MbUnit\VB" "Template" "MbUnit.MvcWebApplicationTestProjectTemplate.VisualBasic.zip"
	WriteRegStr HKLM "SOFTWARE\Microsoft\VisualStudio\9.0\MVC\TestProjectTemplates\MbUnit\VB" "TestFrameworkName" "MbUnit v3"
	WriteRegStr HKLM "SOFTWARE\Microsoft\VisualStudio\9.0\MVC\TestProjectTemplates\MbUnit\VB" "AdditionalInfo" "http://www.mbunit.com/"

	; Run DevEnv /setup to register the templates.
	ExecWait '"$0\devenv.exe" /setup'

	SkipMbUnitVS2008Templates:
SectionEnd
SectionGroupEnd

SectionGroup "Plugins"
Section "MbUnit v2 Plugin" MbUnit2PluginSection
	; Set Section properties
	SetOverwrite on
	
	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\bin\MbUnit2"
	File /r "${TARGETDIR}\bin\MbUnit2\*"
SectionEnd

Section "MSTest Plugin" MSTestPluginSection
	; Set Section properties
	SetOverwrite on
	
	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\bin\MSTest"
	File /r "${TARGETDIR}\bin\MSTest\*"
SectionEnd

Section "NUnit Plugin" NUnitPluginSection
	; Set Section properties
	SetOverwrite on
	
	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\bin\NUnit"
	File /r "${TARGETDIR}\bin\NUnit\*"
SectionEnd

Section "xUnit.Net Plugin" XunitPluginSection
	; Set Section properties
	SetOverwrite on
	
	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\bin\Xunit"
	File /r "${TARGETDIR}\bin\Xunit\*"
SectionEnd
SectionGroupEnd

SectionGroup "Runners"
Section "Echo (Console Test Runner)" EchoSection
	; Set Section properties
	SetOverwrite on
	
	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\bin"
	File "${TARGETDIR}\bin\Gallio.Echo.exe"
	File "${TARGETDIR}\bin\Gallio.Echo.exe.config"
SectionEnd

Section "Icarus (GUI Test Runner)" IcarusSection
	; Set Section properties
	SetOverwrite on
	
	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\bin"
	File "${TARGETDIR}\bin\WeifenLuo.WinFormsUI.Docking.dll"
	File "${TARGETDIR}\bin\ZedGraph.dll"
	File "${TARGETDIR}\bin\ICSharpCode.TextEditor.dll"
	File "${TARGETDIR}\bin\Gallio.Icarus.exe"
	File "${TARGETDIR}\bin\Gallio.Icarus.exe.config"

	CreateDirectory "$SMPROGRAMS\${APPNAME}"
	CreateShortCut "$SMPROGRAMS\${APPNAME}\Icarus GUI Test Runner.lnk" "$INSTDIR\bin\Gallio.Icarus.exe"
SectionEnd

Section "MSBuild Tasks" MSBuildTasksSection
	; Set Section properties
	SetOverwrite on
	
	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\bin"
	File "${TARGETDIR}\bin\Gallio.MSBuildTasks.dll"
	File "${TARGETDIR}\bin\Gallio.MSBuildTasks.xml"
SectionEnd

Section "NAnt Tasks" NAntTasksSection
	; Set Section properties
	SetOverwrite on
	
	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\bin"
	File "${TARGETDIR}\bin\Gallio.NAntTasks.dll"
	File "${TARGETDIR}\bin\Gallio.NAntTasks.xml"
SectionEnd

Section "PowerShell Commands" PowerShellCommandsSection
	; Set Section properties
	SetOverwrite on
	
	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\bin"
	File "${TARGETDIR}\bin\Gallio.PowerShellCommands.dll"
	File "${TARGETDIR}\bin\Gallio.PowerShellCommands.xml"
	File "${TARGETDIR}\bin\Gallio.PowerShellCommands.dll-Help.xml"

	; Registry keys for the snap-in
	WriteRegStr SHCTX "SOFTWARE\Microsoft\PowerShell\1\PowerShellSnapIns\Gallio" "ApplicationBase" "$INSTDIR\bin"
	WriteRegStr SHCTX "SOFTWARE\Microsoft\PowerShell\1\PowerShellSnapIns\Gallio" "AssemblyName" "Gallio.PowerShellCommands, Version=${VERSION}, Culture=neutral, PublicKeyToken=eb9cfa67ee6ab36e"
	WriteRegStr SHCTX "SOFTWARE\Microsoft\PowerShell\1\PowerShellSnapIns\Gallio" "Description" "Gallio Commands."
	WriteRegStr SHCTX "SOFTWARE\Microsoft\PowerShell\1\PowerShellSnapIns\Gallio" "ModuleName" "$INSTDIR\bin\Gallio.PowerShellCommands.dll"
	WriteRegStr SHCTX "SOFTWARE\Microsoft\PowerShell\1\PowerShellSnapIns\Gallio" "PowerShellVersion" "1.0"
	WriteRegStr SHCTX "SOFTWARE\Microsoft\PowerShell\1\PowerShellSnapIns\Gallio" "Vendor" "Gallio"
	WriteRegStr SHCTX "SOFTWARE\Microsoft\PowerShell\1\PowerShellSnapIns\Gallio" "Version" "${VERSION}"
SectionEnd

Var ReSharperInstallDir
Var ReSharperPluginDir
!macro GetReSharperPluginDir RSVersion VSVersion
	ClearErrors
	ReadRegStr $0 HKCU "Software\JetBrains\ReSharper\${RSVersion}\${VSVersion}" "InstallDir"
	IfErrors +3
		StrCpy $ReSharperInstallDir $0
		Goto +8

	ClearErrors
	ReadRegStr $0 HKLM "Software\JetBrains\ReSharper\${RSVersion}\${VSVersion}" "InstallDir"
	IfErrors +3
		StrCpy $ReSharperInstallDir $0
		Goto +3

	StrCpy $ReSharperPluginDir ""
	Goto +5

	StrCmp "current" $UserContext +3
		StrCpy $ReSharperPluginDir "$ReSharperInstallDir\Plugins"
		Goto +2

	StrCpy $ReSharperPluginDir "$APPDATA\JetBrains\ReSharper\${RSVersion}\${VSVersion}\Plugins"
!macroend

Function PatchReSharperConfigFile
	PatchRetry:
	FileOpen $1 "Gallio.ReSharperRunner.dll.config.orig" "r"
	IfErrors PatchError
	FileOpen $2 "Gallio.ReSharperRunner.dll.config" "w"
	IfErrors PatchErrorCloseSource
	
	PatchLoop:
	FileRead $1 $3
	IfErrors PatchDone
	${StrReplace} $3 "..\..\..\Gallio\bin<!-- Pre-configured for debugging purposes -->" "$INSTDIR\bin" $3
	FileWrite $2 $3
	GoTo PatchLoop

	PatchDone:
	ClearErrors
	FileClose $2
	FileClose $1
	Delete "$OUTDIR\Gallio.ReSharperRunner.dll.config.orig"
	IfErrors PatchError
	PatchSkip:
	Return

	PatchErrorCloseSource:
	FileClose $1
	PatchError:
	ClearErrors
	MessageBox MB_ABORTRETRYIGNORE "Error patching ReSharper plug-in configuration file." IDRETRY PatchRetry IDIGNORE PatchSkip
	Abort

FunctionEnd

!macro InstallReSharperRunner RSVersion VSVersion SourcePath
	!insertmacro GetReSharperPluginDir "${RSVersion}" "${VSVersion}"

	StrCmp "" "$ReSharperPluginDir" +5
		SetOutPath "$ReSharperPluginDir\Gallio"
		File "${SourcePath}\Gallio.ReSharperRunner.dll"
		File "/oname=Gallio.ReSharperRunner.dll.config.orig" "${SourcePath}\Gallio.ReSharperRunner.dll.config"
		Call PatchReSharperConfigFile
!macroend

!macro UninstallReSharperRunner RSVersion VSVersion
	!insertmacro GetReSharperPluginDir "${RSVersion}" "${VSVersion}"

	StrCmp "" "$ReSharperPluginDir" +4
		Delete "$ReSharperPluginDir\Gallio\Gallio.ReSharperRunner.dll"
		Delete "$ReSharperPluginDir\Gallio\Gallio.ReSharperRunner.dll.config"
		RMDir "$ReSharperPluginDir\Gallio"
!macroend

Section "ReSharper v3.1 Runner" ReSharperRunnerSection
	; Set Section properties
	SetOverwrite on
	
	; Set Section Files and Shortcuts
	!insertmacro InstallReSharperRunner "v3.1" "vs8.0" "${TARGETDIR}\bin"
	!insertmacro InstallReSharperRunner "v3.1" "vs9.0" "${TARGETDIR}\bin"
SectionEnd

!macro InstallTDNetRunner Key Framework Priority
	WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\${Key}" "" "${Priority}"
	WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\${Key}" "Application" "$INSTDIR\bin\Gallio.Icarus.exe"
	WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\${Key}" "AssemblyPath" "$INSTDIR\bin\Gallio.TDNetRunner.dll"
	WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\${Key}" "TypeName" "Gallio.TDNetRunner.GallioTestRunner"
	WriteRegStr SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\${Key}" "TargetFrameworkAssemblyName" "${Framework}"
!macroend

!macro UninstallTDNetRunner Key
	DeleteRegKey SHCTX "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\${Key}"
!macroend

Section "TestDriven.Net Runner" TDNetAddInSection
	; Set Section properties
	SetOverwrite on
	
	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\bin"
	File "${TARGETDIR}\bin\Gallio.TDNetRunner.dll"

	; Registry Keys
	SectionGetFlags ${MbUnitSection} $0
	IntOp $0 $0 & ${SF_SELECTED}
	IntCmp $0 0 NoMbUnit
		!insertmacro InstallTDNetRunner "Gallio_MbUnit" "MbUnit" "10"
	NoMbUnit:

	; TODO: We should probably allow users to select whether Gallio should override
	;       their existing TD.Net addins.
	SectionGetFlags ${MbUnit2PluginSection} $0
	IntOp $0 $0 & ${SF_SELECTED}
	IntCmp $0 0 NoMbUnit2
		!insertmacro InstallTDNetRunner "Gallio_MbUnit2" "MbUnit.Framework" "5"
	NoMbUnit2:

	SectionGetFlags ${MSTestPluginSection} $0
	IntOp $0 $0 & ${SF_SELECTED}
	IntCmp $0 0 NoMSTest
		!insertmacro InstallTDNetRunner "Gallio_MSTest" "Microsoft.VisualStudio.QualityTools.UnitTestFramework" "5"
	NoMSTest:

	SectionGetFlags ${NUnitPluginSection} $0
	IntOp $0 $0 & ${SF_SELECTED}
	IntCmp $0 0 NoNUnit
		!insertmacro InstallTDNetRunner "Gallio_NUnit" "nunit.framework" "5"
	NoNUnit:

	SectionGetFlags ${XunitPluginSection} $0
	IntOp $0 $0 & ${SF_SELECTED}
	IntCmp $0 0 NoXunit
		!insertmacro InstallTDNetRunner "Gallio_Xunit" "xunit" "5"
	NoXunit:
SectionEnd

SectionGroupEnd

SectionGroup "Tools Integration"

Section "NCover Integration" NCoverSection
	; Set Section properties
	SetOverwrite on
	
	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\bin\NCover"
	File /r "${TARGETDIR}\bin\NCover\*"
SectionEnd

Section "TypeMock.Net Integration" TypeMockSection
	; Set Section properties
	SetOverwrite on
	
	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\bin\TypeMock"
	File /r "${TARGETDIR}\bin\TypeMock\*"
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

	SetOutPath "$INSTDIR\docs"
	File "${TARGETDIR}\docs\Gallio.chm"

	; Create Shortcuts
	CreateShortCut "$INSTDIR\Offline Documentation.lnk" "$INSTDIR\docs\Gallio.chm"
	CreateShortCut "$SMPROGRAMS\${APPNAME}\Offline Documentation.lnk" "$INSTDIR\docs\Gallio.chm"
SectionEnd
!endif

!ifndef MISSING_VS2005_HELP
Section "Visual Studio 2005 Help Docs" VS2005HelpSection
	; Set Section properties
	SetOverwrite on

	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\docs\vs2005"
	File "${TARGETDIR}\docs\vs2005\Gallio.Hx?"
	File "${TARGETDIR}\docs\vs2005\GallioCollection.*"

	SetOutPath "$INSTDIR\extras\H2Reg"
	File "${TARGETDIR}\extras\H2Reg\*"

	; Merge the collection
	ExecWait '"$INSTDIR\extras\H2Reg\H2Reg.exe" -r CmdFile="$INSTDIR\docs\vs2005\GallioCollection.h2reg.ini"'
SectionEnd
!endif
SectionGroupEnd
!endif

SectionGroup "Extras"

Section "CruiseControl.Net extensions" CCNetSection
	; Set Section properties
	SetOverwrite on
	
	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\extras"
	File /r "${TARGETDIR}\extras\CCNet"
SectionEnd

SectionGroupEnd

Section -FinishSection
	WriteRegStr SHCTX "Software\${APPNAME}" "" "$INSTDIR"
	WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayName" "${APPNAME}"
	WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "UninstallString" "$INSTDIR\uninstall.exe"
	WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "InstallLocation" "$INSTDIR"
	WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "HelpLink" "http://www.gallio.org/"
	WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "URLUpdateInfo" "http://www.gallio.org/"
	WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "URLInfoAbout" "http://www.gallio.org/"
	WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "URLInfoAbout" "http://www.gallio.org/"
	WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "NoModify" "1"
	WriteRegStr SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "NoRepair" "1"
	WriteUninstaller "$INSTDIR\uninstall.exe"
SectionEnd

;Uninstall section
Section Uninstall
	; Uninstall from TD.Net
	!insertmacro UninstallTDNetRunner "Gallio_MbUnit"
	!insertmacro UninstallTDNetRunner "Gallio_MbUnit2"
	!insertmacro UninstallTDNetRunner "Gallio_MSTest"
	!insertmacro UninstallTDNetRunner "Gallio_NUnit"
	!insertmacro UninstallTDNetRunner "Gallio_Xunit"

	; Uninstall from ReSharper
	!insertmacro UninstallReSharperRunner "v3.0" "vs8.0"
	!insertmacro UninstallReSharperRunner "v3.0" "vs9.0"

	; Uninstall from PowerShell
	DeleteRegKey SHCTX "SOFTWARE\Microsoft\PowerShell\1\PowerShellSnapIns\Gallio"

	; Uninstall the help collection
	IfFileExists "$INSTDIR\docs\vs2005\GallioCollection.h2reg.ini" 0 +2
		ExecWait '"$INSTDIR\extras\H2Reg\H2Reg.exe" -u CmdFile="$INSTDIR\docs\vs2005\GallioCollection.h2reg.ini"'

	; Uninstall the Visual Studio 2005 templates
	ClearErrors
	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\8.0" "InstallDir"
	IfErrors SkipMbUnitVS2005Templates

	Delete "$0\ItemTemplates\CSharp\Test\MbUnit.TestFixtureTemplate.CSharp.zip"
	Delete "$0\ProjectTemplates\CSharp\Test\MbUnit.TestProjectTemplate.CSharp.zip"

	Delete "$0\ItemTemplates\VisualBasic\Test\MbUnit.TestFixtureTemplate.VisualBasic.zip"
	Delete "$0\ProjectTemplates\VisualBasic\Test\MbUnit.TestProjectTemplate.VisualBasic.zip"

	; Run DevEnv /setup to unregister the templates.
	ExecWait '"$0\devenv.exe" /setup'

	SkipMbUnitVS2005Templates:

	; Uninstall the Visual Studio 2008 templates
	ClearErrors
	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\9.0" "InstallDir"
	IfErrors SkipMbUnitVS2008Templates

	Delete "$0\ItemTemplates\CSharp\Test\MbUnit.TestFixtureTemplate.CSharp.zip"
	Delete "$0\ProjectTemplates\CSharp\Test\MbUnit.TestProjectTemplate.CSharp.zip"
	Delete "$0\ProjectTemplates\CSharp\Test\MbUnit.MvcWebApplicationTestProjectTemplate.CSharp.zip"
	DeleteRegKey HKLM "SOFTWARE\Microsoft\VisualStudio\9.0\MVC\TestProjectTemplates\MbUnit\C#"

	Delete "$0\ItemTemplates\VisualBasic\Test\MbUnit.TestFixtureTemplate.VisualBasic.zip"
	Delete "$0\ProjectTemplates\VisualBasic\Test\MbUnit.TestProjectTemplate.VisualBasic.zip"
	Delete "$0\ProjectTemplates\VisualBasic\Test\MbUnit.MvcWebApplicationTestProjectTemplate.VisualBasic.zip"
	DeleteRegKey HKLM "SOFTWARE\Microsoft\VisualStudio\9.0\MVC\TestProjectTemplates\MbUnit\VB"

	; Run DevEnv /setup to unregister the templates.
	ExecWait '"$0\devenv.exe" /setup'

	SkipMbUnitVS2008Templates:

	; Delete Shortcuts
	RMDir /r "$SMPROGRAMS\${APPNAME}"

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
	!insertmacro MUI_DESCRIPTION_TEXT ${GallioSection} "Installs the Gallio test automation platform."

	!insertmacro MUI_DESCRIPTION_TEXT ${MbUnitSection} "Installs the MbUnit v3 framework components."
	!insertmacro MUI_DESCRIPTION_TEXT ${MbUnitPexSection} "Installs the MbUnit v3 Pex package."
	!insertmacro MUI_DESCRIPTION_TEXT ${MbUnitVS2005TemplatesSection} "Installs the MbUnit v3 Visual Studio 2005 templates."
	!insertmacro MUI_DESCRIPTION_TEXT ${MbUnitVS2008TemplatesSection} "Installs the MbUnit v3 Visual Studio 2008 templates."

	!insertmacro MUI_DESCRIPTION_TEXT ${MbUnit2PluginSection} "Installs the MbUnit v2 plugin.  Enables Gallio to run MbUnit v2 tests."
	!insertmacro MUI_DESCRIPTION_TEXT ${MSTestPluginSection} "Installs the MSTest plugin.  Enables Gallio to run MSTest tests."
	!insertmacro MUI_DESCRIPTION_TEXT ${NUnitPluginSection} "Installs the NUnit plugin.  Enables Gallio to run NUnit tests."
	!insertmacro MUI_DESCRIPTION_TEXT ${XunitPluginSection} "Installs the Xunit plugin.  Enables Gallio to run xUnit.Net tests."

	!insertmacro MUI_DESCRIPTION_TEXT ${EchoSection} "Installs the command-line test runner."
	!insertmacro MUI_DESCRIPTION_TEXT ${IcarusSection} "Installs the GUI-based test runner."
	!insertmacro MUI_DESCRIPTION_TEXT ${MSBuildTasksSection} "Installs the MSBuild tasks."
	!insertmacro MUI_DESCRIPTION_TEXT ${NAntTasksSection} "Installs the NAnt tasks."
	!insertmacro MUI_DESCRIPTION_TEXT ${PowerShellCommandsSection} "Installs the PowerShell commands."
	!insertmacro MUI_DESCRIPTION_TEXT ${ReSharperRunnerSection} "Installs the ReSharper v3 plug-in."
	!insertmacro MUI_DESCRIPTION_TEXT ${TDNetAddInSection} "Installs the TestDriven.Net add-in."

	!insertmacro MUI_DESCRIPTION_TEXT ${NCoverSection} "Provides integration with the NCover code coverage tool."
	!insertmacro MUI_DESCRIPTION_TEXT ${TypeMockSection} "Provides integration with the TypeMock.Net mock object framework."

	!insertmacro MUI_DESCRIPTION_TEXT ${CCNetSection} "Installs additional resources to assist with CruiseControl.Net integration."

	!ifndef MISSING_CHM_HELP
		!insertmacro MUI_DESCRIPTION_TEXT ${CHMHelpSection} "Installs the standalone help documentation CHM file."
	!endif

	!ifndef MISSING_VS2005_HELP
		!insertmacro MUI_DESCRIPTION_TEXT ${VS2005HelpSection} "Installs the integrated help documentation for Visual Studio 2005 access with F1."
	!endif
!insertmacro MUI_FUNCTION_DESCRIPTION_END

; Initialization code
Function .onInit
	; Extract install option pages.
	!insertmacro MUI_INSTALLOPTIONS_EXTRACT "AddRemovePage.ini"
	!insertmacro MUI_INSTALLOPTIONS_EXTRACT "UserSelectionPage.ini"

	; Set installation types.
	; Bits:
	;   1 - Full
	;   2 - Typical
	SectionSetInstTypes ${GallioSection} 7

	SectionSetInstTypes ${MbUnitSection} 3
	SectionSetInstTypes ${MbUnitPexSection} 1
	SectionSetInstTypes ${MbUnitVS2005TemplatesSection} 1
	SectionSetInstTypes ${MbUnitVS2008TemplatesSection} 1

	SectionSetInstTypes ${MbUnit2PluginSection} 1
	SectionSetInstTypes ${MSTestPluginSection} 1
	SectionSetInstTypes ${NUnitPluginSection} 1
	SectionSetInstTypes ${XunitPluginSection} 1

	SectionSetInstTypes ${EchoSection} 3
	SectionSetInstTypes ${IcarusSection} 3
	SectionSetInstTypes ${MSBuildTasksSection} 3
	SectionSetInstTypes ${NAntTasksSection} 3
	SectionSetInstTypes ${PowerShellCommandsSection} 3
	SectionSetInstTypes ${ReSharperRunnerSection} 3
	SectionSetInstTypes ${TDNetAddInSection} 3

	SectionSetInstTypes ${NCoverSection} 1
	SectionSetInstTypes ${TypeMockSection} 1

	SectionSetInstTypes ${CCNetSection} 1
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
		SectionSetText ${VS2005HelpSection} "The integrated help documentation for Visual Studio 2005 requires Visual Studio 2005 Standard Edition or higher to be installed.  Visual Studio 2005 Express Edition is not supported."
	!endif

	; Disable VS2005 templates if not installed.
	ClearErrors
	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\8.0" "InstallDir"
	IfErrors 0 +3
		SectionSetFlags ${MbUnitVS2005TemplatesSection} ${SF_RO}
		SectionSetText ${MbUnitVS2005TemplatesSection} "The MbUnit v3 Visual Studio 2005 templates require Visual Studio 2005 to be installed."

	; Disable VS2008 templates if not installed.
	ClearErrors
	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\9.0" "InstallDir"
	IfErrors 0 +3
		SectionSetFlags ${MbUnitVS2008TemplatesSection} ${SF_RO}
		SectionSetText ${MbUnitVS2008TemplatesSection} "The MbUnit v3 Visual Studio 2008 templates require Visual Studio 2008 to be installed."
FunctionEnd

; Uninstaller initialization code.
Function un.onInit
	ClearErrors
	ReadRegStr $0 HKCU "Software\${APPNAME}" ""
	IfErrors NotInstalledForUser
		SetShellVarContext current
		StrCpy $UserContext "current"
		StrCpy $INSTDIR $0
		Goto Installed
	NotInstalledForUser:

	ClearErrors
	ReadRegStr $0 HKLM "Software\${APPNAME}" ""
	IfErrors NotInstalledForSystem
		SetShellVarContext all
		StrCpy $UserContext "all"
		StrCpy $INSTDIR $0
		Goto Installed
	NotInstalledForSystem:

	MessageBox MB_OK "Gallio does not appear to be installed!  Abandoning uninstallation."
	Abort

	Installed:	
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
	ExecWait '"$OLD_INSTALL_DIR\uninstall.exe" /S _?=$OLD_INSTALL_DIR' $0
	DetailPrint "Uninstaller returned $0"

	IntCmp $INI_VALUE 1 Upgrade
	Quit

	Upgrade:
FunctionEnd

; User-selection page.
Function UserSelectionPageEnter
	!insertmacro MUI_HEADER_TEXT "Installation Options" "Please select for which users to install Gallio."
	!insertmacro MUI_INSTALLOPTIONS_DISPLAY "UserSelectionPage.ini"
FunctionEnd

Function UserSelectionPageLeave
	!insertmacro MUI_INSTALLOPTIONS_READ $INI_VALUE "UserSelectionPage.ini" "Field 2" "State"
	IntCmp $INI_VALUE 0 CurrentUserOnly
		SetShellVarContext all
		StrCpy $UserContext "all"
		Goto Done
	CurrentUserOnly:
		SetShellVarContext current
		StrCpy $UserContext "current"
	Done:
FunctionEnd


; Enforces a dependency from a source section on a target section.
!macro EnforceSelectionDependency SourceSection TargetSection
	SectionGetFlags ${SourceSection} $1

	SectionGetFlags ${TargetSection} $0
	IntOp $0 $0 & ${SF_SELECTED}
	IntCmp $0 0 +3
		IntOp $1 $1 & ${SF_RO_MASK}
		Goto +3
		IntOp $1 $1 & ${SF_SELECTED_MASK}
		IntOp $1 $1 | ${SF_RO}
	
	SectionSetFlags ${SourceSection} $1
!macroend

Function .onSelChange
	!insertmacro EnforceSelectionDependency ${MbUnitPexSection} ${MbUnitSection}
	!insertmacro EnforceSelectionDependency ${MbUnitVS2005TemplatesSection} ${MbUnitSection}
	!insertmacro EnforceSelectionDependency ${MbUnitVS2008TemplatesSection} ${MbUnitSection}
FunctionEnd

