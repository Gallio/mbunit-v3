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

// MbUnit project.
// http://www.mbunit.org

using MbUnit.Framework;
using System;
using System.Collections;

namespace MbUnit.Framework
{
	/// <summary>
	/// Assertion helper for the <see cref="ICollection"/> class.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class contains static helper methods to verify assertions on the
	/// <see cref="ICollection"/> class.
	/// </para>
	/// </remarks>
	public sealed class CollectionAssert
	{
		#region Private constructor
		private CollectionAssert()
		{}		
		#endregion

        #region Synchronized

        /// <summary>
		/// Verifies that the property value <see cref="ICollection.SyncRoot"/>
		/// of <paramref name="expected"/> and <paramref name="actual"/> are equal.
		/// </summary>
		/// <param name="expected">
		/// Instance containing the expected value.
		/// </param>
		/// <param name="actual">
		/// Instance containing the tested value.
		/// </param>
		public static void AreSyncRootEqual(
			ICollection expected,
			ICollection actual
			)
		{
			if (expected==null && actual==null)
				return;
			
			Assert.IsNotNull(expected);
			Assert.IsNotNull(actual);
			AreSyncRootEqual(expected.SyncRoot,actual);
		}
		
		/// <summary>
		/// Verifies that the property value <see cref="ICollection.SyncRoot"/>
		/// of <paramref name="actual"/> is equal to <paramref name="expected"/>.
		/// </summary>
		/// <param name="expected">
		/// Expected value.
		/// </param>
		/// <param name="actual">
		/// Instance containing the tested value.
		/// </param>
		public static void AreSyncRootEqual(
			System.Object expected,
			ICollection actual
			)
		{
			if (expected==null && actual==null)
				return;
			
			Assert.IsNotNull(expected);
			Assert.IsNotNull(actual);
			Assert.AreEqual(expected,actual.SyncRoot,
						"Property SyncRoot not equal");
			
		}		
		/// <summary>
		/// Verifies that the property value <see cref="ICollection.IsSynchronized"/>
		/// is true.
		/// </summary>
		/// <param name="actual">
		/// Instance containing the expected value.
		/// </param>
		public static void IsSynchronized(
			ICollection actual
			)
		{		
			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.IsSynchronized,
						  "Property IsSynchronized is false");
		}

		/// <summary>
		/// Verifies that the property value <see cref="ICollection.IsSynchronized"/>
		/// is false.
		/// </summary>
		/// <param name="actual">
		/// Instance containing the expected value.
		/// </param>
		public static void IsNotSynchronized(
			ICollection actual
			)
		{		
			Assert.IsNotNull(actual);
			Assert.IsFalse(actual.IsSynchronized,
						  "Property IsSynchronized is true");			
		}

		/// <summary>
		/// Verifies that the property value <see cref="ICollection.IsSynchronized"/>
		/// of <paramref name="expected"/> and <paramref name="actual"/> are equal.
		/// </summary>
		/// <param name="expected">
		/// Instance containing the expected value.
		/// </param>
		/// <param name="actual">
		/// Instance containing the tested value.
		/// </param>
		public static void AreIsSynchronizedEqual(
			ICollection expected,
			ICollection actual
			)
		{
			if (expected==null && actual==null)
				return;
			
			Assert.IsNotNull(expected);
			Assert.IsNotNull(actual);
			AreIsSynchronizedEqual(expected.IsSynchronized,actual);
		}
		
		/// <summary>
		/// Verifies that the property value <see cref="ICollection.IsSynchronized"/>
		/// of <paramref name="actual"/> is equal to <paramref name="expected"/>.
		/// </summary>
		/// <param name="expected">
		/// Expected value.
		/// </param>
		/// <param name="actual">
		/// Instance containing the tested value.
		/// </param>
		public static void AreIsSynchronizedEqual(
			System.Boolean expected,
			ICollection actual
			)
		{			
			Assert.IsNotNull(actual);
			Assert.AreEqual(expected,actual.IsSynchronized,
						"Property IsSynchronized not equal");

        }

        #endregion
        
        #region Count

        /// <summary>
        /// Verifies that the property value <see cref="ICollection.Count"/>
        /// of <paramref name="expected"/> and <paramref name="actual"/> are equal.
        /// </summary>
        /// <param name="expected">
        /// Instance containing the expected value.
        /// </param>
        /// <param name="actual">
        /// Instance containing the tested value.
        /// </param>
        public static void AreCountEqual(
            ICollection expected,
            ICollection actual
            )
        {
            if (expected == null && actual == null)
                return;

            Assert.IsNotNull(expected);
            Assert.IsNotNull(actual);
            AreCountEqual(expected.Count, actual);
        }

        /// <summary>
        /// Verifies that the property value <see cref="ICollection.Count"/>
        /// of <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">
        /// Expected value.
        /// </param>
        /// <param name="actual">
        /// Instance containing the tested value.
        /// </param>
        public static void AreCountEqual(
            System.Int32 expected,
            ICollection actual
            )
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Count,
                        "Property Count not equal");

        }

        /// <summary>
        /// Verifies that the <see cref="ICollection.Count"/> property
        /// is synchronized with the number of iterated elements.
        /// </summary>
        /// <param name="col">
        /// Collection to test
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="col"/> is a null reference (Nothing in Visual Basic)
        /// </exception>
        public static void IsCountCorrect(ICollection col)
        {
            if (col == null)
                throw new ArgumentNullException("col");
            int i = 0;
            foreach (Object o in col)
                ++i;
            Assert.AreEqual(i, col.Count);
        }
        #endregion
        
        #region AreEqual

        /// <summary>
        /// Verifies that <paramref name="expected"/> and <paramref name="actual"/>
        /// are equal collections. Element count and element wize equality is verified.
        /// </summary>
        /// <param name="expected">
        /// Expected value.
        /// </param>
        /// <param name="actual">
        /// Instance containing the tested value.
        /// </param>
        public static void AreEqual(
            ICollection expected,
            ICollection actual
            )
        {
            if (expected == null && actual == null)
                return;

            Assert.IsNotNull(expected);
            Assert.IsNotNull(actual);
            AreCountEqual(expected, actual);
            AreElementsEqual(expected, actual);
        }

        /// <summary>
        /// Verifies that <paramref name="expected"/> and <paramref name="actual"/>
        /// are equal collections. Element count and element wize equality is verified.
        /// </summary>
        /// <param name="expected">
        /// Expected value.
        /// </param>
        /// <param name="actual">
        /// Instance containing the tested value.
        /// </param>
        public static void AreElementsEqual(
            IEnumerable expected,
            IEnumerable actual
            )
        {
            if (expected == null && actual == null)
                return;

            Assert.IsNotNull(expected);
            Assert.IsNotNull(actual);

            IEnumerator expectedEn = null;
            IEnumerator actualEn = null;
            try
            {
                expectedEn = expected.GetEnumerator();
                actualEn = actual.GetEnumerator();

                bool exMoveNext;
                bool acMoveNext;
                do
                {
                    exMoveNext = expectedEn.MoveNext();
                    acMoveNext = actualEn.MoveNext();
                    Assert.AreEqual(exMoveNext, acMoveNext,
                                    "Collection do not have the same number of elements");

                    if (exMoveNext)
                    {
                        Assert.AreEqual(expectedEn.Current, actualEn.Current,
                                        "Element of the collection different");
                    }
                } while (exMoveNext);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                IDisposable disp = expectedEn as IDisposable;
                if (disp != null)
                    disp.Dispose();
                disp = actualEn as IDisposable;
                if (disp != null)
                    disp.Dispose();

            }
        }

        #endregion


        //NUnit Comments and Signuates

        #region AllItemsAreInstancesOfType
        /// <summary>
        /// Asserts that all items contained in collection are of the type specified by expectedType.
        /// </summary>
        /// <param name="collection">ICollection of objects to be considered</param>
        /// <param name="expectedType">System.Type that all objects in collection must be instances of</param>
        public static void AllItemsAreInstancesOfType(ICollection collection, Type expectedType)
        {
            AllItemsAreInstancesOfType(collection, expectedType, string.Empty, null);
        }

        /// <summary>
        /// Asserts that all items contained in collection are of the type specified by expectedType.
        /// </summary>
        /// <param name="collection">ICollection of objects to be considered</param>
        /// <param name="expectedType">System.Type that all objects in collection must be instances of</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void AllItemsAreInstancesOfType(ICollection collection, Type expectedType, string message)
        {
            AllItemsAreInstancesOfType(collection, expectedType, message, null);
        }

        /// <summary>
        /// Asserts that all items contained in collection are of the type specified by expectedType.
        /// </summary>
        /// <param name="collection">ICollection of objects to be considered</param>
        /// <param name="expectedType">System.Type that all objects in collection must be instances of</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void AllItemsAreInstancesOfType(ICollection collection, Type expectedType, string message, params object[] args)
        {
            bool fail = false;

            foreach (object o in collection)
            {
                if (o.GetType() != expectedType)
                {
                    fail = true;
                    break;
                }
            }

            if (fail)
            {
                if(args != null)
                    Assert.Fail(message, args);
                else
                    Assert.Fail(message);
            }
        }
        #endregion

        #region AllItemsAreNotNull

        /// <summary>
        /// Asserts that all items contained in collection are not equal to null.
        /// </summary>
        /// <param name="collection">ICollection of objects to be considered</param>
        public static void AllItemsAreNotNull(ICollection collection)
        {
            AllItemsAreNotNull(collection, string.Empty, null);
        }

        /// <summary>
        /// Asserts that all items contained in collection are not equal to null.
        /// </summary>
        /// <param name="collection">ICollection of objects to be considered</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void AllItemsAreNotNull(ICollection collection, string message)
        {
            AllItemsAreNotNull(collection, message, null);
        }

        /// <summary>
        /// Asserts that all items contained in collection are not equal to null.
        /// </summary>
        /// <param name="collection">ICollection of objects to be considered</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void AllItemsAreNotNull(ICollection collection, string message, params object[] args)
        {
            bool fail = false;

            foreach (object o in collection)
            {
                if (o == null)
                {
                    fail = true;
                    break;
                }
            }

            if (fail)
            {
                if (args != null)
                    Assert.Fail(message, args);
                else
                    Assert.Fail(message);
            }
        }
        #endregion

        #region AllItemsAreUnique

        /// <summary>
        /// Ensures that every object contained in collection exists within the collection
        /// once and only once.
        /// </summary>
        /// <param name="collection">ICollection of objects to be considered</param>
        public static void AllItemsAreUnique(ICollection collection)
        {
            AllItemsAreUnique(collection, string.Empty, null);
        }

        /// <summary>
        /// Ensures that every object contained in collection exists within the collection
        /// once and only once.
        /// </summary>
        /// <param name="collection">ICollection of objects to be considered</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void AllItemsAreUnique(ICollection collection, string message)
        {
            AllItemsAreUnique(collection, message, null);
        }

        /// <summary>
        /// Ensures that every object contained in collection exists within the collection
        /// once and only once.
        /// </summary>
        /// <param name="collection">ICollection of objects to be considered</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void AllItemsAreUnique(ICollection collection, string message, params object[] args)
        {
            bool fail = false;
            ArrayList arr = new ArrayList();

            foreach (object o in collection)
            {
                //do a check to see if it is in the collection already
                foreach (object subO in collection)
                {
                    if (o == subO)
                    {
                        if (arr.Contains(o))
                        {
                            fail = true;
                            break;
                        }
                        else
                        {
                            arr.Add(o);
                        }
                    }
                }
            }

            if (fail)
            {
                if (args != null)
                    Assert.Fail(message, args);
                else
                    Assert.Fail(message);
            }
        }
        #endregion

        #region AreEquivalent

        /// <summary>
        /// Asserts that expected and actual are equivalent, containing the same objects but the match may be in any order.
        /// </summary>
        /// <param name="expected">The first ICollection of objects to be considered</param>
        /// <param name="actual">The second ICollection of objects to be considered</param>
        public static void AreEquivalent(ICollection expected, ICollection actual)
        {
            AreEquivalent(expected, actual, string.Empty, null);
        }

        /// <summary>
        /// Asserts that expected and actual are equivalent, containing the same objects but the match may be in any order.
        /// </summary>
        /// <param name="expected">The first ICollection of objects to be considered</param>
        /// <param name="actual">The second ICollection of objects to be considered</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void AreEquivalent(ICollection expected, ICollection actual, string message)
        {
            AreEquivalent(expected, actual, message, null);
        }

        /// <summary>
        /// Asserts that expected and actual are equivalent, containing the same objects but the match may be in any order.
        /// </summary>
        /// <param name="expected">The first ICollection of objects to be considered</param>
        /// <param name="actual">The second ICollection of objects to be considered</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void AreEquivalent(ICollection expected, ICollection actual, string message, params object[] args)
        {
            bool found = true;
            bool foundAll = true;

            foreach (object o in expected)
            {
                found = false;

                //do a check to see if it is in the collection already
                foreach (object subO in actual)
                {
                    if (o == subO)
                    {
                        found = true;
                    }
                }

                if (found)
                    foundAll = true;
                else
                    foundAll = false;
            }

            if (foundAll)
            {
                foreach (object o in actual)
                {
                    found = false;

                    //do a check to see if it is in the collection already
                    foreach (object subO in expected)
                    {
                        if (o == subO)
                        {
                            found = true;
                        }
                    }

                    if (found && foundAll)
                        foundAll = true;
                    else
                        foundAll = false;
                }
            }

            if (!foundAll)
            {
                if (args != null)
                    Assert.Fail(message, args);
                else
                    Assert.Fail(message);
            }
        }
        #endregion

        #region AreNotEqual

        /// <summary>
        /// Asserts that expected and actual are not exactly equal.
        /// </summary>
        /// <param name="expected">The first ICollection of objects to be considered</param>
        /// <param name="actual">The second ICollection of objects to be considered</param>
        public static void AreNotEqual(ICollection expected, ICollection actual)
        {
            AreNotEqual(expected, actual, null, string.Empty, null);
        }

        /// <summary>
        /// Asserts that expected and actual are not exactly equal.
        /// If comparer is not null then it will be used to compare the objects.
        /// </summary>
        /// <param name="expected">The first ICollection of objects to be considered</param>
        /// <param name="actual">The second ICollection of objects to be considered</param>
        /// <param name="comparer">The IComparer to use in comparing objects from each ICollection</param>
        public static void AreNotEqual(ICollection expected, ICollection actual, IComparer comparer)
        {
            AreNotEqual(expected, actual, comparer, string.Empty, null);
        }

        /// <summary>
        /// Asserts that expected and actual are not exactly equal.
        /// </summary>
        /// <param name="expected">The first ICollection of objects to be considered</param>
        /// <param name="actual">The second ICollection of objects to be considered</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void AreNotEqual(ICollection expected, ICollection actual, string message)
        {
            AreNotEqual(expected, actual, null, message, null);
        }

        /// <summary>
        /// Asserts that expected and actual are not exactly equal.
        /// If comparer is not null then it will be used to compare the objects.
        /// </summary>
        /// <param name="expected">The first ICollection of objects to be considered</param>
        /// <param name="actual">The second ICollection of objects to be considered</param>
        /// <param name="comparer">The IComparer to use in comparing objects from each ICollection</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void AreNotEqual(ICollection expected, ICollection actual, IComparer comparer, string message)
        {
            AreNotEqual(expected, actual, comparer, message, null);
        }

        /// <summary>
        /// Asserts that expected and actual are not exactly equal.
        /// </summary>
        /// <param name="expected">The first ICollection of objects to be considered</param>
        /// <param name="actual">The second ICollection of objects to be considered</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void AreNotEqual(ICollection expected, ICollection actual, string message, params object[] args)
        {
            AreNotEqual(expected, actual, null, message, args);
        }

        /// <summary>
        /// Asserts that expected and actual are not exactly equal.
        /// If comparer is not null then it will be used to compare the objects.
        /// </summary>
        /// <param name="expected">The first ICollection of objects to be considered</param>
        /// <param name="actual">The second ICollection of objects to be considered</param>
        /// <param name="comparer">The IComparer to use in comparing objects from each ICollection</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void AreNotEqual(ICollection expected, ICollection actual, IComparer comparer, string message, params object[] args)
        {
            try
            {
                AreEqual(expected, actual);

                //Do fail before now...
                Assert.Fail(message);
            }
            catch (AssertionException)
            {
                //Do Nothing as expected
            }
        }
        #endregion

        #region AreNotEquivalent

        /// <summary>
        /// Asserts that expected and actual are not equivalent.
        /// </summary>
        /// <param name="expected">The first ICollection of objects to be considered</param>
        /// <param name="actual">The second ICollection of objects to be considered</param>
        public static void AreNotEquivalent(ICollection expected, ICollection actual)
        {
            AreNotEquivalent(expected, actual, string.Empty, null);
        }

        /// <summary>
        /// Asserts that expected and actual are not equivalent.
        /// </summary>
        /// <param name="expected">The first ICollection of objects to be considered</param>
        /// <param name="actual">The second ICollection of objects to be considered</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void AreNotEquivalent(ICollection expected, ICollection actual, string message)
        {
            AreNotEquivalent(expected, actual, message, null);
        }

        /// <summary>
        /// Asserts that expected and actual are not equivalent.
        /// </summary>
        /// <param name="expected">The first ICollection of objects to be considered</param>
        /// <param name="actual">The second ICollection of objects to be considered</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void AreNotEquivalent(ICollection expected, ICollection actual, string message, params object[] args)
        {
            try
            {
                AreEquivalent(expected, actual, message, args);

                //Do fail before now...
                Assert.Fail(message);
            }
            catch (AssertionException)
            {
                //Do Nothing as expected
            }
        }
        #endregion

        #region Contains

        /// <summary>
        /// Asserts that collection contains actual as an item.
        /// </summary>
        /// <param name="collection">ICollection of objects to be considered</param>
        /// <param name="actual">Object to be found within collection</param>
        public static void Contains(ICollection collection, Object actual)
        {
            Contains(collection, actual, string.Empty, null);
        }

        /// <summary>
        /// Asserts that collection contains actual as an item.
        /// </summary>
        /// <param name="collection">ICollection of objects to be considered</param>
        /// <param name="actual">Object to be found within collection</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void Contains(ICollection collection, Object actual, string message)
        {
            Contains(collection, actual, message, null);
        }

        /// <summary>
        /// Asserts that collection contains actual as an item.
        /// </summary>
        /// <param name="collection">ICollection of objects to be considered</param>
        /// <param name="actual">Object to be found within collection</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void Contains(ICollection collection, Object actual, string message, params object[] args)
        {
            bool found = false;

            foreach (object o in collection)
            {
                if (o == actual)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                if (args != null)
                    Assert.Fail(message, args);
                else
                    Assert.Fail(message);
            }
        }
        #endregion

        #region DoesNotContain

        /// <summary>
        /// Asserts that collection does not contain actual as an item.
        /// </summary>
        /// <param name="collection">ICollection of objects to be considered</param>
        /// <param name="actual">Object that cannot exist within collection</param>
        public static void DoesNotContain(ICollection collection, Object actual)
        {
            DoesNotContain(collection, actual, string.Empty, null);
        }

        /// <summary>
        /// Asserts that collection does not contain actual as an item.
        /// </summary>
        /// <param name="collection">ICollection of objects to be considered</param>
        /// <param name="actual">Object that cannot exist within collection</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void DoesNotContain(ICollection collection, Object actual, string message)
        {
            DoesNotContain(collection, actual, message, null);
        }

        /// <summary>
        /// Asserts that collection does not contain actual as an item.
        /// </summary>
        /// <param name="collection">ICollection of objects to be considered</param>
        /// <param name="actual">Object that cannot exist within collection</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void DoesNotContain(ICollection collection, Object actual, string message, params object[] args)
        {
            try
            {
                Contains(collection, actual, message, args);

                //Do fail before now...
                Assert.Fail(message);
            }
            catch (AssertionException)
            {
                //Do Nothing as expected
            }
        }
        #endregion

        #region IsNotSubsetOf

        /// <summary>
        /// Asserts that superset is not a subject of subset.
        /// </summary>
        /// <param name="subset">The ICollection superset to be considered</param>
        /// <param name="superset">The ICollection subset to be considered</param>
        public static void IsNotSubsetOf(ICollection subset, ICollection superset)
        {
            IsNotSubsetOf(subset, superset, string.Empty, null);
        }

        /// <summary>
        /// Asserts that superset is not a subject of subset.
        /// </summary>
        /// <param name="subset">The ICollection superset to be considered</param>
        /// <param name="superset">The ICollection subset to be considered</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void IsNotSubsetOf(ICollection subset, ICollection superset, string message)
        {
            IsNotSubsetOf(subset, superset, message, null);
        }

        /// <summary>
        /// Asserts that superset is not a subject of subset.
        /// </summary>
        /// <param name="subset">The ICollection superset to be considered</param>
        /// <param name="superset">The ICollection subset to be considered</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void IsNotSubsetOf(ICollection subset, ICollection superset, string message, params object[] args)
        {
            try
            {
                IsSubsetOf(subset, superset, message, args);

                //Do fail before now...
                Assert.Fail(message);
            }
            catch (AssertionException)
            {
                //Do Nothing as expected
            }
        }
        #endregion

        #region IsSubsetOf

        /// <summary>
        /// Asserts that superset is a subset of subset.
        /// </summary>
        /// <param name="subset">The ICollection superset to be considered</param>
        /// <param name="superset">The ICollection subset to be considered</param>
        public static void IsSubsetOf(ICollection subset, ICollection superset)
        {
            IsSubsetOf(subset, superset, string.Empty, null);
        }

        /// <summary>
        /// Asserts that superset is a subset of subset.
        /// </summary>
        /// <param name="subset">The ICollection superset to be considered</param>
        /// <param name="superset">The ICollection subset to be considered</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void IsSubsetOf(ICollection subset, ICollection superset, string message)
        {
            IsSubsetOf(subset, superset, message, null);
        }

        /// <summary>
        /// Asserts that superset is a subset of subset.
        /// </summary>
        /// <param name="subset">The ICollection superset to be considered</param>
        /// <param name="superset">The ICollection subset to be considered</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void IsSubsetOf(ICollection subset, ICollection superset, string message, params object[] args)
        {
            bool found = false;
            bool foundAll = true;

            //All of superset are in subset
            foreach (object o in superset)
            {
                found = false;

                foreach (object supO in subset)
                {
                    if (o == supO)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                    foundAll = true;
                else
                    foundAll = false;
            }

            if (foundAll)
            {
                if (args != null)
                    Assert.Fail(message, args);
                else
                    Assert.Fail(message);
            }
        }
        #endregion
    
    }
}
