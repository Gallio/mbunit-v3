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
using Gallio.Framework;
using Gallio.Framework.Data;
using Rhino.Mocks;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(PairwiseJoinStrategy))]
    public class PairwiseJoinStrategyTest : BaseTestWithMocks
    {
        [Test]
        public void HandlesDegenerateCaseWithZeroProviders()
        {
            DataBinding[][] bindingsPerProvider = new DataBinding[0][];
            IDataProvider[] providers = new IDataProvider[0];

            List<IList<IDataItem>> items = new List<IList<IDataItem>>(PairwiseJoinStrategy.Instance.Join(providers, bindingsPerProvider, true));
            Assert.AreEqual(0, items.Count);
        }

        [Test]
        public void HandlesDegenerateCaseWithOneProvider()
        {
            DataBinding[][] bindingsPerProvider = new DataBinding[][] {
                new DataBinding[] { new DataBinding(0, null) },
            };

            IDataProvider[] providers = new IDataProvider[] {
                Mocks.StrictMock<IDataProvider>()
            };

            IDataItem[][] itemsPerProvider = new IDataItem[][] {
                new IDataItem[] {
                    new ScalarDataItem<int>(1, null, true),
                    new ScalarDataItem<int>(2, null, false)
                },
            };

            using (Mocks.Record())
            {
                SetupResult.For(providers[0].GetItems(bindingsPerProvider[0], true)).Return(itemsPerProvider[0]);
            }

            using (Mocks.Playback())
            {
                List<IList<IDataItem>> items = new List<IList<IDataItem>>(PairwiseJoinStrategy.Instance.Join(providers, bindingsPerProvider, true));
                Assert.AreEqual(2, items.Count);

                Assert.AreSame(itemsPerProvider[0][0], items[0][0]);
                Assert.AreSame(itemsPerProvider[0][1], items[1][0]);
            }
        }

        [Test]
        public void HandlesDegenerateCaseWithMoreThanOneProviderButOneIsEmpty()
        {
            DataBinding[][] bindingsPerProvider = new DataBinding[][] {
                new DataBinding[] { new DataBinding(0, null) },
                new DataBinding[] { new DataBinding(0, null) },
            };

            IDataProvider[] providers = new IDataProvider[] {
                Mocks.StrictMock<IDataProvider>(),
                Mocks.StrictMock<IDataProvider>()
            };

            IDataItem[][] itemsPerProvider = new IDataItem[][] {
                new IDataItem[] {
                    new ScalarDataItem<int>(1, null, true),
                    new ScalarDataItem<int>(2, null, false)
                },
                new IDataItem[0]
            };

            using (Mocks.Record())
            {
                SetupResult.For(providers[0].GetItems(bindingsPerProvider[0], true)).Return(itemsPerProvider[0]);
                SetupResult.For(providers[1].GetItems(bindingsPerProvider[1], true)).Return(itemsPerProvider[1]);
            }

            using (Mocks.Playback())
            {
                List<IList<IDataItem>> items = new List<IList<IDataItem>>(PairwiseJoinStrategy.Instance.Join(providers, bindingsPerProvider, true));
                Assert.AreEqual(0, items.Count);
            }
        }

        [Test]
        [Row(new int[] { 1, 1 }, 1, Description="1^2.")]
        [Row(new int[] { 2, 2 }, 4, Description="2^2.")]
        [Row(new int[] { 2, 2, 2 }, 4, Description = "2^3.")]
        [Row(new int[] { 3, 3, 3 }, 10, Description = "3^3.")]
        [Row(new int[] { 3, 3, 3, 3 }, 10, Description = "3^4.")]
        [Row(new int[] { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 }, 21, Description = "3^13.")]
        [Row(new int[] { 7, 3, 4, 5 }, 36, Description = "7*3*4*5.")]
        public void JoinProducesCoveringsOfAllPairs(int[] counts, int empiricalUpperBound)
        {
            int dimensions = counts.Length;

            DataBinding binding = new DataBinding(0, null);
            IDataProvider[] providers = new IDataProvider[dimensions];
            DataBinding[][] bindingsPerProvider = new DataBinding[dimensions][];
            for (int i = 0; i < dimensions; i++)
            {
                providers[i] = Mocks.StrictMock<IDataProvider>();
                bindingsPerProvider[i] = new DataBinding[] { binding };

                IDataItem[] providerItems = new IDataItem[counts[i]];
                for (int j = 0; j < counts[i]; j++)
                    providerItems[j] = new ScalarDataItem<int>(j, null, false);

                Expect.Call(providers[i].GetItems(bindingsPerProvider[i], true)).Return(providerItems);
            }

            Mocks.ReplayAll();

            List<IList<IDataItem>> itemLists = new List<IList<IDataItem>>(PairwiseJoinStrategy.Instance.Join(providers, bindingsPerProvider, true));

            int[][] values = new int[itemLists.Count][];
            using (TestLog.BeginSection(String.Format("{0} combinations.", itemLists.Count)))
            {
                for (int i = 0; i < itemLists.Count; i++)
                {
                    IList<IDataItem> itemList = itemLists[i];
                    Assert.AreEqual(dimensions, itemList.Count);

                    values[i] = new int[dimensions];
                    for (int j = 0; j < itemList.Count; j++)
                    {
                        int value = (int)itemList[j].GetValue(binding);
                        values[i][j] = value;

                        if (j != 0)
                            TestLog.Write(",");
                        TestLog.Write(value);
                    }

                    TestLog.WriteLine();
                }
            }

            // Check pairings.
            bool missingPairing = false;
            double meanOccurrences = 0;
            double stdevOccurrences = 0;
            int pairingCount = 0;

            using (TestLog.BeginSection("Pairings"))
            {
                for (int firstDimension = 0; firstDimension < dimensions; firstDimension++)
                {
                    for (int secondDimension = firstDimension + 1; secondDimension < dimensions; secondDimension++)
                    {
                        for (int firstValue = 0; firstValue < counts[firstDimension]; firstValue++)
                        {
                            for (int secondValue = 0; secondValue < counts[secondDimension]; secondValue++)
                            {
                                int occurrences = 0;
                                for (int i = 0; i < values.Length; i++)
                                    if (values[i][firstDimension] == firstValue && values[i][secondDimension] == secondValue)
                                        occurrences += 1;

                                TestLog.WriteLine("{0} x {1} : ({2}, {3}) -> {4} occurrences.",
                                    firstDimension, secondDimension, firstValue, secondValue, occurrences);

                                if (occurrences == 0)
                                    missingPairing = true;

                                pairingCount += 1;
                                double diff = occurrences - meanOccurrences;
                                meanOccurrences += diff / pairingCount;
                                stdevOccurrences += diff * (occurrences - meanOccurrences);
                            }
                        }
                    }
                }
            }

            if (pairingCount > 1)
                stdevOccurrences = Math.Sqrt(stdevOccurrences / (pairingCount - 1));
            else
                stdevOccurrences = 0;

            using (TestLog.BeginSection("Statistics"))
            {
                // A mean of exactly 1 implies we have found a minimal covering.
                // A low standard deviation indicates good uniformity among the covered pairs.  0 would be ideal.
                TestLog.WriteLine("Pairing Occurrence Mean: {0}", meanOccurrences);
                TestLog.WriteLine("Pairing Occurrence Stdev: {0}", stdevOccurrences);
            }

            Assert.IsFalse(missingPairing, "One or more pairings were not covered!");
            Assert.LessThanOrEqualTo(values.Length, empiricalUpperBound, "There were more combinations produced than previously measured.  Has the algorithm gotten worse?");
        }
    }
}