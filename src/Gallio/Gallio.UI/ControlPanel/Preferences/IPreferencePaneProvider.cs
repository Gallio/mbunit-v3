using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Runtime.Extensibility;

namespace Gallio.UI.ControlPanel.Preferences
{
    /// <summary>
    /// Provides a preference pane to be incorporated into the Gallio control panel.
    /// </summary>
    [Traits(typeof(PreferencePaneProviderTraits))]
    public interface IPreferencePaneProvider
    {
        /// <summary>
        /// Creates a preference pane to include in the control panel.
        /// </summary>
        /// <returns>The preference pane</returns>
        PreferencePane CreatePreferencePane();
    }
}
