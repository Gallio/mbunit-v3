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
using MbUnit.Core.Utilities;
using MbUnit.Framework;
using MbUnit.Framework.Services.Runtime;

namespace MbUnit.Core.Services.Runtime
{
    /// <summary>
    /// Resolves assemblies using hint paths and custom resolvers.
    /// </summary>
    public class DefaultAssemblyResolverManager : LongLivingMarshalByRefObject, IAssemblyResolverManager
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

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            hintDirectories = null;
            assemblyResolvers = null;
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
            AddHintDirectoryContainingFile(typeof(DefaultAssemblyResolverManager).Assembly.Location);
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
            if (args.Name.EndsWith("XmlSerializers"))
                return null;

            // Try the custom handler chain
            foreach (IAssemblyResolver assemblyResolver in assemblyResolvers)
            {
                Assembly assembly = assemblyResolver.Resolve(args.Name);
                if (assembly != null)
                    return assembly;
            }

            // Try hint directories
            return RecursiveAssemblyResolve(args);
        }

        private Assembly RecursiveAssemblyResolve(ResolveEventArgs args)
        {
            String[] splitName = args.Name.Split(',');
            String displayName = splitName[0];
            Assembly assembly;

            // Try with current directory
            assembly = ResolveAssembly(Directory.GetCurrentDirectory(), displayName);
            if (assembly != null)
                return assembly;

            // Try with hint directories
            foreach (String directory in hintDirectories)
            {
                assembly = ResolveAssembly(directory, displayName);
                if (assembly != null)
                    return assembly;
            }

            return null;
        }

        private static Assembly ResolveAssembly(string directory, string file)
        {
            string assemblyName = Path.GetFullPath(Path.Combine(directory, file));

            if (File.Exists(assemblyName))
                return Assembly.LoadFrom(assemblyName);

            if (File.Exists(assemblyName + ".dll"))
                return Assembly.LoadFrom(assemblyName + ".dll");

            if (File.Exists(assemblyName + ".exe"))
                return Assembly.LoadFrom(assemblyName + ".exe");

            return null;
        }
    }
}