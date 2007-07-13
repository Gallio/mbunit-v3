using System;
using System.Runtime.Serialization;

namespace MbUnit.Framework.Model
{
    /// <summary>
    /// The type of exception thrown when test model and test framework
    /// operations fail.
    /// </summary>
    [Serializable]
    public class ModelException : Exception
    {
        public ModelException()
        {
        }

        public ModelException(string message)
            : base(message)
        {
        }

        public ModelException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ModelException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
