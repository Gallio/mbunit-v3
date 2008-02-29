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
using Gallio.Framework.Data;
using Gallio.Framework.Data.Binders;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Framework.Data.Binders
{
    [TestFixture]
    [TestsOn(typeof(ConstantDataBindingAccessor))]
    public class ConstantDataBindingAccessorTest : BaseUnitTest
    {
        [Test, ExpectedArgumentNullException]
        public void GetValueThrowsIfItemIsNull()
        {
            new ConstantDataBindingAccessor(42).GetValue(null);
        }

        [Test]
        public void GetValueReturnsSameConstantAndSuppliedInTheConstructor()
        {
            DataBindingItem item = new DataBindingItem(Mocks.Stub<IDataRow>());
            Assert.AreEqual(42, new ConstantDataBindingAccessor(42).GetValue(item));
        }
    }
}