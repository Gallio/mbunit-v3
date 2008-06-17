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
using Gallio.Framework.Conversions;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(BaseDataBinder))]
    public class BaseDataBinderTest : BaseUnitTest
    {
        [Test, ExpectedArgumentNullException]
        public void RegisterThrowsIfContextIsNull()
        {
            ScalarDataBinder binder = new ScalarDataBinder(new DataBinding(0, null), "");
            binder.Register(null, Mocks.Stub<IDataSourceResolver>());
        }

        [Test, ExpectedArgumentNullException]
        public void RegisterThrowsIfResolverIsNull()
        {
            ScalarDataBinder binder = new ScalarDataBinder(new DataBinding(0, null), "");
            binder.Register(new DataBindingContext(Mocks.Stub<IConverter>()), null);
        }
    }
}
