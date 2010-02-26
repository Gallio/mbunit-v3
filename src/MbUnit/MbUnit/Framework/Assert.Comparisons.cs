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
using System.Collections.Generic;
using System.Text;
using Gallio.Common;
using Gallio.Framework;
using Gallio.Framework.Assertions;

namespace MbUnit.Framework
{
    public abstract partial class Assert
    {
        #region AreEqual
        /// <summary>
        /// Verifies that an actual value equals some expected value.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// 
        public static void AreEqual<T>(T expectedValue, T actualValue)
        {
            AreEqual<T>(expectedValue, actualValue, (string)null, null);
        }

        /// <summary>
        /// Verifies that an actual value equals some expected value.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreEqual<T>(T expectedValue, T actualValue, string messageFormat, params object[] messageArgs)
        {
            AreEqual<T>(expectedValue, actualValue, (EqualityComparison<T>)null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that an actual value equals some expected value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="comparer">The comparer to use, or null to use the default one.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreEqual<T>(T expectedValue, T actualValue, IEqualityComparer<T> comparer)
        {
            AreEqual<T>(expectedValue, actualValue, comparer, null, null);
        }

        /// <summary>
        /// Verifies that an actual value equals some expected value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="comparer">The comparer to use, or null to use the default one.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreEqual<T>(T expectedValue, T actualValue, IEqualityComparer<T> comparer, string messageFormat, params object[] messageArgs)
        {
            AreEqual<T>(expectedValue, actualValue, comparer != null ? comparer.Equals : (EqualityComparison<T>)null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that an actual value equals some expected value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="comparer">The comparer to use, or null to use the default one.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreEqual<T>(T expectedValue, T actualValue, EqualityComparison<T> comparer)
        {
            AreEqual<T>(expectedValue, actualValue, comparer, null, null);
        }

        /// <summary>
        /// Verifies that an actual value equals some expected value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="comparer">The comparer to use, or null to use the default one.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreEqual<T>(T expectedValue, T actualValue, EqualityComparison<T> comparer, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (comparer == null)
                    comparer = ComparisonSemantics.Default.Equals;

                if (comparer(expectedValue, actualValue))
                    return null;

                return new AssertionFailureBuilder("Expected values to be equal.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawExpectedAndActualValuesWithDiffs(expectedValue, actualValue)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region AreNotEqual
        /// <summary>
        /// Verifies that an actual value does not equal some unexpected value.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="unexpectedValue">The unexpected value.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreNotEqual<T>(T unexpectedValue, T actualValue)
        {
            AreNotEqual<T>(unexpectedValue, actualValue, (string)null, null);
        }

        /// <summary>
        /// Verifies that an actual value does not equal some unexpected value.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="unexpectedValue">The unexpected value.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreNotEqual<T>(T unexpectedValue, T actualValue, string messageFormat, params object[] messageArgs)
        {
            AreNotEqual<T>(unexpectedValue, actualValue, (EqualityComparison<T>)null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that an actual value does not equal some unexpected value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="unexpectedValue">The unexpected value.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="comparer">The comparer to use, or null to use the default one.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreNotEqual<T>(T unexpectedValue, T actualValue, IEqualityComparer<T> comparer)
        {
            AreNotEqual<T>(unexpectedValue, actualValue, comparer, null, null);
        }

        /// <summary>
        /// Verifies that an actual value does not equal some unexpected value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="unexpectedValue">The unexpected value.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="comparer">The comparer to use, or null to use the default one.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreNotEqual<T>(T unexpectedValue, T actualValue, IEqualityComparer<T> comparer, string messageFormat, params object[] messageArgs)
        {
            AreNotEqual<T>(unexpectedValue, actualValue, comparer != null ? comparer.Equals : (EqualityComparison<T>)null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that an actual value does not equal some unexpected value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="unexpectedValue">The unexpected value.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="comparer">The comparer to use, or null to use the default one.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreNotEqual<T>(T unexpectedValue, T actualValue, EqualityComparison<T> comparer)
        {
            AreNotEqual<T>(unexpectedValue, actualValue, comparer, null, null);
        }

        /// <summary>
        /// Verifies that an actual value does not equal some unexpected value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="unexpectedValue">The unexpected value.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="comparer">The comparer to use, or null to use the default one.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreNotEqual<T>(T unexpectedValue, T actualValue, EqualityComparison<T> comparer, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (comparer == null)
                    comparer = ComparisonSemantics.Default.Equals;

                if (!comparer(unexpectedValue, actualValue))
                    return null;

                return new AssertionFailureBuilder("Expected values to be non-equal.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawLabeledValuesWithDiffs("Unexpected Value", unexpectedValue, "Actual Value", actualValue)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region AreSame
        /// <summary>
        /// Verifies that an actual value is referentially identical to some expected value.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreSame<T>(T expectedValue, T actualValue)
            where T : class
        {
            AreSame<T>(expectedValue, actualValue, (string)null, null);
        }

        /// <summary>
        /// Verifies that an actual value is referentially identical to some expected value.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreSame<T>(T expectedValue, T actualValue, string messageFormat, params object[] messageArgs)
            where T : class
        {
            AssertionHelper.Verify(delegate
            {
                if (ComparisonSemantics.Default.Same(expectedValue, actualValue))
                    return null;

                return new AssertionFailureBuilder("Expected values to be referentially identical.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawExpectedAndActualValuesWithDiffs(expectedValue, actualValue)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region AreNotSame
        /// <summary>
        /// Verifies that an actual value is not referentially identical to some unexpected value.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="unexpectedValue">The unexpected value.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreNotSame<T>(T unexpectedValue, T actualValue)
            where T : class
        {
            AreNotSame<T>(unexpectedValue, actualValue, (string)null, null);
        }

        /// <summary>
        /// Verifies that an actual value is not referentially identical to some unexpected value.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="unexpectedValue">The unexpected value.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreNotSame<T>(T unexpectedValue, T actualValue, string messageFormat, params object[] messageArgs)
            where T : class
        {
            AssertionHelper.Verify(delegate
            {
                if (!ComparisonSemantics.Default.Same(unexpectedValue, actualValue))
                    return null;

                return new AssertionFailureBuilder("Expected values to be referentially different.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawLabeledValuesWithDiffs("Unexpected Value", unexpectedValue, "Actual Value", actualValue)
                    .ToAssertionFailure();
            });
        }
        #endregion


        #region IsTrue
        /// <summary>
        /// Verifies that an actual value is true.
        /// </summary>
        /// <param name="actualValue">The actual value.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void IsTrue(bool actualValue)
        {
            IsTrue(actualValue, null, null);
        }

        /// <summary>
        /// Verifies that an actual value is true.
        /// </summary>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void IsTrue(bool actualValue, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (actualValue)
                    return null;

                return new AssertionFailureBuilder("Expected value to be true.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawActualValue(actualValue)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region IsFalse
        /// <summary>
        /// Verifies that an actual value is false.
        /// </summary>
        /// <param name="actualValue">The actual value.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void IsFalse(bool actualValue)
        {
            IsFalse(actualValue, null, null);
        }

        /// <summary>
        /// Verifies that an actual value is false.
        /// </summary>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void IsFalse(bool actualValue, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (!actualValue)
                    return null;

                return new AssertionFailureBuilder("Expected value to be false.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawActualValue(actualValue)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region IsNull
        /// <summary>
        /// Verifies that an actual value is null.
        /// </summary>
        /// <param name="actualValue">The actual value.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void IsNull(object actualValue)
        {
            IsNull(actualValue, null, null);
        }

        /// <summary>
        /// Verifies that an actual value is null.
        /// </summary>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void IsNull(object actualValue, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (actualValue == null)
                    return null;

                return new AssertionFailureBuilder("Expected value to be null.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawActualValue(actualValue)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region IsNotNull
        /// <summary>
        /// Verifies that an actual value is not null.
        /// </summary>
        /// <param name="actualValue">The actual value.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void IsNotNull(object actualValue)
        {
            IsNotNull(actualValue, null, null);
        }

        /// <summary>
        /// Verifies that an actual value is not null.
        /// </summary>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void IsNotNull(object actualValue, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (actualValue != null)
                    return null;

                return new AssertionFailureBuilder("Expected value to be non-null.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawActualValue(actualValue)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region AreApproximatelyEqual
        /// <summary>
        /// Verifies that an actual value approximately equals some expected value
        /// to within a specified delta.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The values are considered approximately equal if the absolute value of their difference
        /// is less than or equal to the delta.
        /// </para>
        /// <para>
        /// This method works with any comparable type that also supports a subtraction operator
        /// including <see cref="Single" />, <see cref="Double" />, <see cref="Decimal" />,
        /// <see cref="Int32" />, <see cref="DateTime" /> (using a <see cref="TimeSpan" /> delta),
        /// and many others.
        /// </para>
        /// </remarks>
        /// <typeparam name="TValue">The type of values to be compared.</typeparam>
        /// <typeparam name="TDifference">The type of the difference produced when the values are
        /// subtracted, for numeric types this is the same as <typeparamref name="TValue"/> but it
        /// may differ for other types.</typeparam>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="delta">The inclusive delta between the values.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreApproximatelyEqual<TValue, TDifference>(TValue expectedValue, TValue actualValue, TDifference delta)
        {
            AreApproximatelyEqual(expectedValue, actualValue, delta, null, null);
        }

        /// <summary>
        /// Verifies that an actual value approximately equals some expected value
        /// to within a specified delta.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The values are considered approximately equal if the absolute value of their difference
        /// is less than or equal to the delta.
        /// </para>
        /// <para>
        /// This method works with any comparable type that also supports a subtraction operator
        /// including <see cref="Single" />, <see cref="Double" />, <see cref="Decimal" />,
        /// <see cref="Int32" />, <see cref="DateTime" /> (using a <see cref="TimeSpan" /> delta),
        /// and many others.
        /// </para>
        /// </remarks>
        /// <typeparam name="TValue">The type of values to be compared.</typeparam>
        /// <typeparam name="TDifference">The type of the difference produced when the values are
        /// subtracted, for numeric types this is the same as <typeparamref name="TValue"/> but it
        /// may differ for other types.</typeparam>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="delta">The inclusive delta between the values.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreApproximatelyEqual<TValue, TDifference>(TValue expectedValue, TValue actualValue, TDifference delta,
            string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (ComparisonSemantics.Default.ApproximatelyEqual(expectedValue, actualValue, delta))
                    return null;

                return new AssertionFailureBuilder("Expected values to be approximately equal to within a delta.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawExpectedValue(expectedValue)
                    .AddRawActualValue(actualValue)
                    .AddRawLabeledValue("Delta", delta)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region AreNotApproximatelyEqual
        /// <summary>
        /// Verifies that an actual value does not approximately equal some unexpected value
        /// to within a specified delta.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The values are considered approximately equal if the absolute value of their difference
        /// is less than or equal to the delta.
        /// </para>
        /// <para>
        /// This method works with any comparable type that also supports a subtraction operator
        /// including <see cref="Single" />, <see cref="Double" />, <see cref="Decimal" />,
        /// <see cref="Int32" />, <see cref="DateTime" /> (using a <see cref="TimeSpan" /> delta),
        /// and many others.
        /// </para>
        /// </remarks>
        /// <typeparam name="TValue">The type of values to be compared.</typeparam>
        /// <typeparam name="TDifference">The type of the difference produced when the values are
        /// subtracted, for numeric types this is the same as <typeparamref name="TValue"/> but it
        /// may differ for other types.</typeparam>
        /// <param name="unexpectedValue">The expected value.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="delta">The inclusive delta between the values.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreNotApproximatelyEqual<TValue, TDifference>(TValue unexpectedValue, TValue actualValue, TDifference delta)
        {
            AreNotApproximatelyEqual(unexpectedValue, actualValue, delta, null, null);
        }

        /// <summary>
        /// Verifies that an actual value does not approximately equal some unexpected value
        /// to within a specified delta.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The values are considered approximately equal if the absolute value of their difference
        /// is less than or equal to the delta.
        /// </para>
        /// <para>
        /// This method works with any comparable type that also supports a subtraction operator
        /// including <see cref="Single" />, <see cref="Double" />, <see cref="Decimal" />,
        /// <see cref="Int32" />, <see cref="DateTime" /> (using a <see cref="TimeSpan" /> delta),
        /// and many others.
        /// </para>
        /// </remarks>
        /// <typeparam name="TValue">The type of values to be compared.</typeparam>
        /// <typeparam name="TDifference">The type of the difference produced when the values are
        /// subtracted, for numeric types this is the same as <typeparamref name="TValue"/> but it
        /// may differ for other types.</typeparam>
        /// <param name="unexpectedValue">The expected value.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="delta">The inclusive delta between the values.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void AreNotApproximatelyEqual<TValue, TDifference>(TValue unexpectedValue, TValue actualValue, TDifference delta,
            string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (!ComparisonSemantics.Default.ApproximatelyEqual(unexpectedValue, actualValue, delta))
                    return null;

                return new AssertionFailureBuilder("Expected values not to be approximately equal to within a delta.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawLabeledValue("Unexpected Value", unexpectedValue)
                    .AddRawActualValue(actualValue)
                    .AddRawLabeledValue("Delta", delta)
                    .ToAssertionFailure();
            });
        }
        #endregion
    }
}
