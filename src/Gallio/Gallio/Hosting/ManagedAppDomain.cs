using System;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using Gallio.Hosting;
using Gallio.Utilities;

namespace Gallio.Hosting
{
    /// <summary>
    /// Provides a simple way to run code within a fresh <see cref="AppDomain" />
    /// of the current process.  The new <see cref="AppDomain" /> inherits the security
    /// context and assembly resolution policies of the current one.
    /// </summary>
    public sealed class ManagedAppDomain : LongLivingMarshalByRefObject, IDisposable
    {
        private AppDomain appDomain;

        private ManagedAppDomain(AppDomain appDomain)
        {
            this.appDomain = appDomain;
        }

        /// <summary>
        /// Gets the AppDomain, or null if <see cref="Dispose" /> was called.
        /// </summary>
        public AppDomain AppDomain
        {
            get { return appDomain; }
        }

        /// <summary>
        /// Creates an <see cref="AppDomain" /> with the specified name and configures it
        /// to inherit the security context and assembly resolution policies of the current
        /// <see cref="AppDomain" />.
        /// </summary>
        /// <param name="name">The name of the <see cref="AppDomain" /> to create</param>
        /// <returns>A wrapper for the newly created <see cref="AppDomain" /></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        public static ManagedAppDomain Create(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
            Evidence evidence = AppDomain.CurrentDomain.Evidence;
            PermissionSet defaultPermissionSet = new PermissionSet(PermissionState.Unrestricted);
            StrongName[] fullTrustAssemblies = new StrongName[0];

            AppDomain appDomain = AppDomain.CreateDomain(name, evidence, setup, defaultPermissionSet, fullTrustAssemblies);
            ManagedAppDomain owner = new ManagedAppDomain(appDomain);

            try
            {
                Type shimType = typeof(Shim);
                Shim shim = (Shim)appDomain.CreateInstanceFromAndUnwrap(Loader.GetAssemblyLocalPath(shimType.Assembly), shimType.FullName);

                shim.Initialize(owner);
            }
            catch (Exception)
            {
                owner.Dispose();
                throw;
            }

            return owner;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (appDomain != null)
            {
                AppDomain.Unload(appDomain);
                appDomain = null;
            }
        }

        private string ResolveAssemblyLocalPath(string assemblyName, bool reflectionOnly)
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

        /// <exludedoc />
        /// <summary>
        /// This class is intended for internal use only.
        /// </summary>
        public sealed class Shim : LongLivingMarshalByRefObject
        {
            private ManagedAppDomain owner;

            internal void Initialize(ManagedAppDomain owner)
            {
                this.owner = owner;

                AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += ReflectionOnlyAssemblyResolve;
            }

            private Assembly AssemblyResolve(object sender, ResolveEventArgs e)
            {
                string assemblyPath = owner.ResolveAssemblyLocalPath(e.Name, false);
                if (assemblyPath == null)
                    return null;

                return Assembly.LoadFrom(assemblyPath);
            }

            private Assembly ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs e)
            {
                string assemblyPath = owner.ResolveAssemblyLocalPath(e.Name, true);
                if (assemblyPath == null)
                    return null;

                return Assembly.ReflectionOnlyLoadFrom(assemblyPath);
            }
        }
    }
}