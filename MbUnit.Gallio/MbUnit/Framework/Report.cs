using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml.Serialization;
using MbUnit.Core.Collections;
using MbUnit.Core.Runtime;
using MbUnit.Core.Services;
using MbUnit.Core.Services.Report;
using MbUnit.Core.Services.Report.Attachments;

namespace MbUnit.Framework
{
    /// <summary>
    /// The report class provides services for incorporating customized information
    /// into a report.  Using these services a test case or test framework may publish
    /// additional report data streams beyond those that are usually collected by MbUnit.
    /// </summary>
    public static class Report
    {
        /// <summary>
        /// The name of the built-in report stream where the
        /// console input stream for the test is copied.
        /// </summary>
        public const string ConsoleInputStreamName = "ConsoleInput";

        /// <summary>
        /// The name of the built-in report stream where the
        /// console output stream from the test is copied.
        /// </summary>
        public const string ConsoleOutputStreamName = "ConsoleOutput";

        /// <summary>
        /// The name of the built-in report stream where the
        /// console error stream from the test is copied.
        /// </summary>
        public const string ConsoleErrorStreamName = "ConsoleError";

        /// <summary>
        /// The name of the built-in report stream where debug information is reported.
        /// </summary>
        public const string DebugStreamName = "Debug";

        /// <summary>
        /// The name of the built-in report stream where diagnostic trace information is reported.
        /// </summary>
        public const string TraceStreamName = "Trace";

        /// <summary>
        /// The name of the built-in report stream where assertion failures,
        /// exceptions and other failure data are reported.
        /// </summary>
        public const string FailureStreamName = "Failure";

        /// <summary>
        /// Gets the reporting service for the current context.
        /// </summary>
        public static IReportService ReportingService
        {
            get { return Runtime.Instance.ReportingService; }
        }

        /// <summary>
        /// Gets the built-in report stream where the console input
        /// stream for the test is copied.
        /// </summary>
        public static IReportStream ConsoleInput
        {
            get { return Streams[ConsoleInputStreamName]; }
        }

        /// <summary>
        /// Gets the built-in report stream where the console output
        /// stream for the test is copied.
        /// </summary>
        public static IReportStream ConsoleOutput
        {
            get { return Streams[ConsoleInputStreamName]; }
        }

        /// <summary>
        /// Gets the built-in report stream where the console error
        /// stream for the test is copied.
        /// </summary>
        public static IReportStream ConsoleError
        {
            get { return Streams[ConsoleErrorStreamName]; }
        }

        /// <summary>
        /// Gets the built-in report stream where assertion failures,
        /// exceptions and other failure data are reported.
        /// </summary>
        public static IReportStream Failure
        {
            get { return Streams[FailureStreamName]; }
        }

        /// <summary>
        /// Gets the built-in report stream where debug information is reported.
        /// </summary>
        public static IReportStream Debug
        {
            get { return Streams[DebugStreamName]; }
        }

        /// <summary>
        /// Gets the built-in report stream where diagnostic trace information is reported.
        /// </summary>
        public static IReportStream Trace
        {
            get { return Streams[TraceStreamName]; }
        }

        /// <summary>
        /// Gets the current test report.
        /// </summary>
        /// <returns>The report, never null</returns>
        public static IReport Current
        {
            get { return ReportingService.GetReport(Context.CurrentContext); }
        }

        /// <summary>
        /// Gets the lazily-populated collection of report streams for the report.
        /// Streams are automatically created on demand if no stream with the specified name
        /// exists at the time of the request.
        /// </summary>
        /// <returns>The report stream collection, never null</returns>
        public static IReportStreamCollection Streams
        {
            get { return Current.Streams; }
        }

        /// <summary>
        /// Gets the collection of attachments in the report.
        /// </summary>
        public static IAttachmentCollection Attachments
        {
            get { return Current.Attachments; }
        }

        /// <summary>
        /// Attaches an attachment to the report.
        /// If the attachment has already been attached to the report, does nothing.
        /// </summary>
        /// <param name="attachment">The attachment to include</param>
        /// <returns>The attachment</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attachment"/> is null</exception>
        public static Attachment Attach(Attachment attachment)
        {
            return Current.Attach(attachment);
        }

        /// <summary>
        /// Attaches an plain text attachment with mime-type <see cref="MimeTypes.PlainText" />.
        /// </summary>
        /// <param name="text">The text to attach</param>
        /// <returns>The attachment</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is null</exception>
        public static TextAttachment AttachPlainText(string text)
        {
            return Current.AttachPlainText(text);
        }

        /// <summary>
        /// Attaches an HTML attachment with mime-type <see cref="MimeTypes.Html" />.
        /// </summary>
        /// <param name="html">The HTML to attach</param>
        /// <returns>The attachment</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="html"/> is null</exception>
        public static TextAttachment AttachHtml(string html)
        {
            return Current.AttachHtml(html);
        }

        /// <summary>
        /// Attaches an HTML attachment with mime-type <see cref="MimeTypes.XHtml" />.
        /// </summary>
        /// <param name="xhtml">The XHTML to attach</param>
        /// <returns>The attachment</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xhtml"/> is null</exception>
        public static XmlAttachment AttachXHtml(string xhtml)
        {
            return Current.AttachXHtml(xhtml);
        }

        /// <summary>
        /// Attaches an XML attachment with mime-type <see cref="MimeTypes.Xml" />.
        /// </summary>
        /// <param name="xml">The XML to attach</param>
        /// <returns>The attachment</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xml"/> is null</exception>
        public static XmlAttachment AttachXml(string xml)
        {
            return Current.AttachXml(xml);
        }

        /// <summary>
        /// Attaches an image attachment with a mime-type compatible with its internal representation.
        /// </summary>
        /// <param name="image">The image to attach</param>
        /// <returns>The attachment</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="image"/> is null</exception>
        public static BinaryAttachment AttachImage(Image image)
        {
            return Current.AttachImage(image);
        }

        /// <summary>
        /// Attaches an XML-serialized object as an XML attachment with mime-type <see cref="MimeTypes.Xml" />
        /// using the specified serializer.
        /// <seealso cref="XmlSerializer"/>
        /// </summary>
        /// <param name="obj">The object to serialize and embed, must not be null</param>
        /// <param name="xmlSerializer">The xml serializer to use, or null to use the default based on the object's type</param>
        /// <returns>The attachment</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null</exception>
        public static XmlAttachment AttachObjectAsXml(object obj, XmlSerializer xmlSerializer)
        {
            return Current.AttachObjectAsXml(obj, xmlSerializer);
        }
    }
}
