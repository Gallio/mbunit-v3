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
using Gallio.Runtime.Logging;

namespace Gallio.Runtime.Loader
{
    /// <summary>
    /// Initializes the Gallio assembly loading a runtime policies.
    /// </summary>
    /// <remarks>
    /// This type is used only by the standalone Gallio.Loader assembly using late-binding.
    /// It should not be used by any other code.
    /// </remarks>
    internal static class GallioLoaderBootstrap
    {
        public static void InstallAssemblyResolver(string runtimePath)
        {
            if (runtimePath == null)
                throw new ArgumentNullException("runtimePath");

            AssemblyResolverBootstrap.Install(runtimePath);
        }

        public static void SetupRuntime(string runtimePath)
        {
            if (runtimePath == null)
                throw new ArgumentNullException("runtimePath");

            if (!RuntimeAccessor.IsInitialized)
            {
                RuntimeSetup setup = new RuntimeSetup();
                setup.RuntimePath = runtimePath;
                RuntimeBootstrap.Initialize(setup, NullLogger.Instance);
            }
        }

        public static void AddHintDirectory(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            AssemblyResolverBootstrap.AssemblyResolverManager.AddHintDirectory(path);
        }

        public static object Resolve(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            return RuntimeAccessor.ServiceLocator.Resolve(serviceType);
        }
    }
}
