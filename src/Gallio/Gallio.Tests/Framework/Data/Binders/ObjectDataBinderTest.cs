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
using System.Reflection;
using Gallio.Framework.Data;
using Gallio.Framework.Data.Binders;
using Gallio.Framework.Data.Conversions;
using Gallio.Reflection;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Framework.Data.Binders
{
    [TestFixture]
    [TestsOn(typeof(ObjectDataBinder))]
    [DependsOn(typeof(BaseDataBinderTest))]
    [DependsOn(typeof(ObjectCreationSpecTest))]
    public class ObjectDataBinderTest : BaseUnitTest
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfTypeIsNull()
        {
            new ObjectDataBinder(null);
        }

        [Test, ExpectedArgumentNullException]
        public void SetSlotBinderThrowsIfSlotIsNull()
        {
            ObjectDataBinder binder = new ObjectDataBinder(Mocks.Stub<ITypeInfo>());
            binder.SetSlotBinder(null, Mocks.Stub<IDataBinder>());
        }

        [Test, ExpectedArgumentNullException]
        public void SetSlotBinderThrowsIfBinderIsNull()
        {
            ObjectDataBinder binder = new ObjectDataBinder(Mocks.Stub<ITypeInfo>());
            binder.SetSlotBinder(Mocks.Stub<ISlotInfo>(), null);
        }

        [Test]
        public void AccessorThrowsIfItemIsNull()
        {
            ObjectDataBinder binder = new ObjectDataBinder(Mocks.Stub<ITypeInfo>());
            IDataBindingAccessor accessor = binder.Register(new DataBindingContext(Mocks.Stub<IConverter>()), Mocks.Stub<IDataSourceResolver>());

            InterimAssert.Throws<ArgumentNullException>(delegate { accessor.GetValue(null); });
        }

        [Test]
        [Description("We only test one possible slot binding variation here.  We leave the rest up to the tests for the more primitive slot binding routines.")]
        public void AccessorCreatesNewObjectsThroughSlotBinding()
        {
            DataSource source = new DataSource("data");
            source.AddDataSet(new RowSequenceDataSet(new IDataRow[] { new ListDataRow<object>(new object[] { 42, typeof(int) }, null, false) }, 2));

            IDataSourceResolver resolver = Mocks.CreateMock<IDataSourceResolver>();
            
            using (Mocks.Record())
            {
                Expect.Call(resolver.ResolveDataSource("data")).Repeat.Twice().Return(source);
            }

            using (Mocks.Playback())
            {
                ITypeInfo type = Reflector.Wrap(typeof(Holder<>));
                IConstructorInfo constructor = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)[0];
                ISlotInfo valueSlot = constructor.Parameters[0];
                ISlotInfo typeSlot = (IGenericParameterInfo) type.GenericArguments[0];

                DataBindingContext context = new DataBindingContext(new NullConverter());

                ObjectDataBinder binder = new ObjectDataBinder(type);
                binder.SetSlotBinder(valueSlot, new ScalarDataBinder(new SimpleDataBinding(0, null), "data"));
                binder.SetSlotBinder(typeSlot, new ScalarDataBinder(new SimpleDataBinding(1, null), "data"));

                IDataBindingAccessor accessor = binder.Register(context, resolver);
                List<DataBindingItem> items = new List<DataBindingItem>(context.GetItems(true));
                Assert.AreEqual(1, items.Count);

                Holder<int> holder = (Holder<int>)accessor.GetValue(items[0]);
                Assert.AreEqual(42, holder.Value, "Should have set the value via the constructor parameter.");
            }
        }

        private class Holder<T>
        {
            private readonly T value;

            public Holder(T value)
            {
                this.value = value;
            }

            public T Value { get { return value; } }
        }
    }
}
