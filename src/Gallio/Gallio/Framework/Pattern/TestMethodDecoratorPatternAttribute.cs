// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Reflection;
using Gallio.Framework.Pattern;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// <para>
    /// A test decorator pattern attribute applies decorations to an
    /// existing test declared at the method-level.
    /// </para>
    /// </summary>
    [AttributeUsage(PatternAttributeTargets.TestMethod, AllowMultiple = true, Inherited = true)]
    public abstract class TestMethodDecoratorPatternAttribute : DecoratorPatternAttribute
    {
        /// <inheritdoc />
        public override void Process(IPatternScope scope, ICodeElementInfo codeElement)
        {
            IMethodInfo method = codeElement as IMethodInfo;
            Validate(scope, method);

            scope.TestBuilder.AddDeferredAction(codeElement, Order, delegate
            {
                DecorateMethodTest(scope, method);
            });
        }

        /// <summary>
        /// Verifies that the attribute is being used correctly.
        /// </summary>
        /// <param name="scope">The scope</param>
        /// <param name="method">The method</param>
        /// <exception cref="PatternUsageErrorException">Thrown if the attribute is being used incorrectly</exception>
        protected virtual void Validate(IPatternScope scope, IMethodInfo method)
        {
            if (!scope.IsTestDeclaration || method == null)
                ThrowUsageErrorException("This attribute can only be used on a test method.");
        }

        /// <summary>
        /// <para>
        /// Applies decorations to a method-level test.
        /// </para>
        /// <para>
        /// A typical use of this method is to augment the test with additional metadata
        /// or to add additional behaviors to the test.
        /// </para>
        /// </summary>
        /// <param name="methodScope">The method scope</param>
        /// <param name="method">The method</param>
        protected virtual void DecorateMethodTest(IPatternScope methodScope, IMethodInfo method)
        {
        }
    }
}