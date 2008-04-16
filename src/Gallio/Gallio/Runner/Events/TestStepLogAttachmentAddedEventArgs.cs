using System;
using Gallio.Model.Serialization;
using Gallio.Runner.Reports;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Base arguments for an event raised to indicate that an attachment has been added to a test step log.
    /// </summary>
    public abstract class TestStepLogAttachmentAddedEventArgs : TestStepEventArgs
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
        protected TestStepLogAttachmentAddedEventArgs(Report report, TestData test, TestStepRun testStepRun, string attachmentName, string contentType)
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