@echo off

@if defined PROGRAMFILES(x86) (
set installFolder=%PROGRAMFILES(x86)%\JetBrains\ReSharper\v5.1\Bin
) else (
set installFolder=%PROGRAMFILES%\JetBrains\ReSharper\v5.1\Bin
)

set pluginFolder="%installFolder%\Plugins\Gallio"
set externalAnnotationsFolder="%installFolder%\ExternalAnnotations\MbUnit"

md %pluginFolder%

xcopy Gallio.ReSharperRunner\bin\v5.1\Gallio.ReSharperRunner51.dll %pluginFolder% /y
xcopy Gallio.ReSharperRunner\bin\v5.1\Gallio.ReSharperRunner51.pdb %pluginFolder% /y
xcopy Gallio.ReSharperRunner\MbUnit.xml %externalAnnotationsFolder% /y