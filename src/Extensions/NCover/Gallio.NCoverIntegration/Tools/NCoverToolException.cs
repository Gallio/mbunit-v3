using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Gallio.NCoverIntegration.Tools
{
    /// <summary>
    /// The type of exception thrown when an NCover operation fails.
    /// </summary>
    [Serializable]
    public class NCoverToolException : Exception
    {
        /// <summary>
        /// Creates an exception.
        /// </summary>
        public NCoverToolException()
        {
        }

        /// <summary>
        /// Creates an exception.
        /// </summary>
        /// <param name="message">The message.</param>
        public NCoverToolException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public NCoverToolException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates an exception from serialization info.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected NCoverToolException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
