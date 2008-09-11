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
using Gallio.Framework.Data;
using Gallio.Reflection;
using System.Collections;
using Gallio.Framework.Assertions;
using System.Reflection;

namespace MbUnit.Framework.ContractVerifiers.Patterns
{
    /// <summary>
    /// Abstract builder of pattern test for a contract verifier.
    /// </summary>
    public abstract class PatternTestBuilder
    {
        /// <summary>
        /// Gets the name of the pattern test.
        /// </summary>
        protected abstract string Name
        {
            get;
        }

        /// <summary>
        /// Runs the pattern test action.
        /// </summary>
        protected abstract void Run(PatternTestInstanceState state);

        /// <summary>
        /// Constructs a pattern test builder.
        /// </summary>
        /// <param name="targetType">The tested type.</param>
        protected PatternTestBuilder(Type targetType)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }

            this.TargetType = targetType;
        }

        /// <summary>
        /// Gets the target type.
        /// </summary>
        protected Type TargetType
        {
            get;
            private set;
        }

        /// <summary>
        /// Builds the pattern test, then adds it 
        /// to the evaluation scope as a new child test.
        /// </summary>
        /// <param name="scope">The scope of the test pattern.</param>
        public void Build(PatternEvaluationScope scope)
        {
            var test = new PatternTest(Name, null, scope.TestDataContext.CreateChild());
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
                new Action<PatternTestInstanceState>(Run));
        }

        /// <summary>
        /// Helper method which returns a string describing the signature 
        /// of a constructor.
        /// Example: ".ctor(SerializationInfo, StreamingContext)"
        /// </summary>
        /// <param name="parameters">The parameter types of the constructor.</param>
        /// <returns></returns>
        protected static string GetConstructorSignature(Type[] parameters)
        {
            StringBuilder output = new StringBuilder(".ctor(");

            for (int i = 0; i < parameters.Length; i++)
            {
                if (i != 0)
                {
                    output.Append(", ");
                }

                output.Append(parameters[i].Name);
            }

            output.Append(")");
            return output.ToString();
        }

        /// <summary>
        /// Casts the instance of the test fixture into a provider of equivalence classes, 
        /// then returns the resulting collection as an enumeration.
        /// </summary>
        /// <param name="fixtureType">The test fixture type.</param>
        /// <param name="fixtureInstance">The test fixture instance.</param>
        /// <returns></returns>
        protected IEnumerable GetEquivalentClasses(Type fixtureType, object fixtureInstance)
        {
            Type interfaceType = GetIEquivalenceClassProviderInterface(fixtureType);

            AssertionHelper.Verify(() =>
            {
                if (interfaceType != null)
                    return null;

                return new AssertionFailureBuilder("Expected the contract verifier to implement a particular interface.")
                    .AddLabeledValue("Contract Verifier", "Equality")
                    .AddLabeledValue("Expected Interface", "IEquivalentClassProvider<" + TargetType + ">")
                    .ToAssertionFailure();
            });

            return (IEnumerable)interfaceType.InvokeMember("GetEquivalenceClasses",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                null, fixtureInstance, null);
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

        /// <summary>
        /// Returns the target type as a generic IEquatable interface, or
        /// a null reference if it does not implement such an interface.
        /// </summary>
        /// <returns>The interface type or a null reference.</returns>
        protected Type GetIEquivalenceClassProviderInterface(Type fixtureType)
        {
            return GetInterface(fixtureType, typeof(IEquivalenceClassProvider<>)
                .MakeGenericType(TargetType));
        }

    }
}
