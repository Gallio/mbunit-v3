using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Gallio.Common.IO;
using Gallio.Runtime;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Common.Policies;
using Gallio.UI.Progress;

namespace Gallio.Copy
{
    internal class CopyCommand : ICommand
    {
        private readonly string destinationFolder;
        private readonly IList<string> plugins;
        private readonly IFileSystem fileSystem;
        private readonly IUnhandledExceptionPolicy unhandledExceptionPolicy;
        private readonly IRegistry registry;

        public CopyCommand(string destinationFolder, IList<string> plugins, IRegistry registry, 
            IFileSystem fileSystem, IUnhandledExceptionPolicy unhandledExceptionPolicy)
        {
            this.destinationFolder = destinationFolder;
            this.plugins = plugins;
            this.fileSystem = fileSystem;
            this.unhandledExceptionPolicy = unhandledExceptionPolicy;
            this.registry = registry;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            if (plugins.Count == 0)
                return;

            using (progressMonitor.BeginTask("Copying plugins", plugins.Count))
            {
                foreach (var pluginId in plugins)
                {
                    using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
                        CopyPlugin(pluginId, subProgressMonitor);
                }
            }
        }

        private void CopyPlugin(string pluginId, IProgressMonitor progressMonitor)
        {
            try
            {
                using (progressMonitor.BeginTask(string.Format("Copying {0}", pluginId), 100))
                {
                    var pluginDescriptor = registry.Plugins[pluginId];
                    if (pluginDescriptor == null || pluginDescriptor.FilePaths.Count == 0)
                        return;

                    double workPerFile = 100.0 / pluginDescriptor.FilePaths.Count;
                    string relativeDestinationPluginFolder = pluginDescriptor.RecommendedInstallationPath ?? pluginId;

                    foreach (string filePath in pluginDescriptor.FilePaths)
                    {
                        var relativeDestinationFilePath = Path.Combine(relativeDestinationPluginFolder, filePath);
                        progressMonitor.SetStatus(relativeDestinationFilePath);

                        var absoluteSourceFilePath = Path.Combine(pluginDescriptor.BaseDirectory.FullName, filePath);
                        var absoluteDestinationFilePath = Path.Combine(destinationFolder, relativeDestinationFilePath);

                        var absoluteDestinationFileFolder = Path.GetDirectoryName(absoluteDestinationFilePath);
                        if (!fileSystem.DirectoryExists(absoluteDestinationFileFolder))
                            fileSystem.CreateDirectory(absoluteDestinationFileFolder);

                        fileSystem.CopyFile(absoluteSourceFilePath, absoluteDestinationFilePath, true);
                    }
                }
            }
            catch (Exception ex)
            {
                unhandledExceptionPolicy.Report(string.Format("Error copying plugin '{0}'.", pluginId), ex);
            }
        }
    }
}
