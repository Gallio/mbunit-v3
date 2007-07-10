// MbUnit Test Framework
// 
// Copyright (c) 2004 Jonathan de Halleux
//
// This software is provided 'as-is', without any express or implied warranty. 
// 
// In no event will the authors be held liable for any damages arising from 
// the use of this software.
// Permission is granted to anyone to use this software for any purpose, 
// including commercial applications, and to alter it and redistribute it 
// freely, subject to the following restrictions:
//
//		1. The origin of this software must not be misrepresented; 
//		you must not claim that you wrote the original software. 
//		If you use this software in a product, an acknowledgment in the product 
//		documentation would be appreciated but is not required.
//
//		2. Altered source versions must be plainly marked as such, and must 
//		not be misrepresented as being the original software.
//
//		3. This notice may not be removed or altered from any source 
//		distribution.
//		
//		MbUnit HomePage: http://www.mbunit.org
//		Author: Jonathan de Halleux


using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

using MbUnit.Core.Exceptions;

namespace MbUnit.Framework
{
	/// <summary>
	/// Assertion class
	/// </summary>
	/// <include file="MbUnit.Framework.Doc.xml" path="doc/remarkss/remarks[@name='Assert']"/>
	public sealed class Assert
    {
        #region Static fields
        private static ArrayList warnings = ArrayList.Synchronized(new ArrayList());
        private static volatile int assertCount = 0;
        #endregion

        #region Private stuff
        /// <summary>
        /// The Equals method throws an AssertionException. This is done 
        /// to make sure there is no mistake by calling this function.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static new bool Equals(object a, object b)
        {
            throw new AssertionException("Assert.Equals should not be used for Assertions");
        }

        /// <summary>
        /// override the default ReferenceEquals to throw an AssertionException. This 
        /// implementation makes sure there is no mistake in calling this function 
        /// as part of Assert. 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static new void ReferenceEquals(object a, object b)
        {
            throw new AssertionException("Assert.ReferenceEquals should not be used for Assertions");
        }

        /// <summary>
        /// Checks the type of the object, returning true if
        /// the object is a numeric type.
        /// </summary>
        /// <param name="obj">The object to check</param>
        /// <returns>true if the object is a numeric type</returns>
        static private bool IsNumericType(Object obj)
        {
            if (null != obj)
            {
                if (obj is byte) return true;
                if (obj is sbyte) return true;
                if (obj is decimal) return true;
                if (obj is double) return true;
                if (obj is float) return true;
                if (obj is int) return true;
                if (obj is uint) return true;
                if (obj is long) return true;
                if (obj is short) return true;
                if (obj is ushort) return true;

                if (obj is System.Byte) return true;
                if (obj is System.SByte) return true;
                if (obj is System.Decimal) return true;
                if (obj is System.Double) return true;
                if (obj is System.Single) return true;
                if (obj is System.Int32) return true;
                if (obj is System.UInt32) return true;
                if (obj is System.Int64) return true;
                if (obj is System.UInt64) return true;
                if (obj is System.Int16) return true;
                if (obj is System.UInt16) return true;
            }
            return false;
        }

        /// <summary>
        /// Used to compare numeric types.  Comparisons between
        /// same types are fine (Int32 to Int32, or Int64 to Int64),
        /// but the Equals method fails across different types.
        /// This method was added to allow any numeric type to
        /// be handled correctly, by using <c>ToString</c> and
        /// comparing the result
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        static private bool ObjectsEqual(Object expected, Object actual)
        {
            if (IsNumericType(expected) &&
                IsNumericType(actual))
            {
                //
                // Convert to strings and compare result to avoid
                // issues with different types that have the same
                // value
                //
                string sExpected = expected.ToString();
                string sActual = actual.ToString();
                return sExpected.Equals(sActual);
            }
            return expected.Equals(actual);
        }
        #endregion

        #region Constructors
        /// <summary>
        /// A private constructor disallows any instances of this object. 
		/// </summary>
		private Assert()
		{ }
        #endregion

        #region IsTrue, IsFalse
        /// <summary>
        /// Asserts that a condition is true. If the condition is false the method throws
		/// an <see cref="AssertionException"/>.
		/// </summary> 
		/// <param name="condition">The evaluated condition</param>
		/// <param name="format">
		/// The format of the message to display if the condition is false,
 		/// containing zero or more format items. 
		/// </param>
		/// <param name="args">
		/// An <see cref="Object"/> array containing zero or more objects to format. 
		/// </param>
		/// <remarks>
		/// <para>
		/// The error message is formatted using <see cref="String.Format(string, object[])"/>.
		/// </para>
		/// </remarks>
		static public void IsTrue(bool condition, string format, params object[] args) 
		{
            Assert.IncrementAssertCount();
            if (!condition)
				Assert.Fail(format,args);
		}

        static public void IsTrue(bool condition, string message)
        {
            Assert.IncrementAssertCount();
            if (!condition)
                Assert.Fail(message);
        }
    
		/// <summary>
		/// Asserts that a condition is true. If the condition is false the method throws
		/// an <see cref="AssertionException"/>.
		/// </summary>
		/// <param name="condition">The evaluated condition</param>
		static public void IsTrue(bool condition) 
		{
			Assert.IsTrue(condition, string.Empty);
		}

		/// <summary>
		/// Asserts that a condition is false. If the condition is true the method throws
		/// an <see cref="AssertionException"/>.
		/// </summary>
		/// <param name="condition">The evaluated condition</param>
		/// <param name="format">
		/// The format of the message to display if the condition is false,
 		/// containing zero or more format items. 
		/// </param>
		/// <param name="args">
		/// An <see cref="Object"/> array containing zero or more objects to format. 
		/// </param>
		/// <remarks>
		/// <para>
		/// The error message is formatted using <see cref="String.Format(string, object[])"/>.
		/// </para>
		/// </remarks>
		static public void IsFalse(bool condition, string format, params object[] args) 
		{
            Assert.IncrementAssertCount();
            if (condition)
                Assert.Fail(format,args);
		}

        static public void IsFalse(bool condition, string message)
        {
            Assert.IncrementAssertCount();
            if (condition)
                Assert.Fail(message);
        }
		
		/// <summary>
		/// Asserts that a condition is false. If the condition is true the method throws
		/// an <see cref="AssertionException"/>.
		/// </summary>
		/// <param name="condition">The evaluated condition</param>
		static public void IsFalse(bool condition) 
		{
			Assert.IsFalse(condition, string.Empty);
        }
        #endregion

        #region AreEqual
        /// <summary>
        /// Verifies that two doubles are equal considering a delta. If the
		/// expected value is infinity then the delta value is ignored. If 
		/// they are not equals then an <see cref="AssertionException"/> is
		/// thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="delta">The maximum acceptable difference between the
		/// the expected and the actual</param>
        /// <param name="message">The message printed out upon failure</param>
        static public void AreEqual(double expected, 
			double actual, double delta, string message) 
		{
            Assert.IncrementAssertCount();
            // handle infinity specially since subtracting two infinite values gives 
            // NaN and the following test fails
			if (double.IsInfinity(expected)) 
			{
				if (!(expected == actual))
					Assert.FailNotEquals(expected, actual, message);
			} 
			else if (!(Math.Abs(expected-actual) <= delta))
				Assert.FailNotEquals(expected, actual, message);
		}

        static public void AreEqual(double expected,double actual, double delta, string me, 
            string format, params object[] args)
        {
            AreEqual(expected,actual,delta,me,format,args);
        }

		/// <summary>
		/// Verifies that two doubles are equal considering a delta. If the
		/// expected value is infinity then the delta value is ignored. If 
		/// they are not equals then an <see cref="AssertionException"/> is
		/// thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="delta">The maximum acceptable difference between the
		/// the expected and the actual</param>
		static public void AreEqual(double expected, double actual, double delta) 
		{
			Assert.AreEqual(expected, actual, delta, string.Empty);
		}

		/// <summary>
		/// Verifies that two floats are equal considering a delta. If the
		/// expected value is infinity then the delta value is ignored. If 
		/// they are not equals then an <see cref="AssertionException"/> is
		/// thrown.
		/// </summary>
		/// <param name="message">The message printed out upon failure</param>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="delta">The maximum acceptable difference between the
		/// the expected and the actual</param>
		static public void AreEqual(float expected, 
			float actual, float delta, 
			string message)
		{
            Assert.IncrementAssertCount();
            // handle infinity specially since subtracting two infinite values gives 
            // NaN and the following test fails
			if (float.IsInfinity(expected)) 
			{
				if (!(expected == actual))
					Assert.FailNotEquals(expected, actual, message);
			} 
			else if (!(Math.Abs(expected-actual) <= delta))
				Assert.FailNotEquals(expected, actual, message);
		}

        static public void AreEqual(float expected,
            float actual, float delta,
            string format, params object[] args)
        {
            AreEqual(expected, actual, delta, String.Format(format, args));
        }


		/// <summary>
		/// Verifies that two floats are equal considering a delta. If the
		/// expected value is infinity then the delta value is ignored. If 
		/// they are not equals then an <see cref="AssertionException"/> is
		/// thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="delta">The maximum acceptable difference between the
		/// the expected and the actual</param>
		static public void AreEqual(float expected, float actual, float delta) 
		{
			Assert.AreEqual(expected, actual, delta, string.Empty);
		}

		/// <summary>
		/// Verifies that two decimals are equal. If 
		/// they are not equals then an <see cref="AssertionException"/> is
		/// thrown.
		/// </summary>
		/// <param name="message">The message printed out upon failure</param>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		static public void AreEqual(decimal expected, decimal actual,
		                            string message)
		{
            Assert.IncrementAssertCount();
            if (!(expected == actual))
                Assert.FailNotEquals(expected, actual,message);
		}

        /// <summary>
        /// Verifies that two decimals are equal. If 
        /// they are not equals then an <see cref="AssertionException"/> is
        /// thrown.
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="actual">The actual value</param>
        /// <param name="format">
        /// The format of the message to display if the assertion fails,
        /// containing zero or more format items. 
        /// </param>
        /// <param name="args">
        /// An <see cref="Object"/> array containing zero or more objects to format. 
        /// </param>
        /// <remarks>
        /// <para>
        /// The error message is formatted using <see cref="String.Format(string, object[])"/>.
        /// </para>
        /// </remarks>
        static public void AreEqual(decimal expected, decimal actual,
                                    string format, params object[] args)
        {
            Assert.IncrementAssertCount();
            if (!(expected == actual))
                Assert.FailNotEquals(expected, actual, format, args);
        }

		/// <summary>
		/// Verifies that two decimals are equal. If 
		/// they are not equals then an <see cref="AssertionException"/> is
		/// thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		static public void AreEqual(decimal expected, decimal actual) 
		{
			Assert.AreEqual(expected, actual, string.Empty);
		}
		
		/// <summary>
		/// Verifies that two ints are equal. If 
		/// they are not equals then an <see cref="AssertionException"/> is
		/// thrown.
		/// </summary>
		/// <param name="message">The message printed out upon failure</param>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		static public void AreEqual(int expected, int actual, 
		                            string message)
		{
            Assert.IncrementAssertCount();
            if (!(expected == actual))
                Assert.FailNotEquals(expected, actual,message);
		}

        /// <summary>
        /// Verifies that two ints are equal. If 
        /// they are not equals then an <see cref="AssertionException"/> is
        /// thrown.
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="actual">The actual value</param>
        /// <param name="format">
        /// The format of the message to display if the assertion fails,
        /// containing zero or more format items. 
        /// </param>
        /// <param name="args">
        /// An <see cref="Object"/> array containing zero or more objects to format. 
        /// </param>
        /// <remarks>
        /// <para>
        /// The error message is formatted using <see cref="String.Format(string, object[])"/>.
        /// </para>
        /// </remarks>
        static public void AreEqual(int expected, int actual,
                                    string format, params object[] args)
        {
            Assert.IncrementAssertCount();
            if (!(expected == actual))
                Assert.FailNotEquals(expected, actual, format, args);
        }

		/// <summary>
		/// Verifies that two ints are equal. If 
		/// they are not equals then an <see cref="AssertionException"/> is
		/// thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		static public void AreEqual(int expected, int actual) 
		{
			Assert.AreEqual(expected, actual, string.Empty);
		}

        /// <summary>
		/// Verifies that two objects are equal.  Two objects are considered
		/// equal if both are null, or if both have the same value.  All
		/// non-numeric types are compared by using the <c>Equals</c> method.
		/// If they are not equal an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The value that is expected</param>
		/// <param name="actual">The actual value</param>
        /// <param name="format">
        /// The format of the message to display if the assertion fails,
        /// containing zero or more format items. 
        /// </param>
        /// <param name="args">
        /// An <see cref="Object"/> array containing zero or more objects to format. 
        /// </param>
        /// <remarks>
        /// <para>
        /// The error message is formatted using <see cref="String.Format(string, object[])"/>.
        /// </para>
        /// </remarks>
        static public void AreEqual(Object expected, Object actual,
		                            string format, params object[] args)
		{
            AreEqual(expected, actual, String.Format(format, args));
		}

        /// <summary>
        /// Verifies that two objects are equal.  Two objects are considered
        /// equal if both are null, or if both have the same value.  All
        /// non-numeric types are compared by using the <c>Equals</c> method.
        /// If they are not equal an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The value that is expected</param>
        /// <param name="actual">The actual value</param>
        /// <param name="message">The message to display if objects are not equal</param>
        static public void AreEqual(Object expected, Object actual,
                                    string message)
        {
            Assert.IncrementAssertCount();
            if (expected == null && actual == null) return;

            if (expected != null && actual != null)
            {

                if (ObjectsEqual(expected, actual))
                {
                    return;
                }
            }
            Assert.FailNotEquals(expected, actual, message);
        }

		/// <summary>
		/// Verifies that two objects are equal.  Two objects are considered
		/// equal if both are null, or if both have the same value.  All
		/// non-numeric types are compared by using the <c>Equals</c> method.
		/// If they are not equal an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The value that is expected</param>
		/// <param name="actual">The actual value</param>
		static public void AreEqual(Object expected, Object actual) 
		{
			Assert.AreEqual(expected, actual, string.Empty);
		}
		
		/// <summary>
		/// Verifies that the value of the property described by <paramref name="pi"/> is the same
		/// in both ojects.
		/// </summary>
		/// <param name="pi">
		/// Property describing the value to test
		/// </param>
		/// <param name="expected">
		/// Reference object
		/// </param>
		/// <param name="actual">
		/// Actual object
		/// </param>
		/// <param name="indices">
		/// Index of the property.
		/// </param>
		static public void AreValueEqual(PropertyInfo pi, Object expected, Object actual, params Object[] indices)
		{
			Assert.IsNotNull(expected);
			Assert.IsNotNull(actual);

            Assert.IncrementAssertCount();
            // check types
			if (!pi.DeclaringType.IsAssignableFrom(expected.GetType()))
				Assert.Fail("Property declaring type does not match with expected type");
			if (!pi.DeclaringType.IsAssignableFrom(actual.GetType()))
				Assert.Fail("Property declaring type does not match with expected type");
			
			Assert.AreEqual(pi.GetValue(expected,indices),
			                pi.GetValue(actual,indices),
			                String.Format("{0}.{1} property does not have the same value",
			                              pi.DeclaringType.Name,
			                              pi.Name
			                              )
			                );
        }
        #endregion

        #region AreNotEqual

        #region Objects[]
        /// <summary>
        /// Asserts that two objects are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        /// <param name="message">The message to be displayed when the two objects are the same object.</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void AreNotEqual(Object[] expected, Object[] actual, string message, params object[] args)
        {
            bool fail = false;

            if (expected != null && actual != null)
            {
                if (expected.GetLength(0) == actual.GetLength(0))
                {
                    fail = true;
                }

                if (!fail)
                {
                    int position = 0;
                    bool same = true;
                    foreach (object o in expected)
                    {
                        if (!o.Equals(actual.GetValue(position)))
                        {
                            same = false;
                        }
                        position++;
                    }

                    if (same)
                        fail = true;
                }
            }
            else
            {
                fail = true;
            }

            if(fail)
            {
                if (args != null)
                        Assert.FailSame(expected, actual, message, args);
                    else
                        Assert.FailSame(expected, actual, message);
            }
        }


        /// <summary>
        /// Asserts that two objects are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        /// <param name="message">The message to be displayed when the objects are the same</param>
        static public void AreNotEqual(Object[] expected, Object[] actual, string message)
        {
            Assert.AreNotEqual(expected, actual, message, null);
        }

        /// <summary>
        /// Asserts that two objects are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        static public void AreNotEqual(Object[] expected, Object[] actual)
        {
            Assert.AreNotEqual(expected, actual, string.Empty, null);
        }

        #endregion



//NUnit Code
        #region Objects
        /// <summary>
        /// Asserts that two objects are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        /// <param name="message">The message to be displayed when the two objects are the same object.</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void AreNotEqual(Object expected, Object actual, string message, params object[] args)
        {
            if (expected == actual)
                if (args != null)
                    Assert.FailSame(expected, actual, message, args);
                else
                    Assert.FailSame(expected, actual, message);
        }

        /// <summary>
        /// Asserts that two objects are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        /// <param name="message">The message to be displayed when the objects are the same</param>
        static public void AreNotEqual(Object expected, Object actual, string message)
        {
            Assert.AreNotEqual(expected, actual, message, null);
        }

        /// <summary>
        /// Asserts that two objects are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        static public void AreNotEqual(Object expected, Object actual)
        {
            Assert.AreNotEqual(expected, actual, string.Empty, null);
        }

        #endregion

        #region Ints
        /// <summary>
        /// Asserts that two ints are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        /// <param name="message">The message to be displayed when the two objects are the same object.</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void AreNotEqual(int expected, int actual, string message, params object[] args)
        {
            if (actual == expected)
                if (args != null)
                    Assert.FailSame(expected, actual, message, args);
                else
                    Assert.FailSame(expected, actual, message);
        }

        /// <summary>
        /// Asserts that two ints are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        /// <param name="message">The message to be displayed when the objects are the same</param>
        static public void AreNotEqual(int expected, int actual, string message)
        {
            Assert.AreNotEqual(expected, actual, message, null);
        }

        /// <summary>
        /// Asserts that two ints are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        static public void AreNotEqual(int expected, int actual)
        {
            Assert.AreNotEqual(expected, actual, string.Empty, null);
        }
        #endregion

        #region UInts
        /// <summary>
        /// Asserts that two uints are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        /// <param name="message">The message to be displayed when the two objects are the same object.</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void AreNotEqual(uint expected, uint actual, string message, params object[] args)
        {
            if (actual == expected)
                if (args != null)
                    Assert.FailSame(expected, actual, message, args);
                else
                    Assert.FailSame(expected, actual, message);
        }

        /// <summary>
        /// Asserts that two uints are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        /// <param name="message">The message to be displayed when the objects are the same</param>
        static public void AreNotEqual(uint expected, uint actual, string message)
        {
            Assert.AreNotEqual(expected, actual, message, null);
        }

        /// <summary>
        /// Asserts that two uints are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        static public void AreNotEqual(uint expected, uint actual)
        {
            Assert.AreNotEqual(expected, actual, string.Empty, null);
        }
        #endregion

        #region Decimals
        /// <summary>
        /// Asserts that two decimals are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        /// <param name="message">The message to be displayed when the two objects are the same object.</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void AreNotEqual(decimal expected, decimal actual, string message, params object[] args)
        {
            if (actual == expected)
                if (args != null)
                    Assert.FailSame(expected, actual, message, args);
                else
                    Assert.FailSame(expected, actual, message);
        }

        /// <summary>
        /// Asserts that two decimals are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        /// <param name="message">The message to be displayed when the objects are the same</param>
        static public void AreNotEqual(decimal expected, decimal actual, string message)
        {
            Assert.AreNotEqual(expected, actual, message, null);
        }

        /// <summary>
        /// Asserts that two decimals are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        static public void AreNotEqual(decimal expected, decimal actual)
        {
            Assert.AreNotEqual(expected, actual, string.Empty, null);
        }
        #endregion

        #region Floats
        /// <summary>
        /// Asserts that two floats are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        /// <param name="message">The message to be displayed when the two objects are the same object.</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void AreNotEqual(float expected, float actual, string message, params object[] args)
        {
            if (actual == expected)
                if (args != null)
                    Assert.FailSame(expected, actual, message, args);
                else
                    Assert.FailSame(expected, actual, message);
        }

        /// <summary>
        /// Asserts that two floats are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        /// <param name="message">The message to be displayed when the objects are the same</param>
        static public void AreNotEqual(float expected, float actual, string message)
        {
            Assert.AreNotEqual(expected, actual, message, null);
        }

        /// <summary>
        /// Asserts that two floats are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        static public void AreNotEqual(float expected, float actual)
        {
            Assert.AreNotEqual(expected, actual, string.Empty, null);
        }
        #endregion

        #region Doubles
        /// <summary>
        /// Asserts that two doubles are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        /// <param name="message">The message to be displayed when the two objects are the same object.</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void AreNotEqual(double expected, double actual, string message, params object[] args)
        {
            if (actual == expected)
                if (args != null)
                    Assert.FailSame(expected, actual, message, args);
                else
                    Assert.FailSame(expected, actual, message);
        }

        /// <summary>
        /// Asserts that two doubles are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        /// <param name="message">The message to be displayed when the objects are the same</param>
        static public void AreNotEqual(double expected, double actual, string message)
        {
            Assert.AreNotEqual(expected, actual, message, null);
        }

        /// <summary>
        /// Asserts that two doubles are not equal. If they are equal
        /// an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        static public void AreNotEqual(double expected, double actual)
        {
            Assert.AreNotEqual(expected, actual, string.Empty, null);
        }
        #endregion

        #endregion
   
   		#region IsNull, IsNotNull
		/// <summary>
		/// Verifies that the object that is passed in is not equal to <code>null</code>
		/// If the object is not <code>null</code> then an <see cref="AssertionException"/>
		/// is thrown.
		/// </summary>
		/// <param name="anObject">The object that is to be tested</param>
        /// <param name="format">
        /// The format of the message to display if the assertion fails,
        /// containing zero or more format items. 
        /// </param>
        /// <param name="args">
        /// An <see cref="Object"/> array containing zero or more objects to format. 
        /// </param>
        /// <remarks>
        /// <para>
        /// The error message is formatted using <see cref="String.Format(string, object[])"/>.
        /// </para>
        /// </remarks>
        static public void IsNotNull(Object anObject, string format, params object[] args) 
		{
			Assert.IsTrue(anObject != null, format, args); 
		}

        static public void IsNotNull(Object anObject, string message)
        {
            Assert.IsTrue(anObject != null, message);
        }

		/// <summary>
		/// Verifies that the object that is passed in is not equal to <code>null</code>
		/// If the object is not <code>null</code> then an <see cref="AssertionException"/>
		/// is thrown.
		/// </summary>
		/// <param name="anObject">The object that is to be tested</param>
		static public void IsNotNull(Object anObject) 
		{
			Assert.IsNotNull(anObject, string.Empty);
		}    
		    
		/// <summary>
		/// Verifies that the object that is passed in is equal to <code>null</code>
		/// If the object is <code>null</code> then an <see cref="AssertionException"/>
		/// is thrown.
		/// </summary>
		/// <param name="anObject">The object that is to be tested</param>
        /// <param name="format">
        /// The format of the message to display if the assertion fails,
        /// containing zero or more format items. 
        /// </param>
        /// <param name="args">
        /// An <see cref="Object"/> array containing zero or more objects to format. 
        /// </param>
        /// <remarks>
        /// <para>
        /// The error message is formatted using <see cref="String.Format(string, object[])"/>.
        /// </para>
        /// </remarks>
        static public void IsNull(Object anObject, string format, params object[] args) 
		{
			Assert.IsTrue(anObject == null, format, args); 
		}

        static public void IsNull(Object anObject, string format, string message)
        {
            Assert.IsTrue(anObject == null, message);
        }

		/// <summary>
		/// Verifies that the object that is passed in is equal to <code>null</code>
		/// If the object is <code>null</code> then an <see cref="AssertionException"/>
		/// is thrown.
		/// </summary>
		/// <param name="anObject">The object that is to be tested</param>
		static public void IsNull(Object anObject) 
		{
			Assert.IsNull(anObject, string.Empty);
		}
		#endregion    
    
        #region Are same
        /// <summary>
        /// Asserts that two objects refer to the same object. If they
		/// are not the same an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="message">The message to be printed when the two objects are not the same object.</param>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The actual object</param>
		static public void AreSame(Object expected, Object actual, string message)
		{
            Assert.IncrementAssertCount();
            if (object.ReferenceEquals(expected, actual)) return;

            Assert.FailNotSame(expected, actual, message);
        }

        /// <summary>
        /// Asserts that two objects refer to the same object. If they
        /// are not the same an <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        /// <param name="format">
        /// The format of the message to display if the assertion fails,
        /// containing zero or more format items. 
        /// </param>
        /// <param name="args">
        /// An <see cref="Object"/> array containing zero or more objects to format. 
        /// </param>
        /// <remarks>
        /// <para>
        /// The error message is formatted using <see cref="String.Format(string, object[])"/>.
        /// </para>
        /// </remarks>
        static public void AreSame(Object expected, Object actual, string format, params object[] args)
        {
            AreSame(expected, actual, String.Format(format, args));
        }

        /// <summary>
        /// Asserts that two objects refer to the same object. If they
		/// are not the same an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The actual object</param>
		static public void AreSame(Object expected, Object actual) 
		{
			Assert.AreSame(expected, actual, string.Empty);
        }
        #endregion

        #region AreNotSame
        static public void AreNotSame(object expected, object actual)
        {
            Assert.AreNotSame(expected, actual, string.Empty);
        }
        static public void AreNotSame(object expected, object actual, string message)
        {
            Assert.IncrementAssertCount();
            if (!object.ReferenceEquals(expected, actual)) return;
            Assert.FailSame(expected, actual, message);
        }
        static public void AreNotSame(object expected, object actual, string format, params object[] args)
        {
            AreNotSame(expected, actual, String.Format(format, args));
        }
        #endregion

		#region Fail
		/// <summary>
		/// Throws an <see cref="AssertionException"/> with the message that is 
		/// passed in. This is used by the other Assert functions. 
		/// </summary>
        /// <param name="format">
        /// The format of the message to initialize the <see cref="AssertionException"/> with. 
        /// </param>
        /// <param name="args">
        /// An <see cref="Object"/> array containing zero or more objects to format. 
        /// </param>
        /// <remarks>
        /// <para>
        /// The error message is formatted using <see cref="String.Format(string, object[])"/>.
        /// </para>
        /// </remarks>
        static public void Fail(string format, params object[] args) 
		{
            Fail(string.Format(format, args));
		}

        /// <summary>
        /// Throws an <see cref="AssertionException"/> with the message that is 
        /// passed in. This is used by the other Assert functions. 
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="AssertionException"/> with.</param>
        static public void Fail(string message)
        {
            throw new AssertionException(message);
        }

		/// <summary>
		/// Throws an <see cref="AssertionException"/> with the message that is 
		/// passed in. This is used by the other Assert functions. 
		/// </summary>
		static public void Fail() 
		{
			Assert.Fail(string.Empty);
		}
		#endregion
		
        #region Ignore
        /// <summary>
        /// Makes the current test ignored using <see cref="String.Format(string, object[])"/> like
        /// formatting
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Ignore(string format, params object[] args)
        {
            Ignore(String.Format(format, args));
        }
        /// <summary>
        /// Makes the current test ignored using <see cref="String.Format(string, object[])"/> like
        /// formatting
        /// </summary>
        /// <param name="message"></param>
        public static void Ignore(string message)
        {
            if (message == null)
                throw new ArgumentNullException("format");
            throw new IgnoreRunException(message);
        }
        #endregion

        #region LowerThan
        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
		/// <paramref name="right"/>.
		/// </summary>
		static public void LowerThan(int left, int right)
        {
            Assert.IncrementAssertCount();
            Assert.IsTrue(left < right,
                          "{0} is not lower than {1}",
                          left, right);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerThan(int left, int right, 
            string message)
        {
            Assert.IsTrue(left < right,
                          "{0} is not lower than {1}, {2}",
                          left, right, message);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerThan(int left, int right,
            string format, params object[] args)
        {
            LowerThan(left, right, String.Format(format, args));
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerThan(short left, short right)
        {
            Assert.IsTrue(left < right,
                          "{0} is not lower than {1}",
                          left, right);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerThan(short left, short right,
            string message)
        {
            Assert.IsTrue(left < right,
                          "{0} is not lower than {1}, {2}",
                          left, right,message);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerThan(short left, short right,
            string format, params object[] args)
        {
            LowerThan(left, right, String.Format(format, args));
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerThan(byte left, byte right)
        {
            Assert.IsTrue(left < right,
                          "{0} is not lower than {1}",
                          left, right);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerThan(byte left, byte right,
            string message)
        {
            Assert.IsTrue(left < right,
                          "{0} is not lower than {1}, {2}",
                          left, right, message);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerThan(byte left, byte right,
            string format, params object[] args)
        {
            LowerThan(left, right, String.Format(format, args));
        }

        /// <summary>
		/// Verifies that <paramref name="left"/> is strictly lower than
		/// <paramref name="right"/>.
		/// </summary>        
		static public void LowerThan(long left, long right)
        {
            Assert.IsTrue(left < right,
                          "{0} is not lower than {1}",
                          left, right);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>        
        static public void LowerThan(long left, long right,
            string message)
        {
            Assert.IsTrue(left < right,
                          "{0} is not lower than {1}, {2}",
                          left, right, message);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>        
        static public void LowerThan(long left, long right,
            string format, params object[] args)
        {
            LowerThan(left, right, format, String.Format(format, args));
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerThan(double left, double right)
        {
            Assert.IsTrue(left < right,
                          "{0} is not lower than {1}",
                          left, right);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerThan(double left, double right,
            string message)
        {
            Assert.IsTrue(left < right,
                          "{0} is not lower than {1}, {2}",
                          left, right, message);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerThan(double left, double right,
            string format, params object[] args)
        {
            LowerThan(left, right, String.Format(format, args));
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerThan(float left, float right)
        {
            Assert.IsTrue(left < right,
                          "{0} is not lower than {1}",
                          left, right);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerThan(float left, float right,
            string message)
        {
            Assert.IsTrue(left < right,
                          "{0} is not lower than {1}, {2}",
                          left, right, message);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerThan(float left, float right,
            string format, params object[] args)
        {
            LowerThan(left, right, String.Format(format, args));
        }

        /// <summary>
		/// Verifies that <paramref name="left"/> is strictly lower than
		/// <paramref name="right"/>.
		/// </summary>
		static public void LowerThan(IComparable left, IComparable right)
		{
			Assert.IsNotNull(left);
			Assert.IsNotNull(right);
			Assert.IsTrue(left.CompareTo(right)<0,
			              "{0} is not lower than {1}",
			              left,right);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerThan(IComparable left, IComparable right,
            string message)
        {
            Assert.IsNotNull(left);
            Assert.IsNotNull(right);
            Assert.IsTrue(left.CompareTo(right) < 0,
                          "{0} is not lower than {1}, {2}",
                          left, right, message);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerThan(IComparable left, IComparable right,
            string format, params object[] args)
        {
            LowerThan(left, right, String.Format(format, args));
        }
        #endregion

        #region LowerEqualThan
        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerEqualThan(int left, int right)
        {
            Assert.IsTrue(left <= right,
                          "{0} is not lower equal than {1}",
                          left, right);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerEqualThan(int left, int right,
            string message)
        {
            Assert.IsTrue(left <= right,
                          "{0} is not lower equal than {1}, {2}",
                          left, right,message);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerEqualThan(int left, int right,
            string format, params object[] args)
        {
            LowerEqualThan(left, right, String.Format(format, args));
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerEqualThan(short left, short right)
        {
            Assert.IsTrue(left <= right,
                          "{0} is not lower equal than {1}",
                          left, right);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerEqualThan(short left, short right,
            string message)
        {
            Assert.IsTrue(left <= right,
                          "{0} is not lower equal than {1}, {2}",
                          left, right, message);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerEqualThan(short left, short right,
            string format, params object[] args)
        {
            LowerEqualThan(left, right, String.Format(format, args));
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerEqualThan(byte left, byte right)
        {
            Assert.IsTrue(left <= right,
                          "{0} is not lower equal than {1}",
                          left, right);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerEqualThan(byte left, byte right,
            string message)
        {
            Assert.IsTrue(left <= right,
                          "{0} is not lower equal than {1}, {2}",
                          left, right, message);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerEqualThan(byte left, byte right,
            string format, params object[] args)
        {
            LowerEqualThan(left, right, String.Format(format, args));
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>        
        static public void LowerEqualThan(long left, long right)
        {
            Assert.IsTrue(left <= right,
                          "{0} is not lower equal than {1}",
                          left, right);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>        
        static public void LowerEqualThan(long left, long right,
            string message)
        {
            Assert.IsTrue(left <= right,
                          "{0} is not lower equal than {1}, {2}",
                          left, right, message);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>        
        static public void LowerEqualThan(long left, long right,
            string format, params object[] args)
        {
            LowerEqualThan(left, right, String.Format(format, args));
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerEqualThan(double left, double right)
        {
            Assert.IsTrue(left <= right,
                          "{0} is not lower equal than {1}",
                          left, right);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerEqualThan(double left, double right,
            string message)
        {
            Assert.IsTrue(left <= right,
                          "{0} is not lower equal than {1}, {2}",
                          left, right, message);
        }


        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerEqualThan(double left, double right,
            string format, params object[] args)
        {
            LowerEqualThan(left, right, String.Format(format, args));
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerEqualThan(float left, float right)
        {
            Assert.IsTrue(left <= right,
                          "{0} is not lower equal than {1}",
                          left, right);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerEqualThan(float left, float right,
            string message)
        {
            Assert.IsTrue(left <= right,
                          "{0} is not lower equal than {1}, {2}",
                          left, right, message);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly lower than
        /// <paramref name="right"/>.
        /// </summary>
        static public void LowerEqualThan(float left, float right,
            string format, params object[] args)
        {
            LowerEqualThan(left, right, String.Format(format, args));
        }

        /// <summary>
		/// Verifies that <paramref name="left"/> is lower equal than
		/// <paramref name="right"/>.
		/// </summary>		
		static public void LowerEqualThan(IComparable left, IComparable right)
		{
			Assert.IsNotNull(left);
			Assert.IsNotNull(right);
			Assert.IsTrue(left.CompareTo(right)<=0,
			              "{0} is not lower equal than {1}",
			              left,right);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is lower equal than
        /// <paramref name="right"/>.
        /// </summary>		
        static public void LowerEqualThan(IComparable left, IComparable right,
            string message)
        {
            Assert.IsNotNull(left);
            Assert.IsNotNull(right);
            Assert.IsTrue(left.CompareTo(right) <= 0,
                          "{0} is not lower equal than {1}, {2}",
                          left, right, message);
        }

        /// <summary>
		/// Verifies that <paramref name="left"/> is lower equal than
		/// <paramref name="right"/>.
		/// </summary>		
		static public void LowerEqualThan(IComparable left, IComparable right,
            string format, params object[] args)
        {
            LowerEqualThan(left, right, String.Format(format, args));
        }
        #endregion

        #region Less
        //Nunit base - dupe of LowerThan
        #region Ints

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void Less(int arg1, int arg2, string message, params object[] args)
        {
            Assert.LowerThan(arg1, arg2, message, args);
        }

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        /// <param name="message">The message that will be displayed on failure</param>
        static public void Less(int arg1, int arg2, string message)
        {
            Assert.LowerThan(arg1, arg2, message);
        }

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        static public void Less(int arg1, int arg2)
        {
            Assert.LowerThan(arg1, arg2);
        }

        #endregion

        #region UInts

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void Less(uint arg1, uint arg2, string message, params object[] args)
        {
            Assert.LowerThan(arg1, arg2, message, args);
        }

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        /// <param name="message">The message that will be displayed on failure</param>
        static public void Less(uint arg1, uint arg2, string message)
        {
            Assert.LowerThan(arg1, arg2, message);
        }

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        static public void Less(uint arg1, uint arg2)
        {
            Assert.LowerThan(arg1, arg2);
        }

        #endregion

        #region Decimals

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void Less(decimal arg1, decimal arg2, string message, params object[] args)
        {
            Assert.LowerThan(arg1, arg2, message, args);
        }

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        /// <param name="message">The message that will be displayed on failure</param>
        static public void Less(decimal arg1, decimal arg2, string message)
        {
            Assert.LowerThan(arg1, arg2, message);
        }

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        static public void Less(decimal arg1, decimal arg2)
        {
            Assert.LowerThan(arg1, arg2);
        }

        #endregion

        #region Long

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void Less(long arg1, long arg2, string message, params object[] args)
        {
            Assert.LowerThan(arg1, arg2, message, args);
        }

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        /// <param name="message">The message that will be displayed on failure</param>
        static public void Less(long arg1, long arg2, string message)
        {
            Assert.LowerThan(arg1, arg2, message);
        }

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        static public void Less(long arg1, long arg2)
        {
            Assert.LowerThan(arg1, arg2);
        }

        #endregion

        #region Doubles

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void Less(double arg1, double arg2, string message, params object[] args)
        {
            Assert.LowerThan(arg1, arg2, message, args);
        }

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        /// <param name="message">The message that will be displayed on failure</param>
        static public void Less(double arg1, double arg2, string message)
        {
            Assert.LowerThan(arg1, arg2, message);
        }

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        static public void Less(double arg1, double arg2)
        {
            Assert.LowerThan(arg1, arg2);
        }

        #endregion

        #region Floats

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void Less(float arg1, float arg2, string message, params object[] args)
        {
            Assert.LowerThan(arg1, arg2, message, args);
        }

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        /// <param name="message">The message that will be displayed on failure</param>
        static public void Less(float arg1, float arg2, string message)
        {
            Assert.LowerThan(arg1, arg2, message);
        }

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        static public void Less(float arg1, float arg2)
        {
            Assert.LowerThan(arg1, arg2);
        }

        #endregion

        #region IComparables

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void Less(IComparable arg1, IComparable arg2, string message, params object[] args)
        {
            Assert.LowerThan(arg1, arg2, message, args);
        }

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        /// <param name="message">The message that will be displayed on failure</param>
        static public void Less(IComparable arg1, IComparable arg2, string message)
        {
            Assert.LowerThan(arg1, arg2, message);
        }

        /// <summary>
        /// Verifies that the first value is less than the second
        /// value. If it is not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be less</param>
        /// <param name="arg2">The second value, expected to be greater</param>
        static public void Less(IComparable arg1, IComparable arg2)
        {
            Assert.LowerThan(arg1, arg2);
        }

        #endregion

        #endregion

        #region GreaterThan
        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly greater than
        /// <paramref name="right"/>.
        /// </summary>
        static public void GreaterThan(int left, int right)
        {
            Assert.IsTrue(left > right,
                          "{0} is not strictly greater than {1}",
                          left, right);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly greater than
        /// <paramref name="right"/>.
        /// </summary>
        static public void GreaterThan(int left, int right, string message)
        {
            Assert.IsTrue(left > right,
                          "{0} is not strictly greater than {1}, {2}",
                          left, right,message);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly greater than
        /// <paramref name="right"/>.
        /// </summary>
        static public void GreaterThan(int left, int right, string format, params object[] args)
        {
            GreaterThan(left, right, String.Format(format, args));
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly greater than
        /// <paramref name="right"/>.
        /// </summary>
        static public void GreaterThan(short left, short right)
        {
            Assert.IsTrue(left > right,
                          "{0} is not strictly greater than {1}",
                          left, right);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly greater than
        /// <paramref name="right"/>.
        /// </summary>
        static public void GreaterThan(short left, short right, string message)
        {
            Assert.IsTrue(left > right,
                          "{0} is not strictly greater than {1}, {2}",
                          left, right, message);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly greater than
        /// <paramref name="right"/>.
        /// </summary>
        static public void GreaterThan(short left, short right, string format, params object[] args)
        {
            GreaterThan(left, right, String.Format(format, args));
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly greater than
        /// <paramref name="right"/>.
        /// </summary>
        static public void GreaterThan(byte left, byte right)
        {
            Assert.IsTrue(left > right,
                          "{0} is not strictly greater equal than {1}",
                          left, right);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly greater than
        /// <paramref name="right"/>.
        /// </summary>
        static public void GreaterThan(byte left, byte right,string message)
        {
            Assert.IsTrue(left > right,
                          "{0} is not strictly greater equal than {1}, {2}",
                          left, right, message);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly greater than
        /// <paramref name="right"/>.
        /// </summary>
        static public void GreaterThan(byte left, byte right, string format, params object[] args)
        {
            GreaterThan(left, right, String.Format(format, args));
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly greater than
        /// <paramref name="right"/>.
        /// </summary>        
        static public void GreaterThan(long left, long right)
        {
            Assert.IsTrue(left > right,
                          "{0} is not strictly greater than {1}",
                          left, right);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly greater than
        /// <paramref name="right"/>.
        /// </summary>        
        static public void GreaterThan(long left, long right, string message)
        {
            Assert.IsTrue(left > right,
                          "{0} is not strictly greater than {1}, {2}",
                          left, right, message);
        }


        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly greater than
        /// <paramref name="right"/>.
        /// </summary>        
        static public void GreaterThan(long left, long right, string format, params object[] args)
        {
            GreaterThan(left, right, String.Format(format, args));
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly greater than
        /// <paramref name="right"/>.
        /// </summary>
        static public void GreaterThan(double left, double right)
        {
            Assert.IsTrue(left > right,
                          "{0} is not strictly greater than {1}",
                          left, right);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly greater than
        /// <paramref name="right"/>.
        /// </summary>
        static public void GreaterThan(double left, double right, string message)
        {
            Assert.IsTrue(left > right,
                          "{0} is not strictly greater than {1}, {2}",
                          left, right,message);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly greater than
        /// <paramref name="right"/>.
        /// </summary>
        static public void GreaterThan(double left, double right, string format, params object[] args)
        {
            GreaterThan(left, right, String.Format(format, args));
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly greater than
        /// <paramref name="right"/>.
        /// </summary>
        static public void GreaterThan(float left, float right)
        {
            Assert.IsTrue(left > right,
                          "{0} is not strictly greater than {1}",
                          left, right);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly greater than
        /// <paramref name="right"/>.
        /// </summary>
        static public void GreaterThan(float left, float right, string message)
        {
            Assert.IsTrue(left > right,
                          "{0} is not strictly greater than {1}, {2}",
                          left, right,message);
        }


        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly greater than
        /// <paramref name="right"/>.
        /// </summary>
        static public void GreaterThan(float left, float right, string format, params object[] args)
        {
            GreaterThan(left, right, String.Format(format, args));
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly greater than
		/// <paramref name="right"/>.
		/// </summary>		
		static public void GreaterThan(IComparable left, IComparable right)
		{
			Assert.IsNotNull(left);
			Assert.IsNotNull(right);
			Assert.IsTrue(left.CompareTo(right)>0,
			              "{0} is not greater than {1}",
			              left,right);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly greater than
        /// <paramref name="right"/>.
        /// </summary>		
        static public void GreaterThan(IComparable left, IComparable right, string message)
        {
            Assert.IsNotNull(left);
            Assert.IsNotNull(right);
            Assert.IsTrue(left.CompareTo(right) > 0,
                          "{0} is not greater than {1} {2}",
                          left, right, message);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly greater than
        /// <paramref name="right"/>.
        /// </summary>		
        static public void GreaterThan(IComparable left, IComparable right, 
            string format, params object[] args)
        {
            GreaterThan(left, right, String.Format(format, args));
        }
        #endregion

        #region Greater
        //NUnit Base, Dupe of GreaterThan
        #region Ints

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If they are not, then an 
        /// <see cref="AssertionException"/> is thrown. 
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void Greater(int arg1,
            int arg2, string message, params object[] args)
        {
            Assert.GreaterThan(arg1, arg2, message, args);
        }

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If they are not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        /// <param name="message">The message that will be displayed on failure</param>
        static public void Greater(int arg1, int arg2, string message)
        {
            Assert.GreaterThan(arg1, arg2, message);
        }

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If they are not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        static public void Greater(int arg1, int arg2)
        {
            Assert.GreaterThan(arg1, arg2);
        }

        #endregion

        #region UInts

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If they are not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void Greater(uint arg1,
            uint arg2, string message, params object[] args)
        {
            Assert.GreaterThan(arg1, arg2, message, args);
        }

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If they are not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        /// <param name="message">The message that will be displayed on failure</param>
        static public void Greater(uint arg1, uint arg2, string message)
        {
            Assert.GreaterThan(arg1, arg2, message);
        }

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If they are not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        static public void Greater(uint arg1, uint arg2)
        {
            Assert.GreaterThan(arg1, arg2);
        }

        #endregion

        #region Decimals

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If they are not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void Greater(decimal arg1,
            decimal arg2, string message, params object[] args)
        {
            Assert.GreaterThan(arg1, arg2, message, args);
        }

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If they are not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        /// <param name="message">The message that will be displayed on failure</param>
        static public void Greater(decimal arg1, decimal arg2, string message)
        {
            Assert.GreaterThan(arg1, arg2, message);
        }

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If they are not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        static public void Greater(decimal arg1, decimal arg2)
        {
            Assert.GreaterThan(arg1, arg2);
        }

        #endregion

        #region Long

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If they are not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void Greater(long arg1,
            long arg2, string message, params object[] args)
        {
            Assert.GreaterThan(arg1, arg2, message, args);
        }

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If they are not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        /// <param name="message">The message that will be displayed on failure</param>
        static public void Greater(long arg1, long arg2, string message)
        {
            Assert.GreaterThan(arg1, arg2, message);
        }

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If they are not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        static public void Greater(long arg1, long arg2)
        {
            Assert.GreaterThan(arg1, arg2);
        }

        #endregion

        #region Doubles

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If they are not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void Greater(double arg1,
            double arg2, string message, params object[] args)
        {
            Assert.GreaterThan(arg1, arg2, message, args);
        }

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If they are not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        /// <param name="message">The message that will be displayed on failure</param>
        static public void Greater(double arg1,
            double arg2, string message)
        {
            Assert.GreaterThan(arg1, arg2, message);
        }

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If they are not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        static public void Greater(double arg1, double arg2)
        {
            Assert.GreaterThan(arg1, arg2);
        }

        #endregion

        #region Floats

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If they are not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void Greater(float arg1,
            float arg2, string message, params object[] args)
        {
            Assert.GreaterThan(arg1, arg2, message, args);
        }

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If they are not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        /// <param name="message">The message that will be displayed on failure</param>
        static public void Greater(float arg1, float arg2, string message)
        {
            Assert.GreaterThan(arg1, arg2, message);
        }

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If they are not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        static public void Greater(float arg1, float arg2)
        {
            Assert.GreaterThan(arg1, arg2);
        }

        #endregion

        #region IComparables

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If they are not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void Greater(IComparable arg1,
            IComparable arg2, string message, params object[] args)
        {
            Assert.GreaterThan(arg2, arg1, message, args);
        }

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If they are not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        /// <param name="message">The message that will be displayed on failure</param>
        static public void Greater(IComparable arg1, IComparable arg2, string message)
        {
            Assert.GreaterThan(arg1, arg2, message);
        }

        /// <summary>
        /// Verifies that the first value is greater than the second
        /// value. If they are not, then an 
        /// <see cref="AssertionException"/> is thrown.
        /// </summary>
        /// <param name="arg1">The first value, expected to be greater</param>
        /// <param name="arg2">The second value, expected to be less</param>
        static public void Greater(IComparable arg1, IComparable arg2)
        {
            Assert.GreaterThan(arg1, arg2);
        }

        #endregion

        #endregion

        #region GreateEqualThan
        /// <summary>
        /// Verifies that <paramref name="left"/> is greater than
        /// <paramref name="right"/>.
        /// </summary>
        static public void GreaterEqualThan(int left, int right)
        {
            Assert.IsTrue(left >= right,
                          "{0} is not greater than {1}",
                          left, right);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is greater than
        /// <paramref name="right"/>.
        /// </summary>
        static public void GreaterEqualThan(int left, int right, string message)
        {
            Assert.IsTrue(left >= right,
                          "{0} is not greater than {1}, {2}",
                          left, right,message);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is greater than
        /// <paramref name="right"/>.
        /// </summary>
        static public void GreaterEqualThan(int left, int right, 
            string format, params object[] args)
        {
            GreaterEqualThan(left, right, String.Format(format, args));
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is greater than
        /// <paramref name="right"/>.
        /// </summary>
        static public void GreaterEqualThan(short left, short right)
        {
            Assert.IsTrue(left >= right,
                          "{0} is not greater than {1}",
                          left, right);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is greater than
        /// <paramref name="right"/>.
        /// </summary>
        static public void GreaterEqualThan(short left, short right, string message)
        {
            Assert.IsTrue(left >= right,
                          "{0} is not greater than {1}, {2}",
                          left, right,message);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is greater than
        /// <paramref name="right"/>.
        /// </summary>
        static public void GreaterEqualThan(short left, short right, 
            string format, params object[] args)
        {
            GreaterEqualThan(left, right, String.Format(format, args));
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is greater than
        /// <paramref name="right"/>.
        /// </summary>
        static public void GreaterEqualThan(byte left, byte right)
        {
            Assert.IsTrue(left >= right,
                          "{0} is not greater than {1}",
                          left, right);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is greater than
        /// <paramref name="right"/>.
        /// </summary>
        static public void GreaterEqualThan(byte left, byte right, string message)
        {
            Assert.IsTrue(left >= right,
                          "{0} is not greater than {1}, {2}",
                          left, right, message);
        }


        /// <summary>
        /// Verifies that <paramref name="left"/> is greater than
        /// <paramref name="right"/>.
        /// </summary>
        static public void GreaterEqualThan(byte left, byte right, 
            string format, params object[] args)
        {
            GreaterEqualThan(left, right, String.Format(format, args));
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is greater than
        /// <paramref name="right"/>.
        /// </summary>        
        static public void GreaterEqualThan(long left, long right)
        {
            Assert.IsTrue(left >= right,
                          "{0} is not greater than {1}",
                          left, right);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is greater than
        /// <paramref name="right"/>.
        /// </summary>        
        static public void GreaterEqualThan(long left, long right, string message)
        {
            Assert.IsTrue(left >= right,
                          "{0} is not greater than {1}, {2}",
                          left, right, message);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is greater than
        /// <paramref name="right"/>.
        /// </summary>        
        static public void GreaterEqualThan(long left, long right, 
            string format, params object[] args)
        {
            GreaterEqualThan(left, right, String.Format(format, args));
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is greater than
        /// <paramref name="right"/>.
        /// </summary>
        static public void GreaterEqualThan(double left, double right)
        {
            Assert.IsTrue(left >= right,
                          "{0} is not greater than {1}",
                          left, right);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is greater than
        /// <paramref name="right"/>.
        /// </summary>
        static public void GreaterEqualThan(double left, double right, string message)
        {
            Assert.IsTrue(left >= right,
                          "{0} is not greater than {1}",
                          left, right,message);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is greater than
        /// <paramref name="right"/>.
        /// </summary>
        static public void GreaterEqualThan(double left, double right, 
            string format, params object[] args)
        {
            GreaterEqualThan(left, right, String.Format(format, args));
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is greater than
        /// <paramref name="right"/>.
        /// </summary>
        static public void GreaterEqualThan(float left, float right)
        {
            Assert.IsTrue(left >= right,
                          "{0} is not greater than {1}",
                          left, right);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is greater than
        /// <paramref name="right"/>.
        /// </summary>
        static public void GreaterEqualThan(float left, float right, string message)
        {
            Assert.IsTrue(left >= right,
                          "{0} is not greater than {1}, {2}",
                          left, right,message);
        }


        /// <summary>
        /// Verifies that <paramref name="left"/> is greater than
        /// <paramref name="right"/>.
        /// </summary>
        static public void GreaterEqualThan(float left, float right, 
            string format, params object[] args)
        {
            GreaterEqualThan(left, right, String.Format(format, args));
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly greater than
		/// <paramref name="right"/>.
		/// </summary>		
		static public void GreaterEqualThan(IComparable left, IComparable right)
		{
			Assert.IsNotNull(left);
			Assert.IsNotNull(right);
			Assert.IsTrue(left.CompareTo(right)>=0,
			              "{0} is not greater equal than {1}",
			              left,right);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly greater than
        /// <paramref name="right"/>.
        /// </summary>		
        static public void GreaterEqualThan(IComparable left, IComparable right, string message)
        {
            Assert.IsNotNull(left);
            Assert.IsNotNull(right);
            Assert.IsTrue(left.CompareTo(right) >= 0,
                          "{0} is not greater equal than {1}, {2}",
                          left, right, message);
        }

        /// <summary>
        /// Verifies that <paramref name="left"/> is strictly greater than
        /// <paramref name="right"/>.
        /// </summary>		
        static public void GreaterEqualThan(IComparable left, IComparable right, 
            string format, params object[] args)
        {
            GreaterEqualThan(left, right, String.Format(format, args));
        }
        #endregion

        #region Between
        /// <summary>
        /// Asserts that <paramref name="test"/> is between <paramref name="left"/> and
        /// <paramref name="right"/>.
        /// </summary>
        static public void Between(int test, int left, int right)
        {
            Assert.IsTrue(test>=left,
                   "{0} is smaller than {1}",
                   test, left);
            Assert.IsTrue(test<=right,
                   "{0} is greater than {1}",
                   test, right);
        }

        /// <summary>
        /// Asserts that <paramref name="test"/> is between <paramref name="left"/> and
        /// <paramref name="right"/>.
        /// </summary>
        static public void Between(int test, int left, int right, string message)
        {
            Assert.IsTrue(test>=left,
                   "{0} is smaller than {1}, {2}",
                   test, left, message);
            Assert.IsTrue(test<=right,
                   "{0} is greater than {1}, {2}",
                   test, right, message);
        }

        /// <summary>
        /// Asserts that <paramref name="test"/> is between <paramref name="left"/> and
        /// <paramref name="right"/>.
        /// </summary>
        static public void Between(int test, int left, int right, string format, params object[] args)
        {
            Between(test,left,right,String.Format(format,args));
        }

        /// <summary>
        /// Asserts that <paramref name="test"/> is between <paramref name="left"/> and
        /// <paramref name="right"/>.
        /// </summary>
        static public void Between(short test, short left, short right)
        {
            Assert.IsTrue(test >= left,
                   "{0} is smaller than {1}",
                   test, left);
            Assert.IsTrue(test <= right,
                   "{0} is greater than {1}",
                   test, right);
        }

        /// <summary>
        /// Asserts that <paramref name="test"/> is between <paramref name="left"/> and
        /// <paramref name="right"/>.
        /// </summary>
        static public void Between(short test, short left, short right, string message)
        {
            Assert.IsTrue(test >= left,
                   "{0} is smaller than {1}, {2}",
                   test, left,message);
            Assert.IsTrue(test <= right,
                   "{0} is greater than {1}, {2}",
                   test, right,message);
        }

        /// <summary>
        /// Asserts that <paramref name="test"/> is between <paramref name="left"/> and
        /// <paramref name="right"/>.
        /// </summary>
        static public void Between(short test, short left, short right, 
            string format, params object[] args)
        {
            Between(test,left,right,String.Format(format,args));
        }

        /// <summary>
        /// Asserts that <paramref name="test"/> is between <paramref name="left"/> and
        /// <paramref name="right"/>.
        /// </summary>
        static public void Between(byte test, byte left, byte right)
        {
            Assert.IsTrue(test >= left,
                   "{0} is smaller than {1}",
                   test, left);
            Assert.IsTrue(test <= right,
                   "{0} is greater than {1}",
                   test, right);
        }

        /// <summary>
        /// Asserts that <paramref name="test"/> is between <paramref name="left"/> and
        /// <paramref name="right"/>.
        /// </summary>
        static public void Between(byte test, byte left, byte right, string message)
        {
            Assert.IsTrue(test >= left,
                   "{0} is smaller than {1}, {2}",
                   test, left,message);
            Assert.IsTrue(test <= right,
                   "{0} is greater than {1}, {2}",
                   test, right,message);
        }

        /// <summary>
        /// Asserts that <paramref name="test"/> is between <paramref name="left"/> and
        /// <paramref name="right"/>.
        /// </summary>
        static public void Between(byte test, byte left, byte right, 
            string format, params object[] args)
        {
            Between(test,left,right,String.Format(format,args));
        }

        /// <summary>
        /// Asserts that <paramref name="test"/> is between <paramref name="left"/> and
        /// <paramref name="right"/>.
        /// </summary>
        static public void Between(long test, long left, long right)
        {
            Assert.IsTrue(test >= left,
                   "{0} is smaller than {1}",
                   test, left);
            Assert.IsTrue(test <= right,
                   "{0} is greater than {1}",
                   test, right);
        }

        /// <summary>
        /// Asserts that <paramref name="test"/> is between <paramref name="left"/> and
        /// <paramref name="right"/>.
        /// </summary>
        static public void Between(long test, long left, long right, string message)
        {
            Assert.IsTrue(test >= left,
                   "{0} is smaller than {1}, {2}",
                   test, left,message);
            Assert.IsTrue(test <= right,
                   "{0} is greater than {1}, {2}",
                   test, right,message);
        }

        /// <summary>
        /// Asserts that <paramref name="test"/> is between <paramref name="left"/> and
        /// <paramref name="right"/>.
        /// </summary>
        static public void Between(long test, long left, long right, 
            string format, params object[] args)
        {
            Between(test,left,right,String.Format(format,args));
        }

        /// <summary>
        /// Asserts that <paramref name="test"/> is between <paramref name="left"/> and
        /// <paramref name="right"/>.
        /// </summary>
        static public void Between(double test, double left, double right)
        {
            Assert.IsTrue(test >= left,
                   "{0} is smaller than {1}",
                   test, left);
            Assert.IsTrue(test <= right,
                   "{0} is greater than {1}",
                   test, right);
        }

        /// <summary>
        /// Asserts that <paramref name="test"/> is between <paramref name="left"/> and
        /// <paramref name="right"/>.
        /// </summary>
        static public void Between(double test, double left, double right, string message)
        {
            Assert.IsTrue(test >= left,
                   "{0} is smaller than {1}, {2}",
                   test, left,message);
            Assert.IsTrue(test <= right,
                   "{0} is greater than {1}, {2}",
                   test, right,message);
        }

        /// <summary>
        /// Asserts that <paramref name="test"/> is between <paramref name="left"/> and
        /// <paramref name="right"/>.
        /// </summary>
        static public void Between(double test, double left, double right, 
            string format, params object[] args)
        {
            Between(test,left,right,String.Format(format,args));
        }

        /// <summary>
        /// Asserts that <paramref name="test"/> is between <paramref name="left"/> and
        /// <paramref name="right"/>.
        /// </summary>
        static public void Between(float test, float left, float right)
        {
            Assert.IsTrue(test >= left,
                   "{0} is smaller than {1}",
                   test, left);
            Assert.IsTrue(test <= right,
                   "{0} is greater than {1}",
                   test, right);
        }

        /// <summary>
        /// Asserts that <paramref name="test"/> is between <paramref name="left"/> and
        /// <paramref name="right"/>.
        /// </summary>
        static public void Between(float test, float left, float right, string message)
        {
            Assert.IsTrue(test >= left,
                   "{0} is smaller than {1}, {2}",
                   test, left,message);
            Assert.IsTrue(test <= right,
                   "{0} is greater than {1}, {2}",
                   test, right,message);
        }

        /// <summary>
        /// Asserts that <paramref name="test"/> is between <paramref name="left"/> and
        /// <paramref name="right"/>.
        /// </summary>
        static public void Between(float test, float left, float right, 
            string format, params object[] args)
        {
            Between(test,left,right,String.Format(format,args));
        }

        /// <summary>
        /// Asserts that <paramref name="test"/> is between <paramref name="left"/> and
    	/// <paramref name="right"/>.
		/// </summary>
    	static public void Between(IComparable test, IComparable left, IComparable right)
    	{
            Assert.IsNotNull(test);
            Assert.IsNotNull(left);
            Assert.IsNotNull(right);
            Assert.IsTrue(test.CompareTo(left)>=0,
    		       "{0} is smaller than {1}",
    		       test, left);
    		Assert.IsTrue(test.CompareTo(right)<=0,
    		       "{0} is greater than {1}",
    		       test, right);
    	}

        /// <summary>
        /// Asserts that <paramref name="test"/> is between <paramref name="left"/> and
    	/// <paramref name="right"/>.
		/// </summary>
    	static public void Between(
            IComparable test, 
            IComparable left, 
            IComparable right,
            string message)
    	{
            Assert.IsNotNull(test);
            Assert.IsNotNull(left);
            Assert.IsNotNull(right);
            Assert.IsTrue(test.CompareTo(left)>=0,
    		       "{0} is smaller than {1}, {2}",
    		       test, left,message);
    		Assert.IsTrue(test.CompareTo(right)<=0,
    		       "{0} is greater than {1}, {2}",
    		       test, right,message);
    	}

        /// <summary>
        /// Asserts that <paramref name="test"/> is between <paramref name="left"/> and
        /// <paramref name="right"/>.
        /// </summary>
        static public void Between(IComparable test, IComparable left, IComparable right, 
            string format, params object[] args)
        {
            Between(test,left,right,String.Format(format,args));
        }

        #endregion

        #region NotBetween
        /// <summary>
        /// Asserts that <paramref name="test"/> is <strong>not</strong> between <paramref name="left"/> and
        /// <paramref name="right"/>.
        /// </summary>
        static public void NotBetween(int test, int left, int right)
        {
            Assert.IncrementAssertCount();
            if (test.CompareTo(left) < 0)
                return;
            if (test.CompareTo(right) > 0)
                return;

            Assert.Fail(
                   "{0} is in {1} - {2}",
                   test, left, right);

        }
        /// <summary>
        /// Asserts that <paramref name="test"/> is <strong>not</strong> between <paramref name="left"/> and
        /// <paramref name="right"/>.
        /// </summary>
        static public void NotBetween(short test, short left, short right)
        {
            Assert.IncrementAssertCount();
            if (test.CompareTo(left) < 0)
                return;
            if (test.CompareTo(right) > 0)
                return;

            Assert.Fail(
                   "{0} is in {1} - {2}",
                   test, left, right);

        }
        /// <summary>
        /// Asserts that <paramref name="test"/> is <strong>not</strong> between <paramref name="left"/> and
        /// <paramref name="right"/>.
        /// </summary>
        static public void NotBetween(byte test, byte left, byte right)
        {
            Assert.IncrementAssertCount();
            if (test.CompareTo(left) < 0)
                return;
            if (test.CompareTo(right) > 0)
                return;

            Assert.Fail(
                   "{0} is in {1} - {2}",
                   test, left, right);

        }
        /// <summary>
        /// Asserts that <paramref name="test"/> is <strong>not</strong> between <paramref name="left"/> and
        /// <paramref name="right"/>.
        /// </summary>
        static public void NotBetween(long test, long left, long right)
        {
            Assert.IncrementAssertCount();
            if (test.CompareTo(left) < 0)
                return;
            if (test.CompareTo(right) > 0)
                return;

            Assert.Fail(
                   "{0} is in {1} - {2}",
                   test, left, right);

        }
        /// <summary>
        /// Asserts that <paramref name="test"/> is <strong>not</strong> between <paramref name="left"/> and
        /// <paramref name="right"/>.
        /// </summary>
        static public void NotBetween(double test, double left, double right)
        {
            Assert.IncrementAssertCount();
            if (test.CompareTo(left) < 0)
                return;
            if (test.CompareTo(right) > 0)
                return;

            Assert.Fail(
                   "{0} is in {1} - {2}",
                   test, left, right);
        }
        /// <summary>
        /// Asserts that <paramref name="test"/> is <strong>not</strong> between <paramref name="left"/> and
        /// <paramref name="right"/>.
        /// </summary>
        static public void NotBetween(float test, float left, float right)
        {
            Assert.IncrementAssertCount();
            if (test.CompareTo(left) < 0)
                return;
            if (test.CompareTo(right) > 0)
                return;

            Assert.Fail(
                   "{0} is in {1} - {2}",
                   test, left, right);
        }
        /// <summary>
        /// Asserts that <paramref name="test"/> is <strong>not</strong> between <paramref name="left"/> and
    	/// <paramref name="right"/>.
		/// </summary>
    	static public void NotBetween(IComparable test, IComparable left, IComparable right)
    	{
            Assert.IsNotNull(test);
            Assert.IsNotNull(left);
            Assert.IsNotNull(right);
            if (test.CompareTo(left)<0)
    			return;
    		if (test.CompareTo(right)>0)
    			return;
    		
    		Assert.Fail(
    		       "{0} is in {1} - {2}",
    		       test, left, right);
        }
        #endregion
		
		#region In, NotIn
	   	/// <summary>
    	/// Asserts that <paramref name="test"/> is in the dic <paramref name="list"/>.
		/// </summary>    	
    	static public void In(Object test, IDictionary dic)
    	{
            In(test, dic, null);
    	}

        /// <summary>
        /// Asserts that <paramref name="test"/> is in the dic <paramref name="list"/>.
        /// </summary>    	
        static public void In(Object test, IDictionary dic, string message)
        {
            Assert.IsNotNull(test, "tested object is null");
            Assert.IsNotNull(dic, "Dictionary is a null reference");
            Assert.IsTrue(dic.Contains(test),
                          "Dictionary does not contain {0} {1}", test,message);
        }    	

    	/// <summary>
    	/// Asserts that <paramref name="test"/> is in the list <paramref name="list"/>.
		/// </summary>    	
    	static public void In(Object test, IList list)
    	{
            In(test, list, null);
    	}

        /// <summary>
        /// Asserts that <paramref name="test"/> is in the list <paramref name="list"/>.
        /// </summary>    	
        static public void In(Object test, IList list, string message)
        {
            Assert.IsNotNull(list, "List is a null reference");
            Assert.IsTrue(list.Contains(test),
                          "List does not contain {0} {1}", test,message);
        }

    	/// <summary>
    	/// Asserts that <paramref name="test"/> is in the enumerable collection <paramref name="enumerable"/>.
		/// </summary>    	
    	static public void In(Object test, IEnumerable enumerable, string message)
    	{
    		Assert.IsNotNull(enumerable,"Enumerable collection is a null reference");
    		foreach(Object o in enumerable)
    		{
    			if (o==test)
    				return;
    		}
    		Assert.Fail("Collection does not contain {0} {1}",test, message);
    	}    	

    	/// <summary>
    	/// Asserts that <paramref name="test"/> is in the enumerable collection <paramref name="enumerable"/>.
		/// </summary>    	
        static public void In(Object test, IEnumerable enumerable)
        {
            In(test, enumerable, null);
        }

	   	/// <summary>
    	/// Asserts that <paramref name="test"/> is <strong>not</strong> in the dic <paramref name="list"/>.
		/// </summary>    	
    	static public void NotIn(Object test, IDictionary dic, string message)
    	{
            Assert.IsNotNull(test);
            Assert.IsNotNull(dic,"Dictionary is a null reference");
    		Assert.IsFalse(dic.Contains(test),
    		              "Dictionary does contain {0} {1}",test,message);
    	}    	

	   	/// <summary>
    	/// Asserts that <paramref name="test"/> is <strong>not</strong> in the dic <paramref name="list"/>.
		/// </summary>    	
        static public void NotIn(Object test, IDictionary dic)
        {
            NotIn(test, dic, null);
        }

    	/// <summary>
    	/// Asserts that <paramref name="test"/> is <strong>not</strong> in the list <paramref name="list"/>.
		/// </summary>    	
    	static public void NotIn(Object test, IList list,string message)
    	{
    		Assert.IsNotNull(list,"List is a null reference");
    		Assert.IsFalse(list.Contains(test),
    		              "List does contain {0} {1}",test,message);
    	}

        /// <summary>
        /// Asserts that <paramref name="test"/> is <strong>not</strong> in the list <paramref name="list"/>.
        /// </summary>    	
        static public void NotIn(Object test, IList list)
        {
            NotIn(test, list, null);
        }

    	/// <summary>
    	/// Asserts that <paramref name="test"/> is <strong>not</strong> in the enumerable collection <paramref name="enumerable"/>.
		/// </summary>    	
    	static public void NotIn(Object test, IEnumerable enumerable, string message)
    	{
    		Assert.IsNotNull(enumerable,"Enumerable collection is a null reference");
    		foreach(Object o in enumerable)
    		{
    			Assert.IsFalse(o==test,"{0} is part of the enumeration {1}",test, message);
    		}
    	}

        /// <summary>
        /// Asserts that <paramref name="test"/> is <strong>not</strong> in the enumerable collection <paramref name="enumerable"/>.
        /// </summary>    	
        static public void NotIn(Object test, IEnumerable enumerable)
        {
            NotIn(test, enumerable, null);
        }
        #endregion    	

        #region IsEmpty
        //NUnit Code

        //Fails if it is not empty

        /// <summary>
        /// Assert that a string is empty - that is equal to string.Empty
        /// </summary>
        /// <param name="aString">The string to be tested</param>
        /// <param name="message">The message to be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void IsEmpty(string aString, string message, params object[] args)
        {
            if (aString != "" || !aString.Equals(string.Empty))
            {
                if (args != null)
                    Assert.FailIsNotEmpty(aString, message, args);
                else
                    Assert.FailIsNotEmpty(aString, message);
            }
        }

        /// <summary>
        /// Assert that a string is empty - that is equal to string.Emtpy
        /// </summary>
        /// <param name="aString">The string to be tested</param>
        /// <param name="message">The message to be displayed on failure</param>
        public static void IsEmpty(string aString, string message)
        {
            IsEmpty(aString, message, null);
        }

        /// <summary>
        /// Assert that a string is empty - that is equal to string.Emtpy
        /// </summary>
        /// <param name="aString">The string to be tested</param>
        public static void IsEmpty(string aString)
        {
            IsEmpty(aString, string.Empty, null);
        }

        /// <summary>
        /// Assert that an array, list or other collection is empty
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing ICollection</param>
        /// <param name="message">The message to be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void IsEmpty(ICollection collection, string message, params object[] args)
        {
            if (collection.Count != 0)
            {
                if(args != null)
                    Assert.FailIsNotEmpty(collection, message, args);
                else
                    Assert.FailIsNotEmpty(collection, message);
            }
        }

        /// <summary>
        /// Assert that an array, list or other collection is empty
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing ICollection</param>
        /// <param name="message">The message to be displayed on failure</param>
        public static void IsEmpty(ICollection collection, string message)
        {
            IsEmpty(collection, message, null);
        }

        /// <summary>
        /// Assert that an array,list or other collection is empty
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing ICollection</param>
        public static void IsEmpty(ICollection collection)
        {
            IsEmpty(collection, string.Empty, null);
        }
        #endregion

        #region IsNotEmpty
        //NUnit Code

        //Fail when it is empty

        /// <summary>
        /// Assert that a string is empty - that is equal to string.Emtpy
        /// </summary>
        /// <param name="aString">The string to be tested</param>
        /// <param name="message">The message to be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void IsNotEmpty(string aString, string message, params object[] args)
        {
            if (aString == "" || aString.Equals(string.Empty))
            {
                if (args != null)
                    Assert.FailIsEmpty(aString, message, args);
                else
                    Assert.FailIsEmpty(aString, message);
            }
        }

        /// <summary>
        /// Assert that a string is empty - that is equal to string.Emtpy
        /// </summary>
        /// <param name="aString">The string to be tested</param>
        /// <param name="message">The message to be displayed on failure</param>
        public static void IsNotEmpty(string aString, string message)
        {
            IsNotEmpty(aString, message, null);
        }

        /// <summary>
        /// Assert that a string is empty - that is equal to string.Emtpy
        /// </summary>
        /// <param name="aString">The string to be tested</param>
        public static void IsNotEmpty(string aString)
        {
            IsNotEmpty(aString, string.Empty, null);
        }

        /// <summary>
        /// Assert that an array, list or other collection is empty
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing ICollection</param>
        /// <param name="message">The message to be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void IsNotEmpty(ICollection collection, string message, params object[] args)
        {
            if (collection.Count == 0)
            {
                if(args != null)
                    Assert.FailIsEmpty(collection, message, args);
                else
                    Assert.FailIsEmpty(collection, message);
            }
        }

        /// <summary>
        /// Assert that an array, list or other collection is empty
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing ICollection</param>
        /// <param name="message">The message to be displayed on failure</param>
        public static void IsNotEmpty(ICollection collection, string message)
        {
            IsNotEmpty(collection, message, null);
        }

        /// <summary>
        /// Assert that an array,list or other collection is empty
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing ICollection</param>
        public static void IsNotEmpty(ICollection collection)
        {
            IsNotEmpty(collection, string.Empty, null);
        }
        #endregion

        #region IsNaN
        //NUnit Code
        /// <summary>
        /// Verifies that the double is passed is an <code>NaN</code> value.
        /// If the object is not <code>NaN</code> then an <see cref="AssertionException"/>
        /// is thrown.
        /// </summary>
        /// <param name="aDouble">The value that is to be tested</param>
        /// <param name="message">The message to be displayed when the object is not null</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void IsNaN(double aDouble, string message, params object[] args)
        {
            if (!double.IsNaN(aDouble))
            {
                Fail(message);
            }
        }

        /// <summary>
        /// Verifies that the double is passed is an <code>NaN</code> value.
        /// If the object is not <code>NaN</code> then an <see cref="AssertionException"/>
        /// is thrown.
        /// </summary>
        /// <param name="aDouble">The object that is to be tested</param>
        /// <param name="message">The message to be displayed when the object is not null</param>
        static public void IsNaN(double aDouble, string message)
        {
            if (!double.IsNaN(aDouble))
            {
                Fail(message);
            }
        }

        /// <summary>
        /// Verifies that the double is passed is an <code>NaN</code> value.
        /// If the object is not <code>NaN</code> then an <see cref="AssertionException"/>
        /// is thrown.
        /// </summary>
        /// <param name="aDouble">The object that is to be tested</param>
        static public void IsNaN(double aDouble)
        {
            Assert.IsNaN(aDouble, string.Empty);
        }

        #endregion

        #region Contains

        static public void Contains(string s, string contain)
        {
            Assert.IsTrue(s.IndexOf(contain) >= 0, "String [[{0}]] does not contain [[{1}]]",
                s, contain);
        }

        #endregion

        #region TypeAssert
        //Type Asserts modified from NUnit Code.
        #region IsAssignableFrom
        /// <summary>
        /// Asserts that an object may be assigned a  value of a given Type.
        /// </summary>
        /// <param name="expected">The expected Type.</param>
        /// <param name="actual">The object under examination</param>
        static public void IsAssignableFrom(System.Type expected, object actual)
        {
            IsAssignableFrom(expected, actual, "");
        }

        /// <summary>
        /// Asserts that an object may be assigned a  value of a given Type.
        /// </summary>
        /// <param name="expected">The expected Type.</param>
        /// <param name="actual">The object under examination</param>
        /// <param name="message">The messge to display in case of failure</param>
        static public void IsAssignableFrom(System.Type expected, object actual, string message)
        {
            IsAssignableFrom(expected, actual, message, null);
        }

        /// <summary>
        /// Asserts that an object may be assigned a  value of a given Type.
        /// </summary>
        /// <param name="expected">The expected Type.</param>
        /// <param name="actual">The object under examination</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        static public void IsAssignableFrom(System.Type expected, object actual, string message, params object[] args)
        {
            if(!actual.GetType().IsAssignableFrom(expected))
            {
                Assert.Fail(message);
            }
        }
        #endregion

        #region IsNotAssignableFrom
        /// <summary>
        /// Asserts that an object may not be assigned a  value of a given Type.
        /// </summary>
        /// <param name="expected">The expected Type.</param>
        /// <param name="actual">The object under examination</param>
        static public void IsNotAssignableFrom(System.Type expected, object actual)
        {
            IsNotAssignableFrom(expected, actual, "");
        }

        /// <summary>
        /// Asserts that an object may not be assigned a  value of a given Type.
        /// </summary>
        /// <param name="expected">The expected Type.</param>
        /// <param name="actual">The object under examination</param>
        /// <param name="message">The messge to display in case of failure</param>
        static public void IsNotAssignableFrom(System.Type expected, object actual, string message)
        {
            IsNotAssignableFrom(expected, actual, message, null);
        }

        /// <summary>
        /// Asserts that an object may not be assigned a  value of a given Type.
        /// </summary>
        /// <param name="expected">The expected Type.</param>
        /// <param name="actual">The object under examination</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        static public void IsNotAssignableFrom(System.Type expected, object actual, string message, params object[] args)
        {
            try
            {
                IsAssignableFrom(expected, actual, message, args);

                //Do fail before now...
                Assert.Fail(message);
            }
            catch (AssertionException)
            {
                //Do Nothing as expected
            }
        }
        #endregion

        #region IsInstanceOfType
        /// <summary>
        /// Asserts that an object is an instance of a given type.
        /// </summary>
        /// <param name="expected">The expected Type</param>
        /// <param name="actual">The object being examined</param>
        public static void IsInstanceOfType(System.Type expected, object actual)
        {
            IsInstanceOfType(expected, actual, string.Empty, null);
        }

        /// <summary>
        /// Asserts that an object is an instance of a given type.
        /// </summary>
        /// <param name="expected">The expected Type</param>
        /// <param name="actual">The object being examined</param>
        /// <param name="message">A message to display in case of failure</param>
        public static void IsInstanceOfType(System.Type expected, object actual, string message)
        {
            IsInstanceOfType(expected, actual, message, null);
        }

        /// <summary>
        /// Asserts that an object is an instance of a given type.
        /// </summary>
        /// <param name="expected">The expected Type</param>
        /// <param name="actual">The object being examined</param>
        /// <param name="message">A message to display in case of failure</param>
        /// <param name="args">An array of objects to be used in formatting the message</param>
        public static void IsInstanceOfType(System.Type expected, object actual, string message, params object[] args)
        {
            if(!expected.IsInstanceOfType(actual))
            {
                Assert.Fail(message);
            }
        }
        #endregion

        #region IsNotInstanceOfType
        /// <summary>
        /// Asserts that an object is not an instance of a given type.
        /// </summary>
        /// <param name="expected">The expected Type</param>
        /// <param name="actual">The object being examined</param>
        public static void IsNotInstanceOfType(System.Type expected, object actual)
        {
            IsNotInstanceOfType(expected, actual, string.Empty, null);
        }

        /// <summary>
        /// Asserts that an object is not an instance of a given type.
        /// </summary>
        /// <param name="expected">The expected Type</param>
        /// <param name="actual">The object being examined</param>
        /// <param name="message">A message to display in case of failure</param>
        public static void IsNotInstanceOfType(System.Type expected, object actual, string message)
        {
            IsNotInstanceOfType(expected, actual, message, null);
        }

        /// <summary>
        /// Asserts that an object is not an instance of a given type.
        /// </summary>
        /// <param name="expected">The expected Type</param>
        /// <param name="actual">The object being examined</param>
        /// <param name="message">A message to display in case of failure</param>
        /// <param name="args">An array of objects to be used in formatting the message</param>
        public static void IsNotInstanceOfType(System.Type expected, object actual, string message, params object[] args)
        {
            try
            {
                IsInstanceOfType(expected, actual, message, args);

                //Do fail before now...
                Assert.Fail(message);
            }
            catch (AssertionException)
            {
                //Do Nothing as expected
            }
        }
        #endregion

        #endregion

        #region Assert Count
        public static int AssertCount
        {
            get { return Assert.assertCount; }
        }
        public static void ResetAssertCount()
        {
            Assert.assertCount = 0;
        }
        public static void IncrementAssertCount()
        {
            Assert.assertCount++;
        }
        #endregion

        //#region Warnings
        //public static void Warning(string format, params object[] args)
        //{
        //    if (format == null)
        //        throw new ArgumentNullException("format");

        //    string message = String.Format(format, args);
        //    Warning(message);
        //}

        //public static void Warning(string message)
        //{
        //    if (message == null)
        //        throw new ArgumentNullException("message");
        //    ReportWarning warning = new ReportWarning();
        //    warning.Text = message;
        //    warnings.Add(warning);
        //}
        //internal static void ClearCounters()
        //{
        //    warnings.Clear();
        //    assertCount = 0;
        //}
        //internal static void FlushWarnings(ReportRun run)
        //{
        //    if (run != null)
        //    {
        //        foreach (ReportWarning warning in warnings)
        //            run.Warnings.AddReportWarning(warning);
        //    }
        //    warnings.Clear();
        //}
        //#endregion
    
    	#region Private Method
		/// <summary>
		/// This method is called when two objects have been compared and found to be
		/// different. This prints a nice message to the screen. 
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The actual object</param>
        /// <param name="format">
        /// The format of the message to display if the assertion fails,
        /// containing zero or more format items. 
        /// </param>
        /// <param name="args">
        /// An <see cref="Object"/> array containing zero or more objects to format. 
        /// </param>
        /// <remarks>
        /// <para>
        /// The error message is formatted using <see cref="String.Format(string, object[])"/>.
        /// </para>
        /// </remarks>
        static private void FailNotEquals(Object expected, Object actual, string format, params object[] args) 
		{
			throw new NotEqualAssertionException(expected,actual,
				String.Format(format,args));
		}
    
		/// <summary>
		///  This method is called when the two objects are not the same. 
		/// </summary>
		/// <param name="expected">The expected object</param>
		/// <param name="actual">The actual object</param>
        /// <param name="format">
        /// The format of the message to display if the assertion fails,
        /// containing zero or more format items. 
        /// </param>
        /// <param name="args">
        /// An <see cref="Object"/> array containing zero or more objects to format. 
        /// </param>
        /// <remarks>
        /// <para>
        /// The error message is formatted using <see cref="String.Format(string, object[])"/>.
        /// </para>
        /// </remarks>
        static private void FailNotSame(Object expected, Object actual, string format, params object[] args) 
		{
			string formatted=string.Empty;
			if (format != null)
				formatted= format+" ";
			Assert.Fail(format+"expected same",args);
		}

        static private void FailSame(Object expected, Object actual, string format, params object[] args)
        {
            string formatted = string.Empty;
            if (format != null)
                formatted = format + " ";
            Assert.Fail(format + "expected not same", args);
        }


        static private void FailIsEmpty(Object expected, string format, params object[] args)
        {
            string formatted = string.Empty;
            if (format != null)
                formatted = format + " ";
            Assert.Fail(format + "expected not to be empty", args);
        }

        static private void FailIsNotEmpty(Object expected, string format, params object[] args)
        {
            string formatted = string.Empty;
            if (format != null)
                formatted = format + " ";
            Assert.Fail(format + "expected to be empty", args);
        }
		#endregion
	}
}
