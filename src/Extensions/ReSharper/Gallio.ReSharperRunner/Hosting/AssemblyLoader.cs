// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using System.IO;
using System.Reflection;

namespace Gallio.ReSharperRunner.Hosting
{
    /// <summary>
    /// An assembly resolver.
    /// </summary>
    /// <remarks author="jeff">
    /// We can't use <see cref="JetBrains.Util.AssemblyResolver" /> because it compares
    /// <see cref="AssemblyName" /> objects for equality, which doesn't work.
    /// We can't use <see cref="JetBrains.ReSharper.TaskRunnerFramework.AssemblyLoader" /> because
    /// it opens assemblies for exclusive read access, which breaks during debugging
    /// because Visual Studio keeps a lock on the file.
    /// We can't use <see cref="Gallio.Hosting.DefaultAssemblyResolverManager" /> because
    /// we haven't yet loaded up the Gallio assemblies.
    /// *sigh*
    /// </remarks>
    internal class AssemblyLoader : IDisposable
    {
        private static readonly string[] extensions = new string[] { ".dll", ".exe" };

        private readonly List<string> paths;

        public AssemblyLoader()
        {
            paths = new List<string>();

            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
        }

        public void Dispose()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= AssemblyResolve;
        }

        public void RegisterPath(string path)
        {
            paths.Add(path);
        }

        private Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            String[] splitName = args.Name.Split(',');
            String displayName = splitName[0];

            foreach (string path in paths)
            {
                string assemblyFile = Path.GetFullPath(Path.Combine(path, displayName));

                foreach (string extension in extensions)
                {
                    if (File.Exists(assemblyFile + extension))
                        return Assembly.LoadFrom(assemblyFile + extension);
                }
            }

            return null;
        }
    }

}
