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
using System.Reflection;
using System.IO;

namespace Gallio.AutoCAD.Plugin
{
    /// <summary>
    /// Provides access to the <c>GallioLoader</c> type via reflection.
    /// </summary>
    public class ReflectGallioLoader
    {
        private delegate object InitializeDelegate(string runtimePath);
        private delegate void SetupRuntimeDelegate();

        private readonly SetupRuntimeDelegate setupMethod;

        private ReflectGallioLoader(SetupRuntimeDelegate setupMethod)
        {
            if (setupMethod == null)
                throw new ArgumentNullException("setupMethod");
            this.setupMethod = setupMethod;
        }

        /// <summary>
        /// Loads the Gallio.Loader assembly and creates a <see cref="ReflectGallioLoader"/> instance.
        /// </summary>
        /// <param name="runtimePath">The runtime path.</param>
        /// <returns>A <see cref="ReflectGallioLoader"/> instance.</returns>
        /// <exception cref="FileNotFoundException">If the Gallio.Loader assembly can't be found.</exception>
        public static ReflectGallioLoader Initialize(string runtimePath)
        {
            if (!Directory.Exists(runtimePath))
                throw new DirectoryNotFoundException(string.Format("Directory not found: {0}.", runtimePath));

            var initializeDelegate = CreateInitializeDelegate(GallioLoaderAssemblyResolver.Resolve(runtimePath));
            var actualLoader = initializeDelegate(runtimePath);
            var setupRuntimeDelegate = CreateSetupRuntimeDelegate(actualLoader);

            return new ReflectGallioLoader(setupRuntimeDelegate);
        }

        private static InitializeDelegate CreateInitializeDelegate(Assembly loaderAssembly)
        {
            var loaderType = loaderAssembly.GetType("Gallio.Loader.GallioLoader", true);
            var initMethod = loaderType.GetMethod(
                "Initialize",
                BindingFlags.Public | BindingFlags.Static, null,
                new [] { typeof(string)}, null);

            return (InitializeDelegate) Delegate.CreateDelegate(typeof(InitializeDelegate), initMethod, true);
        }

        private static SetupRuntimeDelegate CreateSetupRuntimeDelegate(object actualLoader)
        {
            return (SetupRuntimeDelegate) Delegate.CreateDelegate(typeof(SetupRuntimeDelegate),
                actualLoader, "SetupRuntime", false, true);
        }

        /// <summary>
        /// Delegates to the <c>GallioLoader.SetupRuntime</c> method.
        /// </summary>
        public void SetupRuntime()
        {
            setupMethod();
        }
    }
}
