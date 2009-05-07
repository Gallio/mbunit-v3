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
using Gallio.Runner.Projects;

namespace Gallio.Icarus
{
    /// <summary>
    /// Paths used by Icarus.
    /// </summary>
    public static class Paths
    {
        /// <summary>
        /// The location of the Icarus app data folder.
        /// </summary>
        public static string IcarusAppDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            Path.Combine("Gallio", "Icarus"));
        
        /// <summary>
        /// The location of the default project used by Icarus.
        /// </summary>
        public static string DefaultProject = Path.Combine(IcarusAppDataFolder, "Icarus" + Project.Extension);
        
        /// <summary>
        /// The location of the Icarus settings (options) file
        /// </summary>
        public static string SettingsFile = Path.Combine(IcarusAppDataFolder, "Icarus" + Settings.Extension);

        /// <summary>
        /// The location of the file used to store the configuration of the tabs/windows in Icarus.
        /// </summary>
        public static string DockConfigFile = Path.Combine(IcarusAppDataFolder, @"DockPanel.config");
    }
}
