The installation of the Gallio Cmdlet is automated by the end-user installer
package.  However, for development purposes you can install the Cmdlet using
the InstallUtil.exe utility in a normal command window as follows:

    %SystemRoot%\Microsoft.NET\Framework\v2.0.50727\InstallUtil [Path]\Gallio.PowerShellCommands.dll
    
After that open PowerShell and execute the following command:

    Add-PSSnapIn Gallio

Then you can run Gallio with:

    Run-Gallio [assemblies]

Be aware that each time you open PowerShell you will need to repeat
the last step. If you want to save the configuration you must export
the configuration:

    Export-Console [path]/gallio-enabled-powershell.psc1

Now you can start a "gallio-enabled" powershell console by using the
PSConsoleFile argument:

    [path]/powershell.exe -PSConsoleFile [path]/gallio-enabled-powershell.psc1
