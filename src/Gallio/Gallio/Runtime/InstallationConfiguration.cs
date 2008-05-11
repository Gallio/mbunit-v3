// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Microsoft.Win32;

namespace Gallio.Runtime
{
    /// <summary>
    /// Describes the configuration of a Gallio installation.
    /// </summary>
    [Serializable]
    public class InstallationConfiguration
    {
        private const string RootKey = @"Software\Gallio";
        private const string AdditionalPluginDirectoriesSubKey = @"AdditionalPluginDirectories";

        private readonly List<string> additionalPluginDirectories = new List<string>();

        /// <summary>
        /// Loads the configuration from the registry.
        /// </summary>
        /// <returns>The installed configuration</returns>
        public static InstallationConfiguration LoadFromRegistry()
        {
            InstallationConfiguration configuration = new InstallationConfiguration();
            configuration.LoadFromRegistry(Registry.LocalMachine);
            configuration.LoadFromRegistry(Registry.CurrentUser);
            return configuration;
        }

        /// <summary>
        /// Gets the list of additional plugin directories.
        /// </summary>
        public IList<string> AdditionalPluginDirectories
        {
            get { return additionalPluginDirectories; }
        }

        private void LoadFromRegistry(RegistryKey hive)
        {
            try
            {
                using (RegistryKey rootKey = hive.OpenSubKey(RootKey))
                {
                    if (rootKey == null)
                        return;

                    LoadAdditionalPluginDirectoriesFromRegistry(rootKey);
                }
            }
            catch
            {
            }
        }

        private void LoadAdditionalPluginDirectoriesFromRegistry(RegistryKey rootKey)
        {
            using (RegistryKey subKey = rootKey.OpenSubKey(AdditionalPluginDirectoriesSubKey))
            {
                if (subKey == null)
                    return;

                foreach (string name in subKey.GetValueNames())
                {
                    string value = subKey.GetValue(name) as string;
                    if (value != null)
                        additionalPluginDirectories.Add(value);
                }
            }
        }
    }
}