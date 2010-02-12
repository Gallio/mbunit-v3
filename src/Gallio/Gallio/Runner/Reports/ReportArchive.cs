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
        /// Non-compressed file structure.
        /// </summary>
        Normal,

        /// <summary>
        /// Compressed zip archive.
        /// </summary>
        Zip,
    }

    /// <summary>
    /// Parser for the report archive mode.
    /// </summary>
    public static class ReportArchiveParser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="reportArchive"></param>
        /// <returns></returns>
        public static bool TryParse(string value, out ReportArchive reportArchive)
        {
            return ParseImpl(value, out reportArchive, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ReportArchive Parse(string value)
        {
            ReportArchive reportArchive;
            ParseImpl(value, out reportArchive, true);
            return reportArchive;
        }

        private static bool ParseImpl(string value, out ReportArchive reportArchive, bool throwOnFailure)
        {
            if (String.IsNullOrEmpty(value))
            {
                reportArchive = ReportArchive.Normal;
                return true;
            }

            try
            {
                reportArchive = (ReportArchive)Enum.Parse(typeof(ReportArchive), value, true);
                return true;
            }
            catch (ArgumentException exception)
            {
                if (throwOnFailure)
                {
                    throw new ArgumentException(String.Format("Invalid report archive mode '{0}'. It must be one of the following values: {1}.",
                        value, String.Join(", ", Enum.GetNames(typeof(ReportArchive)))), exception);
                }

                reportArchive = ReportArchive.Normal;
                return false;
            }
        }
    }
}
