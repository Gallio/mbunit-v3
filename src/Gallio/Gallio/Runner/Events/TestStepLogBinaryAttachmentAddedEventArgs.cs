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