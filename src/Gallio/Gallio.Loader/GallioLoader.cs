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
using System.IO;
using System.Reflection;
using Microsoft.Win32;

namespace Gallio.Loader
{
    /// <summary>
    /// <para>
    /// The Gallio loader provides access to installed Gallio assemblies so that we can reference
    /// them even if they are not copied locally.
    /// </para>
    /// <para>
    /// We must avoid copying these assemblies because it is possible for multiple copies
    /// to be loaded in the same process simultaneously in different load context (Load / LoadFrom / LoadFile).
    /// When multiple copies of the same assembly are loaded their types are considered distinct
    /// and they cannot reliably exchange information with other components (like plugins).
    /// This problem was actually observed when two different Visual Studio add-ins installed
    /// in different locations were loaded at the same time.
    /// </para>
    /// <para>
    /// The Gallio loader may be used in situations where 3rd party integration mandates the
    /// installation of a Gallio-dependent assembly outside of the Gallio installation path.
    /// It is fairly typical for application plugin models.  
    /// </para>
    /// <para>
    /// The loader itself will typically be loaded from the GAC or copy-local as usual.
    /// It will then springboard into the locally installed copy of Gallio which is found
    /// by searching the registry.  It is also possible to specify the location
    /// of the Gallio installation explicitly instead.
    /// </para>
    /// <para>
    /// Once the loader has been initialized, all Gallio types should become accessible.
    /// In particular, the runtime can then be initialized.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Then install the assembly resolver in your program BEFORE referencing any
    /// Gallio types.  Beware that types are referenced implicitly by field and
    /// method declarations.  When external code calls directly into your assembly,
    /// you must ensure that all entry points are guarded with appropriate calls
    /// to the loader.  This may be facilitated somewhat by calling the loader
    /// from static initializers declared on the appropriate externally visible classes.
    /// </para>
    /// </remarks>
    public class GallioLoader
    {
        private static readonly string[] RootKeys = new[]
        {
            @"HKEY_CURRENT_USER\Software\Gallio.org\Gallio\3.0",
            @"HKEY_CURRENT_USER\Wow6432Node\Software\Gallio.org\Gallio\3.0",
            @"HKEY_LOCAL_MACHINE\Software\Gallio.org\Gallio\3.0",
            @"HKEY_LOCAL_MACHINE\Wow6432Node\Software\Gallio.org\Gallio\3.0"
        };

        private const string GallioLoaderBootstrapTypeFullName = "Gallio.Runtime.Loader.GallioLoaderBootstrap";
        private const string BootstrapInstallAssemblyResolverMethodName = "InstallAssemblyResolver";
        private const string BootstrapSetupRuntimeMethodName = "SetupRuntime";
        private const string BootstrapAddHintDirectoryMethodName = "AddHintDirectory";

        private static readonly object syncRoot = new object();

        private static GallioLoader instance;
        private static string defaultRuntimePath;
        
        private readonly string runtimePath;
        private readonly Assembly gallioAssembly;
        private readonly Type bootstrapType;

        private GallioLoader(string runtimePath)
        {
            this.runtimePath = runtimePath;

            gallioAssembly = Assembly.LoadFrom(GetGallioDllPath(runtimePath));
            bootstrapType = gallioAssembly.GetType(GallioLoaderBootstrapTypeFullName);
        }

        /// <summary>
        /// <para>
        /// Gets the Gallio loader instance, or null if not initialized.
        /// </para>
        /// </summary>
        /// <remarks>
        /// A null result does not mean that Gallio is not loaded or that its assemblies
        /// cannot be resolved.  It may simply means that this particular loading mechanism
        /// has not been used.
        /// </remarks>
        public static GallioLoader Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Gets the runtime path that will be used by default by the loader.
        /// The path is determined by looking up the location of the Gallio installation
        /// in the registry.  It may be overridden by setting the development runtime path key.
        /// </summary>
        /// <returns>The installed runtime path</returns>
        /// <exception cref="InvalidOperationException">Thrown if Gallio does not appear to be installed</exception>
        public static string GetDefaultRuntimePath()
        {
            lock (syncRoot)
            {
                if (defaultRuntimePath == null)
                {
                    defaultRuntimePath = FindRuntimePath();
                    if (defaultRuntimePath == null)
                        throw new InvalidOperationException(
                            "The Gallio runtime path could not be determined from the registry or by inspecting the location of other assemblies.");
                }
            }

            return defaultRuntimePath;
        }

        /// <summary>
        /// Initializes the Gallio loader (if not already initialized) and returns
        /// its singleton reference.
        /// </summary>
        /// <returns>The loader</returns>
        public static GallioLoader Initialize()
        {
            return Initialize(null);
        }

        /// <summary>
        /// Initializes the Gallio loader (if not already initialized) and returns
        /// its singleton reference.
        /// </summary>
        /// <param name="runtimePath">The runtime path from which to load Gallio,
        /// or null to determine it automatically</param>
        /// <returns>The loader</returns>
        public static GallioLoader Initialize(string runtimePath)
        {
            lock (syncRoot)
            {
                if (instance == null)
                {
                    if (runtimePath == null)
                        runtimePath = GetDefaultRuntimePath();

                    GallioLoader loader = new GallioLoader(runtimePath);
                    loader.InstallAssemblyResolver();

                    instance = loader;
                }

                return instance;
            }
        }

        /// <summary>
        /// Gets the Gallio runtime path.
        /// </summary>
        public string RuntimePath
        {
            get { return runtimePath; }
        }

        /// <summary>
        /// <para>
        /// Sets up the runtime with a default runtime setup using the loader's
        /// runtime path and a null logger.  Does nothing if the runtime has
        /// already been initialized.
        /// </para>
        /// <para>
        /// If you need more control over this behavior, call RuntimeBootstrap
        /// yourself.
        /// </para>
        /// </summary>
        public void SetupRuntime()
        {
            MethodInfo method = bootstrapType.GetMethod(BootstrapSetupRuntimeMethodName);
            method.Invoke(null, new object[] { runtimePath });
        }

        /// <summary>
        /// Adds a hint directory to the assembly resolver.
        /// </summary>
        /// <param name="path">The path of the hint directory to add</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is null</exception>
        public void AddHintDirectory(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            MethodInfo method = bootstrapType.GetMethod(BootstrapAddHintDirectoryMethodName);
            method.Invoke(null, new object[] { path });
        }

        private void InstallAssemblyResolver()
        {
            MethodInfo method = bootstrapType.GetMethod(BootstrapInstallAssemblyResolverMethodName);
            method.Invoke(null, new object[] { runtimePath });
        }


        private static string FindRuntimePath()
        {
            string runtimePath;

            string installationFolder = GetInstallationFolderFromRegistry();
            if (installationFolder != null)
            {
                runtimePath = Path.Combine(installationFolder, "bin");
                if (IsRuntimePathValid(runtimePath))
                    return runtimePath;
            }

            runtimePath = GetDevelopmentRuntimePathFromRegistry();
            if (runtimePath != null && IsRuntimePathValid(runtimePath))
                return runtimePath;

            try
            {
                return Path.GetDirectoryName(Assembly.Load("Gallio").Location);
            }
            catch
            {
            }

            return null;
        }

        private static string GetInstallationFolderFromRegistry()
        {
            foreach (string rootKey in RootKeys)
            {
                string value = Registry.GetValue(rootKey, @"InstallationFolder", null) as string;
                if (value != null)
                    return value;
            }

            return null;
        }

        private static string GetDevelopmentRuntimePathFromRegistry()
        {
            foreach (string rootKey in RootKeys)
            {
                string value = Registry.GetValue(rootKey, @"DevelopmentRuntimePath", null) as string;
                if (value != null)
                    return value;
            }

            return null;
        }

        private static bool IsRuntimePathValid(string runtimePath)
        {
            return File.Exists(GetGallioDllPath(runtimePath));
        }

        private static string GetGallioDllPath(string runtimePath)
        {
            return Path.Combine(runtimePath, "Gallio.dll");
        }
    }
}
