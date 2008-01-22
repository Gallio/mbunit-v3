using System;
using System.Collections.Generic;
using Gallio.Collections;
using Gallio.Data;
using Rhino.Mocks;
using MbUnit.Framework;

namespace Gallio.Tests.Data
{
    [TestFixture]
    [TestsOn(typeof(UnionMergeStrategy))]
    public class UnionMergeStrategyTest : BaseUnitTest
    {
        [Test]
        public void EliminatesDuplicatesAndIncludesBadRows()
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
                IDataRow badRow = Mocks.CreateMock<IDataRow>();
                Expect.Call(badRow.GetValue(bindings[0])).Repeat.Twice().Throw(new InvalidOperationException("Test exception"));

                Expect.Call(providers[0].GetRows(bindings)).Return(new IDataRow[] {
                    new ScalarDataRow<int>(1, null),
                    new ScalarDataRow<int>(1, null),
                    new ScalarDataRow<int>(2, null),
                });

                Expect.Call(providers[1].GetRows(bindings)).Return(EmptyArray<IDataRow>.Instance);

                Expect.Call(providers[2].GetRows(bindings)).Return(new IDataRow[] {
                    badRow,
                    new ScalarDataRow<int>(3, null),
                    new ScalarDataRow<int>(2, null)
                });
            }

            using (Mocks.Playback())
            {
                List<IDataRow> rows = new List<IDataRow>(UnionMergeStrategy.Instance.Merge(providers, bindings));
                Assert.AreEqual(4, rows.Count);

                Assert.AreEqual(1, rows[0].GetValue(bindings[0]));
                Assert.AreEqual(2, rows[1].GetValue(bindings[0]));
                InterimAssert.Throws<InvalidOperationException>(delegate { rows[2].GetValue(bindings[0]); });
                Assert.AreEqual(3, rows[3].GetValue(bindings[0]));
            }
        }
    }
}