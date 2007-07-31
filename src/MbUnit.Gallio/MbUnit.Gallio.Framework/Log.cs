// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using System.Drawing;
using System.Xml.Serialization;
using MbUnit.Framework.Services.ExecutionLogs;
using MbUnit.Framework.Services.Runtime;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// The log class provides services for incorporating customized information
    /// into the execution log for a test.  Using these services a test case or test framework
    /// may publish additional log data streams beyond those that are usually collected by MbUnit.
    /// </para>
    /// <para>
    /// An execution log records the output of a test during its execution including any text
    /// that was written to console output streams, exceptions that occurred, and 
    /// anything else the test writer might want to save.
    /// </para>
    /// <para>
    /// A log consists of zero or more log streams that are opened automatically
    /// on demand to capture independent sequences of log output.  Each stream can
    /// further be broken down into possibly nested sections to classify output
    /// during different phases of test execution (useful for drilling into complex tests).
    /// In addition to text, a log can contain attachments that are either attached
    /// at the top level of the log or embedded into log streams.  Attachments are
    /// typed by mime-type and can contain Text, Xml, Images, Blobs, or any other content.
    /// Certain test frameworks may automatically create attachments to gather all manner
    /// of diagnostic information over the course of the test.
    /// </para>
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Gets the execution log service for the current context.
        /// </summary>
        public static IExecutionLogService ExecutionLogService
        {
            get { return RuntimeHolder.Instance.Resolve<IExecutionLogService>(); }
        }

        /// <summary>
        /// Gets the built-in log stream where the console input
        /// stream for the test is recorded.
        /// </summary>
        public static IExecutionLogStream ConsoleInput
        {
            get { return Streams[ExecutionLogStreams.ConsoleInput]; }
        }

        /// <summary>
        /// Gets the built-in log stream where the console output
        /// stream for the test is recorded.
        /// </summary>
        public static IExecutionLogStream ConsoleOutput
        {
            get { return Streams[ExecutionLogStreams.ConsoleOutput]; }
        }

        /// <summary>
        /// Gets the built-in log stream where the console error
        /// stream for the test is recorded.
        /// </summary>
        public static IExecutionLogStream ConsoleError
        {
            get { return Streams[ExecutionLogStreams.ConsoleError]; }
        }

        /// <summary>
        /// Gets the built-in log stream where assertion failures,
        /// exceptions and other failure data are recorded.
        /// </summary>
        public static IExecutionLogStream Failures
        {
            get { return Streams[ExecutionLogStreams.Failures]; }
        }

        /// <summary>
        /// Gets the built-in log stream where debug information is recorded.
        /// </summary>
        public static IExecutionLogStream Debug
        {
            get { return Streams[ExecutionLogStreams.Debug]; }
        }

        /// <summary>
        /// Gets the built-in log stream where diagnostic trace information is recorded.
        /// </summary>
        public static IExecutionLogStream Trace
        {
            get { return Streams[ExecutionLogStreams.Trace]; }
        }

        /// <summary>
        /// Gets the current test execution log.
        /// </summary>
        /// <returns>The execution log, never null</returns>
        public static IExecutionLog Current
        {
            get { return ExecutionLogService.GetExecutionLog(Context.CurrentContext); }
        }

        /// <summary>
        /// Gets the lazily-populated collection of log streams for the execution log.
        /// Streams are automatically created on demand if no stream with the specified name
        /// exists at the time of the request.
        /// </summary>
        /// <returns>The log stream collection, never null</returns>
        public static IExecutionLogStreamCollection Streams
        {
            get { return Current.Streams; }
        }

        /// <summary>
        /// Attaches an attachment to the execution log.
        /// </summary>
        /// <remarks>
        /// Only one copy of an attachment instance is saved with an execution log even if
        /// <see cref="IExecutionLog.Attach" /> or <see cref="IExecutionLogStream.Embed" /> are
        /// called multiple times with the same instance.  However, an attachment instance
        /// can be embedded multiple times into multiple log streams since each
        /// embedded copy is represented as a link to the same common attachment instance.
        /// </remarks>
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
        /// <param name="xmlSerializer">The xml serializer to use, or null to use the default XmlSerializer
        /// for the object's type</param>
        /// <returns>The attachment</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null</exception>
        public static XmlAttachment AttachObjectAsXml(object obj, XmlSerializer xmlSerializer)
        {
            return Current.AttachObjectAsXml(obj, xmlSerializer);
        }
    }
}
