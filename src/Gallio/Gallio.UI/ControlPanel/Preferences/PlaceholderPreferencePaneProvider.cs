using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.UI.ControlPanel.Preferences
{
    /// <summary>
    /// A preference pane provider that is used as an empty placeholder for a
    /// non-leaf node in the preference pane tree.
    /// </summary>
    public class PlaceholderPreferencePaneProvider : IPreferencePaneProvider
    {
        /// <inheritdoc />
        public PreferencePane CreatePreferencePane()
        {
            return new PlaceholderPreferencePane();
        }
    }
}
