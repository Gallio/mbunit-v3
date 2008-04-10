// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using Gallio.Reflection;
using Gallio.Utilities;

namespace Gallio.Model
{
    /// <summary>
    /// An annotation associates a message with a source code location
    /// to inform the user about a problem.
    /// </summary>
    [Serializable]
    public class Annotation
    {
        private readonly AnnotationType type;
        private readonly ICodeElementInfo codeElement;
        private readonly string message;
        private readonly string details;

        /// <summary>
        /// Creates an annotation with a detail message string.
        /// </summary>
        /// <param name="type">The annotation type</param>
        /// <param name="codeElement">The associated code element, or null if none</param>
        /// <param name="message">The annotation message</param>
        /// <param name="details">Additional details such as exception text or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> is null</exception>
        public Annotation(AnnotationType type, ICodeElementInfo codeElement, string message, string details)
        {
            this.type = type;
            this.codeElement = codeElement;
            this.message = message;
            this.details = details;
        }

        /// <summary>
        /// Creates an annotation with a detail exception.
        /// </summary>
        /// <param name="type">The annotation type</param>
        /// <param name="codeElement">The associated code element, or null if none</param>
        /// <param name="message">The annotation message</param>
        /// <param name="ex">The exception to use as additional details or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> is null</exception>
        public Annotation(AnnotationType type, ICodeElementInfo codeElement, string message, Exception ex)
            : this(type, codeElement, message, ex != null ? ExceptionUtils.SafeToString(ex) : null)
        {
        }

        /// <summary>
        /// Creates an annotation with no details.
        /// </summary>
        /// <param name="type">The annotation type</param>
        /// <param name="codeElement">The associated code element, or null if none</param>
        /// <param name="message">The annotation message</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> is null</exception>
        public Annotation(AnnotationType type, ICodeElementInfo codeElement, string message)
            : this(type, codeElement, message, (string) null)
        {
        }

        /// <summary>
        /// Gets the annotation type.
        /// </summary>
        public AnnotationType Type
        {
            get { return type; }
        }

        /// <summary>
        /// Gets the code element associated with the annotation.
        /// </summary>
        public ICodeElementInfo CodeElement
        {
            get { return codeElement; }
        }

        /// <summary>
        /// Gets the annotation message.
        /// </summary>
        public string Message
        {
            get { return message; }
        }

        /// <summary>
        /// Gets additional details such as exception text, or null if none.
        /// </summary>
        public string Details
        {
            get { return details; }
        }
    }
}
