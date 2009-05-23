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
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Framework.Assertions;
using System.Collections.ObjectModel;
using MbUnit.Framework.ContractVerifiers.Core;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// Contract for verifying the implementation of the generic <see cref="IList{T}"/> interface.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Since the generic <see cref="IList{T}"/> interface is a descendant of the generic <see cref="ICollection{T}"/> interface,
    /// the contract verifier has the same tests as the <see cref="CollectionContract{TCollection,TItem}"/> contract verifier, 
    /// plus the following built-in verifications:
    /// <list type="bullet">
    /// <item>
    /// <term>InsertShouldThrowException</term>
    /// <description>The read-only collection throws an exception when the method <see cref="IList{T}.Insert"/> is called.
    /// <para>
    /// The test is not run when the contract property <see cref="CollectionContract{TCollection,TItem}.IsReadOnly"/>
    /// inherited from <see cref="CollectionContract{TCollection,TItem}"/>, is set to 'false'.
    /// </para>
    /// </description>
    /// </item>
    /// <item>
    /// <term>RemoveAtShouldThrowException</term>
    /// <description>The read-only collection throws an exception when the method <see cref="IList{T}.RemoveAt"/>
    /// is called.
    /// <para>
    /// The test is not run when the contract property <see cref="CollectionContract{TCollection,TItem}.IsReadOnly"/> 
    /// inherited from <see cref="CollectionContract{TCollection,TItem}"/>, is set to 'false'.
    /// </para>
    /// </description>
    /// </item>
    /// <item>
    /// <term>IndexerSetShouldThrowException</term>
    /// <description>The read-only collection throws an exception when the setter of the indexer <see cref="IList{T}.this"/>
    /// is called.
    /// <para>
    /// The test is not run when the contract property <see cref="CollectionContract{TCollection,TItem}.IsReadOnly"/> 
    /// inherited from <see cref="CollectionContract{TCollection,TItem}"/>, is set to 'false'.
    /// </para>
    /// </description>
    /// </item>
    /// <item>
    /// <term>InsertNullArgument</term>
    /// <description>The collection throwns a <see cref="ArgumentNullException"/> when the method <see cref="IList{T}.Insert"/>
    /// is called with a null reference item. 
    /// <para>
    /// The test is not run when the contract property <see cref="CollectionContract{TCollection,TItem}.AcceptNullReference"/> 
    /// inherited from <see cref="CollectionContract{TCollection,TItem}"/>, is set to 'true'.
    /// </para>
    /// </description>
    /// </item>
    /// <item>
    /// <term>IndexOfNullArgument</term>
    /// <description>The collection throwns a <see cref="ArgumentNullException"/> when the method <see cref="IList{T}.IndexOf"/>
    /// is called with a null reference item. 
    /// <para>
    /// The test is not run when the contract property <see cref="CollectionContract{TCollection,TItem}.AcceptNullReference"/> 
    /// inherited from <see cref="CollectionContract{TCollection,TItem}"/>, is set to 'true'.
    /// </para>
    /// </description>
    /// </item>
    /// <item>
    /// <term>IndexerSetNullArgument</term>
    /// <description>The collection throwns a <see cref="ArgumentNullException"/> when the setter of the indexer <see cref="IList{T}.this"/>
    /// is called with a null reference item. 
    /// <para>
    /// The test is not run when the contract property <see cref="CollectionContract{TCollection,TItem}.AcceptNullReference"/> 
    /// inherited from <see cref="CollectionContract{TCollection,TItem}"/>, is set to 'true'.
    /// </para>
    /// </description>
    /// </item>
    /// <item>
    /// <term>InsertItems</term>
    /// <description>
    /// The collection handles correctly with the insertion of new items. The method 
    /// <see cref="ICollection{T}.Contains"/> and the property <see cref="ICollection{T}.Count"/> are expected
    /// to return suitable results as well. The case of duplicate items (object equality) is tested too; according
    /// to the value of contract property <see cref="CollectionContract{TCollection,TItem}.AcceptEqualItems"/>,
    /// inherited from <see cref="CollectionContract{TCollection,TItem}"/>.
    /// <para>
    /// The test is not run when the contract property <see cref="CollectionContract{TCollection,TItem}.IsReadOnly"/> 
    /// inherited from <see cref="CollectionContract{TCollection,TItem}"/>, is set to 'true'.
    /// </para>
    /// </description>
    /// </item>
    /// <item>
    /// <term>InsertItemsAtInvalidIndex</term>
    /// <description>
    /// The collection handles correctly with the insertion of new items at an invalid index. The method
    /// should throw an <see cref="ArgumentOutOfRangeException"/> when called with a negative index or with
    /// an index greater than the number of items in the list.
    /// <para>
    /// The test is not run when the contract property <see cref="CollectionContract{TCollection,TItem}.IsReadOnly"/> 
    /// inherited from <see cref="CollectionContract{TCollection,TItem}"/>, is set to 'true'.
    /// </para>
    /// </description>
    /// </item>
    /// <item>
    /// <term>RemoveItemsAt</term>
    /// The collection handles correctly with the removal of items at specific indexes. The method 
    /// <see cref="ICollection{T}.Contains"/> and the property <see cref="ICollection{T}.Count"/> are expected
    /// to return suitable results as well.
    /// <para>
    /// The test is not run when the contract property <see cref="CollectionContract{TCollection,TItem}.IsReadOnly"/> 
    /// inherited from <see cref="CollectionContract{TCollection,TItem}"/>, is set to 'true'.
    /// </para>
    /// </item>
    /// <item>
    /// <term>RemoveItemsAtInvalidIndex</term>
    /// <description>
    /// The collection handles correctly with the removal of items at an invalid index. The method
    /// should throw an <see cref="ArgumentOutOfRangeException"/> when called with a negative index or with
    /// an index greater than the number of items in the list.
    /// <para>
    /// The test is not run when the contract property <see cref="CollectionContract{TCollection,TItem}.IsReadOnly"/> 
    /// inherited from <see cref="CollectionContract{TCollection,TItem}"/>, is set to 'true'.
    /// </para>
    /// </description>
    /// </item>
    /// <item>
    /// <term>GetItemsAtInvalidIndex</term>
    /// <description>
    /// The collection handles correctly with the retrieval of items at an invalid index. The indexer
    /// should throw an <see cref="ArgumentOutOfRangeException"/> when called with a negative index or with
    /// an index greater than the number of items in the list.
    /// </description>
    /// </item>
    /// <item>
    /// <term>GetSetItemsWithIndexer</term>
    /// <description>
    /// Setting and getting items by using the indexer property works as expected.
    /// <para>
    /// The test is not run when the contract property <see cref="CollectionContract{TCollection,TItem}.IsReadOnly"/> 
    /// inherited from <see cref="CollectionContract{TCollection,TItem}"/>, is set to 'true'.
    /// </para>
    /// </description>
    /// </item>
    /// <item>
    /// <term>IndexOfItem</term>
    /// <description>
    /// The retrieval of the index of an item in the collection works as expected, and the index can be used
    /// effectively to get the item with the getter of the indexer property.
    /// </description>
    /// </item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <typeparam name="TList">The type of the collection implementing <see cref="IList{T}"/>.</typeparam>
    /// <typeparam name="TItem">The type of items contained by the list.</typeparam>
    public class ListContract<TList, TItem> : CollectionContract<TList, TItem>
        where TList : IList<TItem>
    {
        private IEnumerable<Test> GetBaseTests()
        {
            return base.GetContractVerificationTests();
        }

        /// <inheritdoc />
        protected override IEnumerable<Test> GetContractVerificationTests()
        {
            foreach (var test in GetBaseTests())
            {
                yield return test;
            }

            if (IsReadOnly)
            {
                yield return CreateNotSupportedWriteTest("Insert", (list, item) => list.Insert(0, item));
                yield return CreateNotSupportedWriteTest("RemoveAt", (list, item) => list.RemoveAt(0));
                yield return CreateNotSupportedWriteTest("IndexerSet", (list, item) => list[0] = item);
            }
            else
            {
                if (!typeof(TItem).IsValueType && !AcceptNullReference)
                {
                    yield return CreateNullArgumentTest("Insert", list => list.Insert(0, default(TItem)));
                    yield return CreateNullArgumentTest("IndexOf", list => list.IndexOf(default(TItem)));
                    yield return CreateNullArgumentTest("IndexerSet", list =>
                    {
                        AssertDistinctIntancesNotEmpty();
                        if (list.Count == 0) // If the list is empty, then add at least one item.
                            list.Add(DistinctInstances.Instances[0]);

                        list[0] = default(TItem);
                    });
                }

                yield return CreateInsertItemsTest();
                yield return CreateInsertItemsAtInvalidIndexTest();
                yield return CreateRemoveItemsAtTest();
                yield return CreateRemoveItemsAtInvalidIndexTest();
                yield return CreateIndexerGetSetItemsTest();
            }

            yield return CreateIndexOfItemsTest();
            yield return CreateIndexerGetItemsAtInvalidIndexTest();
        }

        private Test CreateInsertItemsTest()
        {
            return new TestCase("InsertItems", () =>
            {
                AssertDistinctIntancesNotEmpty();
                DoInsertCycle(list => 0); // Head insertion.
                DoInsertCycle(list => Math.Min(1, list.Count)); // Insertion at second position .
                DoInsertCycle(list => list.Count); // Queue insertion.
            });
        }

        private void DoInsertCycle(Func<TList, int> getIndex)
        {
            var list = GetSafeDefaultInstance();
            var handler = new ListHandler<TList, TItem>(list, Context);
            var initialContent = new ReadOnlyCollection<TItem>(list);

            foreach (var item in DistinctInstances)
            {
                if (!initialContent.Contains(item))
                {
                    handler.InsertSingleItemOk(getIndex(list), item);
                }

                if (AcceptEqualItems)
                {
                    handler.InsertDuplicateItemOk(getIndex(list), item);
                }
                else
                {
                    handler.InsertDuplicateItemFails(item);
                }
            }
        }

        private Test CreateInsertItemsAtInvalidIndexTest()
        {
            return new TestCase("InsertItemsAtInvalidIndex", () =>
            {
                AssertDistinctIntancesNotEmpty();
                var list = GetSafeDefaultInstance();
                var handler = new ListHandler<TList, TItem>(list, Context);
                var initialContent = new List<TItem>(list);

                Assert.Multiple(() =>
                {
                    foreach (var item in DistinctInstances)
                    {
                        if (!initialContent.Contains(item))
                        {
                            handler.DoActionAtInvalidIndex(x => -1, (l, i) => l.Insert(i, item), "Insert"); // Negative index.
                            handler.DoActionAtInvalidIndex(x => x.Count + 1, (l, i) => l.Insert(i, item), "Insert"); // Index too high.
                        }
                    }
                });
            });
        }

        private Test CreateIndexOfItemsTest()
        {
            return new TestCase("IndexOfItem", () =>
            {
                AssertDistinctIntancesNotEmpty();
                var handler = new ListHandler<TList, TItem>(GetSafeDefaultInstance(), Context);

                foreach (var item in DistinctInstances)
                {
                    handler.IndexOfItem(item, IsReadOnly);
                }
            });
        }

        private Test CreateRemoveItemsAtTest()
        {
            return new TestCase("RemoveItemsAt", () =>
            {
                AssertDistinctIntancesNotEmpty();
                var list = GetSafeDefaultInstance();
                var handler = new ListHandler<TList, TItem>(list, Context);

                foreach (var item in DistinctInstances)
                {
                    handler.RemoveItemAtOk(item);
                }
            });
        }

        private Test CreateRemoveItemsAtInvalidIndexTest()
        {
            return new TestCase("RemoveItemsAtInvalidIndex", () =>
            {
                AssertDistinctIntancesNotEmpty();
                var handler = new ListHandler<TList, TItem>(GetSafeDefaultInstance(), Context);

                Assert.Multiple(() =>
                {
                    handler.DoActionAtInvalidIndex(x => -1, (list, index) => list.RemoveAt(index), "RemoveAt"); // Negative index.
                    handler.DoActionAtInvalidIndex(x => x.Count + 1, (list, index) => list.RemoveAt(index), "RemoveAt"); // Index too high.
                });
            });
        }

        private Test CreateIndexerGetItemsAtInvalidIndexTest()
        {
            return new TestCase("GetItemsAtInvalidIndex", () =>
            {
                AssertDistinctIntancesNotEmpty();
                var handler = new ListHandler<TList, TItem>(GetSafeDefaultInstance(), Context);

                Assert.Multiple(() =>
                {
                    handler.DoActionAtInvalidIndex(x => -1, (list, index) => { var o = list[index]; }, "Indexer Getter"); // Negative index.
                    handler.DoActionAtInvalidIndex(x => x.Count + 1, (list, index) => { var o = list[index]; }, "Indexer Getter"); // Index too high.
                });
            });
        }

        private Test CreateIndexerGetSetItemsTest()
        {
            return new TestCase("GetSetItemsWithIndexer", () =>
            {
                AssertDistinctIntancesNotEmpty();
                var list = GetSafeDefaultInstance();

                if (list.Count == 0) // The default instance is empty: add at least one item.
                    list.Add(DistinctInstances.Instances[0]);

                for (int index = 0; index < list.Count; index++)
                {
                    foreach (var item in DistinctInstances.Instances)
                    {
                        if (!list.Contains(item) || AcceptEqualItems || list.IndexOf(item) == index)
                        {
                            list[index] = item;
                            var actual = list[index];

                            AssertionHelper.Explain(() =>
                                Assert.AreEqual(item, actual),
                                innerFailures => new AssertionFailureBuilder(
                                    "Expected the indexer to return a consistent result.")
                                    .AddRawLabeledValue("Index", index)
                                    .AddRawLabeledValue("Actual Item", actual)
                                    .AddRawLabeledValue("Expected Item", item)
                                    .SetStackTrace(Context.GetStackTraceData())
                                    .AddInnerFailures(innerFailures)
                                    .ToAssertionFailure());
                        }
                    }
                }
            });
        }
    }
}
