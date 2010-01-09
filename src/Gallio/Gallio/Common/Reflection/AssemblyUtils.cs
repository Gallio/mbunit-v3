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
using System.IO;
using System.Reflection;

namespace Gallio.Common.Reflection
{
    /// <summary>
    /// Provides helpers for working with assemblies.
    /// </summary>
    public static class AssemblyUtils
    {
        /// <summary>
        /// Gets the location of the assembly, or null if it is dynamic.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The assembly location as returned by <see cref="Assembly.Location" /> or
        /// null if the assembly is dynamic and does not have a location.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null.</exception>
        public static string GetAssemblyLocation(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(@"assembly");

            try
            {
                return assembly.Location;
            }
            catch (NotSupportedException)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the local path of the assembly prior to shadow copying.
        /// Returns null if the original location of the assembly is not local.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The original non-shadow copied local path of the assembly, or null if none.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null.</exception>
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
        /// <param name="assembly">The assembly.</param>
        /// <returns>The local path of the assembly, preferably its original
        /// non-shadow copied location, or null if the assembly is dynamic and does not
        /// have a location.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null.</exception>
        public static string GetFriendlyAssemblyLocation(Assembly assembly)
        {
            string localPath = GetAssemblyLocalPath(assembly);
            if (localPath != null)
                return localPath;

            return GetAssemblyLocation(assembly);
        }

        /// <summary>
        /// If the assembly codebase is a local file, returns it as a local
        /// path.  Otherwise, returns the assembly codebase Uri.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The assembly's path.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null.</exception>
        public static string GetFriendlyAssemblyCodeBase(Assembly assembly)
        {
            string localPath = GetAssemblyLocalPath(assembly);
            if (localPath != null)
                return localPath;

            return assembly.CodeBase;
        }

        /// <summary>
        /// Gets the culture component of an assembly name.
        /// </summary>
        /// <param name="assemblyName">The assembly name.</param>
        /// <returns>The culture name.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyName"/> is null.</exception>
        public static string GetAssemblyNameCulture(AssemblyName assemblyName)
        {
            if (assemblyName == null)
                throw new ArgumentNullException("assemblyName");

            string culture = assemblyName.CultureInfo.Name;
            return culture.Length != 0 ? culture : "neutral";
        }

        /// <summary>
        /// Returns true if the stream represents a CLI Assembly in Microsoft PE format.
        /// </summary>
        /// <remarks>
        /// This function does not close the stream.
        /// </remarks>
        /// <param name="stream">The stream.</param>
        /// <returns>True if the stream represents an assembly.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stream"/> is null.</exception>
        public static bool IsAssembly(Stream stream)
        {
            return GetAssemblyMetadata(stream, AssemblyMetadataFields.Default) != null;
        }

        /// <summary>
        /// Returns true if the file represents a CLI Assembly in Microsoft PE format.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>True if the file represents an assembly.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="filePath"/> is null.</exception>
        public static bool IsAssembly(string filePath)
        {
            return GetAssemblyMetadata(filePath, AssemblyMetadataFields.Default) != null;
        }

        /// <summary>
        /// Gets metadata about CLI Assembly in Microsoft PE format.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="fields">The optional fields of the <see cref="AssemblyMetadata" /> structure to populate.</param>
        /// <returns>The metadata or null if the stream does not represent a CLI assembly.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stream"/> is null.</exception>
        public static AssemblyMetadata GetAssemblyMetadata(Stream stream, AssemblyMetadataFields fields)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            return AssemblyMetadata.ReadAssemblyMetadata(stream, fields);
        }

        /// <summary>
        /// Gets the Major and Minor components of the CLI runtime version of a CLI Assembly in Microsoft PE format.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="fields">The optional fields of the <see cref="AssemblyMetadata" /> structure to populate.</param>
        /// <returns>The version, of which only the major and minor components are populated,
        /// or null if the stream does not represent a CLI assembly.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="filePath"/> is null.</exception>
        public static AssemblyMetadata GetAssemblyMetadata(string filePath, AssemblyMetadataFields fields)
        {
            if (filePath == null)
                throw new ArgumentNullException("filePath");

            if (!File.Exists(filePath))
                return null;

            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return AssemblyMetadata.ReadAssemblyMetadata(stream, fields);
            }
        }
    }
}