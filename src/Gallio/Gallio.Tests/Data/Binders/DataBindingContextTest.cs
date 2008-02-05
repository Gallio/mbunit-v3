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
using System.Text;
using Gallio.Data;
using Gallio.Data.Binders;
using Gallio.Data.Conversions;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Data.Binders
{
    [TestFixture]
    [TestsOn(typeof(DataBindingContext))]
    public class DataBindingContextTest : BaseUnitTest
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfConverterIsNull()
        {
            new DataBindingContext(null);
        }

        [Test]
        public void ConverterPropertyContainsSameValueAsSpecifiedInTheConstructor()
        {
            IConverter converter = Mocks.Stub<IConverter>();
            DataBindingContext context = new DataBindingContext(converter);
            Assert.AreSame(converter, context.Converter);
        }

        [Test]
        public void DefaultJoinStrategyIsCombinatorial()
        {
            IConverter converter = Mocks.Stub<IConverter>();
            DataBindingContext context = new DataBindingContext(converter);
            Assert.AreSame(CombinatorialJoinStrategy.Instance, context.Strategy);
        }

        [Test]
        public void InitialDataSetsArrayIsEmpty()
        {
            IConverter converter = Mocks.Stub<IConverter>();
            DataBindingContext context = new DataBindingContext(converter);
            Assert.AreEqual(0, context.DataSets.Count);
        }

        [Test, ExpectedArgumentNullException]
        public void RegisterBindingThrowsIfDataSetIsNull()
        {
            IConverter converter = Mocks.Stub<IConverter>();
            DataBindingContext context = new DataBindingContext(converter);
            context.RegisterBinding(null, new SimpleDataBinding(typeof(int)));
        }

        [Test, ExpectedArgumentNullException]
        public void RegisterBindingThrowsIfBindingIsNull()
        {
            IConverter converter = Mocks.Stub<IConverter>();
            DataBindingContext context = new DataBindingContext(converter);
            context.RegisterBinding(Mocks.Stub<IDataSet>(), null);
        }

        [Test]
        public void RegisteredDataSetsAppearExactlyOnceInDataSetsList()
        {
            IDataSet dataSet1 = Mocks.Stub<IDataSet>();
            IDataSet dataSet2 = Mocks.Stub<IDataSet>();

            IConverter converter = Mocks.Stub<IConverter>();
            DataBindingContext context = new DataBindingContext(converter);
            context.RegisterBinding(dataSet1, new SimpleDataBinding(typeof(int)));
            context.RegisterBinding(dataSet2, new SimpleDataBinding(typeof(int)));
            context.RegisterBinding(dataSet1, new SimpleDataBinding(typeof(int)));

            CollectionAssert.AreElementsEqual(new IDataSet[] { dataSet1, dataSet2 }, context.DataSets);
        }

        [Test]
        public void RegisteredBindingsAreFetchedByItemsAccordingToStrategy()
        {
            RowSequenceDataSet dataSet1 = new RowSequenceDataSet(new IDataRow[] { new ScalarDataRow<int>(1, null), new ScalarDataRow<int>(2, null) }, 1, false);
            RowSequenceDataSet dataSet2 = new RowSequenceDataSet(new IDataRow[] { new ScalarDataRow<int>(10, null), new ScalarDataRow<int>(20, null) }, 1, false);

            DataBindingContext context = new DataBindingContext(new NullConverter());

            IDataBindingAccessor accessor1 = context.RegisterBinding(dataSet1, new SimpleDataBinding(typeof(int), null, 0));
            IDataBindingAccessor accessor2 = context.RegisterBinding(dataSet2, new SimpleDataBinding(typeof(int), null, 0));

            List<DataBindingItem> combinatorialItems = new List<DataBindingItem>(context.GetItems());
            Assert.AreEqual(4, combinatorialItems.Count);
            Assert.AreEqual(1, accessor1.GetValue(combinatorialItems[0]));
            Assert.AreEqual(10, accessor2.GetValue(combinatorialItems[0]));
            Assert.AreEqual(1, accessor1.GetValue(combinatorialItems[1]));
            Assert.AreEqual(20, accessor2.GetValue(combinatorialItems[1]));
            Assert.AreEqual(2, accessor1.GetValue(combinatorialItems[2]));
            Assert.AreEqual(10, accessor2.GetValue(combinatorialItems[2]));
            Assert.AreEqual(2, accessor1.GetValue(combinatorialItems[3]));
            Assert.AreEqual(20, accessor2.GetValue(combinatorialItems[3]));

            context.Strategy = SequentialJoinStrategy.Instance;

            List<DataBindingItem> sequentialItems = new List<DataBindingItem>(context.GetItems());
            Assert.AreEqual(2, sequentialItems.Count);
            Assert.AreEqual(1, accessor1.GetValue(sequentialItems[0]));
            Assert.AreEqual(10, accessor2.GetValue(sequentialItems[0]));
            Assert.AreEqual(2, accessor1.GetValue(sequentialItems[1]));
            Assert.AreEqual(20, accessor2.GetValue(sequentialItems[1]));
        }

        [Test]
        public void DataBindingAccessorPerformsConversion()
        {
            IConverter converter = Mocks.CreateMock<IConverter>();

            using (Mocks.Record())
            {
                Expect.Call(converter.Convert(42, typeof(string))).Return("42");
            }

            using (Mocks.Playback())
            {
                RowSequenceDataSet dataSet = new RowSequenceDataSet(new IDataRow[] { new ScalarDataRow<int>(42, null) }, 1, false);
                DataBindingContext context = new DataBindingContext(converter);

                IDataBindingAccessor accessor = context.RegisterBinding(dataSet, new SimpleDataBinding(typeof(string), null, 0));
                List<DataBindingItem> items = new List<DataBindingItem>(context.GetItems());
                Assert.AreEqual(1, items.Count);
                Assert.AreEqual("42", accessor.GetValue(items[0]));
            }
        }

        [Test]
        public void DataBindingAccessorThrowsIfItemIsNull()
        {
            RowSequenceDataSet dataSet = new RowSequenceDataSet(new IDataRow[] { }, 1, false);
            DataBindingContext context = new DataBindingContext(Mocks.Stub<IConverter>());

            IDataBindingAccessor accessor = context.RegisterBinding(dataSet, new SimpleDataBinding(typeof(string), null, 0));
            InterimAssert.Throws<ArgumentNullException>(delegate { accessor.GetValue(null); });
        }
    }
}
