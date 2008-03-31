!include "StrRep.nsh"
!include "Path.nsh"

; Check arguments.
!ifndef VERSION
	!error "The /DVersion=x.y.z.w argument must be specified."
!endif
!ifndef ROOTDIR
	!error "The /DRootDir=... argument must be specified."
!endif

; Common directories
!define BUILDDIR "${ROOTDIR}\build"
!define TARGETDIR "${BUILDDIR}\target"
!define RELEASEDIR "${BUILDDIR}\release"

; Define your application name
!define APPNAME "Gallio"
!define APPNAMEANDVERSION "Gallio v${VERSION}"

; Main Install settings
Name "${APPNAMEANDVERSION}"
InstallDir "$PROGRAMFILES\Gallio"
OutFile "${RELEASEDIR}\GallioBundle-${VERSION}-Setup.exe"

BrandingText "gallio.org"

ShowInstDetails show
ShowUnInstDetails show

InstType "Full"
InstType "MbUnit v3 Only"

; Constants
!define SF_SELECTED_MASK 254
!define SF_RO_MASK 239

; Modern interface settings
!define MUI_COMPONENTSPAGE_SMALLDESC ; Put description on bottom.
!define MUI_ABORTWARNING
!define MUI_WELCOMEFINISHPAGE_BITMAP "banner-left.bmp"
!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_BITMAP "banner-top.bmp"
!define MUI_ICON "installer.ico"
!define MUI_UNICON "uninstaller.ico"
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
!system 'if not exist "${TARGETDIR}\docs\vs\Gallio.HxS" echo !define MISSING_VS_HELP >> "${DETECT_TEMP}"'
!system 'if not exist "${TARGETDIR}\bin\MbUnit.Pex.dll" echo !define MISSING_MBUNIT_PEX_PACKAGE >> "${DETECT_TEMP}"'
!system 'if not exist "${TARGETDIR}\bin\Gallio.ReSharperRunner.dll" echo !define MISSING_RESHARPER_RUNNER >> "${DETECT_TEMP}"'
!include "${DETECT_TEMP}"
!delfile "${DETECT_TEMP}"

; Emit warnings if any components are missing
!ifdef MISSING_CHM_HELP
	!warning "Missing CHM file."
!endif

!ifdef MISSING_VS_HELP
	!warning "Missing Visual Studio help collection."
!endif

!ifdef MISSING_MBUNIT_PEX_PACKAGE
	!warning "Missing MbUnit Pex package."
!endif

!ifdef MISSING_RESHARPER_RUNNER
	!warning "Missing ReSharper runner."
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

	; Add the Gallio bin folder to the path
	${AddPath} "$UserContext" "$INSTDIR\bin"

	; Register the folder so that Visual Studio Add References can find it
	WriteRegStr SHCTX "SOFTWARE\Microsoft\.NETFramework\v2.0.50727\AssemblyFoldersEx\Gallio" "" "$INSTDIR\bin"

	; Create Shortcuts
	SetOutPath "$SMPROGRAMS\${APPNAME}"
	CreateDirectory "$SMPROGRAMS\${APPNAME}"
	CreateShortCut "$SMPROGRAMS\${APPNAME}\Browse Install Folder.lnk" "$INSTDIR\"
	CreateShortCut "$SMPROGRAMS\${APPNAME}\Uninstall.lnk" "$INSTDIR\uninstall.exe"	
	File "${TARGETDIR}\Gallio Website.url"
SectionEnd

SectionGroup "!MbUnit v3"
Section "!MbUnit v3 Framework" MbUnit3Section
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

!ifndef MISSING_MBUNIT_PEX_PACKAGE
Section "MbUnit v3 Pex Package" MbUnit3PexSection
	; Set Section properties
	SetOverwrite on
	
	SetOutPath "$INSTDIR\bin"
	File "${TARGETDIR}\bin\MbUnit.Pex.dll"
SectionEnd
!endif

Section "MbUnit v3 Visual Studio 2005 Templates" MbUnit3VS2005TemplatesSection
	; Set Section properties
	SetOverwrite on

	ClearErrors
	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\8.0" "InstallDir"
	IfErrors SkipVS2005Templates

        ; C# Item Templates
	SetOutPath "$0\ItemTemplates\CSharp\Test"
	File "${TARGETDIR}\extras\Templates\VS2005\ItemTemplates\CSharp\Test\MbUnit3.TestFixtureTemplate.CSharp.zip"

	; C# Project Templates
	SetOutPath "$0\ProjectTemplates\CSharp\Test"
	File "${TARGETDIR}\extras\Templates\VS2005\ProjectTemplates\CSharp\Test\MbUnit3.TestProjectTemplate.CSharp.zip"

        ; VB Item Templates
	SetOutPath "$0\ItemTemplates\VisualBasic\Test"
	File "${TARGETDIR}\extras\Templates\VS2005\ItemTemplates\VisualBasic\Test\MbUnit3.TestFixtureTemplate.VisualBasic.zip"

	; VB Project Templates
	SetOutPath "$0\ProjectTemplates\VisualBasic\Test"
	File "${TARGETDIR}\extras\Templates\VS2005\ProjectTemplates\VisualBasic\Test\MbUnit3.TestProjectTemplate.VisualBasic.zip"

	; Run DevEnv /setup to register the templates.
	ExecWait '"$0\devenv.exe" /setup'

	SkipVS2005Templates:
SectionEnd

Section "MbUnit v3 Visual Studio 2008 Templates" MbUnit3VS2008TemplatesSection
	; Set Section properties
	SetOverwrite on

	ClearErrors
	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\9.0" "InstallDir"
	IfErrors SkipVS2008Templates

        ; C# Item Templates
	SetOutPath "$0\ItemTemplates\CSharp\Test"
	File "${TARGETDIR}\extras\Templates\VS2008\ItemTemplates\CSharp\Test\MbUnit3.TestFixtureTemplate.CSharp.zip"

	; C# Project Templates
	SetOutPath "$0\ProjectTemplates\CSharp\Test"
	File "${TARGETDIR}\extras\Templates\VS2008\ProjectTemplates\CSharp\Test\MbUnit3.TestProjectTemplate.CSharp.zip"
	File "${TARGETDIR}\extras\Templates\VS2008\ProjectTemplates\CSharp\Test\MbUnit3.MvcWebApplicationTestProjectTemplate.CSharp.zip"

	WriteRegStr HKLM "SOFTWARE\Microsoft\VisualStudio\9.0\MVC\TestProjectTemplates\MbUnit3\C#" "Path" "CSharp\Test"
	WriteRegStr HKLM "SOFTWARE\Microsoft\VisualStudio\9.0\MVC\TestProjectTemplates\MbUnit3\C#" "Template" "MbUnit3.MvcWebApplicationTestProjectTemplate.CSharp.zip"
	WriteRegStr HKLM "SOFTWARE\Microsoft\VisualStudio\9.0\MVC\TestProjectTemplates\MbUnit3\C#" "TestFrameworkName" "MbUnit v3"
	WriteRegStr HKLM "SOFTWARE\Microsoft\VisualStudio\9.0\MVC\TestProjectTemplates\MbUnit3\C#" "AdditionalInfo" "http://www.mbunit.com/"

        ; VB Item Templates
	SetOutPath "$0\ItemTemplates\VisualBasic\Test"
	File "${TARGETDIR}\extras\Templates\VS2008\ItemTemplates\VisualBasic\Test\MbUnit3.TestFixtureTemplate.VisualBasic.zip"

	; VB Project Templates
	SetOutPath "$0\ProjectTemplates\VisualBasic\Test"
	File "${TARGETDIR}\extras\Templates\VS2008\ProjectTemplates\VisualBasic\Test\MbUnit3.TestProjectTemplate.VisualBasic.zip"
	File "${TARGETDIR}\extras\Templates\VS2008\ProjectTemplates\VisualBasic\Test\MbUnit3.MvcWebApplicationTestProjectTemplate.VisualBasic.zip"

	WriteRegStr HKLM "SOFTWARE\Microsoft\VisualStudio\9.0\MVC\TestProjectTemplates\MbUnit3\VB" "Path" "VisualBasic\Test"
	WriteRegStr HKLM "SOFTWARE\Microsoft\VisualStudio\9.0\MVC\TestProjectTemplates\MbUnit3\VB" "Template" "MbUnit3.MvcWebApplicationTestProjectTemplate.VisualBasic.zip"
	WriteRegStr HKLM "SOFTWARE\Microsoft\VisualStudio\9.0\MVC\TestProjectTemplates\MbUnit3\VB" "TestFrameworkName" "MbUnit v3"
	WriteRegStr HKLM "SOFTWARE\Microsoft\VisualStudio\9.0\MVC\TestProjectTemplates\MbUnit3\VB" "AdditionalInfo" "http://www.mbunit.com/"

	; Run DevEnv /setup to register the templates.
	ExecWait '"$0\devenv.exe" /setup'

	SkipVS2008Templates:
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
	File "${TARGETDIR}\bin\Aga.Controls.dll"
	File "${TARGETDIR}\bin\ICSharpCode.TextEditor.dll"
	File "${TARGETDIR}\bin\WeifenLuo.WinFormsUI.Docking.dll"
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

!ifndef MISSING_RESHARPER_RUNNER
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
!endif

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
	SectionGetFlags ${MbUnit3Section} $0
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

!ifndef MISSING_CHM_HELP | MISSING_VS_HELP
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

!ifndef MISSING_VS_HELP
Section "Visual Studio Help Docs" VSHelpSection
	; Set Section properties
	SetOverwrite on

	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\docs\vs"
	File "${TARGETDIR}\docs\vs\Gallio.Hx?"
	File "${TARGETDIR}\docs\vs\GallioCollection.*"

	SetOutPath "$INSTDIR\extras\H2Reg"
	File "${TARGETDIR}\extras\H2Reg\*"

	; Merge the collection
	ExecWait '"$INSTDIR\extras\H2Reg\H2Reg.exe" -r CmdFile="$INSTDIR\docs\vs\GallioCollection.h2reg.ini"'
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
	; Remove the Gallio bin folder to the path
	DetailPrint "Removing Gallio from path."
	${un.RemovePath} "$UserContext" "$INSTDIR\bin"

	; Uninstall from assembly folders
	DetailPrint "Removing Gallio from assembly folders."
	DeleteRegKey SHCTX "SOFTWARE\Microsoft\.NETFramework\v2.0.50727\AssemblyFoldersEx\Gallio"

	; Uninstall from TD.Net
	DetailPrint "Uninstalling TestDriven.Net runner."
	!insertmacro UninstallTDNetRunner "Gallio_MbUnit"
	!insertmacro UninstallTDNetRunner "Gallio_MbUnit2"
	!insertmacro UninstallTDNetRunner "Gallio_MSTest"
	!insertmacro UninstallTDNetRunner "Gallio_NUnit"
	!insertmacro UninstallTDNetRunner "Gallio_Xunit"

	; Uninstall from ReSharper
	!ifndef MISSING_RESHARPER_RUNNER
		DetailPrint "Uninstalling ReSharper runner."
		!insertmacro UninstallReSharperRunner "v3.1" "vs8.0"
		!insertmacro UninstallReSharperRunner "v3.1" "vs9.0"
	!endif

	; Uninstall from PowerShell
	DetailPrint "Uninstalling PowerShell runner."
	DeleteRegKey SHCTX "SOFTWARE\Microsoft\PowerShell\1\PowerShellSnapIns\Gallio"

	; Uninstall the help collection
	IfFileExists "$INSTDIR\docs\vs\GallioCollection.h2reg.ini" 0 +3
		DetailPrint "Uninstalling Visual Studio help collection."
		ExecWait '"$INSTDIR\extras\H2Reg\H2Reg.exe" -u CmdFile="$INSTDIR\docs\vs\GallioCollection.h2reg.ini"'

	; Uninstall the Visual Studio 2005 templates
	DetailPrint "Uninstalling Visual Studio 2005 templates."
	ClearErrors
	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\8.0" "InstallDir"
	IfErrors SkipVS2005Templates

	Delete "$0\ItemTemplates\CSharp\Test\MbUnit3.TestFixtureTemplate.CSharp.zip"
	Delete "$0\ProjectTemplates\CSharp\Test\MbUnit3.TestProjectTemplate.CSharp.zip"

	Delete "$0\ItemTemplates\VisualBasic\Test\MbUnit3.TestFixtureTemplate.VisualBasic.zip"
	Delete "$0\ProjectTemplates\VisualBasic\Test\MbUnit3.TestProjectTemplate.VisualBasic.zip"

	; Run DevEnv /setup to unregister the templates.
	ExecWait '"$0\devenv.exe" /setup'

	SkipVS2005Templates:

	; Uninstall the Visual Studio 2008 templates
	DetailPrint "Uninstalling Visual Studio 2008 templates."
	ClearErrors
	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\9.0" "InstallDir"
	IfErrors SkipVS2008Templates

	Delete "$0\ItemTemplates\CSharp\Test\MbUnit3.TestFixtureTemplate.CSharp.zip"
	Delete "$0\ProjectTemplates\CSharp\Test\MbUnit3.TestProjectTemplate.CSharp.zip"
	Delete "$0\ProjectTemplates\CSharp\Test\MbUnit3.MvcWebApplicationTestProjectTemplate.CSharp.zip"
	DeleteRegKey HKLM "SOFTWARE\Microsoft\VisualStudio\9.0\MVC\TestProjectTemplates\MbUnit3\C#"

	Delete "$0\ItemTemplates\VisualBasic\Test\MbUnit3.TestFixtureTemplate.VisualBasic.zip"
	Delete "$0\ProjectTemplates\VisualBasic\Test\MbUnit3.TestProjectTemplate.VisualBasic.zip"
	Delete "$0\ProjectTemplates\VisualBasic\Test\MbUnit3.MvcWebApplicationTestProjectTemplate.VisualBasic.zip"
	DeleteRegKey HKLM "SOFTWARE\Microsoft\VisualStudio\9.0\MVC\TestProjectTemplates\MbUnit3\VB"

	; Run DevEnv /setup to unregister the templates.
	ExecWait '"$0\devenv.exe" /setup'

	SkipVS2008Templates:

	; Delete Shortcuts
	DetailPrint "Uninstalling shortcuts."
	RMDir /r "$SMPROGRAMS\${APPNAME}"

	; Remove from registry...
	DetailPrint "Removing registry keys."
	DeleteRegKey SHCTX "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"
	DeleteRegKey SHCTX "SOFTWARE\${APPNAME}"

	; Delete self
	DetailPrint "Deleting files."
	Delete "$INSTDIR\uninstall.exe"

	; Remove all remaining contents
	RMDir /r "$INSTDIR"
SectionEnd

; Component descriptions
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
	!insertmacro MUI_DESCRIPTION_TEXT ${GallioSection} "Installs the Gallio test automation platform."

	!insertmacro MUI_DESCRIPTION_TEXT ${MbUnit3Section} "Installs the MbUnit v3 framework components."
	!ifndef MISSING_MBUNIT_PEX_PACKAGE
		!insertmacro MUI_DESCRIPTION_TEXT ${MbUnit3PexSection} "Installs the MbUnit v3 Pex package."
	!endif
	!insertmacro MUI_DESCRIPTION_TEXT ${MbUnit3VS2005TemplatesSection} "Installs the MbUnit v3 Visual Studio 2005 templates."
	!insertmacro MUI_DESCRIPTION_TEXT ${MbUnit3VS2008TemplatesSection} "Installs the MbUnit v3 Visual Studio 2008 templates."

	!insertmacro MUI_DESCRIPTION_TEXT ${MbUnit2PluginSection} "Installs the MbUnit v2 plugin.  Enables Gallio to run MbUnit v2 tests."
	!insertmacro MUI_DESCRIPTION_TEXT ${MSTestPluginSection} "Installs the MSTest plugin.  Enables Gallio to run MSTest tests."
	!insertmacro MUI_DESCRIPTION_TEXT ${NUnitPluginSection} "Installs the NUnit plugin.  Enables Gallio to run NUnit tests."
	!insertmacro MUI_DESCRIPTION_TEXT ${XunitPluginSection} "Installs the Xunit plugin.  Enables Gallio to run xUnit.Net tests."

	!insertmacro MUI_DESCRIPTION_TEXT ${EchoSection} "Installs the command-line test runner."
	!insertmacro MUI_DESCRIPTION_TEXT ${IcarusSection} "Installs the GUI-based test runner."
	!insertmacro MUI_DESCRIPTION_TEXT ${MSBuildTasksSection} "Installs the MSBuild tasks."
	!insertmacro MUI_DESCRIPTION_TEXT ${NAntTasksSection} "Installs the NAnt tasks."
	!insertmacro MUI_DESCRIPTION_TEXT ${PowerShellCommandsSection} "Installs the PowerShell commands."
	!ifndef MISSING_RESHARPER_RUNNER
		!insertmacro MUI_DESCRIPTION_TEXT ${ReSharperRunnerSection} "Installs the ReSharper v3.1 plug-in."
	!endif
	!insertmacro MUI_DESCRIPTION_TEXT ${TDNetAddInSection} "Installs the TestDriven.Net add-in."

	!insertmacro MUI_DESCRIPTION_TEXT ${NCoverSection} "Provides integration with the NCover code coverage tool."
	!insertmacro MUI_DESCRIPTION_TEXT ${TypeMockSection} "Provides integration with the TypeMock.Net mock object framework."

	!insertmacro MUI_DESCRIPTION_TEXT ${CCNetSection} "Installs additional resources to assist with CruiseControl.Net integration."

	!ifndef MISSING_CHM_HELP
		!insertmacro MUI_DESCRIPTION_TEXT ${CHMHelpSection} "Installs the standalone help documentation CHM file."
	!endif

	!ifndef MISSING_VS_HELP
		!insertmacro MUI_DESCRIPTION_TEXT ${VSHelpSection} "Installs the integrated documentation for Visual Studio 2005 or Visual Studio 2008 access with F1 Help."
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
	;   2 - MbUnit v3 Only
	SectionSetInstTypes ${GallioSection} 3

	SectionSetInstTypes ${MbUnit3Section} 3
	!ifndef MISSING_MBUNIT_PEX_PACKAGE
		SectionSetInstTypes ${MbUnit3PexSection} 3
	!endif
	SectionSetInstTypes ${MbUnit3VS2005TemplatesSection} 3
	SectionSetInstTypes ${MbUnit3VS2008TemplatesSection} 3

	SectionSetInstTypes ${MbUnit2PluginSection} 1
	SectionSetInstTypes ${MSTestPluginSection} 1
	SectionSetInstTypes ${NUnitPluginSection} 1
	SectionSetInstTypes ${XunitPluginSection} 1

	SectionSetInstTypes ${EchoSection} 3
	SectionSetInstTypes ${IcarusSection} 3
	SectionSetInstTypes ${MSBuildTasksSection} 3
	SectionSetInstTypes ${NAntTasksSection} 3
	SectionSetInstTypes ${PowerShellCommandsSection} 3
	!ifndef MISSING_RESHARPER_RUNNER
		SectionSetInstTypes ${ReSharperRunnerSection} 3
	!endif
	SectionSetInstTypes ${TDNetAddInSection} 3

	SectionSetInstTypes ${NCoverSection} 3
	SectionSetInstTypes ${TypeMockSection} 3

	SectionSetInstTypes ${CCNetSection} 3
	!ifndef MISSING_CHM_HELP
		SectionSetInstTypes ${CHMHelpSection} 3
	!endif
	!ifndef MISSING_VS_HELP
		SectionSetInstTypes ${VSHelpSection} 3
	!endif

	SetCurInstType 0

	; Disable Visual Studio help section if not installed.
	!ifndef MISSING_VS_HELP
	ClearErrors
	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\8.0" "InstallDir"
	IfErrors 0 IncludeVSHelp
	ClearErrors
	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\9.0" "InstallDir"
	IfErrors 0 IncludeVSHelp
		SectionSetFlags ${VSHelpSection} ${SF_RO}
		SectionSetText ${VSHelpSection} "The integrated documentation for Visual Studio requires Visual Studio 2005 or Visual Studio 2008 to be installed."
	IncludeVSHelp:
	!endif

	; Disable VS2005 templates if not installed.
	ClearErrors
	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\8.0" "InstallDir"
	IfErrors 0 +3
		SectionSetFlags ${MbUnit3VS2005TemplatesSection} ${SF_RO}
		SectionSetText ${MbUnit3VS2005TemplatesSection} "The MbUnit v3 Visual Studio 2005 templates require Visual Studio 2005 to be installed."

	; Disable VS2008 templates if not installed.
	ClearErrors
	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\9.0" "InstallDir"
	IfErrors 0 +3
		SectionSetFlags ${MbUnit3VS2008TemplatesSection} ${SF_RO}
		SectionSetText ${MbUnit3VS2008TemplatesSection} "The MbUnit v3 Visual Studio 2008 templates require Visual Studio 2008 to be installed."
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

	; Note: We don't uninstall silently anymore because it takes too
	;       long and it sucks not to get any feedback during the process.
	ExecWait '"$OLD_INSTALL_DIR\uninstall.exe" _?=$OLD_INSTALL_DIR' $0
	IntCmp $0 0 Ok
	MessageBox MB_OK "Cannot proceed because the old version was not successfully uninstalled."
	Abort

	Ok:
	IntCmp $INI_VALUE 1 Upgrade
	Quit

	Upgrade:
	BringToFront
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
	!ifndef MISSING_MBUNIT_PEX_PACKAGE
		!insertmacro EnforceSelectionDependency ${MbUnit3PexSection} ${MbUnit3Section}
	!endif
	!insertmacro EnforceSelectionDependency ${MbUnit3VS2005TemplatesSection} ${MbUnit3Section}
	!insertmacro EnforceSelectionDependency ${MbUnit3VS2008TemplatesSection} ${MbUnit3Section}
FunctionEnd

