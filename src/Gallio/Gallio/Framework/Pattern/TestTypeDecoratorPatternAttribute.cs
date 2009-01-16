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
    /// A test type decorator pattern attribute applies decorations to an
    /// existing test declared at the type-level.
    /// </para>
    /// </summary>
    /// <seealso cref="TestTypePatternAttribute"/>
    [AttributeUsage(PatternAttributeTargets.TestType, AllowMultiple = true, Inherited = true)]
    public abstract class TestTypeDecoratorPatternAttribute : DecoratorPatternAttribute
    {
        /// <inheritdoc />
        public override void Process(IPatternScope scope, ICodeElementInfo codeElement)
        {
            ITypeInfo type = codeElement as ITypeInfo;
            Validate(scope, type);

            scope.TestBuilder.AddDeferredAction(codeElement, Order, delegate
            {
                DecorateTest(scope, type);
            });
        }

        /// <summary>
        /// Verifies that the attribute is being used correctly.
        /// </summary>
        /// <param name="scope">The scope</param>
        /// <param name="type">The type</param>
        /// <exception cref="PatternUsageErrorException">Thrown if the attribute is being used incorrectly</exception>
        protected virtual void Validate(IPatternScope scope, ITypeInfo type)
        {
            if (!scope.IsTestDeclaration || type == null)
                ThrowUsageErrorException("This attribute can only be used on a test type.");
        }

        /// <summary>
        /// <para>
        /// Applies decorations to a type-level test.
        /// </para>
        /// <para>
        /// A typical use of this method is to augment the test with additional metadata
        /// or to add additional behaviors to the test.
        /// </para>
        /// </summary>
        /// <param name="typeScope">The type scope</param>
        /// <param name="type">The type</param>
        protected virtual void DecorateTest(IPatternScope typeScope, ITypeInfo type)
        {
        }
    }
}