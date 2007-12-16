using System;
using System.Runtime.Serialization;

namespace Gallio.Model
{
    /// <summary>
    /// <para>
    /// The type of exception thrown when a test declaration is malformed or
    /// contains an error.  Test framework authors may use this exception to
    /// signal that the user provided an invalid test declaration in the code.
    /// </para>
    /// <para>
    /// When you see this error, check to make sure that the syntax of the test
    /// is correct, all required parameters have been provided and they contain
    /// valid values.
    /// </para>
    /// </summary>
    [Serializable]
    public class TestDeclarationErrorException : ModelException
    {
        /// <summary>
        /// Creates an exception.
        /// </summary>
        public TestDeclarationErrorException()
        {
        }

        /// <summary>
        /// Creates an exception.
        /// </summary>
        /// <param name="message">The message</param>
        public TestDeclarationErrorException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an exception.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="innerException">The inner exception</param>
        public TestDeclarationErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates an exception from serialization info.
        /// </summary>
        /// <param name="info">The serialization info</param>
        /// <param name="context">The streaming context</param>
        protected TestDeclarationErrorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}