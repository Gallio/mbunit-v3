using System;
using System.Reflection;
using System.Threading;
using Gallio.Utilities;

namespace Gallio.Hosting
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
            Resolver remoteResolver = (Resolver) host.CreateInstanceFrom(Loader.GetAssemblyLocalPath(resolverType.Assembly), resolverType.FullName).Unwrap();

            remoteResolver.Initialize(LocalResolver);
        }

        /// <exludedoc />
        /// <summary>
        /// This class is intended for internal use only.
        /// </summary>
        public sealed class Resolver : LongLivingMarshalByRefObject
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
                    return Loader.GetAssemblyLocalPath(assembly);
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
