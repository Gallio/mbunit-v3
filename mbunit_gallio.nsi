; Script generated with the Venis Install Wizard

; Define your application name
!define APPNAME "MbUnit Gallio"
!define APPNAMEANDVERSION "MbUnit Gallio Alpha 1"

; Main Install settings
Name "${APPNAMEANDVERSION}"
InstallDir "$PROGRAMFILES\MbUnit Gallio"
InstallDirRegKey HKLM "Software\${APPNAME}" ""
OutFile "mbunitgallio.exe"

; Modern interface settings
!include "MUI.nsh"

!define MUI_ABORTWARNING

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "MbUnit License.txt"
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

; Set languages (first is default language)
!insertmacro MUI_LANGUAGE "English"
!insertmacro MUI_RESERVEFILE_LANGDLL

Section "MbUnit Gallio" Section1

	; Set Section properties
	SetOverwrite on
	
	WriteRegStr HKCU "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnitGallio" "" "10"
  WriteRegStr HKCU "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnitGallio" "AssemblyPath" "$PROGRAMFILES\MbUnit Gallio\MbUnit.AddIn.TDNet.dll"
  WriteRegStr HKCU "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnitGallio" "TypeName" "MbUnit.AddIn.TDNet.MbUnitTestRunner"
  WriteRegStr HKCU "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnitGallio" "TargetFrameworkAssemblyName" "MbUnit.Framework"

	; Set Section Files and Shortcuts
	SetOutPath "$INSTDIR\"
	File "build\bin\ASL - Apache Software Foundation License.txt"
	File "build\bin\Castle.Core.dll"
	File "build\bin\Castle.DynamicProxy2.dll"
	File "build\bin\Castle.MicroKernel.dll"
	File "build\bin\Castle.Windsor.dll"
	File "build\bin\ICSharpCode.TextEditor.dll"
	File "build\bin\MbUnit License.txt"
	File "build\bin\MbUnit.AddIn.TDNet.dll"
	File "build\bin\MbUnit.Echo.exe"
	File "build\bin\MbUnit.Echo.exe.config"
	File "build\bin\MbUnit.Gallio.Core.dll"
	File "build\bin\MbUnit.Gallio.Core.plugin"
	File "build\bin\MbUnit.Gallio.Core.xml"
	File "build\bin\MbUnit.Gallio.Framework.dll"
	File "build\bin\MbUnit.Gallio.Framework.xml"
	File "build\bin\MbUnit.Tasks.MSBuild.dll"
	File "build\bin\MbUnit.Tasks.MSBuild.xml"
	File "build\bin\MbUnit.Tasks.NAnt.dll"
	File "build\bin\MbUnit.Tasks.NAnt.xml"
	File "build\bin\ZedGraph.dll"
	SetOutPath "$INSTDIR\MbUnit2\"
	File "build\bin\MbUnit2\MbUnit.Framework.2.0.dll"
	File "build\bin\MbUnit2\MbUnit.Framework.dll"
	File "build\bin\MbUnit2\MbUnit.Plugin.MbUnit2Adapter.dll"
	File "build\bin\MbUnit2\MbUnit.Plugin.MbUnit2Adapter.plugin"
	File "build\bin\MbUnit2\QuickGraph.Algorithms.dll"
	File "build\bin\MbUnit2\QuickGraph.dll"
	File "build\bin\MbUnit2\Refly.dll"
	File "build\bin\MbUnit2\TestFu.dll"
	SetOutPath "$INSTDIR\NUnit\"
	File "build\bin\NUnit\license.txt"
	File "build\bin\NUnit\MbUnit.Plugin.NUnitAdapter.dll"
	File "build\bin\NUnit\MbUnit.Plugin.NUnitAdapter.plugin"
	File "build\bin\NUnit\nunit.core.dll"
	File "build\bin\NUnit\nunit.core.extensions.dll"
	File "build\bin\NUnit\nunit.core.interfaces.dll"
	File "build\bin\NUnit\nunit.framework.dll"
	File "build\bin\NUnit\nunit.framework.extensions.dll"
	
	CreateDirectory "$SMPROGRAMS\MbUnit Gallio"
	
	CreateShortCut "$SMPROGRAMS\${APPNAME}\Uninstall.lnk" "$INSTDIR\uninstall.exe"
	CreateShortCut "$SMPROGRAMS\${APPNAME}\MbUnit Offline Documentation.lnk" "$INSTDIR\MbUnit Offline Documentation.url"
  CreateShortCut "$SMPROGRAMS\${APPNAME}\MbUnit Online Documentation.lnk" "$INSTDIR\MbUnit Online Documentation.url"
  CreateShortCut "$SMPROGRAMS\${APPNAME}\MbUnit Website.lnk" "$INSTDIR\MbUnit.url"

SectionEnd

Section -FinishSection

	WriteRegStr HKLM "Software\${APPNAME}" "" "$INSTDIR"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayName" "${APPNAME}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "UninstallString" "$INSTDIR\uninstall.exe"
	WriteUninstaller "$INSTDIR\uninstall.exe"

SectionEnd

; Modern install component descriptions
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
	!insertmacro MUI_DESCRIPTION_TEXT ${Section1} ""
!insertmacro MUI_FUNCTION_DESCRIPTION_END

;Uninstall section
Section Uninstall

	;Remove from registry...
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"
	DeleteRegKey HKLM "SOFTWARE\${APPNAME}"

	; Delete self
	Delete "$INSTDIR\uninstall.exe"

	; Delete Shortcuts
	Delete "$SMPROGRAMS\${APPNAME}\Uninstall.lnk"
	Delete "$SMPROGRAMS\${APPNAME}\MbUnit Website.lnk" 
  Delete "$SMPROGRAMS\${APPNAME}\MbUnit Online Documentation.lnk" 
  Delete "$SMPROGRAMS\${APPNAME}\MbUnit Offline Documentation.lnk" 

	; Clean up MbUnit Gallio
	Delete "$INSTDIR\ASL - Apache Software Foundation License.txt"
	Delete "$INSTDIR\Castle.Core.dll"
	Delete "$INSTDIR\Castle.DynamicProxy2.dll"
	Delete "$INSTDIR\Castle.MicroKernel.dll"
	Delete "$INSTDIR\Castle.Windsor.dll"
	Delete "$INSTDIR\ICSharpCode.TextEditor.dll"
	Delete "$INSTDIR\MbUnit License.txt"
	Delete "$INSTDIR\MbUnit.AddIn.TDNet.dll"
	Delete "$INSTDIR\MbUnit.Echo.exe"
	Delete "$INSTDIR\MbUnit.Echo.exe.config"
	Delete "$INSTDIR\MbUnit.Gallio.Core.dll"
	Delete "$INSTDIR\MbUnit.Gallio.Core.plugin"
	Delete "$INSTDIR\MbUnit.Gallio.Core.xml"
	Delete "$INSTDIR\MbUnit.Gallio.Framework.dll"
	Delete "$INSTDIR\MbUnit.Gallio.Framework.xml"
	Delete "$INSTDIR\MbUnit.Tasks.MSBuild.dll"
	Delete "$INSTDIR\MbUnit.Tasks.MSBuild.xml"
	Delete "$INSTDIR\MbUnit.Tasks.NAnt.dll"
	Delete "$INSTDIR\MbUnit.Tasks.NAnt.xml"
	Delete "$INSTDIR\ZedGraph.dll"
	Delete "$INSTDIR\MbUnit2\MbUnit.Framework.2.0.dll"
	Delete "$INSTDIR\MbUnit2\MbUnit.Framework.dll"
	Delete "$INSTDIR\MbUnit2\MbUnit.Plugin.MbUnit2Adapter.dll"
	Delete "$INSTDIR\MbUnit2\MbUnit.Plugin.MbUnit2Adapter.plugin"
	Delete "$INSTDIR\MbUnit2\QuickGraph.Algorithms.dll"
	Delete "$INSTDIR\MbUnit2\QuickGraph.dll"
	Delete "$INSTDIR\MbUnit2\Refly.dll"
	Delete "$INSTDIR\MbUnit2\TestFu.dll"
	Delete "$INSTDIR\NUnit\license.txt"
	Delete "$INSTDIR\NUnit\MbUnit.Plugin.NUnitAdapter.dll"
	Delete "$INSTDIR\NUnit\MbUnit.Plugin.NUnitAdapter.plugin"
	Delete "$INSTDIR\NUnit\nunit.core.dll"
	Delete "$INSTDIR\NUnit\nunit.core.extensions.dll"
	Delete "$INSTDIR\NUnit\nunit.core.interfaces.dll"
	Delete "$INSTDIR\NUnit\nunit.framework.dll"
	Delete "$INSTDIR\NUnit\nunit.framework.extensions.dll"
	
	DeleteRegKey HKCU "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\MbUnitGallio"

	; Remove remaining directories
	RMDir "$SMPROGRAMS\MbUnit Gallio"
	RMDir "$INSTDIR\NUnit\"
	RMDir "$INSTDIR\MbUnit2\"
	RMDir "$INSTDIR\"

SectionEnd

BrandingText "mbunit.com"

; eof