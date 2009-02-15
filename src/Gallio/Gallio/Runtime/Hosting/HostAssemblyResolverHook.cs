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
using System.Threading;
using Gallio.Runtime.Loader;
using Gallio.Runtime.Remoting;
using Gallio.Reflection;

namespace Gallio.Runtime.Hosting
{
    /// <summary>
    /// Attaches to a <see cref="IHost" /> to provide assembly resolution services.
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
        /// <para>
        /// Installs an assembly resolver that provides access to the installation path
        /// using the <see cref="AssemblyResolverBootstrap" />.
        /// </para>
        /// <para>
        /// Does nothing if the host is local.
        /// </para>
        /// </summary>
        /// <remarks>
        /// This hook is recommended for newly created domains.
        /// </remarks>
        /// <param name="host">The host</param>
        /// <param name="runtimePath">The Gallio runtime path</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="host"/> or
        /// <paramref name="runtimePath" /> is null</exception>
        public static void Bootstrap(IHost host, string runtimePath)
        {
            if (host == null)
                throw new ArgumentNullException("host");
            if (runtimePath == null)
                throw new ArgumentNullException("runtimePath");

            if (!host.IsLocal)
            {
                Resolver remoteResolver = HostUtils.CreateInstance<Resolver>(host);
                remoteResolver.Bootstrap(runtimePath);
            }
        }

        /// <summary>
        /// <para>
        /// Installs an assembly resolver that delegates to the creating <see cref="AppDomain" />'s
        /// assembly resolver to locate assemblies whenever the host is unable to find them.
        /// </para>
        /// <para>
        /// Does nothing if the host is local.
        /// </para>
        /// </summary>
        /// <remarks>
        /// This hook is useful for testing but should not be used in production code.
        /// </remarks>
        /// <param name="host">The host</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="host"/> is null</exception>
        public static void InstallCallback(IHost host)
        {
            if (host == null)
                throw new ArgumentNullException("host");

            if (!host.IsLocal)
            {
                Resolver remoteResolver = HostUtils.CreateInstance<Resolver>(host);
                remoteResolver.InstallCallback(LocalResolver);
            }
        }

        private interface IResolver
        {
            string ResolveAssemblyLocalPath(string assemblyName, bool reflectionOnly);
        }

        private sealed class Resolver : LongLivedMarshalByRefObject, IResolver
        {
            private IResolver masterResolver;

            public void Bootstrap(string runtimePath)
            {
                AssemblyResolverBootstrap.Install(runtimePath);
            }

            public void InstallCallback(IResolver masterResolver)
            {
                this.masterResolver = masterResolver;

                AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += ReflectionOnlyAssemblyResolve;
            }

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
