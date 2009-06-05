// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Runtime.Serialization;

namespace Gallio.Reflection
{
    /// <summary>
    /// The type of exception thrown when a reflection wrapper could not be resolved
    /// to a native reflection target.
    /// </summary>
    [Serializable]
    public class ReflectionWrapperResolveException : Exception
    {
        /// <summary>
        /// Creates an exception.
        /// </summary>
        /// <param name="message">The message.</param>
        public ReflectionWrapperResolveException(string message)
            : base(message)
        {
        }
    
        /// <summary>
        /// Creates an exception for a code element that could not be resolved.
        /// </summary>
        /// <param name="codeElement">The code element that could not be resolved</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/> is nul</exception>
        public ReflectionWrapperResolveException(ICodeElementInfo codeElement)
            : this(codeElement, null)
        {
        }

        /// <summary>
        /// Creates an exception for a code element that could not be resolved.
        /// </summary>
        /// <param name="codeElement">The code element that could not be resolved</param>
        /// <param name="innerException">The inner exception, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/> is nul</exception>
        public ReflectionWrapperResolveException(ICodeElementInfo codeElement, Exception innerException)
            : base(FormatMessage(codeElement), innerException)
        {
        }

        /// <summary>
        /// Creates an exception for an attribute that could not be resolved.
        /// </summary>
        /// <param name="attribute">The attribute that could not be resolved</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attribute"/> is nul</exception>
        public ReflectionWrapperResolveException(IAttributeInfo attribute)
            : this(attribute, null)
        {
        }

        /// <summary>
        /// Creates an exception for an attribute that could not be resolved.
        /// </summary>
        /// <param name="attribute">The attribute that could not be resolved</param>
        /// <param name="innerException">The inner exception, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attribute"/> is nul</exception>
        public ReflectionWrapperResolveException(IAttributeInfo attribute, Exception innerException)
            : base(FormatMessage(attribute), innerException)
        {
        }

        /// <summary>
        /// Creates an exception from serialization info.
        /// </summary>
        /// <param name="info">The serialization info</param>
        /// <param name="context">The streaming context</param>
        protected ReflectionWrapperResolveException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        private static string FormatMessage(ICodeElementInfo codeElement)
        {
            if (codeElement == null)
                throw new ArgumentNullException("codeElement");

            return String.Format("Could not resolve {0} '{1}'.", codeElement.Kind, codeElement);
        }

        private static string FormatMessage(IAttributeInfo attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException("attribute");

            return String.Format("Could not resolve attribute of type '{0}'.", attribute.Type);
        }
    }
}
