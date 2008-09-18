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

using System.Text.RegularExpressions;
using Gallio.Framework.Assertions;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(Assert))]
    public class StringAssertTest
    {
        #region AreEqualIgnoreCase
        [Test]
        public void AreEqualIgnoreCase_Test_with_non_null_values()
        {
            Assert.AreEqualIgnoreCase("test", "TeSt");
        }

        [Test]
        public void AreEqualIgnoreCase_Test_with_IEqualityComparer()
        {
            Assert.AreEqualIgnoreCase("test", "dummy", new AssertTest.TestComparer { EqualsReturn = true }, "message");
        }

        [Test]
        public void AreEqualIgnoreCase_fails_when_simple_values_different()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.AreEqualIgnoreCase("test", "tEsm"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected values to be equal.", failures[0].Description);
            Assert.AreEqual("Expected Value", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("\"test\"", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Actual Value", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("\"tEsm\"", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void AreEqualIgnoreCase_fails_when_one_of_the_values_is_null()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.AreEqualIgnoreCase("test", null));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("\"test\"", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("null", failures[0].LabeledValues[1].FormattedValue.ToString());
        }


        [Test]
        public void AreEqualIgnoreCase_fails_with_custom_message()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.AreEqualIgnoreCase(null, "test", "{0} message {1}", "MB1", "Mb2"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("null", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("\"test\"", failures[0].LabeledValues[1].FormattedValue.ToString());
            Assert.AreEqual("MB1 message Mb2", failures[0].Message);
        }

        [Test]
        public void AreEqualIgnoreCase_fails_with_custom_Compare()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.AreEqualIgnoreCase("test", "tesT", new AssertTest.TestComparer { EqualsReturn = false }));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("\"test\"", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("\"tesT\"", failures[0].LabeledValues[1].FormattedValue.ToString());
        }
        #endregion

        #region FullMatch

        [Test]
        public void FullMatch_sucessful_tests_with_Regex()
        {
            Assert.FullMatch("mbTest", new Regex(@"[\w]{6}"));
        }

        [Test]
        public void FullMatch_sucessful_with_pattern()
        {
            Assert.FullMatch("mbTest", @"[\w]{6}");
        }

        [Test, ExpectedArgumentNullException]
        public void FullMatch_test_for_ArgumentNullException_when_testValue_is_null()
        {
            Assert.FullMatch(null, new Regex(@"[\w]{6}"));
        }

        [Test, ExpectedArgumentNullException]
        public void FullMatch_test_for_ArgumentNullException_when_regex_is_null()
        {
            const Regex re = null;
            Assert.FullMatch("mbTest", re);
        }

        [Test, ExpectedArgumentNullException]
        public void FullMatch_test_for_ArgumentNullException_when_pattern_is_null()
        {
            const string pattern = null;
            Assert.FullMatch("mbTest", pattern);
        }

        [Test]
        public void FullMatch_fails_when_testValue_does_not_match_regex_pattern()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.FullMatch("mbTest", new Regex(@"[\d]{6}")));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected to have an exact match.", failures[0].Description);
            Assert.AreEqual("Test Value", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("\"mbTest\"", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Regex Pattern", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("\"[\\\\d]{6}\"", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void FullMatch_fails_when_testValue_matches_regex_pattern_but_lenght_is_different()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.FullMatch("mbTest", new Regex(@"[\w]{7}")));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("\"mbTest\"", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("\"[\\\\w]{7}\"", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void FullMatch_fail_test_with_custom_message()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.FullMatch("mbTest", new Regex(@"[\w]{7}"), "{0} message {1}", "MB1", "Mb2"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("MB1 message Mb2", failures[0].Message);
        }

        #endregion

        #region Like

        [Test]
        public void Like_sucessful_tests_with_Regex()
        {
            Assert.Like("mbTest", new Regex(@"[\w]+"));
        }

        [Test]
        public void Like_sucessful_with_pattern()
        {
            Assert.Like("mbTest", @"[\w]*");
        }

        [Test, ExpectedArgumentNullException]
        public void Like_test_for_ArgumentNullException_when_testValue_is_null()
        {
            Assert.Like(null, new Regex(@"[\w]{6}"));
        }

        [Test, ExpectedArgumentNullException]
        public void Like_test_for_ArgumentNullException_when_regex_is_null()
        {
            const Regex re = null;
            Assert.Like("mbTest", re);
        }

        [Test, ExpectedArgumentNullException]
        public void Like_test_for_ArgumentNullException_when_pattern_is_null()
        {
            const string pattern = null;
            Assert.Like("mbTest", pattern);
        }

        [Test]
        public void Like_fails_when_testValue_does_not_match_regex_pattern()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.Like("mbTest", new Regex(@"[\d]+")));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected to match Regex pattern.", failures[0].Description);
            Assert.AreEqual("Test Value", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("\"mbTest\"", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Regex Pattern", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("\"[\\\\d]+\"", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void Like_fail_test_with_custom_message()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.Like("mbTest", new Regex(@"[\w]{7}"), "{0} message {1}", "MB1", "Mb2"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("MB1 message Mb2", failures[0].Message);
        }

        #endregion

        #region NotLike

        [Test]
        public void NotLike_sucessful_tests_with_Regex()
        {
            Assert.NotLike("mbTest", new Regex(@"[\d]+"));
        }

        [Test]
        public void NotLike_sucessful_with_pattern()
        {
            Assert.NotLike("mbTest", @"[\d]{2}");
        }

        [Test, ExpectedArgumentNullException]
        public void NotLike_test_for_ArgumentNullException_when_testValue_is_null()
        {
            Assert.NotLike(null, new Regex(@"[\w]{6}"));
        }

        [Test, ExpectedArgumentNullException]
        public void NotLike_test_for_ArgumentNullException_when_regex_is_null()
        {
            const Regex re = null;
            Assert.NotLike("mbTest", re);
        }

        [Test, ExpectedArgumentNullException]
        public void NotLike_test_for_ArgumentNullException_when_pattern_is_null()
        {
            const string pattern = null;
            Assert.NotLike("mbTest", pattern);
        }

        [Test]
        public void NotLike_fails_when_testValue_does_not_match_regex_pattern()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.NotLike("mbTest", new Regex(@"[\w]+")));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected not to match Regex pattern.", failures[0].Description);
            Assert.AreEqual("Test Value", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("\"mbTest\"", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Regex Pattern", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("\"[\\\\w]+\"", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void NotLike_fail_test_with_custom_message()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.NotLike("mbTest", new Regex(@"[\w]{6}"), "{0} message {1}", "MB1", "Mb2"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("MB1 message Mb2", failures[0].Message);
        }

        #endregion

        #region StartsWith

        [Test]
        public void StartsWith_sucessful_tests()
        {
            Assert.StartsWith("mbTest", "mbT");
        }

        [Test]
        public void StartsWith_sucessful_tests_when_testValue_and_pattern_are_null()
        {
            Assert.StartsWith(null, null);
        }

        [Test, ExpectedArgumentNullException]
        public void StartsWith_test_for_ArgumentNullException_when_testValue_is_null()
        {
            Assert.StartsWith(null, "");
        }

        [Test, ExpectedArgumentNullException]
        public void StartsWith_test_for_ArgumentNullException_when_pattern_is_null()
        {
            Assert.StartsWith("mbTest", null);
        }

        [Test]
        public void StartsWith_fails_when_testValue_does_not_start_with_pattern()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.StartsWith("mbTest", "jb"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected to start with the specified pattern.", failures[0].Description);
            Assert.AreEqual("Test Value", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("\"mbTest\"", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Pattern", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("\"jb\"", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void StartsWith_fail_test_with_custom_message()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.StartsWith("mbTest", "jb", "{0} message {1}", "MB1", "Mb2"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("MB1 message Mb2", failures[0].Message);
        }

        #endregion

        #region EndsWith

        [Test]
        public void EndsWith_sucessful_tests()
        {
            Assert.EndsWith("mbTest", "est");
        }

        [Test]
        public void EndsWith_sucessful_tests_when_testValue_and_pattern_are_null()
        {
            Assert.EndsWith(null, null);
        }

        [Test, ExpectedArgumentNullException]
        public void EndsWith_test_for_ArgumentNullException_when_testValue_is_null()
        {
            Assert.EndsWith(null, "");
        }

        [Test, ExpectedArgumentNullException]
        public void EndsWith_test_for_ArgumentNullException_when_pattern_is_null()
        {
            Assert.EndsWith("mbTest", null);
        }

        [Test]
        public void EndsWith_fails_when_testValue_does_not_start_with_pattern()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.EndsWith("mbTest", "jb"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected to end with the specified pattern.", failures[0].Description);
            Assert.AreEqual("Test Value", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("\"mbTest\"", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Pattern", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("\"jb\"", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void EndsWith_fail_test_with_custom_message()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.EndsWith("mbTest", "jb", "{0} message {1}", "MB1", "Mb2"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("MB1 message Mb2", failures[0].Message);
        }

        #endregion

    }
}