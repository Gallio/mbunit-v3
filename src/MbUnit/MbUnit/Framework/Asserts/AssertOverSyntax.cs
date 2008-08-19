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
using System.Collections;
using System.Collections.Generic;
using Gallio;
using Gallio.Model.Diagnostics;

namespace MbUnit.Framework
{
    /// <summary>
    /// Defines methods used with the <see cref="NewAssert.Over" /> syntax for mapping
    /// assertions over complex data structures.
    /// </summary>
    [TestFrameworkInternal]
    public sealed class AssertOverSyntax
    {
        internal static readonly AssertOverSyntax Instance = new AssertOverSyntax();

        private AssertOverSyntax()
        {
        }

        /// <summary>
        /// Evaluates an assertion over sequences of expected and actual values.
        /// </summary>
        /// <typeparam name="TExpected">The expected value type</typeparam>
        /// <typeparam name="TActual">The actual value type</typeparam>
        /// <param name="expectedValues">The expected values, or null</param>
        /// <param name="actualValues">The actual values, or null</param>
        /// <param name="assertion">The assertion to evaluate given an expected value
        /// and an actual value</param>
        public void Sequence<TExpected, TActual>(IEnumerable<TExpected> expectedValues,
            IEnumerable<TActual> actualValues, Action<TExpected, TActual> assertion)
        {
            Sequence(expectedValues, actualValues, assertion, null, null);
        }

        /// <summary>
        /// Evaluates an assertion over sequences of expected and actual values.
        /// </summary>
        /// <typeparam name="TExpected">The expected value type</typeparam>
        /// <typeparam name="TActual">The actual value type</typeparam>
        /// <param name="expectedValues">The expected values, or null</param>
        /// <param name="actualValues">The actual values, or null</param>
        /// <param name="assertion">The assertion to evaluate given an expected value
        /// and an actual value</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        public void Sequence<TExpected, TActual>(IEnumerable<TExpected> expectedValues,
            IEnumerable<TActual> actualValues, Action<TExpected, TActual> assertion,
            string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (expectedValues == null)
                {
                    if (actualValues == null)
                        return null;

                    return new AssertionFailureBuilder("The expected value sequence is null but the actual value sequence is not.")
                        .SetMessage(messageFormat, messageArgs)
                        .SetRawActualValue(actualValues)
                        .ToAssertionFailure();
                }

                int index = 0;
                IEnumerator<TExpected> expectedEnumerator = expectedValues.GetEnumerator();
                IEnumerator<TActual> actualEnumerator = actualValues.GetEnumerator();
                while (expectedEnumerator.MoveNext())
                {
                    if (!actualEnumerator.MoveNext())
                    {
                        return new AssertionFailureBuilder(String.Format("The expected value sequence has {0} elements but the actual value sequence has {1}.",
                            1 + index + CountRemainingElements(expectedEnumerator), index))
                            .SetMessage(messageFormat, messageArgs)
                            .SetRawExpectedAndActualValueWithDiffs(expectedValues, actualValues)
                            .ToAssertionFailure();
                    }

                    AssertionFailure[] failures = AssertionHelper.Eval(delegate
                    {
                        assertion(expectedEnumerator.Current, actualEnumerator.Current);
                    });

                    if (failures.Length != 0)
                    {
                        return new AssertionFailureBuilder(String.Format("Assertion failed at index {0}.", index))
                            .SetMessage(messageFormat, messageArgs)
                            .SetRawExpectedAndActualValueWithDiffs(expectedValues, actualValues)
                            .ToAssertionFailure();
                    }

                    index += 1;
                }

                if (actualEnumerator.MoveNext())
                {
                    return new AssertionFailureBuilder(String.Format("The expected value sequence has {0} elements but the actual value sequence has {1}.",
                        index, index + CountRemainingElements(actualEnumerator) + 1))
                        .SetMessage(messageFormat, messageArgs)
                        .SetRawExpectedAndActualValueWithDiffs(expectedValues, actualValues)
                        .ToAssertionFailure();
                }

                return null;
            });
        }

        private static int CountRemainingElements(IEnumerator enumerator)
        {
            int count = 0;
            while (enumerator.MoveNext())
                count++;
            return count;
        }
    }
}
