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
using Gallio.Framework.Data;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(ConstantDataAccessor))]
    public class ConstantDataBindingAccessorTest : BaseTestWithMocks
    {
        [Test, ExpectedArgumentNullException]
        public void GetValueThrowsIfItemIsNull()
        {
            new ConstantDataAccessor(42).GetValue(null);
        }

        [Test]
        public void GetValueReturnsSameConstantAndSuppliedInTheConstructor()
        {
            IDataItem item = Mocks.Stub<IDataItem>();
            Assert.AreEqual(42, new ConstantDataAccessor(42).GetValue(item));
        }
    }
}