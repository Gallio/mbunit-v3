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
using System.Diagnostics;
using System.Drawing;
using System.Xml.Serialization;
using Gallio.Logging;
using Gallio.Model.Execution;

namespace Gallio.Logging
{
    /// <summary>
    /// <para>
    /// The log class provides services for writing information to the
    /// execution log associated with a test.
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
        #region Current log writer accessor
        /// <summary>
        /// Gets the current log writer.
        /// </summary>
        /// <returns>The execution log, never null</returns>
        /// <exception cref="InvalidOperationException">Thrown if there is no current log writer</exception>
        public static LogWriter Writer
        {
            get
            {
                ITestContext context = TestContextTrackerAccessor.GetInstance().CurrentContext;
                if (context == null)
                    throw new InvalidOperationException("There is no test context currently available.  Consequently there is no current log writer.");
                return context.LogWriter;
            }
        }
        #endregion

        #region Current log writer stream accessors
        /// <summary>
        /// Gets the current stream writer for the built-in log stream where the <see cref="Console.In" />
        /// stream for the test is recorded.
        /// </summary>
        public static LogStreamWriter ConsoleInput
        {
            get { return Writer[LogStreamNames.ConsoleInput]; }
        }

        /// <summary>
        /// Gets the current stream writer for the built-in log stream where the <see cref="Console.Out" />
        /// stream for the test is recorded.
        /// </summary>
        public static LogStreamWriter ConsoleOutput
        {
            get { return Writer[LogStreamNames.ConsoleOutput]; }
        }

        /// <summary>
        /// Gets the current stream writer for the built-in log stream where the <see cref="Console.Error" />
        /// stream for the test is recorded.
        /// </summary>
        public static LogStreamWriter ConsoleError
        {
            get { return Writer[LogStreamNames.ConsoleError]; }
        }

        /// <summary>
        /// Gets the current stream writer for the built-in log stream where diagnostic <see cref="Debug" />
        /// and <see cref="Trace" /> information is recorded.
        /// </summary>
        public static LogStreamWriter DebugTrace
        {
            get { return Writer[LogStreamNames.DebugTrace]; }
        }

        /// <summary>
        /// Gets the current stream writer for the built-in log stream where assertion failures,
        /// exceptions and other failure data are recorded.
        /// </summary>
        public static LogStreamWriter Failures
        {
            get { return Writer[LogStreamNames.Failures]; }
        }

        /// <summary>
        /// Gets the current stream writer for the built-in log stream where warnings are recorded.
        /// </summary>
        public static LogStreamWriter Warnings
        {
            get { return Writer[LogStreamNames.Warnings]; }
        }

        /// <summary>
        /// Gets the current stream writer for the built-in log stream where the output from the convenience methods
        /// of the <see cref="Log" /> class is recorded.
        /// </summary>
        public static LogStreamWriter Default
        {
            get { return Writer[LogStreamNames.Default]; }
        }
        #endregion

        #region Current log writer shortcuts
        /// <summary>
        /// Attaches an attachment to the execution log.
        /// <para>
        /// This is a convenience method that forwards the request to the current log
        /// writer as returned by the <see cref="Writer" /> property.
        /// </para>
        /// </summary>
        /// <remarks>
        /// Only one copy of an attachment instance is saved with an execution log even if
        /// <see cref="LogWriter.Attach" /> or <see cref="LogStreamWriter.Embed" /> are
        /// called multiple times with the same instance.  However, an attachment instance
        /// can be embedded multiple times into multiple log streams since each
        /// embedded copy is represented as a link to the same common attachment instance.
        /// </remarks>
        /// <param name="attachment">The attachment to include</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogStreamWriter.Embed"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attachment"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if a different attachment instance
        /// with the same name was already attached or embedded</exception>
        public static Attachment Attach(Attachment attachment)
        {
            return Writer.Attach(attachment);
        }

        /// <summary>
        /// Attaches an plain text attachment with mime-type <see cref="MimeTypes.PlainText" />.
        /// <para>
        /// This is a convenience method that forwards the request to the current log
        /// writer as returned by the <see cref="Writer" /> property.
        /// </para>
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="text">The text to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogStreamWriter.EmbedPlainText"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is null</exception>
        public static TextAttachment AttachPlainText(string attachmentName, string text)
        {
            return Writer.AttachPlainText(attachmentName, text);
        }

        /// <summary>
        /// Attaches an HTML attachment with mime-type <see cref="MimeTypes.Html" />.
        /// <para>
        /// This is a convenience method that forwards the request to the current log
        /// writer as returned by the <see cref="Writer" /> property.
        /// </para>
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="html">The HTML to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogStreamWriter.EmbedHtml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="html"/> is null</exception>
        public static TextAttachment AttachHtml(string attachmentName, string html)
        {
            return Writer.AttachHtml(attachmentName, html);
        }

        /// <summary>
        /// Attaches an XHTML attachment with mime-type <see cref="MimeTypes.XHtml" />.
        /// <para>
        /// This is a convenience method that forwards the request to the current log
        /// writer as returned by the <see cref="Writer" /> property.
        /// </para>
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="xhtml">The XHTML to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogStreamWriter.EmbedXHtml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xhtml"/> is null</exception>
        public static TextAttachment AttachXHtml(string attachmentName, string xhtml)
        {
            return Writer.AttachXHtml(attachmentName, xhtml);
        }

        /// <summary>
        /// Attaches an XML attachment with mime-type <see cref="MimeTypes.Xml" />.
        /// <para>
        /// This is a convenience method that forwards the request to the current log
        /// writer as returned by the <see cref="Writer" /> property.
        /// </para>
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="xml">The XML to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogStreamWriter.EmbedXml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xml"/> is null</exception>
        public static TextAttachment AttachXml(string attachmentName, string xml)
        {
            return Writer.AttachXml(attachmentName, xml);
        }

        /// <summary>
        /// Attaches an image attachment with a mime-type compatible with its internal representation.
        /// <para>
        /// This is a convenience method that forwards the request to the current log
        /// writer as returned by the <see cref="Writer" /> property.
        /// </para>
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="image">The image to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogStreamWriter.EmbedImage"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="image"/> is null</exception>
        public static BinaryAttachment AttachImage(string attachmentName, Image image)
        {
            return Writer.AttachImage(attachmentName, image);
        }

        /// <summary>
        /// Attaches an XML-serialized object as an XML attachment with mime-type <see cref="MimeTypes.Xml" />
        /// using the default <see cref="XmlSerializer" /> for the object's type.
        /// <para>
        /// This is a convenience method that forwards the request to the current log
        /// writer as returned by the <see cref="Writer" /> property.
        /// </para>
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="obj">The object to serialize and embed, must not be null</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogStreamWriter.EmbedObjectAsXml(string, object)"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null</exception>
        public static TextAttachment AttachObjectAsXml(string attachmentName, object obj)
        {
            return Writer.AttachObjectAsXml(attachmentName, obj);
        }

        /// <summary>
        /// Attaches an XML-serialized object as an XML attachment with mime-type <see cref="MimeTypes.Xml" />
        /// using the specified <see cref="XmlSerializer" />.
        /// <para>
        /// This is a convenience method that forwards the request to the current log
        /// writer as returned by the <see cref="Writer" /> property.
        /// </para>
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="obj">The object to serialize and embed, must not be null</param>
        /// <param name="xmlSerializer">The <see cref="XmlSerializer" /> to use, or null to use the default <see cref="XmlSerializer" />
        /// for the object's type</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogStreamWriter.EmbedObjectAsXml(string, object, XmlSerializer)"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null</exception>
        public static TextAttachment AttachObjectAsXml(string attachmentName, object obj, XmlSerializer xmlSerializer)
        {
            return Writer.AttachObjectAsXml(attachmentName, obj, xmlSerializer);
        }
        #endregion

        #region Current default log stream writer shortcuts
        /// <summary>
        /// Writes a character to the stream.
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        /// <param name="value">The character value</param>
        public static void Write(char value)
        {
            Default.Write(value);
        }

        /// <summary>
        /// Writes a string to the stream.
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        /// <param name="value">The string value</param>
        public static void Write(string value)
        {
            Default.Write(value);
        }

        /// <summary>
        /// Writes a formatted object to the stream.
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        /// <param name="value">The object value</param>
        public static void Write(object value)
        {
            Default.Write(value);
        }

        /// <summary>
        /// Writes an array of characters to the stream.
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        /// <param name="value">The array of characters</param>
        public static void Write(char[] value)
        {
            Default.Write(value);
        }

        /// <summary>
        /// Writes an array of characters to the stream.
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        /// <param name="buffer">The character buffer</param>
        /// <param name="index">The index of the first character in the buffer to write</param>
        /// <param name="count">The number of characters from the buffer to write</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="buffer"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> or <paramref name="count"/> are out of range</exception>
        public static void Write(char[] buffer, int index, int count)
        {
            Default.Write(buffer, index, count);
        }

        /// <summary>
        /// Writes a formatted string to the stream.
        /// <seealso cref="String.Format(string, object[])"/>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        /// <param name="format">The format string</param>
        /// <param name="args">The format string arguments</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="format"/> is null</exception>
        public static void Write(string format, params object[] args)
        {
            Default.Write(format, args);
        }

        /// <summary>
        /// Writes a line delimiter to the stream.
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        public static void WriteLine()
        {
            Default.WriteLine();
        }

        /// <summary>
        /// Writes a character to the stream followed by a line delimiter.
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        /// <param name="value">The character value</param>
        public static void WriteLine(char value)
        {
            Default.WriteLine(value);
        }

        /// <summary>
        /// Writes a string to the stream followed by a line delimiter.
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        /// <param name="value">The string value</param>
        public static void WriteLine(string value)
        {
            Default.WriteLine(value);
        }

        /// <summary>
        /// Writes a formatted object to the stream followed by a line delimiter.
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        /// <param name="value">The object value</param>
        public static void WriteLine(object value)
        {
            Default.WriteLine(value);
        }

        /// <summary>
        /// Writes an array of characters to the stream followed by a line delimiter.
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        /// <param name="value">The array of characters</param>
        public static void WriteLine(char[] value)
        {
            Default.WriteLine(value);
        }

        /// <summary>
        /// Writes a formatted string to the stream followed by a line delimiter.
        /// <seealso cref="String.Format(string, object[])"/>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        /// <param name="format">The format string</param>
        /// <param name="args">The format string arguments</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="format"/> is null</exception>
        public static void WriteLine(string format, params object[] args)
        {
            Default.WriteLine(format, args);
        }

        /// <summary>
        /// Writes an exception to the log within its own section with the name "Exception".
        /// </summary>
        /// <param name="exception">The exception to write</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/> is null</exception>
        public static void WriteException(Exception exception)
        {
            Default.WriteException(exception);
        }

        /// <summary>
        /// Writes an exception to the log within its own section with the specified name.
        /// </summary>
        /// <param name="exception">The exception to write</param>
        /// <param name="sectionName">The section name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/>,
        /// or <paramref name="sectionName"/> is null</exception>
        public static void WriteException(Exception exception, string sectionName)
        {
            Default.WriteException(exception, sectionName);
        }

        /// <summary>
        /// Writes an exception to the log within its own section with the specified name.
        /// </summary>
        /// <param name="exception">The exception to write</param>
        /// <param name="sectionNameFormat">The section name format string</param>
        /// <param name="sectionNameArgs">The section name arguments</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/>,
        /// <paramref name="sectionNameFormat"/> or <paramref name="sectionNameArgs"/> is null</exception>
        public static void WriteException(Exception exception, string sectionNameFormat, params object[] sectionNameArgs)
        {
            Default.WriteException(exception, sectionNameFormat, sectionNameArgs);
        }

        /// <summary>
        /// <para>
        /// Begins a section with the specified name.
        /// Execution log sections may be nested.
        /// </para>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        /// <example>
        /// <code>
        /// using (Log.BeginSection("Doing something interesting"))
        /// {
        ///     Log.WriteLine("Ah ha!");
        /// }
        /// </code>
        /// </example>
        /// <param name="sectionName">The name of the section</param>
        /// <returns>A Disposable object that calls <see cref="EndSection" /> when disposed.  This
        /// is a convenience for using the C# "using" statement to contain log stream sections.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="sectionName"/> is null</exception>
        public static IDisposable BeginSection(string sectionName)
        {
            return Default.BeginSection(sectionName);
        }

        /// <summary>
        /// <para>
        /// Ends the current section.
        /// </para>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if there is no current section</exception>
        public static void EndSection()
        {
            Default.EndSection();
        }

        /// <summary>
        /// Embeds an attachment into the stream.
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        /// <remarks>
        /// Only one copy of an attachment instance is saved with an execution log even if
        /// <see cref="LogWriter.Attach" /> or <see cref="LogStreamWriter.Embed" /> are
        /// called multiple times with the same instance.  However, an attachment instance
        /// can be embedded multiple times into multiple execution log streams since each
        /// embedded copy is represented as a link to the same common attachment instance.
        /// </remarks>
        /// <param name="attachment">The attachment to embed</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogWriter.Attach"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attachment"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if a different attachment instance
        /// with the same name was already attached or embedded</exception>
        public static Attachment Embed(Attachment attachment)
        {
            return Default.Embed(attachment);
        }

        /// <summary>
        /// Embeds an existing attachment into the stream.  This method can be used to
        /// repeatedly embed an existing attachment at multiple points in multiple
        /// streams without needing to keep the <see cref="Attachment" /> instance
        /// itself around.  This can help to reduce memory footprint since the
        /// original <see cref="Attachment" /> instance can be garbage collected shortly
        /// after it is first attached.
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        /// <remarks>
        /// Only one copy of an attachment instance is saved with an execution log even if
        /// <see cref="LogWriter.Attach" /> or <see cref="LogStreamWriter.Embed" /> are
        /// called multiple times with the same instance.  However, an attachment instance
        /// can be embedded multiple times into multiple execution log streams since each
        /// embedded copy is represented as a link to the same common attachment instance.
        /// </remarks>
        /// <param name="attachmentName">The name of the existing attachment to embed</param>
        /// <seealso cref="LogWriter.Attach"/>
        /// <seealso cref="LogStreamWriter.Embed"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attachmentName"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if no attachment with the specified
        /// name has been attached to the log</exception>
        public static void EmbedExisting(string attachmentName)
        {
            Default.EmbedExisting(attachmentName);
        }

        /// <summary>
        /// Embeds an plain text attachment with mime-type <see cref="MimeTypes.PlainText" />.
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="text">The text to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogWriter.AttachPlainText"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is null</exception>
        public static TextAttachment EmbedPlainText(string attachmentName, string text)
        {
            return Default.EmbedPlainText(attachmentName, text);
        }

        /// <summary>
        /// Embeds an HTML attachment with mime-type <see cref="MimeTypes.Html" />.
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="html">The HTML to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogWriter.AttachHtml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="html"/> is null</exception>
        public static TextAttachment EmbedHtml(string attachmentName, string html)
        {
            return Default.EmbedHtml(attachmentName, html);
        }

        /// <summary>
        /// Embeds an XHTML attachment with mime-type <see cref="MimeTypes.XHtml" />.
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="xhtml">The XHTML to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogWriter.AttachXHtml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xhtml"/> is null</exception>
        public static TextAttachment EmbedXHtml(string attachmentName, string xhtml)
        {
            return Default.EmbedXHtml(attachmentName, xhtml);
        }

        /// <summary>
        /// Embeds an XML attachment with mime-type <see cref="MimeTypes.Xml" />.
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="xml">The XML to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogWriter.AttachXml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xml"/> is null</exception>
        public static TextAttachment EmbedXml(string attachmentName, string xml)
        {
            return Default.EmbedXml(attachmentName, xml);
        }

        /// <summary>
        /// Embeds an image attachment with a mime-type compatible with its internal representation.
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="image">The image to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogWriter.AttachImage"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="image"/> is null</exception>
        public static BinaryAttachment EmbedImage(string attachmentName, Image image)
        {
            return Default.EmbedImage(attachmentName, image);
        }

        /// <summary>
        /// Embeds an XML-serialized object as an XML attachment with mime-type <see cref="MimeTypes.Xml" />
        /// using the default <see cref="XmlSerializer" /> for the object's type.
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="obj">The object to serialize and embed, must not be null</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogWriter.AttachObjectAsXml(string, object)"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null</exception>
        public static TextAttachment EmbedObjectAsXml(string attachmentName, object obj)
        {
            return Default.EmbedObjectAsXml(attachmentName, obj);
        }

        /// <summary>
        /// Embeds an XML-serialized object as an XML attachment with mime-type <see cref="MimeTypes.Xml" />
        /// using the specified <see cref="XmlSerializer" />.
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="obj">The object to serialize and embed, must not be null</param>
        /// <param name="xmlSerializer">The <see cref="XmlSerializer" /> to use, or null to use the default <see cref="XmlSerializer" />
        /// for the object's type</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogWriter.AttachObjectAsXml(string, object, XmlSerializer)"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null</exception>
        public static TextAttachment EmbedObjectAsXml(string attachmentName, object obj, XmlSerializer xmlSerializer)
        {
            return Default.EmbedObjectAsXml(attachmentName, obj, xmlSerializer);
        }
        #endregion
    }
}