using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Gallio.Common.Reflection
{
    /// <summary>
    /// Describes characteristics used to match an assembly such as an assembly name and version range.
    /// </summary>
    [Serializable]
    public class AssemblySignature
    {
        private static readonly Regex ParseRegex = new Regex(
            @"^(?<name>[a-zA-Z0-9_.]+)(?:\s*,\s*Version\s*=\s*(?<minVersion>\d+\.\d+\.\d+\.\d+)(?:\s*-\s*(?<maxVersion>\d+\.\d+\.\d+\.\d+))?)?$",
            RegexOptions.CultureInvariant | RegexOptions.Singleline);

        private readonly string name;
        private Version minVersion, maxVersion;

        /// <summary>
        /// Creates an assembly signature.
        /// </summary>
        /// <param name="name">The simple name of the assembly.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref cref="name" /> is null.</exception>
        public AssemblySignature(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            this.name = name;
        }

        /// <summary>
        /// Gets the simple name of the assembly.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Sets the assembly version to match.
        /// </summary>
        /// <param name="version">The version to match, or null if none.</param>
        public void SetVersion(Version version)
        {
            minVersion = version;
            maxVersion = version;
        }

        /// <summary>
        /// Sets the assembly version range to match.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <paramref name="minVersion"/> and <paramref name="maxVersion"/> must
        /// either both be non-null or both be null.
        /// </para>
        /// </remarks>
        /// <param name="minVersion">The minimum assembly version to match inclusively, or null if there is no lower bound.</param>
        /// <param name="maxVersion">The maximum assembly version to match inclusively, or null if there is no upper bound.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="minVersion"/> is null but
        /// not <paramref name="maxVersion"/> or vice-versa.  Also thrown if <paramref name="minVersion"/>
        /// is greater than <paramref name="maxVersion"/>.</exception>
        public void SetVersionRange(Version minVersion, Version maxVersion)
        {
            if (minVersion == null && maxVersion != null
                || minVersion != null && maxVersion == null)
                throw new ArgumentException("Min and max version must either both be non-null or both be null.");
            if (minVersion != null && minVersion > maxVersion)
                throw new ArgumentException("Min version must be less than or equal to max version.");

            this.minVersion = minVersion;
            this.maxVersion = maxVersion;
        }

        /// <summary>
        /// Gets the minimum assembly version to match inclusively, or null if there is no lower bound.
        /// </summary>
        public Version MinVersion
        {
            get { return minVersion; }
        }

        /// <summary>
        /// Gets the maximum assembly version to match inclusively, or null if there is no upper bound.
        /// </summary>
        public Version MaxVersion
        {
            get { return maxVersion; }
        }

        /// <summary>
        /// Returns true if the signature matches the specified assembly name.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When the assembly name is a partial name that omits version information, the version
        /// criteria of the signature are ignored.
        /// </para>
        /// </remarks>
        /// <param name="assemblyName">The assembly name.</param>
        /// <returns>True if the signature matches the assembly name.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyName"/> is null.</exception>
        public bool IsMatch(AssemblyName assemblyName)
        {
            if (assemblyName == null)
                throw new ArgumentNullException("assemblyName");

            if (assemblyName.Name != name)
                return false;

            if (minVersion != null)
            {
                Version assemblyVersion = assemblyName.Version;
                if (assemblyVersion != null)
                {
                    if (minVersion > assemblyVersion || maxVersion < assemblyVersion)
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Parses the assembly signature from a string.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The string takes the form of following forms:
        /// <list type="bullet">
        /// <item>"AssemblyName"</item>
        /// <item>"AssemblyName, Version=1.2.0.0"</item>
        /// <item>"AssemblyName, Version=1.2.0.0-1.3.65535.65535"</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="str">The string to parse.</param>
        /// <returns>The parsed signature.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="str"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="str"/> is malformed.</exception>
        public static AssemblySignature Parse(string str)
        {
            if (str == null)
                throw new ArgumentNullException("str");

            Match match = ParseRegex.Match(str);
            if (! match.Success)
                throw new ArgumentException("The specified assembly signature is not valid.", "str");

            string name = match.Groups["name"].Value;
            string minVersion = match.Groups["minVersion"].Value;
            string maxVersion = match.Groups["maxVersion"].Value;

            var signature = new AssemblySignature(name);
            if (minVersion.Length != 0)
            {
                if (maxVersion.Length != 0)
                {
                    signature.SetVersionRange(new Version(minVersion), new Version(maxVersion));
                }
                else
                {
                    signature.SetVersion(new Version(minVersion));
                }
            }

            return signature;
        }

        /// <summary>
        /// Converts the signature to a string.
        /// </summary>
        /// <returns>The signature in a format that can be subsequently parsed by <see cref="Parse"/>.</returns>
        /// <seealso cref="Parse"/>
        public override string ToString()
        {
            if (minVersion == null && maxVersion == null)
                return name;

            var builder = new StringBuilder(name);
            builder.Append(", Version=");
            builder.Append(minVersion);

            if (minVersion != maxVersion)
            {
                builder.Append('-');
                builder.Append(maxVersion);
            }

            return builder.ToString();
        }
    }
}