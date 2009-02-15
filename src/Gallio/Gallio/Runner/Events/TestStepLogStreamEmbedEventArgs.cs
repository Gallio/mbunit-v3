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
using Gallio.Model.Serialization;
using Gallio.Runner.Reports;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Arguments for an event raised to indicate that an attachment has been embedded into a test step log stream.
    /// </summary>
    public sealed class TestStepLogStreamEmbedEventArgs : TestStepLogStreamEventArgs
    {
        private readonly string attachmentName;

        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="report">The report</param>
        /// <param name="test">The test data</param>
        /// <param name="testStepRun">The test step run</param>
        /// <param name="logStreamName">The log stream name</param>
        /// <param name="attachmentName">The name of the attachment that was embedded</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report"/>, <paramref name="test"/>
        /// <paramref name="testStepRun"/>, <paramref name="logStreamName"/>, or <paramref name="attachmentName"/> is null</exception>
        public TestStepLogStreamEmbedEventArgs(Report report, TestData test, TestStepRun testStepRun, string logStreamName, string attachmentName)
            : base(report, test, testStepRun, logStreamName)
        {
            if (attachmentName == null)
                throw new ArgumentNullException("attachmentName");

            this.attachmentName = attachmentName;
        }

        /// <summary>
        /// Gets the name of the attachment that was embedded.
        /// </summary>
        public string AttachmentName
        {
            get { return attachmentName; }
        }
    }
}
