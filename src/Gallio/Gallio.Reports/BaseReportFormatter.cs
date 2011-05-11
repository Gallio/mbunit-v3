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
using Gallio.Common.Markup;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runner.Reports;

namespace Gallio.Reports
{
    /// <summary>
    /// Abstract base class for report formatters.
    /// </summary>
    public abstract class BaseReportFormatter : IReportFormatter
    {
        /// <summary>
        /// Gets the name of the option that specifies how attachments are saved.
        /// </summary>
        public const string AttachmentContentDispositionOption = @"AttachmentContentDisposition";

        /// <summary>
        /// Gets the name of the option that specifies the page size of the test report (if applicable)
        /// </summary>
        public const string ReportPageSizeOption = @"ReportPageSize";

        /// <summary>
        /// Creates a report formatter.
        /// </summary>
        protected BaseReportFormatter()
        {
        }

        /// <summary>
        /// Gets or sets the default attachment content disposition.
        /// Defaults to <see cref="AttachmentContentDisposition.Absent" />.
        /// </summary>
        public AttachmentContentDisposition DefaultAttachmentContentDisposition
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the attachment content disposition.
        /// </summary>
        /// <param name="options">The formatter options.</param>
        /// <returns>The attachment content disposition.</returns>
        protected AttachmentContentDisposition GetAttachmentContentDisposition(ReportFormatterOptions options)
        {
            string contentDisposition = options.Properties.GetValue(AttachmentContentDispositionOption);
            
            if (contentDisposition != null)
            {
                try
                {
                    return (AttachmentContentDisposition)Enum.Parse(typeof(AttachmentContentDisposition), contentDisposition, true);
                }
                catch (ArgumentException)
                {
                    // Ignore parse error.
                }
            }

            return DefaultAttachmentContentDisposition;
        }

        /// <summary>
        /// Gets the report page size.
        /// </summary>
        /// <param name="options">The formatter options.</param>
        /// <returns>The report page size.</returns>
        protected int GetReportPageSize(ReportFormatterOptions options)
        {
            int value;
            string field = options.Properties.GetValue(ReportPageSizeOption);
            return (field != null) && Int32.TryParse(field, out value) ? value : -1;
        }

        /// <inheritdoc />
        public abstract void Format(IReportWriter reportWriter, ReportFormatterOptions formatterOptions, IProgressMonitor progressMonitor);
    }
}
