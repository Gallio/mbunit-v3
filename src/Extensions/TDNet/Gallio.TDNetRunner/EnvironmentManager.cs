// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Loader;

namespace Gallio.TDNetRunner
{
    /// <summary>
    /// Manages a shared instance of a Gallio runtime environment.
    /// </summary>
    internal static class EnvironmentManager
    {
        private static readonly object sharedEnvironmentSyncRoot = new object();
        private static IGallioRemoteEnvironment sharedEnvironment;

        /// <summary>
        /// Gets the shared remote environment.
        /// </summary>
        /// <returns>The remote environment.</returns>
        public static IGallioRemoteEnvironment GetSharedEnvironment()
        {
            lock (sharedEnvironmentSyncRoot)
            {
                if (sharedEnvironment == null)
                    sharedEnvironment = CreateSharedEnvironment();

                return sharedEnvironment;
            }
        }

        private static IGallioRemoteEnvironment CreateSharedEnvironment()
        {
            IGallioRemoteEnvironment environment = GallioLoader.CreateRemoteEnvironment();
            environment.Loader.AddHintDirectory(GetPluginPath());
            environment.Loader.SetupRuntime();
            return environment;
        }

        private static string GetPluginPath()
        {
            return Path.GetDirectoryName(GetAssemblyPath(typeof(EnvironmentManager).Assembly));
        }

        internal static string GetAssemblyPath(Assembly assembly)
        {
            return new Uri(assembly.CodeBase).LocalPath;
        }
    }
}
