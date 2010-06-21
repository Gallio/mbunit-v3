@echo off

set pluginFolder = "C:\Program Files\JetBrains\ReSharper\v5.1\Bin\Plugins\Gallio"

xcopy Gallio.ReSharperRunner\bin\v5.1\Gallio.Loader.dll %pluginFolder%
xcopy Gallio.ReSharperRunner\bin\v5.1\Gallio.Loader.pdb %pluginFolder%

xcopy Gallio.ReSharperRunner\bin\v5.1\Gallio.ReSharperRunner51.dll %pluginFolder%
xcopy Gallio.ReSharperRunner\bin\v5.1\Gallio.ReSharperRunner51.pdb %pluginFolder%

pause