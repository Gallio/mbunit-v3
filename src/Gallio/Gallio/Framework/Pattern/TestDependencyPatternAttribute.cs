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
using Gallio.Model;
using Gallio.Reflection;
using Gallio.Framework.Pattern;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// <para>
    /// A dependency pattern attribute creates a dependency on the tests defined
    /// by some other code element.
    /// </para>
    /// </summary>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = true, Inherited = true)]
    public abstract class TestDependencyPatternAttribute : PatternAttribute
    {
        /// <inheritdoc />
        public override void Process(PatternEvaluationScope scope, ICodeElementInfo codeElement)
        {
            Validate(scope, codeElement);

            ICodeElementInfo resolvedDependency = GetDependency(scope, codeElement);
            scope.Evaluator.AddFinishModelAction(codeElement, delegate
            {
                bool success = false;
                foreach (PatternEvaluationScope dependentScope in scope.Evaluator.GetScopes(resolvedDependency))
                {
                    if (dependentScope.IsTestDeclaration)
                    {
                        scope.Test.AddDependency(dependentScope.Test);
                        success = true;
                    }
                }

                if (! success)
                    scope.Evaluator.TestModel.AddAnnotation(new Annotation(AnnotationType.Warning, codeElement,
                        "Was unable to resolve a test dependency.", null));
            });
        }

        /// <summary>
        /// Verifies that the attribute is being used correctly.
        /// </summary>
        /// <param name="scope">The scope</param>
        /// <param name="codeElement">The code element</param>
        /// <exception cref="PatternUsageErrorException">Thrown if the attribute is being used incorrectly</exception>
        protected virtual void Validate(PatternEvaluationScope scope, ICodeElementInfo codeElement)
        {
            if (!scope.IsTestDeclaration)
                ThrowUsageErrorException("This attribute can only be used on a test.");
        }

        /// <summary>
        /// Gets the code element that declares the tests on which this test should depend.
        /// </summary>
        /// <param name="scope">The scope</param>
        /// <param name="codeElement">The code element</param>
        /// <returns>The code element representing the dependency</returns>
        protected abstract ICodeElementInfo GetDependency(PatternEvaluationScope scope, ICodeElementInfo codeElement);
    }
}
