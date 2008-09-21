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
using Gallio.Framework.Assertions;

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
    [Obsolete("Use Assert instead.")]
    public static class OldCollectionAssert
    {
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
            if (expected == null && actual == null)
                return;

            OldAssert.IsNotNull(expected);
            OldAssert.IsNotNull(actual);
            AreSyncRootEqual(expected.SyncRoot, actual);
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
            Object expected,
            ICollection actual
            )
        {
            if (expected == null && actual == null)
                return;

            OldAssert.IsNotNull(expected);
            OldAssert.IsNotNull(actual);
            OldAssert.AreEqual(expected, actual.SyncRoot,
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
            OldAssert.IsNotNull(actual);
            OldAssert.IsTrue(actual.IsSynchronized,
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
            OldAssert.IsNotNull(actual);
            OldAssert.IsFalse(actual.IsSynchronized,
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
            if (expected == null && actual == null)
                return;

            OldAssert.IsNotNull(expected);
            OldAssert.IsNotNull(actual);
            AreIsSynchronizedEqual(expected.IsSynchronized, actual);
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
            Boolean expected,
            ICollection actual
            )
        {
            OldAssert.IsNotNull(actual);
            OldAssert.AreEqual(expected, actual.IsSynchronized,
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

            OldAssert.IsNotNull(expected);
            OldAssert.IsNotNull(actual);
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
            Int32 expected,
            ICollection actual
            )
        {
            OldAssert.IsNotNull(actual);
            OldAssert.AreEqual(expected, actual.Count,
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
            OldAssert.AreEqual(i, col.Count);
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

            OldAssert.IsNotNull(expected);
            OldAssert.IsNotNull(actual);
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
            string failMessage;
            bool areEqual = ElementsEqual(expected, actual, out failMessage);
            if (!areEqual) OldAssert.Fail(failMessage);
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
        /// <param name="failMessage">
        /// Reason for unequality.
        /// </param> 
        internal static bool ElementsEqual(IEnumerable expected, IEnumerable actual, out string failMessage)
        {
            failMessage = string.Empty;
            if (expected == null && actual == null)
                return true;
            OldAssert.IsNotNull(expected);
            OldAssert.IsNotNull(actual);

            IEnumerator expectedEnumerator = null;
            IEnumerator actualEnumerator = null;
            try
            {
                expectedEnumerator = expected.GetEnumerator();
                actualEnumerator = actual.GetEnumerator();

                bool expectedHasElement;
                do
                {
                    expectedHasElement = expectedEnumerator.MoveNext();
                    bool actualHasElement = actualEnumerator.MoveNext();
                    if(expectedHasElement != actualHasElement)
                    {
                        failMessage = "Collection do not have the same number of elements";
                        return false;
                    }
                    if (expectedHasElement)
                    {
                        bool equalElements = OldAssert.ObjectsEqual(expectedEnumerator.Current, actualEnumerator.Current);
                        if(!equalElements)
                        {
                            failMessage = "Element of the collection different";
                            return false;
                        }
                    }
                } while (expectedHasElement);
                return true;
            }
            finally
            {
                IDisposable disp = expectedEnumerator as IDisposable;
                if (disp != null)
                    disp.Dispose();
                disp = actualEnumerator as IDisposable;
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
                if (args != null)
                    OldAssert.Fail(message, args);
                else
                    OldAssert.Fail(message);
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
                    OldAssert.Fail(message, args);
                else
                    OldAssert.Fail(message);
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

            if (fail)
            {
                if (args != null)
                    OldAssert.Fail(message, args);
                else
                    OldAssert.Fail(message);
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
            bool found;
            bool foundAll = true;

            foreach (object o in expected)
            {
                //do a check to see if it is in the collection already
                found = CheckItemInCollection(actual, o);
                               
                if (!found)
                {
                    foundAll = false;
                    break;
                }                
            }

            if (foundAll)
            {
                foreach (object o in actual)
                {
                    found = CheckItemInCollection(expected, o);

                    if (!found)
                    {
                        foundAll = false;
                        break;
                    }                    
                }
            }

            if (!foundAll)
            {
                if (args != null)
                    OldAssert.Fail(message, args);
                else
                    OldAssert.Fail(message);
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
            bool needToFail = false;
            
            try
            {
                AreEqual(expected, actual);                                
                needToFail = true;                
            }
            catch (AssertionException)
            {
                //Do Nothing as expected
            }
                        
            if (needToFail)
            {
                OldAssert.Fail(message, args);
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
            
            bool needToFail = false;
            
            try
            {
                AreEquivalent(expected, actual, message, args);                                
                needToFail = true;
                
            }
            catch (AssertionException)
            {
                //Do Nothing as expected
            }
                        
            if (needToFail)
            {
                OldAssert.Fail(message, args);
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
            bool found;
            found = CheckItemInCollection(collection, actual);

            if (!found)
            {
                if (args != null)
                    OldAssert.Fail(message, args);
                else
                    OldAssert.Fail(message);
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
            
            bool needToFail = false;
            
            try
            {
                Contains(collection, actual, message, args);
                               
                needToFail = true;                
            }
            catch (AssertionException)
            {
                //Do Nothing as expected
            }
                        
            if (needToFail)
            {
                OldAssert.Fail(message, args);
            }
            
        }
        #endregion

        #region IsNotSubsetOf
                
        /// <summary>
        /// Asserts that subset is not a subset of superset.
        /// </summary>
        /// <param name="subset">The ICollection subset to be considered</param>
        /// <param name="superset">The ICollection superset to be considered</param>
        public static void IsNotSubsetOf(ICollection subset, ICollection superset)
        {
            IsNotSubsetOf(subset, superset, string.Empty, null);
        }

        /// <summary>
        /// Asserts that subset is not a subset of superset.
        /// </summary>
        /// <param name="subset">The ICollection subset to be considered</param>
        /// <param name="superset">The ICollection superset to be considered</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void IsNotSubsetOf(ICollection subset, ICollection superset, string message)
        {
            IsNotSubsetOf(subset, superset, message, null);
        }

        /// <summary>
        /// Asserts that subset is not a subset of superset.
        /// </summary>
        /// <param name="subset">The ICollection subset to be considered</param>
        /// <param name="superset">The ICollection superset to be considered</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void IsNotSubsetOf(ICollection subset, ICollection superset, string message, params object[] args)
        {
            
            bool needToFail = false;            
            try
            {
                IsSubsetOf(subset, superset, message, args);
                                
                needToFail = true;
                
            }
            catch (AssertionException)
            {
                //Do Nothing as expected
            }
                        
            if (needToFail)
            {
                OldAssert.Fail(message, args);
            }          
        }
       
        #endregion

        #region IsSubsetOf
             
        /// <summary>
        /// Asserts that subset is a subset of superset.
        /// </summary>
        /// <param name="subset">The ICollection subset to be considered</param>
        /// <param name="superset">The ICollection superset to be considered</param>
        public static void IsSubsetOf(ICollection subset, ICollection superset)
        {
            IsSubsetOf(subset, superset, string.Empty, null);
        }

        /// <summary>
        /// Asserts that subset is a subset of superset.
        /// </summary>
        /// <param name="subset">The ICollection subset to be considered</param>
        /// <param name="superset">The ICollection superset to be considered</param>
        /// <param name="message">The message that will be displayed on failure</param>
        public static void IsSubsetOf(ICollection subset, ICollection superset, string message)
        {
            IsSubsetOf(subset, superset, message, null);
        }

        /// <summary>
        /// Asserts that subset is a subset of superset.
        /// </summary>
        /// <param name="subset">The ICollection subset to be considered</param>
        /// <param name="superset">The ICollection superset to be considered</param>
        /// <param name="message">The message that will be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void IsSubsetOf(ICollection subset, ICollection superset, string message, params object[] args)
        {
            bool foundAll = true;

            //All of superset are in subset
            foreach (object o in subset)
            {
                bool found = CheckItemInCollection(superset, o);
                if (!found)
                {
                    foundAll = false;
                    break;
                }
            }

            if (!foundAll)
            {
                if (args != null)
                    OldAssert.Fail(message, args);
                else
                    OldAssert.Fail(message);
            }
        }
    
        #endregion

        /// <summary>
        /// Checks an item if included in a given collection.
        /// </summary>
        /// <param name="collection">The collection to check from</param>
        /// <param name="item">The item to be checked</param>
        /// <returns>True if item is included, False otherwise</returns>
        private static bool CheckItemInCollection(ICollection collection, object item)
        {
            //Reused Arraylist's implementation of Contains, uses Equals override and null checking of items
            IList list = new ArrayList(collection);
            return list.Contains(item);
        }
    }
}

