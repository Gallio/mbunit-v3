@echo off

@if defined PROGRAMFILES(x86) (
set pluginFolder="%PROGRAMFILES(x86)%\JetBrains\ReSharper\v5.1\Bin\Plugins\Gallio"
) else (
set pluginFolder="%PROGRAMFILES%\JetBrains\ReSharper\v5.1\Bin\Plugins\Gallio"
)

md %pluginFolder%

xcopy Gallio.ReSharperRunner\bin\v5.1\Gallio.ReSharperRunner51.dll %pluginFolder% /y
xcopy Gallio.ReSharperRunner\bin\v5.1\Gallio.ReSharperRunner51.pdb %pluginFolder% /y

pause