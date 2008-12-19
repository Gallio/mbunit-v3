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
using Gallio.Model;
using Gallio.Framework.Data;
using Gallio.Reflection;
using System.Collections;
using Gallio.Framework.Assertions;
using System.Reflection;

namespace MbUnit.Framework.ContractVerifiers.Patterns
{
    /// <summary>
    /// Abstract test pattern for contract verifiers.
    /// </summary>
    public abstract class ContractVerifierPattern
    {
        /// <summary>
        /// Gets the name of the test pattern.
        /// </summary>
        protected abstract string Name
        {
            get;
        }

        /// <summary>
        /// Runs the test pattern action.
        /// </summary>
        /// <param name="state"></param>
        protected internal abstract void Run(IContractVerifierPatternInstanceState state);

        /// <summary>
        /// Builds the test pattern, then adds it 
        /// to the evaluation scope as a new child test.
        /// </summary>
        /// <param name="scope">The scope of the test pattern.</param>
        /// <param name="fieldVerifierName">The name of the field defined as a contract verifier.</param>
        public void Build(PatternEvaluationScope scope, string fieldVerifierName)
        {
            var test = new PatternTest(Name, null, scope.TestDataContext.CreateChild());
            test.Metadata.SetValue(MetadataKeys.TestKind, TestKinds.Test);
            test.IsTestCase = true;
            scope.AddChildTest(test);

            test.TestInstanceActions.BeforeTestInstanceChain.After(
                state =>
                {
                    ObjectCreationSpec spec = state.GetFixtureObjectCreationSpec(scope.Parent.CodeElement as ITypeInfo);
                    state.FixtureType = spec.ResolvedType;
                    state.FixtureInstance = spec.CreateInstance();
                });

            test.TestInstanceActions.ExecuteTestInstanceChain.After(
                state =>
                {
                    Run(new ContractVerifierPatternInstanceState(
                        state.FixtureType, state.FixtureInstance, fieldVerifierName));
                });
        }

        /// <summary>
        /// Helper methods that builds a friendly displayable constructor signature.
        /// </summary>
        /// <param name="types">The parameter types of the constructor.</param>
        /// <returns></returns>
        protected static string GetConstructorSignature(IEnumerable<Type> types)
        {
            StringBuilder output = new StringBuilder(".ctor(");
            bool first = true;

            foreach (var type in types)
            {
                if (!first)
                {
                    output.Append(", ");
                }

                output.Append(type.Name);
                first = false;
            }

            return output.Append(")").ToString();
        }

        /// <summary>
        /// Returns the instance of the contract verifier field.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected static AbstractContractVerifier GetFieldInstance(IContractVerifierPatternInstanceState state)
        {
            var fieldInfo = state.FixtureType.GetField(state.FieldVerifierName);
            return (AbstractContractVerifier)fieldInfo.GetValue(state.FixtureInstance);
        }

        /// <summary>
        /// Casts the instance of the test fixture into a provider of equivalence classes, 
        /// then returns the resulting collection as an enumeration.
        /// </summary>
        /// <param name="equivalentClassSource"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        protected static IEnumerable GetEquivalentClasses(PropertyInfo equivalentClassSource, IContractVerifierPatternInstanceState state)
        {
            var field = GetFieldInstance(state);
            var equivalentClasses = (IEnumerable)equivalentClassSource.GetValue(field, null);

            AssertionHelper.Verify(() =>
            {
                if (equivalentClasses != null)
                    return null;

                return new AssertionFailureBuilder("The contract verifier needs " +
                    "a valid collection of equivalence instance classes. Please " +
                    "first initialize the appropriate property.")
                    .AddLabeledValue("Property", equivalentClassSource.Name)
                    .ToAssertionFailure();
            });

            return equivalentClasses;
        }
    }
}
