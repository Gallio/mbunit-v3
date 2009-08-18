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
using System.Reflection;
using System.Text;
using Gallio.Common.Platform;
using Gallio.Common.Reflection;
using Gallio.Model;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.Installer;
using Gallio.Runtime.Logging;
using Gallio.Runtime.Preferences;
using Gallio.Runtime.ProgressMonitoring;
using Microsoft.Win32;
using Registry=Microsoft.Win32.Registry;

namespace Gallio.TDNetRunner.Core
{
    public class TDNetRunnerInstaller : BaseInstaller
    {
        private readonly ITestFrameworkManager testFrameworkManager;
        private readonly IRegistry registry;
        private readonly ILogger logger;
        private readonly TDNetPreferenceManager preferenceManager;

        private const string RunnerRegKeyPrefix = "Gallio";
        private const string LocalMachineRegKey = @"Software\MutantDesign\TestDriven.NET\TestRunners";
        private const string LocalMachineRegKey32Bit = @"Software\Wow6432Node\MutantDesign\TestDriven.NET\TestRunners";

        public static readonly string InstallerId = "TDNetRunner.Installer";

        public TDNetRunnerInstaller(ITestFrameworkManager testFrameworkManager, IRegistry registry, ILogger logger,
            TDNetPreferenceManager preferenceManager)
        {
            this.testFrameworkManager = testFrameworkManager;
            this.registry = registry;
            this.logger = logger;
            this.preferenceManager = preferenceManager;
        }

        public override void Install(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Installing TestDriven.Net Runner", testFrameworkManager.TestFrameworkHandles.Count + 2))
            {
                // Remove old registrations.
                RemoveExistingRegistryKeys(progressMonitor);
                progressMonitor.Worked(1);

                // Register Icarus
                string icarusPath = FindIcarusPath();
                if (icarusPath != null)
                    InstallRegistryKeysForIcarus(icarusPath, progressMonitor);
                progressMonitor.Worked(1);

                // Register frameworks
                foreach (ComponentHandle<ITestFramework, TestFrameworkTraits> testFrameworkHandle in testFrameworkManager.TestFrameworkHandles)
                {
                    TestFrameworkTraits testFrameworkTraits = testFrameworkHandle.GetTraits();
                    TDNetRunnerInstallationMode installationMode = preferenceManager.GetInstallationModeForFramework(testFrameworkHandle.Id);

                    if (installationMode != TDNetRunnerInstallationMode.Disabled)
                    {
                        int priority = installationMode == TDNetRunnerInstallationMode.Default ? 25 : 5;
                        foreach (AssemblySignature frameworkAssembly in testFrameworkTraits.FrameworkAssemblies)
                        {
                            InstallRegistryKeysForFramework(testFrameworkTraits.Name, frameworkAssembly, priority,
                                progressMonitor);
                        }
                    }

                    progressMonitor.Worked(1);
                }
            }
        }

        public override void Uninstall(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Uninstalling TestDriven.Net Runner", 1))
            {
                RemoveExistingRegistryKeys(progressMonitor);
            }
        }

        private string FindIcarusPath()
        {
            IPluginDescriptor icarusPlugin = registry.Plugins["Gallio.Icarus"];
            if (icarusPlugin != null)
            {
                foreach (string searchPath in icarusPlugin.GetSearchPaths("Gallio.Icarus.exe"))
                    if (File.Exists(searchPath))
                        return searchPath;
            }

            return null;
        }

        private void RemoveExistingRegistryKeys(IProgressMonitor progressMonitor)
        {
            RemoveExistingRegistryKeys(progressMonitor, Registry.LocalMachine, LocalMachineRegKey);

            if (ProcessSupport.Is64BitProcess)
                RemoveExistingRegistryKeys(progressMonitor, Registry.LocalMachine, LocalMachineRegKey32Bit);
        }

        private void RemoveExistingRegistryKeys(IProgressMonitor progressMonitor, RegistryKey hiveKey, string rootKeyPath)
        {
            using (RegistryKey rootKey = hiveKey.OpenSubKey(rootKeyPath, true))
            {
                if (rootKey == null)
                    return;

                foreach (string subKeyName in rootKey.GetSubKeyNames())
                {
                    if (subKeyName.StartsWith(RunnerRegKeyPrefix))
                    {
                        string message = string.Format("Deleting TestDriven.Net runner registry key '{0}'.", subKeyName);

                        logger.Log(LogSeverity.Info, message);
                        progressMonitor.SetStatus(message);

                        rootKey.DeleteSubKeyTree(subKeyName);
                    }
                }
            }
        }

        private void InstallRegistryKeysForIcarus(string icarusPath, IProgressMonitor progressMonitor)
        {
            InstallRegistryKeysForIcarus(icarusPath, progressMonitor, Registry.LocalMachine, LocalMachineRegKey);

            if (ProcessSupport.Is64BitProcess)
                InstallRegistryKeysForIcarus(icarusPath, progressMonitor, Registry.LocalMachine, LocalMachineRegKey32Bit);
        }

        private void InstallRegistryKeysForIcarus(string icarusPath, IProgressMonitor progressMonitor, RegistryKey hiveKey, string rootKeyPath)
        {
            string subKeyName = string.Concat(rootKeyPath, @"\", RunnerRegKeyPrefix + "_Icarus"); // Note: 'Gallio_Icarus' is hardcoded in TDNet's config file.
            string message = "Adding TestDriven.Net runner registry key for Icarus.";

            logger.Log(LogSeverity.Info, message);
            progressMonitor.SetStatus(message);

            using (RegistryKey subKey = hiveKey.CreateSubKey(subKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                subKey.SetValue(null, "1");
                subKey.SetValue("Application", icarusPath);
            }
        }

        private void InstallRegistryKeysForFramework(string frameworkName, AssemblySignature frameworkAssembly, int priority, IProgressMonitor progressMonitor)
        {
            InstallRegistryKeysForFramework(frameworkName, frameworkAssembly, priority, progressMonitor, Registry.LocalMachine, LocalMachineRegKey);

            if (ProcessSupport.Is64BitProcess)
                InstallRegistryKeysForFramework(frameworkName, frameworkAssembly, priority, progressMonitor, Registry.LocalMachine, LocalMachineRegKey32Bit);
        }

        private void InstallRegistryKeysForFramework(string frameworkName, AssemblySignature frameworkAssembly, int priority, IProgressMonitor progressMonitor, RegistryKey hiveKey, string rootKeyPath)
        {
            string subKeyName = string.Concat(rootKeyPath, @"\", RunnerRegKeyPrefix, " - ", frameworkName, " (", frameworkAssembly, ")");
            string message = string.Format("Adding TestDriven.Net runner registry key for framework '{0}'.", frameworkName);

            logger.Log(LogSeverity.Info, message);
            progressMonitor.SetStatus(message);

            using (RegistryKey subKey = hiveKey.CreateSubKey(subKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                subKey.SetValue(null, priority.ToString());
                subKey.SetValue("AssemblyPath", AssemblyUtils.GetAssemblyLocalPath(GetType().Assembly));
                subKey.SetValue("TargetFrameworkAssemblyName", frameworkAssembly.ToString()); // n.b. TDNet supports version ranges in the same format we use
                subKey.SetValue("TypeName", "Gallio.TDNetRunner.GallioTestRunner");
                subKey.SetValue("TypeName_Resident", "Gallio.TDNetRunner.GallioResidentTestRunner");
            }
        }
    }
}
