using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using TestDriven.UnitTesting.Exceptions;

    [TestFixture]
    public class StringAssert_Test
    {
        #region AreEqual
        [Test]
        public void AreEqualIgnoreCase()
        {
            StringAssert.AreEqualIgnoreCase("hello", "HELLO");
        }

        #endregion

        #region Contains
        [Test]
        public void DoesNotContain()
        {
            StringAssert.DoesNotContain("hello", 'k');
        }

         #endregion

        #region Emptiness
        [Test]
        public void IsEmpty()
        {
            StringAssert.IsEmpty("");
        }

        [Test]
        public void IsNotEmpty()
        {
            StringAssert.IsNonEmpty(" ");
        }
           
        #endregion
    }

