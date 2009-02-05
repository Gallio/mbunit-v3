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
using System.Collections.Generic;
using System.Reflection;
using Gallio.Collections;
using Gallio.Framework.Assertions;
using Gallio;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// Contract for verifying the implementation of the generic <see cref="ICollection{T}"/>.
    /// </summary>
    /// <typeparam name="TCollection"></typeparam>
    /// <typeparam name="TItem"></typeparam>
    public class CollectionContract<TCollection, TItem> : AbstractContract
        where TCollection : ICollection<TItem>
    {
        /// <summary>
        /// Provides a default instance of the collection which may contain or not items already.
        /// By default, the contract verifier attempts to invoke the default constructor
        /// of the collection. Override the default provider if the tested collection
        /// does not have a default constructor.
        /// </summary>
        public Func<TCollection> GetDefaultInstance
        {
            get;
            set;
        }

        /// <summary>
        /// Determines whether the tested collection is expected to be read-only.
        /// The default value is false.
        /// </summary>
        public bool IsReadOnly
        {
            get;
            set;
        }

        /// <summary>
        /// Determines whether the collection accepts null references as valid items.
        /// The default value is false.
        /// </summary>
        public bool AcceptNullReference
        {
            get;
            set;
        }

        /// <summary>
        /// Determines whether the collection accepts several identical items (by object equality).
        /// The default value is true.
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
        /// Some items which are not in the default collection instance yet.
        /// </item>
        /// <item>
        /// Some items which are in the default collection instance already (if not empty).
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
        public override IEnumerable<Test> GetContractVerificationTests()
        {
            yield return CreateVerifyReadOnlyPropertyTest();

            if (IsReadOnly)
            {
                yield return CreateMethodShouldThrowNotSupportedExceptionTest("Add", (collection, item) => collection.Add(item));
                yield return CreateMethodShouldThrowNotSupportedExceptionTest("Remove", (collection, item) => collection.Remove(item));
                yield return CreateMethodShouldThrowNotSupportedExceptionTest("Clear", (collection, item) => collection.Clear());
            }
            else
            {
                if (!typeof(TItem).IsValueType && !AcceptNullReference)
                {
                    yield return CreateMethodWithNullArgumentShouldThrowExceptionTest("Add", collection => collection.Add(default(TItem)));
                    yield return CreateMethodWithNullArgumentShouldThrowExceptionTest("Remove", collection => collection.Remove(default(TItem)));
                    yield return CreateMethodWithNullArgumentShouldThrowExceptionTest("Contains", collection => collection.Contains(default(TItem)));
                }

                yield return CreateAddItemsTest();
                yield return CreateClearTest();
                yield return CreateAddEqualItemsTest();
                yield return CreateRemoveItemsTest();
            }

            // TODO: Missing test(s) for ICollection<T>.CopyTo.
        }

        private void AssertDistinctIntancesNotEmpty()
        {
            AssertionHelper.Verify(() =>
            {
                if (DistinctInstances.Instances.Count > 0)
                    return null;

                return new AssertionFailureBuilder("Expected the collection of distinct instances to be not empty.\n" +
                    "Please feed the 'DistinctInstances' property of your collection contract with some valid objects.")
                    .ToAssertionFailure();
            });
        }

        private Test CreateVerifyReadOnlyPropertyTest()
        {
            return new TestCase("VerifyReadOnlyProperty", () =>
            { 
                AssertionHelper.Verify(() =>
                {
                    var collection = GetDefaultInstance();

                    if (IsReadOnly == collection.IsReadOnly)
                        return null;

                    return new AssertionFailureBuilder("Expected the collection to be marked as read-only.")
                        .AddLabeledValue("Property", "IsReadOnly")
                        .AddRawLabeledValue("Expected Value", IsReadOnly)
                        .AddRawLabeledValue("Actual Value", collection.IsReadOnly)
                        .ToAssertionFailure();
                });
            });
        }

        private Test CreateMethodShouldThrowNotSupportedExceptionTest(string methodName, Action<TCollection, TItem> invoke)
        {
            return new TestCase(String.Format("{0}ShouldThrowNotSupportedException", methodName), () =>
            {
                AssertDistinctIntancesNotEmpty();
                var collection = GetDefaultInstance();
                var item = DistinctInstances.Instances[0];
                Assert.Throws<NotSupportedException>(() => invoke(collection, item),
                    "The readonly collection should thrown an exception when the method '{0}' is called.", methodName);
            });
        }

        private Test CreateMethodWithNullArgumentShouldThrowExceptionTest(string methodName, Action<TCollection> invoke)
        {
            return new TestCase(String.Format("{0}NullArgumentShouldThrowArgumentNullException", methodName), () =>
            {
                var collection = GetDefaultInstance();
                Assert.Throws<ArgumentNullException>(() => invoke(collection),
                    "The collection should thrown an exception when the method '{0}' is called with a null reference argument.", methodName);
            });
        }

        private Test CreateClearTest()
        {
            return new TestCase("Clear", () =>
            {
                var collection = GetDefaultInstance();

                if (collection.Count == 0)
                {
                    // The default instance is empty: add some items.
                    AssertDistinctIntancesNotEmpty();

                    foreach (var item in DistinctInstances)
                    {
                        collection.Add(item);
                    }
                }

                collection.Clear();

                AssertionHelper.Verify(() =>
                {
                    if (collection.Count == 0)
                        return null;

                    return new AssertionFailureBuilder("Expected the collection to be empty once the 'Clear' method was called.")
                        .AddLabeledValue("Property", "Count")
                        .AddRawLabeledValue("Expected Value", 0)
                        .AddRawLabeledValue("Actual Value", collection.Count)
                        .ToAssertionFailure();
                });
            });
        }

        private Test CreateAddItemsTest()
        {
            return new TestCase("AddItems", () =>
            {
                AssertDistinctIntancesNotEmpty();
                var collection = GetDefaultInstance();
                int count = collection.Count;
                bool addedOnce = false;

                foreach (var item in DistinctInstances)
                {
                    if (!collection.Contains(item))
                    {
                        addedOnce = true;
                        collection.Add(item);

                        AssertionHelper.Verify(() =>
                        {
                            if ((++count) == collection.Count)
                                return null;

                            return new AssertionFailureBuilder("Expected the collection to have a certain number of items after the 'Add' method was called.")
                                .AddLabeledValue("Property", "Count")
                                .AddRawLabeledValue("Expected Value", count)
                                .AddRawLabeledValue("Actual Value", collection.Count)
                                .ToAssertionFailure();
                        });

                        AssertionHelper.Verify(() =>
                        {
                            if (collection.Contains(item))
                                return null;

                            return new AssertionFailureBuilder("Expected the collection to contain a just added item.")
                                .AddRawLabeledValue("Added Item", item)
                                .ToAssertionFailure();
                        });
                    }
                }

                AssertionHelper.Verify(() =>
                {
                    if (addedOnce)
                        return null;

                    return new AssertionFailureBuilder("At least one of the specified distinct instances " +
                        "should not be in the provided default collection instance.")
                        .ToAssertionFailure();
                });
            });
        }

        private Test CreateAddEqualItemsTest()
        {
            return new TestCase("AddEqualItems", () =>
            {
                AssertDistinctIntancesNotEmpty();
                var collection = GetDefaultInstance();
                collection.Clear();
                var item = DistinctInstances.Instances[0];
                collection.Add(item);

                if (AcceptEqualItems)
                {
                    Assert.DoesNotThrow(() => collection.Add(item), "Expected the collection to accept several times the same item.");

                    AssertionHelper.Verify(() =>
                    {
                        if (collection.Count == 2)
                            return null;

                        return new AssertionFailureBuilder("Expected the collection to contain several times the same item.")
                            .AddLabeledValue("Property", "Count")
                            .AddRawLabeledValue("Expected Value", 2)
                            .AddRawLabeledValue("Actual Value", collection.Count)
                            .ToAssertionFailure();
                    });
                }
                else
                {
                    AssertionHelper.Verify(() =>
                    {
                        try
                        {
                            collection.Add(item);
                        }
                        catch (Exception) // Any kind of exception will make it.
                        {
                            return null;
                        }

                        return new AssertionFailureBuilder("Expected the collection to throw an exception when adding several times the same item.")
                            .ToAssertionFailure();
                    });
                }
            });
        }

        private Test CreateRemoveItemsTest()
        {
            return new TestCase("RemoveItems", () =>
            {
                AssertDistinctIntancesNotEmpty();
                var collection = GetDefaultInstance();
                int countTrack = collection.Count;
                
                foreach (var item in DistinctInstances)
                {
                    bool alreadyIn = collection.Contains(item);
                    bool addedAsDoublet = false;

                    if (!alreadyIn || AcceptEqualItems)
                    {
                        collection.Add(item);
                        countTrack++;
                        addedAsDoublet = alreadyIn && AcceptEqualItems;
                    }

                    bool removed = collection.Remove(item);
                    countTrack--;

                    AssertionHelper.Verify(() =>
                    {
                        if (removed)
                            return null;

                        return new AssertionFailureBuilder("Expected the method 'Remove' to return a success value while removing an existing item from the collection.")
                            .AddRawLabeledValue("Expected Value", true)
                            .AddRawLabeledValue("Actual Value", removed)
                            .ToAssertionFailure();
                    });

                    AssertionHelper.Verify(() =>
                    {
                        if (countTrack == collection.Count)
                            return null;

                        return new AssertionFailureBuilder("Expected the collection to have a certain number of items after the 'Remove' method was called successfully.")
                            .AddLabeledValue("Property", "Count")
                            .AddRawLabeledValue("Expected Value", countTrack)
                            .AddRawLabeledValue("Actual Value", collection.Count)
                            .ToAssertionFailure();
                    });

                    if (addedAsDoublet)
                    {
                        AssertionHelper.Verify(() =>
                        {
                            if (collection.Contains(item))
                                return null;

                            return new AssertionFailureBuilder("Expected the collection to contain still another similar item after removing a doublet.\n" +
                                "The 'Remove' method should only remove the first occurrence of the searched item.")
                                .AddRawLabeledValue("Item", item)
                                .ToAssertionFailure();
                        });
                    }
                    else
                    {
                        AssertionHelper.Verify(() =>
                        {
                            if (!collection.Contains(item))
                                return null;

                            return new AssertionFailureBuilder("Expected the collection to not contain a single item which was previously removed.")
                                .AddRawLabeledValue("Item", item)
                                .ToAssertionFailure();
                        });
                    }
                }
            });
        }
    }
}
