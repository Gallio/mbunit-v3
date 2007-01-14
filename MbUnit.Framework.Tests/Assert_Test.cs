using System;
using System.Collections;
using MbUnit.Framework;
using MbUnit.Core.Exceptions;


namespace MbUnit.Framework.Tests.Asserts
{
    [TestFixture]
    public class Assert_Test
    {
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
        public void Ignore()
        {
            Assert.Ignore("Because I want to");
        }

        [Test]
        public void Fail()
        {
            bool bolPass = true;
            try
            {
                Assert.Fail();
            }
            catch (AssertionException ex)
            {
                bolPass = false;
            }

            if (bolPass == true)
            {
                Assert.IsFalse(true, "Assert fail has failed");
            }

        }

        [Test]
        public void IsNull()
        {
            Assert.IsNull(null);
        }

        [Test]
        public void IsNotNull()
        {
            Assert.IsNotNull("hello");
        }

        [Test]
        public void IsTrue()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void IsFalse()
        {
            Assert.IsFalse(false);
        }
        #endregion

        #region AreEqual
        [Test]
        public void AreEqualInt()
        {
            Assert.AreEqual(0, 0);
        }

        [Test]
        public void AreEqualIntDelta()
        {
            Assert.AreEqual(0, 1, 1);
        }

        [Test]
        public void AreEqualString()
        {
            Assert.AreEqual("hello", "hello");
        }

        [Test]
        public void AreEqualDecimal()
        {
            Decimal l = 0;
            Decimal r = 0;
            Assert.AreEqual(l, r);
        }

        [Test]
        public void AreEqualDoubleDelta()
        {
            Assert.AreEqual(0.0, 1.0, 1.0);
        }

        [Test]
        public void AreEqualFloatDelta()
        {
            float l = 0;
            float r = 1;
            Assert.AreEqual(l, r, r);
        }

        [Test]
        public void AreSame()
        {
            ArrayList list = new ArrayList();
            Assert.AreSame(list, list);
        }
        #endregion

        #region AreNotEqual
//NUnit Tests
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
        public void NullNotEqualToNonNull()
        {
            Assert.AreNotEqual(null, 3);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void NullEqualsNull()
        {
            Assert.AreNotEqual(null, null);
        }

        [Test]
        public void ArraysNotEqual()
        {
            Assert.AreNotEqual(new object[] { 1, 2, 3 }, new int[] { 1, 3, 2 });
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void ArraysNotEqualFails()
        {
            Assert.AreNotEqual(new object[] { 1, 2, 3 }, new object[] { 1, 2, 3 });
        }

        [Test]
        public void UInt()
        {
            uint u1 = 5;
            uint u2 = 8;
            Assert.AreNotEqual(u1, u2);
        }

        #endregion

        #region Between

        #region int
        [Test]
        public void BetweenInt()
        {
            Assert.Between(1, 0, 2);
        }
        [Test]
        public void BetweenLowerEqualInt()
        {
            Assert.Between(1, 1, 2);
        }
        [Test]
        public void BetweenUpperEqualInt()
        {
            Assert.Between(2, 0, 2);
        }
        #endregion

        #region short
        [Test]
        public void BetweenShort()
        {
            Assert.Between((short)1, (short)0, (short)2);
        }
        [Test]
        public void BetweenLowerEqualShort()
        {
            Assert.Between((short)1, (short)1, (short)2);
        }
        [Test]
        public void BetweenUpperEqualShort()
        {
            Assert.Between((short)2, (short)0, (short)2);
        }
        #endregion

        #region byte
        [Test]
        public void BetweenByte()
        {
            Assert.Between((byte)1, (byte)0, (byte)2);
        }
        [Test]
        public void BetweenLowerEqualByte()
        {
            Assert.Between((byte)1, (byte)1, (byte)2);
        }
        [Test]
        public void BetweenUpperEqualByte()
        {
            Assert.Between((byte)2, (byte)0, (byte)2);
        }
        #endregion

        #region long
        [Test]
        public void BetweenLong()
        {
            Assert.Between((long)1, (long)0, (long)2);
        }
        [Test]
        public void BetweenLowerEqualLong()
        {
            Assert.Between((long)1, (long)1, (long)2);
        }
        [Test]
        public void BetweenUpperEqualLong()
        {
            Assert.Between((long)2, (long)0, (long)2);
        }
        #endregion

        #region double
        [Test]
        public void BetweenDouble()
        {
            Assert.Between((double)1, (double)0, (double)2);
        }
        [Test]
        public void BetweenLowerEqualDouble()
        {
            Assert.Between((double)1, (double)1, (double)2);
        }
        [Test]
        public void BetweenUpperEqualDouble()
        {
            Assert.Between((double)2, (double)0, (double)2);
        }
        #endregion

        #region float
        [Test]
        public void BetweenFloat()
        {
            Assert.Between((float)1, (float)0, (float)2);
        }
        [Test]
        public void BetweenLowerEqualFloat()
        {
            Assert.Between((float)1, (float)1, (float)2);
        }
        [Test]
        public void BetweenUpperEqualFloat()
        {
            Assert.Between((float)2, (float)0, (float)2);
        }
        #endregion

        #endregion

        #region NotBetween

        #region int
        [Test]
        public void NotBetweenIntInside()
        {
            Assert.NotBetween(4, 2, 3);
        }

        [Test]
        public void NotBetweenLowerInt()
        {
            Assert.NotBetween(0, 2, 3);
        }

        [Test]
        public void NotBetweenUpperInt()
        {
            Assert.NotBetween(4, 2, 3);
        }
        #endregion

        #region short
        [Test]
        public void NotBetweenShortInside()
        {
            Assert.NotBetween((short)4, (short)2, (short)3);
        }
        [Test]
        public void NotBetweenLowerShort()
        {
            Assert.NotBetween((short)0, (short)2, (short)3);
        }

        [Test]
        public void NotBetweenUpperShort()
        {
            Assert.NotBetween((short)4, (short)2, (short)3);
        }
        #endregion

        #region byte

        [Test]
        public void NotBetweenByteInside()
        {
            Assert.NotBetween((byte)4, (byte)2, (byte)3);
        }
        [Test]
        public void NotBetweenLowerByte()
        {
            Assert.NotBetween((byte)0, (byte)2, (byte)3);
        }

        [Test]
        public void NotBetweenUpperByte()
        {
            Assert.NotBetween((byte)4, (byte)2, (byte)3);
        }
        #endregion

        #region long
        [Test]
        public void NotBetweenLongInside()
        {
            Assert.NotBetween((long)4, (long)2, (long)3);
        }
        [Test]
        public void NotBetweenLowerLong()
        {
            Assert.NotBetween((long)0, (long)2, (long)3);
        }

        [Test]
        public void NotBetweenUpperLong()
        {
            Assert.NotBetween((long)4, (long)2, (long)3);
        }
        #endregion

        #region double
        [Test]
        public void NotBetweenDoubleInside()
        {
            Assert.NotBetween((double)4, (double)2, (double)3);
        }
        [Test]
        public void NotBetweenLowerDouble()
        {
            Assert.NotBetween((double)0, (double)2, (double)3);
        }

        [Test]
        public void NotBetweenUpperDouble()
        {
            Assert.NotBetween((double)4, (double)2, (double)3);
        }
        #endregion

        #region float
        [Test] 
        public void NotBetweenFloatInside()
        {
            Assert.NotBetween((float)4, (float)2, (float)3);
        }

        [Test]
        public void NotBetweenLowerFloat()
        {
            Assert.NotBetween((float)0, (float)2, (float)3);
        }

        [Test]
        public void NotBetweenUpperFloat()
        {
            Assert.NotBetween((float)4, (float)2, (float)3);
        }
        #endregion

        #endregion

        #region <, <=, >, >=
        #region LowerThan
        [Test]
        public void LowerThanInt()
        {
            Assert.LowerThan(0, 1);
        }
        [Test]
        public void LowerThanShort()
        {
            Assert.LowerThan((short)0, (short)1);
        }

        [Test]
        public void LowerThanByte()
        {
            Assert.LowerThan((byte)0, (byte)1);
        }
        [Test]
        public void LowerThanLong()
        {
            Assert.LowerThan((long)0, (long)1);
        }

        [Test]
        public void LowerThanDouble()
        {
            Assert.LowerThan((double)0, (double)1);
        }

        [Test]
        public void LowerThanFloat()
        {
            Assert.LowerThan((float)0, (float)1);
        }
        //[Test]
        //public void LowerThanFailEqualFloat()
        //{
        //    Assert.LowerThan((float)0, (float)0);
        //}
        //[Test]
        //public void LowerThanFailLessFloat()
        //{
        //    Assert.LowerThan((float)0, (float)-1);
        //}
        #endregion

        #region LowerEqualThan
        [Test]
        public void LowerEqualThanInt()
        {
            Assert.LowerEqualThan(0, 1);
        }
        [Test]
        public void LowerEqualThanEqualInt()
        {
            Assert.LowerEqualThan(0, 0);
        }

        [Test]
        public void LowerEqualThanShort()
        {
            Assert.LowerEqualThan((short)0, (short)1);
        }
        [Test]
        public void LowerEqualThanEqualShort()
        {
            Assert.LowerEqualThan((short)0, (short)0);
        }

        [Test]
        public void LowerEqualThanByte()
        {
            Assert.LowerEqualThan((byte)0, (byte)1);
        }
        [Test]
        public void LowerEqualThanEqualByte()
        {
            Assert.LowerEqualThan((byte)0, (byte)0);
        }

        [Test]
        public void LowerEqualThanLong()
        {
            Assert.LowerEqualThan((long)0, (long)1);
        }
        [Test]
        public void LowerEqualThanEqualLong()
        {
            Assert.LowerEqualThan((long)0, (long)0);
        }

        [Test]
        public void LowerEqualThanDouble()
        {
            Assert.LowerEqualThan((double)0, (double)1);
        }
        [Test]
        public void LowerEqualThanEqualDouble()
        {
            Assert.LowerEqualThan((double)0, (double)0);
        }

        [Test]
        public void LowerEqualThanFloat()
        {
            Assert.LowerEqualThan((float)0, (float)1);
        }
        [Test]
        public void LowerEqualThanEqualFloat()
        {
            Assert.LowerEqualThan((float)0, (float)0);
        }

        #endregion

        #region Less
        [Test]
        public void LessInt()
        {
            Assert.Less(0, 1);
        }
        [Test]
        public void LessShort()
        {
            Assert.Less((short)0, (short)1);
        }

        [Test]
        public void LessByte()
        {
            Assert.Less((byte)0, (byte)1);
        }
        [Test]
        public void LessLong()
        {
            Assert.Less((long)0, (long)1);
        }

        [Test]
        public void LessDouble()
        {
            Assert.Less((double)0, (double)1);
        }

        [Test]
        public void LessFloat()
        {
            Assert.Less((float)0, (float)1);
        }
        #endregion

        #region GreaterThan
        [Test]
        public void GreaterThanInt()
        {
            Assert.GreaterThan(1, 0);
        }

        [Test]
        public void GreaterThanShort()
        {
            Assert.GreaterThan((short)1, (short)0);
        }

        [Test]
        public void GreaterThanByte()
        {
            Assert.GreaterThan((byte)1, (byte)0);
        }

        [Test]
        public void GreaterThanLong()
        {
            Assert.GreaterThan((long)1, (long)0);
        }

        [Test]
        public void GreaterThanDouble()
        {
            Assert.GreaterThan((double)1, (double)0);
        }

        [Test]
        public void GreaterThanFloat()
        {
            Assert.GreaterThan((float)1, (float)0);
        }

        #endregion

        #region Greater
        [Test]
        public void GreaterInt()
        {
            Assert.Greater(1, 0);
        }

        [Test]
        public void GreaterShort()
        {
            Assert.Greater((short)1, (short)0);
        }

        [Test]
        public void GreaterByte()
        {
            Assert.Greater((byte)1, (byte)0);
        }

        [Test]
        public void GreaterLong()
        {
            Assert.Greater((long)1, (long)0);
        }

        [Test]
        public void GreaterDouble()
        {
            Assert.Greater((double)1, (double)0);
        }

        [Test]
        public void GreaterFloat()
        {
            Assert.Greater((float)1, (float)0);
        }

        #endregion

        #region GreaterEqualThan
        [Test]
        public void GreaterEqualThanInt()
        {
            Assert.GreaterEqualThan(1, 0);
        }

        [Test]
        public void GreaterEqualThanShort()
        {
            Assert.GreaterEqualThan((short)1, (short)0);
        }
        [Test]
        public void GreaterEqualThanEqualShort()
        {
            Assert.GreaterEqualThan((short)0, (short)0);
        }

        [Test]
        public void GreaterEqualThanByte()
        {
            Assert.GreaterEqualThan((byte)1, (byte)0);
        }

        [Test]
        public void GreaterEqualThanEqualByte()
        {
            Assert.GreaterEqualThan((byte)0, (byte)0);
        }

        [Test]
        public void GreaterEqualThanLong()
        {
            Assert.GreaterEqualThan((long)1, (long)0);
        }

        [Test]
        public void GreaterEqualThanEqualLong()
        {
            Assert.GreaterEqualThan((long)0, (long)0);
        }

        [Test]
        public void GreaterEqualThanDouble()
        {
            Assert.GreaterEqualThan((double)1, (double)0);
        }

        [Test]
        public void GreaterEqualThanEqualDouble()
        {
            Assert.GreaterEqualThan((double)0, (double)0);
        }

        [Test]
        public void GreaterEqualThanFloat()
        {
            Assert.GreaterEqualThan((float)1, (float)0);
        }

        [Test]
        public void GreaterEqualThanEqualFloat()
        {
            Assert.GreaterEqualThan((float)0, (float)0);
        }

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
    }
}
