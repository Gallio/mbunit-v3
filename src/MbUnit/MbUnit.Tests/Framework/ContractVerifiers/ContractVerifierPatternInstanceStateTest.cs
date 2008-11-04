using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers.Patterns;

namespace MbUnit.Tests.Framework.ContractVerifiers.Patterns
{
    [TestFixture]
    public class ContractVerifierPatternInstanceStateTest
    {
        [Test]
        public void ConstructsOk(
            [Column(null, typeof(object))] Type fixtureType,
            [Column(true,false)] bool nullInstance)
        {
            var fixtureInstance = nullInstance ? null : new object();
            var state = new ContractVerifierPatternInstanceState(fixtureType, fixtureInstance);
            Assert.AreEqual(fixtureType, state.FixtureType);
            Assert.AreSame(fixtureInstance, state.FixtureInstance);
        }
    }
}
