using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using MbUnit.Framework.Services.Assertions;

namespace MbUnit.Framework.Exceptions
{
    /// <summary>
    /// The exception type used to signal MbUnit assertion failures.
    /// </summary>
    public class AssertionException : Exception
    {
        public AssertionException()
        {
        }

        public AssertionException(string message)
            : base(message)
        {
        }

        public AssertionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public AssertionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Creates an assertion exception to signal a failure of the specified
        /// assertion yielding a particular result.
        /// </summary>
        /// <param name="assertionResult">The assertion result, must not be null</param>
        public AssertionException(AssertionResult assertionResult)
            : base(assertionResult.ResultMessage, assertionResult.ResultException)
        {
            this.assertionResult = assertionResult;
        }

        /// <summary>
        /// Gets the associated assertion result or null if none was supplied during construction.
        /// </summary>
        public AssertionResult AssertionResult
        {
            get { return assertionResult; }
        }
        private AssertionResult assertionResult;
    }
}
