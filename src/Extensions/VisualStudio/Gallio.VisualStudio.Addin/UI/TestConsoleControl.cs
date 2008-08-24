using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Gallio.VisualStudio.Toolkit.ToolWindows;

namespace Gallio.VisualStudio.Addin.UI
{
    [ComVisible(true)]
    public partial class TestConsoleControl : ToolWindowControl
    {
        public TestConsoleControl()
        {
            InitializeComponent();
        }
    }
}
