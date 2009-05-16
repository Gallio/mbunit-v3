using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.UI.ControlPanel.Preferences
{
    /// <summary>
    /// Provides the preference pane for the Gallio core services.
    /// </summary>
    public class GallioPreferencePaneProvider : IPreferencePaneProvider
    {
        /// <inheritdoc />
        public PreferencePane CreatePreferencePane()
        {
            return new GallioPreferencePane();
        }
    }
}
