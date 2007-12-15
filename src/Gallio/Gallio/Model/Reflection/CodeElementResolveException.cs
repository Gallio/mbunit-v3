using System;
using System.Runtime.Serialization;

namespace Gallio.Model.Reflection
{
    /// <summary>
    /// The type of exception thrown when a <see cref="ICodeElementInfo" /> could not be resolved.
    /// </summary>
    [Serializable]
    public class CodeElementResolveException : Exception
    {
        /// <summary>
        /// Creates an exception.
        /// </summary>
        /// <param name="codeElement">The code element that could not be resolved</param>
        public CodeElementResolveException(ICodeElementInfo codeElement)
            : this(codeElement, null)
        {
        }

        /// <summary>
        /// Creates an exception.
        /// </summary>
        /// <param name="codeElement">The code element that could not be resolved</param>
        /// <param name="innerException">The inner exception</param>
        public CodeElementResolveException(ICodeElementInfo codeElement, Exception innerException)
            : base(String.Format("Could not resolve {0} '{1}'.", codeElement.Kind, codeElement), innerException)
        {
        }

        /// <summary>
        /// Creates an exception from serialization info.
        /// </summary>
        /// <param name="info">The serialization info</param>
        /// <param name="context">The streaming context</param>
        protected CodeElementResolveException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}