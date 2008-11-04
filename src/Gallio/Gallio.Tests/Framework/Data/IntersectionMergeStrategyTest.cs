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
using Gallio.Collections;
using Gallio.Framework.Data;
using Rhino.Mocks;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(IntersectionMergeStrategy))]
    public class IntersectionMergeStrategyTest : BaseTestWithMocks
    {
        [Test]
        public void HandlesDegenerateCaseWithZeroProviders()
        {
            DataBinding[] bindings = new DataBinding[] {
                new DataBinding(0, null)
            };
            IDataProvider[] providers = new IDataProvider[0];

            List<IDataItem> items = new List<IDataItem>(IntersectionMergeStrategy.Instance.Merge(providers, bindings, true));
            Assert.AreEqual(0, items.Count);
        }

        [Test]
        public void HandlesDegenerateCaseWithOneProvider()
        {
            DataBinding[] bindings = new DataBinding[] {
                new DataBinding(0, null)
            };
            IDataProvider[] providers = new IDataProvider[] {
                Mocks.StrictMock<IDataProvider>()
            };

            using (Mocks.Record())
            {
                Expect.Call(providers[0].GetItems(bindings, true)).Return(new IDataItem[] {
                    new ScalarDataItem<int>(1, null, true),
                    new ScalarDataItem<int>(2, null, false),
                    new ScalarDataItem<int>(3, null, true)
                });
            }

            using (Mocks.Playback())
            {
                List<IDataItem> items = new List<IDataItem>(IntersectionMergeStrategy.Instance.Merge(providers, bindings, true));
                Assert.AreEqual(3, items.Count);

                Assert.AreEqual(1, items[0].GetValue(bindings[0]));
                Assert.IsTrue(items[0].IsDynamic);

                Assert.AreEqual(2, items[1].GetValue(bindings[0]));
                Assert.IsFalse(items[1].IsDynamic);

                Assert.AreEqual(3, items[2].GetValue(bindings[0]));
                Assert.IsTrue(items[2].IsDynamic);
            }
        }

        [Test]
        public void KeepsOnlyIntersectionIncludingRightNumberOfDuplicatesAndExcludesBadItems()
        {
            DataBinding[] bindings = new DataBinding[] {
                new DataBinding(0, null)
            };
            IDataProvider[] providers = new IDataProvider[] {
                Mocks.StrictMock<IDataProvider>(),
                Mocks.StrictMock<IDataProvider>(),
                Mocks.StrictMock<IDataProvider>()
            };

            using (Mocks.Record())
            {
                IDataItem badItem = Mocks.StrictMock<IDataItem>();
                Expect.Call(badItem.GetValue(bindings[0])).Throw(new InvalidOperationException("Test exception"));

                Expect.Call(providers[0].GetItems(bindings, true)).Return(new IDataItem[] {
                    new ScalarDataItem<int>(1, null, false),
                    new ScalarDataItem<int>(2, null, true),
                    new ScalarDataItem<int>(1, null, false),
                    new ScalarDataItem<int>(3, null, false),
                    new ScalarDataItem<int>(6, null, false),
                });

                Expect.Call(providers[1].GetItems(bindings, true)).Return(new IDataItem[] {
                    new ScalarDataItem<int>(1, null, false),
                    new ScalarDataItem<int>(1, null, false),
                    badItem,
                    new ScalarDataItem<int>(2, null, true),
                    new ScalarDataItem<int>(6, null, false),
                    new ScalarDataItem<int>(1, null, false),
                    new ScalarDataItem<int>(4, null, false),
                });

                Expect.Call(providers[2].GetItems(bindings, true)).Return(new IDataItem[] {
                    new ScalarDataItem<int>(1, null, false),
                    new ScalarDataItem<int>(1, null, false),
                    new ScalarDataItem<int>(5, null, false),
                    new ScalarDataItem<int>(3, null, false),
                    new ScalarDataItem<int>(2, null, true)
                });
            }

            using (Mocks.Playback())
            {
                List<IDataItem> items = new List<IDataItem>(IntersectionMergeStrategy.Instance.Merge(providers, bindings, true));
                Assert.AreEqual(3, items.Count);

                Assert.AreEqual(1, items[0].GetValue(bindings[0]));
                Assert.IsFalse(items[0].IsDynamic);

                Assert.AreEqual(1, items[1].GetValue(bindings[0]));
                Assert.IsFalse(items[1].IsDynamic);

                Assert.AreEqual(2, items[2].GetValue(bindings[0]));
                Assert.IsTrue(items[2].IsDynamic);
            }
        }
    }
}