using Gallio.Copy.Properties;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Copy.Commands
{
    public class ComparePluginsCommand : ICommand
    {
        private readonly PluginTreeModel sourcePlugins;
        private readonly PluginTreeModel targetPlugins;

        public ComparePluginsCommand(PluginTreeModel sourcePlugins, PluginTreeModel targetPlugins)
        {
            this.sourcePlugins = sourcePlugins;
            this.targetPlugins = targetPlugins;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask(Resources.ComparePluginsCommand_Comparing_source_and_target_plugins, 100))
            {
                
            }
        }
    }
}