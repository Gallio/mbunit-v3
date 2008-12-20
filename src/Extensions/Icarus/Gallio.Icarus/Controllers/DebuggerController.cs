using System;
using EnvDTE;
using Gallio.VisualStudio.Interop;

namespace Gallio.Icarus.Controllers
{
    internal class DebuggerController
    {
        internal static void Attach()
        {
            VisualStudioSupport.SafeRunWithActiveVisualStudio(dte =>
            {
                foreach (Process process in dte.Debugger.LocalProcesses)
                {
                    if (!process.Name.Contains("Gallio.Host.exe"))
                        continue;
                    process.Attach();
                    break;
                }
            }, TimeSpan.FromSeconds(30));
        }
    }
}
