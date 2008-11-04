// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Framework.Data;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(BoundDataAccessor))]
    public class BoundDataAccessorTest : BaseTestWithMocks
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfDataBindingIsNull()
        {
            new BoundDataAccessor(null);
        }

        [Test]
        public void GetValueThrowsIfItemIsNull()
        {
            BoundDataAccessor accessor = new BoundDataAccessor(new DataBinding(0, null));

            Assert.Throws<ArgumentNullException>(delegate { accessor.GetValue(null); });
        }

        [Test]
        public void GetValueCallsRowsGetValueWithTheBinding()
        {
            IDataItem item = Mocks.StrictMock<IDataItem>();

            DataBinding binding = new DataBinding(0, null);

            using (Mocks.Record())
            {
                Expect.Call(item.GetValue(binding)).Return(42);
            }

            using (Mocks.Playback())
            {
                BoundDataAccessor accessor = new BoundDataAccessor(binding);
                Assert.AreEqual(42, accessor.GetValue(item));
            }
        }
    }
}
