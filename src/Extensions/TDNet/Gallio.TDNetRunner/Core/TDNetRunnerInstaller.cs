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
using Gallio.Runtime.ProgressMonitoring;
using Microsoft.Win32;
using Registry=Microsoft.Win32.Registry;

namespace Gallio.TDNetRunner.Core
{
    public class TDNetRunnerInstaller : BaseInstaller
    {
        private readonly ITestFrameworkManager frameworkManager;
        private readonly IRegistry registry;
        private readonly ILogger logger;

        private const string RunnerRegKeyPrefix = "Gallio";
        private const string LocalMachineRegKey = @"Software\MutantDesign\TestDriven.NET\TestRunners";
        private const string LocalMachineRegKey32Bit = @"Software\Wow6432Node\MutantDesign\TestDriven.NET\TestRunners";

        public static readonly string InstallerId = "TDNetRunner.Installer";

        public TDNetRunnerInstaller(ITestFrameworkManager frameworkManager, IRegistry registry, ILogger logger)
        {
            this.frameworkManager = frameworkManager;
            this.registry = registry;
            this.logger = logger;
        }

        public override void Install(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Installing TestDriven.Net Runner", frameworkManager.FrameworkHandles.Count + 2))
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
                foreach (ComponentHandle<ITestFramework, TestFrameworkTraits> frameworkHandles in frameworkManager.FrameworkHandles)
                {
                    TestFrameworkTraits frameworkTraits = frameworkHandles.GetTraits();
                    TDNetRunnerInstallationMode installationMode = GetInstallationModeForFramework(frameworkTraits.Id);

                    if (installationMode != TDNetRunnerInstallationMode.Disabled)
                    {
                        int priority = installationMode == TDNetRunnerInstallationMode.Default ? 25 : 5;
                        foreach (string frameworkAssemblyName in frameworkTraits.FrameworkAssemblyNames)
                        {
                            InstallRegistryKeysForFramework(frameworkTraits.Name, frameworkAssemblyName, priority,
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

        private TDNetRunnerInstallationMode GetInstallationModeForFramework(Guid frameworkId)
        {
            return TDNetRunnerInstallationMode.Default;
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
            string subKeyName = string.Concat(rootKeyPath, @"\", RunnerRegKeyPrefix, " - Icarus");
            string message = "Adding TestDriven.Net runner registry key for Icarus.";

            logger.Log(LogSeverity.Info, message);
            progressMonitor.SetStatus(message);

            using (RegistryKey subKey = hiveKey.CreateSubKey(subKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                subKey.SetValue(null, "1");
                subKey.SetValue("Application", icarusPath);
            }
        }

        private void InstallRegistryKeysForFramework(string frameworkName, string frameworkAssemblyName, int priority, IProgressMonitor progressMonitor)
        {
            InstallRegistryKeysForFramework(frameworkName, frameworkAssemblyName, priority, progressMonitor, Registry.LocalMachine, LocalMachineRegKey);

            if (ProcessSupport.Is64BitProcess)
                InstallRegistryKeysForFramework(frameworkName, frameworkAssemblyName, priority, progressMonitor, Registry.LocalMachine, LocalMachineRegKey32Bit);
        }

        private void InstallRegistryKeysForFramework(string frameworkName, string frameworkAssemblyName, int priority, IProgressMonitor progressMonitor, RegistryKey hiveKey, string rootKeyPath)
        {
            string subKeyName = string.Concat(rootKeyPath, @"\", RunnerRegKeyPrefix, " - ", frameworkName, " (", frameworkAssemblyName, ")");
            string message = string.Format("Adding TestDriven.Net runner registry key for framework '{0}'.", frameworkName);

            logger.Log(LogSeverity.Info, message);
            progressMonitor.SetStatus(message);

            using (RegistryKey subKey = hiveKey.CreateSubKey(subKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                subKey.SetValue(null, priority.ToString());
                subKey.SetValue("AssemblyPath", AssemblyUtils.GetAssemblyLocalPath(GetType().Assembly));
                subKey.SetValue("TargetFrameworkAssemblyName", frameworkAssemblyName);
                subKey.SetValue("TypeName", "Gallio.TDNetRunner.GallioTestRunner");
                subKey.SetValue("TypeName_Resident", "Gallio.TDNetRunner.GallioResidentTestRunner");
            }
        }
    }
}
