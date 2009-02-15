// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using Gallio.Framework;
using Gallio.Framework.Assertions;
using Gallio;
using System.Collections.Generic;

namespace MbUnit.Framework
{
    public abstract partial class Assert
    {
        #region GreaterThan

        /// <summary>
        /// Verifies that the left value is greater than the right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        public static void GreaterThan<T>(T left, T right)
        {
            GreaterThan(left, right, (string)null, null);
        }

        /// <summary>
        /// Verifies that the left value is greater than the right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void GreaterThan<T>(T left, T right, string messageFormat, params object[] messageArgs)
        {
            GreaterThan(left, right, (Comparison<T>)null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that the left value is greater than the right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void GreaterThan<T>(T left, T right, IComparer<T> comparer)
        {
            GreaterThan(left, right, comparer, null, null);
        }

        /// <summary>
        /// Verifies that the left value is greater than the right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void GreaterThan<T>(T left, T right, IComparer<T> comparer, string messageFormat, params object[] messageArgs)
        {
            GreaterThan(left, right, comparer != null ? comparer.Compare : (Comparison<T>)null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that the left value is greater than the right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void GreaterThan<T>(T left, T right, Comparison<T> comparer)
        {
            GreaterThan(left, right, comparer, null, null);
        }

        /// <summary>
        /// Verifies that the left value is greater than the right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void GreaterThan<T>(T left, T right, Comparison<T> comparer, string messageFormat, params object[] messageArgs)
        {
            AssertOrder(left, right, comparer
                    , "Expected left to be greater than right."
                    , compareResult => compareResult > 0
                    , messageFormat, messageArgs);
        }
        #endregion

        #region GreaterThanOrEqual
        /// <summary>
        /// Verifies that the left value is greater than or equal to the right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        public static void GreaterThanOrEqualTo<T>(T left, T right)
        {
            GreaterThanOrEqualTo(left, right, (string)null, null);
        }

        /// <summary>
        /// Verifies that the left value is greater than or equal to the right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void GreaterThanOrEqualTo<T>(T left, T right, string messageFormat, params object[] messageArgs)
        {
            GreaterThanOrEqualTo(left, right, (Comparison<T>)null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that the left value is greater than or equal to the right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void GreaterThanOrEqualTo<T>(T left, T right, IComparer<T> comparer)
        {
            GreaterThanOrEqualTo(left, right, comparer, null, null);
        }

        /// <summary>
        /// Verifies that the left value is greater than or equal to the right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void GreaterThanOrEqualTo<T>(T left, T right, IComparer<T> comparer, string messageFormat, params object[] messageArgs)
        {
            GreaterThanOrEqualTo(left, right, comparer != null ? comparer.Compare : (Comparison<T>)null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that the left value is greater than or equal to the right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void GreaterThanOrEqualTo<T>(T left, T right, Comparison<T> comparer)
        {
            GreaterThanOrEqualTo(left, right, comparer, null, null);
        }

        /// <summary>
        /// Verifies that the left value is greater or equal than right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void GreaterThanOrEqualTo<T>(T left, T right, Comparison<T> comparer, string messageFormat, params object[] messageArgs)
        {
            AssertOrder(left, right, comparer
                 , "Expected left to be greater or equal than right."
                 , compareResult => compareResult >= 0
                 , messageFormat, messageArgs);
        }

        #endregion

        #region LessThan

        /// <summary>
        /// Verifies that the left value is less than the right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        public static void LessThan<T>(T left, T right)
        {
            LessThan(left, right, (string)null, null);
        }

        /// <summary>
        /// Verifies that the left value is less than the right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void LessThan<T>(T left, T right, string messageFormat, params object[] messageArgs)
        {
            LessThan(left, right, (Comparison<T>)null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that the left value is less than the right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void LessThan<T>(T left, T right, IComparer<T> comparer)
        {
            LessThan(left, right, comparer, null, null);
        }

        /// <summary>
        /// Verifies that the left value is less than the right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void LessThan<T>(T left, T right, IComparer<T> comparer, string messageFormat, params object[] messageArgs)
        {
            LessThan(left, right, comparer != null ? comparer.Compare : (Comparison<T>)null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that the left value is less than the right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void LessThan<T>(T left, T right, Comparison<T> comparer)
        {
            LessThan(left, right, comparer, null, null);
        }

        /// <summary>
        /// Verifies that the left value is less than the right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void LessThan<T>(T left, T right, Comparison<T> comparer, string messageFormat, params object[] messageArgs)
        {
            AssertOrder(left, right, comparer
                    , "Expected left to be less than right."
                    , compareResult => compareResult < 0
                    , messageFormat, messageArgs);
        }
        #endregion

        #region LessThanOrEqual

        /// <summary>
        /// Verifies that the left value is less than or equal to the right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        public static void LessThanOrEqualTo<T>(T left, T right)
        {
            LessThanOrEqualTo(left, right, (string)null, null);
        }

        /// <summary>
        /// Verifies that the left value is less than or equal to the right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void LessThanOrEqualTo<T>(T left, T right, string messageFormat, params object[] messageArgs)
        {
            LessThanOrEqualTo(left, right, (Comparison<T>)null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that the left value is less than or equal to the right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void LessThanOrEqualTo<T>(T left, T right, IComparer<T> comparer)
        {
            LessThanOrEqualTo(left, right, comparer, null, null);
        }

        /// <summary>
        /// Verifies that the left value is less than or equal to the right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void LessThanOrEqualTo<T>(T left, T right, IComparer<T> comparer, string messageFormat, params object[] messageArgs)
        {
            LessThanOrEqualTo(left, right, comparer != null ? comparer.Compare : (Comparison<T>)null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that the left value is less than or equal to the right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void LessThanOrEqualTo<T>(T left, T right, Comparison<T> comparer)
        {
            LessThanOrEqualTo(left, right, comparer, null, null);
        }

        /// <summary>
        /// Verifies that the left value is less than or equal to the right value according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void LessThanOrEqualTo<T>(T left, T right, Comparison<T> comparer, string messageFormat, params object[] messageArgs)
        {
            AssertOrder(left, right, comparer
                    , "Expected left to be less or equal than right."
                    , compareResult => compareResult <= 0
                    , messageFormat, messageArgs);
        }
        #endregion

        #region Between

        /// <summary>
        /// Verifies that a test value is between left and right values according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="actualValue">The actual value</param>
        /// <param name="minimum">Inclusive minimum value</param>
        /// <param name="maximum">Inclusive maximum value</param>
        public static void Between<T>(T actualValue, T minimum, T maximum)
        {
            Between(actualValue, minimum, maximum, (string)null, null);
        }

        /// <summary>
        /// Verifies that a test value is between left and right values according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="actualValue">The actual value</param>
        /// <param name="minimum">Inclusive minimum value</param>
        /// <param name="maximum">Inclusive maximum value</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void Between<T>(T actualValue, T minimum, T maximum, string messageFormat, params object[] messageArgs)
        {
            Between(actualValue, minimum, maximum, (Comparison<T>)null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that a test value is between left and right values according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="actualValue">The actual value</param>
        /// <param name="minimum">Inclusive minimum value</param>
        /// <param name="maximum">Inclusive maximum value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void Between<T>(T actualValue, T minimum, T maximum, IComparer<T> comparer)
        {
            Between(actualValue, minimum, maximum, comparer, null, null);
        }

        /// <summary>
        /// Verifies that a test value is between left and right values according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="actualValue">The actual value</param>
        /// <param name="minimum">Inclusive minimum value</param>
        /// <param name="maximum">Inclusive maximum value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void Between<T>(T actualValue, T minimum, T maximum, IComparer<T> comparer, string messageFormat, params object[] messageArgs)
        {
            Between(actualValue, minimum, maximum, comparer != null ? comparer.Compare : (Comparison<T>)null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that a test value is between left and right values according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="actualValue">The actual value</param>
        /// <param name="minimum">Inclusive minimum value</param>
        /// <param name="maximum">Inclusive maximum value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void Between<T>(T actualValue, T minimum, T maximum, Comparison<T> comparer)
        {
            Between(actualValue, minimum, maximum, comparer, null, null);
        }

        /// <summary>
        /// Verifies that a test value is between left and right values according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="actualValue">The actual value</param>
        /// <param name="minimum">Inclusive minimum value</param>
        /// <param name="maximum">Inclusive maximum value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void Between<T>(T actualValue, T minimum, T maximum, Comparison<T> comparer, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (comparer == null)
                    comparer = ComparisonSemantics.Compare;

                if (comparer(actualValue, minimum) >= 0
                    && comparer(actualValue, maximum) <= 0)
                    return null;

                return new AssertionFailureBuilder("The actual value should be between the minimum and maximum values.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawActualValue(actualValue)
                    .AddRawLabeledValue("Minimum Value", minimum)
                    .AddRawLabeledValue("Maximum Value", maximum)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region NotBetween

        /// <summary>
        /// Verifies that a test value is not between left and right values according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="actualValue">The actual value</param>
        /// <param name="minimum">Inclusive minimum value</param>
        /// <param name="maximum">Inclusive maximum value</param>
        public static void NotBetween<T>(T actualValue, T minimum, T maximum)
        {
            NotBetween(actualValue, minimum, maximum, (string)null, null);
        }

        /// <summary>
        /// Verifies that a test value is not between left and right values according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="actualValue">The actual value</param>
        /// <param name="minimum">Inclusive minimum value</param>
        /// <param name="maximum">Inclusive maximum value</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void NotBetween<T>(T actualValue, T minimum, T maximum, string messageFormat, params object[] messageArgs)
        {
            NotBetween(actualValue, minimum, maximum, (Comparison<T>)null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that a test value is not between left and right values according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="actualValue">The actual value</param>
        /// <param name="minimum">Inclusive minimum value</param>
        /// <param name="maximum">Inclusive maximum value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void NotBetween<T>(T actualValue, T minimum, T maximum, IComparer<T> comparer)
        {
            NotBetween(actualValue, minimum, maximum, comparer, null, null);
        }

        /// <summary>
        /// Verifies that a test value is not between left and right values according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="actualValue">The actual value</param>
        /// <param name="minimum">Inclusive minimum value</param>
        /// <param name="maximum">Inclusive maximum value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void NotBetween<T>(T actualValue, T minimum, T maximum, IComparer<T> comparer, string messageFormat, params object[] messageArgs)
        {
            NotBetween(actualValue, minimum, maximum, comparer != null ? comparer.Compare : (Comparison<T>)null, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that a test value is not between left and right values according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="actualValue">The actual value</param>
        /// <param name="minimum">Inclusive minimum value</param>
        /// <param name="maximum">Inclusive maximum value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void NotBetween<T>(T actualValue, T minimum, T maximum, Comparison<T> comparer)
        {
            NotBetween(actualValue, minimum, maximum, comparer, null, null);
        }

        /// <summary>
        /// Verifies that a test value is not between left and right values according to a particular comparer.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="actualValue">The actual value</param>
        /// <param name="minimum">Inclusive minimum value</param>
        /// <param name="maximum">Inclusive maximum value</param>
        /// <param name="comparer">The comparer to use, or null to use the default one</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void NotBetween<T>(T actualValue, T minimum, T maximum, Comparison<T> comparer, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (comparer == null)
                    comparer = ComparisonSemantics.Compare;

                if (comparer(actualValue, minimum) < 0
                    || comparer(actualValue, maximum) > 0)
                    return null;

                return new AssertionFailureBuilder("The actual value should not be between the minimum and maximum values.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawActualValue(actualValue)
                    .AddRawLabeledValue("Minimum Value", minimum)
                    .AddRawLabeledValue("Maximum Value", maximum)
                    .ToAssertionFailure();
            });
        }
        #endregion

        private static void AssertOrder<T>(T left, T right, Comparison<T> comparer, string exceptionMessage,
            Func<int, bool> orderingPredicate, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(() =>
            {
                if (comparer == null)
                    comparer = ComparisonSemantics.Compare;

                if (orderingPredicate(comparer(left, right)))
                    return null;

                return new AssertionFailureBuilder(exceptionMessage)
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawLabeledValue("Left Value", left)
                    .AddRawLabeledValue("Right Value", right)
                    .ToAssertionFailure();
            });
        }
    }
}
