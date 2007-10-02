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
using MbUnit.Framework.Kernel.ExecutionLogs;
using MbUnit.Framework.Kernel.Runtime;

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
        #region Current log writer accessor
        /// <summary>
        /// Gets the current log writer.
        /// </summary>
        /// <returns>The execution log, never null</returns>
        public static LogWriter Writer
        {
            get { return Context.CurrentContext.LogWriter; }
        }
        #endregion

        #region Current log writer stream accessors
        /// <summary>
        /// Gets the current stream writer for the built-in log stream where the console input
        /// stream for the test is recorded.
        /// </summary>
        public static LogStreamWriter ConsoleInput
        {
            get { return Writer[LogStreamNames.ConsoleInput]; }
        }

        /// <summary>
        /// Gets the current stream writer built-in log stream where the console output
        /// stream for the test is recorded.
        /// </summary>
        public static LogStreamWriter ConsoleOutput
        {
            get { return Writer[LogStreamNames.ConsoleOutput]; }
        }

        /// <summary>
        /// Gets the current stream writer built-in log stream where the console error
        /// stream for the test is recorded.
        /// </summary>
        public static LogStreamWriter ConsoleError
        {
            get { return Writer[LogStreamNames.ConsoleError]; }
        }

        /// <summary>
        /// Gets the current stream writer built-in log stream where assertion failures,
        /// exceptions and other failure data are recorded.
        /// </summary>
        public static LogStreamWriter Failures
        {
            get { return Writer[LogStreamNames.Failures]; }
        }

        /// <summary>
        /// Gets the current stream writer built-in log stream where debug information is recorded.
        /// </summary>
        public static LogStreamWriter Debug
        {
            get { return Writer[LogStreamNames.Debug]; }
        }

        /// <summary>
        /// Gets the current stream writer built-in log stream where diagnostic trace information is recorded.
        /// </summary>
        public static LogStreamWriter Trace
        {
            get { return Writer[LogStreamNames.Trace]; }
        }

        /// <summary>
        /// Gets the current stream writer built-in log stream where the output from the convenience methods
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
        /// <param name="text">The text to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogStreamWriter.EmbedPlainText"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is null</exception>
        public static TextAttachment AttachPlainText(string text)
        {
            return Writer.AttachPlainText(text);
        }

        /// <summary>
        /// Attaches an HTML attachment with mime-type <see cref="MimeTypes.Html" />.
        /// <para>
        /// This is a convenience method that forwards the request to the current log
        /// writer as returned by the <see cref="Writer" /> property.
        /// </para>
        /// </summary>
        /// <param name="html">The HTML to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogStreamWriter.EmbedHtml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="html"/> is null</exception>
        public static TextAttachment AttachHtml(string html)
        {
            return Writer.AttachHtml(html);
        }

        /// <summary>
        /// Attaches an XHTML attachment with mime-type <see cref="MimeTypes.XHtml" />.
        /// <para>
        /// This is a convenience method that forwards the request to the current log
        /// writer as returned by the <see cref="Writer" /> property.
        /// </para>
        /// </summary>
        /// <param name="xhtml">The XHTML to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogStreamWriter.EmbedXHtml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xhtml"/> is null</exception>
        public static XmlAttachment AttachXHtml(string xhtml)
        {
            return Writer.AttachXHtml(xhtml);
        }

        /// <summary>
        /// Attaches an XML attachment with mime-type <see cref="MimeTypes.Xml" />.
        /// <para>
        /// This is a convenience method that forwards the request to the current log
        /// writer as returned by the <see cref="Writer" /> property.
        /// </para>
        /// </summary>
        /// <param name="xml">The XML to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogStreamWriter.EmbedXml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xml"/> is null</exception>
        public static XmlAttachment AttachXml(string xml)
        {
            return Writer.AttachXml(xml);
        }

        /// <summary>
        /// Attaches an image attachment with a mime-type compatible with its internal representation.
        /// <para>
        /// This is a convenience method that forwards the request to the current log
        /// writer as returned by the <see cref="Writer" /> property.
        /// </para>
        /// </summary>
        /// <param name="image">The image to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogStreamWriter.EmbedImage"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="image"/> is null</exception>
        public static BinaryAttachment AttachImage(Image image)
        {
            return Writer.AttachImage(image);
        }

        /// <summary>
        /// Attaches an XML-serialized object as an XML attachment with mime-type <see cref="MimeTypes.Xml" />
        /// using the default <see cref="XmlSerializer" /> for the object's type.
        /// <para>
        /// This is a convenience method that forwards the request to the current log
        /// writer as returned by the <see cref="Writer" /> property.
        /// </para>
        /// </summary>
        /// <param name="obj">The object to serialize and embed, must not be null</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogStreamWriter.EmbedObjectAsXml(object)"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null</exception>
        public static XmlAttachment AttachObjectAsXml(object obj)
        {
            return Writer.AttachObjectAsXml(obj);
        }

        /// <summary>
        /// Attaches an XML-serialized object as an XML attachment with mime-type <see cref="MimeTypes.Xml" />
        /// using the specified <see cref="XmlSerializer" />.
        /// <para>
        /// This is a convenience method that forwards the request to the current log
        /// writer as returned by the <see cref="Writer" /> property.
        /// </para>
        /// </summary>
        /// <param name="obj">The object to serialize and embed, must not be null</param>
        /// <param name="xmlSerializer">The <see cref="XmlSerializer" /> to use, or null to use the default <see cref="XmlSerializer" />
        /// for the object's type</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogStreamWriter.EmbedObjectAsXml(object, XmlSerializer)"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null</exception>
        public static XmlAttachment AttachObjectAsXml(object obj, XmlSerializer xmlSerializer)
        {
            return Writer.AttachObjectAsXml(obj, xmlSerializer);
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
        /// Begins a section with the specified name.
        /// Execution log sections may be nested.
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        /// <param name="sectionName">The name of the section</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="sectionName"/> is null</exception>
        public static void BeginSection(string sectionName)
        {
            Default.BeginSection(sectionName);
        }

        /// <summary>
        /// Ends the current section.
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
        /// Embeds an plain text attachment with mime-type <see cref="MimeTypes.PlainText" />.
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        /// <param name="text">The text to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogWriter.AttachPlainText"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is null</exception>
        public static TextAttachment EmbedPlainText(string text)
        {
            return Default.EmbedPlainText(text);
        }

        /// <summary>
        /// Embeds an HTML attachment with mime-type <see cref="MimeTypes.Html" />.
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        /// <param name="html">The HTML to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogWriter.AttachHtml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="html"/> is null</exception>
        public static TextAttachment EmbedHtml(string html)
        {
            return Default.EmbedHtml(html);
        }

        /// <summary>
        /// Embeds an XHTML attachment with mime-type <see cref="MimeTypes.XHtml" />.
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        /// <param name="xhtml">The XHTML to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogWriter.AttachXHtml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xhtml"/> is null</exception>
        public static XmlAttachment EmbedXHtml(string xhtml)
        {
            return Default.EmbedXHtml(xhtml);
        }

        /// <summary>
        /// Embeds an XML attachment with mime-type <see cref="MimeTypes.Xml" />.
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        /// <param name="xml">The XML to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogWriter.AttachXml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xml"/> is null</exception>
        public static XmlAttachment EmbedXml(string xml)
        {
            return Default.EmbedXml(xml);
        }

        /// <summary>
        /// Embeds an image attachment with a mime-type compatible with its internal representation.
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        /// <param name="image">The image to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogWriter.AttachImage"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="image"/> is null</exception>
        public static BinaryAttachment EmbedImage(Image image)
        {
            return Default.EmbedImage(image);
        }

        /// <summary>
        /// Embeds an XML-serialized object as an XML attachment with mime-type <see cref="MimeTypes.Xml" />
        /// using the default <see cref="XmlSerializer" /> for the object's type.
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        /// <param name="obj">The object to serialize and embed, must not be null</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogWriter.AttachObjectAsXml(object)"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null</exception>
        public static XmlAttachment EmbedObjectAsXml(object obj)
        {
            return Default.EmbedObjectAsXml(obj);
        }

        /// <summary>
        /// Embeds an XML-serialized object as an XML attachment with mime-type <see cref="MimeTypes.Xml" />
        /// using the specified <see cref="XmlSerializer" />.
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </summary>
        /// <param name="obj">The object to serialize and embed, must not be null</param>
        /// <param name="xmlSerializer">The <see cref="XmlSerializer" /> to use, or null to use the default <see cref="XmlSerializer" />
        /// for the object's type</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogWriter.AttachObjectAsXml(object, XmlSerializer)"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null</exception>
        public static XmlAttachment EmbedObjectAsXml(object obj, XmlSerializer xmlSerializer)
        {
            return Default.EmbedObjectAsXml(obj, xmlSerializer);
        }
        #endregion
    }
}