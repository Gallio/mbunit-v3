@echo off

set pluginFolder="C:\Program Files\JetBrains\ReSharper\v5.1\Bin\Plugins\Gallio"

md %pluginFolder%

xcopy Gallio.ReSharperRunner\bin\v5.1\Gallio.ReSharperRunner51.dll %pluginFolder% /y
xcopy Gallio.ReSharperRunner\bin\v5.1\Gallio.ReSharperRunner51.pdb %pluginFolder% /y

pause