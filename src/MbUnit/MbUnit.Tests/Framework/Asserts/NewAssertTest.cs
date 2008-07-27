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
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(NewAssert))]
    public class NewAssertTest
    {
        #region GreaterThan
        [Test]
        public void GreaterThan_int_test()
        {
            NewAssert.GreaterThan(5, 4);
        }

        [Test]
        public void GreaterThan_should_fail_when_type_is_not_IComparable()
        {
            AssertionFailure[] failures = AssertHelper.Eval(() => 
                NewAssert.GreaterThan(new Exception(), new Exception()));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("System.InvalidOperationException: No ordering comparison defined on type System.Exception.\r\n"
                , failures[0].Exceptions[0]);
        }

        [Test]
        public void GreaterThan_fails_when_left_value_is_not_greater_than_right()
        {
            AssertionFailure[] failures = AssertHelper.Eval(() => 
                NewAssert.GreaterThan(5, 5));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("5", failures[0].LabeledValues[0].Value);
            NewAssert.AreEqual("5", failures[0].LabeledValues[1].Value);
        }

        [Test]
        public void GreaterThan_fail_when_left_value_is_null()
        {
            AssertionFailure[] failures = AssertHelper.Eval(() => 
                NewAssert.GreaterThan(null, "abc"));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("Expected left to be greater than right.", failures[0].Description);
            NewAssert.AreEqual("Left Value", failures[0].LabeledValues[0].Key);
            NewAssert.AreEqual("null", failures[0].LabeledValues[0].Value);
            NewAssert.AreEqual("Right Value", failures[0].LabeledValues[1].Key);
            NewAssert.AreEqual("\"abc\"", failures[0].LabeledValues[1].Value);
        }

        [Test]
        public void GreaterThan_fail_when_left_value_is_null_with_custom_message()
        {
            AssertionFailure[] failures = AssertHelper.Eval(() => NewAssert.GreaterThan(null, "abc", "custom message."));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("custom message.", failures[0].Message);
        }

        [Test]
        public void GreaterThan_fail_when_left_value_is_null_with_custom_message_and_argument()
        {
            AssertionFailure[] failures = AssertHelper.Eval(() => NewAssert.GreaterThan(null, "abc", "{0} message.", "MbUnit"));
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

        #region GreaterOrEqualThan
        [Test]
        [Row(5, 4), Row(5, 5)]
        public void GreaterOrEqualThan_int_test(int left, int right)
        {
            NewAssert.GreaterOrEqualThan(left, right);
        }

        [Test]
        public void GreaterOrEqualThan_fails_when_left_value_is_not_greater_or_equal_than_right()
        {
            AssertionFailure[] failures = AssertHelper.Eval(() => NewAssert.GreaterOrEqualThan(5, 6));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("Expected left to be greater or equal than right.", failures[0].Description);
        }

        [Test]
        public void GreaterOrEqualThan_fail_when_left_value_is_null()
        {
            AssertionFailure[] failures = AssertHelper.Eval(() => NewAssert.GreaterOrEqualThan(null, "abc"));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("Left Value", failures[0].LabeledValues[0].Key);
            NewAssert.AreEqual("Right Value", failures[0].LabeledValues[1].Key);
            NewAssert.AreEqual("null", failures[0].LabeledValues[0].Value);
            NewAssert.AreEqual("\"abc\"", failures[0].LabeledValues[1].Value);
        }

        [Test]
        public void GreaterOrEqualThan_fail_when_left_value_is_null_with_custom_message()
        {
            AssertionFailure[] failures = AssertHelper.Eval(() => NewAssert.GreaterOrEqualThan(null, "abc", "custom message."));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("custom message.", failures[0].Message);
        }

        [Test]
        public void GreaterOrEqualThan_fail_when_left_value_is_null_with_custom_message_and_argument()
        {
            AssertionFailure[] failures = AssertHelper.Eval(() => NewAssert.GreaterOrEqualThan(null, "abc", "{0} message.", "MbUnit"));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("MbUnit message.", failures[0].Message);
        }

        [Test]
        public void GreaterOrEqualThan_double_test()
        {
            NewAssert.GreaterOrEqualThan(0.001, 0.0001);
        }

        [Test]
        [Row("abc", "ab")]
        [Row("abc", null)]
        public void GreaterOrEqualThan_string_test(string left, string right)
        {
            NewAssert.GreaterOrEqualThan(left, right);
        }

        [Test]
        public void GreaterOrEqualThan_with_delegate_test()
        {
            NewAssert.GreaterOrEqualThan(4, 4, (left, right) => left.CompareTo(0) + right.CompareTo(0));
        }

        [Test]
        public void GreaterOrEqualThan_with_delegate_and_message_test()
        {
            NewAssert.GreaterOrEqualThan(0, 0, (left, right) => left.CompareTo(0) + right.CompareTo(0), "custom compare");
        }

        [Test]
        public void GreaterOrEqualThan_with_both_values_null()
        {
            const string s1 = null;
            const string s2 = null;
            NewAssert.GreaterOrEqualThan(s1, s2);
        }

        [Test]
        public void GreaterOrEqualThan_with_message()
        {
            NewAssert.GreaterOrEqualThan("two", "one", "custom message");
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
            AssertionFailure[] failures = AssertHelper.Eval(() => NewAssert.LessThan("mb", "mb"));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("Expected left to be less than right.", failures[0].Description);
        }

        [Test]
        public void LessThan_on_failure_test_with_values_only()
        {
            AssertionFailure[] failures = AssertHelper.Eval(() => NewAssert.LessThan("abc", null));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("Left Value", failures[0].LabeledValues[0].Key);
            NewAssert.AreEqual("Right Value", failures[0].LabeledValues[1].Key);
            NewAssert.AreEqual("\"abc\"", failures[0].LabeledValues[0].Value);
            NewAssert.AreEqual("null", failures[0].LabeledValues[1].Value);
        }

        [Test]
        public void LessThan_fail_when_left_value_is_null_with_custom_message()
        {
            AssertionFailure[] failures = AssertHelper.Eval(() => NewAssert.LessThan(5, 1.1, "custom message."));
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
            AssertionFailure[] failures = AssertHelper.Eval(() => NewAssert.LessThan("d", "abc", "{0} message.", "MbUnit"));
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

        #region LessOrEqualThan

        [Test]
        [Row(3, 4), Row(-2, -2)]
        public void LessOrEqualThan_int_test(int left, int right)
        {
            NewAssert.LessOrEqualThan(left, right);
        }

        [Test]
        public void LessOrEqualThan_fails_when_left_value_is_not_less_or_equal_than_right()
        {
            AssertionFailure[] failures = AssertHelper.Eval(() => NewAssert.LessOrEqualThan("ms", "mb"));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("Expected left to be less or equal than right.", failures[0].Description);
        }

        [Test]
        public void LessOrEqualThan_fail_when_left_value_is_null()
        {
            AssertionFailure[] failures = AssertHelper.Eval(() => NewAssert.LessOrEqualThan("abc", null));
            NewAssert.AreEqual(1, failures.Length);
        }

        [Test]
        public void LowerEqualThan_fail_when_left_value_is_null_with_custom_message()
        {
            AssertionFailure[] failures = AssertHelper.Eval(() => NewAssert.LessOrEqualThan("abc", null, "custom message."));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("custom message.", failures[0].Message);
        }

        [Test]
        public void LowerEqualThan_fail_when_left_value_is_null_with_custom_message_and_argument()
        {
            AssertionFailure[] failures = AssertHelper.Eval(() => NewAssert.LessOrEqualThan("k", "abc", "{0} message.", "MbUnit"));
            NewAssert.AreEqual(1, failures.Length);
            NewAssert.AreEqual("MbUnit message.", failures[0].Message);
        }

        [Test]
        public void LowerEqualThan_double_test()
        {
            NewAssert.LessOrEqualThan(0.0001, 0.001);
        }

        [Test]
        [Row("ab", "abc")]
        public void LowerEqualThan_string_test(string left, string right)
        {
            NewAssert.LessOrEqualThan(left, right);
        }

        [Test]
        public void LowerEqualThan_with_delegate_test()
        {
            NewAssert.LessOrEqualThan(3, 4, (left, right) => left - right);
        }

        [Test]
        public void LowerEqualThan_with_delegate_and_message()
        {
            NewAssert.LessOrEqualThan(-6, 6, "custom message");
        }

        [Test]
        public void LowerEqualThan_with_delegate_and_message_test()
        {
            NewAssert.LessOrEqualThan(3, 4, (left, right) => left - right, "custom message");
        }
        #endregion

        class NonGenericCompare : IComparable
        {
            public int CompareTo(object obj)
            {
                return 1;
            }
        }
    }
}