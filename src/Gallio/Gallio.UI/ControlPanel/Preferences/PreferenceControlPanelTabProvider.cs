using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Common;
using Gallio.Runtime.Extensibility;
using Gallio.UI.ControlPanel;

namespace Gallio.UI.ControlPanel.Preferences
{
    /// <summary>
    /// Provides the preferences control panel tab.
    /// </summary>
    public class PreferenceControlPanelTabProvider : IControlPanelTabProvider
    {
        private readonly ComponentHandle<IPreferencePaneProvider, PreferencePaneProviderTraits>[] preferencePaneProviderHandles;

        /// <summary>
        /// Creates a control panel tab provider for preference panes.
        /// </summary>
        /// <param name="preferencePaneProviderHandles">The preference page provider handles, not null</param>
        public PreferenceControlPanelTabProvider(ComponentHandle<IPreferencePaneProvider, PreferencePaneProviderTraits>[] preferencePaneProviderHandles)
        {
            this.preferencePaneProviderHandles = preferencePaneProviderHandles;
        }

        /// <inheritdoc />
        public ControlPanelTab CreateControlPanelTab()
        {
            var controlPanelTab = new PreferenceControlPanelTab();

            Array.Sort(preferencePaneProviderHandles, (x, y) => x.GetTraits().Order.CompareTo(y.GetTraits().Order));

            foreach (var preferencePaneProviderHandle in preferencePaneProviderHandles)
            {
                PreferencePaneProviderTraits traits = preferencePaneProviderHandle.GetTraits();

                controlPanelTab.AddPane(traits.Path, traits.Icon,
                    GetPreferencePaneFactory(preferencePaneProviderHandle));
            }

            return controlPanelTab;
        }

        private static Func<PreferencePane> GetPreferencePaneFactory(ComponentHandle<IPreferencePaneProvider, PreferencePaneProviderTraits> preferencePaneProviderHandle)
        {
            return () => preferencePaneProviderHandle.GetComponent().CreatePreferencePane();
        }
    }
}
