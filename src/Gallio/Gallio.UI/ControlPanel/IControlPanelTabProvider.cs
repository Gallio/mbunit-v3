using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Runtime.Extensibility;

namespace Gallio.UI.ControlPanel
{
    /// <summary>
    /// Provides a preference pane to be incorporated into the Gallio control panel.
    /// </summary>
    [Traits(typeof(ControlPanelTabProviderTraits))]
    public interface IControlPanelTabProvider
    {
        /// <summary>
        /// Creates a control panel tab to include in the control panel.
        /// </summary>
        /// <returns>The control panel tab</returns>
        ControlPanelTab CreateControlPanelTab();
    }
}
