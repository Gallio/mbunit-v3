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

using System;
using Gallio.Framework.Data;
using Gallio.Framework.Data.Binders;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Framework.Data.Binders
{
    [TestFixture]
    [TestsOn(typeof(RowDataBindingAccessor))]
    public class RowDataBindingAccessorTest : BaseUnitTest
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfDataBindingIsNull()
        {
            new RowDataBindingAccessor(null);
        }

        [Test]
        public void GetValueThrowsIfItemIsNull()
        {
            RowDataBindingAccessor accessor = new RowDataBindingAccessor(new SimpleDataBinding(0, null));

            InterimAssert.Throws<ArgumentNullException>(delegate { accessor.GetValue(null); });
        }

        [Test]
        public void GetValueCallsRowsGetValueWithTheBinding()
        {
            IDataRow row = Mocks.CreateMock<IDataRow>();

            DataBinding binding = new SimpleDataBinding(0, null);
            DataBindingItem item = new DataBindingItem(row);

            using (Mocks.Record())
            {
                Expect.Call(row.GetValue(binding)).Return(42);
            }

            using (Mocks.Playback())
            {
                RowDataBindingAccessor accessor = new RowDataBindingAccessor(binding);
                Assert.AreEqual(42, accessor.GetValue(item));
            }
        }
    }
}
