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
using System.Reflection;
using System.Threading;
using Gallio.Runtime.Remoting;
using Gallio.Reflection;

namespace Gallio.Runtime.Hosting
{
    /// <summary>
    /// Attaches to a <see cref="IHost" /> to provide assembly
    /// resolution services.  Installs an <see cref="AppDomain.AssemblyResolve" /> hook
    /// that delegates to the creating <see cref="AppDomain" />'s assembly resolver
    /// to locate assemblies whenever the host is unable to find them.
    /// </summary>
    public static class HostAssemblyResolverHook
    {
        private static Resolver localResolver;

        private static Resolver LocalResolver
        {
            get
            {
                if (localResolver == null)
                    Interlocked.CompareExchange(ref localResolver, new Resolver(), null);
                return localResolver;
            }
        }

        /// <summary>
        /// Installs the assembly resolver hook in the specified host.
        /// </summary>
        /// <param name="host">The host</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="host"/> is null</exception>
        public static void Install(IHost host)
        {
            Type resolverType = typeof(Resolver);
            Resolver remoteResolver = (Resolver) host.CreateInstanceFrom(AssemblyUtils.GetAssemblyLocalPath(resolverType.Assembly), resolverType.FullName).Unwrap();

            remoteResolver.Initialize(LocalResolver);
        }

        /// <exludedoc />
        /// <summary>
        /// This class is intended for internal use only.
        /// </summary>
        public sealed class Resolver : LongLivedMarshalByRefObject
        {
            private Resolver masterResolver;

            /// <excludedoc />
            /// <summary>
            /// This method is intended for internal use only.
            /// </summary>
            public void Initialize(Resolver masterResolver)
            {
                this.masterResolver = masterResolver;

                AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += ReflectionOnlyAssemblyResolve;
            }

            /// <excludedoc />
            /// <summary>
            /// This method is intended for internal use only.
            /// </summary>
            public string ResolveAssemblyLocalPath(string assemblyName, bool reflectionOnly)
            {
                try
                {
                    Assembly assembly = reflectionOnly ? Assembly.ReflectionOnlyLoad(assemblyName) : Assembly.Load(assemblyName);
                    return AssemblyUtils.GetAssemblyLocalPath(assembly);
                }
                catch
                {
                    return null;
                }
            }

            private Assembly AssemblyResolve(object sender, ResolveEventArgs e)
            {
                string assemblyPath = masterResolver.ResolveAssemblyLocalPath(e.Name, false);
                if (assemblyPath == null)
                    return null;

                return Assembly.LoadFrom(assemblyPath);
            }

            private Assembly ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs e)
            {
                string assemblyPath = masterResolver.ResolveAssemblyLocalPath(e.Name, true);
                if (assemblyPath == null)
                    return null;

                return Assembly.ReflectionOnlyLoadFrom(assemblyPath);
            }
        }
    }
}
