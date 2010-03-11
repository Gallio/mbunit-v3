using Gallio.Copy.Properties;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Copy.Commands
{
    public class UpdatePluginFolderCommand : ICommand
    {
        public PluginTreeModel PluginTreeModel { get; private set; }
        public string PluginFolder { get; private set; }

        public UpdatePluginFolderCommand(PluginTreeModel pluginTreeModel, 
            string pluginFolder)
        {
            PluginTreeModel = pluginTreeModel;
            PluginFolder = pluginFolder;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask(Resources.UpdatePluginFolderCommand_Updating_list_of_plugins, 100))
            {
                var registry = new Registry();
                var pluginLoader = SetupPluginLoader(progressMonitor);
                var pluginCatalog = PopulateCatalog(progressMonitor, pluginLoader);
                ApplyCatalogToRegistry(progressMonitor, pluginCatalog, registry);
                PluginTreeModel.UpdatePluginList(registry);
            }
        }

        private static void ApplyCatalogToRegistry(IProgressMonitor progressMonitor, 
            IPluginCatalog pluginCatalog, IRegistry registry)
        {
            using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(45))
            {
                subProgressMonitor.BeginTask(Resources.UpdatePluginFolderCommand_Applying_catalog_to_registry, 100);
                pluginCatalog.ApplyTo(registry);
            }
        }

        private PluginLoader SetupPluginLoader(IProgressMonitor progressMonitor)
        {
            var pluginLoader = new PluginLoader();

            using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(10))
            {
                subProgressMonitor.BeginTask(Resources.UpdatePluginFolderCommand_Adding_plugin_path, 100);
                pluginLoader.AddPluginPath(PluginFolder);
            }
            
            return pluginLoader;
        }

        private static PluginCatalog PopulateCatalog(IProgressMonitor progressMonitor, 
            IPluginLoader pluginLoader)
        {
            var pluginCatalog = new PluginCatalog();
            
            using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(45))
            {
                subProgressMonitor.BeginTask(Resources.UpdatePluginFolderCommand_Populating_catalog, 100);
                pluginLoader.PopulateCatalog(pluginCatalog);
            }
            
            return pluginCatalog;
        }
    }
}
