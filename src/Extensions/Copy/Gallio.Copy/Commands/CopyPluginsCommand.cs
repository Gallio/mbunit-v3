using System;
using System.Collections.Generic;
using System.IO;
using Gallio.Common.IO;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Common.Policies;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Copy.Commands
{
    public class CopyPluginsCommand : ICommand
    {
        private readonly IList<IPluginDescriptor> sourcePlugins;
        private readonly string sourcePluginFolder;
        private readonly string targetPluginFolder;
        private readonly IUnhandledExceptionPolicy exceptionPolicy;
        private readonly IFileSystem fileSystem;

        public CopyPluginsCommand(IList<IPluginDescriptor> sourcePlugins, string sourcePluginFolder, string targetPluginFolder, 
            IUnhandledExceptionPolicy exceptionPolicy, IFileSystem fileSystem)
        {
            this.sourcePlugins = sourcePlugins;
            this.sourcePluginFolder = sourcePluginFolder;
            this.targetPluginFolder = targetPluginFolder;
            this.exceptionPolicy = exceptionPolicy;
            this.fileSystem = fileSystem;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            if (sourcePlugins.Count == 0)
                return;

            using (progressMonitor.BeginTask("Copying plugins", sourcePlugins.Count))
            {
                foreach (var plugin in sourcePlugins)
                {
                    using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
                        SafeCopy(plugin, subProgressMonitor);
                }
            }
        }

        private void SafeCopy(IPluginDescriptor plugin, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask(string.Format("Copying {0} plugin", plugin.PluginId), plugin.FilePaths.Count))
            {
                var sourceFolder = Path.Combine(sourcePluginFolder, plugin.RecommendedInstallationPath);
                var targetFolder = Path.Combine(targetPluginFolder, plugin.RecommendedInstallationPath);

                foreach (var file in plugin.FilePaths)
                {
                    var sourceFileInfo = new FileInfo(Path.Combine(sourceFolder, file));
                    var destination = new FileInfo(Path.Combine(targetFolder, file));

                    try
                    {
                        if (fileSystem.DirectoryExists(destination.DirectoryName) == false)
                            fileSystem.CreateDirectory(destination.DirectoryName);

                        fileSystem.CopyFile(sourceFileInfo.FullName, destination.FullName, true);
                    }
                    catch (Exception ex)
                    {
                        exceptionPolicy.Report(string.Format("Error copying file: {0}", file), ex);
                    }
                }
            }
        }
    }
}