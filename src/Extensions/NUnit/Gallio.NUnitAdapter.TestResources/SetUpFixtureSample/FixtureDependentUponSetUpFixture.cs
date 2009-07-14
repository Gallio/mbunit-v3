using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Gallio.NUnitAdapter.TestResources.SetUpFixtureSample
{
    [SetUpFixture]
    public class FixtureDependentUponSetUpFixture
    {
        /// <summary>
        /// This test checks that the SetUpFixture has been set up.
        /// </summary>
        [Test]
        public void VerifyThatSetUpFixtureRan()
        {
            Assert.IsTrue(SetUpFixture.IsSetUp);
        }
    }
}