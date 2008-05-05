// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Xml.XPath;

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
    /// This type may be used in situations where 3rd party integration mandates the
    /// installation of a Gallio-dependent assembly outside of the Gallio installation path.
    /// It is fairly typical for application plugin models.  
    /// </para>
    /// <para>
    /// When using this mechanism, one must ensure that no code will be accessed which
    /// depends on Gallio types until the resolver has been installed.  Thus it may be
    /// necessary to create "shims" at all external entry points into the system to ensure
    /// that the resolver is configured.  If the types contain fields that are Gallio types,
    /// these fields will need to be encapsulated.
    /// </para>
    /// <para>
    /// An alternative strategy for dealing with this assembly referencing problem would be to
    /// install the main Gallio assemblies in the GAC.  However, that can create its own
    /// share of problems because it is possible for both the GAC'd assembly and its non-GAC'd
    /// sibling assembly to the loaded at the same time.
    /// </para>
    /// <para>
    /// In sum, this is an awful hack, but a rather useful one given the absence of other
    /// acceptable workarounds.
    /// </para>
    /// <example>
    /// <para>
    /// To use the assembly resolver, create a configuration file for your application's
    /// primary assembly that specifies the path of the main Gallio assembly like this:
    /// </para>
    /// <code>
    /// <![CDATA[
    /// <configuration>
    ///   <gallio>
    ///     <installation>
    ///       <path>Absolute-path-to-Gallio-installation-folder-binaries</path>
    ///     </installation>
    ///   </gallio>
    /// </configuration>
    /// ]]>
    /// </code>
    /// <para>
    /// Then install the assembly resolver in your program BEFORE referencing any
    /// Gallio types.  Beware that types are referenced implicitly by field and
    /// method declarations.  So you may need to rearrange your code somewhat.
    /// </para>
    /// </example>
    /// </summary>
    /// <remarks>
    /// <para>
    /// This hack should be replaced by a redistributable version-independent
    /// contract assembly someday.
    /// </para>
    /// </remarks>
    public class GallioLoader
    {
        private static readonly object syncRoot = new object();
        private static GallioLoader instance;
        
        private readonly string installationPath;

        private GallioLoader(string installationPath)
        {
            this.installationPath = installationPath;
        }

        /// <summary>
        /// Initializes the Gallio loader (if not already initialized) and returns
        /// its singleton reference.
        /// </summary>
        /// <param name="assembly">The primary assembly whose configuration file
        /// contains the Gallio installation path</param>
        /// <returns>The loader</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if the loader could not be initialized due to missing
        /// configuration information</exception>
        public static GallioLoader Initialize(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            lock (syncRoot)
            {
                if (instance == null)
                {
                    string installationPath = GetInstallationPath(assembly);
                    GallioLoader loader = new GallioLoader(installationPath);
                    loader.InstallAssemblyResolver();

                    instance = loader;
                }

                return instance;
            }
        }

        /// <summary>
        /// Gets the Gallio installation path.
        /// </summary>
        public string InstallationPath
        {
            get { return installationPath; }
        }

        private static string GetInstallationPath(Assembly assembly)
        {
            string installationPath;
            string configFilePath = new Uri(assembly.CodeBase).LocalPath + ".config";
            try
            {
                XPathDocument doc = new XPathDocument(configFilePath);
                XPathNavigator navigator = doc.CreateNavigator().SelectSingleNode("//gallio/installation/path");
                installationPath = navigator.Value;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(String.Format("Could not load Gallio configuration from file: '{0}'.", configFilePath), ex);
            }

            if (string.IsNullOrEmpty(installationPath))
                throw new InvalidOperationException(String.Format("Missing Gallio installation path parameter in configuration file: '{0}'.", configFilePath));

            return installationPath;
        }

        private void InstallAssemblyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
        }

        private Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            String[] splitName = args.Name.Split(',');
            String displayName = splitName[0];

            string assemblyFile = Path.GetFullPath(Path.Combine(installationPath, displayName)) + ".dll";
            if (File.Exists(assemblyFile))
                return Assembly.LoadFrom(assemblyFile);

            return null;
        }
    }
}
