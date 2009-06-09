using System.IO;
using Gallio.Common.Reflection;

namespace Gallio.AutoCAD
{
    /// <summary>
    /// Finds the AutoCAD plugin location. 
    /// </summary>
    public static class AcadPluginLocator
    {
        private const string AcadPluginFileName = "Gallio.AutoCAD.Plugin.dll";
        private static string location;

        /// <summary>
        /// Gets the location of the AutoCAD plugin.
        /// </summary>
        /// <returns>The location of the AutoCAD plugin.</returns>
        /// <exception cref="FileNotFoundException">If the AutoCAD plugin can't be found.</exception>
        public static string GetAcadPluginLocation()
        {
            if (location != null)
                return location;

            location =
                TryExtensionDirectory()
                ?? TryProjectOutputDirectory();

            if (location == null)
                throw new FileNotFoundException("Unable to find Gallio.AutoCAD.Plugin assembly.");

            return location;
        }

        private static string TryExtensionDirectory()
        {
            var acadRunnerDir = Path.GetDirectoryName(
                AssemblyUtils.GetAssemblyLocalPath(typeof(AcadPluginLocator).Assembly));

            var path = Path.Combine(acadRunnerDir, AcadPluginFileName);
            return File.Exists(path) ? path : null;
        }

        private static string TryProjectOutputDirectory()
        {
            var dir = Path.GetDirectoryName(
                AssemblyUtils.GetAssemblyLocalPath(typeof(AcadPluginLocator).Assembly));

            while (dir != null && Path.GetFileName(dir) != @"src")
                dir = Path.GetDirectoryName(dir);

            if (dir == null)
                return null;

            var projectOutputDir = Path.Combine(dir, @"Extensions\AutoCAD\Gallio.AutoCAD.Plugin\bin");
            var path = Path.Combine(projectOutputDir, AcadPluginFileName);
            return File.Exists(path) ? path : null;
        }
    }
}
