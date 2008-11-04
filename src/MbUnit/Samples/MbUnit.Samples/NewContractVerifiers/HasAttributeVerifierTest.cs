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
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using MbUnit.Framework.NewContractVerifiers;

namespace MbUnit.Samples.NewContractVerifiers
{
    [TestFixture]
    public class HasAttributeVerifierTest
    {
        public class FooAttribute : Attribute
        {
        }

        [Foo]
        public class SampleWithAttribute
        {
        }

        public class SampleWithoutAttribute
        {
        }

        //[ContractVerifier]
        //public readonly HasAttributeVerifier<SampleWithoutAttribute, FooAttribute> Test2
        //    = new HasAttributeVerifier<SampleWithoutAttribute, FooAttribute>();

        [Test]
        public void Witness()
        {
            Assert.IsTrue(typeof(SampleWithAttribute).IsDefined(typeof(FooAttribute), false));
            Assert.IsFalse(typeof(SampleWithoutAttribute).IsDefined(typeof(FooAttribute), false));
        }

        [ContractVerifier]
        public readonly HasAttributeVerifier<SampleWithAttribute, FooAttribute> Test1
            = new HasAttributeVerifier<SampleWithAttribute, FooAttribute>();

    }
}
