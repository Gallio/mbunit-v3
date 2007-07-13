using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Core.Services.Context;

namespace MbUnit.Core.Services.Report
{
    /// <summary>
    /// The report service manages the report for a given test execution context.
    /// </summary>
    /// <remarks>
    /// The operations on this interface are thread-safe.
    /// </remarks>
    public interface IReportService
    {
        /// <summary>
        /// Gets the report for the specified context.
        /// </summary>
        /// <param name="context">The test execution context</param>
        /// <returns>The report, never null</returns>
        IReport GetReport(IContext context);
    }
}
