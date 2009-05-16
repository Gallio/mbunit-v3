using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Gallio.UI.ControlPanel;

namespace Gallio.UI.ControlPanel.Preferences
{
    /// <summary>
    /// Base class for components that present preference panels.
    /// </summary>
    public partial class PreferencePane : SettingsEditor
    {
        /// <summary>
        /// Creates a preference pane.
        /// </summary>
        public PreferencePane()
        {
            InitializeComponent();
        }
    }
}
