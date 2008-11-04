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
using System.Reflection;
using Gallio;

namespace MbUnit.Framework.ContractVerifiers.Patterns.Equality
{
    /// <summary>
    /// General purpose test pattern for contract verifiers.
    /// It verifies that a given method evaluating equality between
    /// two instances, returns the expected result according to
    /// the equivalence class the objects belongs to.
    /// </summary>
    internal class EqualityPattern : ContractVerifierPattern
    {
        private EqualityPatternSettings settings;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">Settings.</param>
        internal EqualityPattern(EqualityPatternSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            this.settings = settings;
        }

        /// <inheritdoc />
        protected override string Name
        {
            get
            {
                return settings.Name;
            }
        }

        /// <inheritdoc />
        protected internal override void Run(IContractVerifierPatternInstanceState state)
        {
            AssertionHelper.Verify(() =>
            {
                if (settings.EqualityMethodInfo != null)
                    return null;

                return new AssertionFailureBuilder("Equality method expected to be implemented.")
                    .AddLabeledValue("Expected Method", settings.SignatureDescription)
                    .ToAssertionFailure();
            });


            VerifyEqualityContract(state.FixtureType, state.FixtureInstance,
                (a, b) =>
                {
                    bool output = settings.EqualityMethodInfo.IsStatic ?
                        (bool)settings.EqualityMethodInfo.Invoke(null, new object[] { a, b }) :
                        (bool)settings.EqualityMethodInfo.Invoke(a, new object[] { b });
                    return settings.Inequality ? !output : output;
                });
        }

        private void VerifyEqualityContract(Type fixtureType, object fixtureInstance, Func<object, object, bool> equals)
        {
            // Get the equivalence classes before entering the multiple assertion block in
            // order to catch any missing implementation of IEquivalentClassProvider<T> before.
            IEnumerable equivalenceClasses = GetEquivalentClasses(settings.TargetType, fixtureType, fixtureInstance);

            Assert.Multiple(() =>
            {
                VerifyEqualityBetweenTwoNullReferences(equals);
                int i = 0;

                foreach (object a in equivalenceClasses)
                {
                    int j = 0;

                    foreach (object b in GetEquivalentClasses(settings.TargetType, fixtureType, fixtureInstance))
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
            if (!settings.TargetType.IsValueType && settings.EqualityMethodInfo.IsStatic)
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
            if (!settings.TargetType.IsValueType)
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

                if (settings.EqualityMethodInfo.IsStatic)
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
