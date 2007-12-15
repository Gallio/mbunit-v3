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
    /// A test parameter decorator pattern attribute applies decorations to an
    /// existing <see cref="MbUnitTestParameter" />.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter
        | AttributeTargets.GenericParameter, AllowMultiple = true, Inherited = true)]
    public abstract class ParameterDecoratorPatternAttribute : DecoratorPatternAttribute
    {
        /// <inheritdoc />
        public override void ProcessTestParameter(ITestParameterBuilder testParameterBuilder, ICodeElementInfo codeElement)
        {
            ISlotInfo slot = (ISlotInfo)codeElement;

            testParameterBuilder.AddDecorator(Order, delegate(ITestParameterBuilder decoratedTestParameterBuilder)
            {
                DecorateTestParameter(decoratedTestParameterBuilder, slot);
            });
        }

        /// <summary>
        /// <para>
        /// Applies decorations to a <see cref="MbUnitTestParameter" />.
        /// </para>
        /// <para>
        /// A typical use of this method is to augment the test parameter with additional metadata
        /// or to add additional behaviors to the test parameter.
        /// </para>
        /// </summary>
        /// <param name="builder">The test builder</param>
        /// <param name="slot">The slot</param>
        protected virtual void DecorateTestParameter(ITestParameterBuilder builder, ISlotInfo slot)
        {
        }
    }
}
