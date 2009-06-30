using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

#if LOADER
namespace Gallio.Loader // separate copy of version policy embedded in Gallio.Loader
#else
namespace Gallio.Common.Policies
#endif
{
    /// <summary>
    /// Gets version information for Gallio components.
    /// </summary>
    public static class VersionPolicy
    {
        /// <summary>
        /// Generates a version label from a version number for display purposes.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A Gallio version label consists of a major version, minor version and build number and
        /// is of the form "3.1 build 456".  The revision number is not displayed.
        /// </para>
        /// <para>
        /// Gallio component assemblies contain two different version numbers: an assembly version number
        /// and a file version number.  To preserve assembly compatibility across builds of the same version,
        /// the assembly version number omits build number and revision information.  Consequently
        /// <see cref="GetVersionLabel(System.Reflection.Assembly)" /> derived the version label from the file version number.
        /// </para>
        /// </remarks>
        /// <param name="version">The version to transform into a label.</param>
        /// <returns>The version label.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="version"/> is null.</exception>
        public static string GetVersionLabel(Version version)
        {
            if (version == null)
                throw new ArgumentNullException("version");

            return String.Format("{0}.{1} build {2}", version.Major, version.Minor, version.Build);
        }

        /// <summary>
        /// Gets a version label of a Gallio component assembly for display purposes.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A Gallio version label consists of a major version, minor version and build number and
        /// is of the form "3.1 build 456".  The revision number is not displayed.
        /// </para>
        /// <para>
        /// Gallio component assemblies contain two different version numbers: an assembly version number
        /// and a file version number.  To preserve assembly compatibility across builds of the same version,
        /// the assembly version number omits build number and revision information.  Consequently
        /// <see cref="GetVersionLabel(System.Reflection.Assembly)" /> derived the version label from the file version number.
        /// </para>
        /// </remarks>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The version label.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null.</exception>
        public static string GetVersionLabel(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            return GetVersionLabel(GetVersionNumber(assembly));
        }

        /// <summary>
        /// Gets the version number of an assembly for display purposes.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Returns the file version (<see cref="AssemblyFileVersionAttribute" />) when available,
        /// otherwise returns the assembly version.
        /// </para>
        /// </remarks>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The version number.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null.</exception>
        public static Version GetVersionNumber(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(@"assembly");

            var attribs = (AssemblyFileVersionAttribute[]) assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
            if (attribs.Length != 0)
                return new Version(attribs[0].Version);

            return assembly.GetName().Version;
        }
    }
}
