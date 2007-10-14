
using System;
using System.Reflection;
using MbUnit.Hosting;

namespace MbUnit.Hosting
{
    /// <summary>
    /// The loader class provides services for controlling how assemblies are
    /// resolved and for loading related resources such as XML documentation.
    /// </summary>
    public static class Loader
    {
        private static IAssemblyResolverManager cachedAssemblyResolverManager;
        private static IXmlDocumentationResolver cachedDocumentationResolver;

        static Loader()
        {
            Runtime.InstanceChanged += delegate
            {
                cachedAssemblyResolverManager = null;
                cachedDocumentationResolver = null;
            };
        }

        /// <summary>
        /// Gets the assembly resolver manager.
        /// </summary>
        public static IAssemblyResolverManager AssemblyResolverManager
        {
            get
            {
                if (cachedAssemblyResolverManager == null)
                    cachedAssemblyResolverManager = Runtime.Instance.Resolve<IAssemblyResolverManager>();
                return cachedAssemblyResolverManager;
            }
        }

        /// <summary>
        /// Gets the XML documentation resolver.
        /// </summary>
        public static IXmlDocumentationResolver XmlDocumentationResolver
        {
            get
            {
                if (cachedDocumentationResolver == null)
                    cachedDocumentationResolver = Runtime.Instance.Resolve<IXmlDocumentationResolver>();
                return cachedDocumentationResolver;
            }
        }

        /// <summary>
        /// Gets the local path of the assembly prior to shadow copying.
        /// Returns null if the original location of the assembly is not local.
        /// </summary>
        /// <param name="assembly">The assembly</param>
        /// <returns>The original non-shadow copied local path of the assembly, or null if none</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null</exception>
        public static string GetAssemblyLocalPath(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(@"assembly");

            try
            {
                Uri uri = new Uri(assembly.CodeBase);
                if (uri.IsFile)
                    return uri.LocalPath;
            }
            catch (Exception)
            {
                // Ignore other weird problems getting the local path.
            }

            return null;
        }

        /// <summary>
        /// Gets the original local path of the assembly prior to shadow
        /// copying, if it is local.  Otherwise, returns the shadow-copied
        /// assembly location.
        /// </summary>
        /// <param name="assembly">The assembly</param>
        /// <returns>The local path of the assembly, preferably its original
        /// non-shadow copied location</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null</exception>
        public static string GetFriendlyAssemblyLocation(Assembly assembly)
        {
            string localPath = GetAssemblyLocalPath(assembly);
            if (localPath != null)
                return localPath;

            return assembly.Location;
        }

        /// <summary>
        /// If the assembly codebase is a local file, returns it as a local
        /// path.  Otherwise, returns the assembly codebase Uri.
        /// </summary>
        /// <param name="assembly">The assembly</param>
        /// <returns>The assembly's path</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null</exception>
        public static string GetFriendlyAssemblyCodeBase(Assembly assembly)
        {
            string localPath = GetAssemblyLocalPath(assembly);
            if (localPath != null)
                return localPath;

            return assembly.CodeBase;
        }
    }
}
