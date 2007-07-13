using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace MbUnit.TestResources.NUnit
{
    [TestFixture]
    public class SimpleTest
    {
        [Test]
        public void Pass()
        {
        }

        [Test]
        public void Fail()
        {
            Assert.Fail("Boom");
        }
    }
}
