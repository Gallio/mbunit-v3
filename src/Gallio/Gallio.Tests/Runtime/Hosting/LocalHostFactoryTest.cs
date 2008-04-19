using System;
using Gallio.Framework.Utilities;
using Gallio.Runtime.Hosting;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.Hosting
{
    [TestFixture]
    [TestsOn(typeof(LocalHost))]
    [TestsOn(typeof(LocalHostFactory))]
    public class LocalHostFactoryTest : AbstractHostFactoryTest
    {
        public override IHostFactory Factory
        {
            get { return new LocalHostFactory(); }
        }

        [Test]
        public void IsLocalFlagShouldBeTrue()
        {
            using (IHost host = Factory.CreateHost(new HostSetup(), new LogStreamLogger()))
                Assert.IsTrue(host.IsLocal);
        }
    }
}