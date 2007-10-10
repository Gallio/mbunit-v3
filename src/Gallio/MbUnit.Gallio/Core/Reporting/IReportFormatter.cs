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
using System.Collections.Specialized;
using System.Text;
using MbUnit.Core.ProgressMonitoring;

namespace MbUnit.Core.Reporting
{
    /// <summary>
    /// A report formatter provides a strategy for formatting reports for human consumption.
    /// </summary>
    public interface IReportFormatter
    {
        /// <summary>
        /// Gets the unique name of the formatter.  The name is used by the user to select
        /// a formatter case-insensitively.  eg. "XML", "HTML", "MHTML".
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the preferred extension used by the formatter to save its reports.
        /// eg. "xml".
        /// </summary>
        string PreferredExtension { get; }

        /// <summary>
        /// Formats a report and saves it to a file specified by the report context.
        /// Overwrites the file and replaces associated resources if they exist.
        /// </summary>
        /// <param name="report">The report to format</param>
        /// <param name="reportContext">The report context specifying the formatted report file to generate</param>
        /// <param name="options">Custom options for the report formatter</param>
        /// <param name="progressMonitor">The progress monitor</param>
        void Format(Report report, ReportContext reportContext, NameValueCollection options,
            IProgressMonitor progressMonitor);
    }
}
