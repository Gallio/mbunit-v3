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

using System.Collections.Generic;
using Gallio.Framework.Data;
using Rhino.Mocks;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(CombinatorialJoinStrategy))]
    public class CombinatorialJoinStrategyTest : BaseTestWithMocks
    {
        [Test]
        public void HandlesDegenerateCaseWithZeroProviders()
        {
            DataBinding[][] bindingsPerProvider = new DataBinding[0][];
            IDataProvider[] providers = new IDataProvider[0];

            List<IList<IDataItem>> items = new List<IList<IDataItem>>(CombinatorialJoinStrategy.Instance.Join(providers, bindingsPerProvider, true));
            Assert.AreEqual(0, items.Count);
        }

        [Test]
        public void JoinsItemsCombinatorially()
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
                    new ScalarDataItem<int>(1, null, false),
                    new ScalarDataItem<int>(2, null, true)
                },
                new IDataItem[] {
                    new ScalarDataItem<int>(1, null, false),
                    new ScalarDataItem<int>(2, null, false),
                    new ScalarDataItem<int>(3, null, false)
                },
                new IDataItem[] {
                    new ScalarDataItem<int>(1, null, false),
                    new ScalarDataItem<int>(2, null, true)
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
                List<IList<IDataItem>> items = new List<IList<IDataItem>>(CombinatorialJoinStrategy.Instance.Join(providers, bindingsPerProvider, true));
                Assert.AreEqual(12, items.Count);

                int index = 0;
                for (int i = 0; i < itemsPerProvider[0].Length; i++)
                {
                    for (int j = 0; j < itemsPerProvider[1].Length; j++)
                    {
                        for (int k = 0; k < itemsPerProvider[2].Length; k++)
                        {
                            Assert.AreSame(itemsPerProvider[0][i], items[index][0]);
                            Assert.AreSame(itemsPerProvider[1][j], items[index][1]);
                            Assert.AreSame(itemsPerProvider[2][k], items[index][2]);

                            index += 1;
                        }
                    }
                }
            }
        }
    }
}