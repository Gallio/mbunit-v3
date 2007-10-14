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
using MbUnit.Core.IO;
using MbUnit.Core.ProgressMonitoring;

namespace MbUnit.Runner.Reports
{
    /// <summary>
    /// The report manager provides services for manipulating reports.
    /// </summary>
    public interface IReportManager
    {
        /// <summary>
        /// Gets the names of all available report formatters.
        /// </summary>
        IList<string> GetFormatterNames();

        /// <summary>
        /// Gets a report formatter by name.
        /// </summary>
        /// <param name="name">The name of the report formatter, matched case-insensitively</param>
        /// <returns>The report formatter, or null if none exist with the specified name</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        IReportFormatter GetFormatter(string name);

        /// <summary>
        /// Formats a report and saves it to a file.
        /// Overwrites the file and replaces associated resources if they exist.
        /// </summary>
        /// <seealso cref="IReportFormatter.Format"/> for important remarks.
        /// <param name="formatterName">The formatter name</param>
        /// <param name="report">The report to format</param>
        /// <param name="reportContext">The report context specifying the formatted report file to generate</param>
        /// <param name="options">Custom options for the report formatter</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatterName"/>, <paramref name="report"/>,
        /// <paramref name="reportContext"/>, <paramref name="options"/> or <paramref name="progressMonitor"/> is null</exception>
        void Format(string formatterName, Report report, ReportContext reportContext,
            NameValueCollection options, IProgressMonitor progressMonitor);

        /// <summary>
        /// Saves the report as XML to the specified file.
        /// </summary>
        /// <param name="report">The report</param>
        /// <param name="reportContext">The report context specifying the report file to save</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report"/>, <paramref name="reportContext"/>
        /// or <paramref name="progressMonitor"/> is null</exception>
        void SaveReport(Report report, ReportContext reportContext, IProgressMonitor progressMonitor);

        /// <summary>
        /// Loads the report from XML from the specified file.
        /// </summary>
        /// <param name="reportContext">The report context specifying the report file to load</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reportContext"/> or <paramref name="progressMonitor "/> is null</exception>
        Report LoadReport(ReportContext reportContext, IProgressMonitor progressMonitor);

        /// <summary>
        /// Creates a report context.
        /// </summary>
        /// <param name="formatterName">The report formatter name</param>
        /// <param name="reportDirectory">The directory in which the report should reside</param>
        /// <param name="reportFileNameFormat">A format string used to construct the name of report files (without the extension).
        /// Within the format string, <c>{0}</c> is replaced by the date and <c>{1}</c> by the time.</param>
        /// <param name="reportTime">The date/time when the report was produced.</param>
        /// <param name="fileSystem">The file system</param>
        /// <returns>The full path of the report</returns>
        ReportContext CreateReportContext(string formatterName, string reportDirectory, string reportFileNameFormat, DateTime reportTime, IFileSystem fileSystem);
    }
}
