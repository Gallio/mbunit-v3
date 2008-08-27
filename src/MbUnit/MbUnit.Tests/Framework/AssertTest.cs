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
using Gallio.Framework.Assertions;
using Gallio.Model.Diagnostics;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(Assert))]
    public class AssertTest
    {
        #region AreEqual
        #endregion

        #region GreaterThan
        [Test]
        public void GreaterThan_int_test()
        {
            Assert.GreaterThan(5, 4);
        }

        [Test]
        public void GreaterThan_should_fail_when_type_is_not_IComparable()
        {
            AssertionFailure[] failures = Capture(() => 
                Assert.GreaterThan(new Exception(), new Exception()));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("System.InvalidOperationException: No ordering comparison defined on type System.Exception."
                , failures[0].Exceptions[0].ToString());
        }

        [Test]
        public void GreaterThan_fails_when_left_value_is_not_greater_than_right()
        {
            AssertionFailure[] failures = Capture(() => 
                Assert.GreaterThan(5, 5));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("5", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("5", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void GreaterThan_fail_when_left_value_is_null()
        {
            AssertionFailure[] failures = Capture(() => 
                Assert.GreaterThan(null, "abc"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected left to be greater than right.", failures[0].Description);
            Assert.AreEqual("Left Value", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("null", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Right Value", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("\"abc\"", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void GreaterThan_fail_when_left_value_is_null_with_custom_message()
        {
            AssertionFailure[] failures = Capture(() => Assert.GreaterThan(null, "abc", "custom message."));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("custom message.", failures[0].Message);
        }

        [Test]
        public void GreaterThan_fail_when_left_value_is_null_with_custom_message_and_argument()
        {
            AssertionFailure[] failures = Capture(() => Assert.GreaterThan(null, "abc", "{0} message.", "MbUnit"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("MbUnit message.", failures[0].Message);
        }

        [Test]
        public void GreaterThan_double_test()
        {
            Assert.GreaterThan(0.001, 0.0001);
        }

        [Test]
        [Row("abc", "ab")]
        [Row("abc", null)]
        public void GreaterThan_string_test(string left, string right)
        {
            Assert.GreaterThan(left, right);
        }

        [Test]
        public void GreaterThan_with_delegate_test()
        {
            Assert.GreaterThan(4, 3, (left, right) => left.CompareTo(0) + right.CompareTo(0));
        }

        [Test]
        public void GreaterThan_with_delegate_and_message_test()
        {
            Assert.GreaterThan(0, 1, (left, right) => left.CompareTo(0) + right.CompareTo(0), "custom compare");
        }

        [Test]
        public void GreaterThan_with_message_test()
        {
            Assert.GreaterThan(0, -1, "custom message");
        }

        [Test]
        public void GreaterThan_with_non_generic_IComparable()
        {
            Assert.GreaterThan(new NonGenericCompare(), null);
        }

        #endregion

        #region GreaterThanOrEqual
        [Test]
        [Row(5, 4), Row(5, 5)]
        public void GreaterThanOrEqual_int_test(int left, int right)
        {
            Assert.GreaterThanOrEqual(left, right);
        }

        [Test]
        public void GreaterThanOrEqual_fails_when_left_value_is_not_greater_or_equal_than_right()
        {
            AssertionFailure[] failures = Capture(() => Assert.GreaterThanOrEqual(5, 6));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected left to be greater or equal than right.", failures[0].Description);
        }

        [Test]
        public void GreaterThanOrEqual_fail_when_left_value_is_null()
        {
            AssertionFailure[] failures = Capture(() => Assert.GreaterThanOrEqual(null, "abc"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Left Value", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("Right Value", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("null", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("\"abc\"", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void GreaterThanOrEqual_fail_when_left_value_is_null_with_custom_message()
        {
            AssertionFailure[] failures = Capture(() => Assert.GreaterThanOrEqual(null, "abc", "custom message."));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("custom message.", failures[0].Message);
        }

        [Test]
        public void GreaterThanOrEqual_fail_when_left_value_is_null_with_custom_message_and_argument()
        {
            AssertionFailure[] failures = Capture(() => Assert.GreaterThanOrEqual(null, "abc", "{0} message.", "MbUnit"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("MbUnit message.", failures[0].Message);
        }

        [Test]
        public void GreaterThanOrEqual_double_test()
        {
            Assert.GreaterThanOrEqual(0.001, 0.0001);
        }

        [Test]
        [Row("abc", "ab")]
        [Row("abc", null)]
        public void GreaterThanOrEqual_string_test(string left, string right)
        {
            Assert.GreaterThanOrEqual(left, right);
        }

        [Test]
        public void GreaterThanOrEqual_with_delegate_test()
        {
            Assert.GreaterThanOrEqual(4, 4, (left, right) => left.CompareTo(0) + right.CompareTo(0));
        }

        [Test]
        public void GreaterThanOrEqual_with_delegate_and_message_test()
        {
            Assert.GreaterThanOrEqual(0, 0, (left, right) => left.CompareTo(0) + right.CompareTo(0), "custom compare");
        }

        [Test]
        public void GreaterThanOrEqual_with_both_values_null()
        {
            const string s1 = null;
            const string s2 = null;
            Assert.GreaterThanOrEqual(s1, s2);
        }

        [Test]
        public void GreaterThanOrEqual_with_message()
        {
            Assert.GreaterThanOrEqual("two", "one", "custom message");
        }

        #endregion

        #region LessThan
        [Test]
        public void LessThan_int_test()
        {
            Assert.LessThan(3, 4);
        }

        [Test]
        public void LessThan_fails_when_left_value_is_not_less_than_right()
        {
            AssertionFailure[] failures = Capture(() => Assert.LessThan("mb", "mb"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected left to be less than right.", failures[0].Description);
        }

        [Test]
        public void LessThan_on_failure_test_with_values_only()
        {
            AssertionFailure[] failures = Capture(() => Assert.LessThan("abc", null));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Left Value", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("Right Value", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("\"abc\"", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("null", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void LessThan_fail_when_left_value_is_null_with_custom_message()
        {
            AssertionFailure[] failures = Capture(() => Assert.LessThan(5, 1.1, "custom message."));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("custom message.", failures[0].Message);
        }

        [Test]
        public void LessThan_when_left_value_is_null_test()
        {
            Assert.LessThan(null, "abc");
        }

        [Test]
        public void LessThan_check_custom_message_and_argument_on_failure()
        {
            AssertionFailure[] failures = Capture(() => Assert.LessThan("d", "abc", "{0} message.", "MbUnit"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("MbUnit message.", failures[0].Message);
        }

        [Test]
        public void LessThan_double_test()
        {
            Assert.LessThan(0.0001, 0.001);
        }

        [Test]
        [Row("ab", "abc")]
        public void LessThan_string_test(string left, string right)
        {
            Assert.LessThan(left, right);
        }

        [Test]
        public void LessThan_with_delegate_test()
        {
            Assert.LessThan(3, 4, (left, right) => left - right);
        }

        [Test]
        public void LessThan_with_message_test()
        {
            Assert.LessThan(null, "six", "custom message");
        }

        [Test]
        public void LessThan_with_delegate_and_message_test()
        {
            Assert.LessThan(-5, 3, (left, right) => left - right, "custom message");
        }
        #endregion

        #region LessThanOrEqual

        [Test]
        [Row(3, 4), Row(-2, -2)]
        public void LessThanOrEqual_int_test(int left, int right)
        {
            Assert.LessThanOrEqual(left, right);
        }

        [Test]
        public void LessThanOrEqual_fails_when_left_value_is_not_less_or_equal_than_right()
        {
            AssertionFailure[] failures = Capture(() => Assert.LessThanOrEqual("ms", "mb"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected left to be less or equal than right.", failures[0].Description);
        }

        [Test]
        public void LessThanOrEqual_fail_when_left_value_is_null()
        {
            AssertionFailure[] failures = Capture(() => Assert.LessThanOrEqual("abc", null));
            Assert.AreEqual(1, failures.Length);
        }

        [Test]
        public void LessThanOrEqual_fail_when_left_value_is_null_with_custom_message()
        {
            AssertionFailure[] failures = Capture(() => Assert.LessThanOrEqual("abc", null, "custom message."));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("custom message.", failures[0].Message);
        }

        [Test]
        public void LessThanOrEqual_fail_when_left_value_is_null_with_custom_message_and_argument()
        {
            AssertionFailure[] failures = Capture(() => Assert.LessThanOrEqual("k", "abc", "{0} message.", "MbUnit"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("MbUnit message.", failures[0].Message);
        }

        [Test]
        public void LessThanOrEqual_double_test()
        {
            Assert.LessThanOrEqual(0.0001, 0.001);
        }

        [Test]
        [Row("ab", "abc")]
        public void LessThanOrEqual_string_test(string left, string right)
        {
            Assert.LessThanOrEqual(left, right);
        }

        [Test]
        public void LessThanOrEqual_with_delegate_test()
        {
            Assert.LessThanOrEqual(3, 4, (left, right) => left - right);
        }

        [Test]
        public void LessThanOrEqual_with_delegate_and_message()
        {
            Assert.LessThanOrEqual(-6, 6, "custom message");
        }

        [Test]
        public void LessThanOrEqual_with_delegate_and_message_test()
        {
            Assert.LessThanOrEqual(3, 4, (left, right) => left - right, "custom message");
        }
        #endregion

        #region Between
        [Test]
        [Row(2, 1, 3)]
        [Row(-1, -1, 3)]
        [Row(3, 1, 3)]
        public void Between_int_test(int test, int left, int right)
        {
            Assert.Between(test, left, right);
        }

        [Test]
        public void Between_nullable_int_test()
        {
            int? test = null;
            int? left = null;
            Assert.Between(test, left, 6);
        }


        [Test]
        public void Between_fails_when_test_value_is_left_of_the_range()
        {
            AssertionFailure[] failures = Capture(() => Assert.Between(0, 1, 3));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("The actual value should be between the minimum and maximum values.", failures[0].Description);
            Assert.AreEqual("Actual Value", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("0", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Minimum Value", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("1", failures[0].LabeledValues[1].FormattedValue.ToString());
            Assert.AreEqual("Maximum Value", failures[0].LabeledValues[2].Label);
            Assert.AreEqual("3", failures[0].LabeledValues[2].FormattedValue.ToString());
        }
        #endregion

        #region NotBetween
        [Test]
        [Row(-2, 1, 3)]
        [Row(5, -1, 3)]
        public void NotBetween_int_test(int test, int left, int right)
        {
            Assert.NotBetween(test, left, right);
        }

        [Test]
        public void NotBetween_fails_when_test_value_is_left_of_the_range()
        {
            AssertionFailure[] failures = Capture(() => Assert.NotBetween(1, 1, 3));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("The actual value should not be between the minimum and maximum values.", failures[0].Description);
            Assert.AreEqual("Actual Value", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("1", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Minimum Value", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("1", failures[0].LabeledValues[1].FormattedValue.ToString());
            Assert.AreEqual("Maximum Value", failures[0].LabeledValues[2].Label);
            Assert.AreEqual("3", failures[0].LabeledValues[2].FormattedValue.ToString());
        }
        #endregion

        #region Fail

        [Test]
        public void Fail_without_parameters()
        {
            AssertionFailure[] failures = Capture(Assert.Fail);
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("An assertion failed.", failures[0].Description);
            Assert.AreEqual("", failures[0].Message);
        }

        [Test]
        public void Fail_with_message_and_arguments()
        {
            AssertionFailure[] failures = Capture(() => Assert.Fail("{0} {1}.", "MbUnit", "message"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("An assertion failed.", failures[0].Description);
            Assert.AreEqual("MbUnit message.", failures[0].Message);
        }

        #endregion

        #region Contains (for collections)

        [Test]
        public void Contains_generic_ICollection_int_test()
        {
            Assert.Contains(new List<int>(new[] { 1, 2, 3 }), 2);
        }

        [Test]
        [Row("2", new[] { "1", "2", "3" })]
        [Row(null, new[] { "1", "2", null, "3" })]
        public void Contains_list_string_test(string testValue, string[] list)
        {
            Assert.Contains(new List<string>(list), testValue);
        }

        [Test]
        [Row(new[] { 1, 2, 3 }, "[1, 2, 3]")]
        [Row(new[] { 1, 2 }, "[1, 2]")]
        [Row(new[] { 1, 2, 3, 5 }, "[1, 2, 3, 5]")]
        public void Contains_fails_when_test_value_is_not_in_the_list(int[] listItems, string expectedCollection)
        {
            AssertionFailure[] failures = Capture(() =>
                Assert.Contains(new List<int>(listItems), 4));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the value to appear within the enumeration.", failures[0].Description);
            Assert.AreEqual("Expected Value", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("4", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Enumeration", failures[0].LabeledValues[1].Label);
            Assert.AreEqual(expectedCollection, failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        [Row("test", new[] { "1", "2", "3" }, "\"test\"", "[\"1\", \"2\", \"3\"]")]
        [Row(null, new[] { "1", "2", "3" }, "null", "[\"1\", \"2\", \"3\"]")]
        public void Contains_fails_when_test_value_is_not_in_the_string_list(string testValue, string[] listItems, string expectedLabledValue, string expectedCollection)
        {
            AssertionFailure[] failures = Capture(() =>
                Assert.Contains(new List<string>(listItems), testValue));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the value to appear within the enumeration.", failures[0].Description);
            Assert.AreEqual("Expected Value", failures[0].LabeledValues[0].Label);
            Assert.AreEqual(expectedLabledValue, failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Enumeration", failures[0].LabeledValues[1].Label);
            Assert.AreEqual(expectedCollection, failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void ContainsKey_dictionary_int_test()
        {
            Assert.ContainsKey(new Dictionary<int, int>
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
                Assert.ContainsKey(dictionary, 0));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the key to appear within the dictionary.", failures[0].Description);
            Assert.AreEqual("Key", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("0", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Dictionary", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("[1: \"1\", 2: \"2`\"]", failures[0].LabeledValues[1].FormattedValue.ToString());
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
                Assert.ContainsKey(dictionary, new List<int>(new[] { 5 }), "{0} message.", "custom"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the key to appear within the dictionary.", failures[0].Description);
            Assert.AreEqual("Key", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("[5]", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Dictionary", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("[[1, 2]: \"1\", [3, 4]: \"2`\"]",
                failures[0].LabeledValues[1].FormattedValue.ToString());
            Assert.AreEqual("custom message.", failures[0].Message);
        }
        #endregion

        #region TypeAssert
        #region IsAssignableFrom

        [Test]
        public void IsAssignableFrom_without_custom_message()
        {
            Assert.IsAssignableFrom(typeof(FormatException), new SystemException());
        }

        [Test]
        [ExpectedArgumentNullException]
        public void IsAssignableFrom_with_null_expectedType()
        {
            Assert.IsAssignableFrom(null, new SystemException());
        }

        [Test]
        [ExpectedArgumentNullException]
        public void IsAssignableFrom_with_null_actualValue()
        {
            Assert.IsAssignableFrom(typeof(int), null);
        }

        [Test]
        public void IsAssignableFrom_fails_when_object_is_not_assignable_for_classes()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsAssignableFrom(typeof(string), new Int32()));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the actual type to be assignable to the expected type.", failures[0].Description);
            Assert.AreEqual("Actual Type", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("int", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Expected Type", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("string", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void IsAssignableFrom_fails_when_object_is_not_assignable_for_arrays()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsAssignableFrom(typeof(int[,]), new int[2]));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the actual type to be assignable to the expected type.", failures[0].Description);
            Assert.AreEqual("Actual Type", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("int[]", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Expected Type", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("int[,]", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void IsAssignableFrom_fails_when_object_is_not_assignable_for_generics()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsAssignableFrom(typeof(List<int>), new List<Type>()));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("System.Collections.Generic.List<System.Type>", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("System.Collections.Generic.List<int>", failures[0].LabeledValues[1].FormattedValue.ToString());
        }
        #endregion

        #region IsNotAssignableFrom

        [Test]
        public void IsNotAssignableFrom_without_custom_message()
        {
            Assert.IsNotAssignableFrom(typeof(string), new Int32());
        }

        [Test]
        [ExpectedArgumentNullException]
        public void IsNotAssignableFrom_with_null_expectedType()
        {
            Assert.IsNotAssignableFrom(null, new SystemException());
        }

        [Test]
        [ExpectedArgumentNullException]
        public void IsNotAssignableFrom_with_null_actualValue()
        {
            Assert.IsNotAssignableFrom(typeof(int), null);
        }

        [Test]
        public void IsNotAssignableFrom_fails_when_object_is_not_assignable_for_classes()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsNotAssignableFrom(typeof(int), new Int32()));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the actual type not to be assignable to the expected type.", failures[0].Description);
            Assert.AreEqual("Actual Type", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("int", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Expected Type", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("int", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void IsNotAssignableFrom_fails_when_object_is_not_assignable_for_arrays()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsNotAssignableFrom(typeof(int[,]), new int[2, 2]));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the actual type not to be assignable to the expected type.", failures[0].Description);
            Assert.AreEqual("Actual Type", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("int[,]", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Expected Type", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("int[,]", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        #endregion
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