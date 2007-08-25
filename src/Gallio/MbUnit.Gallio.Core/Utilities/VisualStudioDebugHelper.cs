// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Diagnostics;
using System.IO;
using System.Text;
using Castle.Core;
using MbUnit.Core.Runtime;
using MbUnit.Framework.Kernel.Runtime;

namespace MbUnit.Core.Utilities
{
    /// <summary>
    /// The Visual Studio debug helper is used when debugging MbUnit within
    /// Visual Studio.  It assumes a few things about the project file layout
    /// on the filesystem to ensure that plugins get loaded.
    /// </summary>
    internal static class VisualStudioDebugHelper
    {
        [Conditional("DEBUG")]
        public static void ConfigureRuntimeForDebugging(IAssemblyResolverManager assemblyResolverManager, WindsorRuntime runtime)
        {
            // Add plugin bin directories assuming the user is running inside Visual Studio.
            foreach (string mbunitDirectory in assemblyResolverManager.MbUnitDirectories)
            {
                DirectoryInfo pluginProjectDir = new DirectoryInfo(Path.GetFullPath(Path.Combine(mbunitDirectory, @"..\..\..\Plugins")));
                if (pluginProjectDir.Exists)
                {
                    foreach (DirectoryInfo projectDir in pluginProjectDir.GetDirectories())
                    {
                        string pluginBinDir = Path.Combine(projectDir.FullName, "bin");
                        if (!projectDir.FullName.Contains("Tests") && Directory.Exists(pluginBinDir))
                            runtime.AddPluginDirectory(pluginBinDir);
                    }
                }
            }
        }
    }
}
