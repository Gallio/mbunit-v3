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
using Gallio.Framework;
using Gallio.Model.Diagnostics;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(NewAssert))]
    public class NewAssertTest
    {
        #region AreEqual
        #endregion

        #region GreaterThan
        [Test]
        public void GreaterThan_int_test()
        {
            NewAssert.GreaterThan(5, 4);
        }

        [Test]
        public void GreaterThan_should_fail_when_type_is_not_IComparable()
        {
            AssertionFailure[] failures = Capture(() => 
                NewAssert.GreaterThan(new Exception(), new Exception()));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("System.InvalidOperationException: No ordering comparison defined on type System.Exception."
                , failures[0].Exceptions[0].ToString());
        }

        [Test]
        public void GreaterThan_fails_when_left_value_is_not_greater_than_right()
        {
            AssertionFailure[] failures = Capture(() => 
                NewAssert.GreaterThan(5, 5));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("5", failures[0].LabeledValues[0].FormattedValue.ToString());
            NewAssert.AreEqual("5", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void GreaterThan_fail_when_left_value_is_null()
        {
            AssertionFailure[] failures = Capture(() => 
                NewAssert.GreaterThan(null, "abc"));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("Expected left to be greater than right.", failures[0].Description);
            NewAssert.AreEqual("Left Value", failures[0].LabeledValues[0].Label);
            NewAssert.AreEqual("null", failures[0].LabeledValues[0].FormattedValue.ToString());
            NewAssert.AreEqual("Right Value", failures[0].LabeledValues[1].Label);
            NewAssert.AreEqual("\"abc\"", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void GreaterThan_fail_when_left_value_is_null_with_custom_message()
        {
            AssertionFailure[] failures = Capture(() => NewAssert.GreaterThan(null, "abc", "custom message."));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("custom message.", failures[0].Message);
        }

        [Test]
        public void GreaterThan_fail_when_left_value_is_null_with_custom_message_and_argument()
        {
            AssertionFailure[] failures = Capture(() => NewAssert.GreaterThan(null, "abc", "{0} message.", "MbUnit"));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("MbUnit message.", failures[0].Message);
        }

        [Test]
        public void GreaterThan_double_test()
        {
            NewAssert.GreaterThan(0.001, 0.0001);
        }

        [Test]
        [Row("abc", "ab")]
        [Row("abc", null)]
        public void GreaterThan_string_test(string left, string right)
        {
            NewAssert.GreaterThan(left, right);
        }

        [Test]
        public void GreaterThan_with_delegate_test()
        {
            NewAssert.GreaterThan(4, 3, (left, right) => left.CompareTo(0) + right.CompareTo(0));
        }

        [Test]
        public void GreaterThan_with_delegate_and_message_test()
        {
            NewAssert.GreaterThan(0, 1, (left, right) => left.CompareTo(0) + right.CompareTo(0), "custom compare");
        }

        [Test]
        public void GreaterThan_with_message_test()
        {
            NewAssert.GreaterThan(0, -1, "custom message");
        }

        [Test]
        public void GreaterThan_with_non_generic_IComparable()
        {
            NewAssert.GreaterThan(new NonGenericCompare(), null);
        }

        #endregion

        #region GreaterThanOrEqual
        [Test]
        [Row(5, 4), Row(5, 5)]
        public void GreaterThanOrEqual_int_test(int left, int right)
        {
            NewAssert.GreaterThanOrEqual(left, right);
        }

        [Test]
        public void GreaterThanOrEqual_fails_when_left_value_is_not_greater_or_equal_than_right()
        {
            AssertionFailure[] failures = Capture(() => NewAssert.GreaterThanOrEqual(5, 6));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("Expected left to be greater or equal than right.", failures[0].Description);
        }

        [Test]
        public void GreaterThanOrEqual_fail_when_left_value_is_null()
        {
            AssertionFailure[] failures = Capture(() => NewAssert.GreaterThanOrEqual(null, "abc"));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("Left Value", failures[0].LabeledValues[0].Label);
            NewAssert.AreEqual("Right Value", failures[0].LabeledValues[1].Label);
            NewAssert.AreEqual("null", failures[0].LabeledValues[0].FormattedValue.ToString());
            NewAssert.AreEqual("\"abc\"", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void GreaterThanOrEqual_fail_when_left_value_is_null_with_custom_message()
        {
            AssertionFailure[] failures = Capture(() => NewAssert.GreaterThanOrEqual(null, "abc", "custom message."));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("custom message.", failures[0].Message);
        }

        [Test]
        public void GreaterThanOrEqual_fail_when_left_value_is_null_with_custom_message_and_argument()
        {
            AssertionFailure[] failures = Capture(() => NewAssert.GreaterThanOrEqual(null, "abc", "{0} message.", "MbUnit"));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("MbUnit message.", failures[0].Message);
        }

        [Test]
        public void GreaterThanOrEqual_double_test()
        {
            NewAssert.GreaterThanOrEqual(0.001, 0.0001);
        }

        [Test]
        [Row("abc", "ab")]
        [Row("abc", null)]
        public void GreaterThanOrEqual_string_test(string left, string right)
        {
            NewAssert.GreaterThanOrEqual(left, right);
        }

        [Test]
        public void GreaterThanOrEqual_with_delegate_test()
        {
            NewAssert.GreaterThanOrEqual(4, 4, (left, right) => left.CompareTo(0) + right.CompareTo(0));
        }

        [Test]
        public void GreaterThanOrEqual_with_delegate_and_message_test()
        {
            NewAssert.GreaterThanOrEqual(0, 0, (left, right) => left.CompareTo(0) + right.CompareTo(0), "custom compare");
        }

        [Test]
        public void GreaterThanOrEqual_with_both_values_null()
        {
            const string s1 = null;
            const string s2 = null;
            NewAssert.GreaterThanOrEqual(s1, s2);
        }

        [Test]
        public void GreaterThanOrEqual_with_message()
        {
            NewAssert.GreaterThanOrEqual("two", "one", "custom message");
        }

        #endregion

        #region LessThan
        [Test]
        public void LessThan_int_test()
        {
            NewAssert.LessThan(3, 4);
        }

        [Test]
        public void LessThan_fails_when_left_value_is_not_less_than_right()
        {
            AssertionFailure[] failures = Capture(() => NewAssert.LessThan("mb", "mb"));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("Expected left to be less than right.", failures[0].Description);
        }

        [Test]
        public void LessThan_on_failure_test_with_values_only()
        {
            AssertionFailure[] failures = Capture(() => NewAssert.LessThan("abc", null));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("Left Value", failures[0].LabeledValues[0].Label);
            NewAssert.AreEqual("Right Value", failures[0].LabeledValues[1].Label);
            NewAssert.AreEqual("\"abc\"", failures[0].LabeledValues[0].FormattedValue.ToString());
            NewAssert.AreEqual("null", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void LessThan_fail_when_left_value_is_null_with_custom_message()
        {
            AssertionFailure[] failures = Capture(() => NewAssert.LessThan(5, 1.1, "custom message."));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("custom message.", failures[0].Message);
        }

        [Test]
        public void LessThan_when_left_value_is_null_test()
        {
            NewAssert.LessThan(null, "abc");
        }

        [Test]
        public void LessThan_check_custom_message_and_argument_on_failure()
        {
            AssertionFailure[] failures = Capture(() => NewAssert.LessThan("d", "abc", "{0} message.", "MbUnit"));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("MbUnit message.", failures[0].Message);
        }

        [Test]
        public void LessThan_double_test()
        {
            NewAssert.LessThan(0.0001, 0.001);
        }

        [Test]
        [Row("ab", "abc")]
        public void LessThan_string_test(string left, string right)
        {
            NewAssert.LessThan(left, right);
        }

        [Test]
        public void LessThan_with_delegate_test()
        {
            NewAssert.LessThan(3, 4, (left, right) => left - right);
        }

        [Test]
        public void LessThan_with_message_test()
        {
            NewAssert.LessThan(null, "six", "custom message");
        }

        [Test]
        public void LessThan_with_delegate_and_message_test()
        {
            NewAssert.LessThan(-5, 3, (left, right) => left - right, "custom message");
        }
        #endregion

        #region LessThanOrEqual

        [Test]
        [Row(3, 4), Row(-2, -2)]
        public void LessThanOrEqual_int_test(int left, int right)
        {
            NewAssert.LessThanOrEqual(left, right);
        }

        [Test]
        public void LessThanOrEqual_fails_when_left_value_is_not_less_or_equal_than_right()
        {
            AssertionFailure[] failures = Capture(() => NewAssert.LessThanOrEqual("ms", "mb"));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("Expected left to be less or equal than right.", failures[0].Description);
        }

        [Test]
        public void LessThanOrEqual_fail_when_left_value_is_null()
        {
            AssertionFailure[] failures = Capture(() => NewAssert.LessThanOrEqual("abc", null));
            NewAssert.AreEqual(1, failures.Length);
        }

        [Test]
        public void LessThanOrEqual_fail_when_left_value_is_null_with_custom_message()
        {
            AssertionFailure[] failures = Capture(() => NewAssert.LessThanOrEqual("abc", null, "custom message."));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("custom message.", failures[0].Message);
        }

        [Test]
        public void LessThanOrEqual_fail_when_left_value_is_null_with_custom_message_and_argument()
        {
            AssertionFailure[] failures = Capture(() => NewAssert.LessThanOrEqual("k", "abc", "{0} message.", "MbUnit"));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("MbUnit message.", failures[0].Message);
        }

        [Test]
        public void LessThanOrEqual_double_test()
        {
            NewAssert.LessThanOrEqual(0.0001, 0.001);
        }

        [Test]
        [Row("ab", "abc")]
        public void LessThanOrEqual_string_test(string left, string right)
        {
            NewAssert.LessThanOrEqual(left, right);
        }

        [Test]
        public void LessThanOrEqual_with_delegate_test()
        {
            NewAssert.LessThanOrEqual(3, 4, (left, right) => left - right);
        }

        [Test]
        public void LessThanOrEqual_with_delegate_and_message()
        {
            NewAssert.LessThanOrEqual(-6, 6, "custom message");
        }

        [Test]
        public void LessThanOrEqual_with_delegate_and_message_test()
        {
            NewAssert.LessThanOrEqual(3, 4, (left, right) => left - right, "custom message");
        }
        #endregion

        #region Between
        [Test]
        [Row(2, 1, 3)]
        [Row(-1, -1, 3)]
        [Row(3, 1, 3)]
        public void Between_int_test(int test, int left, int right)
        {
            NewAssert.Between(test, left, right);
        }

        [Test]
        public void Between_nullable_int_test()
        {
            int? test = null;
            int? left = null;
            NewAssert.Between(test, left, 6);
        }


        [Test]
        public void Between_fails_when_test_value_is_left_of_the_range()
        {
            AssertionFailure[] failures = Capture(() => NewAssert.Between(0, 1, 3));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("The actual value should be between the minimum and maximum values.", failures[0].Description);
            NewAssert.AreEqual("Actual Value", failures[0].LabeledValues[0].Label);
            NewAssert.AreEqual("0", failures[0].LabeledValues[0].FormattedValue.ToString());
            NewAssert.AreEqual("Minimum Value", failures[0].LabeledValues[1].Label);
            NewAssert.AreEqual("1", failures[0].LabeledValues[1].FormattedValue.ToString());
            NewAssert.AreEqual("Maximum Value", failures[0].LabeledValues[2].Label);
            NewAssert.AreEqual("3", failures[0].LabeledValues[2].FormattedValue.ToString());
        }
        #endregion

        #region NotBetween
        [Test]
        [Row(-2, 1, 3)]
        [Row(5, -1, 3)]
        public void NotBetween_int_test(int test, int left, int right)
        {
            NewAssert.NotBetween(test, left, right);
        }

        [Test]
        public void NotBetween_fails_when_test_value_is_left_of_the_range()
        {
            AssertionFailure[] failures = Capture(() => NewAssert.NotBetween(1, 1, 3));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("The actual value should not be between the minimum and maximum values.", failures[0].Description);
            NewAssert.AreEqual("Actual Value", failures[0].LabeledValues[0].Label);
            NewAssert.AreEqual("1", failures[0].LabeledValues[0].FormattedValue.ToString());
            NewAssert.AreEqual("Minimum Value", failures[0].LabeledValues[1].Label);
            NewAssert.AreEqual("1", failures[0].LabeledValues[1].FormattedValue.ToString());
            NewAssert.AreEqual("Maximum Value", failures[0].LabeledValues[2].Label);
            NewAssert.AreEqual("3", failures[0].LabeledValues[2].FormattedValue.ToString());
        }
        #endregion

        #region Fail

        [Test]
        public void Fail_without_parameters()
        {
            AssertionFailure[] failures = Capture(NewAssert.Fail);
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("An assertion failed.", failures[0].Description);
            NewAssert.AreEqual("", failures[0].Message);
        }

        [Test]
        public void Fail_with_message_and_arguments()
        {
            AssertionFailure[] failures = Capture(() => NewAssert.Fail("{0} {1}.", "MbUnit", "message"));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("An assertion failed.", failures[0].Description);
            NewAssert.AreEqual("MbUnit message.", failures[0].Message);
        }

        #endregion

        #region Contains (for collections)

        [Test]
        public void Contains_generic_ICollection_int_test()
        {
            NewAssert.Contains(new List<int>(new[] { 1, 2, 3 }), 2);
        }

        [Test]
        [Row("2", new[] { "1", "2", "3" })]
        [Row(null, new[] { "1", "2", null, "3" })]
        public void Contains_list_string_test(string testValue, string[] list)
        {
            NewAssert.Contains(new List<string>(list), testValue);
        }

        [Test]
        [Row(new[] { 1, 2, 3 }, "[1, 2, 3]")]
        [Row(new[] { 1, 2 }, "[1, 2]")]
        [Row(new[] { 1, 2, 3, 5 }, "[1, 2, 3, 5]")]
        public void Contains_fails_when_test_value_is_not_in_the_list(int[] listItems, string expectedCollection)
        {
            AssertionFailure[] failures = Capture(() =>
                NewAssert.Contains(new List<int>(listItems), 4));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("Expected the value to appear within the enumeration.", failures[0].Description);
            NewAssert.AreEqual("Expected Value", failures[0].LabeledValues[0].Label);
            NewAssert.AreEqual("4", failures[0].LabeledValues[0].FormattedValue.ToString());
            NewAssert.AreEqual("Enumeration", failures[0].LabeledValues[1].Label);
            NewAssert.AreEqual(expectedCollection, failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        [Row("test", new[] { "1", "2", "3" }, "\"test\"", "[\"1\", \"2\", \"3\"]")]
        [Row(null, new[] { "1", "2", "3" }, "null", "[\"1\", \"2\", \"3\"]")]
        public void Contains_fails_when_test_value_is_not_in_the_string_list(string testValue, string[] listItems, string expectedLabledValue, string expectedCollection)
        {
            AssertionFailure[] failures = Capture(() =>
                NewAssert.Contains(new List<string>(listItems), testValue));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("Expected the value to appear within the enumeration.", failures[0].Description);
            NewAssert.AreEqual("Expected Value", failures[0].LabeledValues[0].Label);
            NewAssert.AreEqual(expectedLabledValue, failures[0].LabeledValues[0].FormattedValue.ToString());
            NewAssert.AreEqual("Enumeration", failures[0].LabeledValues[1].Label);
            NewAssert.AreEqual(expectedCollection, failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void ContainsKey_dictionary_int_test()
        {
            NewAssert.ContainsKey(new Dictionary<int, int>
              {
                  {1, 1},
                  {2, 2}
              }, 2);
        }

        [Test]
        public void ContainsKey_fails_when_test_value_is_not_in_the_dictionary()
        {
            var dictionary = new Dictionary<int, string>
                                 {
                { 1, "1" },
                { 2, "2`" },
            };
            AssertionFailure[] failures = Capture(() =>
                NewAssert.ContainsKey(dictionary, 0));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("Expected the key to appear within the dictionary.", failures[0].Description);
            NewAssert.AreEqual("Key", failures[0].LabeledValues[0].Label);
            NewAssert.AreEqual("0", failures[0].LabeledValues[0].FormattedValue.ToString());
            NewAssert.AreEqual("Dictionary", failures[0].LabeledValues[1].Label);
            NewAssert.AreEqual("[1: \"1\", 2: \"2`\"]", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void Contains_fails_when_test_value_is_not_in_the_dictionary_key_is_reference_type()
        {
            var dictionary = new Dictionary<List<int>, string>
                                 {
                { new List<int>(new[] {1, 2}), "1" },
                { new List<int>(new[] {3, 4}), "2`" },
            };
            AssertionFailure[] failures = Capture(() =>
                NewAssert.ContainsKey(dictionary, new List<int>(new[] { 5 }), "{0} message.", "custom"));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("Expected the key to appear within the dictionary.", failures[0].Description);
            NewAssert.AreEqual("Key", failures[0].LabeledValues[0].Label);
            NewAssert.AreEqual("[5]", failures[0].LabeledValues[0].FormattedValue.ToString());
            NewAssert.AreEqual("Dictionary", failures[0].LabeledValues[1].Label);
            NewAssert.AreEqual("[[1, 2]: \"1\", [3, 4]: \"2`\"]",
                failures[0].LabeledValues[1].FormattedValue.ToString());
            NewAssert.AreEqual("custom message.", failures[0].Message);
        }
        #endregion

        [TestFrameworkInternal]
        private static AssertionFailure[] Capture(Gallio.Action action)
        {
            AssertionFailure[] failures = AssertionHelper.Eval(action, AssertionFailureBehavior.Throw);
            foreach (AssertionFailure failure in failures)
                failure.WriteTo(TestLog.Default);
            return failures;
        }

        private sealed class NonGenericCompare : IComparable
        {
            public int CompareTo(object obj)
            {
                return 1;
            }
        }
    }
}