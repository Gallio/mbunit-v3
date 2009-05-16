using System;
using System.Collections.Generic;
using System.Text;
using Gallio.UI.ControlPanel.Preferences;

namespace Gallio.TDNetRunner.UI.Preferences
{
    public class TDNetPreferencePaneProvider : IPreferencePaneProvider
    {
        public PreferencePane CreatePreferencePane()
        {
            return new TDNetPreferencePane();
        }
    }
}
