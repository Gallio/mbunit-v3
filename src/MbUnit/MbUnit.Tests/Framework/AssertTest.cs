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
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    // FIXME: May contain NUnit derived code but is missing proper attribution!
    //        Need to follow-up with the original contributor.
    [TestFixture]
    [TestsOn(typeof(Assert))]
    public class AssertTest
    {
        const string TEST_FORMAT_STRING = "Failed: Param1={0} Param2={1}";
        const string TEST_FORMAT_STRING_PARAM1 = "param1";
        const int TEST_FORMAT_STRING_PARAM2 = 2;
        const string EXPECTED_FORMATTED_MESSAGE = "Failed: Param1=param1 Param2=2";
        const string EXPECTED_FAIL_MESSAGE = "Test Failed";


    #region Messages

        [Test]
        public void FailWithCustomMessage()
        {
            string message = "CustomMessage";
            try
            {
                Assert.Fail("CustomMessage");
            }
            catch (AssertionException ex)
            {
                Assert.AreEqual(message, ex.Message);
            }
        }     

        #endregion

        #region Fail, Ignore, IsNull, IsNotNull, IsTrue, IsFalse

        [Test]
        public void Fail()
        {
            bool bolPass = true;
            try
            {
                Assert.Fail();
            }
            catch (AssertionException)
            {
                bolPass = false;
            }

            if (bolPass == true)
            {
                Assert.IsFalse(true, "Assert fail has failed");
            }

        }

        #region Ignore

        /* FIXME
        [Test, ExpectedException(typeof(IgnoreRunException))]
        public void Ignore()
        {
            Assert.Ignore("This test will be ignored.");
        }

        [Test, ExpectedArgumentNullException]
        public void IgnoreWithNullMessage()
        {
            Assert.Ignore(null);
        }

        [Test]
        public void IgnoreWithFormattedMessage()
        {
            bool asserted = false;
            
            try
            {
                Assert.Ignore(TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (IgnoreRunException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert Ignore(message, args) has failed");
        }
         * */

        #endregion

        #region IsNull

        [Test]
        public void IsNull()
        {
            Assert.IsNull(null);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void IsNullFail()
        {
            Assert.IsNull("non-null string");
        }

        [Test]
        public void IsNullWithMessage()
        {
            Assert.IsNull(null, "IsNull has failed.");
        }

        [Test]
        public void IsNullWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.IsNull(new object(), EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert IsNull(message) has failed");
        }

        [Test]
        public void IsNullWithFormattedMessage()
        {
            Assert.IsNull(null, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void IsNullWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.IsNull(new object(), TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert IsNull(message, args) has failed");
        }
        #endregion

        #region IsNotNull

        [Test]
        public void IsNotNull()
        {
            Assert.IsNotNull(new object());
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void IsNotNullFail()
        {
            Assert.IsNotNull(null);
        }

        [Test]
        public void IsNotNullWithMessage()
        {
            Assert.IsNotNull(new object(), "IsNotNull has failed.");
        }

        [Test]
        public void IsNotNullWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.IsNotNull(null, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert IsNotNull(message) has failed");
        }

        [Test]
        public void IsNotNullWithFormattedMessage()
        {
            Assert.IsNotNull(new object(), TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }
        
        [Test]
        public void IsNotNullWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.IsNotNull(null, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert IsNotNull(message, args) has failed");
        }
        #endregion


        #region IsTrue

        [Test]
        public void IsTrue()
        {
            Assert.IsTrue(true);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void IsTrueFail()
        {
            Assert.IsTrue(false);
        }

        [Test]
        public void IsTrueWithMessage()
        {
            Assert.IsTrue(true, "IsTrue Failed");
        }

        [Test]
        public void IsTrueWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.IsTrue(false, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert IsTrue(message) has failed");
        }

        [Test]
        public void IsTrueWithFormattedMessage()
        {
            Assert.IsTrue(true, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void IsTrueWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.IsTrue(false, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert IsTrue(message, args) has failed");
        }


        #endregion

        #region IsFalse

        [Test]
        public void IsFalse()
        {
            Assert.IsFalse(false);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void IsFalseFail()
        {
            Assert.IsFalse(true);
        }

        [Test]
        public void IsFalseWithMessage()
        {
            Assert.IsFalse(false, "IsFalse has failed.");
        }

        [Test]
        public void IsFalseWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.IsFalse(true, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert IsFalse(message) has failed");
        }

        [Test]
        public void IsFalseWithFormattedMessage()
        {
            Assert.IsFalse(false, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void IsFalseWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.IsFalse(true, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert IsFalse(message, args) has failed");
        }
        #endregion


        #endregion

        #region AreEqual

        [Test]
        public void AreEqualWithMessage()
        {
            Assert.AreEqual("abcd", "abcd", EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void AreEqualWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.AreEqual("abcd", "dcba", EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert AreEqual(message) has failed");
        }

        [Test]
        public void AreEqualWithFormattedMessage()
        {
            Assert.AreEqual("abcd", "abcd", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void AreEqualWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.AreEqual("abcd", "dbca", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert AreEqual(message, args) has failed");
        }

        [Test]
        public void AreEqual_NullArguments()
        {
            Assert.AreEqual(null, null);
        }

        #region AreEqual (String)

        [Test]
        public void AreEqualString()
        {
            Assert.AreEqual("hello", "hello");
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreEqualStringActualNullFail()
        {
            Assert.AreEqual("hello", null);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreEqualStringExpectedNullFail()
        {
            Assert.AreEqual(null, "hello");
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreEqualStringOrdinalCompare()
        {
            Assert.AreEqual("hello", "HELLO");
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreEqualStringFail()
        {
            Assert.AreEqual("hello", "world");
        }

        [Test]
        public void AreEqualEmptyString()
        {
            Assert.AreEqual("", "");
        }

        #endregion

        #region AreEqual (Double)
        // TODO: It'd be great to refactor these tests using Generics (when under 2.0)

        [Test]
        public void AreEqualDoubleZero()
        {
            Assert.AreEqual((double)0.0, (double)0.0);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreEqualDoubleFail()
        {
            Assert.AreEqual((double)0.0, (double)1.0);
        }

        [Test]
        public void AreEqualDoublePositive()
        {
            Assert.AreEqual((double)1.0, (double)1.0);
        }

        [Test]
        public void AreEqualDoubleNegative()
        {
            Assert.AreEqual((double)-1.0, (double)-1.0);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreEqualDoubleNegativeFail()
        {
            Assert.AreEqual((double)-1.0, (double)1.0);
        }

        [Test]
        public void AreEqualDoubleDeltaNegativeInfinity()
        {
            Assert.AreEqual(double.NegativeInfinity, double.NegativeInfinity, (double) 0.0);
        }

        [Test]
        public void AreEqualDoubleDeltaPositiveInfinity()
        {
            Assert.AreEqual(double.PositiveInfinity, double.PositiveInfinity, (double)0.0);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreEqualDoubleDeltaExpectedInfinityFail()
        {
            Assert.AreEqual(double.PositiveInfinity, (double)1.0, (double)0.0);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreEqualDoubleDeltaActualInfinityFail()
        {
            Assert.AreEqual((double)1.0, double.PositiveInfinity, (double)0.0);
        }

        [Test]
        public void AreEqualDoubleDelta()
        {
            Assert.AreEqual((double)0.0, (double)1.0, (double)1.0);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreEqualDoubleDeltaFail()
        {
            Assert.AreEqual((double)0.0, (double)2.0, (double)1.0);
        }

        [Test]
        [ExpectedArgumentException()]
        public void AreEqualDoubleDeltaNegative()
        {
            Assert.AreEqual((double)0.0, (double)0.0, (double)-1.0);
        }

        
        #endregion

        #region AreEqual (Float)

        [Test]
        public void AreEqualFloatZero()
        {
            Assert.AreEqual((float)0.0, (float) 0.0);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreEqualFloatFail()
        {
            Assert.AreEqual((float)0.0, (float)1.0);
        }

        [Test]
        public void AreEqualFloatPositive()
        {
            Assert.AreEqual((float)1.0, (float)1.0);
        }

        [Test]
        public void AreEqualFloatNegative()
        {
            Assert.AreEqual((float)-1.0, (float)-1.0);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreEqualFloatNegativeFail()
        {
            Assert.AreEqual((float)-1.0, (float)1.0);
        }

        [Test]
        public void AreEqualFloatDeltaNegativeInfinity()
        {
            Assert.AreEqual(float.NegativeInfinity, float.NegativeInfinity, (float)1.0);
        }

        [Test]
        public void AreEqualFloatDeltaPositiveInfinity()
        {
            Assert.AreEqual(float.PositiveInfinity, float.PositiveInfinity, (float)1.0);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreEqualFloatDeltaExpectedInfinityFail()
        {
            Assert.AreEqual(float.PositiveInfinity, (float)1.0, (float)1.0);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreEqualFloatDeltaActualInfinityFail()
        {
            Assert.AreEqual((float)1.0, float.PositiveInfinity, (float)1.0);
        }

        [Test]
        public void AreEqualFloatDelta()
        {
            Assert.AreEqual((float)0.0, (float)1.0, (float)1.0);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreEqualFloatDeltaFail()
        {
            Assert.AreEqual((float)0.0, (float)2.0, (float)1.0);
        }

        [Test]
        [ExpectedArgumentException()]
        public void AreEqualFloatDeltaNegative()
        {
            Assert.AreEqual((float)0.0, (float)0.0, (float)-1.0);
        }

        [Test]
        public void AreEqualFloatDeltaWithMessage()
        {
            Assert.AreEqual((float)1.0, (float)1.0, (float)0.0, "Assert AreEqual(message) has failed");
        }

        [Test]
        public void AreEqualFloatDeltaWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.AreEqual((float)0.0, (float)1.0, (float)0.0, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert AreEqual(message) has failed");
        }

        [Test]
        public void AreEqualFloatDeltaWithFormattedMessage()
        {
            float l = 1.0f;
            float r = 1.0f;
            float d = 1.0f;
            Assert.AreEqual(l, r, d, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void AreEqualFloatDeltaWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.AreEqual((float)0.0, (float)1.0, (float)0.0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert AreEqual(message, args) has failed");
        }
        #endregion

        #region AreEqual (Arrays)

        [Test]
        public void AreEqual_EmptyIntArrays()
        {
            Assert.AreEqual(new int[] {}, new int[] {});
        }

        [Test]
        public void AreEqual_EqualIntArrays()
        {
            Assert.AreEqual(new int[] { 1, 2, 3 }, new int[] { 1, 2, 3 });
        }

        [Test]
        public void AreEqual_NotEqualForArrayAndNonArray()
        {
            Assert.AreNotEqual(new int[] { 1, 2, 3 }, 3);
        }

        [Test]
        public void AreEqual_ArrayOfNullValues()
        {
            object[] a = new object[3];
            object[] b = new object[3];
            Assert.AreEqual(a, b);
        }

        [Test]
        public void AreEqual_EqualArrayWithNullElements()
        {
            object[] a = { 1, 2, null };
            object[] b = { 1, 2, null };
            Assert.AreEqual(a, b);
        }

        [Test]
        public void AreEqual_EqualArrayWithObjectElements()
        {
            object objectA = new object();
            object objectB = new object();
            object objectC = new object();

            object[] a = { objectA, objectB, objectC };
            object[] b = { objectA, objectB, objectC };
            Assert.AreEqual(a, b);
        }

        [Test]
        public void AreEqual_EqualStringArrays()
        {
            Assert.AreEqual(new string[] { "1", "2", "3" }, new string[] { "1", "2", "3" });
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreEqual_UnEqualIntArrays()
        {
            Assert.AreEqual(new int[] { 1, 2, 3 }, new int[] { 1, 2, 4 });
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreEqual_UnEqualSizeIntArrays()
        {
            Assert.AreEqual(new int[0], new int[] { 1, 2 });
        }

        #endregion

        #region AreEqual (Decimal)

        private const Decimal TEST_DECIMAL = (Decimal) 1.034;
        private const Decimal TEST_OTHER_DECIMAL = (Decimal) 2.034;
 
        [Test]
        public void AreEqualDecimalZero()
        {
            Assert.AreEqual((decimal)0.0, (decimal)0.0);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreEqualDecimalFail()
        {
            Assert.AreEqual(TEST_DECIMAL, TEST_OTHER_DECIMAL);
        }

        [Test]
        public void AreEqualDecimalPositive()
        {
            Assert.AreEqual(TEST_DECIMAL, TEST_DECIMAL);
        }

        [Test]
        public void AreEqualDecimalNegative()
        {
            Assert.AreEqual(-TEST_DECIMAL, -TEST_DECIMAL);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreEqualDecimalNegativeFail()
        {
            Assert.AreEqual(-TEST_DECIMAL, TEST_DECIMAL);
        }

        [Test]
        public void AreEqualDecimalWithMessage()
        {
            Decimal l = TEST_DECIMAL;
            Decimal r = TEST_DECIMAL;
            Assert.AreEqual(l, r, "Assert AreEqual(message) has failed");
        }

        [Test]
        public void AreEqualDecimalWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.AreEqual(TEST_DECIMAL, TEST_OTHER_DECIMAL, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert AreEqual(message) has failed");
        }

        [Test]
        public void AreEqualDecimalWithFormattedMessage()
        {
            Assert.AreEqual(TEST_DECIMAL, TEST_DECIMAL, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void AreEqualDecimalWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.AreEqual(TEST_DECIMAL, TEST_OTHER_DECIMAL, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert AreEqual(message, args) has failed");
        }
        #endregion

        #region AreEqual (Integer)

        [Test]
        public void AreEqualIntZero()
        {
            Assert.AreEqual(0, 0);
        }
        
        [Test, ExpectedException(typeof(AssertionException))]
        public void AreEqualIntFail()
        {
            Assert.AreEqual(0, 1);
        }

        [Test]
        public void AreEqualIntPositive()
        {
            Assert.AreEqual(1, 1);
        }

        [Test]
        public void AreEqualIntNegative()
        {
            Assert.AreEqual(-1, -1);
        }
        
        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreEqualIntNegativeFail()
        {
            Assert.AreEqual(-1, 1);
        }

        [Test]
        public void AreEqualIntDelta()
        {
            Assert.AreEqual(0, 1, 1);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreEqualIntDeltaFail()
        {
            Assert.AreEqual(0, 2, 1);
        }

        [Test]
        [ExpectedArgumentException()]
        public void AreEqualIntDeltaNegative()
        {
            Assert.AreEqual(0, 0, -1);
        }

        [Test]
        public void AreEqualIntWithMessage()
        {
            Assert.AreEqual(1, 1, "Assert AreEqual(message) has failed");
        }

        [Test]
        public void AreEqualIntWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.AreEqual(0, 1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert AreEqual(message) has failed");
        }

        [Test]
        public void AreEqualIntWithFormattedMessage()
        {
            Assert.AreEqual(1,1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void AreEqualIntWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.AreEqual(0,1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert AreEqual(message, args) has failed");
        }

        #endregion

        #endregion

        #region AreNotEqual

        [Test]
        public void NotEqual()
        {
            Assert.AreNotEqual(5, 3);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void NotEqualFails()
        {
            Assert.AreNotEqual(5, 5);
        }

        [Test]
        public void NullExpectedNotEqualToNonNullActual()
        {
            Assert.AreNotEqual(null, 3);
        }

        [Test]
        public void NonNullExpectedNotEqualToNullActual()
        {
            Assert.AreNotEqual(3, null);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void NullEqualsNull()
        {
            Assert.AreNotEqual(null, null);
        }

        [Test]
        public void AreNotEqual_WithMessage()
        {
            Assert.AreNotEqual("abcd", "dcba", EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void AreNotEqualWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.AreNotEqual("abcd", "abcd", EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert AreNotEqual(message) has failed");
        }

        [Test]
        public void AreNotEqual_WithFormattedMessage()
        {
            Assert.AreNotEqual("abcd", "dcba", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void AreNotEqualWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.AreNotEqual("abcd", "abcd", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert AreNotEqual(message, args) has failed");
        }

        #region AreNotEqual (Array)

        [Test]
        public void AreNotEqualArray()
        {
            Assert.AreNotEqual(new object[] { 1, 2, 3 }, new int[] { 1, 3, 2 });
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualArray_EmptyObjectArraysFail()
        {
            object[] objectA = new object[] {};
            object[] objectB = new object[] { };

            Assert.AreNotEqual(objectA, objectB);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualArray_EmptyValueTypeArraysFail()
        {
            int[] valueArrayA = new int[] { };
            int[] valueArrayB = new int[] { };

            Assert.AreNotEqual(valueArrayA, valueArrayB);
        }

        [Test]
        public void AreNotEqualArray_UnEqualValueTypeArrays()
        {
            int[] valueArrayA = new int[] { 1, 2, 3 };
            int[] valueArrayB = new int[] { 2, 3, 1 };

            Assert.AreNotEqual(valueArrayA, valueArrayB);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualArray_EqualValueTypeArraysFail()
        {
            int[] valueArrayA = new int[] { 1, 2, 3 };
            int[] valueArrayB = new int[] { 1, 2, 3 };

            Assert.AreNotEqual(valueArrayA, valueArrayB);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualArrayFails()
        {
            Assert.AreNotEqual(new object[] { 1, 2, 3 }, new object[] { 1, 2, 3 });
        }

        [Test]
        public void AreNotEqualArrayUnEqualLength()
        {
            Assert.AreNotEqual(new object[] { 1, 2, 3 }, new object[] { 1, 2, 3, 4 });
        }

        [Test]
        public void AreNotEqualArrayUnEqualLengthPaddedWithNulls()
        {
            Assert.AreNotEqual(new object[] { 1, 2, 3 }, new object[] { 1, 2, 3, null, null });
        }

        [Test]
        public void AreNotEqualTwoArraysContainingNull()
        {
            Assert.AreNotEqual(new object[] { 1, 2, null }, new object[] { 1, null, 3 });
        }

        [Test]
        public void AreNotEqualArrayWithMessage()
        {
            Assert.AreNotEqual(new object[] { 1, 2, 3 }, new object[] { 2, 1, 3 }, EXPECTED_FAIL_MESSAGE);
        }
        [Test]
        public void AreNotEqualArrayWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.AreNotEqual(new object[] { 1, 2, 3 }, new object[] { 1, 2, 3 }, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert AreNotEqual(message) has failed");
        }

        [Test]
        public void AreNotEqualArrayWithFormattedMessage()
        {
            Assert.AreNotEqual(new object[] { 1, 2, 3 }, new object[] { 2, 1, 3 }, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void AreNotEqualArrayWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.AreNotEqual(new object[] { 1, 2, 3 }, new object[] { 1, 2, 3 }, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert AreNotEqual(message, args) has failed");
        }

        #endregion

        #region AreNotEqual (String)

        [Test]
        public void AreNotEqualString()
        {
            Assert.AreNotEqual("hello", "world");
        }

        [Test]
        public void AreNotEqualStringActualNullFail()
        {
            Assert.AreNotEqual("hello", null);
        }

        [Test]
        public void AreNotEqualStringExpectedNullFail()
        {
            Assert.AreNotEqual(null, "hello");
        }

        [Test]
        public void AreNotEqualStringOrdinalCompare()
        {
            Assert.AreNotEqual("hello", "HELLO");
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreNotEqualStringFail()
        {
            Assert.AreNotEqual("hello", "hello");
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreNotEqualEmptyString()
        {
            Assert.AreNotEqual("", "");
        }

        #endregion

        #region AreNotEqual (Double)

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreNotEqualDoubleZero()
        {
            Assert.AreNotEqual((double)0.0, (double)0.0);
        }

        [Test]
        public void AreNotEqualDouble()
        {
            Assert.AreNotEqual((double)0.0, (double)1.0);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreNotEqualDoublePositiveFail()
        {
            Assert.AreNotEqual((double)1.0, (double)1.0);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreNotEqualDoubleNegativeFail()
        {
            Assert.AreNotEqual((double)-1.0, (double)-1.0);
        }

        [Test]
        public void AreNotEqualDoubleNegativeAndPositive()
        {
            Assert.AreNotEqual((double)-1.0, (double)1.0);
        }

        [Test]
        public void AreNotEqualDoubleWithMessage()
        {
            Assert.AreNotEqual((double)1.0, (double)2.0, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualDoubleWithMessageFail()
        {
            Assert.AreNotEqual((double) 1.0, (double) 1.0, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void AreNotEqualDoubleWithFormattedMessage()
        {
            Assert.AreNotEqual((double)1.0, (double)2.0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualDoubleWithFormattedMessageFail()
        {
            Assert.AreNotEqual((double)1.0, (double)1.0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        #endregion

        #region AreNotEqual (Float)

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualFloatZero()
        {
            Assert.AreNotEqual((float)0.0, (float)0.0);
        }

        [Test]
        public void AreNotEqualFloat()
        {
            Assert.AreNotEqual((float)0.0, (float)1.0);
        }

        [Test]
        public void AreNotEqualFloatPositive()
        {
            Assert.AreNotEqual((float)1.0, (float)2.0);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualFloatPositiveFail()
        {
            Assert.AreNotEqual((float)1.0, (float)1.0);
        }

        [Test]
        public void AreNotEqualFloatNegative()
        {
            Assert.AreNotEqual((float)-1.0, (float)-2.0);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualFloatNegativeFail()
        {
            Assert.AreNotEqual((float)-1.0, (float)-1.0);
        }

        [Test]
        public void AreNotEqualFloatNegativeAndPositive()
        {
            Assert.AreNotEqual((float)-1.0, (float)1.0);
        }


        [Test]
        public void AreNotEqualFloatWithMessage()
        {
            Assert.AreNotEqual((float)1.0, (float)3.0, "Assert AreNotEqual(message) has failed");
        }

        [Test]
        public void AreNotEqualFloatWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.AreNotEqual((float)1.0, (float)1.0, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert AreNotEqual(message) has failed");
        }

        [Test]
        public void AreNotEqualFloatWithFormattedMessage()
        {
            float l = 1.0f;
            float r = 3.0f;
            Assert.AreNotEqual(l, r, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void AreNotEqualFloatWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.AreNotEqual((float)1.0, (float)1.0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert AreNotEqual(message, args) has failed");
        }
        #endregion

        #region AreEqual (Decimal)

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualDecimalZero()
        {
            Assert.AreNotEqual((decimal)0.0, (decimal)0.0);
        }

        [Test]
        public void AreNotEqualDecimal()
        {
            Assert.AreNotEqual(TEST_DECIMAL, TEST_OTHER_DECIMAL);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualDecimalPositiveFail()
        {
            Assert.AreNotEqual(TEST_DECIMAL, TEST_DECIMAL);
        }

        [Test]
        public void AreNotEqualDecimalNegative()
        {
            Assert.AreNotEqual(-TEST_DECIMAL, -TEST_OTHER_DECIMAL);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualDecimalNegativeFail()
        {
            Assert.AreNotEqual(-TEST_DECIMAL, -TEST_DECIMAL);
        }

        [Test]
        public void AreNotEqualDecimalNegativePositive()
        {
            Assert.AreNotEqual(-TEST_DECIMAL, TEST_DECIMAL);
        }

        [Test]
        public void AreNotEqualDecimalWithMessage()
        {
            Assert.AreNotEqual(TEST_DECIMAL, TEST_OTHER_DECIMAL, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualDecimalWithMessageFail()
        {
            Assert.AreNotEqual(TEST_DECIMAL, TEST_DECIMAL, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void AreNotEqualDecimalWithFormattedMessage()
        {
            Assert.AreNotEqual(TEST_DECIMAL, TEST_OTHER_DECIMAL, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualDecimalWithFormattedMessageFail()
        {
            Assert.AreNotEqual(TEST_DECIMAL, TEST_DECIMAL, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        #region AreNotEqual (Integer)

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualIntZero()
        {
            Assert.AreNotEqual(0, 0);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualIntFail()
        {
            Assert.AreNotEqual(1, 1);
        }

        [Test]
        public void AreNotEqualIntPositive()
        {
            Assert.AreNotEqual(1, 2);
        }

        [Test]
        public void AreNotEqualIntNegative()
        {
            Assert.AreNotEqual(-1, -2);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualIntNegativeFail()
        {
            Assert.AreNotEqual(-1, -1);
        }

        [Test]
        public void AreNotEqualIntWithMessage()
        {
            Assert.AreNotEqual(1, 2, "Assert AreEqual(message) has failed");
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualIntWithMessageFail()
        {
            Assert.AreNotEqual(1, 1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void AreNotEqualIntWithFormattedMessage()
        {
            Assert.AreNotEqual(1, 2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualIntWithFormattedMessageFail()
        {
            Assert.AreNotEqual(1, 1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        #endregion

        #region AreNotEqual (Unsigned Integer)

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualUIntZero()
        {
            uint ua = 0;
            uint ub = 0;

            Assert.AreNotEqual(ua, ub);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualUIntFail()
        {
            uint ua = 1;
            uint ub = 1;

            Assert.AreNotEqual(ua, ub);
        }

        [Test]
        public void AreNotEqualUIntPositive()
        {
            uint ua = 1;
            uint ub = 2;

            Assert.AreNotEqual(ua, ub);
        }

        [Test]
        public void AreNotEqualUIntWithMessage()
        {
            uint ua = 1;
            uint ub = 2;

            Assert.AreNotEqual(ua, ub, "Assert AreEqual(message) has failed");
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualUIntWithMessageFail()
        {
            uint ua = 1;
            uint ub = 1;

            Assert.AreNotEqual(ua, ub, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void AreNotEqualUIntWithFormattedMessage()
        {
            uint ua = 1;
            uint ub = 2;

            Assert.AreNotEqual(ua, ub, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualUIntWithFormattedMessageFail()
        {
            uint ua = 1;
            uint ub = 1;

            Assert.AreNotEqual(ua, ub, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        #endregion

        #endregion

        #endregion

        #region Between

        #region int
        
        // TODO: Swap the arguments (fails!)
        [Test]
        [Row(1, 0, 2, Description = "Valid Case")]
        [Row(3, 0, 2, Description = "Fail, Above Upper Bound", ExpectedException = typeof(AssertionException))]
        [Row(0, 0, 2, Description = "Valid Case, Equal To Lower Bound")]
        [Row(2, 0, 2, Description = "Valid Case, Equal To Upper Bound")]
        [Row(1, 2, 0, Description = "Valid Case, Left Greater than Right")]
        [Row(3, 2, 0, Description = "Fail, Above Upper Bound, Left Greater than Right", ExpectedException = typeof(AssertionException))]
        [Row(0, 2, 0, Description = "Valid Case, Equal To Lower Bound, Left Greater than Right")]
        [Row(2, 2, 0, Description = "Valid Case, Equal To Upper Bound, Left Greater than Right")]
        [Row(-3, 0, 2, Description = "Fail, Negative Below Lower Bound", ExpectedException = typeof(AssertionException))]
        [Row(-1, -2, 0, Description = "Valid, Left Negative")]
        [Row(-1, -2, 1, Description = "Valid, Left Negative, Right Positive")]
        [Row(-2, -3, -1, Description = "Valid, Negative, Below Negative Bounds")]
        [Row(-2, -1, -3, Description = "Valid, Negative, Below Negative Bounds, Left Greater than Right")]
        public void BetweenIntParamTest(int test, int left, int right)
        {
            Assert.Between(test, left, right);
        }

        [Test]
        public void BetweenIntWithMessage()
        {
            Assert.Between(1, 0, 2, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void BetweenIntWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.Between(3, 0, 2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert Between(message) has failed");
        }

        [Test]
        public void BetweenIntWithFormattedMessage()
        {
            Assert.Between(1, 0, 2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void BetweenIntWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.Between(3, 0, 2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert Between[int](message, args) has failed");
        }

        #endregion

        #region short

        [Test]
        [Row(1, 0, 2, Description = "Valid Case")]
        [Row(3, 0, 2, Description = "Fail, Above Upper Bound", ExpectedException = typeof(AssertionException))]
        [Row(0, 0, 2, Description = "Valid Case, Equal To Lower Bound")]
        [Row(2, 0, 2, Description = "Valid Case, Equal To Upper Bound")]
        [Row(1, 2, 0, Description = "Valid Case, Left Greater than Right")]
        [Row(3, 2, 0, Description = "Fail, Above Upper Bound, Left Greater than Right", ExpectedException = typeof(AssertionException))]
        [Row(0, 2, 0, Description = "Valid Case, Equal To Lower Bound, Left Greater than Right")]
        [Row(2, 2, 0, Description = "Valid Case, Equal To Upper Bound, Left Greater than Right")]
        [Row(-3, 0, 2, Description = "Fail, Negative Below Lower Bound", ExpectedException = typeof(AssertionException))]
        [Row(-1, -2, 0, Description = "Valid, Left Negative")]
        [Row(-1, -2, 1, Description = "Valid, Left Negative, Right Positive")]
        [Row(-2, -3, -1, Description = "Valid, Negative, Below Negative Bounds")]
        [Row(-2, -1, -3, Description = "Valid, Negative, Below Negative Bounds, Left Greater than Right")]
        public void BetweenShortParamTest(short test, short left, short right)
        {
            Assert.Between(test, left, right);
        }

        [Test]
        public void BetweenShortWithMessage()
        {
            Assert.Between((short)1, (short)0, (short)2, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void BetweenShortWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.Between((short)2, (short)0, (short)1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert Between[short](message) has failed");
        }

        [Test]
        public void BetweenShortWithFormattedMessage()
        {
            Assert.Between((short)1, (short)0, (short)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void BetweenShortWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.Between((short)2, (short)0, (short)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert Between[short](message, args) has failed");
        }

        #endregion

        #region byte

        [Test]
        [Row(1, 0, 2, Description = "Valid Case")]
        [Row(3, 0, 2, Description = "Fail, Above Upper Bound", ExpectedException = typeof(AssertionException))]
        [Row(0, 0, 2, Description = "Valid Case, Equal To Lower Bound")]
        [Row(2, 0, 2, Description = "Valid Case, Equal To Upper Bound")]
        [Row(1, 2, 0, Description = "Valid Case, Left Greater than Right")]
        [Row(3, 2, 0, Description = "Fail, Above Upper Bound, Left Greater than Right", ExpectedException = typeof(AssertionException))]
        [Row(0, 2, 0, Description = "Valid Case, Equal To Lower Bound, Left Greater than Right")]
        [Row(2, 2, 0, Description = "Valid Case, Equal To Upper Bound, Left Greater than Right")]
        public void BetweenByteParamTest(byte test, byte left, byte right)
        {
            Assert.Between(test, left, right);
        }

        [Test]
        public void BetweenByteWithMessage()
        {
            Assert.Between((byte)1, (byte)0, (byte)2, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void BetweenByteWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.Between((byte)3, (byte)0, (byte) 2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert Between[byte](message) has failed");
        }

        [Test]
        public void BetweenByteWithFormattedMessage()
        {
            Assert.Between((byte)1, (byte)0, (byte)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void BetweenByteWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.Between((byte)3, (byte)0, (byte) 2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert Between[byte](message, args) has failed");
        }

        #endregion

        #region long

        [Test]
        [Row(1, 0, 2, Description = "Valid Case")]
        [Row(3, 0, 2, Description = "Fail, Above Upper Bound", ExpectedException = typeof(AssertionException))]
        [Row(0, 0, 2, Description = "Valid Case, Equal To Lower Bound")]
        [Row(2, 0, 2, Description = "Valid Case, Equal To Upper Bound")]
        [Row(1, 2, 0, Description = "Valid Case, Left Greater than Right")]
        [Row(3, 2, 0, Description = "Fail, Above Upper Bound, Left Greater than Right", ExpectedException = typeof(AssertionException))]
        [Row(0, 2, 0, Description = "Valid Case, Equal To Lower Bound, Left Greater than Right")]
        [Row(2, 2, 0, Description = "Valid Case, Equal To Upper Bound, Left Greater than Right")]
        [Row(-3, 0, 2, Description = "Fail, Negative Below Lower Bound", ExpectedException = typeof(AssertionException))]
        [Row(-1, -2, 0, Description = "Valid, Left Negative")]
        [Row(-1, -2, 1, Description = "Valid, Left Negative, Right Positive")]
        [Row(-2, -3, -1, Description = "Valid, Negative, Below Negative Bounds")]
        [Row(-2, -1, -3, Description = "Valid, Negative, Below Negative Bounds, Left Greater than Right")]
        public void BetweenLongParamTest(long test, long left, long right)
        {
            Assert.Between(test, left, right);
        }

        [Test]
        public void BetweenLongWithMessage()
        {
            Assert.Between((long)1, (long)0, (long)2, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void BetweenLongWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.Between((long)3, (long)0, (long)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert Between[long](message) has failed");
        }

        [Test]
        public void BetweenLongWithFormattedMessage()
        {
            Assert.Between((long)1, (long)0, (long)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void BetweenLongWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.Between((long)3, (long)0, (long)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert Between[long](message, args) has failed");
        }

#endregion

        #region double

        [Test]
        [Row(1, 0, 2, Description = "Valid Case")]
        [Row(3, 0, 2, Description = "Fail, Above Upper Bound", ExpectedException = typeof(AssertionException))]
        [Row(0, 0, 2, Description = "Valid Case, Equal To Lower Bound")]
        [Row(2, 0, 2, Description = "Valid Case, Equal To Upper Bound")]
        [Row(1, 2, 0, Description = "Valid Case, Left Greater than Right")]
        [Row(3, 2, 0, Description = "Fail, Above Upper Bound, Left Greater than Right", ExpectedException = typeof(AssertionException))]
        [Row(0, 2, 0, Description = "Valid Case, Equal To Lower Bound, Left Greater than Right")]
        [Row(2, 2, 0, Description = "Valid Case, Equal To Upper Bound, Left Greater than Right")]
        [Row(-3, 0, 2, Description = "Fail, Negative Below Lower Bound", ExpectedException = typeof(AssertionException))]
        [Row(-1, -2, 0, Description = "Valid, Left Negative")]
        [Row(-1, -2, 1, Description = "Valid, Left Negative, Right Positive")]
        [Row(-2, -3, -1, Description = "Valid, Negative, Below Negative Bounds")]
        [Row(-2, -1, -3, Description = "Valid, Negative, Below Negative Bounds, Left Greater than Right")]
        public void BetweenDoubleParamTest(double test, double left, double right)
        {
            Assert.Between(test, left, right);
        }

        [Test]
        public void BetweenDoubleWithMessage()
        {
            Assert.Between((double)1, (double)0, (double)2, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void BetweenDoubleWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.Between((double)3,(double)0,(double)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert Between[double](message) has failed");
        }

        [Test]
        public void BetweenDoubleWithFormattedMessage()
        {
            Assert.Between((double)1, (double)0, (double)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void BetweenDoubleWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.Between((double)3,(double)0,(double)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert Between[double](message, args) has failed");
        }

        #endregion

        #region float

        [Test]
        [Row(1, 0, 2, Description = "Valid Case")]
        [Row(3, 0, 2, Description = "Fail, Above Upper Bound", ExpectedException = typeof(AssertionException))]
        [Row(0, 0, 2, Description = "Valid Case, Equal To Lower Bound")]
        [Row(2, 0, 2, Description = "Valid Case, Equal To Upper Bound")]
        [Row(1, 2, 0, Description = "Valid Case, Left Greater than Right")]
        [Row(3, 2, 0, Description = "Fail, Above Upper Bound, Left Greater than Right", ExpectedException = typeof(AssertionException))]
        [Row(0, 2, 0, Description = "Valid Case, Equal To Lower Bound, Left Greater than Right")]
        [Row(2, 2, 0, Description = "Valid Case, Equal To Upper Bound, Left Greater than Right")]
        [Row(-3, 0, 2, Description = "Fail, Negative Below Lower Bound", ExpectedException = typeof(AssertionException))]
        [Row(-1, -2, 0, Description = "Valid, Left Negative")]
        [Row(-1, -2, 1, Description = "Valid, Left Negative, Right Positive")]
        [Row(-2, -3, -1, Description = "Valid, Negative, Below Negative Bounds")]
        [Row(-2, -1, -3, Description = "Valid, Negative, Below Negative Bounds, Left Greater than Right")]
        public void BetweenFloatParamTest(float test, float left, float right)
        {
            Assert.Between(test, left, right);
        }

        [Test]
        public void BetweenFloatWithMessage()
        {
            Assert.Between((float)1, (float)0, (float)2, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void BetweenFloatWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.Between((float)3,(float)0,(float)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert Between[float](message) has failed");
        }

        [Test]
        public void BetweenFloatWithFormattedMessage()
        {
            Assert.Between((float)1,(float)0,(float)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void BetweenFloatWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.Between((float)3,(float)0,(float)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert Between[float](message, args) has failed");
        }

#endregion

        #region IComparable

        [Test]
        [Row("b", "a", "c", Description = "Valid Case")]
        [Row("d", "a", "c", Description = "Fail", ExpectedException = (typeof(AssertionException)))]
        [Row(2, 1, 3)]
        [Row(-1, -2, 0)]
        [Row(-1, -2, 1)]
        [Row(-2, -1, -3)]
        [Row(-1, -3, -1)]
        [Row(null, null, null, ExpectedException = (typeof(AssertionException)))]
        [Row(null, 1, 3, ExpectedException = (typeof(AssertionException)))]
        [Row(2, null, 3, ExpectedException = (typeof(AssertionException)))]
        [Row(2, 1, null, ExpectedException = (typeof(AssertionException)))]
        [Row(4, 1, 3, Description = "Fail", ExpectedException = (typeof(AssertionException)))]
        public void BetweenIComparableParamTest(object test, object left, object right)
        {
            Assert.Between((IComparable) test, (IComparable) left, (IComparable) right);
        }

        [Test]
        public void BetweenIComparableWithMessage()
        {
            Assert.Between((IComparable)"b", (IComparable)"a", (IComparable)"c", EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void BetweenIComparableWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.Between((IComparable)"d", (IComparable)"a", (IComparable)"c", EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert Between[IComparable](message) has failed");
        }

        [Test]
        public void BetweenIComparableWithFormattedMessage()
        {
            Assert.Between((IComparable)"b", (IComparable)"a", (IComparable)"c", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void BetweenIComparableWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.Between((IComparable)"d", (IComparable)"a", (IComparable)"c", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert Between[IComparable](message, args) has failed");
        }

#endregion

        #endregion

        #region NotBetween

        #region int

        [Test]
        [Row(4, 2, 3, Description = "Above Upper Bound")]
        [Row(2, 1, 3, Description = "Fail", ExpectedException = (typeof(AssertionException)))]
        [Row(2, 3, 1, Description = "Fail, Left Greater than Right", ExpectedException = (typeof(AssertionException)))]
        [Row(4, 3, 2, Description = "Above Upper Bound, Left Greater than Right")]
        [Row(1, 2, 3, Description = "Below Lower Bound")]
        [Row(1, 3, 2, Description = "Below Lower Bound, Left Greater that Right")]
        [Row(-1, -2, 0, Description = "Fail, Negative", ExpectedException = (typeof(AssertionException)))]
        [Row(-1, 0, -2, Description = "Fail, Negative, Left Greater that Right", ExpectedException = (typeof(AssertionException)))]
        [Row(-3, -2, -1, Description = "Below Lower Negative Bound")]
        [Row(-3, -1, -2, Description = "Below Lower Negative Bound, Left Greater that Right")]
        public void NotBetweenIntParamTest(int test, int left, int right)
        {
            Assert.NotBetween(test, left, right);
        }

        #endregion

        #region short

        [Test]
        [Row(4, 2, 3, Description = "Above Upper Bound")]
        [Row(2, 1, 3, Description = "Fail", ExpectedException = (typeof(AssertionException)))]
        [Row(2, 3, 1, Description = "Fail, Left Greater than Right", ExpectedException = (typeof(AssertionException)))]
        [Row(4, 3, 2, Description = "Above Upper Bound, Left Greater than Right")]
        [Row(1, 2, 3, Description = "Below Lower Bound")]
        [Row(1, 3, 2, Description = "Below Lower Bound, Left Greater that Right")]
        [Row(-1, -2, 0, Description = "Fail, Negative", ExpectedException = (typeof(AssertionException)))]
        [Row(-1, 0, -2, Description = "Fail, Negative, Left Greater that Right", ExpectedException = (typeof(AssertionException)))]
        [Row(-3, -2, -1, Description = "Below Lower Negative Bound")]
        [Row(-3, -1, -2, Description = "Below Lower Negative Bound, Left Greater that Right")]
        public void NotBetweenIntParamTest(short test, short left, short right)
        {
            Assert.NotBetween(test, left, right);
        }
        #endregion

        #region byte

        [Test]
        [Row(4, 2, 3, Description = "Above Upper Bound")]
        [Row(2, 1, 3, Description = "Fail", ExpectedException = (typeof(AssertionException)))]
        [Row(2, 3, 1, Description = "Fail, Left Greater than Right", ExpectedException = (typeof(AssertionException)))]
        [Row(4, 3, 2, Description = "Above Upper Bound, Left Greater than Right")]
        [Row(1, 2, 3, Description = "Below Lower Bound")]
        [Row(1, 3, 2, Description = "Below Lower Bound, Left Greater that Right")]
        public void NotBetweenByteParamTest(byte test, byte left, byte right)
        {
            Assert.NotBetween(test, left, right);
        }

        #endregion

        #region long
        [Test]
        [Row(4, 2, 3, Description = "Above Upper Bound")]
        [Row(2, 1, 3, Description = "Fail", ExpectedException = (typeof(AssertionException)))]
        [Row(2, 3, 1, Description = "Fail, Left Greater than Right", ExpectedException = (typeof(AssertionException)))]
        [Row(4, 3, 2, Description = "Above Upper Bound, Left Greater than Right")]
        [Row(1, 2, 3, Description = "Below Lower Bound")]
        [Row(1, 3, 2, Description = "Below Lower Bound, Left Greater that Right")]
        [Row(-1, -2, 0, Description = "Fail, Negative", ExpectedException = (typeof(AssertionException)))]
        [Row(-1, 0, -2, Description = "Fail, Negative, Left Greater that Right", ExpectedException = (typeof(AssertionException)))]
        [Row(-3, -2, -1, Description = "Below Lower Negative Bound")]
        [Row(-3, -1, -2, Description = "Below Lower Negative Bound, Left Greater that Right")]
        public void NotBetweenLongParamTest(long test, long left, long right)
        {
            Assert.NotBetween(test, left, right);
        }
        #endregion

        #region double
        
        [Test]
        [Row(4, 2, 3, Description = "Above Upper Bound")]
        [Row(2, 1, 3, Description = "Fail", ExpectedException = (typeof(AssertionException)))]
        [Row(2, 3, 1, Description = "Fail, Left Greater than Right", ExpectedException = (typeof(AssertionException)))]
        [Row(4, 3, 2, Description = "Above Upper Bound, Left Greater than Right")]
        [Row(1, 2, 3, Description = "Below Lower Bound")]
        [Row(1, 3, 2, Description = "Below Lower Bound, Left Greater that Right")]
        [Row(-1, -2, 0, Description = "Fail, Negative", ExpectedException = (typeof(AssertionException)))]
        [Row(-1, 0, -2, Description = "Fail, Negative, Left Greater that Right", ExpectedException = (typeof(AssertionException)))]
        [Row(-3, -2, -1, Description = "Below Lower Negative Bound")]
        [Row(-3, -1, -2, Description = "Below Lower Negative Bound, Left Greater that Right")]
        public void NotBetweenDoubleParamTest(double test, double left, double right)
        {
            Assert.NotBetween(test, left, right);
        }

        #endregion

        #region float
        
        [Test]
        [Row(4, 2, 3, Description = "Above Upper Bound")]
        [Row(2, 1, 3, Description = "Fail", ExpectedException = (typeof(AssertionException)))]
        [Row(2, 3, 1, Description = "Fail, Left Greater than Right", ExpectedException = (typeof(AssertionException)))]
        [Row(4, 3, 2, Description = "Above Upper Bound, Left Greater than Right")]
        [Row(1, 2, 3, Description = "Below Lower Bound")]
        [Row(1, 3, 2, Description = "Below Lower Bound, Left Greater that Right")]
        [Row(-1, -2, 0, Description = "Fail, Negative", ExpectedException = (typeof(AssertionException)))]
        [Row(-1, 0, -2, Description = "Fail, Negative, Left Greater that Right", ExpectedException = (typeof(AssertionException)))]
        [Row(-3, -2, -1, Description = "Below Lower Negative Bound")]
        [Row(-3, -1, -2, Description = "Below Lower Negative Bound, Left Greater that Right")]
        public void NotBetweenFloatParamTest(float test, float left, float right)
        {
            Assert.NotBetween(test, left, right);
        }

        #endregion

        #region IComparable

        [Test]
        [Row("d", "a", "c", Description = "Valid Case")]
        [Row("a", "b", "c", Description = "Valid Case")]
        [Row("b", "a", "c", Description = "Fail", ExpectedException = (typeof(AssertionException)))]
        [Row(4, 1, 3)]
        [Row(1, 2, 3)]
        [Row(null, null, null, ExpectedException = (typeof(AssertionException)))]
        [Row(null, 1, 3, ExpectedException = (typeof(AssertionException)))]
        [Row(2, null, 3, ExpectedException = (typeof(AssertionException)))]
        [Row(2, 1, null, ExpectedException = (typeof(AssertionException)))]
        [Row(2, 1, 3, Description = "Fail", ExpectedException = (typeof(AssertionException)))]
        public void NotBetweenIComparableParamTest(object test, object left, object right)
        {
            Assert.NotBetween((IComparable) test, (IComparable)left, (IComparable)right);
        }
        #endregion

        #endregion

        #region <, <=, >, >=
        #region LowerThan

        #region int

        [Test]
        [Row(0,1, Description = "Valid Case")]
        [Row(2,1, Description = "Invalid Case", ExpectedException = typeof (AssertionException))]
        public void LowerThanIntParamTest(int left, int right)
        {
            Assert.LowerThan(left, right);
        }

        [Test]
        public void LowerThanIntWithMessage()
        {
            Assert.LowerThan(0,1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LowerThanIntWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.LowerThan(2,1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert LowerThan[int](message) has failed");
        }

        [Test]
        public void LowerThanIntWithFormattedMessage()
        {
            Assert.LowerThan(0,1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LowerThanIntWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.LowerThan(2,1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert LowerThan[int](message, args) has failed");
        }
 
        #endregion

        #region short

        [Test]
        [Row((short)0, (short)1, Description = "Valid Case")]
        [Row((short)2, (short)1, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void LowerThanShortParamTest(short left, short right)
        {
            Assert.LowerThan(left, right);
        }

        [Test]
        public void LowerThanShortWithMessage()
        {
            Assert.LowerThan((short)0, (short)1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LowerThanShortWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.LowerThan((short)2, (short)1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert LowerThan[short](message) has failed");
        }

        [Test]
        public void LowerThanShortWithFormattedMessage()
        {
            Assert.LowerThan((short)0, (short)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LowerThanShortWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.LowerThan((short)2, (short) 1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert LowerThan[short](message, args) has failed");
        }


        #endregion

        #region byte

        [Test]
        [Row((byte) 0, (byte) 1, Description = "Valid Case")]
        [Row((byte) 2, (byte) 1, Description = "Invalid Case", ExpectedException = typeof (AssertionException))]
        public void LowerThanByteParamTest(byte left, byte right)
        {
            Assert.LowerThan(left, right);
        }

        [Test]
        public void LowerThanByteWithMessage()
        {
            Assert.LowerThan((byte)0, (byte)1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LowerThanByteWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.LowerThan((byte) 2, (byte) 1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert LowerThan[byte](message) has failed");
        }

        [Test]
        public void LowerThanByteWithFormattedMessage()
        {
            Assert.LowerThan((byte)0, (byte)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LowerThanByteWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.LowerThan((byte) 2, (byte) 1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert LowerThan[byte](message, args) has failed");
        }

        #endregion

        #region double

        [Test]
        [Row((double) 0, (double) 1, Description = "Valid Case")]
        [Row((double) 3, (double) 1, Description = "Invalid Case", ExpectedException = typeof (AssertionException))]
        public void LowerThanDoubleParamTest(double left, double right)
        {
            Assert.LowerThan(left, right);
        }

        [Test]
        public void LowerThanDoubleWithMessage()
        {
            Assert.LowerThan((double) 0, (double) 1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LowerThanDoubleWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.LowerThan((double) 3, (double) 1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert LowerThan[double](message) has failed");
        }

        [Test]
        public void LowerThanDoubleWithFormattedMessage()
        {
            Assert.LowerThan((double) 0, (double) 1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LowerThanDoubleWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.LowerThan((double) 3, (double) 1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert LowerThan[double](message, args) has failed");
        }


        #endregion

        #region long

        [Test]
        [Row((long)0, (long) 1, Description = "Valid Case")]
        [Row((long)3, (long) 1, Description = "Invalid Case", ExpectedException = typeof (AssertionException))]
        public void LowerThanLongParamTest(long left, long right)
        {
            Assert.LowerThan(left, right);
        }

        [Test]
        public void LowerThanLongWithMessage()
        {
            Assert.LowerThan((long)0, (long) 1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LowerThanLongWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.LowerThan((long)3, (long) 1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert LowerThan[long](message) has failed");
        }

        [Test]
        public void LowerThanLongWithFormattedMessage()
        {
            Assert.LowerThan((long)0, (long) 1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LowerThanLongWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.LowerThan((long)3, (long) 1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert LowerThan[long](message, args) has failed");
        }


        #endregion

        #region float

        [Test]
        [Row((float)0, (float)2, Description = "Valid Case")]
        [Row((float)3, (float)2, Description = "Invalid Case", ExpectedException = typeof (AssertionException))]
        public void LowerThanFloatParamTest(float left, float right)
        {
            Assert.LowerThan(left, right);
        }

        [Test]
        public void LowerThanFloatWithMessage()
        {
            Assert.LowerThan((float)0, (float)2, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LowerThanFloatWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.LowerThan((float)3, (float)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert LowerThan[float](message) has failed");
        }

        [Test]
        public void LowerThanFloatWithFormattedMessage()
        {
            Assert.LowerThan((float)0, (float)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LowerThanFloatWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.LowerThan((float)3, (float)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert LowerThan[float](message, args) has failed");
        }


        #endregion

        #region IComparable

        [Test]
        [Row("aaaa", "zzzz", Description = "Valid Case")]
        [Row(1, 2, Description = "Valid Case")]
        [Row(null, null, ExpectedException = (typeof(AssertionException)))]
        [Row(null, 1, ExpectedException = (typeof(AssertionException)))]
        [Row(2, null, ExpectedException = (typeof(AssertionException)))]
        [Row("zzzz", "aaaa", Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void LowerThanIComparableParamTest(object left, object right)
        {
            Assert.LowerThan((IComparable)left, (IComparable)right);
        }

        [Test]
        public void LowerThanIComparableWithMessage()
        {
            Assert.LowerThan((IComparable)"aaaa", (IComparable)"zzzz", EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LowerThanIComparableWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.LowerThan((IComparable)"zzzz", (IComparable)"aaaa", EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert LowerThan[IComparable](message) has failed");
        }

        [Test]
        public void LowerThanIComparableWithFormattedMessage()
        {
            Assert.LowerThan((IComparable)"aaaa", (IComparable)"zzzz", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LowerThanIComparableWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.LowerThan((IComparable)"zzzz", (IComparable)"aaaa", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert LowerThan[IComparable](message, args) has failed");
        }


        #endregion


        #endregion

        #region Less

        #region int

        [Test]
        [Row(0, 1, Description = "Valid Case")]
        [Row(2, 1, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void LessIntParamTest(int left, int right)
        {
            Assert.Less(left, right);
        }

        [Test]
        public void LessIntWithMessage()
        {
            Assert.Less(0, 1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LessIntWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.Less(2, 1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert Less[int](message) has failed");
        }

        [Test]
        public void LessIntWithFormattedMessage()
        {
            Assert.Less(0, 1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LessIntWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.Less(2, 1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert Less[int](message, args) has failed");
        }

        #endregion

        #region uint

        [Test]
        [Row((uint)0, (uint)1, Description = "Valid Case")]
        [Row((uint)2, (uint)1, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void LessUIntParamTest(uint left, uint right)
        {
            Assert.Less(left, right);
        }

        [Test]
        public void LessUIntWithMessage()
        {
            Assert.Less((uint)0, (uint)1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LessUIntWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.Less((uint)2, (uint)1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert Less[uint](message) has failed");
        }

        [Test]
        public void LessUIntWithFormattedMessage()
        {
            Assert.Less((uint)0, (uint)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LessUIntWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.Less((uint)2, (uint)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert Less[uint](message, args) has failed");
        }


        #endregion

        #region decimal

        [Test]
        [Row(0, 1, Description = "Valid Case")]
        [Row(2, 1, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void LessDecimalParamTest(decimal left, decimal right)
        {
            Assert.Less(left, right);
        }

        [Test]
        public void LessDecimalWithMessage()
        {
            Assert.Less((decimal)0, (decimal)1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LessDecimalWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.Less((decimal)2, (decimal)1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert Less[decimal](message) has failed");
        }

        [Test]
        public void LessDecimalWithFormattedMessage()
        {
            Assert.Less((decimal)0, (decimal)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LessDecimalWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.Less((decimal)2, (decimal)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert Less[decimal](message, args) has failed");
        }

        #endregion

        #region double

        [Test]
        [Row((double)0, (double)1, Description = "Valid Case")]
        [Row((double)3, (double)1, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void LessDoubleParamTest(double left, double right)
        {
            Assert.Less(left, right);
        }

        [Test]
        public void LessDoubleWithMessage()
        {
            Assert.Less((double)0, (double)1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LessDoubleWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.Less((double)3, (double)1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert Less[double](message) has failed");
        }

        [Test]
        public void LessDoubleWithFormattedMessage()
        {
            Assert.Less((double)0, (double)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LessDoubleWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.Less((double)3, (double)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert Less[double](message, args) has failed");
        }


        #endregion

        #region long

        [Test]
        [Row((long)0, (long)1, Description = "Valid Case")]
        [Row((long)3, (long)1, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void LessLongParamTest(long left, long right)
        {
            Assert.Less(left, right);
        }

        [Test]
        public void LessLongWithMessage()
        {
            Assert.Less((long)0, (long)1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LessLongWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.Less((long)3, (long)1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert Less[long](message) has failed");
        }

        [Test]
        public void LessLongWithFormattedMessage()
        {
            Assert.Less((long)0, (long)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LessLongWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.Less((long)3, (long)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert Less[long](message, args) has failed");
        }


        #endregion

        #region float

        [Test]
        [Row((float)0, (float)2, Description = "Valid Case")]
        [Row((float)3, (float)2, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void LessFloatParamTest(float left, float right)
        {
            Assert.Less(left, right);
        }

        [Test]
        public void LessFloatWithMessage()
        {
            Assert.Less((float)0, (float)2, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LessFloatWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.Less((float)3, (float)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert Less[float](message) has failed");
        }

        [Test]
        public void LessFloatWithFormattedMessage()
        {
            Assert.Less((float)0, (float)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LessFloatWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.Less((float)3, (float)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert Less[float](message, args) has failed");
        }


        #endregion

        #region IComparable

        [Test]
        [Row("aaaa","zzzz", Description = "Valid Case")]
        [Row(1, 2, Description = "Valid Case")]
        [Row("zzzz", "aaaa", Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void LessIComparableParamTest(object left, object right)
        {
            Assert.Less((IComparable)left, (IComparable)right);
        }

        [Test]
        public void LessIComparableWithMessage()
        {
            Assert.Less((IComparable)"aaaa", (IComparable)"zzzz", EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LessIComparableWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.Less((IComparable)"zzzz", (IComparable)"aaaa", EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert Less[IComparable](message) has failed");
        }

        [Test]
        public void LessIComparableWithFormattedMessage()
        {
            Assert.Less((IComparable)"aaaa", (IComparable)"zzzz", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LessIComparableWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.Less((IComparable)"zzzz", (IComparable)"aaaa", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert Less[IComparable](message, args) has failed");
        }


        #endregion

        #endregion

        #region LowerEqualThan

        #region int

        [Test]
        [Row((int) 0, (int) 1, Description = "Valid Case (Lower Than)")]
        [Row((int)0, (int)0, Description = "Valid Case (Zero Equal)")]
        [Row((int)1, (int)1, Description = "Valid Case (Positive Equal)")]
        [Row((int)-1, (int)-1, Description = "Valid Case (Negative Equal)")]
        [Row((int)2, (int)1, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void LowerEqualThanIntParamTest(int left, int right)
        {
            Assert.LowerEqualThan(left, right);
        }

        [Test]
        public void LowerEqualThanIntWithMessage()
        {
            Assert.LowerEqualThan((int) 0, (int) 1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LowerEqualThanIntWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.LowerEqualThan((int) 2, (int) 1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert LowerEqualThan[int](message) has failed");
        }

        [Test]
        public void LowerEqualThanIntWithFormattedMessage()
        {
            Assert.LowerEqualThan((int) 0, (int) 1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LowerEqualThanIntWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.LowerEqualThan((int) 2, (int) 1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert LowerEqualThan[int](message, args) has failed");
        }


        #endregion

        #region short

        [Test]
        [Row(0, 1, Description = "Valid Case (Lower Than)")]
        [Row(0, 0, Description = "Valid Case (Zero Equal)")]
        [Row(1, 1, Description = "Valid Case (Positive Equal)")]
        [Row(-1, -1, Description = "Valid Case (Negative Equal)")]
        [Row(2, 1, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void LowerEqualThanShortParamTest(short left, short right)
        {
            Assert.LowerEqualThan(left, right);
        }

        [Test]
        public void LowerEqualThanShortWithMessage()
        {
            Assert.LowerEqualThan((short) 0, (short) 1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LowerEqualThanShortWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.LowerEqualThan((short) 2, (short) 1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert LowerEqualThan[short](message) has failed");
        }

        [Test]
        public void LowerEqualThanShortWithFormattedMessage()
        {
            Assert.LowerEqualThan((short) 0, (short) 1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LowerEqualThanShortWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.LowerEqualThan((short) 2, (short) 1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert LowerEqualThan[short](message, args) has failed");
        }

        #endregion

        #region byte

        [Test]
        [Row(0, 1, Description = "Valid Case (Lower Than)")]
        [Row(0, 0, Description = "Valid Case (Zero Equal)")]
        [Row(1, 1, Description = "Valid Case (Positive Equal)")]
        [Row(2, 1, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void LowerEqualThanByteParamTest(byte left, byte right)
        {
            Assert.LowerEqualThan(left, right);
        }

        [Test]
        public void LowerEqualThanByteWithMessage()
        {
            Assert.LowerEqualThan((byte)0,(byte)1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LowerEqualThanByteWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.LowerEqualThan((byte)2,(byte)1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert LowerEqualThan[byte](message) has failed");
        }

        [Test]
        public void LowerEqualThanByteWithFormattedMessage()
        {
            Assert.LowerEqualThan((byte)0,(byte)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LowerEqualThanByteWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.LowerEqualThan((byte)2,(byte)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert LowerEqualThan[byte](message, args) has failed");
        }

        #endregion

        #region long

        [Test]
        [Row(0, 1, Description = "Valid Case (Lower Than)")]
        [Row(0, 0, Description = "Valid Case (Zero Equal)")]
        [Row(1, 1, Description = "Valid Case (Positive Equal)")]
        [Row(-1, -1, Description = "Valid Case (Negative Equal)")]
        [Row(2, 1, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void LowerEqualThanLongParamTest(long left, long right)
        {
            Assert.LowerEqualThan(left, right);
        }

        [Test]
        public void LowerEqualThanLongWithMessage()
        {
            Assert.LowerEqualThan((long) 0, (long)1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LowerEqualThanLongWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.LowerEqualThan((long) 2, (long)1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert LowerEqualThan[long](message) has failed");
        }

        [Test]
        public void LowerEqualThanLongWithFormattedMessage()
        {
            Assert.LowerEqualThan((long) 0, (long)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LowerEqualThanLongWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.LowerEqualThan((long) 2, (long)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert LowerEqualThan[long](message, args) has failed");
        }


        #endregion

        #region double

        [Test]
        [Row(0, 1, Description = "Valid Case (Lower Than)")]
        [Row(0, 0, Description = "Valid Case (Zero Equal)")]
        [Row(1, 1, Description = "Valid Case (Positive Equal)")]
        [Row(-1, -1, Description = "Valid Case (Negative Equal)")]
        [Row(2, 1, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void LowerEqualThanDoubleParamTest(double left, double right)
        {
            Assert.LowerEqualThan(left, right);
        }

        [Test]
        public void LowerEqualThanDoubleWithMessage()
        {
            Assert.LowerEqualThan((double)0,(double)1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LowerEqualThanDoubleWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.LowerEqualThan((double)2,(double)1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert LowerEqualThan[double](message) has failed");
        }

        [Test]
        public void LowerEqualThanDoubleWithFormattedMessage()
        {
            Assert.LowerEqualThan((double)0,(double)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LowerEqualThanDoubleWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.LowerEqualThan((double)2,(double)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert LowerEqualThan[double](message, args) has failed");
        }


        #endregion

        #region float

        [Test]
        [Row(0, 1, Description = "Valid Case (Lower Than)")]
        [Row(0, 0, Description = "Valid Case (Zero Equal)")]
        [Row(1, 1, Description = "Valid Case (Positive Equal)")]
        [Row(-1, -1, Description = "Valid Case (Negative Equal)")]
        [Row(2, 1, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void LowerEqualThanFloatParamTest(float left, float right)
        {
            Assert.LowerEqualThan(left, right);
        }

        [Test]
        public void LowerEqualThanFloatWithMessage()
        {
            Assert.LowerEqualThan((float)0, (float)1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LowerEqualThanFloatWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.LowerEqualThan((float)2, (float)1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert LowerEqualThan[float](message) has failed");
        }

        [Test]
        public void LowerEqualThanFloatWithFormattedMessage()
        {
            Assert.LowerEqualThan((float)0, (float)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LowerEqualThanFloatWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.LowerEqualThan((float)2, (float)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert LowerEqualThan[float](message, args) has failed");
        }


        #endregion

        #region IComparable

        [Test]
        [Row("aaaa", "zzzz", Description = "Valid Case")]
        [Row("aaaa", "aaaa", Description = "Valid Case (Equals)")]
        [Row(1, 2, Description = "Valid Case")]
        [Row(1, 1, Description = "Valid Case (Equals)")]
        [Row(null, null, ExpectedException = (typeof(AssertionException)))]
        [Row(null, 1, ExpectedException = (typeof(AssertionException)))]
        [Row(2, null, ExpectedException = (typeof(AssertionException)))]
        [Row("zzzz", "aaaa", Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void LowerEqualThanIComparableParamTest(object left, object right)
        {
            Assert.LowerEqualThan((IComparable)left, (IComparable)right);
        }

        [Test]
        public void LowerEqualThanIComparableWithMessage()
        {
            Assert.LowerEqualThan((IComparable)"aaaa", (IComparable)"zzzz", EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LowerEqualThanIComparableWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.LowerEqualThan((IComparable)"zzzz", (IComparable)"aaaa", EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert LowerEqualThan[IComparable](message) has failed");
        }

        [Test]
        public void LowerEqualThanIComparableWithFormattedMessage()
        {
            Assert.LowerEqualThan((IComparable)"aaaa", (IComparable)"zzzz", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LowerEqualThanIComparableWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.LowerEqualThan((IComparable)"zzzz", (IComparable)"aaaa", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert LowerEqualThan[IComparable](message, args) has failed");
        }


        #endregion

        #endregion

        #region GreaterThan

        #region int

        [Test]
        [Row((int)1, (int)0, Description = "Valid Case (Greater Than)")]
        [Row((int)1, (int)2, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void GreaterThanIntParamTest(int left, int right)
        {
            Assert.GreaterThan(left, right);
        }

        [Test]
        public void GreaterThanIntWithMessage()
        {
            Assert.GreaterThan((int)1, (int)0, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void GreaterThanIntWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.GreaterThan((int)1, (int)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert GreaterThan[int](message) has failed");
        }

        [Test]
        public void GreaterThanIntWithFormattedMessage()
        {
            Assert.GreaterThan((int)1, (int)0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void GreaterThanIntWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.GreaterThan((int)1, (int)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert GreaterThan[int](message, args) has failed");
        }


        #endregion

        #region short

        [Test]
        [Row(1, 0, Description = "Valid Case (Greater Than)")]
        [Row(1, 2, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void GreaterThanShortParamTest(short left, short right)
        {
            Assert.GreaterThan(left, right);
        }

        [Test]
        public void GreaterThanShortWithMessage()
        {
            Assert.GreaterThan((short)1, (short)0, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void GreaterThanShortWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.GreaterThan((short)1, (short)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert GreaterThan[short](message) has failed");
        }

        [Test]
        public void GreaterThanShortWithFormattedMessage()
        {
            Assert.GreaterThan((short)1, (short)0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void GreaterThanShortWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.GreaterThan((short)1, (short)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert GreaterThan[short](message, args) has failed");
        }

        #endregion

        #region byte

        [Test]
        [Row(1, 0, Description = "Valid Case (Greater Than)")]
        [Row(1, 2, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void GreaterThanByteParamTest(byte left, byte right)
        {
            Assert.GreaterThan(left, right);
        }

        [Test]
        public void GreaterThanByteWithMessage()
        {
            Assert.GreaterThan((byte)1, (byte)0, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void GreaterThanByteWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.GreaterThan((byte)1, (byte)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert GreaterThan[byte](message) has failed");
        }

        [Test]
        public void GreaterThanByteWithFormattedMessage()
        {
            Assert.GreaterThan((byte)1, (byte)0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void GreaterThanByteWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.GreaterThan((byte)1, (byte)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert GreaterThan[byte](message, args) has failed");
        }

        #endregion

        #region long

        [Test]
        [Row(1, 0, Description = "Valid Case (Greater Than)")]
        [Row(1, 2, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void GreaterThanLongParamTest(long left, long right)
        {
            Assert.GreaterThan(left, right);
        }

        [Test]
        public void GreaterThanLongWithMessage()
        {
            Assert.GreaterThan((long)1, (long)0, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void GreaterThanLongWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.GreaterThan((long)1, (long)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert GreaterThan[long](message) has failed");
        }

        [Test]
        public void GreaterThanLongWithFormattedMessage()
        {
            Assert.GreaterThan((long)1, (long)0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void GreaterThanLongWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.GreaterThan((long)1, (long)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert GreaterThan[long](message, args) has failed");
        }


        #endregion

        #region double

        [Test]
        [Row(1, 0, Description = "Valid Case (Greater Than)")]
        [Row(1, 2, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void GreaterThanDoubleParamTest(double left, double right)
        {
            Assert.GreaterThan(left, right);
        }

        [Test]
        public void GreaterThanDoubleWithMessage()
        {
            Assert.GreaterThan((double)1, (double)0, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void GreaterThanDoubleWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.GreaterThan((double)1, (double)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert GreaterThan[double](message) has failed");
        }

        [Test]
        public void GreaterThanDoubleWithFormattedMessage()
        {
            Assert.GreaterThan((double)1, (double)0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void GreaterThanDoubleWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.GreaterThan((double)1, (double)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert GreaterThan[double](message, args) has failed");
        }


        #endregion

        #region float

        [Test]
        [Row(1, 0, Description = "Valid Case (Greater Than)")]
        [Row(1, 2, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void GreaterThanFloatParamTest(float left, float right)
        {
            Assert.GreaterThan(left, right);
        }

        [Test]
        public void GreaterThanFloatWithMessage()
        {
            Assert.GreaterThan((float)1, (float)0, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void GreaterThanFloatWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.GreaterThan((float)1, (float)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert GreaterThan[float](message) has failed");
        }

        [Test]
        public void GreaterThanFloatWithFormattedMessage()
        {
            Assert.GreaterThan((float)1, (float)0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void GreaterThanFloatWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.GreaterThan((float)1, (float)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert GreaterThan[float](message, args) has failed");
        }


        #endregion

        #region IComparable

        [Test]
        [Row("z", "a", Description = "Valid Case")]
        [Row(2, 1, Description = "Valid Case")]
        [Row(null, null, ExpectedException = (typeof(AssertionException)))]
        [Row(null, 1, ExpectedException = (typeof(AssertionException)))]
        [Row(2, null, ExpectedException = (typeof(AssertionException)))]
        [Row("a", "z", Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void GreaterThanIComparableParamTest(object left, object right)
        {
            Assert.GreaterThan((IComparable)left, (IComparable)right);
        }

        [Test]
        public void GreaterThanIComparableWithMessage()
        {
            Assert.GreaterThan((IComparable)"z", (IComparable)"a", EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void GreaterThanIComparableWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.GreaterThan((IComparable)"a", (IComparable)"z", EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert GreaterThan[IComparable](message) has failed");
        }

        [Test]
        public void GreaterThanIComparableWithFormattedMessage()
        {
            Assert.GreaterThan((IComparable)"z", (IComparable)"a", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void GreaterThanIComparableWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.GreaterThan((IComparable)"a", (IComparable)"z", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert GreaterThan[IComparable](message, args) has failed");
        }


        #endregion

        #endregion

        #region Greater

        [Test]
        public void GreaterInt()
        {
            Assert.Greater(1, 0);
        }

        [Test]
        public void GreaterIntWithMessage()
        {
            Assert.Greater(1, 0, "Int is not greater");
        }

        [Test]
        public void GreaterIntWithMessageAndArgs()
        {
            Assert.Greater(1, 0, "{0} is not greater than {1}", 1, 0);
        }

        [Test]
        public void GreaterUint()
        {
            Assert.Greater((uint)1, (uint)0);
        }

        [Test]
        public void GreaterUintWithMessage()
        {
            Assert.Greater((uint)1, (uint)0, "Int is not greater");
        }

        [Test]
        public void GreaterUintWithMessageAndArgs()
        {
            Assert.Greater((uint)1, (uint)0, "{0} is not greater than {1}", 1, 0);
        }

        [Test]
        public void GreaterShort()
        {
            Assert.Greater((short)1, (short)0);
        }

        [Test]
        public void GreaterShortWithMessage()
        {
            Assert.Greater((short)1, (short)0, "Short is not greater");
        }

        [Test]
        public void GreaterShortWithMessageAndArgs()
        {
            Assert.Greater((short)1, (short)0, "{0} is not greater than {1}", (short)1, (short)0);
        }
        
        [Test]
        public void GreaterByte()
        {
            Assert.Greater((byte)1, (byte)0);
        }

        [Test]
        public void GreaterByteWithMessage()
        {
            Assert.Greater((byte)1, (byte)0, "Byte is not greater");
        }

        [Test]
        public void GreaterByteWithMessageAndArgs()
        {
            Assert.Greater((byte)1, (byte)0, "{0} is not greater than {1}", (byte)0, (byte)1);
        }

        [Test]
        public void GreaterDecimal()
        {
            Assert.Greater((decimal)1, (decimal)0);
        }
        
        [Test]
        public void GreaterDecimalWithMessage()
        {
            Assert.Greater((decimal)1, (decimal)0, "Decimal is not greater");
        }

        [Test]
        public void GreaterDecimalWithMessageAndArgs()
        {
            Assert.Greater((decimal)1, (decimal)0, "{0} is not greater than {1}", (decimal)1, (decimal)0);
        }

        [Test]
        public void GreaterLong()
        {
            Assert.Greater((long)1, (long)0);
        }

        [Test]
        public void GreaterLongWithMessage()
        {
            Assert.Greater((long)1, (long)0, "Long is not greater");
        }

        [Test]
        public void GreaterLongWithMessageAndArgs()
        {
            Assert.Greater((long)1, (long)0, "{0} is not greater than {1}", (long)1, (long)0);
        }

        [Test]
        public void GreaterDouble()
        {
            Assert.Greater((double)1, (double)0);
        }

        [Test]
        public void GreaterDoubleWithMessage()
        {
            Assert.Greater((double)1, (double)0, "Double is not greater");
        }

        [Test]
        public void GreaterDoubleWithMessageAndArgs()
        {
            Assert.Greater((double)1, (double)0, "{0} is not greater than {1}", (double)1, (double)0);
        }

        [Test]
        public void GreaterFloat()
        {
            Assert.Greater((float)1, (float)0);
        }

        [Test]
        public void GreaterFloatWithMessage()
        {
            Assert.Greater((float)1, (float)0, "Float is not greater");
        }

        [Test]
        public void GreaterFloatWithMessageAndArgs()
        {
            Assert.Greater((float)1, (float)0, "{0} is not greater than {1}", (float)1, (float)0);
        }

        [Test]
        public void GreaterIComparable()
        {
            Assert.Greater(DateTime.Now, new DateTime(2000, 1, 1));
        }

        [Test]
        public void GreaterIComparableWithMessage()
        {
            Assert.Greater(DateTime.Now, new DateTime(2000, 1, 1), "DateTime is not greater");
        }

        [Test]
        public void GreaterIComparableWithMessageAndArgs()
        {
            DateTime actual = DateTime.Now;
            DateTime expected = new DateTime(2000, 1, 1);
            Assert.Greater(actual, expected, "{0} is not greater than {1}", actual, expected);
        }

        #endregion

        #region GreaterEqualThan

        #region int

        [Test]
        [Row((int)1, (int)0, Description = "Valid Case (Greater Than)")]
        [Row((int)0, (int)0, Description = "Valid Case (Zero Equal)")]
        [Row((int)1, (int)1, Description = "Valid Case (Positive Equal)")]
        [Row((int)-1, (int)-1, Description = "Valid Case (Negative Equal)")]
        [Row((int)1, (int)2, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void GreaterEqualThanIntParamTest(int left, int right)
        {
            Assert.GreaterEqualThan(left, right);
        }

        [Test]
        public void GreaterEqualThanIntWithMessage()
        {
            Assert.GreaterEqualThan((int)1, (int)0, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void GreaterEqualThanIntWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.GreaterEqualThan((int)1, (int)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert GreaterEqualThan[int](message) has failed");
        }

        [Test]
        public void GreaterEqualThanIntWithFormattedMessage()
        {
            Assert.GreaterEqualThan((int)1, (int)0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void GreaterEqualThanIntWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.GreaterEqualThan((int)1, (int)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert GreaterEqualThan[int](message, args) has failed");
        }


        #endregion

        #region short

        [Test]
        [Row(1, 0, Description = "Valid Case (Greater Than)")]
        [Row(0, 0, Description = "Valid Case (Zero Equal)")]
        [Row(1, 1, Description = "Valid Case (Positive Equal)")]
        [Row(-1, -1, Description = "Valid Case (Negative Equal)")]
        [Row(1, 2, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void GreaterEqualThanShortParamTest(short left, short right)
        {
            Assert.GreaterEqualThan(left, right);
        }

        [Test]
        public void GreaterEqualThanShortWithMessage()
        {
            Assert.GreaterEqualThan((short)1, (short)0, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void GreaterEqualThanShortWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.GreaterEqualThan((short)1, (short)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert GreaterEqualThan[short](message) has failed");
        }

        [Test]
        public void GreaterEqualThanShortWithFormattedMessage()
        {
            Assert.GreaterEqualThan((short)1, (short)0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void GreaterEqualThanShortWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.GreaterEqualThan((short)1, (short)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert GreaterEqualThan[short](message, args) has failed");
        }

        #endregion

        #region byte

        [Test]
        [Row(1, 0, Description = "Valid Case (Greater Than)")]
        [Row(0, 0, Description = "Valid Case (Zero Equal)")]
        [Row(1, 1, Description = "Valid Case (Positive Equal)")]
        [Row(1, 2, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void GreaterEqualThanByteParamTest(byte left, byte right)
        {
            Assert.GreaterEqualThan(left, right);
        }

        [Test]
        public void GreaterEqualThanByteWithMessage()
        {
            Assert.GreaterEqualThan((byte)1, (byte)0, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void GreaterEqualThanByteWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.GreaterEqualThan((byte)1, (byte)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert GreaterEqualThan[byte](message) has failed");
        }

        [Test]
        public void GreaterEqualThanByteWithFormattedMessage()
        {
            Assert.GreaterEqualThan((byte)1, (byte)0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void GreaterEqualThanByteWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.GreaterEqualThan((byte)1, (byte)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert GreaterEqualThan[byte](message, args) has failed");
        }

        #endregion

        #region long

        [Test]
        [Row(1, 0, Description = "Valid Case (Greater Than)")]
        [Row(0, 0, Description = "Valid Case (Zero Equal)")]
        [Row(1, 1, Description = "Valid Case (Positive Equal)")]
        [Row(-1, -1, Description = "Valid Case (Negative Equal)")]
        [Row(1, 2, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void GreaterEqualThanLongParamTest(long left, long right)
        {
            Assert.GreaterEqualThan(left, right);
        }

        [Test]
        public void GreaterEqualThanLongWithMessage()
        {
            Assert.GreaterEqualThan((long)1, (long)0, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void GreaterEqualThanLongWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.GreaterEqualThan((long)1, (long)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert GreaterEqualThan[long](message) has failed");
        }

        [Test]
        public void GreaterEqualThanLongWithFormattedMessage()
        {
            Assert.GreaterEqualThan((long)1, (long)0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void GreaterEqualThanLongWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.GreaterEqualThan((long)1, (long)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert GreaterEqualThan[long](message, args) has failed");
        }


        #endregion

        #region double

        [Test]
        [Row(1, 0, Description = "Valid Case (Greater Than)")]
        [Row(0, 0, Description = "Valid Case (Zero Equal)")]
        [Row(1, 1, Description = "Valid Case (Positive Equal)")]
        [Row(-1, -1, Description = "Valid Case (Negative Equal)")]
        [Row(1, 2, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void GreaterEqualThanDoubleParamTest(double left, double right)
        {
            Assert.GreaterEqualThan(left, right);
        }

        [Test]
        public void GreaterEqualThanDoubleWithMessage()
        {
            Assert.GreaterEqualThan((double)1, (double)0, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void GreaterEqualThanDoubleWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.GreaterEqualThan((double)1, (double)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert GreaterEqualThan[double](message) has failed");
        }

        [Test]
        public void GreaterEqualThanDoubleWithFormattedMessage()
        {
            Assert.GreaterEqualThan((double)1, (double)0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void GreaterEqualThanDoubleWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.GreaterEqualThan((double)1, (double)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert GreaterEqualThan[double](message, args) has failed");
        }


        #endregion

        #region float

        [Test]
        [Row(1, 0, Description = "Valid Case (Greater Than)")]
        [Row(0, 0, Description = "Valid Case (Zero Equal)")]
        [Row(1, 1, Description = "Valid Case (Positive Equal)")]
        [Row(-1, -1, Description = "Valid Case (Negative Equal)")]
        [Row(1, 2, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void GreaterEqualThanFloatParamTest(float left, float right)
        {
            Assert.GreaterEqualThan(left, right);
        }

        [Test]
        public void GreaterEqualThanFloatWithMessage()
        {
            Assert.GreaterEqualThan((float)1, (float)0, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void GreaterEqualThanFloatWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.GreaterEqualThan((float)1, (float)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert GreaterEqualThan[float](message) has failed");
        }

        [Test]
        public void GreaterEqualThanFloatWithFormattedMessage()
        {
            Assert.GreaterEqualThan((float)1, (float)0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void GreaterEqualThanFloatWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.GreaterEqualThan((float)1, (float)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert GreaterEqualThan[float](message, args) has failed");
        }


        #endregion

        #region IComparable

        [Test]
        [Row("z", "a", Description = "Valid Case")]
        [Row("a", "a", Description = "Valid Case (Equals)")]
        [Row(2, 1, Description = "Valid Case")]
        [Row(1, 1, Description = "Valid Case (Equals)")]
        [Row(null, null, ExpectedException = (typeof(AssertionException)))]
        [Row(null, 1, ExpectedException = (typeof(AssertionException)))]
        [Row(2, null, ExpectedException = (typeof(AssertionException)))]
        [Row("a", "z", Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void GreaterEqualThanIComparableParamTest(object left, object right)
        {
            Assert.GreaterEqualThan((IComparable)left, (IComparable)right);
        }

        [Test]
        public void GreaterEqualThanIComparableWithMessage()
        {
            Assert.GreaterEqualThan((IComparable)"z", (IComparable)"a", EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void GreaterEqualThanIComparableWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.GreaterEqualThan((IComparable)"a", (IComparable)"z", EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert GreaterEqualThan[IComparable](message) has failed");
        }

        [Test]
        public void GreaterEqualThanIComparableWithFormattedMessage()
        {
            Assert.GreaterEqualThan((IComparable)"z", (IComparable)"a", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void GreaterEqualThanIComparableWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.GreaterEqualThan((IComparable)"a", (IComparable)"z", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert GreaterEqualThan[IComparable](message, args) has failed");
        }


        #endregion

        #endregion

        #endregion

        #region In, NotIn

        #region In

        #region IDictionary
        [Test]
        public void InDictionary()
        {
            Hashtable dic = new Hashtable();
            string test = "test";
            dic.Add(test, null);
            Assert.In(test, dic);
        }

        #endregion

        #region IList

        [Test]
        public void InListTestNull()
        {
            ArrayList list = new ArrayList();
            list.Add(null);
            Assert.In(null, list);
        }
        [Test]
        public void InList()
        {
            ArrayList list = new ArrayList();
            string test = "test";
            list.Add(test);
            Assert.In(test, list);
        }

        #endregion

        #region IEnumerable
     
        [Test]
        public void InEnumerableTestNull()
        {
            ArrayList list = new ArrayList();
            list.Add(null);
            Assert.In(null, (IEnumerable)list);
        }

        [Test]
        public void InEnumerable()
        {
            ArrayList list = new ArrayList();
            string test = "test";
            list.Add(test);
            Assert.In(test, (IEnumerable)list);
        }

        [Test]
        public void In_ValueEquality()
        {
            string[] stringArray = {"item1", "item2"};
            IEnumerable enumerableStringArray = stringArray;
            string item1 = string.Format("item{0}", 1);
            Assert.In(item1, enumerableStringArray);
        }

        [Test]
        public void In_NullItem()
        {
            string[] stringArray = { "item1", null, "item2" };
            IEnumerable enumerableStringArray = stringArray;
            Assert.In(null, enumerableStringArray);
        }

        [Test]
        public void InEnumerableWithMessage()
        {
            ArrayList list = new ArrayList();
            string test = "test";
            list.Add(test);

            Assert.In(test, (IEnumerable)list, "InEnumerable Failed");
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void InEnumerableFail()
        {
            ArrayList list = new ArrayList();
            string test = "test";
            list.Add(test);

            Assert.In("someOtherObject", (IEnumerable)list);
        }

        [Test]
        public void InEnumerableWithMessageFail()
        {
            ArrayList list = new ArrayList();
            string test = "test";
            list.Add(test);

            bool asserted = false;
            try
            {
                Assert.In("someOtherObject", (IEnumerable)list, "InEnumerable Failed");
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf("InEnumerable Failed") >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert InEnumerable(message) has failed");
        }
        #endregion

        #endregion

        #region NotIn

        #region IDictionary
     
        [Test]
        public void NotInDictionary()
        {
            Hashtable dic = new Hashtable();
            string test = "test";
            dic.Add(test, null);
            Assert.NotIn(test + "modified", dic);
        }

        #endregion

        #region IList

        [Test]
        public void NotInListTestNull()
        {
            Assert.NotIn(null, new ArrayList());
        }
        [Test]
        public void NotInList()
        {
            ArrayList list = new ArrayList();
            string test = "test";
            list.Add(test);
            Assert.NotIn(test + "modified", list);
        }

        #endregion

        #region IEnumerable

        [Test]
        public void NotInEnumerableTestNull()
        {
            Assert.NotIn(null, (IEnumerable)new ArrayList());
        }

        [Test]
        public void NotInEnumerable()
        {
            ArrayList list = new ArrayList();
            string test = "test";
            list.Add(test);
            Assert.NotIn(test + "modified", (IEnumerable)list);
        }

        #endregion

        #endregion

        #endregion

        #region IsEmpty
        //NUnit Code
        [Test]
        public void IsEmpty()
        {
            Assert.IsEmpty("", "Failed on empty String");
            Assert.IsEmpty(new int[0], "Failed on empty Array");
            Assert.IsEmpty(new ArrayList(), "Failed on empty ArrayList");
            Assert.IsEmpty(new Hashtable(), "Failed on empty Hashtable");
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void IsEmptyFailsOnString()
        {
            Assert.IsEmpty("Hi!");
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void IsEmptyFailsOnNullString()
        {
            Assert.IsEmpty((string)null);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void IsEmptyFailsOnNonEmptyArray()
        {
            Assert.IsEmpty(new int[] { 1, 2, 3 });
        }


        #endregion

        #region IsNotEmpty
        [Test]
        public void IsNotEmpty()
        {
            ArrayList arr = new ArrayList();
            arr.Add("Testing");

            Hashtable hash = new Hashtable();
            hash.Add("MbUnit", "Testing");

            Assert.IsNotEmpty("MbUnit", "Failed on non empty String");
            Assert.IsNotEmpty(new int[1] { 1 }, "Failed on non empty Array");
            Assert.IsNotEmpty(arr, "Failed on non empty ArrayList");
            Assert.IsNotEmpty(hash, "Failed on empty Hashtable");
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void IsNotEmptyFailsOnString()
        {
            Assert.IsNotEmpty(string.Empty);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void IsNotEmptyFailsOnNonEmptyArray()
        {
            Assert.IsNotEmpty(new int[0] { });
        }


        #endregion

        #region IsNan
        //Nunit Code
        [Test]
        public void IsNaN()
        {
            Assert.IsNaN(double.NaN);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void IsNaNFails()
        {
            Assert.IsNaN(10.0);
        }

        #endregion

        #region Contains

        [Test]
        public void Contains()
        {
            string s = "MbUnit";
            string contain = "Unit";
            Assert.Contains(s, contain);
        }

        #endregion

        #region Equals

        [Test, ExpectedException(typeof(AssertionException))]
        public void Equals()
        {
            Assert.Equals(null, null);
        }

        #endregion

        #region AreSame

        [Test]
        public void AreSame()
        {
            object objectA = new object();

            Assert.AreSame(objectA, objectA);
        }
        
        [Test]
        public void AreSameDifferentReference()
        {
            object objectA = new object();
            object objectB = objectA;

            Assert.AreSame(objectA, objectB);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreSameInteger_Fail()
        {
            Assert.AreSame(0, 0);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreSameInteger_ImplicitBox_Fail()
        {
            int intA = 1;

            // Due to boxing, these will not refer to the same object
            Assert.AreSame(intA, intA);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreSameInteger_ExplicitBoxToValue_Fail()
        {
            int intA = 1;
            object objectA = (object) intA;
            
            // Due to boxing, these will not refer to the same object
            Assert.AreSame(objectA, intA);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreSameInteger_ExplicitBoxToSameValue_Fail()
        {
            int intA = 1;
            object objectA = (object)intA;
            object objectB = (object)intA;

            // These will not refer to the same object
            Assert.AreSame(objectA, objectB);
        }

        [Test]
        public void AreSame_String()
        {
            // Both strings will be interned, so are the same object
            Assert.AreSame("A String", "A String");
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreSame_NonInterned_NewString_Fail()
        {
            string stringA = "A String";
            string stringB = new String(new char[] { 'A', ' ', 'S', 't', 'r', 'i', 'n', 'g' });

            Assert.AreSame(stringA, stringB);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreSame_NonInterned_ConcatString_Fail()
        {
            // JIT will not intern the concat string, so these are not the same
            string stringA = "A String";
            string stringB = String.Concat("A ","String");

            Assert.AreSame(stringA, stringB);
        }

        [Test]
        public void AreSame_InternedString()
        {
            string stringA = "A String";
            string stringB = String.Intern(String.Concat("A ", "String"));

            Assert.AreSame(stringA, stringB);
        }

        [Test]
        public void AreSame_Array()
        {
            int[] arrayA = new int[] { 1, 2, 3 };
            int[] arrayB = arrayA;

            Assert.AreSame(arrayA, arrayB);
        } 
        
        [Test, ExpectedException(typeof(AssertionException))]
        public void AreSame_Array_Fail()
        {
            int[] arrayA = new int[] { 1, 2, 3 };
            int[] arrayB = new int[] { 1, 2, 3 };

            Assert.AreSame(arrayA, arrayB);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreSame_ArrayClone_Fail()
        {
            int[] arrayA = new int[] { 1, 2, 3 };
            int[] arrayB = (int[]) arrayA.Clone();

            Assert.AreSame(arrayA, arrayB);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreSameFail()
        {
            object objectA = new object();
            object objectB = new object();

            Assert.AreSame(objectA, objectB);
        }

        [Test]
        public void AreSameNull()
        {
            Assert.AreSame(null, null);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreSameExpectedNull()
        {
            Assert.AreSame(null, new object());
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreSameActualNull()
        {
            Assert.AreSame(new object(), null);
        }

        [Test]
        public void AreSameWithMessage()
        {
            object objectA = new object();
            Assert.AreSame(objectA, objectA, "Assert AreSame(message) has failed");
        }

        [Test]
        public void AreSameWithMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.AreSame(new object(), new object(), EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert AreSame(message) has failed");
        }

        [Test]
        public void AreSameWithFormattedMessage()
        {
            object objectA = new object();
            Assert.AreSame(objectA, objectA, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void AreSameWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                Assert.AreSame(new object(), new object(), TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert AreSame(message, args) has failed");
        }

        #endregion

        #region AreNotSame

        [Test]
        public void AreNotSame()
        {
            object objectA = new object();
            object objectB = new object();

            Assert.AreNotSame(objectA, objectB);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreNotSameFail()
        {
            object objectA = new object();
            object objectB = objectA;

            Assert.AreNotSame(objectA, objectB);
        }

        [Test]
        public void AreNotSameWithMessage()
        {
            Assert.AreNotSame(new object(), new object(), "Assert AreNotSame(message) has failed");
        }

        [Test]
        public void AreNotSameWithMessageFail()
        {
            bool asserted = false;

            try
            {
                object objectA = new object();
                Assert.AreNotSame(objectA, objectA, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert AreNotSame(message) has failed");
        }

        [Test]
        public void AreNotSameWithFormattedMessage()
        {
            Assert.AreNotSame(new object(), new object(), TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void AreNotSameWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                object objectA = new object();
                Assert.AreNotSame(objectA, objectA, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                Assert.Fail("Assert AreNotSame(message, args) has failed");
        }

        #endregion

        #region AreValueEqual

        //[Test]
        //public void AreValueEqual()
        //{
        //}

        //[Test, ExpectedException(typeof(AssertionException))]
        //public void AreValueEqualFail()
        //{
        //}

        #endregion
    }
}
