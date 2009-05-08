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
using System.Text;
using Gallio.Common;
using Gallio.Framework.Assertions;

namespace MbUnit.Framework.ContractVerifiers.Core
{
    /// <summary>
    /// Helper class to ease test operations on the tested list.
    /// </summary>
    /// <typeparam name="TList">The type of the list collection.</typeparam>
    /// <typeparam name="TItem">The type of the list items.</typeparam>
    internal class ListHandler<TList, TItem> : CollectionHandler<TList, TItem>
        where TList : IList<TItem>
    {
        public ListHandler(TList list, ContractVerificationContext context)
            : base(list, context)
        {
        }

        /// <summary>
        /// Gets the tested list.
        /// </summary>
        protected TList List
        {
            get
            {
                return (TList)Collection;
            }
        }

        private void AssertInsertItemOk(int index, TItem item, string failureMessage)
        {
            AssertionHelper.Explain(() =>
                Assert.DoesNotThrow(() =>
                {
                    List.Insert(index, item);
                    CountTrack++;
                }),
                innerFailures => new AssertionFailureBuilder(
                    failureMessage + "\nAn exception was thrown while none was expected.")
                    .SetStackTrace(Context.GetStackTraceData())
                    .AddInnerFailures(innerFailures)
                    .ToAssertionFailure());
        }

        private void AssertInsertItemFails(int index, TItem item, string failureMessage)
        {
            AssertionHelper.Explain(() =>
                Assert.Throws<Exception>(() =>
                {
                    List.Insert(index, item);
                    CountTrack++;
                }),
                innerFailures => new AssertionFailureBuilder(
                    failureMessage + "\nNo exception was thrown while one was expected.")
                    .AddLabeledValue("Expected Exception Type", "Any")
                    .SetStackTrace(Context.GetStackTraceData())
                    .AddInnerFailures(innerFailures)
                    .ToAssertionFailure());
        }

        private void AssertIndexOfMissingItem(TItem item)
        {
            int result = List.IndexOf(item);

            AssertionHelper.Explain(() =>
                Assert.LessThan(result, 0),
                innerFailures => new AssertionFailureBuilder(
                    "Expected the method to return a negative value indicating that a missing item is not present in the list.")
                    .AddLabeledValue("Method", "IndexOf")
                    .AddRawLabeledValue("Actual Result", result)
                    .SetStackTrace(Context.GetStackTraceData())
                    .AddInnerFailures(innerFailures)
                    .ToAssertionFailure());
        }

        private void AssertIndexOfPresentItem(TItem item)
        {
            int result = List.IndexOf(item);

            AssertionHelper.Explain(() =>
                Assert.GreaterThanOrEqualTo(result, 0),
                innerFailures => new AssertionFailureBuilder(
                    "Expected the method to return zero or a positive value indicating that an item was found in the list.")
                    .AddLabeledValue("Method", "IndexOf")
                    .AddRawLabeledValue("Actual Result", result)
                    .SetStackTrace(Context.GetStackTraceData())
                    .AddInnerFailures(innerFailures)
                    .ToAssertionFailure());
        }

        private void AssertIndexOfConsistentWithIndexer(TItem item)
        {
            int index = List.IndexOf(item);

            if (index >= 0) // Just ignore missing items.
            {
                TItem actual = List[index];
            
                AssertionHelper.Explain(() =>
                    Assert.AreEqual(item, actual),
                    innerFailures => new AssertionFailureBuilder("Inconsistent result returned by the indexer accessor and " +
                        "the 'IndexOf' method. The getter of indexer accessor called with a given index has returned an item " +
                        "which is not equal to the item used against the 'IndexOf' method to obtain the index.")
                        .AddRawLabeledValue("Original Item", item)
                        .AddRawLabeledValue("Related Index", index)
                        .AddRawLabeledValue("Found Item", actual)
                        .SetStackTrace(Context.GetStackTraceData())
                        .AddInnerFailures(innerFailures)
                        .ToAssertionFailure());
            }
        }

        public void InsertSingleItemOk(int index, TItem item)
        {
            AssertInsertItemOk(index, item, "Expected the list to accept successfully the insertion of a new item.");
            AssertCount("Expected the list to have a certain number of items after a new element was inserted.");
            AssertContained(item, "Expected the list to contain a just inserted item.");
        }

        public void DoActionAtInvalidIndex(Func<TList, int> getInvalidIndex, Action<TList, int> action, string actionName)
        {
            int invalidIndex = getInvalidIndex(List);

            AssertionHelper.Explain(() =>
                Assert.Throws<ArgumentOutOfRangeException>(() => action(List, invalidIndex)),
                innerFailures => new AssertionFailureBuilder(
                    "Expected a method of the list to throw an particular exception when called with a invalid argument.")
                    .AddLabeledValue("Method", actionName)
                    .AddLabeledValue("Invalid Argument Name", "index")
                    .AddRawLabeledValue("Invalid Argument Value", invalidIndex)
                    .AddRawLabeledValue("Expected Exception", typeof(ArgumentOutOfRangeException))
                    .SetStackTrace(Context.GetStackTraceData())
                    .AddInnerFailures(innerFailures)
                    .ToAssertionFailure());
        }

        public void InsertDuplicateItemOk(int index, TItem item)
        {
            AssertInsertItemOk(index, item, "Expected the list to accept successfully a duplicate item.");
            AssertCount("Expected the list to have a certain number of items after a duplicate element was added.");
        }

        public void InsertDuplicateItemFails(TItem item)
        {
            AssertAddItemFails(item, "Expected the list to refuse several identical items.");
        }

        public void IndexOfItem(TItem item, bool isReadonly)
        {
            if (!List.Contains(item))
            {
                AssertIndexOfMissingItem(item);

                if (!isReadonly)
                {
                    AssertAddItemOk(item, "Expected the collection to accept successfully a new item.");
                }
            }

            if (List.Contains(item))
            {
                AssertIndexOfPresentItem(item);
                AssertIndexOfConsistentWithIndexer(item);
            }
        }

        public void RemoveItemAtOk(TItem item)
        {
            if (!List.Contains(item))
            {
                AddSingleItemOk(item);
            }

            List.RemoveAt(List.IndexOf(item));
            CountTrack--;
            AssertCount("Expected the collection to have a certain number of items after an element was removed.");
        }
    }
}