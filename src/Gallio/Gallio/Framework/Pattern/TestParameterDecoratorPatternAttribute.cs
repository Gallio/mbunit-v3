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
using Gallio.Common.Reflection;
using Gallio.Framework.Pattern;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// A test parameter decorator pattern attribute applies decorations to an
    /// existing <see cref="PatternTestParameter" />.
    /// </summary>
    [AttributeUsage(PatternAttributeTargets.TestParameter, AllowMultiple = true, Inherited = true)]
    public abstract class TestParameterDecoratorPatternAttribute : DecoratorPatternAttribute
    {
        /// <inheritdoc />
        public override void Process(IPatternScope scope, ICodeElementInfo codeElement)
        {
            ISlotInfo slot = codeElement as ISlotInfo;
            Validate(scope, slot);

            scope.TestParameterBuilder.AddDeferredAction(codeElement, Order, delegate
            {
                DecorateTestParameter(scope, slot);
            });
        }

        /// <summary>
        /// Verifies that the attribute is being used correctly.
        /// </summary>
        /// <param name="scope">The scope</param>
        /// <param name="slot">The slot</param>
        /// <exception cref="PatternUsageErrorException">Thrown if the attribute is being used incorrectly</exception>
        protected virtual void Validate(IPatternScope scope, ISlotInfo slot)
        {
            if (!scope.IsTestParameterDeclaration || slot == null)
                ThrowUsageErrorException("This attribute can only be used on a test parameter.");
        }

        /// <summary>
        /// <para>
        /// Applies decorations to a <see cref="PatternTestParameter" />.
        /// </para>
        /// <para>
        /// A typical use of this method is to augment the test parameter with additional metadata
        /// or to add additional behaviors to the test parameter.
        /// </para>
        /// </summary>
        /// <param name="slotScope">The slot scope</param>
        /// <param name="slot">The slot</param>
        protected virtual void DecorateTestParameter(IPatternScope slotScope, ISlotInfo slot)
        {
        }
    }
}