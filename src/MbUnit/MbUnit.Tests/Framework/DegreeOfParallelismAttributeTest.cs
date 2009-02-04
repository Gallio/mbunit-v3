using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework.Pattern;
using MbUnit.Framework;

// Note: Used by DegreeOfParalleism test.
[assembly: DegreeOfParallelism(3)]

namespace MbUnit.Tests.Framework
{
    public class DegreeOfParallelismAttributeTest
    {
        [Test]
        public void AttributeShouldSetExecutionParameterAtRuntime()
        {
            Assert.AreEqual(3, TestAssemblyExecutionParameters.DegreeOfParallelism);
        }

        [Test]
        public void DisallowsFewerThanOneThread()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DegreeOfParallelismAttribute(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DegreeOfParallelismAttribute(-1));
        }

        [Test]
        public void ConstructorSetsField()
        {
            var attrib = new DegreeOfParallelismAttribute(1);
            Assert.AreEqual(1, attrib.DegreeOfParallelism);

            attrib = new DegreeOfParallelismAttribute(5);
            Assert.AreEqual(5, attrib.DegreeOfParallelism);
        }
    }
}
