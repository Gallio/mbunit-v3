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
using Gallio.Common;
using Gallio.Common.Markup;

namespace Gallio.Common.Diagnostics
{
    /// <summary>
    /// Describes an exception in a serializable form.
    /// </summary>
    [Serializable]
    public sealed class ExceptionData : IMarkupStreamWritable
    {
        private readonly string type;
        private readonly string message;
        private readonly StackTraceData stackTrace;
        private readonly ExceptionData innerException;

        /// <summary>
        /// Creates an exception data object from an exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/> is null.</exception>
        public ExceptionData(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            type = exception.GetType().FullName;
            message = ExceptionUtils.SafeGetMessage(exception);
            stackTrace = new StackTraceData(ExceptionUtils.SafeGetStackTrace(exception));
            if (exception.InnerException != null)
                innerException = new ExceptionData(exception.InnerException);
        }

        /// <summary>
        /// Creates an exception data object.
        /// </summary>
        /// <param name="type">The exception type full name.</param>
        /// <param name="message">The exception message text.</param>
        /// <param name="stackTrace">The exception stack trace.</param>
        /// <param name="innerException">The inner exception data, or null if none.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/>,
        /// <paramref name="message"/> or <paramref name="stackTrace"/> is null.</exception>
        public ExceptionData(string type, string message, string stackTrace, ExceptionData innerException)
            : this(type, message, new StackTraceData(stackTrace), innerException)
        {
        }

        /// <summary>
        /// Creates an exception data object.
        /// </summary>
        /// <param name="type">The exception type full name.</param>
        /// <param name="message">The exception message text.</param>
        /// <param name="stackTrace">The exception stack trace.</param>
        /// <param name="innerException">The inner exception data, or null if none.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/>,
        /// <paramref name="message"/> or <paramref name="stackTrace"/> is null.</exception>
        public ExceptionData(string type, string message, StackTraceData stackTrace, ExceptionData innerException)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (message == null)
                throw new ArgumentNullException("message");
            if (stackTrace == null)
                throw new ArgumentNullException("stackTrace");

            this.type = type;
            this.message = message;
            this.stackTrace = stackTrace;
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
            StringMarkupDocumentWriter writer = new StringMarkupDocumentWriter(false);
            WriteTo(writer.Default);
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

                if (!stackTrace.IsEmpty)
                {
                    writer.WriteLine();
                    stackTrace.WriteTo(writer);
                }
            }
        }
    }
}
