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
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using Gallio.Common.Reflection;

namespace Gallio.Runtime.Loader
{
    /// <summary>
    /// Locates Gallio.Loader.dll.
    /// </summary>
    public static class GallioLoaderLocator
    {
        /// <summary>
        /// Gets the path of the Gallio.Loader assembly (unless it is in the GAC) so that it
        /// can be loaded by external code using <see cref="Assembly.LoadFrom(string)" />.
        /// </summary>
        /// <returns>The path of Gallio.Loader.dll, or null if Gallio.Loader.dll is
        /// installed in the GAC.</returns>
        /// <exception cref="RuntimeException">Thrown if the Gallio.Loader assembly is not available.</exception>
        [DebuggerNonUserCode]
        public static string GetGallioLoaderAssemblyPath()
        {
            var gallioAssemblyName = typeof(GallioLoaderLocator).Assembly.GetName();
            var gallioLoaderAssemblyName = new AssemblyName("Gallio.Loader")
            {
                Version = gallioAssemblyName.Version,
                CultureInfo = CultureInfo.InvariantCulture
            };
            gallioLoaderAssemblyName.SetPublicKeyToken(gallioAssemblyName.GetPublicKeyToken());

            try
            {
                Assembly loaderAssembly = Assembly.Load(gallioLoaderAssemblyName);
                if (loaderAssembly.GlobalAssemblyCache)
                    return null;

                return AssemblyUtils.GetFriendlyAssemblyLocation(loaderAssembly);
            }
            catch (Exception ex)
            {
                // Could not find the loader.
                throw new RuntimeException("Could not find the Gallio.Loader assembly.  It should be registered in the GAC or installed as a plugin.", ex);
            }
        }
    }
}
