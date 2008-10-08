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
using System.Collections.Generic;
using System.Text;
using Gallio.Framework.Pattern;
using Gallio.Model.Diagnostics;
using Gallio.Reflection;
using Gallio.Model;
using Gallio.Framework.Data;
using MbUnit.Framework.ContractVerifiers.Patterns;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// Abstract attribute to qualify test fixtures that verify implementation contracts.
    /// </summary>
    [AttributeUsage(PatternAttributeTargets.TestType, AllowMultiple = true, Inherited = true)]
    public abstract class VerifyContractAttribute : TestDecoratorPatternAttribute
    {
        private readonly string testPatternName;

        /// <inheritdoc />
        protected VerifyContractAttribute(string testPatternName, Type targetType)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }

            this.testPatternName = testPatternName;
            this.TargetType = targetType;
        }

        /// <summary>
        /// Gets the type of the object to verify.
        /// </summary>
        protected Type TargetType
        {
            get;
            private set;
        }

        /// <inheritdoc />
        protected override void DecorateTest(PatternEvaluationScope scope, ICodeElementInfo codeElement)
        {
            base.DecorateTest(scope, codeElement);
            var contractTest = new PatternTest(testPatternName, null, scope.TestDataContext.CreateChild());
            contractTest.IsTestCase = false;
            contractTest.Metadata.SetValue(MetadataKeys.TestKind, TestKinds.Group);
            AddPatternTests(scope.AddChildTest(contractTest));
        }

        /// <inheritdoc />
        protected override void Validate(PatternEvaluationScope scope, ICodeElementInfo codeElement)
        {
            base.Validate(scope, codeElement);

            if (!scope.IsTestDeclaration)
                ThrowUsageErrorException("This attribute can only be used on a test fixture.");
        }

        private void AddPatternTests(PatternEvaluationScope scope)
        {
            foreach (PatternTestBuilder item in GetPatternTestBuilders())
            {
                item.Build(scope);
            }
        }

        /// <summary>
        /// Provides builders of pattern tests for the contract verifier.
        /// </summary>
        /// <returns>An enumeration of pattern test builders.</returns>
        protected abstract IEnumerable<PatternTestBuilder> GetPatternTestBuilders();

        /// <summary>
        /// Gets the interface of a particular type if it is implemented by another type,
        /// otherwise returns null.
        /// </summary>
        /// <param name="implementationType">The implementation type</param>
        /// <param name="interfaceType">The interface type</param>
        /// <returns>The interface type or null if it is not implemented by the implementation type</returns>
        protected static Type GetInterface(Type implementationType, Type interfaceType)
        {
            return interfaceType.IsAssignableFrom(implementationType) ? interfaceType : null;
        }
    }
}
