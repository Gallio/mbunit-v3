using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Gallio.VisualStudio.Toolkit.ToolWindows
{
    /// <summary>
    /// Abstract base control class for tool windows.
    /// </summary>
    [ComVisible(true)]
    public class ToolWindowControl : UserControl
    {
        private ToolWindow toolWindow;

        /// <summary>
        /// Gets the tool window.
        /// </summary>
        public ToolWindow ToolWindow
        {
            get { return toolWindow; }
            internal set { toolWindow = value; }
        }
    }
}
