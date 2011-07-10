@echo off

set ReSharperVersion=v5.1

@if defined PROGRAMFILES(x86) (
set installFolder=%PROGRAMFILES(x86)%\JetBrains\ReSharper\%ReSharperVersion%\Bin
) else (
set installFolder=%PROGRAMFILES%\JetBrains\ReSharper\%ReSharperVersion%\Bin
)

set pluginFolder="%installFolder%\Plugins\Gallio"
set externalAnnotationsFolder="%installFolder%\ExternalAnnotations\MbUnit"

md %pluginFolder%
md %externalAnnotationsFolder%

xcopy Gallio.ReSharperRunner\bin\%ReSharperVersion%\Gallio.ReSharperRunner51.dll %pluginFolder% /y
xcopy Gallio.ReSharperRunner\bin\%ReSharperVersion%\Gallio.ReSharperRunner51.pdb %pluginFolder% /y
xcopy Gallio.ReSharperRunner\MbUnit.xml %externalAnnotationsFolder% /y