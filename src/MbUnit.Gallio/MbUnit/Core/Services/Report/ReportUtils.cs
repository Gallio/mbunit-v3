using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Core.Services.Report
{
    /// <summary>
    /// Provides utility functions for managing reports.
    /// </summary>
    public static class ReportUtils
    {
        /// <summary>
        /// Gets the report for the current context.
        /// </summary>
        /// <returns>The current report</returns>
        public static IReport GetCurrentReport()
        {
            Runtime.Runtime runtime = Runtime.Runtime.Instance;
            return runtime.ReportingService.GetReport(runtime.ContextManager.CurrentContext);
        }
    }
}
