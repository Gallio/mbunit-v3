using System;
using System.Runtime.Serialization;

namespace Gallio.Ambience
{
    /// <summary>
    /// Describes a problem accessing Gallio Ambience.
    /// </summary>
    [Serializable]
    public class AmbienceException : Exception
    {
        /// <summary>
        /// Creates an exception.
        /// </summary>
        public AmbienceException()
        {
        }

        /// <summary>
        /// Creates an exception.
        /// </summary>
        /// <param name="message">The message</param>
        public AmbienceException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an exception.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="innerException">The inner exception</param>
        public AmbienceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates an exception from serialization info.
        /// </summary>
        /// <param name="info">The serialization info</param>
        /// <param name="context">The streaming context</param>
        protected AmbienceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
