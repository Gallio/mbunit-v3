using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// The type of file archive used to enclose a test report.
    /// </summary>
    public enum ReportArchive
    {
        /// <summary>
        /// Flat system files.
        /// </summary>
        Flat,

        /// <summary>
        /// Compressed zip archive.
        /// </summary>
        Zip,
    }
}
