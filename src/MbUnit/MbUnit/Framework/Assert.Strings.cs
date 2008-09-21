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
using System.Text.RegularExpressions;
using Gallio.Framework.Assertions;

namespace MbUnit.Framework
{
    public abstract partial class Assert
    {
        #region Contains
        /// <summary>
        /// Verifies that a string contains some expected value.
        /// </summary>
        /// <param name="actualValue">The actual value</param>
        /// <param name="expectedSubstring">The expected substring</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void Contains(string actualValue, string expectedSubstring)
        {
            Contains(actualValue, expectedSubstring, null);
        }

        /// <summary>
        /// Verifies that a string contains some expected value.
        /// </summary>
        /// <param name="actualValue">The actual value</param>
        /// <param name="expectedSubstring">The expected substring</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void Contains(string actualValue, string expectedSubstring, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (actualValue.Contains(expectedSubstring))
                    return null;

                return new AssertionFailureBuilder("Expected string to contain a particular substring.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawLabeledValue("Expected Substring", expectedSubstring)
                    .AddRawActualValue(actualValue)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region DoesNotContain
        /// <summary>
        /// Verifies that a string does not contain some unexpected substring.
        /// </summary>
        /// <param name="actualValue">The actual value</param>
        /// <param name="unexpectedSubstring">The unexpected substring</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void DoesNotContain(string actualValue, string unexpectedSubstring)
        {
            DoesNotContain(actualValue, unexpectedSubstring, null);
        }

        /// <summary>
        /// Verifies that a string does not contain some unexpected substring.
        /// </summary>
        /// <param name="actualValue">The actual value</param>
        /// <param name="unexpectedSubstring">The unexpected substring</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void DoesNotContain(string actualValue, string unexpectedSubstring, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (!actualValue.Contains(unexpectedSubstring))
                    return null;

                return new AssertionFailureBuilder("Expected string to not contain a particular substring.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawLabeledValue("Unexpected Substring", unexpectedSubstring)
                    .AddRawActualValue(actualValue)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region AreEqual
        /// <summary>
        /// Asserts that two strings are equal according to a particular string comparison mode.
        /// </summary>
        /// <param name="expectedValue">The expected value</param>
        /// <param name="actualValue">The actual value</param>
        /// <param name="comparisonType">The string comparison type</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void AreEqual(string expectedValue, string actualValue, StringComparison comparisonType)
        {
            AreEqual(expectedValue, actualValue, comparisonType, null, null);
        }

        /// <summary>
        /// Asserts that two strings are equal according to a particular string comparison mode.
        /// </summary>
        /// <param name="expectedValue">The expected value</param>
        /// <param name="actualValue">The actual value</param>
        /// <param name="comparisonType">The string comparison type</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void AreEqual(string expectedValue, string actualValue, StringComparison comparisonType, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (String.Compare(expectedValue, actualValue, comparisonType) == 0)
                    return null;

                AssertionFailureBuilder builder = new AssertionFailureBuilder("Expected values to be equal according to string comparison type.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawLabeledValue("Comparison Type", comparisonType);

                if (comparisonType == StringComparison.CurrentCultureIgnoreCase
                    || comparisonType == StringComparison.InvariantCultureIgnoreCase
                    || comparisonType == StringComparison.OrdinalIgnoreCase)
                    builder.AddRawLabeledValue("Expected Value", expectedValue)
                        .AddRawLabeledValue("Actual Value", actualValue);
                else
                    builder.AddRawExpectedAndActualValuesWithDiffs(expectedValue, actualValue);

                return builder.ToAssertionFailure();
            });
        }
        #endregion

        #region AreNotEqual
        /// <summary>
        /// Asserts that two strings are not equal according to a particular string comparison mode.
        /// </summary>
        /// <param name="unexpectedValue">The unexpected value</param>
        /// <param name="actualValue">The actual value</param>
        /// <param name="comparisonType">The string comparison type</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void AreNotEqual(string unexpectedValue, string actualValue, StringComparison comparisonType)
        {
            AreNotEqual(unexpectedValue, actualValue, comparisonType, null, null);
        }

        /// <summary>
        /// Asserts that two strings are not equal according to a particular string comparison mode.
        /// </summary>
        /// <param name="unexpectedValue">The unexpected value</param>
        /// <param name="actualValue">The actual value</param>
        /// <param name="comparisonType">The string comparison type</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void AreNotEqual(string unexpectedValue, string actualValue, StringComparison comparisonType, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(delegate
            {
                if (String.Compare(unexpectedValue, actualValue, comparisonType) != 0)
                    return null;

                return new AssertionFailureBuilder("Expected values to be unequal according to string comparison type.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawLabeledValue("Comparison Type", comparisonType)
                    .AddRawLabeledValue("Unexpected Value", unexpectedValue)
                    .AddRawLabeledValue("Actual Value", actualValue)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region FullMatch
        /// <summary>
        /// Verifies that testValue matches regular expression pattern exactly.
        /// </summary>
        /// <param name="testValue">The test value</param>
        /// <param name="regexPattern">Regular expression pattern</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void FullMatch(string testValue, string regexPattern)
        {
            FullMatch(testValue, regexPattern, null, null);
        }

        /// <summary>
        /// Verifies that testValue matches regular expression pattern exactly.
        /// </summary>
        /// <param name="testValue">The test value</param>
        /// <param name="regexPattern">Regular expression pattern</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void FullMatch(string testValue, string regexPattern, string messageFormat, params object[] messageArgs)
        {
            FullMatch(testValue, new Regex(regexPattern), messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that testValue matches regular expression pattern exactly.
        /// </summary>
        /// <param name="testValue">The test value</param>
        /// <param name="regEx">Regular expression</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void FullMatch(string testValue, Regex regEx)
        {
            FullMatch(testValue, regEx, null, null);
        }

        /// <summary>
        /// Verifies that testValue matches regular expression pattern exactly.
        /// </summary>
        /// <param name="testValue">The test value</param>
        /// <param name="regEx">Regular expression</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void FullMatch(string testValue, Regex regEx, string messageFormat, params object[] messageArgs)
        {
            if (testValue == null)
                throw new ArgumentNullException("testValue");
            if (regEx == null)
                throw new ArgumentNullException("regEx");

            AssertionHelper.Verify(delegate
            {
                Match match = regEx.Match(testValue);
                if (match.Success && testValue.Length.Equals(match.Length))
                    return null;

                return new AssertionFailureBuilder("Expected to have an exact match.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawLabeledValue("Test Value", testValue)
                    .AddRawLabeledValue("Regex Pattern", regEx.ToString())
                    .ToAssertionFailure();
            });
        }

        #endregion

        #region Like

        /// <summary>
        /// Verifies that testValue matches regular expression pattern.
        /// </summary>
        /// <param name="testValue">The test value</param>
        /// <param name="regexPattern">Regular expression pattern</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void Like(string testValue, string regexPattern)
        {
            Like(testValue, regexPattern, null, null);
        }

        /// <summary>
        /// Verifies that testValue matches regular expression pattern.
        /// </summary>
        /// <param name="testValue">The test value</param>
        /// <param name="regexPattern">Regular expression pattern</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void Like(string testValue, string regexPattern, string messageFormat, params object[] messageArgs)
        {
            Like(testValue, new Regex(regexPattern), messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that testValue matches regular expression pattern.
        /// </summary>
        /// <param name="testValue">The test value</param>
        /// <param name="regEx">Regular expression</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void Like(string testValue, Regex regEx)
        {
            Like(testValue, regEx, null, null);
        }

        /// <summary>
        /// Verifies that testValue matches regular expression pattern.
        /// </summary>
        /// <param name="testValue">The test value</param>
        /// <param name="regEx">Regular expression</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void Like(string testValue, Regex regEx, string messageFormat, params object[] messageArgs)
        {
            if (testValue == null)
                throw new ArgumentNullException("testValue");
            if (regEx == null)
                throw new ArgumentNullException("regEx");

            AssertionHelper.Verify(delegate
            {
                if (regEx.Match(testValue).Success)
                    return null;

                return new AssertionFailureBuilder("Expected to match Regex pattern.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawLabeledValue("Test Value", testValue)
                    .AddRawLabeledValue("Regex Pattern", regEx.ToString())
                    .ToAssertionFailure();
            });
        }

        #endregion

        #region NotLike

        /// <summary>
        /// Verifies that testValue doesn't matches regular expression pattern.
        /// </summary>
        /// <param name="testValue">The test value</param>
        /// <param name="regexPattern">Regular expression pattern</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void NotLike(string testValue, string regexPattern)
        {
            NotLike(testValue, regexPattern, null, null);
        }

        /// <summary>
        /// Verifies that testValue doesn't matches regular expression pattern.
        /// </summary>
        /// <param name="testValue">The test value</param>
        /// <param name="regexPattern">Regular expression pattern</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void NotLike(string testValue, string regexPattern, string messageFormat, params object[] messageArgs)
        {
            NotLike(testValue, new Regex(regexPattern), messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that testValue doesn't matches regular expression pattern.
        /// </summary>
        /// <param name="testValue">The test value</param>
        /// <param name="regEx">Regular expression</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void NotLike(string testValue, Regex regEx)
        {
            NotLike(testValue, regEx, null, null);
        }

        /// <summary>
        /// Verifies that testValue doesn't matches regular expression pattern.
        /// </summary>
        /// <param name="testValue">The test value</param>
        /// <param name="regEx">Regular expression</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void NotLike(string testValue, Regex regEx, string messageFormat, params object[] messageArgs)
        {
            if (testValue == null)
                throw new ArgumentNullException("testValue");
            if (regEx == null)
                throw new ArgumentNullException("regEx");

            AssertionHelper.Verify(delegate
            {
                if (!regEx.Match(testValue).Success)
                    return null;

                return new AssertionFailureBuilder("Expected not to match Regex pattern.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawLabeledValue("Test Value", testValue)
                    .AddRawLabeledValue("Regex Pattern", regEx.ToString())
                    .ToAssertionFailure();
            });
        }

        #endregion

        #region StartsWith

        /// <summary>
        /// Verifies that testValue starts with the specified pattern.
        /// </summary>
        /// <param name="testValue">The test value</param>
        /// <param name="pattern">Regular expression pattern</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void StartsWith(string testValue, string pattern)
        {
            StartsWith(testValue, pattern, null, null);
        }

        /// <summary>
        /// Verifies that testValue starts with the specified pattern.
        /// </summary>
        /// <param name="testValue">The test value</param>
        /// <param name="pattern">Regular expression pattern</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void StartsWith(string testValue, string pattern, string messageFormat, params object[] messageArgs)
        {
            if (testValue == null && pattern == null)
                return;

            if (testValue == null)
                throw new ArgumentNullException("testValue");
            if (pattern == null)
                throw new ArgumentNullException("pattern");

            AssertionHelper.Verify(delegate
            {
                if (testValue.StartsWith(pattern))
                    return null;

                return new AssertionFailureBuilder("Expected to start with the specified pattern.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawLabeledValue("Test Value", testValue)
                    .AddRawLabeledValue("Pattern", pattern)
                    .ToAssertionFailure();
            });
        }

        #endregion

        #region EndsWith

        /// <summary>
        /// Verifies that testValue ends with the specified pattern.
        /// </summary>
        /// <param name="testValue">The test value</param>
        /// <param name="pattern">Regular expression pattern</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void EndsWith(string testValue, string pattern)
        {
            EndsWith(testValue, pattern, null, null);
        }

        /// <summary>
        /// Verifies that testValue ends with the specified pattern.
        /// </summary>
        /// <param name="testValue">The test value</param>
        /// <param name="pattern">Regular expression pattern</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void EndsWith(string testValue, string pattern, string messageFormat, params object[] messageArgs)
        {
            if (testValue == null && pattern == null)
                return;

            if (testValue == null)
                throw new ArgumentNullException("testValue");
            if (pattern == null)
                throw new ArgumentNullException("pattern");

            AssertionHelper.Verify(delegate
            {
                if (testValue.EndsWith(pattern))
                    return null;

                return new AssertionFailureBuilder("Expected to end with the specified pattern.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawLabeledValue("Test Value", testValue)
                    .AddRawLabeledValue("Pattern", pattern)
                    .ToAssertionFailure();
            });
        }

        #endregion
    }
}