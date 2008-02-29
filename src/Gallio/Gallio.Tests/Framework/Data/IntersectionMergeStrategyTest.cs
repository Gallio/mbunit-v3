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
using Rhino.Mocks;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(IntersectionMergeStrategy))]
    public class IntersectionMergeStrategyTest : BaseUnitTest
    {
        [Test]
        public void HandlesDegenerateCaseWithZeroProviders()
        {
            DataBinding[] bindings = new DataBinding[] {
                new SimpleDataBinding(0, null)
            };
            IDataProvider[] providers = new IDataProvider[0];

            List<IDataRow> rows = new List<IDataRow>(IntersectionMergeStrategy.Instance.Merge(providers, bindings, true));
            Assert.AreEqual(0, rows.Count);
        }

        [Test]
        public void HandlesDegenerateCaseWithOneProvider()
        {
            DataBinding[] bindings = new DataBinding[] {
                new SimpleDataBinding(0, null)
            };
            IDataProvider[] providers = new IDataProvider[] {
                Mocks.CreateMock<IDataProvider>()
            };

            using (Mocks.Record())
            {
                Expect.Call(providers[0].GetRows(bindings, true)).Return(new IDataRow[] {
                    new ScalarDataRow<int>(1, null, true),
                    new ScalarDataRow<int>(2, null, false),
                    new ScalarDataRow<int>(3, null, true)
                });
            }

            using (Mocks.Playback())
            {
                List<IDataRow> rows = new List<IDataRow>(IntersectionMergeStrategy.Instance.Merge(providers, bindings, true));
                Assert.AreEqual(3, rows.Count);

                Assert.AreEqual(1, rows[0].GetValue(bindings[0]));
                Assert.IsTrue(rows[0].IsDynamic);

                Assert.AreEqual(2, rows[1].GetValue(bindings[0]));
                Assert.IsFalse(rows[1].IsDynamic);

                Assert.AreEqual(3, rows[2].GetValue(bindings[0]));
                Assert.IsTrue(rows[2].IsDynamic);
            }
        }

        [Test]
        public void KeepsOnlyIntersectionIncludingRightNumberOfDuplicatesAndExcludesBadRows()
        {
            DataBinding[] bindings = new DataBinding[] {
                new SimpleDataBinding(0, null)
            };
            IDataProvider[] providers = new IDataProvider[] {
                Mocks.CreateMock<IDataProvider>(),
                Mocks.CreateMock<IDataProvider>(),
                Mocks.CreateMock<IDataProvider>()
            };

            using (Mocks.Record())
            {
                IDataRow badRow = Mocks.CreateMock<IDataRow>();
                Expect.Call(badRow.GetValue(bindings[0])).Throw(new InvalidOperationException("Test exception"));

                Expect.Call(providers[0].GetRows(bindings, true)).Return(new IDataRow[] {
                    new ScalarDataRow<int>(1, null, false),
                    new ScalarDataRow<int>(2, null, true),
                    new ScalarDataRow<int>(1, null, false),
                    new ScalarDataRow<int>(3, null, false),
                    new ScalarDataRow<int>(6, null, false),
                });

                Expect.Call(providers[1].GetRows(bindings, true)).Return(new IDataRow[] {
                    new ScalarDataRow<int>(1, null, false),
                    new ScalarDataRow<int>(1, null, false),
                    badRow,
                    new ScalarDataRow<int>(2, null, true),
                    new ScalarDataRow<int>(6, null, false),
                    new ScalarDataRow<int>(1, null, false),
                    new ScalarDataRow<int>(4, null, false),
                });

                Expect.Call(providers[2].GetRows(bindings, true)).Return(new IDataRow[] {
                    new ScalarDataRow<int>(1, null, false),
                    new ScalarDataRow<int>(1, null, false),
                    new ScalarDataRow<int>(5, null, false),
                    new ScalarDataRow<int>(3, null, false),
                    new ScalarDataRow<int>(2, null, true)
                });
            }

            using (Mocks.Playback())
            {
                List<IDataRow> rows = new List<IDataRow>(IntersectionMergeStrategy.Instance.Merge(providers, bindings, true));
                Assert.AreEqual(3, rows.Count);

                Assert.AreEqual(1, rows[0].GetValue(bindings[0]));
                Assert.IsFalse(rows[0].IsDynamic);

                Assert.AreEqual(1, rows[1].GetValue(bindings[0]));
                Assert.IsFalse(rows[1].IsDynamic);

                Assert.AreEqual(2, rows[2].GetValue(bindings[0]));
                Assert.IsTrue(rows[2].IsDynamic);
            }
        }
    }
}