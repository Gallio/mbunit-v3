// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Model.Serialization;
using Gallio.Runner.Reports;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Base arguments for an event raised to indicate that an attachment has been added to a test step log.
    /// </summary>
    public abstract class TestStepLogAttachArgs : TestStepEventArgs
    {
        private readonly string attachmentName;
        private readonly string contentType;

        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="report">The report</param>
        /// <param name="test">The test data</param>
        /// <param name="testStepRun">The test step run</param>
        /// <param name="attachmentName">The attachment name</param>
        /// <param name="contentType">The content type</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report>"/>, <paramref name="test"/>
        /// <paramref name="testStepRun"/>, <paramref name="attachmentName"/> or <paramref name="contentType" /> is null</exception>
        protected TestStepLogAttachArgs(Report report, TestData test, TestStepRun testStepRun, string attachmentName, string contentType)
            : base(report, test, testStepRun)
        {
            if (attachmentName == null)
                throw new ArgumentNullException("attachmentName");
            if (contentType == null)
                throw new ArgumentNullException("contentType");

            this.attachmentName = attachmentName;
            this.contentType = contentType;
        }

        /// <summary>
        /// Gets the attachment name.
        /// </summary>
        public string AttachmentName
        {
            get { return attachmentName; }
        }

        /// <summary>
        /// Gets the attachment content type.
        /// </summary>
        public string ContentType
        {
            get { return contentType; }
        }
    }
}
