using System.IO;
using System.Reflection;

namespace Gallio.AutoCAD.Plugin
{
    ///<summary>
    /// Supports resolving the <c>Gallio.Loader</c> assembly.
    ///</summary>
    public static class GallioLoaderAssemblyResolver
    {
        /// <summary>
        /// Resolves the <c>Gallio.Loader</c> assembly.
        /// </summary>
        /// <param name="runtimePath">The Gallio runtime path.</param>
        /// <returns>The <c>Gallio.Loader</c> assembly.</returns>
        /// <remarks>
        /// The assembly is searched for in the following locations in order:
        /// <list type="number">
        /// <item>
        /// <description>
        /// The Load context via the assembly's fully qualified name. The fully
        /// qualified assembly name is based off the that of <c>Gallio.dll</c>.
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// The LoadFrom context. <c>Gallio.Loader.dll</c> is assumed to be in
        /// the root of the runtime path.
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// The project output folder. This is helpful to those developing Gallio itself.
        /// </description>
        /// </item>
        /// </list>
        /// </remarks>
        public static Assembly Resolve(string runtimePath)
        {
            if (!Directory.Exists(runtimePath))
                throw new DirectoryNotFoundException(string.Format("Directory does not exist: {0}", runtimePath));

            var assembly =
                TryLoadContext(runtimePath)
                ?? TryLoadFromContext(runtimePath)
                ?? TryProjectOutputDirectory(runtimePath);

            if (assembly == null)
                throw new FileNotFoundException("Unable to find the Gallio.Loader assembly.");

            return assembly;
        }

        private static Assembly TryLoadContext(string runtimePath)
        {
            var loaderName = FindFullyQualifiedAssemblyName(runtimePath);

            try
            {
                if (loaderName != null)
                    return Assembly.Load(loaderName.FullName);
            }
            catch (FileNotFoundException)
            {
                // Not found in Load context.
            }

            return null;
        }

        private static AssemblyName FindFullyQualifiedAssemblyName(string runtimePath)
        {
            var gallioAssembly = Path.Combine(runtimePath, "Gallio.dll");
            if (!File.Exists(gallioAssembly))
                return null;

            var gallioAssemblyName = AssemblyName.GetAssemblyName(gallioAssembly);

            // We're assuming here that the full assembly names for Gallio.Loader.dll
            // and Gallio.dll will share everything except the simple name portion.
            return new AssemblyName(gallioAssemblyName.FullName) { Name = "Gallio.Loader" };
        }

        private static Assembly TryLoadFromContext(string directory)
        {
            var loaderAssembly = Path.Combine(directory, "Gallio.Loader.dll");
            if (!File.Exists(loaderAssembly))
                return null;

            return Assembly.LoadFrom(loaderAssembly);
        }

        private static Assembly TryProjectOutputDirectory(string directory)
        {
            var srcDir = directory;
            while (srcDir != null && Path.GetFileName(srcDir) != @"src")
                srcDir = Path.GetDirectoryName(srcDir);

            if (srcDir == null)
                return null;

            var projectOutputDir = Path.Combine(srcDir, @"Gallio\Gallio.Loader\bin");
            return TryLoadFromContext(projectOutputDir);
        }
    }
}
