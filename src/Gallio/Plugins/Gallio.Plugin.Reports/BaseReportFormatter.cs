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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Gallio.Hosting.ProgressMonitoring;
using Gallio.Runner.Reports;

namespace Gallio.Plugin.Reports
{
    /// <summary>
    /// Abstract base class for report formatters.
    /// </summary>
    public abstract class BaseReportFormatter : IReportFormatter
    {
        private readonly string name;
        private ExecutionLogAttachmentContentDisposition defaultAttachmentContentDisposition;

        /// <summary>
        /// Gets the name of the option that how attachments are saved.
        /// </summary>
        public const string AttachmentContentDispositionOption = @"AttachmentContentDisposition";

        /// <summary>
        /// Creates a report formatter.
        /// </summary>
        /// <param name="name">The formatter name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        protected BaseReportFormatter(string name)
        {
            if (name == null)
                throw new ArgumentNullException(@"name");

            this.name = name;
        }

        /// <summary>
        /// Gets or sets the default attachment content disposition.
        /// Defaults to <see cref="ExecutionLogAttachmentContentDisposition.Absent" />.
        /// </summary>
        public ExecutionLogAttachmentContentDisposition DefaultAttachmentContentDisposition
        {
            get { return defaultAttachmentContentDisposition; }
            set { defaultAttachmentContentDisposition = value; }
        }

        /// <summary>
        /// Gets the attachment content disposition.
        /// </summary>
        /// <param name="options">The formatter options</param>
        /// <returns>The attachment content disposition</returns>
        protected ExecutionLogAttachmentContentDisposition GetAttachmentContentDisposition(NameValueCollection options)
        {
            string option = options.Get(AttachmentContentDispositionOption);
            if (option != null)
            {
                try
                {
                    return (ExecutionLogAttachmentContentDisposition)Enum.Parse(typeof(ExecutionLogAttachmentContentDisposition), option, true);
                }
                catch (ArgumentException)
                {
                    // Ignore parse error.
                }
            }

            return defaultAttachmentContentDisposition;
        }

        /// <inheritdoc />
        public string Name
        {
            get { return name; }
        }

        /// <inheritdoc />
        public abstract void Format(IReportWriter reportWriter, NameValueCollection formatterOptions,
            IProgressMonitor progressMonitor);
    }
}
