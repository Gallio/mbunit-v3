using System;
using Gallio.Model.Serialization;
using Gallio.Runner.Reports;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Arguments for an event raised to indicate that a text attachment has been added to a test step log.
    /// </summary>
    public sealed class TestStepLogTextAttachmentAddedEventArgs : TestStepLogAttachmentAddedEventArgs
    {
        private readonly string text;

        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="report">The report</param>
        /// <param name="test">The test data</param>
        /// <param name="testStepRun">The test step run</param>
        /// <param name="attachmentName">The attachment name</param>
        /// <param name="contentType">The content type</param>
        /// <param name="text">The attached text</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report>"/>, <paramref name="test"/>
        /// <paramref name="testStepRun"/>, <paramref name="attachmentName"/>, <paramref name="contentType" /> or <paramref name="text" /> is null</exception>
        public TestStepLogTextAttachmentAddedEventArgs(Report report, TestData test, TestStepRun testStepRun, string attachmentName, string contentType, string text)
            : base(report, test, testStepRun, attachmentName, contentType)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            this.text = text;
        }

        /// <summary>
        /// Gets the attached text.
        /// </summary>
        public string Text
        {
            get { return text; }
        }
    }
}