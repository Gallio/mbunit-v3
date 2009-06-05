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

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// A contribution method pattern attribute applies decorations to a containing scope
    /// such as by introducing a new setup or teardown action to a test.
    /// </summary>
    [AttributeUsage(PatternAttributeTargets.ContributionMethod, AllowMultiple=false, Inherited=true)]
    public abstract class ContributionMethodPatternAttribute : DecoratorPatternAttribute
    {
        /// <inheritdoc />
        public override bool IsPrimary
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override bool IsTestPart(IPatternEvaluator evaluator, ICodeElementInfo codeElement)
        {
            return true;
        }

        /// <inheritdoc />
        public override void Consume(IPatternScope containingScope, ICodeElementInfo codeElement, bool skipChildren)
        {
            IMethodInfo method = codeElement as IMethodInfo;
            Validate(containingScope, method);

            containingScope.TestComponentBuilder.AddDeferredAction(codeElement, Order, delegate
            {
                DecorateContainingScope(containingScope, method);
            });
        }

        /// <summary>
        /// Verifies that the attribute is being used correctly.
        /// </summary>
        /// <param name="containingScope">The containing scope.</param>
        /// <param name="method">The method.</param>
        /// <exception cref="PatternUsageErrorException">Thrown if the attribute is being used incorrectly.</exception>
        protected virtual void Validate(IPatternScope containingScope, IMethodInfo method)
        {
            if (! containingScope.IsTestDeclaration || method == null)
                ThrowUsageErrorException(String.Format("This attribute can only be used on a method within a test type."));
        }

        /// <summary>
        /// <para>
        /// Applies decorations to the containing test.
        /// </para>
        /// </summary>
        /// <param name="containingScope">The containing scope.</param>
        /// <param name="method">The method to process.</param>
        protected virtual void DecorateContainingScope(IPatternScope containingScope, IMethodInfo method)
        {
        }
    }
}
