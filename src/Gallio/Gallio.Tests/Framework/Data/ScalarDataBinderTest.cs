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
using Gallio.Runtime.Conversions;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(ScalarDataBinder))]
    [DependsOn(typeof(BaseDataBinderTest))]
    public class ScalarDataBinderTest : BaseTestWithMocks
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfDataBindingIsNull()
        {
            new ScalarDataBinder(null, "");
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfSourceNameIsNull()
        {
            new ScalarDataBinder(new DataBinding(0, null), null);
        }

        [Test]
        public void RegisterThrowsIfTheDataSourceCannotBeResolvedByName()
        {
            ScalarDataBinder binder = new ScalarDataBinder( new DataBinding(0, null), "name");

            IDataSourceResolver resolver = Mocks.StrictMock<IDataSourceResolver>();

            using (Mocks.Record())
            {
                Expect.Call(resolver.ResolveDataSource("name")).Return(null);
            }

            using (Mocks.Playback())
            {
                DataBindingContext context = new DataBindingContext(new NullConverter());
                Assert.Throws<DataBindingException>(delegate { binder.Register(context, resolver); });
            }
        }

        [Test]
        public void AccessorObtainsAValueFromTheRow()
        {
            DataBinding binding = new DataBinding(0, null);
            ScalarDataBinder binder = new ScalarDataBinder(binding, "name");

            IDataSourceResolver resolver = Mocks.StrictMock<IDataSourceResolver>();
            DataBindingContext context = new DataBindingContext(new NullConverter());

            DataSource source = new DataSource("name");
            source.AddDataSet(new ItemSequenceDataSet(new IDataItem[]
            {
                new ScalarDataItem<int>(42, null, false),
                new ScalarDataItem<string>("42", null, false)
            }, 1));

            using (Mocks.Record())
            {
                Expect.Call(resolver.ResolveDataSource("name")).Return(source);
            }

            using (Mocks.Playback())
            {
                IDataAccessor accessor = binder.Register(context, resolver);
                Assert.IsTrue(context.DataSets.Contains(source), "The data sets list should contain the source that was resolved during binder registration.");

                List<IDataItem> items = new List<IDataItem>(context.GetItems(true));
                Assert.Count(2, items);

                Assert.AreEqual(42, accessor.GetValue(items[0]));
                Assert.AreEqual("42", accessor.GetValue(items[1]));
            }
        }
    }
}
