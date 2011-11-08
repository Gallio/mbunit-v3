@echo off

set ReSharperVersion=v6.1

set pluginFolder="%APPDATA%\JetBrains\ReSharper\%ReSharperVersion%\vs10.0\Plugins\Gallio"
set externalAnnotationsFolder="%APPDATA%\JetBrains\ReSharper\%ReSharperVersion%\vs10.0\ExternalAnnotations\MbUnit"

if not exist %pluginFolder% md %pluginFolder%
if not exist %externalAnnotationsFolder% md %externalAnnotationsFolder%

xcopy Gallio.ReSharperRunner\bin\%ReSharperVersion%\Gallio.ReSharperRunner61.dll %pluginFolder% /y
xcopy Gallio.ReSharperRunner\bin\%ReSharperVersion%\Gallio.ReSharperRunner61.pdb %pluginFolder% /y
xcopy Gallio.ReSharperRunner\MbUnit.xml %externalAnnotationsFolder% /y