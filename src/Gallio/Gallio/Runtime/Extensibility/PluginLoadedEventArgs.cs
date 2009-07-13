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
using System.IO;
using Gallio.Runtime.Extensibility.Schema;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// Details of a plugin that has just been loaded.
    /// </summary>
    public class PluginLoadedEventArgs : EventArgs
    {
        /// <summary>
        /// The loaded plugin.
        /// </summary>
        public Plugin Plugin { get; private set; }
        /// <summary>
        /// The base directory for the plugin.
        /// </summary>
        public DirectoryInfo BaseDirectory { get; private set; }
        /// <summary>
        /// The file path of the plugin.
        /// </summary>
        public string PluginFilePath { get; private set; }

        ///<summary>
        /// Constructor.
        ///</summary>
        ///<param name="plugin">The loaded plugin.</param>
        ///<param name="baseDirectory"></param>
        ///<param name="pluginFilePath"></param>
        public PluginLoadedEventArgs(Plugin plugin, DirectoryInfo baseDirectory, string pluginFilePath)
        {
            Plugin = plugin;
            BaseDirectory = baseDirectory;
            PluginFilePath = pluginFilePath;
        }
    }
}
