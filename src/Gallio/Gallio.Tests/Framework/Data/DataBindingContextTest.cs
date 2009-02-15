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
using Gallio.Framework.Data;
using Gallio.Framework.Conversions;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(DataBindingContext))]
    public class DataBindingContextTest : BaseTestWithMocks
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
            context.RegisterBinding(null, new DataBinding(0, null));
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
            context.RegisterBinding(dataSet1, new DataBinding(0, null));
            context.RegisterBinding(dataSet2, new DataBinding(0, null));
            context.RegisterBinding(dataSet1, new DataBinding(0, null));

            Assert.AreElementsEqual(new IDataSet[] { dataSet1, dataSet2 }, context.DataSets);
        }

        [Test]
        public void RegisteredBindingsAreFetchedByItemsAccordingToStrategy()
        {
            ItemSequenceDataSet dataSet1 = new ItemSequenceDataSet(new IDataItem[] { new ScalarDataItem<int>(1, null, false), new ScalarDataItem<int>(2, null, false) }, 1);
            ItemSequenceDataSet dataSet2 = new ItemSequenceDataSet(new IDataItem[] { new ScalarDataItem<int>(10, null, false), new ScalarDataItem<int>(20, null, false) }, 1);

            DataBindingContext context = new DataBindingContext(new NullConverter());

            IDataAccessor accessor1 = context.RegisterBinding(dataSet1, new DataBinding(0, null));
            IDataAccessor accessor2 = context.RegisterBinding(dataSet2, new DataBinding(0, null));

            List<IDataItem> combinatorialItems = new List<IDataItem>(context.GetItems(true));
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

            List<IDataItem> sequentialItems = new List<IDataItem>(context.GetItems(true));
            Assert.AreEqual(2, sequentialItems.Count);
            Assert.AreEqual(1, accessor1.GetValue(sequentialItems[0]));
            Assert.AreEqual(10, accessor2.GetValue(sequentialItems[0]));
            Assert.AreEqual(2, accessor1.GetValue(sequentialItems[1]));
            Assert.AreEqual(20, accessor2.GetValue(sequentialItems[1]));
        }

        [Test]
        public void DataBindingAccessorThrowsIfItemIsNull()
        {
            ItemSequenceDataSet dataSet = new ItemSequenceDataSet(new IDataItem[] { }, 1);
            DataBindingContext context = new DataBindingContext(Mocks.Stub<IConverter>());

            IDataAccessor accessor = context.RegisterBinding(dataSet, new DataBinding(0, null));
            Assert.Throws<ArgumentNullException>(delegate { accessor.GetValue(null); });
        }

        [Test]
        public void GetItemsReturnsASingleNullDataRowIfThereAreNoDataSetsRegistered()
        {
            DataBindingContext context = new DataBindingContext(Mocks.Stub<IConverter>());

            List<IDataItem> items = new List<IDataItem>(context.GetItems(false));
            Assert.AreEqual(1, items.Count);
            Assert.AreSame(NullDataItem.Instance, items[0]);
        }
    }
}
