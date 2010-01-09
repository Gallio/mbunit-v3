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
using Gallio.Framework.Data;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(SequentialJoinStrategy))]
    public class SequentialJoinStrategyTest : BaseTestWithMocks
    {
        [Test]
        public void JoinsItemsSequentiallyAndPadsWithNullsUntilExhausted()
        {
            DataBinding[][] bindingsPerProvider = new DataBinding[][] {
                new DataBinding[] { new DataBinding(0, null) },
                new DataBinding[] { },
                new DataBinding[] { new DataBinding(0, null) },
            };

            IDataProvider[] providers = new IDataProvider[] {
                Mocks.StrictMock<IDataProvider>(),
                Mocks.StrictMock<IDataProvider>(),
                Mocks.StrictMock<IDataProvider>()
            };

            IDataItem[][] itemsPerProvider = new IDataItem[][] {
                new IDataItem[] {
                    new ScalarDataItem<int>(1, null, true),
                    new ScalarDataItem<int>(2, null, false)
                },
                new IDataItem[] { },
                new IDataItem[] {
                    new ScalarDataItem<int>(1, null, false),
                    new ScalarDataItem<int>(2, null, false),
                    new ScalarDataItem<int>(3, null, false)
                }
            };

            using (Mocks.Record())
            {
                Expect.Call(providers[0].GetItems(bindingsPerProvider[0], true)).Return(itemsPerProvider[0]);
                Expect.Call(providers[1].GetItems(bindingsPerProvider[1], true)).Return(itemsPerProvider[1]);
                Expect.Call(providers[2].GetItems(bindingsPerProvider[2], true)).Return(itemsPerProvider[2]);
            }

            using (Mocks.Playback())
            {
                List<IList<IDataItem>> items = new List<IList<IDataItem>>(SequentialJoinStrategy.Instance.Join(providers, bindingsPerProvider, true));
                Assert.AreEqual(3, items.Count);

                Assert.AreSame(itemsPerProvider[0][0], items[0][0]);
                Assert.AreSame(NullDataItem.Instance, items[0][1]);
                Assert.AreSame(itemsPerProvider[2][0], items[0][2]);

                Assert.AreSame(itemsPerProvider[0][1], items[1][0]);
                Assert.AreSame(NullDataItem.Instance, items[1][1]);
                Assert.AreSame(itemsPerProvider[2][1], items[1][2]);

                Assert.AreSame(NullDataItem.Instance, items[2][0]);
                Assert.AreSame(NullDataItem.Instance, items[2][1]);
                Assert.AreSame(itemsPerProvider[2][2], items[2][2]);
            }
        }
    }
}