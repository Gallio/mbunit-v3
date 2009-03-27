// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using System.Reflection;
using Gallio.Collections;
using Gallio.Framework.Assertions;
using Gallio;
using System.Collections.ObjectModel;
using MbUnit.Framework.ContractVerifiers.Core;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// <para>
    /// Contract for verifying the implementation of the generic <see cref="ICollection{T}"/>.
    /// </para>
    /// <para>
    /// Built-in verifications:
    /// <list type="bullet">
    /// <item>
    /// <term>VerifyReadOnlyProperty</term>
    /// <description>The <see cref="ICollection{T}.IsReadOnly"/> property returns a value in accordance to 
    /// the expected result determined in the declaration of the contract verifier, by setting the property
    /// <see cref="CollectionContract{TCollection,TItem}.IsReadOnly"/>. By default, the collection is
    /// expected to be not read-only (items can be added and removed, and the collection to be cleared).</description>
    /// </item>
    /// <item>
    /// <term>AddShouldThrowException</term>
    /// <description>The read-only collection throws an exception when the method <see cref="ICollection{T}.Add"/> is called.
    /// <remarks>
    /// The test is not run when the contract property <see cref="CollectionContract{TCollection,TItem}.IsReadOnly"/> is set to 'false'.
    /// </remarks>
    /// </description>
    /// </item>
    /// <item>
    /// <term>RemoveShouldThrowException</term>
    /// <description>The read-only collection throws an exception when the method <see cref="ICollection{T}.Remove"/>
    /// is called.
    /// <remarks>
    /// The test is not run when the contract property <see cref="CollectionContract{TCollection,TItem}.IsReadOnly"/> is set to 'false'.
    /// </remarks>
    /// </description>
    /// </item>
    /// <item>
    /// <term>ClearShouldThrowException</term>
    /// <description>The read-only collection throws an exception when the method <see cref="ICollection{T}.Clear"/> is called. 
    /// <remarks>
    /// The test is not run when the contract property <see cref="CollectionContract{TCollection,TItem}.IsReadOnly"/> is set to 'false'.
    /// </remarks>
    /// </description>
    /// </item>
    /// <item>
    /// <term>AddNullArgument</term>
    /// <description>The collection throwns a <see cref="ArgumentNullException"/> when the method <see cref="ICollection{T}.Add"/>
    /// is called with a null reference item. 
    /// <remarks>
    /// The test is not run when the contract property <see cref="CollectionContract{TCollection,TItem}.AcceptNullReference"/> is set to 'true'.
    /// </remarks>
    /// </description>
    /// </item>
    /// <item>
    /// <term>RemoveNullArgument</term>
    /// <description>
    /// The collection throwns a <see cref="ArgumentNullException"/> when the method <see cref="ICollection{T}.Remove"/>
    /// is called with a null reference item.
    /// <remarks>
    /// The test is not run when the contract property <see cref="CollectionContract{TCollection,TItem}.AcceptNullReference"/> is set to 'true'.
    /// </remarks>
    /// </description>
    /// </item>
    /// <item>
    /// <term>ContainsNullArgument</term>
    /// <description>
    /// The collection throwns a <see cref="ArgumentNullException"/> when the method <see cref="ICollection{T}.Contains"/> 
    /// is called with a null reference item. 
    /// <remarks>
    /// The test is not run when the contract property 
    /// <see cref="CollectionContract{TCollection,TItem}.AcceptNullReference"/> is set to 'true'.
    /// </remarks>
    /// </description>
    /// </item>
    /// <item>
    /// <term>AddItems</term>
    /// <description>
    /// The collection handles correctly with the addition of new items. The method 
    /// <see cref="ICollection{T}.Contains"/> and the property <see cref="ICollection{T}.Count"/> are expected
    /// to return suited results as well. The case of duplicate items (object equality) is tested too; according
    /// to the value of contract property <see cref="CollectionContract{TCollection,TItem}.AcceptEqualItems"/>.
    /// <remarks>
    /// The test is not run when the contract property <see cref="CollectionContract{TCollection,TItem}.IsReadOnly"/> is set to 'true'.
    /// </remarks>
    /// </description>
    /// </item>
    /// <item>
    /// <term>RemoveItems</term>
    /// The collection handles correctly with the removal of items. The method 
    /// <see cref="ICollection{T}.Contains"/> and the property <see cref="ICollection{T}.Count"/> are expected
    /// to return suited results as well.
    /// <remarks>
    /// The test is not run when the contract property <see cref="CollectionContract{TCollection,TItem}.IsReadOnly"/> is set to 'true'.
    /// </remarks>
    /// </item>
    /// <item>
    /// <term>ClearItems</term>
    /// <description>
    /// The collection is cleared as expected when the method <see cref="ICollection{T}.Clear"/> is called.
    /// <remarks>
    /// The test is not run when the contract property <see cref="CollectionContract{TCollection,TItem}.IsReadOnly"/> is set to 'true'.
    /// </remarks>
    /// </description>
    /// </item>
    /// <item>
    /// <term>CopyTo</term>
    /// <description>
    /// The collection performs a copy of the items in an output array. The implementation is expected to
    /// handle properly with null arguments, and a valid or an invalid index argument.
    /// </description>
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
    /// <typeparam name="TCollection">The type of the collection implementing <see cref="ICollection{T}"/>.</typeparam>
    /// <typeparam name="TItem">The type of items contained in the collection.</typeparam>
    public class CollectionContract<TCollection, TItem> : AbstractContract
        where TCollection : ICollection<TItem>
    {
        /// <summary>
        /// <para>
        /// Provides a default instance of the collection to test.
        /// By default, the contract verifier attempts to invoke the default constructor to get an valid instance. 
        /// Overwrite the default provider if the collection has no default constructor, or if you want 
        /// the contract verifier to use a particular instance.
        /// <example>
        /// <code><![CDATA[
        /// [VerifyContract]
        /// public readonly IContract CollectionTests = new CollectionContract<MyCollection, int>
        /// {
        ///     GetDefaultInstance = () => new MyCollection(1, 2, 3)
        /// };
        /// ]]></code>
        /// </example>
        /// </para>
        /// </summary>
        public Func<TCollection> GetDefaultInstance
        {
            get;
            set;
        }

        /// <summary>
        /// <para>
        /// Determines whether the tested collection is expected to be read-only.
        /// </para>
        /// <remarks>
        /// The default value is 'false'.
        /// </remarks>
        /// </summary>
        public bool IsReadOnly
        {
            get;
            set;
        }

        /// <summary>
        /// <para>
        /// Determines whether the collection is expected to accept null references as valid items.
        /// </para>
        /// <remarks>
        /// The default value is 'false'.
        /// </remarks>
        /// </summary>
        public bool AcceptNullReference
        {
            get;
            set;
        }

        /// <summary>
        /// <para>
        /// Determines whether the collection is expected to accept several identical items (object equality).
        /// The default value is true.
        /// </para>
        /// <remarks>
        /// The default value is 'true'.
        /// </remarks>
        /// </summary>
        public bool AcceptEqualItems
        {
            get;
            set;
        }

        /// <summary>
        /// <para>
        /// Gets a collection of distinct object instances that feeds the different tests.
        /// </para>
        /// <para>
        /// In order to optimize the tests, consider to provide:
        /// <list type="bullet">
        /// <item>
        /// items which are not in the default collection instance yet.
        /// </item>
        /// <item>
        /// items which are in the default collection instance already (if not empty).
        /// </item>
        /// </list>
        /// </para>
        /// </summary>
        public DistinctInstanceCollection<TItem> DistinctInstances
        {
            get;
            set;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CollectionContract()
        {
            DistinctInstances = new DistinctInstanceCollection<TItem>();
            GetDefaultInstance = () => Activator.CreateInstance<TCollection>();
            AcceptEqualItems = true;
        }

        /// <inheritdoc />
        protected override IEnumerable<Test> GetContractVerificationTests()
        {
            yield return CreateVerifyReadOnlyPropertyTest();

            if (IsReadOnly)
            {
                yield return CreateNotSupportedWriteTest("Add", (collection, item) => collection.Add(item));
                yield return CreateNotSupportedWriteTest("Remove", (collection, item) => collection.Remove(item));
                yield return CreateNotSupportedWriteTest("Clear", (collection, item) => collection.Clear());
            }
            else
            {
                if (!typeof(TItem).IsValueType && !AcceptNullReference)
                {
                    yield return CreateNullArgumentTest("Add", collection => collection.Add(default(TItem)));
                    yield return CreateNullArgumentTest("Remove", collection => collection.Remove(default(TItem)));
                    yield return CreateNullArgumentTest("Contains", collection => collection.Contains(default(TItem)));
                }

                yield return CreateClearTest();
                yield return CreateRemoveItemsTest();
                yield return CreateAddItemsTest();
            }

            yield return CreateCopyToTest();
        }

        /// <summary>
        /// Asserts that the collection of distinct instances specified by the user is not empty.
        /// </summary>
        protected void AssertDistinctIntancesNotEmpty()
        {
            AssertionHelper.Explain(() =>
                Assert.GreaterThan(DistinctInstances.Instances.Count, 0),
                innerFailures => new AssertionFailureBuilder("Expected the collection of distinct instances to be not empty.\n" +
                "Please feed the 'DistinctInstances' property of your collection contract with some valid objects.")
                .SetStackTrace(Context.GetStackTraceData())
                .AddInnerFailures(innerFailures)
                .ToAssertionFailure());
        }

        private Test CreateVerifyReadOnlyPropertyTest()
        {
            return new TestCase("VerifyReadOnlyProperty", () =>
            {
                var collection = GetDefaultInstance();

                AssertionHelper.Explain(() =>
                    Assert.AreEqual(IsReadOnly, collection.IsReadOnly),
                    innerFailures => new AssertionFailureBuilder(
                    "Expected the read-only property of the collection to return a specific value.")
                    .AddLabeledValue("Property", "IsReadOnly")
                    .AddRawLabeledValue("Expected Value", IsReadOnly)
                    .AddRawLabeledValue("Actual Value", collection.IsReadOnly)
                    .SetStackTrace(Context.GetStackTraceData())
                    .AddInnerFailures(innerFailures)
                    .ToAssertionFailure());
            });
        }

        /// <summary>
        /// Creates a test that invokes an action over the collection, which
        /// is supposed to not be supported. The test expects that an exception be thrown.
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="invoke"></param>
        /// <returns></returns>
        protected Test CreateNotSupportedWriteTest(string methodName, Action<TCollection, TItem> invoke)
        {
            return new TestCase(String.Format("{0}ShouldThrowException", methodName), () =>
            {
                AssertDistinctIntancesNotEmpty();

                foreach(var item in DistinctInstances)
                {
                    var collection = GetDefaultInstance();

                    AssertionHelper.Explain(() =>
                        Assert.Throws<Exception>(() => invoke(collection, item)),
                        innerFailures => new AssertionFailureBuilder(
                            "Expected a method of the read-only collection to throw an exception when called.")
                            .AddLabeledValue("Method", methodName)
                            .SetStackTrace(Context.GetStackTraceData())
                            .AddInnerFailures(innerFailures)
                            .ToAssertionFailure());
                }
            });
        }

        /// <summary>
        /// Creates a test which runs an action over the collection with
        /// a null argument. The test expects that an exception be thrown.
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="invoke"></param>
        /// <returns></returns>
        protected Test CreateNullArgumentTest(string methodName, Action<TCollection> invoke)
        {
            return new TestCase(String.Format("{0}NullArgument", methodName), () =>
            {
                var collection = GetDefaultInstance();

                AssertionHelper.Explain(() =>
                    Assert.Throws<ArgumentNullException>(() => invoke(collection)),
                    innerFailures => new AssertionFailureBuilder(
                        "Expected a method to throw an exception when called with a null argument.")
                        .AddLabeledValue("Method", methodName)
                        .AddRawLabeledValue("Expected Exception", typeof(ArgumentNullException))
                        .SetStackTrace(Context.GetStackTraceData())
                        .AddInnerFailures(innerFailures)
                        .ToAssertionFailure());
            });
        }

        private Test CreateClearTest()
        {
            return new TestCase("ClearItems", () =>
            {
                AssertDistinctIntancesNotEmpty();
                var collection = GetDefaultInstance();

                if (collection.Count == 0) 
                {
                    // The default instance is empty: add some items.
                    foreach (var item in DistinctInstances)
                    {
                        collection.Add(item);
                    }
                }

                collection.Clear();

                AssertionHelper.Explain(() =>
                    Assert.AreEqual(0, collection.Count),
                    innerFailures => new AssertionFailureBuilder(
                        "Expected the collection to be empty once the 'Clear' method was called on it.")
                        .AddLabeledValue("Property", "Count")
                        .AddRawLabeledValue("Expected Value", 0)
                        .AddRawLabeledValue("Actual Value", collection.Count)
                        .SetStackTrace(Context.GetStackTraceData())
                        .AddInnerFailures(innerFailures)
                        .ToAssertionFailure());
            });
        }

        private Test CreateAddItemsTest()
        {
            return new TestCase("AddItems", () =>
            {
                AssertDistinctIntancesNotEmpty();
                var collection = GetDefaultInstance();
                var handler = new CollectionHandler<TCollection, TItem>(collection, Context);
                var initialContent = new ReadOnlyCollection<TItem>(new List<TItem>(collection));

                foreach (var item in DistinctInstances)
                {
                    if (!initialContent.Contains(item))
                    {
                        handler.AddSingleItemOk(item);
                    }

                    if (AcceptEqualItems)
                    {
                        handler.AddDuplicateItemOk(item);
                    }
                    else
                    {
                        handler.AddDuplicateItemFails(item);
                    }
                }
            });
        }

        private Test CreateRemoveItemsTest()
        {
            return new TestCase("RemoveItems", () =>
            {
                AssertDistinctIntancesNotEmpty();
                var collection = GetDefaultInstance();
                var handler = new CollectionHandler<TCollection, TItem>(collection, Context);
                var initialContent = new ReadOnlyCollection<TItem>(new List<TItem>(collection));

                foreach (var item in DistinctInstances)
                {
                    if (!initialContent.Contains(item))
                    {
                        handler.RemoveMissingItemFails(item);
                        handler.AddSingleItemOk(item);
                    }

                    handler.RemoveItemOk(item);
                }
            });
        }

        private void AssertCopyToThrowException(Action action, string failureMessage, string label, object value)
        {
            AssertionHelper.Explain(() =>
                Assert.Throws<Exception>(() => action()),
                innerFailures => new AssertionFailureBuilder(
                    "Expected the method to throw an exception " + failureMessage)
                    .AddLabeledValue("Method", "CopyTo")
                    .AddRawLabeledValue(label, value)
                    .SetStackTrace(Context.GetStackTraceData())
                    .AddInnerFailures(innerFailures)
                    .ToAssertionFailure());
        }

        private void AssertCopyToNotThrowException(Action action, string failureMessage)
        {
            AssertionHelper.Explain(() =>
                Assert.DoesNotThrow(() => action()),
                innerFailures => new AssertionFailureBuilder(failureMessage)
                    .AddLabeledValue("Method", "CopyTo")
                    .SetStackTrace(Context.GetStackTraceData())
                    .AddInnerFailures(innerFailures)
                    .ToAssertionFailure());
        }

        private Test CreateCopyToTest()
        {
            return new TestCase("CopyTo", () =>
            {
                AssertDistinctIntancesNotEmpty();
                var collection = GetDefaultInstance();
                var handler = new CollectionHandler<TCollection, TItem>(collection, Context);
                var initialContent = new ReadOnlyCollection<TItem>(new List<TItem>(collection));

                foreach (var item in DistinctInstances)
                {
                    if (!initialContent.Contains(item))
                    {
                        handler.AddSingleItemOk(item);
                    }
                }

                const int extension = 5;
                int count = collection.Count;
                var target = new TItem[count + extension];
                AssertCopyToThrowException(() => collection.CopyTo(null, 0), "when called with a null argument.", "Argument Name", "array");
                AssertCopyToThrowException(() => collection.CopyTo(target, -1), "when called with a negative index.", "Argument Name", "arrayIndex");

                for (int i = 0; i <= extension; i++)
                {
                    AssertCopyToNotThrowException(() => collection.CopyTo(target, i), "Expected the method to not throw an exception.");
                    var clone = new TItem[count];
                    Array.Copy(target, i, clone, 0, count);
                    Assert.AreElementsEqual(collection, clone);
                }

                AssertCopyToThrowException(() => collection.CopyTo(target, extension + 1), "when called with a too large array index.", "Argument Name", "arrayIndex");
            });
        }
    }
}
