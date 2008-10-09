using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Ambience.Impl;
using MbUnit.Framework;

namespace Gallio.Ambience.Tests
{
    [TestsOn(typeof(AmbienceClient))]
    public class AmbienceClientTest
    {
        private const int PortNumber = 65435;
        private AmbienceServer server;

        [FixtureSetUp]
        public void SetUp()
        {
            server = new AmbienceServer(new AmbienceServerConfiguration()
            {
                Port = PortNumber,
                DatabasePath = "AmbienceClientTest.db"
            });
            server.Start();
        }

        [FixtureTearDown]
        public void TearDown()
        {
            if (server != null)
            {
                server.Dispose();
                server = null;
            }
        }

        [Test]
        public void ConnectThrowsIfConfigIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => AmbienceClient.Connect(null));
        }

        [Test]
        public void ContainerThrowsIfDisposed()
        {
            using (AmbienceClient client = AmbienceClient.Connect(new AmbienceClientConfiguration() { Port = PortNumber }))
            {
                client.Dispose();

                Assert.Throws<ObjectDisposedException>(() => GC.KeepAlive(client.Container));
            }
        }

        [Test]
        public void ContainerIsAWrapperForDb4o()
        {
            using (AmbienceClient client = AmbienceClient.Connect(new AmbienceClientConfiguration() { Port = PortNumber }))
            {
                Assert.IsInstanceOfType(typeof(Db4oAmbientDataContainer), client.Container);
            }
        }
    }
}
