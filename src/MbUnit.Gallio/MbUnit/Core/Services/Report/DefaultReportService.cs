using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Core.Services.Context;
using MbUnit.Core.Services.Report;
using MbUnit.Core.Services.Report.Xml;

namespace MbUnit.Core.Services
{
    /// <summary>
    /// The default implementation of the report service.
    /// </summary>
    public sealed class DefaultReportService : IReportService
    {
        private const string ContextDataKey = "$$DefaultReportService.ContextData$$";

        public IReport GetReport(IContext context)
        {
            ContextData state = GetInitializedContextData(context);
            return state.Report;
        }

        /// <summary>
        /// Gets the report service data for the specified context.
        /// Initializes it if absent.
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The assertion state</returns>
        private ContextData GetInitializedContextData(IContext context)
        {
            lock (context.SyncRoot)
            {
                ContextData contextData;
                if (context.TryGetData(ContextDataKey, out contextData))
                {
                    contextData = new ContextData(this);
                    context.SetData(ContextDataKey, contextData);
                }

                return contextData;
            }
        }

        /// <summary>
        /// Maintains report service state information associated with the context.
        /// </summary>
        private class ContextData
        {
            /// <summary>
            /// Create the state for the specified report service.
            /// </summary>
            /// <param name="service">The report service</param>
            public ContextData(DefaultReportService service)
            {
                report = new XmlReport();
            }

            /// <summary>
            /// Gets the report.
            /// </summary>
            public IReport Report
            {
                get { return report; }
            }
            private IReport report;
        }
    }
}
