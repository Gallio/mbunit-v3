@echo off

set ReSharperVersion=v7.0

set pluginFolder="%APPDATA%\JetBrains\ReSharper\%ReSharperVersion%\vs10.0\Plugins\Gallio"
set externalAnnotationsFolder="%APPDATA%\JetBrains\ReSharper\%ReSharperVersion%\vs10.0\ExternalAnnotations\MbUnit"

if not exist %pluginFolder% md %pluginFolder%
if not exist %externalAnnotationsFolder% md %externalAnnotationsFolder%

xcopy Gallio.ReSharperRunner\bin\%ReSharperVersion%\Gallio.ReSharperRunner70.dll %pluginFolder% /y
xcopy Gallio.ReSharperRunner\bin\%ReSharperVersion%\Gallio.ReSharperRunner70.pdb %pluginFolder% /y
xcopy Gallio.ReSharperRunner\MbUnit.xml %externalAnnotationsFolder% /y