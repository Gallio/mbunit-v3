using System;
using System.Runtime.Serialization;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// The type of exception thrown when the test runner or one of its
    /// supportive components like a test domain fails in an unrecoverable
    /// manner.  It can happen that the test results will be lost or incomplete.
    /// </summary>
    [Serializable]
    public class FatalRunnerException : Exception
    {
        public FatalRunnerException()
        {
        }

        public FatalRunnerException(string message)
            : base(message)
        {
        }

        public FatalRunnerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected FatalRunnerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
