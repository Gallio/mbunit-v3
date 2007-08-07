using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Castle.Core;
using MbUnit.Core.Services.Runtime;
using MbUnit.Framework.Services.Runtime;

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
