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
using Gallio.Framework.Assertions;
using System.Collections;
using Gallio;
using System.Reflection;

namespace MbUnit.Framework.ContractVerifiers.Patterns
{
    /// <summary>
    /// Builder of pattern test for the contract verifiers.
    /// The generated test verifies that all an equality
    /// operator/method works as expected.
    /// </summary>
    public class PatternTestBuilderEquals : PatternTestBuilder
    {
        private string friendlyName;
        private MethodInfo equalityMethod;
        private string signatureDescription;
        private bool inequality;

        /// <summary>
        /// Constructs a pattern test builder.
        /// The resulting test verifies that the target type has
        /// the specified attribute.
        /// </summary>
        /// <param name="targetType">The target type.</param>
        /// <param name="friendlyName">A friendly name for the equality operation.</param>
        /// <param name="inequality">Indicates whether the method represents an inequality operation.</param>
        /// <param name="signatureDescription">A friendly signature description.</param>
        /// <param name="equalityMethod">The equality operation</param>
        public PatternTestBuilderEquals(Type targetType, string friendlyName, 
            bool inequality, string signatureDescription, MethodInfo equalityMethod)
            : base(targetType)
        {
            this.friendlyName = friendlyName;
            this.equalityMethod = equalityMethod;
            this.signatureDescription = signatureDescription;
            this.inequality = inequality;
        }

        /// <inheritdoc />
        protected override string Name
        {
            get
            {
                return friendlyName;
            }
        }

        /// <inheritdoc />
        protected override void Run(PatternTestInstanceState state)
        {
            AssertionHelper.Verify(() =>
            {
                if (equalityMethod != null)
                    return null;

                return new AssertionFailureBuilder("Equality method expected to be implemented.")
                    .AddLabeledValue("Expected Method", signatureDescription)
                    .ToAssertionFailure();
            });


            VerifyEqualityContract(state.FixtureType, state.FixtureInstance,
                (a, b) =>
                {
                    bool output = equalityMethod.IsStatic ?
                        (bool)equalityMethod.Invoke(null, new object[] { a, b }) :
                        (bool)equalityMethod.Invoke(a, new object[] { b });
                    return inequality ? !output : output;
                });
        }

        private void VerifyEqualityContract(Type fixtureType, object fixtureInstance, Func<object, object, bool> equals)
        {
            // Get the equivalence classes before entering the multiple assertion block in
            // order to catch any missing implementation of IEquivalentClassProvider<T> before.
            IEnumerable equivalenceClasses = GetEquivalentClasses(fixtureType, fixtureInstance);

            Assert.Multiple(() =>
            {
                VerifyEqualityBetweenTwoNullReferences(equals);
                int i = 0;

                foreach (object a in equivalenceClasses)
                {
                    int j = 0;

                    foreach (object b in GetEquivalentClasses(fixtureType, fixtureInstance))
                    {
                        CompareEquivalentInstances((IEnumerable)a, (IEnumerable)b, i == j, equals);
                        j++;
                    }

                    i++;
                }
            });
        }

        private void CompareEquivalentInstances(IEnumerable a, IEnumerable b,
            bool equalityExpected, Func<object, object, bool> equals)
        {
            foreach (object x in a)
            {
                AssertionHelper.Verify(() =>
                {
                    if (equals(x, x))
                        return null;

                    return new AssertionFailureBuilder("The equality operator should consider a value equal to itself.")
                        .AddRawLabeledValue("Value", x)
                        .ToAssertionFailure();
                });

                VerifyNullReferenceEquality(x, equals);

                foreach (object y in b)
                {
                    AssertionHelper.Verify(() =>
                    {
                        if (equals(x, y) == equalityExpected)
                            return null;

                        return new AssertionFailureBuilder("The equality operator should consider the left value " +
                            "and the right value " + (equalityExpected ? String.Empty : "not ") + "to be equal.")
                            .AddRawLabeledValuesWithDiffs("Left Value", x, "Right Value", y)
                            .ToAssertionFailure();
                    });

                    AssertionHelper.Verify(() =>
                    {
                        if (equals(y, x) == equalityExpected)
                            return null;

                        return new AssertionFailureBuilder("The equality operator should consider the left value " +
                            "and the right value " + (equalityExpected ? String.Empty : "not ") + "to be equal.")
                            .AddRawLabeledValuesWithDiffs("Left Value", x, "Right Value", y)
                            .ToAssertionFailure();
                    });
                }
            }
        }

        private void VerifyEqualityBetweenTwoNullReferences(Func<object, object, bool> equals)
        {
            if (!TargetType.IsValueType && equalityMethod.IsStatic)
            {
                AssertionHelper.Verify(() =>
                {
                    bool actualResult;

                    try
                    {
                        if (actualResult = equals(null, null))
                            return null;
                    }
                    catch (TargetInvocationException exception)
                    {
                        return new AssertionFailureBuilder("The equality result between the left and the right values does not meet expectations.")
                            .AddRawLabeledValue("Left Value", null)
                            .AddRawLabeledValue("Right Value", null)
                            .AddRawLabeledValue("Expected Result", true)
                            .AddRawLabeledValue("Actual Result", exception.InnerException)
                            .ToAssertionFailure();
                    }
                    catch (NullReferenceException exception)
                    {
                        return new AssertionFailureBuilder("The equality result between the left and the right values does not meet expectations.")
                            .AddRawLabeledValue("Left Value", null)
                            .AddRawLabeledValue("Right Value", null)
                            .AddRawLabeledValue("Expected Result", true)
                            .AddRawLabeledValue("Actual Result", exception)
                            .ToAssertionFailure();
                    }

                    return new AssertionFailureBuilder("The equality result between the left and the right values does not meet expectations.")
                        .AddRawLabeledValue("Left Value", null)
                        .AddRawLabeledValue("Right Value", null)
                        .AddRawLabeledValue("Expected Result", true)
                        .AddRawLabeledValue("Actual Result", actualResult)
                        .ToAssertionFailure();
                });
            }
        }

        private void VerifyNullReferenceEquality(object x, Func<object, object, bool> equals)
        {
            if (!TargetType.IsValueType)
            {
                AssertionHelper.Verify(() =>
                {
                    if (!equals(x, null))
                        return null;

                    return new AssertionFailureBuilder("The equality result between the left and the right values does not meet expectations.")
                        .AddRawLabeledValue("Left Value", x)
                        .AddRawLabeledValue("Right Value", null)
                        .AddRawLabeledValue("Expected Result", false)
                        .AddRawLabeledValue("Actual Result", true)
                        .ToAssertionFailure();
                });

                if (equalityMethod.IsStatic)
                {
                    AssertionHelper.Verify(() =>
                    {
                        try
                        {
                            if (!equals(null, x))
                                return null;
                        }
                        catch (TargetInvocationException exception)
                        {
                            return new AssertionFailureBuilder("The equality result between the left and the right values does not meet expectations.")
                                .AddRawLabeledValue("Left Value", null)
                                .AddRawLabeledValue("Right Value", x)
                                .AddRawLabeledValue("Expected Result", false)
                                .AddRawLabeledValue("Actual Result", exception.InnerException)
                                .ToAssertionFailure();
                        }
                        catch (NullReferenceException exception)
                        {
                            return new AssertionFailureBuilder("The equality result between the left and the right values does not meet expectations.")
                                .AddRawLabeledValue("Left Value", null)
                                .AddRawLabeledValue("Right Value", x)
                                .AddRawLabeledValue("Expected Result", false)
                                .AddRawLabeledValue("Actual Result", exception)
                                .ToAssertionFailure();
                        }

                        return new AssertionFailureBuilder("The equality result between the left and the right values does not meet expectations.")
                            .AddRawLabeledValue("Left Value", null)
                            .AddRawLabeledValue("Right Value", x)
                            .AddRawLabeledValue("Expected Result", false)
                            .AddRawLabeledValue("Actual Result", true)
                            .ToAssertionFailure();
                    });
                }
            }
        }
    }
}
