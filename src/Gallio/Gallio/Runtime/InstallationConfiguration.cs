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
using System.IO;
using Microsoft.Win32;

namespace Gallio.Runtime
{
    /// <summary>
    /// Describes the configuration of a Gallio installation.
    /// </summary>
    [Serializable]
    public class InstallationConfiguration
    {
        private static readonly Pair<RegistryKey, string>[] RootKeys = new[]
        {
            new Pair<RegistryKey, string>(Registry.CurrentUser, @"Software\Gallio.org\Gallio\3.0"),
            new Pair<RegistryKey, string>(Registry.CurrentUser, @"Software\Wow6432Node\Gallio.org\Gallio\3.0"),
            new Pair<RegistryKey, string>(Registry.LocalMachine, @"Software\Gallio.org\Gallio\3.0"),
            new Pair<RegistryKey, string>(Registry.LocalMachine, @"Software\Wow6432Node\Gallio.org\Gallio\3.0")
        };

        private const string AdditionalPluginDirectoriesSubKey = @"AdditionalPluginDirectories";

        private string installedVersion;
        private string installationFolder;
        private string developmentRuntimePath;
        private readonly List<string> additionalPluginDirectories = new List<string>();

        /// <summary>
        /// Loads the configuration from the registry.
        /// </summary>
        /// <returns>The installed configuration</returns>
        public static InstallationConfiguration LoadFromRegistry()
        {
            InstallationConfiguration configuration = new InstallationConfiguration();

            foreach (Pair<RegistryKey, string> pair in RootKeys)
                configuration.LoadFromRegistry(pair.First, pair.Second);

            return configuration;
        }

        /// <summary>
        /// Get or sets the version that was installed, or null if Gallio is not installed.
        /// </summary>
        public string InstalledVersion
        {
            get { return installedVersion; }
            set { installedVersion = value; }
        }

        /// <summary>
        /// Get or sets the folder where Gallio was installed, or null if Gallio is not installed.
        /// </summary>
        public string InstallationFolder
        {
            get { return installationFolder; }
            set { installationFolder = value; }
        }

        /// <summary>
        /// <para>
        /// Gets or sets the runtime path of the development copy of Gallio to use
        /// for running tests, or null if not applicable.
        /// </para>
        /// <para>
        /// To facilitate development of Gallio, it is possible to redirect an out-of-process
        /// or isolated app-domain test runner to a different version of Gallio.  That way a
        /// standard installation of Gallio can be running in the IDE and then delegating to
        /// the development version when it actually needs to run tests.
        /// </para>
        /// <para>
        /// Of course, this mechanism can only hook into certain APIs like the test runner
        /// API.  Consequently it may still be necessary to reinstall Gallio and/or restart
        /// the IDE if the objective is to test changes made to the IDE add-ins themselves.
        /// </para>
        /// </summary>
        public string DevelopmentRuntimePath
        {
            get { return developmentRuntimePath; }
            set { developmentRuntimePath = value; }
        }

        /// <summary>
        /// Gets the list of additional plugin directories.
        /// </summary>
        public IList<string> AdditionalPluginDirectories
        {
            get { return additionalPluginDirectories; }
        }


        /// <summary>
        /// Returns true if <see cref="DevelopmentRuntimePath" /> is set and refers to a directory
        /// that exists.
        /// </summary>
        /// <returns>True if the development runtime path is valid</returns>
        public bool IsDevelopmentRuntimePathValid()
        {
            try
            {
                return !string.IsNullOrEmpty(developmentRuntimePath) && Directory.Exists(developmentRuntimePath);
            }
            catch (IOException)
            {
                return false;
            }
        }

        private void LoadFromRegistry(RegistryKey hive, string rootKeyName)
        {
            try
            {
                using (RegistryKey rootKey = hive.OpenSubKey(rootKeyName))
                {
                    if (rootKey == null)
                        return;

                    if (installedVersion == null)
                        installedVersion = rootKey.GetValue(@"Version", installedVersion) as string;

                    if (installationFolder == null)
                        installationFolder = rootKey.GetValue(@"InstallationFolder", installationFolder) as string;

                    if (developmentRuntimePath == null)
                        developmentRuntimePath = rootKey.GetValue(@"DevelopmentRuntimePath", developmentRuntimePath) as string;

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