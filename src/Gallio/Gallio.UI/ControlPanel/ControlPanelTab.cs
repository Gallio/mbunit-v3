using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Gallio.UI.ControlPanel
{
    /// <summary>
    /// Base class for components that present control panel tabs.
    /// </summary>
    public partial class ControlPanelTab : SettingsEditor
    {
        /// <summary>
        /// Creates a control panel tab.
        /// </summary>
        public ControlPanelTab()
        {
            InitializeComponent();
        }
    }
}
