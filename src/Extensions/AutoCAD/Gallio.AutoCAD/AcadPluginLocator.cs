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
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Gallio.Common.Collections;
using Gallio.Common.IO;
using Gallio.Common.Reflection;
using Gallio.Runtime.Logging;

namespace Gallio.AutoCAD
{
    /// <summary>
    /// Finds the AutoCAD plugin location. 
    /// </summary>
    public class AcadPluginLocator : IAcadPluginLocator
    {
        private readonly ILogger logger;
        private readonly IFileSystem fileSystem;

        /// <summary>
        /// Creates a new AutoCAD plugin locator.
        /// </summary>
        /// <param name="logger">A logger.</param>
        /// <param name="fileSystem">A file system.</param>
        public AcadPluginLocator(ILogger logger, IFileSystem fileSystem)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");
            if (fileSystem == null)
                throw new ArgumentNullException("fileSystem");
            this.logger = logger;
            this.fileSystem = fileSystem;
        }

        /// <inheritdoc/>
        public virtual string GetPluginPath(string acadVersion)
        {
            Version runtimeVersion;
            if (TryParse(acadVersion, out runtimeVersion) == false)
            {
                logger.Log(LogSeverity.Warning, string.Format("Unable to parse ACADVER value: {0}. Falling back to highest available version.", acadVersion));
                runtimeVersion = null;
            }

            string path = null;
            var highest = new Version(0, 0);
            foreach (var pluginPath in GetPlugins())
            {
                var version = GetPluginVersion(pluginPath);
                if (version == null)
                    continue;
                if (runtimeVersion != null)
                {
                    if (version == runtimeVersion)
                        return pluginPath;
                    if (version > runtimeVersion)
                        continue;
                }
                if (version > highest)
                {
                    path = pluginPath;
                    highest = version;
                }
            }

            if (path == null)
                throw new FileNotFoundException("Unable to find Gallio.AutoCAD.Plugin assembly.");

            return path;
        }

        /// <summary>
        /// Converts the specified ACADVER value into a <see cref="Version"/>.
        /// </summary>
        /// <remarks>
        /// Autodesk has been using the format "XX.Ys (LMS Tech)" for the ACADVER value since AutoCAD 2005.
        /// The "XX" is the major version number and "Y" is the minor version number.
        /// </remarks>
        private static bool TryParse(string acadVersion, out Version version)
        {
            version = null;
            if (acadVersion == null)
                return false;
            var components = acadVersion.Split(new[] { '.', 's', ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
            if (components.Length < 2)
                return false;
            int major;
            if (int.TryParse(components[0], NumberStyles.None, CultureInfo.InvariantCulture, out major) == false)
                return false;
            int minor;
            if (int.TryParse(components[1], NumberStyles.None, CultureInfo.InvariantCulture, out minor) == false)
                return false;
            version = new Version(major, minor);
            return true;
        }

        private IEnumerable<string> GetPlugins()
        {
            var acadRunnerDir = Path.GetDirectoryName(
                AssemblyUtils.GetAssemblyLocalPath(typeof(AcadPluginLocator).Assembly));

            const string searchPattern = "Gallio.AutoCAD.Plugin*.dll";
            foreach (var path in GetFilesInDirectory(acadRunnerDir, searchPattern, SearchOption.TopDirectoryOnly))
                yield return path;

            // Check if Gallio is running directly out of its source tree.
            string devPath = acadRunnerDir;
            while (devPath != null && Path.GetFileName(devPath) != @"src")
                devPath = Path.GetDirectoryName(devPath);

            if (devPath == null)
                yield break;

            var projectOutputDir = Path.Combine(devPath, @"Extensions\AutoCAD\Gallio.AutoCAD.Plugin\bin");
            if (fileSystem.DirectoryExists(projectOutputDir))
            {
                foreach (var path in GetFilesInDirectory(projectOutputDir, searchPattern, SearchOption.AllDirectories))
                    yield return path;
            }
        }

        private IEnumerable<string> GetFilesInDirectory(string path, string searchPattern, SearchOption searchOption)
        {
            try
            {
                return fileSystem.GetFilesInDirectory(path, searchPattern, searchOption);
            }
            catch (UnauthorizedAccessException e)
            {
                logger.Log(LogSeverity.Error, "Unable to access the AutoCAD plugin directory.", e);
            }
            catch (IOException e)
            {
                logger.Log(LogSeverity.Error, "Error occurred while searching the AutoCAD plugin directory.", e);
            }

            return EmptyArray<string>.Instance;
        }


        /// <summary>
        /// Gets the version of AutoCAD that the specified plugin DLL targets.
        /// </summary>
        /// <returns>
        /// The version targetted or <c>null</c> if it could not be determined.
        /// </returns>
        /// <remarks>
        /// This is done by getting the version number out of filenames in the format
        /// Gallio.AutoCAD.Plugin{version}.dll. The version component should be in
        /// the format XXY. The first two characters are interpreted as the major
        /// version number and the next character (and any subsequent characters) as the
        /// minor version number. All of these characters must be digits.
        /// </remarks>
        private static Version GetPluginVersion(string path)
        {
            var filename = Path.GetFileName(path);
            if (filename == null)
                return null;

            var match = Regex.Match(filename, @"Gallio\.AutoCAD\.Plugin(\d{3,})\.dll", RegexOptions.IgnoreCase);
            if (match.Success == false)
                return null;

            var versionText = match.Groups[1].Value;
            int major = int.Parse(versionText.Substring(0, 2), NumberStyles.None, CultureInfo.InvariantCulture);
            int minor = int.Parse(versionText.Substring(2), NumberStyles.None, CultureInfo.InvariantCulture);
            return new Version(major, minor);
        }
    }
}
