// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
