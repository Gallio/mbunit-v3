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
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;

namespace Gallio.Runtime.Hosting
{
    /// <summary>
    /// Utilities for working with AppDomains.
    /// </summary>
    public static class AppDomainUtils
    {
        /// <summary>
        /// Creates an AppDomain.
        /// </summary>
        /// <param name="applicationName">The application name for the new AppDomain, or null if none</param>
        /// <param name="applicationBaseDirectory">The application base directory for the new AppDomain, or null to use the current one</param>
        /// <param name="configurationFile">The configuration file for the new AppDomain, or null to use the current one</param>
        /// <param name="enableShadowCopy">If true, enables shadow copying within the AppDomain</param>
        /// <returns>The new AppDomain</returns>
        public static AppDomain CreateAppDomain(string applicationName, string applicationBaseDirectory, string configurationFile, bool enableShadowCopy)
        {
            AppDomainSetup appDomainSetup = new AppDomainSetup();

            appDomainSetup.ApplicationName = applicationName ?? string.Empty;
            appDomainSetup.ApplicationBase = applicationBaseDirectory ?? AppDomain.CurrentDomain.BaseDirectory;

            if (configurationFile != null)
            {
                // NOTE: We can also use AppDomainSetup.SetConfigurationBytes but it only applies to
                //       CLR-internal configuration settings such as the assembly binding policy.
                //       In order for other configuration mechanisms to operate correctly, we must
                //       use a real configuration file on disk instead.
                appDomainSetup.ConfigurationFile = configurationFile;
            }

            if (enableShadowCopy)
            {
                appDomainSetup.ShadowCopyFiles = @"true";
                appDomainSetup.ShadowCopyDirectories = null;
            }

            // TODO: Might need to be more careful about how the Evidence is derived.
            Evidence evidence = AppDomain.CurrentDomain.Evidence;
            PermissionSet defaultPermissionSet = new PermissionSet(PermissionState.Unrestricted);
            StrongName[] fullTrustAssemblies = new StrongName[0];
            return AppDomain.CreateDomain(appDomainSetup.ApplicationName, evidence, appDomainSetup, defaultPermissionSet, fullTrustAssemblies);
        }
    }
}
