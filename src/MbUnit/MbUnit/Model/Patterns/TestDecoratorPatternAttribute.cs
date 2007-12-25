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
using Gallio.Reflection;
using MbUnit.Model.Builder;

namespace MbUnit.Model.Patterns
{
    /// <summary>
    /// <para>
    /// A test decorator pattern attribute applies decorations to an
    /// existing type or method level <see cref="MbUnitTest" />.
    /// </para>
    /// </summary>
    /// <seealso cref="TestTypePatternAttribute"/>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method,
        AllowMultiple = true, Inherited = true)]
    public abstract class TestDecoratorPatternAttribute : DecoratorPatternAttribute
    {
        /// <inheritdoc />
        public override void ProcessTest(ITestBuilder testBuilder, ICodeElementInfo codeElement)
        {
            testBuilder.AddDecorator(Order, delegate(ITestBuilder typeTestBuilder)
            {
                DecorateTest(typeTestBuilder, codeElement);
            });
        }

        /// <summary>
        /// <para>
        /// Applies decorations to a method or type-level <see cref="MbUnitTest" />.
        /// </para>
        /// <para>
        /// A typical use of this method is to augment the test with additional metadata
        /// or to add additional behaviors to the test.
        /// </para>
        /// </summary>
        /// <param name="builder">The test builder</param>
        /// <param name="codeElement">The code element</param>
        protected virtual void DecorateTest(ITestBuilder builder, ICodeElementInfo codeElement)
        {
        }
    }
}