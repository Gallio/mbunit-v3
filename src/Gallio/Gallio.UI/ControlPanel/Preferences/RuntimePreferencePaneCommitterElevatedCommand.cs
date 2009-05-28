using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Runtime;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime.Security;

namespace Gallio.UI.ControlPanel.Preferences
{
    /// <summary>
    /// Applies changes made by the <see cref="RuntimePreferencePane" />.
    /// </summary>
    public class RuntimePreferencePaneCommitterElevatedCommand : BaseElevatedCommand<InstallationConfiguration, object>
    {
        /// <inheritdoc />
        protected override object Execute(InstallationConfiguration arguments, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Saving runtime configuration changes.", 1))
            {
                arguments.SaveAdditionalPluginDirectoriesToRegistry();
                return null;
            }
        }
    }
}
