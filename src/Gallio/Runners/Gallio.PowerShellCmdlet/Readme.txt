In order to install the Cmdlet you must use the InstallUtil utility
in a normal command window (this will be automated by Gallio's installer
in the future):

    %SystemRoot%\Microsoft.NET\Framework\v2.0.50727\InstallUtil [Path]\Gallio.PowerShellCmdlet.dll
    
After that open PowerShell and execute the following command:

    Add-PSSnapIn GallioCmdlet

Be aware that each time you open PowerShell you will need to repeat
the last step. If you want to save the configuration you must export
the configuration:

    Export-Console [path]/gallio-enabled-powershell.psc1

Now you can start a "gallio-enabled" powershell console by using the
PSConsoleFile argument:

    [path]/powershell.exe -PSConsoleFile [path]/gallio-enabled-powershell.psc1