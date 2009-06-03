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
using Gallio.Common.Diagnostics;
using Gallio.Common.Reflection;
using Gallio.Framework.Pattern;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// The <see cref="PatternAttribute" /> class is the base class for all pattern test framework
    /// attributes.  It associates a code element with a <see cref="IPattern" /> for building
    /// up parts of the test model using reflection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Subclasses of <see cref="IPattern" /> define simpler interfaces for implementing
    /// the semantics of common types of attributes such as test factories, decorators,
    /// and data providers.  Refer to the documentation of each subclass for details on its use.
    /// </para>
    /// </remarks>
    /// <seealso cref="IPattern"/>
    /// <seealso cref="PatternTestFramework"/>
    [SystemInternal]
    public abstract class PatternAttribute : Attribute, IPattern
    {
        /// <inheritdoc />
        public virtual bool IsPrimary
        {
            get { return false; }
        }

        /// <inheritdoc />
        public virtual bool IsTest(IPatternEvaluator evaluator, ICodeElementInfo codeElement)
        {
            return false;
        }

        /// <inheritdoc />
        public virtual bool IsTestPart(IPatternEvaluator evaluator, ICodeElementInfo codeElement)
        {
            return IsTest(evaluator, codeElement);
        }

        /// <inheritdoc />
        public virtual void Consume(IPatternScope containingScope, ICodeElementInfo codeElement, bool skipChildren)
        {
        }

        /// <inheritdoc />
        public virtual void Process(IPatternScope scope, ICodeElementInfo codeElement)
        {
        }

        /// <summary>
        /// Throws a <see cref="PatternUsageErrorException" /> with the specified message
        /// including a short heading that identifies the attribute type.
        /// </summary>
        /// <param name="message">The message, not null.</param>
        protected void ThrowUsageErrorException(string message)
        {
            ThrowUsageErrorException(message, null);
        }

        /// <summary>
        /// Throws a <see cref="PatternUsageErrorException" /> with the specified message
        /// including a short heading that identifies the attribute type.
        /// </summary>
        /// <param name="message">The message, not null.</param>
        /// <param name="ex">The associated exception, or null if none.</param>
        protected void ThrowUsageErrorException(string message, Exception ex)
        {
            throw new PatternUsageErrorException(String.Format("[{0}] - {1}", GetType().Name, message), ex);
        }
    }
}