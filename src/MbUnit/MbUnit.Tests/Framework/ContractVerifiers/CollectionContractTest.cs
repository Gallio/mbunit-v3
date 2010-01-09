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

namespace MbUnit.Tests.Framework.ContractVerifiers
{
    [RunSample(typeof(BasicSampleTest))]
    [RunSample(typeof(NotWriteableBasicTest))]
    [RunSample(typeof(ReadOnlySampleTest))]
    [RunSample(typeof(NotReadOnlySampleTest))]
    [RunSample(typeof(BasicSampleWithNoDefaultConstructorTest))]
    [RunSample(typeof(BasicNullableSampleTest))]
    [RunSample(typeof(BuggyAddSampleTest))]
    [RunSample(typeof(BuggyClearSampleTest))]
    [RunSample(typeof(NoDoubletSampleTest))]
    public class CollectionContractTest : AbstractContractTest
    {
        [Test]
        [Row(typeof(BasicSampleTest), "VerifyReadOnlyProperty", TestStatus.Passed)]
        [Row(typeof(BasicSampleTest), "AddShouldThrowException", TestStatus.Inconclusive)]
        [Row(typeof(BasicSampleTest), "RemoveShouldThrowException", TestStatus.Inconclusive)]
        [Row(typeof(BasicSampleTest), "ClearShouldThrowException", TestStatus.Inconclusive)]
        [Row(typeof(BasicSampleTest), "AddNullArgument", TestStatus.Inconclusive)]
        [Row(typeof(BasicSampleTest), "RemoveNullArgument", TestStatus.Inconclusive)]
        [Row(typeof(BasicSampleTest), "ContainsNullArgument", TestStatus.Inconclusive)]
        [Row(typeof(BasicSampleTest), "AddItems", TestStatus.Passed)]
        [Row(typeof(BasicSampleTest), "ClearItems", TestStatus.Passed)]
        [Row(typeof(BasicSampleTest), "RemoveItems", TestStatus.Passed)]
        [Row(typeof(BasicSampleTest), "CopyTo", TestStatus.Passed)]
        [Row(typeof(NotWriteableBasicTest), "VerifyReadOnlyProperty", TestStatus.Failed)]
        [Row(typeof(NotWriteableBasicTest), "AddShouldThrowException", TestStatus.Inconclusive)]
        [Row(typeof(NotWriteableBasicTest), "RemoveShouldThrowException", TestStatus.Inconclusive)]
        [Row(typeof(NotWriteableBasicTest), "ClearShouldThrowException", TestStatus.Inconclusive)]
        [Row(typeof(NotWriteableBasicTest), "AddNullArgument", TestStatus.Inconclusive)]
        [Row(typeof(NotWriteableBasicTest), "RemoveNullArgument", TestStatus.Inconclusive)]
        [Row(typeof(NotWriteableBasicTest), "ContainsNullArgument", TestStatus.Inconclusive)]
        [Row(typeof(NotWriteableBasicTest), "AddItems", TestStatus.Failed)]
        [Row(typeof(NotWriteableBasicTest), "ClearItems", TestStatus.Failed)]
        [Row(typeof(NotWriteableBasicTest), "RemoveItems", TestStatus.Failed)]
        [Row(typeof(ReadOnlySampleTest), "VerifyReadOnlyProperty", TestStatus.Passed)]
        [Row(typeof(ReadOnlySampleTest), "AddShouldThrowException", TestStatus.Passed)]
        [Row(typeof(ReadOnlySampleTest), "RemoveShouldThrowException", TestStatus.Passed)]
        [Row(typeof(ReadOnlySampleTest), "ClearShouldThrowException", TestStatus.Passed)]
        [Row(typeof(ReadOnlySampleTest), "AddNullArgument", TestStatus.Inconclusive)]
        [Row(typeof(ReadOnlySampleTest), "RemoveNullArgument", TestStatus.Inconclusive)]
        [Row(typeof(ReadOnlySampleTest), "ContainsNullArgument", TestStatus.Inconclusive)]
        [Row(typeof(ReadOnlySampleTest), "AddItems", TestStatus.Inconclusive)]
        [Row(typeof(ReadOnlySampleTest), "ClearItems", TestStatus.Inconclusive)]
        [Row(typeof(ReadOnlySampleTest), "RemoveItems", TestStatus.Inconclusive)]
        [Row(typeof(ReadOnlySampleTest), "CopyTo", TestStatus.Passed)]
        [Row(typeof(NotReadOnlySampleTest), "VerifyReadOnlyProperty", TestStatus.Failed)]
        [Row(typeof(NotReadOnlySampleTest), "AddShouldThrowException", TestStatus.Failed)]
        [Row(typeof(NotReadOnlySampleTest), "RemoveShouldThrowException", TestStatus.Failed)]
        [Row(typeof(NotReadOnlySampleTest), "ClearShouldThrowException", TestStatus.Failed)]
        [Row(typeof(NotReadOnlySampleTest), "AddNullArgument", TestStatus.Inconclusive)]
        [Row(typeof(NotReadOnlySampleTest), "RemoveNullArgument", TestStatus.Inconclusive)]
        [Row(typeof(NotReadOnlySampleTest), "ContainsNullArgument", TestStatus.Inconclusive)]
        [Row(typeof(NotReadOnlySampleTest), "AddItems", TestStatus.Inconclusive)]
        [Row(typeof(NotReadOnlySampleTest), "ClearItems", TestStatus.Inconclusive)]
        [Row(typeof(NotReadOnlySampleTest), "RemoveItems", TestStatus.Inconclusive)]
        [Row(typeof(BasicSampleWithNoDefaultConstructorTest), "VerifyReadOnlyProperty", TestStatus.Passed)]
        [Row(typeof(BasicSampleWithNoDefaultConstructorTest), "AddShouldThrowException", TestStatus.Inconclusive)]
        [Row(typeof(BasicSampleWithNoDefaultConstructorTest), "RemoveShouldThrowException", TestStatus.Inconclusive)]
        [Row(typeof(BasicSampleWithNoDefaultConstructorTest), "ClearShouldThrowException", TestStatus.Inconclusive)]
        [Row(typeof(BasicSampleWithNoDefaultConstructorTest), "AddNullArgument", TestStatus.Inconclusive)]
        [Row(typeof(BasicSampleWithNoDefaultConstructorTest), "RemoveNullArgument", TestStatus.Inconclusive)]
        [Row(typeof(BasicSampleWithNoDefaultConstructorTest), "ContainsNullArgument", TestStatus.Inconclusive)]
        [Row(typeof(BasicSampleWithNoDefaultConstructorTest), "AddItems", TestStatus.Passed)]
        [Row(typeof(BasicSampleWithNoDefaultConstructorTest), "ClearItems", TestStatus.Passed)]
        [Row(typeof(BasicSampleWithNoDefaultConstructorTest), "RemoveItems", TestStatus.Passed)]
        [Row(typeof(BasicNullableSampleTest), "AddNullArgument", TestStatus.Passed)]
        [Row(typeof(BasicNullableSampleTest), "RemoveNullArgument", TestStatus.Passed)]
        [Row(typeof(BasicNullableSampleTest), "ContainsNullArgument", TestStatus.Passed)]
        [Row(typeof(BuggyAddSampleTest), "AddItems", TestStatus.Failed)]
        [Row(typeof(BuggyClearSampleTest), "AddItems", TestStatus.Passed)]
        [Row(typeof(BuggyClearSampleTest), "ClearItems", TestStatus.Failed)]
        [Row(typeof(NoDoubletSampleTest), "AddItems", TestStatus.Passed)]
        [Row(typeof(NoDoubletSampleTest), "RemoveItems", TestStatus.Passed)]
        public void VerifySampleCollectionContract(Type fixtureType, string testMethodName, TestStatus expectedTestStatus)
        {
            VerifySampleContract("CollectionTests", fixtureType, testMethodName, expectedTestStatus);
        }

        #region Sample contracts

        [Explicit]
        internal class BasicSampleTest
        {
            [VerifyContract]
            public readonly IContract CollectionTests = new CollectionContract<BasicSample<int>, int>
            {
                IsReadOnly = false,
                DistinctInstances = { 1, 2, 3 }
            };
        }

        [Explicit]
        internal class NotWriteableBasicTest
        {
            [VerifyContract]
            public readonly IContract CollectionTests = new CollectionContract<ReadOnlySample<int>, int>
            {
                IsReadOnly = false,
                DistinctInstances = { 1, 2, 3 }
            };
        }

        [Explicit]
        internal class ReadOnlySampleTest
        {
            [VerifyContract]
            public readonly IContract CollectionTests = new CollectionContract<ReadOnlySample<int>, int>
            {
                IsReadOnly = true,
                DistinctInstances = { 1, 2, 3 }
            };
        }

        [Explicit]
        internal class NotReadOnlySampleTest
        {
            [VerifyContract]
            public readonly IContract CollectionTests = new CollectionContract<BasicSample<int>, int>
            {
                IsReadOnly = true,
                DistinctInstances = { 1, 2, 3 }
            };
        }

        [Explicit]
        internal class BasicSampleWithNoDefaultConstructorTest
        {
            [VerifyContract]
            public readonly IContract CollectionTests = new CollectionContract<BasicSampleWithNoDefaultConstructor<int>, int>
            {
                DefaultInstance = () => new BasicSampleWithNoDefaultConstructor<int>(new[] { 1, 2, 3 }),
                DistinctInstances = { 4, 5, 6 },
            };
        }

        [Explicit]
        internal class BasicNullableSampleTest
        {
            [VerifyContract]
            public readonly IContract CollectionTests = new CollectionContract<BasicSample<Foo>, Foo>
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

        [Explicit]
        internal class BuggyAddSampleTest
        {
            [VerifyContract]
            public readonly IContract CollectionTests = new CollectionContract<BuggyAddSample<int>, int>
            {
                IsReadOnly = false,
                DistinctInstances = { 1, 2, 3 }
            };
        }

        [Explicit]
        internal class BuggyClearSampleTest
        {
            [VerifyContract]
            public readonly IContract CollectionTests = new CollectionContract<BuggyClearSample<Foo>, Foo>
            {
                IsReadOnly = false,
                DistinctInstances = { new Foo(1), new Foo(2), new Foo(3) }
            };
        }

        [Explicit]
        internal class NoDoubletSampleTest
        {
            [VerifyContract]
            public readonly IContract CollectionTests = new CollectionContract<NoDoubletSample<Foo>, Foo>
            {
                IsReadOnly = false,
                AcceptEqualItems = false,
                DistinctInstances = { new Foo(1), new Foo(2), new Foo(3) }
            };
        }

        #endregion

        #region Sample collections

        // Minimal and operational implementation of a collection.
        internal class BasicSample<T> : ICollection<T>
        {
            private List<T> items = new List<T>();

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
        }

        // Minimal and operational implementation of a read-only collection.
        // Add, Remove, or Clear throws a NotSupportedException.
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

            public override  bool IsReadOnly
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
        }

        // Same as "BasicSample" but with hiding the default constructor.
        // Most of tests will fail if the user does not provide manually a default instance 
        // to the contract verifyer by using the property "DefaultInstance".
        internal class BasicSampleWithNoDefaultConstructor<T> : BasicSample<T>
        {
            public BasicSampleWithNoDefaultConstructor(IEnumerable<T> items)
            {
                foreach (T item in items)
                {
                    Add(item);
                }
            }
        }

        // Same as "BasicSample", but with a buggy implementation of "Add".
        internal class BuggyAddSample<T> : BasicSample<T>
        {
            public override void Add(T item)
            {
                // Oops! Missing implementation here!
            }
        }

        // Same as "BasicSample", but with a buggy implementation of "Clear".
        internal class BuggyClearSample<T> : BasicSample<T>
        {
            public override void Clear()
            {
                // Empty the trash yourself!
            }
        }

        // Same as "BasicSample", which does not accept doublet items.
        internal class NoDoubletSample<T> : BasicSample<T>
        {
            public override void Add(T item)
            {
                CheckItemNotNull(item);

                if (Contains(item))
                {
                    throw new ArgumentException();
                }

                base.Add(item);
            }
        }

        #endregion
    }
}
