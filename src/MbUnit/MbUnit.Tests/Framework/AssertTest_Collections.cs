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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework.Assertions;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(Assert))]
	public class AssertTest_Collections : BaseAssertTest
	{
        #region AreElementsEqual
        [Test]
        public void AreElementsEqual_with_strings()
        {
            Assert.AreElementsEqual("12", "12");
        }

        [Test]
        public void AreElementsEqual_with_different_types()
        {
            Assert.AreElementsEqual(new[] { 1, 2 }, new List<int> { 1, 2 });
        }

        [Test]
        public void AreElementsEqual_with_custom_comparer()
        {
            Assert.AreElementsEqual("12", "34", (expected, actual) => expected + 2 == actual);

        }

        [Test]
        public void AreElementsEqual_fails_when_elements_are_in_different_order()
        {
            AssertionFailure[] failures = Capture(()
                => Assert.AreElementsEqual(new[] { 1, 2 }, new List<int> { 2, 1 }));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected elements to be equal but they differ in at least one position.", failures[0].Description);
            Assert.AreEqual("Expected Sequence", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("[1, 2]", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Actual Sequence", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("[2, 1]", failures[0].LabeledValues[1].FormattedValue.ToString());
            Assert.AreEqual("Element Index", failures[0].LabeledValues[2].Label);
            Assert.AreEqual("0", failures[0].LabeledValues[2].FormattedValue.ToString());
            Assert.AreEqual("Expected Element", failures[0].LabeledValues[3].Label);
            Assert.AreEqual("1", failures[0].LabeledValues[3].FormattedValue.ToString());
            Assert.AreEqual("Actual Element", failures[0].LabeledValues[4].Label);
            Assert.AreEqual("2", failures[0].LabeledValues[4].FormattedValue.ToString());
        }

        [Test]
        public void AreElementsEqual_fails_with_custom_message()
        {
            AssertionFailure[] failures = Capture(() => Assert.AreElementsEqual("1", "2", "{0} message", "custom"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("custom message", failures[0].Message);
        }

        #endregion

        #region AreElementsNotEqual
        [Test]
        public void AreElementsNotEqual_with_strings()
        {
            Assert.AreElementsNotEqual("12", "1");
        }

        [Test]
        public void AreElementsNotEqual_with_different_types()
        {
            Assert.AreElementsNotEqual(new[] { 1, 2 }, new List<int> { 1, 3 });
        }

        [Test]
        public void AreElementsNotEqual_with_custom_comparer()
        {
            Assert.AreElementsNotEqual("12", "12", (expected, actual) => expected + 2 == actual);
        }

        [Test]
        public void AreElementsNotEqual_fails_when_elements_are_in_different_order()
        {
            AssertionFailure[] failures = Capture(()
                => Assert.AreElementsNotEqual(new[] { 1, 2 }, new List<int> { 1, 2 }));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the unexpected and actual sequence to have different elements but all elements were equal.", failures[0].Description);
            Assert.AreEqual("Unexpected Sequence", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("[1, 2]", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Actual Sequence", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("[1, 2]", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void AreElementsNotEqual_fails_with_custom_message()
        {
            AssertionFailure[] failures = Capture(() => Assert.AreElementsNotEqual("2", "2", "{0} message", "custom"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("custom message", failures[0].Message);
        }

        #endregion

        #region AreElementsEqualIgnoringOrder
        [Test]
        public void AreElementsEqualIgnoringOrder_with_strings()
        {
            Assert.AreElementsEqualIgnoringOrder("122", "212");
        }

        [Test]
        public void AreElementsEqualIgnoringOrder_with_different_types()
        {
            Assert.AreElementsEqualIgnoringOrder(new[] { 2, 2, 3, 1 }, new List<int> { 1, 3, 2, 2 });
        }

        [Test]
        public void AreElementsEqualIgnoringOrder_with_custom_comparer()
        {
            Assert.AreElementsEqualIgnoringOrder("12", "43", (expected, actual) => expected + 2 == actual);
        }

        [Test]
        public void AreElementsEqualIgnoringOrder_fails_when_excess_or_missing_elements()
        {
            AssertionFailure[] failures = Capture(()
                => Assert.AreElementsEqualIgnoringOrder(new[] { 1, 2, 3, 2, 3, 1 }, new List<int> { 4, 2, 1, 1, 4, 1, 4 }));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected elements to be equal but possibly in a different order.", failures[0].Description);
            Assert.AreEqual("Equal Elements", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("[1, 1, 2]", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Excess Elements", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("[1, 4, 4, 4]", failures[0].LabeledValues[1].FormattedValue.ToString());
            Assert.AreEqual("Missing Elements", failures[0].LabeledValues[2].Label);
            Assert.AreEqual("[2, 3, 3]", failures[0].LabeledValues[2].FormattedValue.ToString());
        }

        [Test]
        public void AreElementsEqualIgnoringOrder_fails_with_custom_message()
        {
            AssertionFailure[] failures = Capture(() => Assert.AreElementsEqualIgnoringOrder("1", "2", "{0} message", "custom"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("custom message", failures[0].Message);
        }

        #endregion

        #region AreElementsSame

        [Test]
        public void AreElementsSame_with_objects()
        {
            var o = new object();
            Assert.AreElementsSame(new[] { o }, new[] { o });
        }

        [Test]
        public void AreElementsSame_with_different_types()
        {
            var o1 = new object();
            var o2 = new object();
            Assert.AreElementsSame(new[] { o1, o2 }, new List<object> { o1, o2 });
        }

        [Test]
        public void AreElementsSame_fails_when_elements_are_in_different_order()
        {
            var o1 = new object();
            var o2 = new object();
            AssertionFailure[] failures = Capture(()
                => Assert.AreElementsSame(new[] { o1, o2 }, new List<object> { o2, o1 }));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected elements to be referentially equal but they differ in at least one position.", failures[0].Description);
        }

        [Test]
        public void AreElementsSame_fails_with_custom_message()
        {
            var o1 = new object();
            var o2 = new object();
            AssertionFailure[] failures = Capture(() => Assert.AreElementsSame(new[] { o1 }, new[] { o2 }, "{0} message", "custom"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("custom message", failures[0].Message);
        }

        #endregion

        #region AreElementsNotSame
        [Test]
        public void AreElementsNotSame_with_objects()
        {
            var o1 = new object();
            var o2 = new object();
            Assert.AreElementsNotSame(new[] { o1 }, new[] { o2 });
        }

        [Test]
        public void AreElementsNotSame_with_different_types()
        {
            var o1 = new object();
            var o2 = new object();
            var o3 = new object();
            Assert.AreElementsNotSame(new[] { o1, o2 }, new List<object> { o1, o3 });
        }

        [Test]
        public void AreElementsNotSame_fails_with_custom_message()
        {
            var o = new object();
            AssertionFailure[] failures = Capture(() => Assert.AreElementsNotSame(new[] { o }, new[] { o }, "{0} message", "custom"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("custom message", failures[0].Message);
        }

        #endregion

        #region AreElementsSameIgnoringOrder

        [Test]
        public void AreElementsSameIgnoringOrder_with_different_types()
        {
            var o1 = new object();
            var o2 = new object();
            var o3 = new object();
            Assert.AreElementsSameIgnoringOrder(new[] { o2, o2, o3, o1 }, new List<object> { o1, o3, o2, o2 });
        }

        [Test]
        public void AreElementsSameIgnoringOrder_fails_when_excess_or_missing_elements()
        {
            var o1 = new object();
            var o2 = new object();
            var o3 = new object();
            var o4 = new object();
            AssertionFailure[] failures = Capture(()
                => Assert.AreElementsSameIgnoringOrder(new[] { o1, o2, o3, o2, o3, o1 }, new List<object> { o4, o2, o1, o1, o4, o1, o4 }));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected elements to be referentially equal but possibly in a different order.", failures[0].Description);
        }

        [Test]
        public void AreElementsSameIgnoringOrder_fails_with_custom_message()
        {
            var o1 = new object();
            var o2 = new object();
            AssertionFailure[] failures = Capture(() => Assert.AreElementsSameIgnoringOrder(new[] { o1 }, new[] { o2 }, "{0} message", "custom"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("custom message", failures[0].Message);
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
            Assert.AreEqual("Expected Key", failures[0].LabeledValues[0].Label);
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
            Assert.AreEqual("Expected Key", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("[5]", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Dictionary", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("[[1, 2]: \"1\", [3, 4]: \"2`\"]",
                failures[0].LabeledValues[1].FormattedValue.ToString());
            Assert.AreEqual("custom message.", failures[0].Message);
        }
        #endregion

        #region ForAll

        [Test]
        public void ForAll_should_pass()
        {
            var data = new[] { "Athos", "Porthos", "Aramis" };
            Assert.ForAll(data, x => x.Contains("a") || x.Contains("o"));
        }

        [Test]
        public void ForAll_should_fail()
        {
            var data = new[] { "Athos", "Porthos", "Aramis" };
            AssertionFailure[] failures = Capture(() => Assert.ForAll(data, x => x.StartsWith("A")));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected all the elements of the sequence to meet the specified condition, but at least one failed.", failures[0].Description);
        }

        #endregion

        #region Exists

        [Test]
        public void Exists_should_pass()
        {
            var data = new[] { "Athos", "Porthos", "Aramis" };
            Assert.Exists(data, x => x.Contains("th"));
        }

        [Test]
        public void Exists_should_fail()
        {
            var data = new[] { "Athos", "Porthos", "Aramis" };
            AssertionFailure[] failures = Capture(() => Assert.Exists(data, x => x == "D'Artagnan"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected at least one element of the sequence to meet the specified condition, but none passed.", failures[0].Description);
        }

        #endregion
    }
}
