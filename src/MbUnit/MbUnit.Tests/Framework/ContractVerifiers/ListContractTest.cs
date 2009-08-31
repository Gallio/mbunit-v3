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
using System.Linq;
using System.Runtime.Serialization;
using Gallio.Model;
using Gallio.Runner.Reports;
using Gallio.Tests;
using Gallio.Tests.Integration;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;

namespace MbUnit.Tests.Framework.ContractVerifiers
{
    [RunSample(typeof(BasicSampleTest))]
    [RunSample(typeof(NotWriteableBasicTest))]
    [RunSample(typeof(ReadOnlySampleTest))]
    [RunSample(typeof(BasicNullableSampleTest))]
    [RunSample(typeof(GenericFactoryTest<>))]
    public class ListContractTest : AbstractContractTest
    {
        [Test]
        [Row(typeof(BasicSampleTest), "InsertShouldThrowException", TestStatus.Inconclusive)]
        [Row(typeof(BasicSampleTest), "RemoveAtShouldThrowException", TestStatus.Inconclusive)]
        [Row(typeof(BasicSampleTest), "IndexerSetShouldThrowException", TestStatus.Inconclusive)]
        [Row(typeof(BasicSampleTest), "InsertNullArgument", TestStatus.Inconclusive)]
        [Row(typeof(BasicSampleTest), "IndexerSetNullArgument", TestStatus.Inconclusive)]
        [Row(typeof(BasicSampleTest), "InsertItems", TestStatus.Passed)]
        [Row(typeof(BasicSampleTest), "InsertItemsAtInvalidIndex", TestStatus.Passed)]
        [Row(typeof(BasicSampleTest), "IndexOfItem", TestStatus.Passed)]
        [Row(typeof(BasicSampleTest), "RemoveItemsAtInvalidIndex", TestStatus.Passed)]
        [Row(typeof(BasicSampleTest), "RemoveItemsAt", TestStatus.Passed)]
        [Row(typeof(BasicSampleTest), "GetItemsAtInvalidIndex", TestStatus.Passed)]
        [Row(typeof(BasicSampleTest), "GetSetItemsWithIndexer", TestStatus.Passed)]
        [Row(typeof(NotWriteableBasicTest), "InsertShouldThrowException", TestStatus.Inconclusive)]
        [Row(typeof(NotWriteableBasicTest), "RemoveAtShouldThrowException", TestStatus.Inconclusive)]
        [Row(typeof(NotWriteableBasicTest), "IndexerSetShouldThrowException", TestStatus.Inconclusive)]
        [Row(typeof(NotWriteableBasicTest), "InsertNullArgument", TestStatus.Inconclusive)]
        [Row(typeof(NotWriteableBasicTest), "IndexerSetNullArgument", TestStatus.Inconclusive)]
        [Row(typeof(NotWriteableBasicTest), "InsertItems", TestStatus.Failed)]
        [Row(typeof(NotWriteableBasicTest), "InsertItemsAtInvalidIndex", TestStatus.Failed)]
        [Row(typeof(NotWriteableBasicTest), "RemoveItemsAtInvalidIndex", TestStatus.Failed)]
        [Row(typeof(NotWriteableBasicTest), "RemoveItemsAt", TestStatus.Failed)]
        [Row(typeof(NotWriteableBasicTest), "GetSetItemsWithIndexer", TestStatus.Failed)]
        [Row(typeof(ReadOnlySampleTest), "InsertShouldThrowException", TestStatus.Passed)]
        [Row(typeof(ReadOnlySampleTest), "RemoveAtShouldThrowException", TestStatus.Passed)]
        [Row(typeof(ReadOnlySampleTest), "IndexerSetShouldThrowException", TestStatus.Passed)]
        [Row(typeof(ReadOnlySampleTest), "InsertNullArgument", TestStatus.Inconclusive)]
        [Row(typeof(ReadOnlySampleTest), "IndexerSetNullArgument", TestStatus.Inconclusive)]
        [Row(typeof(ReadOnlySampleTest), "InsertItems", TestStatus.Inconclusive)]
        [Row(typeof(ReadOnlySampleTest), "InsertItemsAtInvalidIndex", TestStatus.Inconclusive)]
        [Row(typeof(ReadOnlySampleTest), "RemoveItemsAtInvalidIndex", TestStatus.Inconclusive)]
        [Row(typeof(ReadOnlySampleTest), "RemoveItemsAt", TestStatus.Inconclusive)]
        [Row(typeof(ReadOnlySampleTest), "GetItemsAtInvalidIndex", TestStatus.Passed)]
        [Row(typeof(ReadOnlySampleTest), "GetSetItemsWithIndexer", TestStatus.Inconclusive)]
        [Row(typeof(ReadOnlySampleTest), "IndexOfItem", TestStatus.Passed)]
        [Row(typeof(BasicNullableSampleTest), "InsertNullArgument", TestStatus.Passed)]
        [Row(typeof(BasicNullableSampleTest), "IndexerSetNullArgument", TestStatus.Passed)]
        [Row(typeof(BasicNullableSampleTest), "InsertItems", TestStatus.Passed)]
        [Row(typeof(BasicNullableSampleTest), "InsertItemsAtInvalidIndex", TestStatus.Passed)]
        [Row(typeof(BasicNullableSampleTest), "IndexOfItem", TestStatus.Passed)]
        [Row(typeof(BasicNullableSampleTest), "RemoveItemsAtInvalidIndex", TestStatus.Passed)]
        [Row(typeof(BasicNullableSampleTest), "RemoveItemsAt", TestStatus.Passed)]
        [Row(typeof(BasicNullableSampleTest), "GetItemsAtInvalidIndex", TestStatus.Passed)]
        [Row(typeof(BasicNullableSampleTest), "GetSetItemsWithIndexer", TestStatus.Passed)]
        [Row(typeof(GenericFactoryTest<int>), "InsertShouldThrowException", TestStatus.Passed)]
        [Row(typeof(GenericFactoryTest<int>), "RemoveAtShouldThrowException", TestStatus.Passed)]
        [Row(typeof(GenericFactoryTest<int>), "IndexerSetShouldThrowException", TestStatus.Passed)]
        [Row(typeof(GenericFactoryTest<int>), "InsertNullArgument", TestStatus.Inconclusive)]
        [Row(typeof(GenericFactoryTest<int>), "IndexerSetNullArgument", TestStatus.Inconclusive)]
        [Row(typeof(GenericFactoryTest<int>), "InsertItems", TestStatus.Inconclusive)]
        [Row(typeof(GenericFactoryTest<int>), "InsertItemsAtInvalidIndex", TestStatus.Inconclusive)]
        [Row(typeof(GenericFactoryTest<int>), "RemoveItemsAtInvalidIndex", TestStatus.Inconclusive)]
        [Row(typeof(GenericFactoryTest<int>), "RemoveItemsAt", TestStatus.Inconclusive)]
        [Row(typeof(GenericFactoryTest<int>), "GetItemsAtInvalidIndex", TestStatus.Passed)]
        [Row(typeof(GenericFactoryTest<int>), "GetSetItemsWithIndexer", TestStatus.Inconclusive)]
        [Row(typeof(GenericFactoryTest<int>), "IndexOfItem", TestStatus.Passed)]
        public void VerifySampleListContract(Type fixtureType, string testMethodName, TestStatus expectedTestStatus)
        {
            VerifySampleContract("ListTests", fixtureType, testMethodName, expectedTestStatus);
        }

        #region Sample contracts

        [Explicit]
        internal class BasicSampleTest
        {
            [VerifyContract]
            public readonly IContract ListTests = new ListContract<BasicSample<int>, int>
            {
                IsReadOnly = false,
                DistinctInstances = { 1, 2, 3 }
            };
        }

        [Explicit]
        internal class NotWriteableBasicTest
        {
            [VerifyContract]
            public readonly IContract ListTests = new ListContract<ReadOnlySample<int>, int>
            {
                IsReadOnly = false,
                DistinctInstances = { 1, 2, 3 }
            };
        }

        [Explicit]
        internal class ReadOnlySampleTest
        {
            [VerifyContract]
            public readonly IContract ListTests = new ListContract<ReadOnlySample<int>, int>
            {
                IsReadOnly = true,
                DistinctInstances = { 1, 2, 3 }
            };
        }

        [Explicit]
        internal class BasicNullableSampleTest
        {
            [VerifyContract]
            public readonly IContract ListTests = new ListContract<BasicSample<Foo>, Foo>
            {
                IsReadOnly = false,
                DistinctInstances = { new Foo(1), new Foo(2), new Foo(3) },
                AcceptNullReference = false
            };
        }

        internal class Foo
        {
            private readonly int value;

            public Foo(int value)
            {
                this.value = value;
            }
        }

        internal static class TypeFactory
        {
            internal static IEnumerable<Type> GetTypes()
            {
                yield return typeof(int);
            }
        }

        [Explicit]
        internal class GenericFactoryTest<[Factory(typeof(TypeFactory), "GetTypes")] T>
        {
            [VerifyContract]
            public readonly IContract ListTests = new ListContract<ReadOnlyCollection<int>, int>
            {
                IsReadOnly = true,
                DefaultInstance = () => new ReadOnlyCollection<int>(new[] { 0, 5, 7, 2 }),
                DistinctInstances = new DistinctInstanceCollection<int>(new[] { 5, 10 }),
            };
        }

        #endregion

        #region Sample lists

        /// <summary>
        /// Minimal and operational implementation of a collection.
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        internal class BasicSample<T> : IList<T>
        {
            private readonly List<T> items = new List<T>();

            protected void CheckItemNotNull(T item)
            {
                if (item == null)
                {
                    throw new ArgumentNullException("item");
                }
            }

            public virtual void Add(T item)
            {
                CheckItemNotNull(item);
                items.Add(item);
            }

            public virtual void Clear()
            {
                items.Clear();
            }

            public virtual bool Contains(T item)
            {
                CheckItemNotNull(item);
                return items.Contains(item);
            }

            public virtual void CopyTo(T[] array, int arrayIndex)
            {
                items.CopyTo(array, arrayIndex);
            }

            public virtual int Count
            {
                get
                {
                    return items.Count;
                }
            }

            public virtual bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            public virtual bool Remove(T item)
            {
                CheckItemNotNull(item);
                return items.Remove(item);
            }

            public virtual IEnumerator<T> GetEnumerator()
            {
                return items.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                foreach (T item in items)
                {
                    yield return item;
                }
            }

            public virtual int IndexOf(T item)
            {
                CheckItemNotNull(item);
                return items.IndexOf(item);
            }

            public virtual void Insert(int index, T item)
            {
                CheckItemNotNull(item);
                items.Insert(index, item);
            }

            public virtual void RemoveAt(int index)
            {
                items.RemoveAt(index);
            }

            public virtual T this[int index]
            {
                get
                {
                    return items[index];
                }

                set
                {
                    CheckItemNotNull(value);
                    items[index] = value;
                }
            }
        }

        /// <summary>
        /// Minimal and operational implementation of a read-only collection.
        /// Add, Remove, or Clear throws a NotSupportedException.
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        internal class ReadOnlySample<T> : BasicSample<T>
        {
            public override void Add(T item)
            {
                throw new NotSupportedException();
            }

            public override void Clear()
            {
                throw new NotSupportedException();
            }

            public override bool IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            public override bool Remove(T item)
            {
                throw new NotSupportedException();
            }

            public override void Insert(int index, T item)
            {
                throw new NotSupportedException();
            }

            public override void RemoveAt(int index)
            {
                throw new NotSupportedException();
            }
        }

        #endregion
    }
}
