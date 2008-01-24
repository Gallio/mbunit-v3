// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using System.Collections.Specialized;
using Gallio.Hosting;
using Gallio.Hosting.ProgressMonitoring;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// The report manager provides services for manipulating reports.
    /// </summary>
    public interface IReportManager
    {
        /// <summary>
        /// Gets a resolver for resolving registered
        /// <see cref="IReportFormatter" /> components by name.
        /// </summary>
        IRegisteredComponentResolver<IReportFormatter> FormatterResolver { get; }

        /// <summary>
        /// Formats the report indicated by the report writer.
        /// </summary>
        /// <param name="reportWriter">The report writer</param>
        /// <param name="formatterName">The formatter name</param>
        /// <param name="formatterOptions">Custom options for the report formatter</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reportWriter"/>, <paramref name="formatterName"/>,
        /// <paramref name="formatterOptions"/> or <paramref name="progressMonitor"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is no formatter with the specified name</exception>
        void Format(IReportWriter reportWriter, string formatterName, NameValueCollection formatterOptions, IProgressMonitor progressMonitor);

        /// <summary>
        /// Gets a report reader to load a report from the specified container.
        /// </summary>
        /// <param name="reportContainer">The report container</param>
        /// <returns>The report reader</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reportContainer"/> is null</exception>
        IReportReader CreateReportReader(IReportContainer reportContainer);

        /// <summary>
        /// Gets a report writer to save or format a report to the specified container.
        /// </summary>
        /// <param name="report">The report</param>
        /// <param name="reportContainer">The report container</param>
        /// <returns>The report writer</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report"/> or <paramref name="reportContainer"/> is null</exception>
        IReportWriter CreateReportWriter(Report report, IReportContainer reportContainer);
    }
}
