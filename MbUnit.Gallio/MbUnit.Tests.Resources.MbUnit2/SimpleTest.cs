using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;

namespace MbUnit.Tests.Resources.MbUnit2
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
