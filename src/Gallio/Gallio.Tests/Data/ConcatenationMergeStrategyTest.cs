using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Collections;
using Gallio.Data;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Data
{
    [TestFixture]
    [TestsOn(typeof(ConcatenationMergeStrategy))]
    public class ConcatenationMergeStrategyTest : BaseUnitTest
    {
        [Test]
        public void CombinesRowsIntoASingleSequence()
        {
            DataBinding[] bindings = new DataBinding[] {
                new SimpleDataBinding(typeof(int), null, 0)
            };
            IDataProvider[] providers = new IDataProvider[] {
                Mocks.CreateMock<IDataProvider>(),
                Mocks.CreateMock<IDataProvider>(),
                Mocks.CreateMock<IDataProvider>()
            };

            using (Mocks.Record())
            {
                Expect.Call(providers[0].GetRows(bindings)).Return(new IDataRow[] {
                    new ScalarDataRow<int>(1, null),
                    new ScalarDataRow<int>(2, null),
                });

                Expect.Call(providers[1].GetRows(bindings)).Return(EmptyArray<IDataRow>.Instance);

                Expect.Call(providers[2].GetRows(bindings)).Return(new IDataRow[] {
                    new ScalarDataRow<int>(3, null),
                });
            }

            using (Mocks.Playback())
            {
                List<IDataRow> rows = new List<IDataRow>(ConcatenationMergeStrategy.Instance.Merge(providers, bindings));
                Assert.AreEqual(3, rows.Count);

                Assert.AreEqual(1, rows[0].GetValue(bindings[0]));
                Assert.AreEqual(2, rows[1].GetValue(bindings[0]));
                Assert.AreEqual(3, rows[2].GetValue(bindings[0]));
            }
        }
    }
}
