// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Model.Reflection;

namespace MbUnit.Model.Builder
{
    /// <summary>
    /// <para>
    /// The <see cref="PatternAttribute" /> class is the base class for all MbUnit framework
    /// attributes.  It associates a code element with a <see cref="IPattern" /> for building
    /// <see cref="MbUnitTest" /> and <see cref="MbUnitTestParameter" /> objects using reflection.
    /// </para>
    /// <para>
    /// Subclasses of <see cref="PatternAttribute" /> define simpler interfaces for implementing
    /// the semantics of common types of MbUnit attributes such as test factories, decorators,
    /// and data providers.  Refer to the documentation of each subclass for details on its use.
    /// </para>
    /// </summary>
    /// <seealso cref="IPattern"/>
    [AttributeUsage(AttributeTargets.All, AllowMultiple=true, Inherited=true)]
    public abstract class PatternAttribute : Attribute, IPattern
    {
        /// <inheritdoc />
        public virtual bool Consume(ITestBuilder containingTestBuilder, ICodeElementInfo codeElement)
        {
            return false;
        }

        /// <inheritdoc />
        public virtual void ProcessTest(ITestBuilder testBuilder, ICodeElementInfo codeElement)
        {
        }

        /// <inheritdoc />
        public virtual void ProcessTestParameter(ITestParameterBuilder testParameterBuilder, ICodeElementInfo codeElement)
        {
        }
    }
}
