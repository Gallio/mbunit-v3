using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Gallio.BuildTools.Tasks
{
    /// <summary>
    /// Parses a version number into its constituent parts.
    /// </summary>
    public class ParseVersion : Task
    {
        [Required]
        public string Version { get; set; }

        [Output]
        public string Major { get; private set; }

        [Output]
        public string Minor { get; private set; }

        [Output]
        public string Build { get; private set; }

        [Output]
        public string Revision { get; private set; }

        public override bool Execute()
        {
            Version version = new Version(Version);
            Major = version.Major.ToString(CultureInfo.InvariantCulture);
            Minor = version.Minor.ToString(CultureInfo.InvariantCulture);
            Build = version.Build.ToString(CultureInfo.InvariantCulture);
            Revision = version.Revision.ToString(CultureInfo.InvariantCulture);
            return true;
        }
    }
}
