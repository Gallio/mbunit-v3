// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using MbUnit.Framework;

namespace MbUnit.Core.RuntimeSupport
{
    /// <summary>
    /// Resolves assemblies using hint paths and custom resolvers.
    /// </summary>
    public class DefaultAssemblyResolverManager : IAssemblyResolverManager
    {
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
        public IList<string> MbUnitDirectories
        {
            get
            {
                List<string> directories = new List<string>();

                Uri assemblyUri = new Uri(typeof(DefaultAssemblyResolverManager).Assembly.CodeBase);
                if (assemblyUri.IsFile)
                {
                    try
                    {
                        directories.Add(assemblyUri.LocalPath);
                    }
                    catch (InvalidOperationException)
                    {
                        // Ignore problems getting the local directory
                    }
                }

                return directories;
            }
        }

        /// <inheritdoc />
        public void AddHintDirectory(string hintDirectory)
        {
            if (hintDirectory == null)
                throw new ArgumentNullException("hintDirectory");

            hintDirectory = Path.GetFullPath(hintDirectory);

            if (!hintDirectories.Contains(hintDirectory))
                hintDirectories.Add(hintDirectory);
        }

        /// <inheritdoc />
        public void AddHintDirectoryContainingFile(string file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            string directory = Path.GetDirectoryName(file);
            if (directory == null || directory.Length == 0)
                directory = ".";

            AddHintDirectory(directory);
        }

        /// <inheritdoc />
        public void AddMbUnitDirectories()
        {
            foreach (string directory in MbUnitDirectories)
                AddHintDirectory(directory);
        }

        /// <inheritdoc />
        public void AddAssemblyResolver(IAssemblyResolver assemblyResolver)
        {
            if (assemblyResolver == null)
                throw new ArgumentNullException("assemblyResolver");

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
            String[] splitName = args.Name.Split(',');
            String displayName = splitName[0];
            Assembly assembly;

            // Try with current directory
            assembly = ResolveAssembly(Directory.GetCurrentDirectory(), displayName, reflectionOnly);
            if (assembly != null)
                return assembly;

            // Try with hint directories
            foreach (String directory in hintDirectories)
            {
                assembly = ResolveAssembly(directory, displayName, reflectionOnly);
                if (assembly != null)
                    return assembly;
            }

            return null;
        }

        private static Assembly ResolveAssembly(string directory, string file, bool reflectionOnly)
        {
            string assemblyName = Path.GetFullPath(Path.Combine(directory, file));

            if (File.Exists(assemblyName))
                return LoadFrom(assemblyName, reflectionOnly);

            if (File.Exists(assemblyName + ".dll"))
                return LoadFrom(assemblyName + ".dll", reflectionOnly);

            if (File.Exists(assemblyName + ".exe"))
                return LoadFrom(assemblyName + ".exe", reflectionOnly);

            return null;
        }

        private static Assembly LoadFrom(string file, bool reflectionOnly)
        {
            return reflectionOnly ? Assembly.ReflectionOnlyLoadFrom(file) : Assembly.LoadFrom(file);
        }
    }
}