using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using EnvDTE;
using EnvDTE80;
using Gallio.VisualStudio.Toolkit;
using Gallio.VisualStudio.Toolkit.ToolWindows;

namespace Gallio.VisualStudio.Addin.UI
{
    public class TestConsole : ToolWindow
    {
        private static readonly Guid Guid = new Guid("E968F45E-4E94-4f9d-9023-06AE260872B2");

        public TestConsole(Shell shell)
            : base(shell, typeof(TestConsoleControl), Guid, "Gallio Test Console")
        {
        }
    }
}
