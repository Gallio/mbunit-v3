// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using System.Text;
using Gallio.Common.Markup;
using Gallio.Runner.Reports.Schema;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// A report reader provides services for loading a previously saved report
    /// from a <see cref="IReportContainer" />.
    /// </summary>
    public interface IReportReader
    {
        /// <summary>
        /// Gets the report container.
        /// </summary>
        IReportContainer ReportContainer { get; }

        /// <summary>
        /// Loads the report from an XML file.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The path of the saved report is constructed by appending the extension ".xml"
        /// to the container's <see cref="IReportContainer.ReportName" />.
        /// </para>
        /// </remarks>
        /// <param name="loadAttachmentContents">If true, loads attachment
        /// contents in referenced content files if they were not embedded otherwise
        /// the attachment contents are not loaded (but may be loaded later using <see cref="LoadReportAttachments" />).</param>
        /// <param name="progressMonitor">The progress monitor.</param>
        /// <returns>The loaded report.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor "/> is null.</exception>
        Report LoadReport(bool loadAttachmentContents, IProgressMonitor progressMonitor);

        /// <summary>
        /// Loads referenced report attachments from the container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method has the effect of populating the contents of all <see cref="AttachmentData" />
        /// nodes in the report that have non-null <see cref="AttachmentData.ContentPath" />.
        /// </para>
        /// </remarks>
        /// <param name="report">The report whose attachments are to be loaded.</param>
        /// <param name="progressMonitor">The progress monitor.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report" /> or <paramref name="progressMonitor "/> is null.</exception>
        void LoadReportAttachments(Report report, IProgressMonitor progressMonitor);
    }
}
