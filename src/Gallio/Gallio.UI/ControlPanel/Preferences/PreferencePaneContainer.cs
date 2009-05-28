using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Gallio.UI.ControlPanel.Preferences
{
    /// <summary>
    /// Contains a preference pane and decorates it with a title.
    /// </summary>
    internal partial class PreferencePaneContainer : UserControl
    {
        public PreferencePaneContainer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the preference pane, or null if none.
        /// </summary>
        public PreferencePane PreferencePane
        {
            get { return (PreferencePane) outerTableLayoutPanel.GetControlFromPosition(0, 1); }
            set { outerTableLayoutPanel.Controls.Add(value, 0, 1); }
        }

        /// <summary>
        /// Gets or sets the preference pane title, or null if none.
        /// </summary>
        public string Title
        {
            get { return titleLabel.Text; }
            set { titleLabel.Text = value; }
        }
    }
}
