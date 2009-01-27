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
        /// <remarks>
        /// <para>
        /// This assertion will fail if the string is null.
        /// </para>
        /// </remarks>
        /// <param name="actualValue">The actual value</param>
        /// <param name="expectedSubstring">The expected substring</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedSubstring"/> is null</exception>
        public static void Contains(string actualValue, string expectedSubstring)
        {
            Contains(actualValue, expectedSubstring, null);
        }

        /// <summary>
        /// Verifies that a string contains some expected value.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the string is null.
        /// </para>
        /// </remarks>
        /// <param name="actualValue">The actual value</param>
        /// <param name="expectedSubstring">The expected substring</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedSubstring"/> is null</exception>
        public static void Contains(string actualValue, string expectedSubstring, string messageFormat, params object[] messageArgs)
        {
            if (expectedSubstring == null)
                throw new ArgumentNullException("expectedSubstring");

            AssertionHelper.Verify(delegate
            {
                if (actualValue != null && actualValue.Contains(expectedSubstring))
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
        /// <remarks>
        /// <para>
        /// This assertion will fail if the string is null.
        /// </para>
        /// </remarks>
        /// <param name="actualValue">The actual value</param>
        /// <param name="unexpectedSubstring">The unexpected substring</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="unexpectedSubstring"/> is null</exception>
        public static void DoesNotContain(string actualValue, string unexpectedSubstring)
        {
            DoesNotContain(actualValue, unexpectedSubstring, null);
        }

        /// <summary>
        /// Verifies that a string does not contain some unexpected substring.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the string is null.
        /// </para>
        /// </remarks>
        /// <param name="actualValue">The actual value</param>
        /// <param name="unexpectedSubstring">The unexpected substring</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="unexpectedSubstring"/> is null</exception>
        public static void DoesNotContain(string actualValue, string unexpectedSubstring, string messageFormat, params object[] messageArgs)
        {
            if (unexpectedSubstring == null)
                throw new ArgumentNullException("unexpectedSubstring");

            AssertionHelper.Verify(delegate
            {
                if (actualValue != null && !actualValue.Contains(unexpectedSubstring))
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
                    .AddLabeledValue("Comparison Type", comparisonType.ToString());

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
        /// Verifies that a string matches regular expression pattern exactly.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the string is null.
        /// </para>
        /// </remarks>
        /// <seealso cref="Regex.Match(string, string, RegexOptions)"/>
        /// <param name="actualValue">The actual value</param>
        /// <param name="regexPattern">The regular expression pattern</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="regexPattern"/> is null</exception>
        public static void FullMatch(string actualValue, string regexPattern)
        {
            FullMatch(actualValue, regexPattern, RegexOptions.None, null, null);
        }

        /// <summary>
        /// Verifies that a string matches regular expression pattern exactly.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the string is null.
        /// </para>
        /// </remarks>
        /// <seealso cref="Regex.Match(string, string, RegexOptions)"/>
        /// <param name="actualValue">The actual value</param>
        /// <param name="regexPattern">The regular expression pattern</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="regexPattern"/> is null</exception>
        public static void FullMatch(string actualValue, string regexPattern, string messageFormat, params object[] messageArgs)
        {
            FullMatch(actualValue, regexPattern, RegexOptions.None, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that a string matches regular expression pattern exactly.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the string is null.
        /// </para>
        /// </remarks>
        /// <seealso cref="Regex.Match(string, string, RegexOptions)"/>
        /// <param name="actualValue">The actual value</param>
        /// <param name="regexPattern">The regular expression pattern</param>
        /// <param name="regexOptions">The regular expression options</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="regexPattern"/> is null</exception>
        public static void FullMatch(string actualValue, string regexPattern, RegexOptions regexOptions)
        {
            FullMatch(actualValue, regexPattern, regexOptions, null, null);
        }

        /// <summary>
        /// Verifies that a string matches regular expression pattern exactly.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the string is null.
        /// </para>
        /// </remarks>
        /// <seealso cref="Regex.Match(string, string, RegexOptions)"/>
        /// <param name="actualValue">The actual value</param>
        /// <param name="regexPattern">The regular expression pattern</param>
        /// <param name="regexOptions">The regular expression options</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="regexPattern"/> is null</exception>
        public static void FullMatch(string actualValue, string regexPattern, RegexOptions regexOptions, string messageFormat, params object[] messageArgs)
        {
            if (regexPattern == null)
                throw new ArgumentNullException("regexPattern");

            FullMatch(actualValue, new Regex(regexPattern, regexOptions), messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that a string matches regular expression pattern exactly.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the string is null.
        /// </para>
        /// </remarks>
        /// <seealso cref="Regex.Match(string)"/>
        /// <param name="actualValue">The actual value</param>
        /// <param name="regex">The regular expression</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="regex"/> is null</exception>
        public static void FullMatch(string actualValue, Regex regex)
        {
            FullMatch(actualValue, regex, null, null);
        }

        /// <summary>
        /// Verifies that a string matches regular expression pattern exactly.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the string is null.
        /// </para>
        /// </remarks>
        /// <seealso cref="Regex.Match(string)"/>
        /// <param name="actualValue">The actual value</param>
        /// <param name="regex">The regular expression</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="regex"/> is null</exception>
        public static void FullMatch(string actualValue, Regex regex, string messageFormat, params object[] messageArgs)
        {
            if (regex == null)
                throw new ArgumentNullException("regex");

            AssertionHelper.Verify(delegate
            {
                if (actualValue != null)
                {
                    Match match = regex.Match(actualValue);
                    if (match.Success && actualValue.Length == match.Length)
                        return null;
                }

                return new AssertionFailureBuilder("Expected a string to exactly match a regular expression pattern.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawActualValue(actualValue)
                    .AddRawLabeledValue("Regex Pattern", regex.ToString())
                    .ToAssertionFailure();
            });
        }

        #endregion

        #region Like

        /// <summary>
        /// Verifies that a string contains a full or partial match of a regular expression pattern.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the string is null.
        /// </para>
        /// </remarks>
        /// <seealso cref="Regex.IsMatch(string, string, RegexOptions)"/>
        /// <param name="actualValue">The actual value</param>
        /// <param name="regexPattern">The regular expression pattern</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="regexPattern"/> is null</exception>
        public static void Like(string actualValue, string regexPattern)
        {
            Like(actualValue, regexPattern, RegexOptions.None, null, null);
        }

        /// <summary>
        /// Verifies that a string contains a full or partial match of a regular expression pattern.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the string is null.
        /// </para>
        /// </remarks>
        /// <seealso cref="Regex.IsMatch(string, string, RegexOptions)"/>
        /// <param name="actualValue">The actual value</param>
        /// <param name="regexPattern">The regular expression pattern</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="regexPattern"/> is null</exception>
        public static void Like(string actualValue, string regexPattern, string messageFormat, params object[] messageArgs)
        {
            Like(actualValue, regexPattern, RegexOptions.None, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that a string contains a full or partial match of a regular expression pattern.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the string is null.
        /// </para>
        /// </remarks>
        /// <seealso cref="Regex.IsMatch(string, string, RegexOptions)"/>
        /// <param name="actualValue">The actual value</param>
        /// <param name="regexPattern">The regular expression pattern</param>
        /// <param name="regexOptions">The regular expression options</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="regexPattern"/> is null</exception>
        public static void Like(string actualValue, string regexPattern, RegexOptions regexOptions)
        {
            Like(actualValue, regexPattern, regexOptions, null, null);
        }

        /// <summary>
        /// Verifies that a string contains a full or partial match of a regular expression pattern.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the string is null.
        /// </para>
        /// </remarks>
        /// <seealso cref="Regex.IsMatch(string, string, RegexOptions)"/>
        /// <param name="actualValue">The actual value</param>
        /// <param name="regexPattern">The regular expression pattern</param>
        /// <param name="regexOptions">The regular expression options</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="regexPattern"/> is null</exception>
        public static void Like(string actualValue, string regexPattern, RegexOptions regexOptions, string messageFormat, params object[] messageArgs)
        {
            if (regexPattern == null)
                throw new ArgumentNullException("regexPattern");

            Like(actualValue, new Regex(regexPattern, regexOptions), messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that a string contains a full or partial match of a regular expression pattern.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the string is null.
        /// </para>
        /// </remarks>
        /// <seealso cref="Regex.IsMatch(string)"/>
        /// <param name="actualValue">The actual value</param>
        /// <param name="regex">The regular expression</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="regex"/> is null</exception>
        public static void Like(string actualValue, Regex regex)
        {
            Like(actualValue, regex, null, null);
        }

        /// <summary>
        /// Verifies that a string contains a full or partial match of a regular expression pattern.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the string is null.
        /// </para>
        /// </remarks>
        /// <seealso cref="Regex.IsMatch(string)"/>
        /// <param name="actualValue">The actual value</param>
        /// <param name="regex">The regular expression</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="regex"/> is null</exception>
        public static void Like(string actualValue, Regex regex, string messageFormat, params object[] messageArgs)
        {
            if (regex == null)
                throw new ArgumentNullException("regex");

            AssertionHelper.Verify(delegate
            {
                if (actualValue != null && regex.IsMatch(actualValue))
                    return null;

                return new AssertionFailureBuilder("Expected a string to contain a full or partial match of a regular expression pattern.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawActualValue(actualValue)
                    .AddRawLabeledValue("Regex Pattern", regex.ToString())
                    .ToAssertionFailure();
            });
        }

        #endregion

        #region NotLike

        /// <summary>
        /// Verifies that a string does not contain a full or partial match of a regular expression pattern.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the string is null.
        /// </para>
        /// </remarks>
        /// <seealso cref="Regex.IsMatch(string, string, RegexOptions)"/>
        /// <param name="actualValue">The actual value</param>
        /// <param name="regexPattern">The regular expression pattern</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="regexPattern"/> is null</exception>
        public static void NotLike(string actualValue, string regexPattern)
        {
            NotLike(actualValue, regexPattern, RegexOptions.None, null, null);
        }

        /// <summary>
        /// Verifies that a string does not contain a full or partial match of a regular expression pattern.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the string is null.
        /// </para>
        /// </remarks>
        /// <seealso cref="Regex.IsMatch(string, string, RegexOptions)"/>
        /// <param name="actualValue">The actual value</param>
        /// <param name="regexPattern">The regular expression pattern</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="regexPattern"/> is null</exception>
        public static void NotLike(string actualValue, string regexPattern, string messageFormat, params object[] messageArgs)
        {
            NotLike(actualValue, regexPattern, RegexOptions.None, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that a string does not contain a full or partial match of a regular expression pattern.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the string is null.
        /// </para>
        /// </remarks>
        /// <seealso cref="Regex.IsMatch(string, string, RegexOptions)"/>
        /// <param name="actualValue">The actual value</param>
        /// <param name="regexPattern">The regular expression pattern</param>
        /// <param name="regexOptions">The regular expression options</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="regexPattern"/> is null</exception>
        public static void NotLike(string actualValue, string regexPattern, RegexOptions regexOptions)
        {
            NotLike(actualValue, regexPattern, regexOptions, null, null);
        }

        /// <summary>
        /// Verifies that a string does not contain a full or partial match of a regular expression pattern.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the string is null.
        /// </para>
        /// </remarks>
        /// <seealso cref="Regex.IsMatch(string, string, RegexOptions)"/>
        /// <param name="actualValue">The actual value</param>
        /// <param name="regexPattern">The regular expression pattern</param>
        /// <param name="regexOptions">The regular expression options</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="regexPattern"/> is null</exception>
        public static void NotLike(string actualValue, string regexPattern, RegexOptions regexOptions, string messageFormat, params object[] messageArgs)
        {
            if (regexPattern == null)
                throw new ArgumentNullException("regexPattern");

            NotLike(actualValue, new Regex(regexPattern, regexOptions), messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that a string does not contain a full or partial match of a regular expression pattern.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the string is null.
        /// </para>
        /// </remarks>
        /// <seealso cref="Regex.IsMatch(string)"/>
        /// <param name="actualValue">The actual value</param>
        /// <param name="regex">The regular expression</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="regex"/> is null</exception>
        public static void NotLike(string actualValue, Regex regex)
        {
            NotLike(actualValue, regex, null, null);
        }

        /// <summary>
        /// Verifies that a string does not contain a full or partial match of a regular expression pattern.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the string is null.
        /// </para>
        /// </remarks>
        /// <seealso cref="Regex.IsMatch(string)"/>
        /// <param name="actualValue">The actual value</param>
        /// <param name="regex">The regular expression</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="regex"/> is null</exception>
        public static void NotLike(string actualValue, Regex regex, string messageFormat, params object[] messageArgs)
        {
            if (regex == null)
                throw new ArgumentNullException("regex");

            AssertionHelper.Verify(delegate
            {
                if (actualValue != null && ! regex.IsMatch(actualValue))
                    return null;

                return new AssertionFailureBuilder("Expected a string to not contain a full or partial match of a regular expression pattern.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawActualValue(actualValue)
                    .AddRawLabeledValue("Regex Pattern", regex.ToString())
                    .ToAssertionFailure();
            });
        }

        #endregion

        #region StartsWith

        /// <summary>
        /// Verifies that a string starts with the specified text.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the string is null.
        /// </para>
        /// </remarks>
        /// <seealso cref="String.StartsWith(string)"/>
        /// <param name="actualValue">The actual value</param>
        /// <param name="expectedText">The expected pattern</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedText"/> is null</exception>
        public static void StartsWith(string actualValue, string expectedText)
        {
            StartsWith(actualValue, expectedText, null, null);
        }

        /// <summary>
        /// Verifies that a string starts with the specified text.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the string is null.
        /// </para>
        /// </remarks>
        /// <seealso cref="String.StartsWith(string)"/>
        /// <param name="actualValue">The actual value</param>
        /// <param name="expectedText">The expected pattern</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedText"/> is null</exception>
        public static void StartsWith(string actualValue, string expectedText, string messageFormat, params object[] messageArgs)
        {
            if (expectedText == null)
                throw new ArgumentNullException("expectedText");

            AssertionHelper.Verify(delegate
            {
                if (actualValue != null && actualValue.StartsWith(expectedText))
                    return null;

                return new AssertionFailureBuilder("Expected string to start with the specified text.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawActualValue(actualValue)
                    .AddRawLabeledValue("Expected Text", expectedText)
                    .ToAssertionFailure();
            });
        }

        #endregion

        #region EndsWith

        /// <summary>
        /// Verifies that a string ends with the specified text.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the string is null.
        /// </para>
        /// </remarks>
        /// <seealso cref="String.EndsWith(string)"/>
        /// <param name="actualValue">The actual value</param>
        /// <param name="expectedText">The expected pattern</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedText"/> is null</exception>
        public static void EndsWith(string actualValue, string expectedText)
        {
            EndsWith(actualValue, expectedText, null, null);
        }

        /// <summary>
        /// Verifies that a string ends with the specified text.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the string is null.
        /// </para>
        /// </remarks>
        /// <seealso cref="String.EndsWith(string)"/>
        /// <param name="actualValue">The actual value</param>
        /// <param name="expectedText">The expected pattern</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedText"/> is null</exception>
        public static void EndsWith(string actualValue, string expectedText, string messageFormat, params object[] messageArgs)
        {
            if (expectedText == null)
                throw new ArgumentNullException("expectedText");

            AssertionHelper.Verify(delegate
            {
                if (actualValue != null && actualValue.EndsWith(expectedText))
                    return null;

                return new AssertionFailureBuilder("Expected string to end with the specified text.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawActualValue(actualValue)
                    .AddRawLabeledValue("Expected Text", expectedText)
                    .ToAssertionFailure();
            });
        }

        #endregion
    }
}