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

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// Abstract attribute to qualify test fixtures that verify contracts.
    /// </summary>
    [AttributeUsage(PatternAttributeTargets.TestType, AllowMultiple = true, Inherited = true)]
    public abstract class VerifyContractAttribute : TestDecoratorPatternAttribute
    {
        private readonly string testPatternName;

        /// <inheritdoc />
        protected VerifyContractAttribute(string testPatternName)
        {
            this.testPatternName = testPatternName;
        }

        /// <inheritdoc />
        protected override void DecorateTest(PatternEvaluationScope scope, ICodeElementInfo codeElement)
        {
            base.DecorateTest(scope, codeElement);

            var contractTest = new PatternTest(testPatternName, null, scope.TestDataContext.CreateChild());
            contractTest.IsTestCase = false;

            var contractScope = scope.AddChildTest(contractTest);
            AddContractTests(contractScope);
        }

        /// <inheritdoc />
        protected override void Validate(PatternEvaluationScope scope, ICodeElementInfo codeElement)
        {
            base.Validate(scope, codeElement);

            if (!scope.IsTestDeclaration)
                ThrowUsageErrorException("This attribute can only be used on a test fixture.");
        }

        /// <summary>
        /// Injects in the pattern evaluation scope some test methods
        /// that will verify the good implementation of the contract according
        /// to the current options.
        /// </summary>
        /// <param name="scope">The scope of the verify contract pattern</param>
        protected abstract void AddContractTests(PatternEvaluationScope scope);

        /// <summary>
        /// Add the specified test action.
        /// </summary>
        /// <param name="scope">The scope of the verify contract pattern</param>
        /// <param name="name">The name of the test to add</param>
        /// <param name="description">The description of the test to add</param>
        /// <param name="action">The action to invoke when the test runs</param>
        protected PatternTest AddContractTest(PatternEvaluationScope scope, string name, 
            string description, Action<PatternTestInstanceState> action)
        {
            var test = new PatternTest(name, null, scope.TestDataContext.CreateChild());
            test.Metadata.SetValue(MetadataKeys.Description, description);
            test.IsTestCase = true;
            scope.AddChildTest(test);

            test.TestInstanceActions.BeforeTestInstanceChain.After(
                state =>
                {
                    ObjectCreationSpec spec = state.GetFixtureObjectCreationSpec(scope.Parent.CodeElement as ITypeInfo);
                    state.FixtureType = spec.ResolvedType;
                    state.FixtureInstance = spec.CreateInstance();
                });

            test.TestInstanceActions.ExecuteTestInstanceChain.After(action);
            return test;
        }

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
