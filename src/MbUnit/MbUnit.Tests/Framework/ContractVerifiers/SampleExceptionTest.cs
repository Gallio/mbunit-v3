using System;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using System.Runtime.Serialization;

namespace MbUnit.Tests.Framework.ContractVerifiers
{
    [TestFixture]
    [VerifyExceptionContract(typeof(SampleException), 
        ImplementsSerialization = true, 
        ImplementsStandardConstructors = true)]
    public class SampleExceptionTest
    {
    }

    [Serializable]
    public class SampleException : Exception, ISerializable
    {
        public SampleException()
        {
        }

        public SampleException(string message) : base(message)
        {
        }

        public SampleException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        protected SampleException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
