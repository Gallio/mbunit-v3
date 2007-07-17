// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Core.Services.Contexts;
using MbUnit.Framework.Services.Reports;
using MbUnit.Core.Services.Reports.Xml;
using MbUnit.Framework.Services.Contexts;

namespace MbUnit.Core.Services.Reports
{
    /// <summary>
    /// The default implementation of the report service.
    /// </summary>
    public sealed class DefaultReportService : IReportService
    {
        private const string ContextDataKey = "$$DefaultReportService.ContextData$$";

        /// <inheritdoc />
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
            private IReport report;

            /// <summary>
            /// Create the state for the specified report service.
            /// </summary>
            /// <param name="service">The report service</param>
            public ContextData(DefaultReportService service)
            {
                //report = new XmlReport();
            }

            /// <summary>
            /// Gets the report.
            /// </summary>
            public IReport Report
            {
                get { return report; }
            }
        }
    }
}
