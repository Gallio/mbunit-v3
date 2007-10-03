using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MbUnit.Framework
{
    /// <summary>
    /// A client exception wraps an error that occurred within client code that
    /// the framework is interacting with.  When client exceptions are reported to
    /// the user, the description should should exclude details about what the framework
    /// was doing and focus purely on the problem that actually occurred within
    /// the client code to ensure that the information is relevant.
    /// </summary>
    [Serializable]
    public class ClientException : Exception
    {
        /// <summary>
        /// Creates an exception.
        /// </summary>
        public ClientException()
        {
        }

        /// <summary>
        /// Creates an exception.
        /// </summary>
        /// <param name="message">The message</param>
        public ClientException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an exception.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="innerException">The inner exception</param>
        public ClientException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates an exception from serialization info.
        /// </summary>
        /// <param name="info">The serialization info</param>
        /// <param name="context">The streaming context</param>
        protected ClientException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
