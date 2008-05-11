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
using System.Reflection;
using System.IO;
using Gallio.Runtime.Loader;

namespace Gallio.Runtime.Loader
{
    /// <summary>
    /// Resolves assemblies using hint paths and custom resolvers.
    /// </summary>
    public class DefaultAssemblyResolverManager : IAssemblyResolverManager
    {
        private static readonly string[] extensions = new string[] { ".dll", ".exe" };

        private List<string> hintDirectories;
        private List<IAssemblyResolver> assemblyResolvers;

        /// <summary>
        /// Initializes the assembly resolver manager.
        /// </summary>
        public DefaultAssemblyResolverManager()
        {
            hintDirectories = new List<string>();
            assemblyResolvers = new List<IAssemblyResolver>();

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomain_ReflectionOnlyAssemblyResolve;

            AddBuiltInHintDirectories();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= CurrentDomain_ReflectionOnlyAssemblyResolve;

            hintDirectories = null;
            assemblyResolvers = null;
        }

        /// <inheritdoc />
        public void AddHintDirectory(string hintDirectory)
        {
            if (hintDirectory == null)
                throw new ArgumentNullException(@"hintDirectory");

            hintDirectory = Path.GetFullPath(hintDirectory);

            if (!hintDirectories.Contains(hintDirectory))
                hintDirectories.Add(hintDirectory);
        }

        /// <inheritdoc />
        public void AddAssemblyResolver(IAssemblyResolver assemblyResolver)
        {
            if (assemblyResolver == null)
                throw new ArgumentNullException(@"assemblyResolver");

            assemblyResolvers.Add(assemblyResolver);
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return CustomAssemblyResolve(args, false);
        }

        private Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return CustomAssemblyResolve(args, true);
        }

        private Assembly CustomAssemblyResolve(ResolveEventArgs args, bool reflectionOnly)
        {
            // Try the custom handler chain
            foreach (IAssemblyResolver assemblyResolver in assemblyResolvers)
            {
                Assembly assembly = assemblyResolver.Resolve(args.Name, reflectionOnly);
                if (assembly != null)
                    return assembly;
            }

            // Try hint directories
            return RecursiveAssemblyResolve(args, reflectionOnly);
        }

        private Assembly RecursiveAssemblyResolve(ResolveEventArgs args, bool reflectionOnly)
        {
            // Note: The name passed into the resolve event has already had assembly binding policy applied
            //       to it.  So if there was a partial qualification or binding redirect then it will
            //       already be applied (no need to call AppDomain.ApplyPolicy).
            AssemblyName assemblyName = new AssemblyName(args.Name);
            Assembly assembly;

            // Try with current directory
            assembly = ResolveAssembly(assemblyName, Directory.GetCurrentDirectory(), reflectionOnly);
            if (assembly != null)
                return assembly;

            // Try with hint directories
            foreach (String directory in hintDirectories)
            {
                assembly = ResolveAssembly(assemblyName, directory, reflectionOnly);
                if (assembly != null)
                    return assembly;
            }

            return null;
        }

        private static Assembly ResolveAssembly(AssemblyName assemblyName, string directory, bool reflectionOnly)
        {
            foreach (string extension in extensions)
            {
                string file = assemblyName.Name + extension;
                string assemblyPath = Path.GetFullPath(Path.Combine(directory, file));

                if (File.Exists(assemblyPath))
                {
                    if (assemblyName.FullName != assemblyName.Name)
                    {
                        AssemblyName actualAssemblyName = AssemblyName.GetAssemblyName(assemblyPath);
                        if (assemblyName.FullName != actualAssemblyName.FullName)
                            continue;
                    }

                    return LoadFrom(assemblyPath, reflectionOnly);
                }
            }

            return null;
        }

        private static Assembly LoadFrom(string file, bool reflectionOnly)
        {
            return reflectionOnly ? Assembly.ReflectionOnlyLoadFrom(file) : Assembly.LoadFrom(file);
        }

        private void AddBuiltInHintDirectories()
        {
            AddHintDirectory(Path.GetDirectoryName(Reflection.AssemblyUtils.GetFriendlyAssemblyLocation(typeof(DefaultAssemblyResolverManager).Assembly)));
        }
    }
}