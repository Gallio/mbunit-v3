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
using System.Collections.Generic;
using System.Text;
using Gallio.Framework.Assertions;

namespace MbUnit.Framework.ContractVerifiers.Core
{
    // Helper class to ease test operations on the tested collection.
    internal class CollectionHandler<TCollection, TItem>
        where TCollection : ICollection<TItem>
    {
        private readonly TCollection collection;
        private readonly ContractVerificationContext context;

        protected ContractVerificationContext Context
        {
            get
            {
                return context;
            }
        }

        protected int CountTrack
        {
            get;
            set;
        }

        protected TCollection Collection
        {
            get
            {
                return collection;
            }
        }

        internal CollectionHandler(TCollection collection, ContractVerificationContext context)
        {
            this.collection = collection;
            this.CountTrack = collection.Count;
            this.context = context;
        }

        protected void AssertAddItemOk(TItem item, string failureMessage)
        {
            AssertionHelper.Explain(() =>
                Assert.DoesNotThrow(() =>
                {
                    collection.Add(item);
                    CountTrack++;
                }),
                innerFailures => new AssertionFailureBuilder(
                    failureMessage + "\nAn exception was thrown while none was expected.")
                    .SetStackTrace(context.GetStackTraceData())
                    .AddInnerFailures(innerFailures)
                    .ToAssertionFailure());
        }

        protected void AssertCount(string failureMessage)
        {
            int actual = collection.Count;

            AssertionHelper.Explain(() =>
                Assert.AreEqual(CountTrack, actual),
                innerFailures => new AssertionFailureBuilder(failureMessage)
                    .AddLabeledValue("Property", "Count")
                    .AddRawLabeledValue("Expected Value", CountTrack)
                    .AddRawLabeledValue("Actual Value", actual)
                    .SetStackTrace(context.GetStackTraceData())
                    .AddInnerFailures(innerFailures)
                    .ToAssertionFailure());
        }

        protected void AssertContained(TItem item, string failureMessage)
        {
            AssertionHelper.Explain(() =>
                Assert.Contains(collection, item),
                innerFailures => new AssertionFailureBuilder(failureMessage)
                    .AddRawLabeledValue("Just Added Item", item)
                    .AddLabeledValue("Method Called", "Contains")
                    .AddRawLabeledValue("Expected Returned Value", true)
                    .AddRawLabeledValue("Actual Returned Value", false)
                    .SetStackTrace(context.GetStackTraceData())
                    .AddInnerFailures(innerFailures)
                    .ToAssertionFailure());
        }

        protected void AssertAddItemFails(TItem item, string failureMessage)
        {
            AssertionHelper.Explain(() =>
                Assert.Throws<Exception>(() => // Any type of exception will make it!
                {
                    collection.Add(item);
                    CountTrack++;
                }),
                innerFailures => new AssertionFailureBuilder(
                    failureMessage + "\nNo exception was thrown while one was expected.")
                    .AddLabeledValue("Expected Exception Type", "Any")
                    .SetStackTrace(context.GetStackTraceData())
                    .AddInnerFailures(innerFailures)
                    .ToAssertionFailure());
        }

        protected void AssertRemoveItem(TItem item, bool expectedResult, string failureMessage)
        {
            bool actualResult = collection.Remove(item);

            if (actualResult)
            {
                CountTrack--;
            }

            AssertionHelper.Explain(() =>
                Assert.AreEqual(expectedResult, actualResult),
                innerFailures => new AssertionFailureBuilder(failureMessage)
                    .AddRawLabeledValue("Item", item)
                    .AddLabeledValue("Method", "Remove")
                    .AddRawLabeledValue("Expected Returned Value", expectedResult)
                    .AddRawLabeledValue("Actual Returned Value", actualResult)
                    .SetStackTrace(context.GetStackTraceData())
                    .AddInnerFailures(innerFailures)
                    .ToAssertionFailure());
        }

        public void AddSingleItemOk(TItem item)
        {
            AssertAddItemOk(item, "Expected the collection to accept successfully a new item.");
            AssertCount("Expected the collection to have a certain number of items after a new element was added.");
            AssertContained(item, "Expected the collection to contain a just added item.");
        }

        public void AddDuplicateItemOk(TItem item)
        {
            AssertAddItemOk(item, "Expected the collection to accept successfully a duplicate item.");
            AssertCount("Expected the collection to have a certain number of items after a duplicate element was added.");
        }

        public void AddDuplicateItemFails(TItem item)
        {
            AssertAddItemFails(item, "Expected the collection to refuse several identical items.");
        }

        public void RemoveMissingItemFails(TItem item)
        {
            AssertRemoveItem(item, false, "Expected the collection to not remove a missing item.");
        }

        public void RemoveItemOk(TItem item)
        {
            AssertRemoveItem(item, true, "Expected the collection to remove successfully an existing item.");
            AssertCount("Expected the collection to have a certain number of items after an element was removed.");
        }
    }
}
