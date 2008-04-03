// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using Gallio.Framework;
using Gallio.Framework.Data;
using Rhino.Mocks;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(PairwiseJoinStrategy))]
    public class PairwiseJoinStrategyTest : BaseUnitTest
    {
        [Test]
        public void HandlesDegenerateCaseWithZeroProviders()
        {
            DataBinding[][] bindingsPerProvider = new DataBinding[0][];
            IDataProvider[] providers = new IDataProvider[0];

            List<IList<IDataRow>> rows = new List<IList<IDataRow>>(PairwiseJoinStrategy.Instance.Join(providers, bindingsPerProvider, true));
            Assert.AreEqual(0, rows.Count);
        }

        [Test]
        public void HandlesDegenerateCaseWithOneProvider()
        {
            DataBinding[][] bindingsPerProvider = new DataBinding[][] {
                new DataBinding[] { new SimpleDataBinding(0, null) },
            };

            IDataProvider[] providers = new IDataProvider[] {
                Mocks.CreateMock<IDataProvider>()
            };

            IDataRow[][] rowsPerProvider = new IDataRow[][] {
                new IDataRow[] {
                    new ScalarDataRow<int>(1, null, true),
                    new ScalarDataRow<int>(2, null, false)
                },
            };

            using (Mocks.Record())
            {
                SetupResult.For(providers[0].GetRows(bindingsPerProvider[0], true)).Return(rowsPerProvider[0]);
            }

            using (Mocks.Playback())
            {
                List<IList<IDataRow>> rows = new List<IList<IDataRow>>(PairwiseJoinStrategy.Instance.Join(providers, bindingsPerProvider, true));
                Assert.AreEqual(2, rows.Count);

                Assert.AreSame(rowsPerProvider[0][0], rows[0][0]);
                Assert.AreSame(rowsPerProvider[0][1], rows[1][0]);
            }
        }

        [Test]
        public void HandlesDegenerateCaseWithMoreThanOneProviderButOneIsEmpty()
        {
            DataBinding[][] bindingsPerProvider = new DataBinding[][] {
                new DataBinding[] { new SimpleDataBinding(0, null) },
                new DataBinding[] { new SimpleDataBinding(0, null) },
            };

            IDataProvider[] providers = new IDataProvider[] {
                Mocks.CreateMock<IDataProvider>(),
                Mocks.CreateMock<IDataProvider>()
            };

            IDataRow[][] rowsPerProvider = new IDataRow[][] {
                new IDataRow[] {
                    new ScalarDataRow<int>(1, null, true),
                    new ScalarDataRow<int>(2, null, false)
                },
                new IDataRow[0]
            };

            using (Mocks.Record())
            {
                SetupResult.For(providers[0].GetRows(bindingsPerProvider[0], true)).Return(rowsPerProvider[0]);
                SetupResult.For(providers[1].GetRows(bindingsPerProvider[1], true)).Return(rowsPerProvider[1]);
            }

            using (Mocks.Playback())
            {
                List<IList<IDataRow>> rows = new List<IList<IDataRow>>(PairwiseJoinStrategy.Instance.Join(providers, bindingsPerProvider, true));
                Assert.AreEqual(0, rows.Count);
            }
        }

        [Test]
        [Row(new int[] { 1, 1 }, Description="1^2.")]
        [Row(new int[] { 2, 2 }, Description="2^2.")]
        [Row(new int[] { 2, 2, 2 }, Description = "2^3.")]
        [Row(new int[] { 3, 3, 3 }, Description = "3^3.")]
        [Row(new int[] { 3, 3, 3, 3 }, Description = "3^4.")]
        [Row(new int[] { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 }, Description = "3^13.")]
        [Row(new int[] { 7, 3, 4, 5 }, Description = "7*3*4*5.")]
        public void JoinProducesCoveringsOfAllPairs(int[] dimensionCounts)
        {
            DataBinding binding = new SimpleDataBinding(0, null);
            IDataProvider[] providers = new IDataProvider[dimensionCounts.Length];
            DataBinding[][] bindingsPerProvider = new DataBinding[dimensionCounts.Length][];
            for (int i = 0; i < dimensionCounts.Length; i++)
            {
                providers[i] = Mocks.CreateMock<IDataProvider>();
                bindingsPerProvider[i] = new DataBinding[] { binding };

                IDataRow[] providerRows = new IDataRow[dimensionCounts[i]];
                for (int j = 0; j < dimensionCounts[i]; j++)
                    providerRows[j] = new ScalarDataRow<int>(j, null, false);

                Expect.Call(providers[i].GetRows(bindingsPerProvider[i], true)).Return(providerRows);
            }

            Mocks.ReplayAll();

            List<IList<IDataRow>> rowLists = new List<IList<IDataRow>>(PairwiseJoinStrategy.Instance.Join(providers, bindingsPerProvider, true));

            foreach (IList<IDataRow> rowList in rowLists)
            {
                Assert.AreEqual(dimensionCounts.Length, rowList.Count);

                for (int i = 0; i < rowList.Count; i++)
                {
                    if (i != 0)
                        Log.Write(",");

                    Log.Write(rowList[i].GetValue(binding));
                }

                Log.WriteLine();
            }
        }
    }
}