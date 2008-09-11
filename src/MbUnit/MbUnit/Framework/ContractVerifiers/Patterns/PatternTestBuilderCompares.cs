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
    /// The generated test verifies that all a comparison
    /// operator/method works as expected.
    /// </summary>
    /// <typeparam name="T">The type of the result returned by the comparison method/operator.</typeparam>
    public class PatternTestBuilderCompares<T> : PatternTestBuilder
    {
        private string friendlyName;
        private MethodInfo comparisonMethod;
        private string signatureDescription;
        private Func<int, int, T> refers;
        private Func<T, string> formatResult;
        private Func<T, T> postProcess;

        /// <summary>
        /// Constructs a pattern test builder.
        /// The resulting test verifies that the target type has
        /// the specified attribute.
        /// </summary>
        /// <param name="targetType">The target type.</param>
        /// <param name="friendlyName">A friendly name for the equality operation.</param>
        /// <param name="signatureDescription">A friendly signature description.</param>
        /// <param name="comparisonMethod">The equality operation</param>
        /// <param name="refers">An equivalent comparison method which gives the same results with Int32 parameters.</param>
        /// <param name="formatResult">A function which formats the result into a friendly text.</param>
        /// <param name="postProcess">Post-processor function for the result of the comparison method, in 
        /// order to make it comparable with the result of the reference.</param>
        public PatternTestBuilderCompares(Type targetType, string friendlyName,
            string signatureDescription, MethodInfo comparisonMethod, Func<int, int, T> refers,
            Func<T, string> formatResult, Func<T, T> postProcess)
            : base(targetType)
        {
            this.friendlyName = friendlyName;
            this.comparisonMethod = comparisonMethod;
            this.signatureDescription = signatureDescription;
            this.refers = refers;
            this.formatResult = formatResult;
            this.postProcess = postProcess;
        }

        /// <summary>
        /// Constructs a pattern test builder.
        /// The resulting test verifies that the target type has
        /// the specified attribute.
        /// </summary>
        /// <param name="targetType">The target type.</param>
        /// <param name="friendlyName">A friendly name for the equality operation.</param>
        /// <param name="signatureDescription">A friendly signature description.</param>
        /// <param name="comparisonMethod">The equality operation</param>
        /// <param name="refers">An equivalent comparison method which gives the same results with Int32 parameters.</param>
        public PatternTestBuilderCompares(Type targetType, string friendlyName,
            string signatureDescription, MethodInfo comparisonMethod, Func<int, int, T> refers)
            : this(targetType, friendlyName, signatureDescription, comparisonMethod, refers, x => x.ToString(), x => x)
        {
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
                if (comparisonMethod != null)
                    return null;

                return new AssertionFailureBuilder("Comparison method/operator expected to be implemented.")
                    .AddLabeledValue("Expected Method", signatureDescription)
                    .ToAssertionFailure();
            });

            VerifyComparisonContract(state.FixtureType, state.FixtureInstance,
                (a, b) => postProcess(comparisonMethod.IsStatic ?
                        (T)comparisonMethod.Invoke(null, new object[] { a, b }) :
                        (T)comparisonMethod.Invoke(a, new object[] { b })));
        }

        private void VerifyComparisonContract(Type fixtureType, object fixtureInstance, Func<object, object, T> compares)
        {
            // Get the equivalence classes before entering the multiple assertion block in
            // order to catch any missing implementation of IEquivalentClassProvider<T> before.
            IEnumerable equivalenceClasses = GetEquivalentClasses(fixtureType, fixtureInstance);

            Assert.Multiple(() =>
            {
                VerifyEqualityBetweenTwoNullReferences(compares);
                int i = 0;

                foreach (object a in equivalenceClasses)
                {
                    int j = 0;

                    foreach (object b in GetEquivalentClasses(fixtureType, fixtureInstance))
                    {
                        CompareEquivalentInstances((IEnumerable)a, (IEnumerable)b, postProcess(refers(i, j)), compares);
                        j++;
                    }

                    i++;
                }
            });
        }

        private void CompareEquivalentInstances(IEnumerable a, IEnumerable b, T expectedResult, Func<object, object, T> compares)
        {
            foreach (object x in a)
            {
                VerifyNullReferenceComparison(x, compares);

                if (comparisonMethod.IsStatic)
                {
                    foreach (object y in b)
                    {
                        AssertionHelper.Verify(() =>
                        {
                            T actualResult = compares(x, y);
                            if (expectedResult.Equals(actualResult))
                                return null;

                            return new AssertionFailureBuilder("The comparison result between left and right values does not meet expectations.")
                                .AddRawLabeledValue("Left Value", x)
                                .AddRawLabeledValue("Right Value", y)
                                .AddLabeledValue("Expected Result", formatResult(expectedResult))
                                .AddLabeledValue("Actual Result", formatResult(actualResult))
                                .ToAssertionFailure();
                        });
                    }
                }
            }
        }

        private void VerifyEqualityBetweenTwoNullReferences(Func<object, object, T> compares)
        {
            if (!TargetType.IsValueType && comparisonMethod.IsStatic)
            {
                AssertionHelper.Verify(() =>
                {
                    T actualResult;
                    T expectedResult = postProcess(refers(0, 0));

                    try
                    {
                        actualResult = compares(null, null);
                        if (expectedResult.Equals(actualResult))
                            return null;
                    }
                    catch (TargetInvocationException exception)
                    {
                        return new AssertionFailureBuilder("The comparison result between left and right values does not meet expectations.")
                            .AddRawLabeledValue("Left Value", null)
                            .AddRawLabeledValue("Right Value", null)
                            .AddLabeledValue("Expected Result", formatResult(expectedResult))
                            .AddRawLabeledValue("Actual Result", exception.InnerException)
                            .ToAssertionFailure();
                    }
                    catch (NullReferenceException exception)
                    {
                        return new AssertionFailureBuilder("The comparison result between left and right values does not meet expectations.")
                            .AddRawLabeledValue("Left Value", null)
                            .AddRawLabeledValue("Right Value", null)
                            .AddLabeledValue("Expected Result", formatResult(expectedResult))
                            .AddRawLabeledValue("Actual Result", exception)
                            .ToAssertionFailure();
                    }

                    return new AssertionFailureBuilder("The comparison result between left and right values does not meet expectations.")
                        .AddRawLabeledValue("Left Value", null)
                        .AddRawLabeledValue("Right Value", null)
                        .AddLabeledValue("Expected Result", formatResult(expectedResult))
                        .AddLabeledValue("Actual Result", formatResult(actualResult))
                        .ToAssertionFailure();
                });
            }
        }

        private void VerifyNullReferenceComparison(object x, Func<object, object, T> compares)
        {
            if (!TargetType.IsValueType)
            {
                AssertionHelper.Verify(() =>
                {
                    T actualResult = compares(x, null);
                    T expectedResult = postProcess(refers(0, Int32.MinValue));
                    if (expectedResult.Equals(actualResult))
                        return null;

                    return new AssertionFailureBuilder("The comparison result between left and right values does not meet expectations.")
                        .AddRawLabeledValue("Left Value", x)
                        .AddRawLabeledValue("Right Value", null)
                        .AddLabeledValue("Expected Result", formatResult(expectedResult))
                        .AddLabeledValue("Actual Result", formatResult(actualResult))
                        .ToAssertionFailure();
                });

                if (comparisonMethod.IsStatic)
                {
                    AssertionHelper.Verify(() =>
                    {
                        T actualResult;
                        T expectedResult = postProcess(refers(Int32.MinValue, 0));

                        try
                        {
                            actualResult = compares(null, x);
                            if (expectedResult.Equals(actualResult))
                                return null;
                        }
                        catch (TargetInvocationException exception)
                        {
                            return new AssertionFailureBuilder("The comparison result between left and right values does not meet expectations.")
                                .AddRawLabeledValue("Left Value", null)
                                .AddRawLabeledValue("Right Value", x)
                                .AddLabeledValue("Expected Result", formatResult(expectedResult))
                                .AddRawLabeledValue("Actual Result", exception.InnerException)
                                .ToAssertionFailure();
                        }
                        catch (NullReferenceException exception)
                        {
                            return new AssertionFailureBuilder("The comparison result between left and right values does not meet expectations.")
                                .AddRawLabeledValue("Left Value", null)
                                .AddRawLabeledValue("Right Value", x)
                                .AddLabeledValue("Expected Result", formatResult(expectedResult))
                                .AddRawLabeledValue("Actual Result", exception)
                                .ToAssertionFailure();
                        }

                        return new AssertionFailureBuilder("The comparison result between left and right values does not meet expectations.")
                            .AddRawLabeledValue("Left Value", null)
                            .AddRawLabeledValue("Right Value", x)
                            .AddLabeledValue("Expected Result", formatResult(expectedResult))
                            .AddLabeledValue("Actual Result", formatResult(actualResult))
                            .ToAssertionFailure();
                    });
                }
            }
        }

    }
}
