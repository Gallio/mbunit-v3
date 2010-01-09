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
using System.Threading;
using Gallio.Common.Platform;

namespace Gallio.Common.Reflection.Impl
{
    /// <summary>
    /// Helpers for working with <see cref="IDebugSymbolResolver" />.
    /// </summary>
    public static class DebugSymbolUtils
    {
        private static IDebugSymbolResolver resolver;

        /// <summary>
        /// Gets the singleton debug symbol resolver used by these utilities.
        /// </summary>
        public static IDebugSymbolResolver Resolver
        {
            get
            {
                if (resolver == null)
                    Interlocked.CompareExchange(ref resolver, CreateResolver(false), null);
                return resolver;
            }
        }

        /// <summary>
        /// Creates a new debug symbol resolver appropriate for this platform.
        /// </summary>
        /// <param name="avoidLocks">If true, avoids taking a lock on the PDB files but may use more memory or storage.</param>
        /// <returns>The debug symbol resolver.</returns>
        public static IDebugSymbolResolver CreateResolver(bool avoidLocks)
        {
            if (DotNetRuntimeSupport.IsUsingMono)
                return new MonoDebugSymbolResolver(avoidLocks);

            return new ComDebugSymbolResolver(avoidLocks);
        }

        /// <summary>
        /// Gets the location of a source file that contains the declaration of a type, or
        /// unknown if not available.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The code location, or <see cref="CodeLocation.Unknown" /> if unknown.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null.</exception>
        public static CodeLocation GetSourceLocation(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            CodeLocation location = GuessSourceLocationForType(type, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
            if (location == CodeLocation.Unknown)
                location = GuessSourceLocationForType(type, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
            return location;
        }

        private static CodeLocation GuessSourceLocationForType(Type type, BindingFlags bindingFlags)
        {
            CodeLocation location = GuessSourceLocationForTypeFromItsMethods(type.GetConstructors(bindingFlags));
            if (location == CodeLocation.Unknown)
                location = GuessSourceLocationForTypeFromItsMethods(type.GetMethods(bindingFlags));
            return location;
        }

        private static CodeLocation GuessSourceLocationForTypeFromItsMethods(IEnumerable<MethodBase> methods)
        {
            foreach (MethodBase method in methods)
            {
                CodeLocation codeLocation = GetSourceLocation(method);
                if (codeLocation != CodeLocation.Unknown)
                    return new CodeLocation(codeLocation.Path, 0, 0);
            }

            return CodeLocation.Unknown;
        }

        /// <summary>
        /// Gets the location of a source file that contains the declaration of a method, or
        /// unknown if not available.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The source location, or <see cref="CodeLocation.Unknown" /> if unknown.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="method"/> is null.</exception>
        public static CodeLocation GetSourceLocation(MethodBase method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            Assembly assembly = method.DeclaringType.Assembly;
            string assemblyPath = AssemblyUtils.GetAssemblyLocation(assembly); // use the shadow-copied location if applicable
            if (assemblyPath == null)
                return CodeLocation.Unknown;

            if (AppDomain.CurrentDomain.ShadowCopyFiles)
            {
                try
                {
                    string pdbPath = Path.ChangeExtension(assemblyPath, @".pdb");
                    if (!File.Exists(pdbPath))
                    {
                        string originalAssemblyPath = AssemblyUtils.GetAssemblyLocalPath(assembly);
                        if (originalAssemblyPath == null)
                            return CodeLocation.Unknown;

                        string originalPdbPath = Path.ChangeExtension(originalAssemblyPath, @".pdb");
                        if (!File.Exists(originalPdbPath))
                            return CodeLocation.Unknown;

                        File.Copy(originalPdbPath, pdbPath);
                    }
                }
                catch (IOException)
                {
                    return CodeLocation.Unknown;
                }
            }

            return Resolver.GetSourceLocationForMethod(assemblyPath, method.MetadataToken);
        }
    }
}
