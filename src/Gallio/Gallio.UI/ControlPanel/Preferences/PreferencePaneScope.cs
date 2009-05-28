using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.UI.ControlPanel.Preferences
{
    /// <summary>
    /// Specifies the scope of the changes effected by a preference pane.
    /// </summary>
    public enum PreferencePaneScope
    {
        /// <summary>
        /// Changes affect the current user only.
        /// </summary>
        User = 0,

        /// <summary>
        /// Changes affect all users of the machine.
        /// </summary>
        Machine = 1
    }
}
