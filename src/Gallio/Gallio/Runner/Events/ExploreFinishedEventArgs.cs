using System;
using Gallio.Runner.Reports;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Arguments for an event raised to indicate that test exploration has finished.
    /// </summary>
    public sealed class ExploreFinishedEventArgs : OperationFinishedEventArgs
    {
        private readonly Report report;

        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="success">True if test exploration was successful</param>
        /// <param name="report">The report, including test model data on success</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report"/> is null</exception>
        public ExploreFinishedEventArgs(bool success, Report report)
            : base(success)
        {
            if (report == null)
                throw new ArgumentNullException("report");

            this.report = report;
        }

        /// <summary>
        /// Gets the report, including test model data on success.
        /// </summary>
        public Report Report
        {
            get { return report; }
        }
    }
}