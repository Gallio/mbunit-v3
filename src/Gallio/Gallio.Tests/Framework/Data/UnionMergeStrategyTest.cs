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
using Gallio.Common.Collections;
using Gallio.Framework.Data;
using Rhino.Mocks;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(UnionMergeStrategy))]
    public class UnionMergeStrategyTest : BaseTestWithMocks
    {
        [Test]
        public void EliminatesDuplicatesAndIncludesBadItems()
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
                Expect.Call(badItem.GetValue(bindings[0])).Repeat.Twice().Throw(new InvalidOperationException("Test exception"));

                Expect.Call(providers[0].GetItems(bindings, true)).Return(new IDataItem[] {
                    new ScalarDataItem<int>(1, null, true),
                    new ScalarDataItem<int>(1, null, true),
                    new ScalarDataItem<int>(2, null, false),
                });

                Expect.Call(providers[1].GetItems(bindings, true)).Return(EmptyArray<IDataItem>.Instance);

                Expect.Call(providers[2].GetItems(bindings, true)).Return(new IDataItem[] {
                    badItem,
                    new ScalarDataItem<int>(3, null, true),
                    new ScalarDataItem<int>(2, null, true)
                });
            }

            using (Mocks.Playback())
            {
                List<IDataItem> items = new List<IDataItem>(UnionMergeStrategy.Instance.Merge(providers, bindings, true));
                Assert.AreEqual(4, items.Count);

                Assert.AreEqual(1, items[0].GetValue(bindings[0]));
                Assert.IsTrue(items[0].IsDynamic);
                Assert.AreEqual(2, items[1].GetValue(bindings[0]));
                Assert.IsFalse(items[1].IsDynamic);
                Assert.Throws<InvalidOperationException>(delegate { items[2].GetValue(bindings[0]); });
                Assert.AreEqual(3, items[3].GetValue(bindings[0]));
                Assert.IsTrue(items[3].IsDynamic);
            }
        }
    }
}