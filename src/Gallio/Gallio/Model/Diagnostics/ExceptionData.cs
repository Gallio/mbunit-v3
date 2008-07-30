using System;
using System.IO;
using Gallio.Model.Logging;
using Gallio.Utilities;

namespace Gallio.Model.Diagnostics
{
    /// <summary>
    /// Provides raw serializable information about an exception.
    /// </summary>
    [Serializable]
    public sealed class ExceptionData
    {
        private readonly string type;
        private readonly string message;
        private readonly string stackTrace;
        private readonly ExceptionData innerException;

        /// <summary>
        /// Creates an exception data object from an exception.
        /// </summary>
        /// <param name="exception">The exception</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/> is null</exception>
        public ExceptionData(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            type = exception.GetType().FullName;
            message = ExceptionUtils.SafeGetMessage(exception);
            stackTrace = ExceptionUtils.SafeGetStackTrace(exception);
            if (exception.InnerException != null)
                innerException = new ExceptionData(exception.InnerException);
        }

        /// <summary>
        /// Creates an exception data object.
        /// </summary>
        /// <param name="type">The exception type full name</param>
        /// <param name="message">The exception message text</param>
        /// <param name="stackTrace">The formatted exception stack trace</param>
        /// <param name="innerException">The inner exception data, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/>,
        /// <paramref name="message"/> or <paramref name="stackTrace"/> is null</exception>
        public ExceptionData(string type, string message, string stackTrace, ExceptionData innerException)
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
        /// Gets the formatted exception stack trace.
        /// </summary>
        public string StackTrace
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
        /// <returns>The formatted exception</returns>
        public override string ToString()
        {
            StringTestLogWriter writer = new StringTestLogWriter(false);
            WriteTo(writer.Default);
            return writer.ToString();
        }

        /// <summary>
        /// Logs the exception with markers to distinguish its component elements.
        /// </summary>
        /// <param name="writer">The log stream writer</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="writer"/> is null</exception>
        public void WriteTo(TestLogStreamWriter writer)
        {
            using (writer.BeginMarker(MarkerClasses.Exception))
            {
                using (writer.BeginMarker(MarkerClasses.ExceptionType))
                    writer.Write(type);

                if (message.Length != 0)
                {
                    writer.Write(@": ");
                    using (writer.BeginMarker(MarkerClasses.ExceptionMessage))
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

                if (!String.IsNullOrEmpty(stackTrace))
                {
                    writer.Write(Environment.NewLine);
                    using (writer.BeginMarker(MarkerClasses.StackTrace))
                        writer.Write(stackTrace);
                }
            }
        }
    }
}