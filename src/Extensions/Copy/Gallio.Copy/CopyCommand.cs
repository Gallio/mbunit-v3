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
        private readonly Dictionary<string, string> pluginLocations;

        public CopyCommand(string destinationFolder, IList<string> plugins, Dictionary<string, string> pluginLocations, 
            IFileSystem fileSystem, IUnhandledExceptionPolicy unhandledExceptionPolicy)
        {
            this.destinationFolder = destinationFolder;
            this.plugins = plugins;
            this.fileSystem = fileSystem;
            this.unhandledExceptionPolicy = unhandledExceptionPolicy;
            this.pluginLocations = pluginLocations;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
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
            using (progressMonitor.BeginTask(string.Format("Copying {0}", pluginId), 100))
            {
                try
                {
                    var pluginDescriptor = RuntimeAccessor.Registry.Plugins[pluginId];

                    string pluginFolder;
                    using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(10))
                        pluginFolder = CreatePluginFolder(pluginId, subProgressMonitor);

                    using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(10))
                        CopyPluginFile(pluginId, pluginFolder, subProgressMonitor);

                    using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(40))
                        CopyResources(pluginId, pluginFolder, subProgressMonitor);

                    using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(40))
                        CopyAssemblies(pluginFolder, pluginDescriptor, subProgressMonitor);
                }
                catch (Exception ex)
                {
                    unhandledExceptionPolicy.Report("Error copying plugin", ex);
                }
            }
        }

        private void CopyResources(string pluginId, string pluginFolder, IProgressMonitor progressMonitor)
        {
            var pluginLocation = pluginLocations[pluginId];

            var xmlDocument = new XmlDocument();
            xmlDocument.Load(pluginLocation);

            // find all nodes containing a plugin uri (plugin://)
            var resourceNodes = xmlDocument.SelectNodes("//*[starts-with(text(),'plugin://')]");
            if (resourceNodes == null || resourceNodes.Count == 0)
                return;

            using (progressMonitor.BeginTask("Copying resources", resourceNodes.Count))
            {
                foreach (XmlNode resourceNode in resourceNodes)
                {
                    CopyResource(resourceNode);
                    progressMonitor.Worked(1);
                }
            }

            // special case for Reports
            if (pluginId == "Gallio.Reports")
                CopyReportResources(pluginLocation, pluginFolder);
        }

        private void CopyReportResources(string pluginLocation, string pluginFolder)
        {
            var pluginBaseDirectory = Path.GetDirectoryName(pluginLocation);
            const string resources = "Resources";
            var resourceDirectory = Path.Combine(pluginBaseDirectory, resources);
            var targetFolder = Path.Combine(pluginFolder, resources);
            
            fileSystem.CopyDirectory(resourceDirectory, targetFolder);
        }

        private void CopyResource(XmlNode resourceNode)
        {
            var uri = new Uri(resourceNode.InnerText);
            string resourcePath;
            try
            {
                resourcePath = RuntimeAccessor.ResourceLocator.ResolveResourcePath(uri);
            }
            catch (RuntimeException)
            {
                // not a valid plugin:// resource
                return;
            }

            if (!fileSystem.FileExists(resourcePath))
                return;

            // construct destination path
            var relativeLocalPath = uri.AbsolutePath.Substring(1).Replace('/', Path.DirectorySeparatorChar);
            var destination = Path.Combine(Path.Combine(destinationFolder, uri.Host), relativeLocalPath);

            // don't bother if we've done it already
            if (fileSystem.FileExists(destination))
                return;

            fileSystem.CopyFile(resourcePath, destination, false);
        }

        private string CreatePluginFolder(string pluginId, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Creating plugin folder", 1))
            {
                var pluginFolder = Path.Combine(destinationFolder, pluginId);

                if (fileSystem.DirectoryExists(pluginFolder))
                    fileSystem.DeleteDirectory(pluginFolder, true);

                fileSystem.CreateDirectory(pluginFolder);
                return pluginFolder;
            }
        }

        private void CopyAssemblies(string pluginFolder, IPluginDescriptor pluginDescriptor, IProgressMonitor progressMonitor)
        {
            if (pluginDescriptor.AssemblyBindings.Count == 0)
                return;

            using (progressMonitor.BeginTask("Copying assemblies", pluginDescriptor.AssemblyBindings.Count))
            {
                foreach (var assemblyReference in pluginDescriptor.AssemblyBindings)
                {
                    if (assemblyReference.CodeBase == null)
                        continue;

                    var assembly = Path.Combine(pluginFolder,
                                                Path.GetFileName(assemblyReference.CodeBase.LocalPath));
                    fileSystem.CopyFile(assemblyReference.CodeBase.LocalPath, assembly, true);

                    progressMonitor.Worked(1);
                }
            }
        }

        private void CopyPluginFile(string pluginId, string pluginFolder, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Copying plugin file", 1))
            {
                var pluginLocation = pluginLocations[pluginId];
                var pluginDestination = Path.Combine(pluginFolder, Path.GetFileName(pluginLocation));
                fileSystem.CopyFile(pluginLocation, pluginDestination, false);
            }
        }
    }
}
