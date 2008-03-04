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
using Gallio.Framework.Pattern;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// <para>
    /// The <see cref="PatternAttribute" /> class is the base class for all pattern test framework
    /// attributes.  It associates a code element with a <see cref="PatternTest" /> for building
    /// <see cref="PatternTestParameter" /> and <see cref="PatternAttribute" /> objects using reflection.
    /// </para>
    /// <para>
    /// Subclasses of <see cref="IPattern" /> define simpler interfaces for implementing
    /// the semantics of common types of attributes such as test factories, decorators,
    /// and data providers.  Refer to the documentation of each subclass for details on its use.
    /// </para>
    /// </summary>
    /// <seealso cref="IPattern"/>
    /// <seealso cref="PatternTestFramework"/>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public abstract class PatternAttribute : Attribute, IPattern
    {
        /// <inheritdoc />
        public virtual bool Consume(IPatternTestBuilder containingTestBuilder, ICodeElementInfo codeElement)
        {
            return false;
        }

        /// <inheritdoc />
        public virtual void ProcessTest(IPatternTestBuilder testBuilder, ICodeElementInfo codeElement)
        {
        }

        /// <inheritdoc />
        public virtual void ProcessTestParameter(IPatternTestParameterBuilder testParameterBuilder, ICodeElementInfo codeElement)
        {
        }
    }
}