// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Model.Serialization;
using Gallio.Runner.Reports;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Arguments for an event raised to indicate that a binary attachment has been added to a test step log.
    /// </summary>
    public sealed class TestStepLogBinaryAttachmentAddedEventArgs : TestStepLogAttachmentAddedEventArgs
    {
        private readonly byte[] bytes;

        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="report">The report</param>
        /// <param name="test">The test data</param>
        /// <param name="testStepRun">The test step run</param>
        /// <param name="attachmentName">The attachment name</param>
        /// <param name="contentType">The content type</param>
        /// <param name="bytes">The attached bytes</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report>"/>, <paramref name="test"/>
        /// <paramref name="testStepRun"/>, <paramref name="attachmentName"/>, <paramref name="contentType" /> or <paramref name="bytes" /> is null</exception>
        public TestStepLogBinaryAttachmentAddedEventArgs(Report report, TestData test, TestStepRun testStepRun, string attachmentName, string contentType, byte[] bytes)
            : base(report, test, testStepRun, attachmentName, contentType)
        {
            if (bytes == null)
                throw new ArgumentNullException("bytes");

            this.bytes = bytes;
        }

        /// <summary>
        /// Gets the attached bytes.
        /// </summary>
        public byte[] Bytes
        {
            get { return bytes; }
        }
    }
}
