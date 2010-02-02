// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Reflection;
using Gallio.Common;
using Gallio.Common.Diagnostics;
using Gallio.Common.Reflection;
using Gallio.Framework;
using Gallio.Framework.Pattern;
using System.Collections.Generic;
using Gallio.Model;

namespace MbUnit.Framework
{
    /// <summary>
    /// Base class for all the patterns defining an extension point of the framework.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Extension point attributes act through a static method.
    /// </para>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.ContributionMethod, AllowMultiple = false, Inherited = true)]
    public abstract class ExtensionPointPatternAttribute : DecoratorPatternAttribute
    {
        /// <inheritdoc />
        public override bool IsPrimary
        {
            get
            {
                return true;
            }
        }

        /// <inheritdoc />
        public override IList<TestPart> GetTestParts(IPatternEvaluator evaluator, ICodeElementInfo codeElement)
        {
            return new[] { new TestPart() { IsTestContribution = true } };
        }

        /// <inheritdoc />
        public override void Consume(IPatternScope containingScope, ICodeElementInfo codeElement, bool skipChildren)
        {
            var method = codeElement as IMethodInfo;
            Validate(containingScope, method);
            containingScope.TestComponentBuilder.AddDeferredAction(codeElement, Order, () => DecorateContainingScope(containingScope, method));
        }

        /// <summary>
        /// Verifies that the attribute is being used correctly.
        /// </summary>
        /// <param name="containingScope">The containing scope.</param>
        /// <param name="method">The method.</param>
        /// <exception cref="PatternUsageErrorException">Thrown if the attribute is being used incorrectly.</exception>
        protected virtual void Validate(IPatternScope containingScope, IMethodInfo method)
        {
            if (!containingScope.IsTestDeclaration || method == null)
                ThrowUsageErrorException(String.Format("This attribute can only be used on a method within a test type."));

            if (!method.IsStatic)
                ThrowUsageErrorException(String.Format("Expected the custom extensibility method '{0}' to be static.", method.Name));
        }

        /// <summary>
        /// Applies decorations to the containing test.
        /// </summary>
        /// <param name="containingScope">The containing scope.</param>
        /// <param name="method">The method to process.</param>
        protected virtual void DecorateContainingScope(IPatternScope containingScope, IMethodInfo method)
        {
        }
    }
}
