using System;
using System.Collections.Generic;
using System.IO;
using Gallio.Common.Policies;
using Gallio.Runtime;

namespace Gallio.Copy
{
    internal class CopyController : ICopyController
    {
        public IList<string> Plugins
        {
            get
            {
                var plugins = new List<string>();
                foreach (var pluginDescriptor in RuntimeAccessor.Registry.Plugins)
                {
                    plugins.Add(pluginDescriptor.PluginId);
                }
                return plugins;
            }
        }

        public void CopyTo(string destinationFolder, IList<string> plugins)
        {
            foreach (var pluginId in plugins)
            {
                try
                {

                    var pluginDescriptor = RuntimeAccessor.Registry.Plugins[pluginId];

                    var pluginFolder = Path.Combine(destinationFolder, pluginId);
                    if (!Directory.Exists(pluginFolder))
                        Directory.CreateDirectory(pluginFolder);

                    // copy required assemblies
                    foreach (var assemblyBinding in pluginDescriptor.AssemblyBindings)
                    {
                        if (assemblyBinding.CodeBase == null)
                            return;

                        var assembly = Path.Combine(pluginFolder, 
                            Path.GetFileName(assemblyBinding.CodeBase.LocalPath));
                        File.Copy(assemblyBinding.CodeBase.LocalPath, assembly);
                    }
                }
                catch (Exception ex)
                {
                    UnhandledExceptionPolicy.Report("Error copying plugin", ex);
                }
            }
        }
    }
}
