// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using System.Reflection;
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Common.Markup;
using Gallio.Common.Normalization;

namespace Gallio.Common.Diagnostics
{
    /// <summary>
    /// Describes an exception in a serializable form.
    /// </summary>
    [Serializable]
    public sealed class ExceptionData : IMarkupStreamWritable, INormalizable<ExceptionData>
    {
        private readonly string type;
        private readonly string message;
        private readonly StackTraceData stackTrace;
        private readonly ExceptionData innerException;
        private readonly PropertySet properties;

        /// <summary>
        /// Gets an empty read-only property set that can be used to indicate that
        /// an exception data object does not contain any properties.
        /// </summary>
        public static readonly PropertySet NoProperties
            = new PropertySet().AsReadOnly();

        /// <summary>
        /// Creates an exception data object from an exception.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Captures the exception type, message, stack trace and properties from
        /// the exception object.  To filter out specific properties, mark them with
        /// <see cref="SystemInternalAttribute"/>.
        /// </para>
        /// </remarks>
        /// <param name="exception">The exception.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/> is null.</exception>
        public ExceptionData(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            type = exception.GetType().FullName;
            message = ExceptionUtils.SafeGetMessage(exception);
            stackTrace = new StackTraceData(ExceptionUtils.SafeGetStackTrace(exception));
            properties = ExtractExceptionProperties(exception).AsReadOnly();

            if (exception.InnerException != null)
                innerException = new ExceptionData(exception.InnerException);
        }

        /// <summary>
        /// Creates an exception data object.
        /// </summary>
        /// <param name="type">The exception type full name.</param>
        /// <param name="message">The exception message text.</param>
        /// <param name="stackTrace">The exception stack trace.</param>
        /// <param name="properties">The exception properties, or <see cref="NoProperties"/>
        /// if none.</param>
        /// <param name="innerException">The inner exception data, or null if none.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/>,
        /// <paramref name="message"/>, <paramref name="properties"/>
        /// or <paramref name="stackTrace"/> is null.</exception>
        public ExceptionData(string type, string message, string stackTrace,
            PropertySet properties, ExceptionData innerException)
            : this(type, message, new StackTraceData(stackTrace), properties, innerException)
        {
        }

        /// <summary>
        /// Creates an exception data object.
        /// </summary>
        /// <param name="type">The exception type full name.</param>
        /// <param name="message">The exception message text.</param>
        /// <param name="stackTrace">The exception stack trace.</param>
        /// <param name="properties">The exception properties, or <see cref="NoProperties"/>
        /// if none.</param>
        /// <param name="innerException">The inner exception data, or null if none.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/>,
        /// <paramref name="message"/>, <paramref name="properties"/>
        /// or <paramref name="stackTrace"/> is null.</exception>
        public ExceptionData(string type, string message, StackTraceData stackTrace,
            PropertySet properties, ExceptionData innerException)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (message == null)
                throw new ArgumentNullException("message");
            if (stackTrace == null)
                throw new ArgumentNullException("stackTrace");
            if (properties == null)
                throw new ArgumentNullException("properties");

            this.type = type;
            this.message = message;
            this.stackTrace = stackTrace;
            this.properties = properties.Count == 0
                ? NoProperties
                : properties.AsReadOnly();
            this.innerException = innerException;
        }

        /// <summary>
        /// Gets the exception type full name.
        /// </summary>
        public string Type
        {
            get { return type; }
        }

        /// <summary>
        /// Gets the exception message text.
        /// </summary>
        public string Message
        {
            get { return message; }
        }

        /// <summary>
        /// Gets the exception stack trace.
        /// </summary>
        public StackTraceData StackTrace
        {
            get { return stackTrace; }
        }

        /// <summary>
        /// Gets the inner exception data, or null if none.
        /// </summary>
        public ExceptionData InnerException
        {
            get { return innerException; }
        }

        /// <summary>
        /// Gets the read-only set of formatted exception properties.
        /// </summary>
        public PropertySet Properties
        {
            get { return properties; }
        }

        /// <inheritdoc />
        public ExceptionData Normalize()
        {
            string normalizedType = NormalizationUtils.NormalizeName(type);
            string normalizedMessage = NormalizationUtils.NormalizeXmlText(message);
            StackTraceData normalizedStackTrace = stackTrace.Normalize();
            PropertySet normalizedProperties = NormalizationUtils.NormalizeCollection<PropertySet, KeyValuePair<string, string>>(properties,
                () => new PropertySet(),
                x => new KeyValuePair<string, string>(NormalizationUtils.NormalizeName(x.Key),
                    NormalizationUtils.NormalizeXmlText(x.Value)),
                (x, y) => x.Key == y.Key && x.Value == y.Value);
            ExceptionData normalizedInnerException = innerException != null ? innerException.Normalize() : null;

            if (ReferenceEquals(type, normalizedType)
                && ReferenceEquals(message, normalizedMessage)
                && ReferenceEquals(stackTrace, normalizedStackTrace)
                && ReferenceEquals(properties, normalizedProperties)
                && ReferenceEquals(innerException, normalizedInnerException))
                return this;

            return new ExceptionData(normalizedType, normalizedMessage, normalizedStackTrace, normalizedProperties, normalizedInnerException);
        }

        /// <summary>
        /// Formats the exception to a string similar to the one that the .Net framework
        /// would ordinarily construct.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The exception will not be terminated by a new line.
        /// </para>
        /// </remarks>
        /// <returns>The formatted exception.</returns>
        public override string ToString()
        {
            return ToString(false);
        }

        /// <summary>
        /// Formats the exception to a string similar to the one that the .Net framework
        /// would ordinarily construct.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The exception will not be terminated by a new line.
        /// </para>
        /// </remarks>
        /// <param name="useStandardFormatting">If true, strictly follows the standard .Net
        /// exception formatting by excluding the display of exception properties.</param>
        /// <returns>The formatted exception.</returns>
        public string ToString(bool useStandardFormatting)
        {
            StringMarkupDocumentWriter writer = new StringMarkupDocumentWriter(false);
            WriteTo(writer.Default, useStandardFormatting);
            return writer.ToString();
        }

        /// <summary>
        /// Writes the exception in a structured format with markers to distinguish its component elements.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The exception will not be terminated by a new line.
        /// </para>
        /// </remarks>
        /// <param name="writer">The log stream writer.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="writer"/> is null.</exception>
        public void WriteTo(MarkupStreamWriter writer)
        {
            WriteTo(writer, false);
        }

        /// <summary>
        /// Writes the exception in a structured format with markers to distinguish its component elements.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The exception will not be terminated by a new line.
        /// </para>
        /// </remarks>
        /// <param name="writer">The log stream writer.</param>
        /// <param name="useStandardFormatting">If true, strictly follows the standard .Net
        /// exception formatting by excluding the display of exception properties.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="writer"/> is null.</exception>
        public void WriteTo(MarkupStreamWriter writer, bool useStandardFormatting)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            using (writer.BeginMarker(Marker.Exception))
            {
                using (writer.BeginMarker(Marker.ExceptionType))
                    writer.Write(type);

                if (message.Length != 0)
                {
                    writer.Write(@": ");
                    using (writer.BeginMarker(Marker.ExceptionMessage))
                        writer.Write(message);
                }

                if (innerException != null)
                {
                    writer.Write(@" ---> ");
                    innerException.WriteTo(writer);
                    writer.Write(Environment.NewLine);
                    writer.Write(@"   --- ");
                    writer.Write("End of inner exception stack trace"); // todo localize me
                    writer.Write(@" ---");
                }

                if (!useStandardFormatting)
                {
                    foreach (KeyValuePair<string, string> property in properties)
                    {
                        writer.WriteLine();
                        using (writer.BeginMarker(Marker.ExceptionPropertyName))
                            writer.Write(property.Key);
                        writer.Write(@": ");
                        using (writer.BeginMarker(Marker.ExceptionPropertyValue))
                            writer.Write(property.Value);
                    }
                }

                if (!stackTrace.IsEmpty)
                {
                    writer.WriteLine();
                    stackTrace.WriteTo(writer);
                }
            }
        }

        private static PropertySet ExtractExceptionProperties(Exception exception)
        {
            PropertySet properties = null;

            foreach (PropertyInfo propertyInfo in exception.GetType().GetProperties())
            {
                string propertyName = propertyInfo.Name;
                if (IsBuiltInPropertyName(propertyName))
                    continue;

                if (IsIgnoredProperty(propertyInfo))
                    continue;

                if (propertyInfo.GetIndexParameters().Length != 0)
                    continue;

                string propertyValue;
                try
                {
                    object obj = propertyInfo.GetValue(exception, null);
                    if (obj == null)
                        continue;

                    propertyValue = obj.ToString();
                }
                catch
                {
                    continue;
                }

                if (properties == null)
                    properties = new PropertySet();

                if (! properties.ContainsKey(propertyName))
                    properties.Add(propertyName, propertyValue);
            }

            return properties ?? NoProperties;
        }

        private static bool IsBuiltInPropertyName(string propertyName)
        {
            switch (propertyName)
            {
                case "Data":
                case "Message":
                case "InnerException":
                case "Source":
                case "StackTrace":
                case "TargetSite":
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsIgnoredProperty(PropertyInfo propertyInfo)
        {
            return propertyInfo.IsDefined(typeof(SystemInternalAttribute), true);
        }
    }
}
