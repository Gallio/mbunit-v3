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
using Gallio.Framework.Assertions;
using MbUnit.Framework;

#pragma warning disable 0618

namespace MbUnit.Compatibility.Tests.Framework
{
    // FIXME: May contain NUnit derived code but is missing proper attribution!
    //        Need to follow-up with the original contributor.
    [TestFixture]
    [TestsOn(typeof(OldAssert))]
    public class OldAssertTest
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
                OldAssert.Fail("CustomMessage");
            }
            catch (AssertionException ex)
            {
                OldAssert.AreEqual(message, ex.Message);
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
                OldAssert.Fail();
            }
            catch (AssertionException)
            {
                bolPass = false;
            }

            if (bolPass == true)
            {
                OldAssert.IsFalse(true, "Assert fail has failed");
            }

        }

        #region Ignore

        /* FIXME
        [Test, ExpectedException(typeof(IgnoreRunException))]
        public void Ignore()
        {
            OldAssert.Ignore("This test will be ignored.");
        }

        [Test, ExpectedArgumentNullException]
        public void IgnoreWithNullMessage()
        {
            OldAssert.Ignore(null);
        }

        [Test]
        public void IgnoreWithFormattedMessage()
        {
            bool asserted = false;
            
            try
            {
                OldAssert.Ignore(TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (IgnoreRunException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert Ignore(message, args) has failed");
        }
         * */

        #endregion

        #region IsNull

        [Test]
        public void IsNull()
        {
            OldAssert.IsNull(null);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void IsNullFail()
        {
            OldAssert.IsNull("non-null string");
        }

        [Test]
        public void IsNullWithMessage()
        {
            OldAssert.IsNull(null, "IsNull has failed.");
        }

        [Test]
        public void IsNullWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.IsNull(new object(), EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert IsNull(message) has failed");
        }

        [Test]
        public void IsNullWithFormattedMessage()
        {
            OldAssert.IsNull(null, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void IsNullWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.IsNull(new object(), TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert IsNull(message, args) has failed");
        }
        #endregion

        #region IsNotNull

        [Test]
        public void IsNotNull()
        {
            OldAssert.IsNotNull(new object());
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void IsNotNullFail()
        {
            OldAssert.IsNotNull(null);
        }

        [Test]
        public void IsNotNullWithMessage()
        {
            OldAssert.IsNotNull(new object(), "IsNotNull has failed.");
        }

        [Test]
        public void IsNotNullWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.IsNotNull(null, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert IsNotNull(message) has failed");
        }

        [Test]
        public void IsNotNullWithFormattedMessage()
        {
            OldAssert.IsNotNull(new object(), TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }
        
        [Test]
        public void IsNotNullWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.IsNotNull(null, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert IsNotNull(message, args) has failed");
        }
        #endregion


        #region IsTrue

        [Test]
        public void IsTrue()
        {
            OldAssert.IsTrue(true);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void IsTrueFail()
        {
            OldAssert.IsTrue(false);
        }

        [Test]
        public void IsTrueWithMessage()
        {
            OldAssert.IsTrue(true, "IsTrue Failed");
        }

        [Test]
        public void IsTrueWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.IsTrue(false, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert IsTrue(message) has failed");
        }

        [Test]
        public void IsTrueWithFormattedMessage()
        {
            OldAssert.IsTrue(true, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void IsTrueWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.IsTrue(false, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert IsTrue(message, args) has failed");
        }


        #endregion

        #region IsFalse

        [Test]
        public void IsFalse()
        {
            OldAssert.IsFalse(false);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void IsFalseFail()
        {
            OldAssert.IsFalse(true);
        }

        [Test]
        public void IsFalseWithMessage()
        {
            OldAssert.IsFalse(false, "IsFalse has failed.");
        }

        [Test]
        public void IsFalseWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.IsFalse(true, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert IsFalse(message) has failed");
        }

        [Test]
        public void IsFalseWithFormattedMessage()
        {
            OldAssert.IsFalse(false, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void IsFalseWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.IsFalse(true, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert IsFalse(message, args) has failed");
        }
        #endregion


        #endregion

        #region AreEqual

        [Test]
        public void AreEqualWithMessage()
        {
            OldAssert.AreEqual("abcd", "abcd", EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void AreEqualWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.AreEqual("abcd", "dcba", EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert AreEqual(message) has failed");
        }

        [Test]
        public void AreEqualWithFormattedMessage()
        {
            OldAssert.AreEqual("abcd", "abcd", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void AreEqualWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.AreEqual("abcd", "dbca", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert AreEqual(message, args) has failed");
        }

        [Test]
        public void AreEqual_NullArguments()
        {
            OldAssert.AreEqual(null, null);
        }

        #region AreEqual (String)

        [Test]
        public void AreEqualString()
        {
            OldAssert.AreEqual("hello", "hello");
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreEqualStringActualNullFail()
        {
            OldAssert.AreEqual("hello", null);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreEqualStringExpectedNullFail()
        {
            OldAssert.AreEqual(null, "hello");
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreEqualStringOrdinalCompare()
        {
            OldAssert.AreEqual("hello", "HELLO");
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreEqualStringFail()
        {
            OldAssert.AreEqual("hello", "world");
        }

        [Test]
        public void AreEqualEmptyString()
        {
            OldAssert.AreEqual("", "");
        }

        #endregion

        #region AreEqual (Double)
        // TODO: It'd be great to refactor these tests using Generics (when under 2.0)

        [Test]
        public void AreEqualDoubleZero()
        {
            OldAssert.AreEqual((double)0.0, (double)0.0);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreEqualDoubleFail()
        {
            OldAssert.AreEqual((double)0.0, (double)1.0);
        }

        [Test]
        public void AreEqualDoublePositive()
        {
            OldAssert.AreEqual((double)1.0, (double)1.0);
        }

        [Test]
        public void AreEqualDoubleNegative()
        {
            OldAssert.AreEqual((double)-1.0, (double)-1.0);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreEqualDoubleNegativeFail()
        {
            OldAssert.AreEqual((double)-1.0, (double)1.0);
        }

        [Test]
        public void AreEqualDoubleDeltaNegativeInfinity()
        {
            OldAssert.AreEqual(double.NegativeInfinity, double.NegativeInfinity, (double) 0.0);
        }

        [Test]
        public void AreEqualDoubleDeltaPositiveInfinity()
        {
            OldAssert.AreEqual(double.PositiveInfinity, double.PositiveInfinity, (double)0.0);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreEqualDoubleDeltaExpectedInfinityFail()
        {
            OldAssert.AreEqual(double.PositiveInfinity, (double)1.0, (double)0.0);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreEqualDoubleDeltaActualInfinityFail()
        {
            OldAssert.AreEqual((double)1.0, double.PositiveInfinity, (double)0.0);
        }

        [Test]
        public void AreEqualDoubleDelta()
        {
            OldAssert.AreEqual((double)0.0, (double)1.0, (double)1.0);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreEqualDoubleDeltaFail()
        {
            OldAssert.AreEqual((double)0.0, (double)2.0, (double)1.0);
        }

        [Test]
        [ExpectedArgumentException()]
        public void AreEqualDoubleDeltaNegative()
        {
            OldAssert.AreEqual((double)0.0, (double)0.0, (double)-1.0);
        }

        
        #endregion

        #region AreEqual (Float)

        [Test]
        public void AreEqualFloatZero()
        {
            OldAssert.AreEqual((float)0.0, (float) 0.0);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreEqualFloatFail()
        {
            OldAssert.AreEqual((float)0.0, (float)1.0);
        }

        [Test]
        public void AreEqualFloatPositive()
        {
            OldAssert.AreEqual((float)1.0, (float)1.0);
        }

        [Test]
        public void AreEqualFloatNegative()
        {
            OldAssert.AreEqual((float)-1.0, (float)-1.0);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreEqualFloatNegativeFail()
        {
            OldAssert.AreEqual((float)-1.0, (float)1.0);
        }

        [Test]
        public void AreEqualFloatDeltaNegativeInfinity()
        {
            OldAssert.AreEqual(float.NegativeInfinity, float.NegativeInfinity, (float)1.0);
        }

        [Test]
        public void AreEqualFloatDeltaPositiveInfinity()
        {
            OldAssert.AreEqual(float.PositiveInfinity, float.PositiveInfinity, (float)1.0);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreEqualFloatDeltaExpectedInfinityFail()
        {
            OldAssert.AreEqual(float.PositiveInfinity, (float)1.0, (float)1.0);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreEqualFloatDeltaActualInfinityFail()
        {
            OldAssert.AreEqual((float)1.0, float.PositiveInfinity, (float)1.0);
        }

        [Test]
        public void AreEqualFloatDelta()
        {
            OldAssert.AreEqual((float)0.0, (float)1.0, (float)1.0);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreEqualFloatDeltaFail()
        {
            OldAssert.AreEqual((float)0.0, (float)2.0, (float)1.0);
        }

        [Test]
        [ExpectedArgumentException()]
        public void AreEqualFloatDeltaNegative()
        {
            OldAssert.AreEqual((float)0.0, (float)0.0, (float)-1.0);
        }

        [Test]
        public void AreEqualFloatDeltaWithMessage()
        {
            OldAssert.AreEqual((float)1.0, (float)1.0, (float)0.0, "Assert AreEqual(message) has failed");
        }

        [Test]
        public void AreEqualFloatDeltaWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.AreEqual((float)0.0, (float)1.0, (float)0.0, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert AreEqual(message) has failed");
        }

        [Test]
        public void AreEqualFloatDeltaWithFormattedMessage()
        {
            float l = 1.0f;
            float r = 1.0f;
            float d = 1.0f;
            OldAssert.AreEqual(l, r, d, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void AreEqualFloatDeltaWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.AreEqual((float)0.0, (float)1.0, (float)0.0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert AreEqual(message, args) has failed");
        }
        #endregion

        #region AreEqual (Arrays)

        [Test]
        public void AreEqual_EmptyIntArrays()
        {
            OldAssert.AreEqual(new int[] {}, new int[] {});
        }

        [Test]
        public void AreEqual_EqualIntArrays()
        {
            OldAssert.AreEqual(new int[] { 1, 2, 3 }, new int[] { 1, 2, 3 });
        }

        [Test]
        public void AreEqual_NotEqualForArrayAndNonArray()
        {
            OldAssert.AreNotEqual(new int[] { 1, 2, 3 }, 3);
        }

        [Test]
        public void AreEqual_ArrayOfNullValues()
        {
            object[] a = new object[3];
            object[] b = new object[3];
            OldAssert.AreEqual(a, b);
        }

        [Test]
        public void AreEqual_EqualArrayWithNullElements()
        {
            object[] a = { 1, 2, null };
            object[] b = { 1, 2, null };
            OldAssert.AreEqual(a, b);
        }

        [Test]
        public void AreEqual_EqualArrayWithObjectElements()
        {
            object objectA = new object();
            object objectB = new object();
            object objectC = new object();

            object[] a = { objectA, objectB, objectC };
            object[] b = { objectA, objectB, objectC };
            OldAssert.AreEqual(a, b);
        }

        [Test]
        public void AreEqual_EqualStringArrays()
        {
            OldAssert.AreEqual(new string[] { "1", "2", "3" }, new string[] { "1", "2", "3" });
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreEqual_UnEqualIntArrays()
        {
            OldAssert.AreEqual(new int[] { 1, 2, 3 }, new int[] { 1, 2, 4 });
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreEqual_UnEqualSizeIntArrays()
        {
            OldAssert.AreEqual(new int[0], new int[] { 1, 2 });
        }

        #endregion

        #region AreEqual (Decimal)

        private const Decimal TEST_DECIMAL = (Decimal) 1.034;
        private const Decimal TEST_OTHER_DECIMAL = (Decimal) 2.034;
 
        [Test]
        public void AreEqualDecimalZero()
        {
            OldAssert.AreEqual((decimal)0.0, (decimal)0.0);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreEqualDecimalFail()
        {
            OldAssert.AreEqual(TEST_DECIMAL, TEST_OTHER_DECIMAL);
        }

        [Test]
        public void AreEqualDecimalPositive()
        {
            OldAssert.AreEqual(TEST_DECIMAL, TEST_DECIMAL);
        }

        [Test]
        public void AreEqualDecimalNegative()
        {
            OldAssert.AreEqual(-TEST_DECIMAL, -TEST_DECIMAL);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreEqualDecimalNegativeFail()
        {
            OldAssert.AreEqual(-TEST_DECIMAL, TEST_DECIMAL);
        }

        [Test]
        public void AreEqualDecimalWithMessage()
        {
            Decimal l = TEST_DECIMAL;
            Decimal r = TEST_DECIMAL;
            OldAssert.AreEqual(l, r, "Assert AreEqual(message) has failed");
        }

        [Test]
        public void AreEqualDecimalWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.AreEqual(TEST_DECIMAL, TEST_OTHER_DECIMAL, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert AreEqual(message) has failed");
        }

        [Test]
        public void AreEqualDecimalWithFormattedMessage()
        {
            OldAssert.AreEqual(TEST_DECIMAL, TEST_DECIMAL, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void AreEqualDecimalWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.AreEqual(TEST_DECIMAL, TEST_OTHER_DECIMAL, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert AreEqual(message, args) has failed");
        }
        #endregion

        #region AreEqual (Integer)

        [Test]
        public void AreEqualIntZero()
        {
            OldAssert.AreEqual(0, 0);
        }
        
        [Test, ExpectedException(typeof(AssertionException))]
        public void AreEqualIntFail()
        {
            OldAssert.AreEqual(0, 1);
        }

        [Test]
        public void AreEqualIntPositive()
        {
            OldAssert.AreEqual(1, 1);
        }

        [Test]
        public void AreEqualIntNegative()
        {
            OldAssert.AreEqual(-1, -1);
        }
        
        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreEqualIntNegativeFail()
        {
            OldAssert.AreEqual(-1, 1);
        }

        [Test]
        public void AreEqualIntDelta()
        {
            OldAssert.AreEqual(0, 1, 1);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreEqualIntDeltaFail()
        {
            OldAssert.AreEqual(0, 2, 1);
        }

        [Test]
        [ExpectedArgumentException()]
        public void AreEqualIntDeltaNegative()
        {
            OldAssert.AreEqual(0, 0, -1);
        }

        [Test]
        public void AreEqualIntWithMessage()
        {
            OldAssert.AreEqual(1, 1, "Assert AreEqual(message) has failed");
        }

        [Test]
        public void AreEqualIntWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.AreEqual(0, 1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert AreEqual(message) has failed");
        }

        [Test]
        public void AreEqualIntWithFormattedMessage()
        {
            OldAssert.AreEqual(1,1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void AreEqualIntWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.AreEqual(0,1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert AreEqual(message, args) has failed");
        }

        #endregion

        #endregion

        #region AreNotEqual

        [Test]
        public void NotEqual()
        {
            OldAssert.AreNotEqual(5, 3);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void NotEqualFails()
        {
            OldAssert.AreNotEqual(5, 5);
        }

        [Test]
        public void NullExpectedNotEqualToNonNullActual()
        {
            OldAssert.AreNotEqual(null, 3);
        }

        [Test]
        public void NonNullExpectedNotEqualToNullActual()
        {
            OldAssert.AreNotEqual(3, null);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void NullEqualsNull()
        {
            OldAssert.AreNotEqual(null, null);
        }

        [Test]
        public void AreNotEqual_WithMessage()
        {
            OldAssert.AreNotEqual("abcd", "dcba", EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void AreNotEqualWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.AreNotEqual("abcd", "abcd", EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert AreNotEqual(message) has failed");
        }

        [Test]
        public void AreNotEqual_WithFormattedMessage()
        {
            OldAssert.AreNotEqual("abcd", "dcba", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void AreNotEqualWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.AreNotEqual("abcd", "abcd", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert AreNotEqual(message, args) has failed");
        }

        #region AreNotEqual (Array)

        [Test]
        public void AreNotEqualArray()
        {
            OldAssert.AreNotEqual(new object[] { 1, 2, 3 }, new int[] { 1, 3, 2 });
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualArray_EmptyObjectArraysFail()
        {
            object[] objectA = new object[] {};
            object[] objectB = new object[] { };

            OldAssert.AreNotEqual(objectA, objectB);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualArray_EmptyValueTypeArraysFail()
        {
            int[] valueArrayA = new int[] { };
            int[] valueArrayB = new int[] { };

            OldAssert.AreNotEqual(valueArrayA, valueArrayB);
        }

        [Test]
        public void AreNotEqualArray_UnEqualValueTypeArrays()
        {
            int[] valueArrayA = new int[] { 1, 2, 3 };
            int[] valueArrayB = new int[] { 2, 3, 1 };

            OldAssert.AreNotEqual(valueArrayA, valueArrayB);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualArray_EqualValueTypeArraysFail()
        {
            int[] valueArrayA = new int[] { 1, 2, 3 };
            int[] valueArrayB = new int[] { 1, 2, 3 };

            OldAssert.AreNotEqual(valueArrayA, valueArrayB);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualArrayFails()
        {
            OldAssert.AreNotEqual(new object[] { 1, 2, 3 }, new object[] { 1, 2, 3 });
        }

        [Test]
        public void AreNotEqualArrayUnEqualLength()
        {
            OldAssert.AreNotEqual(new object[] { 1, 2, 3 }, new object[] { 1, 2, 3, 4 });
        }

        [Test]
        public void AreNotEqualArrayUnEqualLengthPaddedWithNulls()
        {
            OldAssert.AreNotEqual(new object[] { 1, 2, 3 }, new object[] { 1, 2, 3, null, null });
        }

        [Test]
        public void AreNotEqualTwoArraysContainingNull()
        {
            OldAssert.AreNotEqual(new object[] { 1, 2, null }, new object[] { 1, null, 3 });
        }

        [Test]
        public void AreNotEqualArrayWithMessage()
        {
            OldAssert.AreNotEqual(new object[] { 1, 2, 3 }, new object[] { 2, 1, 3 }, EXPECTED_FAIL_MESSAGE);
        }
        [Test]
        public void AreNotEqualArrayWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.AreNotEqual(new object[] { 1, 2, 3 }, new object[] { 1, 2, 3 }, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert AreNotEqual(message) has failed");
        }

        [Test]
        public void AreNotEqualArrayWithFormattedMessage()
        {
            OldAssert.AreNotEqual(new object[] { 1, 2, 3 }, new object[] { 2, 1, 3 }, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void AreNotEqualArrayWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.AreNotEqual(new object[] { 1, 2, 3 }, new object[] { 1, 2, 3 }, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert AreNotEqual(message, args) has failed");
        }

        #endregion

        #region AreNotEqual (String)

        [Test]
        public void AreNotEqualString()
        {
            OldAssert.AreNotEqual("hello", "world");
        }

        [Test]
        public void AreNotEqualStringActualNullFail()
        {
            OldAssert.AreNotEqual("hello", null);
        }

        [Test]
        public void AreNotEqualStringExpectedNullFail()
        {
            OldAssert.AreNotEqual(null, "hello");
        }

        [Test]
        public void AreNotEqualStringOrdinalCompare()
        {
            OldAssert.AreNotEqual("hello", "HELLO");
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreNotEqualStringFail()
        {
            OldAssert.AreNotEqual("hello", "hello");
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreNotEqualEmptyString()
        {
            OldAssert.AreNotEqual("", "");
        }

        #endregion

        #region AreNotEqual (Double)

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreNotEqualDoubleZero()
        {
            OldAssert.AreNotEqual((double)0.0, (double)0.0);
        }

        [Test]
        public void AreNotEqualDouble()
        {
            OldAssert.AreNotEqual((double)0.0, (double)1.0);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreNotEqualDoublePositiveFail()
        {
            OldAssert.AreNotEqual((double)1.0, (double)1.0);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreNotEqualDoubleNegativeFail()
        {
            OldAssert.AreNotEqual((double)-1.0, (double)-1.0);
        }

        [Test]
        public void AreNotEqualDoubleNegativeAndPositive()
        {
            OldAssert.AreNotEqual((double)-1.0, (double)1.0);
        }

        [Test]
        public void AreNotEqualDoubleWithMessage()
        {
            OldAssert.AreNotEqual((double)1.0, (double)2.0, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualDoubleWithMessageFail()
        {
            OldAssert.AreNotEqual((double) 1.0, (double) 1.0, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void AreNotEqualDoubleWithFormattedMessage()
        {
            OldAssert.AreNotEqual((double)1.0, (double)2.0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualDoubleWithFormattedMessageFail()
        {
            OldAssert.AreNotEqual((double)1.0, (double)1.0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        #endregion

        #region AreNotEqual (Float)

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualFloatZero()
        {
            OldAssert.AreNotEqual((float)0.0, (float)0.0);
        }

        [Test]
        public void AreNotEqualFloat()
        {
            OldAssert.AreNotEqual((float)0.0, (float)1.0);
        }

        [Test]
        public void AreNotEqualFloatPositive()
        {
            OldAssert.AreNotEqual((float)1.0, (float)2.0);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualFloatPositiveFail()
        {
            OldAssert.AreNotEqual((float)1.0, (float)1.0);
        }

        [Test]
        public void AreNotEqualFloatNegative()
        {
            OldAssert.AreNotEqual((float)-1.0, (float)-2.0);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualFloatNegativeFail()
        {
            OldAssert.AreNotEqual((float)-1.0, (float)-1.0);
        }

        [Test]
        public void AreNotEqualFloatNegativeAndPositive()
        {
            OldAssert.AreNotEqual((float)-1.0, (float)1.0);
        }


        [Test]
        public void AreNotEqualFloatWithMessage()
        {
            OldAssert.AreNotEqual((float)1.0, (float)3.0, "Assert AreNotEqual(message) has failed");
        }

        [Test]
        public void AreNotEqualFloatWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.AreNotEqual((float)1.0, (float)1.0, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert AreNotEqual(message) has failed");
        }

        [Test]
        public void AreNotEqualFloatWithFormattedMessage()
        {
            float l = 1.0f;
            float r = 3.0f;
            OldAssert.AreNotEqual(l, r, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void AreNotEqualFloatWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.AreNotEqual((float)1.0, (float)1.0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert AreNotEqual(message, args) has failed");
        }
        #endregion

        #region AreEqual (Decimal)

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualDecimalZero()
        {
            OldAssert.AreNotEqual((decimal)0.0, (decimal)0.0);
        }

        [Test]
        public void AreNotEqualDecimal()
        {
            OldAssert.AreNotEqual(TEST_DECIMAL, TEST_OTHER_DECIMAL);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualDecimalPositiveFail()
        {
            OldAssert.AreNotEqual(TEST_DECIMAL, TEST_DECIMAL);
        }

        [Test]
        public void AreNotEqualDecimalNegative()
        {
            OldAssert.AreNotEqual(-TEST_DECIMAL, -TEST_OTHER_DECIMAL);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualDecimalNegativeFail()
        {
            OldAssert.AreNotEqual(-TEST_DECIMAL, -TEST_DECIMAL);
        }

        [Test]
        public void AreNotEqualDecimalNegativePositive()
        {
            OldAssert.AreNotEqual(-TEST_DECIMAL, TEST_DECIMAL);
        }

        [Test]
        public void AreNotEqualDecimalWithMessage()
        {
            OldAssert.AreNotEqual(TEST_DECIMAL, TEST_OTHER_DECIMAL, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualDecimalWithMessageFail()
        {
            OldAssert.AreNotEqual(TEST_DECIMAL, TEST_DECIMAL, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void AreNotEqualDecimalWithFormattedMessage()
        {
            OldAssert.AreNotEqual(TEST_DECIMAL, TEST_OTHER_DECIMAL, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualDecimalWithFormattedMessageFail()
        {
            OldAssert.AreNotEqual(TEST_DECIMAL, TEST_DECIMAL, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        #region AreNotEqual (Integer)

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualIntZero()
        {
            OldAssert.AreNotEqual(0, 0);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualIntFail()
        {
            OldAssert.AreNotEqual(1, 1);
        }

        [Test]
        public void AreNotEqualIntPositive()
        {
            OldAssert.AreNotEqual(1, 2);
        }

        [Test]
        public void AreNotEqualIntNegative()
        {
            OldAssert.AreNotEqual(-1, -2);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualIntNegativeFail()
        {
            OldAssert.AreNotEqual(-1, -1);
        }

        [Test]
        public void AreNotEqualIntWithMessage()
        {
            OldAssert.AreNotEqual(1, 2, "Assert AreEqual(message) has failed");
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualIntWithMessageFail()
        {
            OldAssert.AreNotEqual(1, 1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void AreNotEqualIntWithFormattedMessage()
        {
            OldAssert.AreNotEqual(1, 2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualIntWithFormattedMessageFail()
        {
            OldAssert.AreNotEqual(1, 1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        #endregion

        #region AreNotEqual (Unsigned Integer)

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualUIntZero()
        {
            uint ua = 0;
            uint ub = 0;

            OldAssert.AreNotEqual(ua, ub);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualUIntFail()
        {
            uint ua = 1;
            uint ub = 1;

            OldAssert.AreNotEqual(ua, ub);
        }

        [Test]
        public void AreNotEqualUIntPositive()
        {
            uint ua = 1;
            uint ub = 2;

            OldAssert.AreNotEqual(ua, ub);
        }

        [Test]
        public void AreNotEqualUIntWithMessage()
        {
            uint ua = 1;
            uint ub = 2;

            OldAssert.AreNotEqual(ua, ub, "Assert AreEqual(message) has failed");
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualUIntWithMessageFail()
        {
            uint ua = 1;
            uint ub = 1;

            OldAssert.AreNotEqual(ua, ub, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void AreNotEqualUIntWithFormattedMessage()
        {
            uint ua = 1;
            uint ub = 2;

            OldAssert.AreNotEqual(ua, ub, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void AreNotEqualUIntWithFormattedMessageFail()
        {
            uint ua = 1;
            uint ub = 1;

            OldAssert.AreNotEqual(ua, ub, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
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
            OldAssert.Between(test, left, right);
        }

        [Test]
        public void BetweenIntWithMessage()
        {
            OldAssert.Between(1, 0, 2, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void BetweenIntWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.Between(3, 0, 2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert Between(message) has failed");
        }

        [Test]
        public void BetweenIntWithFormattedMessage()
        {
            OldAssert.Between(1, 0, 2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void BetweenIntWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.Between(3, 0, 2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert Between[int](message, args) has failed");
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
            OldAssert.Between(test, left, right);
        }

        [Test]
        public void BetweenShortWithMessage()
        {
            OldAssert.Between((short)1, (short)0, (short)2, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void BetweenShortWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.Between((short)2, (short)0, (short)1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert Between[short](message) has failed");
        }

        [Test]
        public void BetweenShortWithFormattedMessage()
        {
            OldAssert.Between((short)1, (short)0, (short)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void BetweenShortWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.Between((short)2, (short)0, (short)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert Between[short](message, args) has failed");
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
            OldAssert.Between(test, left, right);
        }

        [Test]
        public void BetweenByteWithMessage()
        {
            OldAssert.Between((byte)1, (byte)0, (byte)2, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void BetweenByteWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.Between((byte)3, (byte)0, (byte) 2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert Between[byte](message) has failed");
        }

        [Test]
        public void BetweenByteWithFormattedMessage()
        {
            OldAssert.Between((byte)1, (byte)0, (byte)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void BetweenByteWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.Between((byte)3, (byte)0, (byte) 2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert Between[byte](message, args) has failed");
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
            OldAssert.Between(test, left, right);
        }

        [Test]
        public void BetweenLongWithMessage()
        {
            OldAssert.Between((long)1, (long)0, (long)2, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void BetweenLongWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.Between((long)3, (long)0, (long)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert Between[long](message) has failed");
        }

        [Test]
        public void BetweenLongWithFormattedMessage()
        {
            OldAssert.Between((long)1, (long)0, (long)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void BetweenLongWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.Between((long)3, (long)0, (long)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert Between[long](message, args) has failed");
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
            OldAssert.Between(test, left, right);
        }

        [Test]
        public void BetweenDoubleWithMessage()
        {
            OldAssert.Between((double)1, (double)0, (double)2, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void BetweenDoubleWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.Between((double)3,(double)0,(double)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert Between[double](message) has failed");
        }

        [Test]
        public void BetweenDoubleWithFormattedMessage()
        {
            OldAssert.Between((double)1, (double)0, (double)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void BetweenDoubleWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.Between((double)3,(double)0,(double)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert Between[double](message, args) has failed");
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
            OldAssert.Between(test, left, right);
        }

        [Test]
        public void BetweenFloatWithMessage()
        {
            OldAssert.Between((float)1, (float)0, (float)2, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void BetweenFloatWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.Between((float)3,(float)0,(float)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert Between[float](message) has failed");
        }

        [Test]
        public void BetweenFloatWithFormattedMessage()
        {
            OldAssert.Between((float)1,(float)0,(float)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void BetweenFloatWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.Between((float)3,(float)0,(float)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert Between[float](message, args) has failed");
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
            OldAssert.Between((IComparable) test, (IComparable) left, (IComparable) right);
        }

        [Test]
        public void BetweenIComparableWithMessage()
        {
            OldAssert.Between((IComparable)"b", (IComparable)"a", (IComparable)"c", EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void BetweenIComparableWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.Between((IComparable)"d", (IComparable)"a", (IComparable)"c", EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert Between[IComparable](message) has failed");
        }

        [Test]
        public void BetweenIComparableWithFormattedMessage()
        {
            OldAssert.Between((IComparable)"b", (IComparable)"a", (IComparable)"c", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void BetweenIComparableWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.Between((IComparable)"d", (IComparable)"a", (IComparable)"c", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert Between[IComparable](message, args) has failed");
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
            OldAssert.NotBetween(test, left, right);
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
            OldAssert.NotBetween(test, left, right);
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
            OldAssert.NotBetween(test, left, right);
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
            OldAssert.NotBetween(test, left, right);
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
            OldAssert.NotBetween(test, left, right);
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
            OldAssert.NotBetween(test, left, right);
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
            OldAssert.NotBetween((IComparable) test, (IComparable)left, (IComparable)right);
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
            OldAssert.LowerThan(left, right);
        }

        [Test]
        public void LowerThanIntWithMessage()
        {
            OldAssert.LowerThan(0,1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LowerThanIntWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.LowerThan(2,1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert LowerThan[int](message) has failed");
        }

        [Test]
        public void LowerThanIntWithFormattedMessage()
        {
            OldAssert.LowerThan(0,1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LowerThanIntWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.LowerThan(2,1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert LowerThan[int](message, args) has failed");
        }
 
        #endregion

        #region short

        [Test]
        [Row((short)0, (short)1, Description = "Valid Case")]
        [Row((short)2, (short)1, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void LowerThanShortParamTest(short left, short right)
        {
            OldAssert.LowerThan(left, right);
        }

        [Test]
        public void LowerThanShortWithMessage()
        {
            OldAssert.LowerThan((short)0, (short)1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LowerThanShortWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.LowerThan((short)2, (short)1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert LowerThan[short](message) has failed");
        }

        [Test]
        public void LowerThanShortWithFormattedMessage()
        {
            OldAssert.LowerThan((short)0, (short)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LowerThanShortWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.LowerThan((short)2, (short) 1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert LowerThan[short](message, args) has failed");
        }


        #endregion

        #region byte

        [Test]
        [Row((byte) 0, (byte) 1, Description = "Valid Case")]
        [Row((byte) 2, (byte) 1, Description = "Invalid Case", ExpectedException = typeof (AssertionException))]
        public void LowerThanByteParamTest(byte left, byte right)
        {
            OldAssert.LowerThan(left, right);
        }

        [Test]
        public void LowerThanByteWithMessage()
        {
            OldAssert.LowerThan((byte)0, (byte)1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LowerThanByteWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.LowerThan((byte) 2, (byte) 1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert LowerThan[byte](message) has failed");
        }

        [Test]
        public void LowerThanByteWithFormattedMessage()
        {
            OldAssert.LowerThan((byte)0, (byte)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LowerThanByteWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.LowerThan((byte) 2, (byte) 1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert LowerThan[byte](message, args) has failed");
        }

        #endregion

        #region double

        [Test]
        [Row((double) 0, (double) 1, Description = "Valid Case")]
        [Row((double) 3, (double) 1, Description = "Invalid Case", ExpectedException = typeof (AssertionException))]
        public void LowerThanDoubleParamTest(double left, double right)
        {
            OldAssert.LowerThan(left, right);
        }

        [Test]
        public void LowerThanDoubleWithMessage()
        {
            OldAssert.LowerThan((double) 0, (double) 1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LowerThanDoubleWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.LowerThan((double) 3, (double) 1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert LowerThan[double](message) has failed");
        }

        [Test]
        public void LowerThanDoubleWithFormattedMessage()
        {
            OldAssert.LowerThan((double) 0, (double) 1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LowerThanDoubleWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.LowerThan((double) 3, (double) 1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert LowerThan[double](message, args) has failed");
        }


        #endregion

        #region long

        [Test]
        [Row((long)0, (long) 1, Description = "Valid Case")]
        [Row((long)3, (long) 1, Description = "Invalid Case", ExpectedException = typeof (AssertionException))]
        public void LowerThanLongParamTest(long left, long right)
        {
            OldAssert.LowerThan(left, right);
        }

        [Test]
        public void LowerThanLongWithMessage()
        {
            OldAssert.LowerThan((long)0, (long) 1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LowerThanLongWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.LowerThan((long)3, (long) 1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert LowerThan[long](message) has failed");
        }

        [Test]
        public void LowerThanLongWithFormattedMessage()
        {
            OldAssert.LowerThan((long)0, (long) 1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LowerThanLongWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.LowerThan((long)3, (long) 1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert LowerThan[long](message, args) has failed");
        }


        #endregion

        #region float

        [Test]
        [Row((float)0, (float)2, Description = "Valid Case")]
        [Row((float)3, (float)2, Description = "Invalid Case", ExpectedException = typeof (AssertionException))]
        public void LowerThanFloatParamTest(float left, float right)
        {
            OldAssert.LowerThan(left, right);
        }

        [Test]
        public void LowerThanFloatWithMessage()
        {
            OldAssert.LowerThan((float)0, (float)2, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LowerThanFloatWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.LowerThan((float)3, (float)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert LowerThan[float](message) has failed");
        }

        [Test]
        public void LowerThanFloatWithFormattedMessage()
        {
            OldAssert.LowerThan((float)0, (float)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LowerThanFloatWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.LowerThan((float)3, (float)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert LowerThan[float](message, args) has failed");
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
            OldAssert.LowerThan((IComparable)left, (IComparable)right);
        }

        [Test]
        public void LowerThanIComparableWithMessage()
        {
            OldAssert.LowerThan((IComparable)"aaaa", (IComparable)"zzzz", EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LowerThanIComparableWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.LowerThan((IComparable)"zzzz", (IComparable)"aaaa", EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert LowerThan[IComparable](message) has failed");
        }

        [Test]
        public void LowerThanIComparableWithFormattedMessage()
        {
            OldAssert.LowerThan((IComparable)"aaaa", (IComparable)"zzzz", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LowerThanIComparableWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.LowerThan((IComparable)"zzzz", (IComparable)"aaaa", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert LowerThan[IComparable](message, args) has failed");
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
            OldAssert.Less(left, right);
        }

        [Test]
        public void LessIntWithMessage()
        {
            OldAssert.Less(0, 1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LessIntWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.Less(2, 1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert Less[int](message) has failed");
        }

        [Test]
        public void LessIntWithFormattedMessage()
        {
            OldAssert.Less(0, 1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LessIntWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.Less(2, 1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert Less[int](message, args) has failed");
        }

        #endregion

        #region uint

        [Test]
        [Row((uint)0, (uint)1, Description = "Valid Case")]
        [Row((uint)2, (uint)1, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void LessUIntParamTest(uint left, uint right)
        {
            OldAssert.Less(left, right);
        }

        [Test]
        public void LessUIntWithMessage()
        {
            OldAssert.Less((uint)0, (uint)1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LessUIntWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.Less((uint)2, (uint)1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert Less[uint](message) has failed");
        }

        [Test]
        public void LessUIntWithFormattedMessage()
        {
            OldAssert.Less((uint)0, (uint)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LessUIntWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.Less((uint)2, (uint)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert Less[uint](message, args) has failed");
        }


        #endregion

        #region decimal

        [Test]
        [Row(0, 1, Description = "Valid Case")]
        [Row(2, 1, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void LessDecimalParamTest(decimal left, decimal right)
        {
            OldAssert.Less(left, right);
        }

        [Test]
        public void LessDecimalWithMessage()
        {
            OldAssert.Less((decimal)0, (decimal)1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LessDecimalWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.Less((decimal)2, (decimal)1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert Less[decimal](message) has failed");
        }

        [Test]
        public void LessDecimalWithFormattedMessage()
        {
            OldAssert.Less((decimal)0, (decimal)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LessDecimalWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.Less((decimal)2, (decimal)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert Less[decimal](message, args) has failed");
        }

        #endregion

        #region double

        [Test]
        [Row((double)0, (double)1, Description = "Valid Case")]
        [Row((double)3, (double)1, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void LessDoubleParamTest(double left, double right)
        {
            OldAssert.Less(left, right);
        }

        [Test]
        public void LessDoubleWithMessage()
        {
            OldAssert.Less((double)0, (double)1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LessDoubleWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.Less((double)3, (double)1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert Less[double](message) has failed");
        }

        [Test]
        public void LessDoubleWithFormattedMessage()
        {
            OldAssert.Less((double)0, (double)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LessDoubleWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.Less((double)3, (double)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert Less[double](message, args) has failed");
        }


        #endregion

        #region long

        [Test]
        [Row((long)0, (long)1, Description = "Valid Case")]
        [Row((long)3, (long)1, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void LessLongParamTest(long left, long right)
        {
            OldAssert.Less(left, right);
        }

        [Test]
        public void LessLongWithMessage()
        {
            OldAssert.Less((long)0, (long)1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LessLongWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.Less((long)3, (long)1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert Less[long](message) has failed");
        }

        [Test]
        public void LessLongWithFormattedMessage()
        {
            OldAssert.Less((long)0, (long)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LessLongWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.Less((long)3, (long)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert Less[long](message, args) has failed");
        }


        #endregion

        #region float

        [Test]
        [Row((float)0, (float)2, Description = "Valid Case")]
        [Row((float)3, (float)2, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void LessFloatParamTest(float left, float right)
        {
            OldAssert.Less(left, right);
        }

        [Test]
        public void LessFloatWithMessage()
        {
            OldAssert.Less((float)0, (float)2, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LessFloatWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.Less((float)3, (float)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert Less[float](message) has failed");
        }

        [Test]
        public void LessFloatWithFormattedMessage()
        {
            OldAssert.Less((float)0, (float)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LessFloatWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.Less((float)3, (float)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert Less[float](message, args) has failed");
        }


        #endregion

        #region IComparable

        [Test]
        [Row("aaaa","zzzz", Description = "Valid Case")]
        [Row(1, 2, Description = "Valid Case")]
        [Row("zzzz", "aaaa", Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void LessIComparableParamTest(object left, object right)
        {
            OldAssert.Less((IComparable)left, (IComparable)right);
        }

        [Test]
        public void LessIComparableWithMessage()
        {
            OldAssert.Less((IComparable)"aaaa", (IComparable)"zzzz", EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LessIComparableWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.Less((IComparable)"zzzz", (IComparable)"aaaa", EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert Less[IComparable](message) has failed");
        }

        [Test]
        public void LessIComparableWithFormattedMessage()
        {
            OldAssert.Less((IComparable)"aaaa", (IComparable)"zzzz", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LessIComparableWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.Less((IComparable)"zzzz", (IComparable)"aaaa", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert Less[IComparable](message, args) has failed");
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
            OldAssert.LowerEqualThan(left, right);
        }

        [Test]
        public void LowerEqualThanIntWithMessage()
        {
            OldAssert.LowerEqualThan((int) 0, (int) 1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LowerEqualThanIntWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.LowerEqualThan((int) 2, (int) 1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert LowerEqualThan[int](message) has failed");
        }

        [Test]
        public void LowerEqualThanIntWithFormattedMessage()
        {
            OldAssert.LowerEqualThan((int) 0, (int) 1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LowerEqualThanIntWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.LowerEqualThan((int) 2, (int) 1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert LowerEqualThan[int](message, args) has failed");
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
            OldAssert.LowerEqualThan(left, right);
        }

        [Test]
        public void LowerEqualThanShortWithMessage()
        {
            OldAssert.LowerEqualThan((short) 0, (short) 1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LowerEqualThanShortWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.LowerEqualThan((short) 2, (short) 1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert LowerEqualThan[short](message) has failed");
        }

        [Test]
        public void LowerEqualThanShortWithFormattedMessage()
        {
            OldAssert.LowerEqualThan((short) 0, (short) 1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LowerEqualThanShortWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.LowerEqualThan((short) 2, (short) 1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert LowerEqualThan[short](message, args) has failed");
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
            OldAssert.LowerEqualThan(left, right);
        }

        [Test]
        public void LowerEqualThanByteWithMessage()
        {
            OldAssert.LowerEqualThan((byte)0,(byte)1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LowerEqualThanByteWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.LowerEqualThan((byte)2,(byte)1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert LowerEqualThan[byte](message) has failed");
        }

        [Test]
        public void LowerEqualThanByteWithFormattedMessage()
        {
            OldAssert.LowerEqualThan((byte)0,(byte)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LowerEqualThanByteWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.LowerEqualThan((byte)2,(byte)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert LowerEqualThan[byte](message, args) has failed");
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
            OldAssert.LowerEqualThan(left, right);
        }

        [Test]
        public void LowerEqualThanLongWithMessage()
        {
            OldAssert.LowerEqualThan((long) 0, (long)1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LowerEqualThanLongWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.LowerEqualThan((long) 2, (long)1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert LowerEqualThan[long](message) has failed");
        }

        [Test]
        public void LowerEqualThanLongWithFormattedMessage()
        {
            OldAssert.LowerEqualThan((long) 0, (long)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LowerEqualThanLongWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.LowerEqualThan((long) 2, (long)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert LowerEqualThan[long](message, args) has failed");
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
            OldAssert.LowerEqualThan(left, right);
        }

        [Test]
        public void LowerEqualThanDoubleWithMessage()
        {
            OldAssert.LowerEqualThan((double)0,(double)1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LowerEqualThanDoubleWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.LowerEqualThan((double)2,(double)1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert LowerEqualThan[double](message) has failed");
        }

        [Test]
        public void LowerEqualThanDoubleWithFormattedMessage()
        {
            OldAssert.LowerEqualThan((double)0,(double)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LowerEqualThanDoubleWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.LowerEqualThan((double)2,(double)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert LowerEqualThan[double](message, args) has failed");
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
            OldAssert.LowerEqualThan(left, right);
        }

        [Test]
        public void LowerEqualThanFloatWithMessage()
        {
            OldAssert.LowerEqualThan((float)0, (float)1, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LowerEqualThanFloatWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.LowerEqualThan((float)2, (float)1, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert LowerEqualThan[float](message) has failed");
        }

        [Test]
        public void LowerEqualThanFloatWithFormattedMessage()
        {
            OldAssert.LowerEqualThan((float)0, (float)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LowerEqualThanFloatWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.LowerEqualThan((float)2, (float)1, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert LowerEqualThan[float](message, args) has failed");
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
            OldAssert.LowerEqualThan((IComparable)left, (IComparable)right);
        }

        [Test]
        public void LowerEqualThanIComparableWithMessage()
        {
            OldAssert.LowerEqualThan((IComparable)"aaaa", (IComparable)"zzzz", EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void LowerEqualThanIComparableWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.LowerEqualThan((IComparable)"zzzz", (IComparable)"aaaa", EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert LowerEqualThan[IComparable](message) has failed");
        }

        [Test]
        public void LowerEqualThanIComparableWithFormattedMessage()
        {
            OldAssert.LowerEqualThan((IComparable)"aaaa", (IComparable)"zzzz", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void LowerEqualThanIComparableWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.LowerEqualThan((IComparable)"zzzz", (IComparable)"aaaa", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert LowerEqualThan[IComparable](message, args) has failed");
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
            OldAssert.GreaterThan(left, right);
        }

        [Test]
        public void GreaterThanIntWithMessage()
        {
            OldAssert.GreaterThan((int)1, (int)0, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void GreaterThanIntWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.GreaterThan((int)1, (int)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert GreaterThan[int](message) has failed");
        }

        [Test]
        public void GreaterThanIntWithFormattedMessage()
        {
            OldAssert.GreaterThan((int)1, (int)0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void GreaterThanIntWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.GreaterThan((int)1, (int)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert GreaterThan[int](message, args) has failed");
        }


        #endregion

        #region short

        [Test]
        [Row(1, 0, Description = "Valid Case (Greater Than)")]
        [Row(1, 2, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void GreaterThanShortParamTest(short left, short right)
        {
            OldAssert.GreaterThan(left, right);
        }

        [Test]
        public void GreaterThanShortWithMessage()
        {
            OldAssert.GreaterThan((short)1, (short)0, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void GreaterThanShortWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.GreaterThan((short)1, (short)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert GreaterThan[short](message) has failed");
        }

        [Test]
        public void GreaterThanShortWithFormattedMessage()
        {
            OldAssert.GreaterThan((short)1, (short)0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void GreaterThanShortWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.GreaterThan((short)1, (short)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert GreaterThan[short](message, args) has failed");
        }

        #endregion

        #region byte

        [Test]
        [Row(1, 0, Description = "Valid Case (Greater Than)")]
        [Row(1, 2, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void GreaterThanByteParamTest(byte left, byte right)
        {
            OldAssert.GreaterThan(left, right);
        }

        [Test]
        public void GreaterThanByteWithMessage()
        {
            OldAssert.GreaterThan((byte)1, (byte)0, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void GreaterThanByteWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.GreaterThan((byte)1, (byte)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert GreaterThan[byte](message) has failed");
        }

        [Test]
        public void GreaterThanByteWithFormattedMessage()
        {
            OldAssert.GreaterThan((byte)1, (byte)0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void GreaterThanByteWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.GreaterThan((byte)1, (byte)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert GreaterThan[byte](message, args) has failed");
        }

        #endregion

        #region long

        [Test]
        [Row(1, 0, Description = "Valid Case (Greater Than)")]
        [Row(1, 2, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void GreaterThanLongParamTest(long left, long right)
        {
            OldAssert.GreaterThan(left, right);
        }

        [Test]
        public void GreaterThanLongWithMessage()
        {
            OldAssert.GreaterThan((long)1, (long)0, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void GreaterThanLongWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.GreaterThan((long)1, (long)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert GreaterThan[long](message) has failed");
        }

        [Test]
        public void GreaterThanLongWithFormattedMessage()
        {
            OldAssert.GreaterThan((long)1, (long)0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void GreaterThanLongWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.GreaterThan((long)1, (long)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert GreaterThan[long](message, args) has failed");
        }


        #endregion

        #region double

        [Test]
        [Row(1, 0, Description = "Valid Case (Greater Than)")]
        [Row(1, 2, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void GreaterThanDoubleParamTest(double left, double right)
        {
            OldAssert.GreaterThan(left, right);
        }

        [Test]
        public void GreaterThanDoubleWithMessage()
        {
            OldAssert.GreaterThan((double)1, (double)0, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void GreaterThanDoubleWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.GreaterThan((double)1, (double)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert GreaterThan[double](message) has failed");
        }

        [Test]
        public void GreaterThanDoubleWithFormattedMessage()
        {
            OldAssert.GreaterThan((double)1, (double)0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void GreaterThanDoubleWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.GreaterThan((double)1, (double)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert GreaterThan[double](message, args) has failed");
        }


        #endregion

        #region float

        [Test]
        [Row(1, 0, Description = "Valid Case (Greater Than)")]
        [Row(1, 2, Description = "Invalid Case", ExpectedException = typeof(AssertionException))]
        public void GreaterThanFloatParamTest(float left, float right)
        {
            OldAssert.GreaterThan(left, right);
        }

        [Test]
        public void GreaterThanFloatWithMessage()
        {
            OldAssert.GreaterThan((float)1, (float)0, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void GreaterThanFloatWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.GreaterThan((float)1, (float)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert GreaterThan[float](message) has failed");
        }

        [Test]
        public void GreaterThanFloatWithFormattedMessage()
        {
            OldAssert.GreaterThan((float)1, (float)0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void GreaterThanFloatWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.GreaterThan((float)1, (float)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert GreaterThan[float](message, args) has failed");
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
            OldAssert.GreaterThan((IComparable)left, (IComparable)right);
        }

        [Test]
        public void GreaterThanIComparableWithMessage()
        {
            OldAssert.GreaterThan((IComparable)"z", (IComparable)"a", EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void GreaterThanIComparableWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.GreaterThan((IComparable)"a", (IComparable)"z", EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert GreaterThan[IComparable](message) has failed");
        }

        [Test]
        public void GreaterThanIComparableWithFormattedMessage()
        {
            OldAssert.GreaterThan((IComparable)"z", (IComparable)"a", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void GreaterThanIComparableWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.GreaterThan((IComparable)"a", (IComparable)"z", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert GreaterThan[IComparable](message, args) has failed");
        }


        #endregion

        #endregion

        #region Greater

        [Test]
        public void GreaterInt()
        {
            OldAssert.Greater(1, 0);
        }

        [Test]
        public void GreaterIntWithMessage()
        {
            OldAssert.Greater(1, 0, "Int is not greater");
        }

        [Test]
        public void GreaterIntWithMessageAndArgs()
        {
            OldAssert.Greater(1, 0, "{0} is not greater than {1}", 1, 0);
        }

        [Test]
        public void GreaterUint()
        {
            OldAssert.Greater((uint)1, (uint)0);
        }

        [Test]
        public void GreaterUintWithMessage()
        {
            OldAssert.Greater((uint)1, (uint)0, "Int is not greater");
        }

        [Test]
        public void GreaterUintWithMessageAndArgs()
        {
            OldAssert.Greater((uint)1, (uint)0, "{0} is not greater than {1}", 1, 0);
        }

        [Test]
        public void GreaterShort()
        {
            OldAssert.Greater((short)1, (short)0);
        }

        [Test]
        public void GreaterShortWithMessage()
        {
            OldAssert.Greater((short)1, (short)0, "Short is not greater");
        }

        [Test]
        public void GreaterShortWithMessageAndArgs()
        {
            OldAssert.Greater((short)1, (short)0, "{0} is not greater than {1}", (short)1, (short)0);
        }
        
        [Test]
        public void GreaterByte()
        {
            OldAssert.Greater((byte)1, (byte)0);
        }

        [Test]
        public void GreaterByteWithMessage()
        {
            OldAssert.Greater((byte)1, (byte)0, "Byte is not greater");
        }

        [Test]
        public void GreaterByteWithMessageAndArgs()
        {
            OldAssert.Greater((byte)1, (byte)0, "{0} is not greater than {1}", (byte)0, (byte)1);
        }

        [Test]
        public void GreaterDecimal()
        {
            OldAssert.Greater((decimal)1, (decimal)0);
        }
        
        [Test]
        public void GreaterDecimalWithMessage()
        {
            OldAssert.Greater((decimal)1, (decimal)0, "Decimal is not greater");
        }

        [Test]
        public void GreaterDecimalWithMessageAndArgs()
        {
            OldAssert.Greater((decimal)1, (decimal)0, "{0} is not greater than {1}", (decimal)1, (decimal)0);
        }

        [Test]
        public void GreaterLong()
        {
            OldAssert.Greater((long)1, (long)0);
        }

        [Test]
        public void GreaterLongWithMessage()
        {
            OldAssert.Greater((long)1, (long)0, "Long is not greater");
        }

        [Test]
        public void GreaterLongWithMessageAndArgs()
        {
            OldAssert.Greater((long)1, (long)0, "{0} is not greater than {1}", (long)1, (long)0);
        }

        [Test]
        public void GreaterDouble()
        {
            OldAssert.Greater((double)1, (double)0);
        }

        [Test]
        public void GreaterDoubleWithMessage()
        {
            OldAssert.Greater((double)1, (double)0, "Double is not greater");
        }

        [Test]
        public void GreaterDoubleWithMessageAndArgs()
        {
            OldAssert.Greater((double)1, (double)0, "{0} is not greater than {1}", (double)1, (double)0);
        }

        [Test]
        public void GreaterFloat()
        {
            OldAssert.Greater((float)1, (float)0);
        }

        [Test]
        public void GreaterFloatWithMessage()
        {
            OldAssert.Greater((float)1, (float)0, "Float is not greater");
        }

        [Test]
        public void GreaterFloatWithMessageAndArgs()
        {
            OldAssert.Greater((float)1, (float)0, "{0} is not greater than {1}", (float)1, (float)0);
        }

        [Test]
        public void GreaterIComparable()
        {
            OldAssert.Greater(DateTime.Now, new DateTime(2000, 1, 1));
        }

        [Test]
        public void GreaterIComparableWithMessage()
        {
            OldAssert.Greater(DateTime.Now, new DateTime(2000, 1, 1), "DateTime is not greater");
        }

        [Test]
        public void GreaterIComparableWithMessageAndArgs()
        {
            DateTime actual = DateTime.Now;
            DateTime expected = new DateTime(2000, 1, 1);
            OldAssert.Greater(actual, expected, "{0} is not greater than {1}", actual, expected);
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
            OldAssert.GreaterEqualThan(left, right);
        }

        [Test]
        public void GreaterEqualThanIntWithMessage()
        {
            OldAssert.GreaterEqualThan((int)1, (int)0, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void GreaterEqualThanIntWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.GreaterEqualThan((int)1, (int)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert GreaterEqualThan[int](message) has failed");
        }

        [Test]
        public void GreaterEqualThanIntWithFormattedMessage()
        {
            OldAssert.GreaterEqualThan((int)1, (int)0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void GreaterEqualThanIntWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.GreaterEqualThan((int)1, (int)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert GreaterEqualThan[int](message, args) has failed");
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
            OldAssert.GreaterEqualThan(left, right);
        }

        [Test]
        public void GreaterEqualThanShortWithMessage()
        {
            OldAssert.GreaterEqualThan((short)1, (short)0, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void GreaterEqualThanShortWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.GreaterEqualThan((short)1, (short)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert GreaterEqualThan[short](message) has failed");
        }

        [Test]
        public void GreaterEqualThanShortWithFormattedMessage()
        {
            OldAssert.GreaterEqualThan((short)1, (short)0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void GreaterEqualThanShortWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.GreaterEqualThan((short)1, (short)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert GreaterEqualThan[short](message, args) has failed");
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
            OldAssert.GreaterEqualThan(left, right);
        }

        [Test]
        public void GreaterEqualThanByteWithMessage()
        {
            OldAssert.GreaterEqualThan((byte)1, (byte)0, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void GreaterEqualThanByteWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.GreaterEqualThan((byte)1, (byte)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert GreaterEqualThan[byte](message) has failed");
        }

        [Test]
        public void GreaterEqualThanByteWithFormattedMessage()
        {
            OldAssert.GreaterEqualThan((byte)1, (byte)0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void GreaterEqualThanByteWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.GreaterEqualThan((byte)1, (byte)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert GreaterEqualThan[byte](message, args) has failed");
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
            OldAssert.GreaterEqualThan(left, right);
        }

        [Test]
        public void GreaterEqualThanLongWithMessage()
        {
            OldAssert.GreaterEqualThan((long)1, (long)0, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void GreaterEqualThanLongWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.GreaterEqualThan((long)1, (long)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert GreaterEqualThan[long](message) has failed");
        }

        [Test]
        public void GreaterEqualThanLongWithFormattedMessage()
        {
            OldAssert.GreaterEqualThan((long)1, (long)0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void GreaterEqualThanLongWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.GreaterEqualThan((long)1, (long)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert GreaterEqualThan[long](message, args) has failed");
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
            OldAssert.GreaterEqualThan(left, right);
        }

        [Test]
        public void GreaterEqualThanDoubleWithMessage()
        {
            OldAssert.GreaterEqualThan((double)1, (double)0, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void GreaterEqualThanDoubleWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.GreaterEqualThan((double)1, (double)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert GreaterEqualThan[double](message) has failed");
        }

        [Test]
        public void GreaterEqualThanDoubleWithFormattedMessage()
        {
            OldAssert.GreaterEqualThan((double)1, (double)0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void GreaterEqualThanDoubleWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.GreaterEqualThan((double)1, (double)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert GreaterEqualThan[double](message, args) has failed");
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
            OldAssert.GreaterEqualThan(left, right);
        }

        [Test]
        public void GreaterEqualThanFloatWithMessage()
        {
            OldAssert.GreaterEqualThan((float)1, (float)0, EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void GreaterEqualThanFloatWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.GreaterEqualThan((float)1, (float)2, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert GreaterEqualThan[float](message) has failed");
        }

        [Test]
        public void GreaterEqualThanFloatWithFormattedMessage()
        {
            OldAssert.GreaterEqualThan((float)1, (float)0, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void GreaterEqualThanFloatWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.GreaterEqualThan((float)1, (float)2, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert GreaterEqualThan[float](message, args) has failed");
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
            OldAssert.GreaterEqualThan((IComparable)left, (IComparable)right);
        }

        [Test]
        public void GreaterEqualThanIComparableWithMessage()
        {
            OldAssert.GreaterEqualThan((IComparable)"z", (IComparable)"a", EXPECTED_FAIL_MESSAGE);
        }

        [Test]
        public void GreaterEqualThanIComparableWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.GreaterEqualThan((IComparable)"a", (IComparable)"z", EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert GreaterEqualThan[IComparable](message) has failed");
        }

        [Test]
        public void GreaterEqualThanIComparableWithFormattedMessage()
        {
            OldAssert.GreaterEqualThan((IComparable)"z", (IComparable)"a", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void GreaterEqualThanIComparableWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.GreaterEqualThan((IComparable)"a", (IComparable)"z", TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert GreaterEqualThan[IComparable](message, args) has failed");
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
            OldAssert.In(test, dic);
        }

        #endregion

        #region IList

        [Test]
        public void InListTestNull()
        {
            ArrayList list = new ArrayList();
            list.Add(null);
            OldAssert.In(null, list);
        }
        [Test]
        public void InList()
        {
            ArrayList list = new ArrayList();
            string test = "test";
            list.Add(test);
            OldAssert.In(test, list);
        }

        #endregion

        #region IEnumerable
     
        [Test]
        public void InEnumerableTestNull()
        {
            ArrayList list = new ArrayList();
            list.Add(null);
            OldAssert.In(null, (IEnumerable)list);
        }

        [Test]
        public void InEnumerable()
        {
            ArrayList list = new ArrayList();
            string test = "test";
            list.Add(test);
            OldAssert.In(test, (IEnumerable)list);
        }

        [Test]
        public void In_ValueEquality()
        {
            string[] stringArray = {"item1", "item2"};
            IEnumerable enumerableStringArray = stringArray;
            string item1 = string.Format("item{0}", 1);
            OldAssert.In(item1, enumerableStringArray);
        }

        [Test]
        public void In_NullItem()
        {
            string[] stringArray = { "item1", null, "item2" };
            IEnumerable enumerableStringArray = stringArray;
            OldAssert.In(null, enumerableStringArray);
        }

        [Test]
        public void InEnumerableWithMessage()
        {
            ArrayList list = new ArrayList();
            string test = "test";
            list.Add(test);

            OldAssert.In(test, (IEnumerable)list, "InEnumerable Failed");
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void InEnumerableFail()
        {
            ArrayList list = new ArrayList();
            string test = "test";
            list.Add(test);

            OldAssert.In("someOtherObject", (IEnumerable)list);
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
                OldAssert.In("someOtherObject", (IEnumerable)list, "InEnumerable Failed");
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf("InEnumerable Failed") >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert InEnumerable(message) has failed");
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
            OldAssert.NotIn(test + "modified", dic);
        }

        #endregion

        #region IList

        [Test]
        public void NotInListTestNull()
        {
            OldAssert.NotIn(null, new ArrayList());
        }
        [Test]
        public void NotInList()
        {
            ArrayList list = new ArrayList();
            string test = "test";
            list.Add(test);
            OldAssert.NotIn(test + "modified", list);
        }

        #endregion

        #region IEnumerable

        [Test]
        public void NotInEnumerableTestNull()
        {
            OldAssert.NotIn(null, (IEnumerable)new ArrayList());
        }

        [Test]
        public void NotInEnumerable()
        {
            ArrayList list = new ArrayList();
            string test = "test";
            list.Add(test);
            OldAssert.NotIn(test + "modified", (IEnumerable)list);
        }

        #endregion

        #endregion

        #endregion

        #region IsEmpty
        //NUnit Code
        [Test]
        public void IsEmpty()
        {
            OldAssert.IsEmpty("", "Failed on empty String");
            OldAssert.IsEmpty(new int[0], "Failed on empty Array");
            OldAssert.IsEmpty(new ArrayList(), "Failed on empty ArrayList");
            OldAssert.IsEmpty(new Hashtable(), "Failed on empty Hashtable");
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void IsEmptyFailsOnString()
        {
            OldAssert.IsEmpty("Hi!");
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void IsEmptyFailsOnNullString()
        {
            OldAssert.IsEmpty((string)null);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void IsEmptyFailsOnNonEmptyArray()
        {
            OldAssert.IsEmpty(new int[] { 1, 2, 3 });
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

            OldAssert.IsNotEmpty("MbUnit", "Failed on non empty String");
            OldAssert.IsNotEmpty(new int[1] { 1 }, "Failed on non empty Array");
            OldAssert.IsNotEmpty(arr, "Failed on non empty ArrayList");
            OldAssert.IsNotEmpty(hash, "Failed on empty Hashtable");
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void IsNotEmptyFailsOnString()
        {
            OldAssert.IsNotEmpty(string.Empty);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void IsNotEmptyFailsOnNonEmptyArray()
        {
            OldAssert.IsNotEmpty(new int[0] { });
        }


        #endregion

        #region IsNan
        //Nunit Code
        [Test]
        public void IsNaN()
        {
            OldAssert.IsNaN(double.NaN);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void IsNaNFails()
        {
            OldAssert.IsNaN(10.0);
        }

        #endregion

        #region Contains

        [Test]
        public void Contains()
        {
            string s = "MbUnit";
            string contain = "Unit";
            OldAssert.Contains(s, contain);
        }

        #endregion

        #region Equals

        [Test, ExpectedException(typeof(AssertionException))]
        public void Equals()
        {
            OldAssert.Equals(null, null);
        }

        #endregion

        #region AreSame

        [Test]
        public void AreSame()
        {
            object objectA = new object();

            OldAssert.AreSame(objectA, objectA);
        }
        
        [Test]
        public void AreSameDifferentReference()
        {
            object objectA = new object();
            object objectB = objectA;

            OldAssert.AreSame(objectA, objectB);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreSameInteger_Fail()
        {
            OldAssert.AreSame(0, 0);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreSameInteger_ImplicitBox_Fail()
        {
            int intA = 1;

            // Due to boxing, these will not refer to the same object
            OldAssert.AreSame(intA, intA);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreSameInteger_ExplicitBoxToValue_Fail()
        {
            int intA = 1;
            object objectA = (object) intA;
            
            // Due to boxing, these will not refer to the same object
            OldAssert.AreSame(objectA, intA);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreSameInteger_ExplicitBoxToSameValue_Fail()
        {
            int intA = 1;
            object objectA = (object)intA;
            object objectB = (object)intA;

            // These will not refer to the same object
            OldAssert.AreSame(objectA, objectB);
        }

        [Test]
        public void AreSame_String()
        {
            // Both strings will be interned, so are the same object
            OldAssert.AreSame("A String", "A String");
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreSame_NonInterned_NewString_Fail()
        {
            string stringA = "A String";
            string stringB = new String(new char[] { 'A', ' ', 'S', 't', 'r', 'i', 'n', 'g' });

            OldAssert.AreSame(stringA, stringB);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreSame_NonInterned_ConcatString_Fail()
        {
            // JIT will not intern the concat string, so these are not the same
            string stringA = "A String";
            string stringB = String.Concat("A ","String");

            OldAssert.AreSame(stringA, stringB);
        }

        [Test]
        public void AreSame_InternedString()
        {
            string stringA = "A String";
            string stringB = String.Intern(String.Concat("A ", "String"));

            OldAssert.AreSame(stringA, stringB);
        }

        [Test]
        public void AreSame_Array()
        {
            int[] arrayA = new int[] { 1, 2, 3 };
            int[] arrayB = arrayA;

            OldAssert.AreSame(arrayA, arrayB);
        } 
        
        [Test, ExpectedException(typeof(AssertionException))]
        public void AreSame_Array_Fail()
        {
            int[] arrayA = new int[] { 1, 2, 3 };
            int[] arrayB = new int[] { 1, 2, 3 };

            OldAssert.AreSame(arrayA, arrayB);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreSame_ArrayClone_Fail()
        {
            int[] arrayA = new int[] { 1, 2, 3 };
            int[] arrayB = (int[]) arrayA.Clone();

            OldAssert.AreSame(arrayA, arrayB);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreSameFail()
        {
            object objectA = new object();
            object objectB = new object();

            OldAssert.AreSame(objectA, objectB);
        }

        [Test]
        public void AreSameNull()
        {
            OldAssert.AreSame(null, null);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreSameExpectedNull()
        {
            OldAssert.AreSame(null, new object());
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreSameActualNull()
        {
            OldAssert.AreSame(new object(), null);
        }

        [Test]
        public void AreSameWithMessage()
        {
            object objectA = new object();
            OldAssert.AreSame(objectA, objectA, "Assert AreSame(message) has failed");
        }

        [Test]
        public void AreSameWithMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.AreSame(new object(), new object(), EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert AreSame(message) has failed");
        }

        [Test]
        public void AreSameWithFormattedMessage()
        {
            object objectA = new object();
            OldAssert.AreSame(objectA, objectA, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void AreSameWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                OldAssert.AreSame(new object(), new object(), TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert AreSame(message, args) has failed");
        }

        #endregion

        #region AreNotSame

        [Test]
        public void AreNotSame()
        {
            object objectA = new object();
            object objectB = new object();

            OldAssert.AreNotSame(objectA, objectB);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void AreNotSameFail()
        {
            object objectA = new object();
            object objectB = objectA;

            OldAssert.AreNotSame(objectA, objectB);
        }

        [Test]
        public void AreNotSameWithMessage()
        {
            OldAssert.AreNotSame(new object(), new object(), "Assert AreNotSame(message) has failed");
        }

        [Test]
        public void AreNotSameWithMessageFail()
        {
            bool asserted = false;

            try
            {
                object objectA = new object();
                OldAssert.AreNotSame(objectA, objectA, EXPECTED_FAIL_MESSAGE);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FAIL_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert AreNotSame(message) has failed");
        }

        [Test]
        public void AreNotSameWithFormattedMessage()
        {
            OldAssert.AreNotSame(new object(), new object(), TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
        }

        [Test]
        public void AreNotSameWithFormattedMessageFail()
        {
            bool asserted = false;

            try
            {
                object objectA = new object();
                OldAssert.AreNotSame(objectA, objectA, TEST_FORMAT_STRING, TEST_FORMAT_STRING_PARAM1, TEST_FORMAT_STRING_PARAM2);
            }
            catch (AssertionException ex)
            {
                OldAssert.IsTrue(ex.Message.IndexOf(EXPECTED_FORMATTED_MESSAGE) >= 0);
                asserted = true;
            }

            if (!asserted)
                OldAssert.Fail("Assert AreNotSame(message, args) has failed");
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
