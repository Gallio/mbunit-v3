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
