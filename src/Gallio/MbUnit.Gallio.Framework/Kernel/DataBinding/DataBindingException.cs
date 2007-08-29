using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MbUnit.Framework.Kernel.DataBinding
{
    /// <summary>
    /// The exception type used to report data binding problems.
    /// </summary>
    [Serializable]
    public class DataBindingException : Exception
    {
        /// <summary>
        /// Creates an exception.
        /// </summary>
        public DataBindingException()
        {
        }

        /// <summary>
        /// Creates an exception.
        /// </summary>
        /// <param name="message">The message</param>
        public DataBindingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an exception.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="innerException">The inner exception</param>
        public DataBindingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates an exception from serialization info.
        /// </summary>
        /// <param name="info">The serialization info</param>
        /// <param name="context">The streaming context</param>
        protected DataBindingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
