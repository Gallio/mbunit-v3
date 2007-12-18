using System;
using System.Diagnostics;

namespace Gallio.Runner
{
    /// <summary>
    /// Provides helper functions for test runner tools.
    /// </summary>
    public static class TestRunnerUtils
    {
        /// <summary>
        /// Presents a generated report to the user using the default viewing
        /// application for the report's document type.
        /// </summary>
        /// <param name="reportDocumentPath">The path of the report</param>
        public static void ShowReportDocument(string reportDocumentPath)
        {
            if (reportDocumentPath == null)
                throw new ArgumentNullException("reportDocumentPath");

            Process.Start(reportDocumentPath);
        }
    }
}
