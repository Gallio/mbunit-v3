using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.VisualStudio.Toolkit.Actions
{
    /// <summary>
    /// Describes the status of the action.
    /// </summary>
    public enum ActionButtonStatus
    {
        /// <summary>
        /// Enables the action to be pressed.
        /// </summary>
        Enabled = 0,
        
        /// <summary>
        /// Hides the action.
        /// </summary>
        Invisible,

        /// <summary>
        /// Displays a pushed toggle (on) state.
        /// </summary>
        Latched,

        /// <summary>
        /// Opposite of <see cref="Latched"/>.  Displays an unpushed toggle (off) state.
        /// </summary>
        Ninched
    }
}
