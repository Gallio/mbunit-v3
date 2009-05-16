using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Gallio.UI.ControlPanel
{
    /// <summary>
    /// Base class for user controls for editing settings with deferred application.
    /// </summary>
    public class SettingsEditor : UserControl
    {
        /// <summary>
        /// Event raised when the settings have changed.
        /// </summary>
        public event EventHandler SettingsChanged;

        /// <summary>
        /// Gets or sets whether the settings have changed.
        /// </summary>
        public bool HasSettingsChanges { get; protected set; }

        /// <summary>
        /// Applies settings changes.
        /// </summary>
        /// <remarks>
        /// The default implementation simply sets <see cref="HasSettingsChanges" /> to false.
        /// </remarks>
        public virtual void ApplySettingsChanges()
        {
            HasSettingsChanges = false;
        }

        /// <summary>
        /// Sets <see cref="HasSettingsChanges" /> to true and raises the <see cref="SettingsChanged" /> event
        /// on the first change.
        /// </summary>
        /// <param name="e">The event arguments</param>
        protected virtual void OnSettingsChanged(EventArgs e)
        {
            if (!HasSettingsChanges)
            {
                HasSettingsChanges = true;

                if (SettingsChanged != null)
                    SettingsChanged(this, e);
            }
        }
    }
}
