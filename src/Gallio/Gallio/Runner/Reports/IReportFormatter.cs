// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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
using Gallio.Runtime;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// A report formatter provides a strategy for formatting reports for human consumption.
    /// </summary>
    public interface IReportFormatter : IRegisteredComponent
    {
        /// <summary>
        /// Formats the report indicated by the report writer.
        /// </summary>
        /// <param name="reportWriter">The report writer</param>
        /// <param name="formatterOptions">The report formatter options</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reportWriter"/>,
        /// <paramref name="formatterOptions"/> or <paramref name="progressMonitor"/> is null</exception>
        void Format(IReportWriter reportWriter, ReportFormatterOptions formatterOptions, IProgressMonitor progressMonitor);
    }
}
