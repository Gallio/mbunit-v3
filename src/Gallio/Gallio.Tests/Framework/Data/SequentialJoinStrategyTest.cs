// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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

using System;
using System.Collections.Generic;
using Gallio.Collections;
using Gallio.Framework.Data;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(SequentialJoinStrategy))]
    public class SequentialJoinStrategyTest : BaseUnitTest
    {
        [Test]
        public void JoinsRowsSequentiallyAndPadsWithNullsUntilExhausted()
        {
            DataBinding[][] bindingsPerProvider = new DataBinding[][] {
                new DataBinding[] { new SimpleDataBinding(typeof(int), null, 0) },
                new DataBinding[] { },
                new DataBinding[] { new SimpleDataBinding(typeof(int), null, 0) },
            };

            IDataProvider[] providers = new IDataProvider[] {
                Mocks.CreateMock<IDataProvider>(),
                Mocks.CreateMock<IDataProvider>(),
                Mocks.CreateMock<IDataProvider>()
            };

            IDataRow[][] rowsPerProvider = new IDataRow[][] {
                new IDataRow[] {
                    new ScalarDataRow<int>(1, null),
                    new ScalarDataRow<int>(2, null)
                },
                new IDataRow[] { },
                new IDataRow[] {
                    new ScalarDataRow<int>(1, null),
                    new ScalarDataRow<int>(2, null),
                    new ScalarDataRow<int>(3, null)
                }
            };

            using (Mocks.Record())
            {
                Expect.Call(providers[0].GetRows(bindingsPerProvider[0])).Return(rowsPerProvider[0]);
                Expect.Call(providers[1].GetRows(bindingsPerProvider[1])).Return(rowsPerProvider[1]);
                Expect.Call(providers[2].GetRows(bindingsPerProvider[2])).Return(rowsPerProvider[2]);
            }

            using (Mocks.Playback())
            {
                List<IList<IDataRow>> rows = new List<IList<IDataRow>>(SequentialJoinStrategy.Instance.Join(providers, bindingsPerProvider));
                Assert.AreEqual(3, rows.Count);

                Assert.AreSame(rowsPerProvider[0][0], rows[0][0]);
                Assert.AreSame(NullDataRow.Instance, rows[0][1]);
                Assert.AreSame(rowsPerProvider[2][0], rows[0][2]);

                Assert.AreSame(rowsPerProvider[0][1], rows[1][0]);
                Assert.AreSame(NullDataRow.Instance, rows[1][1]);
                Assert.AreSame(rowsPerProvider[2][1], rows[1][2]);

                Assert.AreSame(NullDataRow.Instance, rows[2][0]);
                Assert.AreSame(NullDataRow.Instance, rows[2][1]);
                Assert.AreSame(rowsPerProvider[2][2], rows[2][2]);
            }
        }
    }
}