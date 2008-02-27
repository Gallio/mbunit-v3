// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
    /// A contribution pattern attribute applies decorations to a containing test
    /// such as by introducing a new setup or teardown action.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public abstract class ContributionPatternAttribute : DecoratorPatternAttribute
    {
        /// <inheritdoc />
        public override bool Consume(IPatternTestBuilder containingTestBuilder, ICodeElementInfo codeElement)
        {
            containingTestBuilder.AddDecorator(Order, delegate(IPatternTestBuilder methodTestBuilder)
            {
                DecorateContainingTest(methodTestBuilder, codeElement);
            });

            return true;
        }

        /// <summary>
        /// <para>
        /// Applies decorations to the containing <see cref="PatternTest" />.
        /// </para>
        /// </summary>
        /// <param name="containingTestBuilder">The containing test builder</param>
        /// <param name="codeElement">The code element</param>
        protected virtual void DecorateContainingTest(IPatternTestBuilder containingTestBuilder, ICodeElementInfo codeElement)
        {
        }
    }
}