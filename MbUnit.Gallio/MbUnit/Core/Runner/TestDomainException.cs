using System;
using System.Runtime.Serialization;
using MbUnit.Core.Runner;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// The type of exception thrown when a <see cref="ITestDomain" />
    /// operation fails.
    /// </summary>
    [Serializable]
    public class TestDomainException : Exception
    {
        public TestDomainException()
        {
        }

        public TestDomainException(string message)
            : base(message)
        {
        }

        public TestDomainException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public TestDomainException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
