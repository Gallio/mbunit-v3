using System;
using System.Collections.Generic;
using System.Linq;
using MbUnit.Framework;

namespace Gallio.Ambience.Tests
{
    [TestsOn(typeof(Ambient))]
    public class AmbientTest
    {
        private const int PortNumber = 65436;
        private const string UserName = "Test";
        private const string Password = "Password";

        private AmbienceServer server;

        [FixtureSetUp]
        public void SetUp()
        {
            server = new AmbienceServer(new AmbienceServerConfiguration()
            {
                Port = PortNumber,
                DatabasePath = "AmbientTest.db",
                Credential = { UserName = UserName, Password = Password }
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
        public void DefaultConfiguration_PopulatedFromConfigSection()
        {
            AmbienceClientConfiguration config = Ambient.DefaultClientConfiguration;
            Assert.AreEqual("localhost", config.HostName);
            Assert.AreEqual(PortNumber, config.Port);
            Assert.AreEqual(UserName, config.Credential.UserName);
            Assert.AreEqual(Password, config.Credential.Password);
        }

        [Test]
        public void DefaultConfiguration_CanOverrideSetting()
        {
            AmbienceClientConfiguration oldConfig = Ambient.DefaultClientConfiguration;
            try
            {
                AmbienceClientConfiguration newConfig = new AmbienceClientConfiguration();
                Ambient.DefaultClientConfiguration = newConfig;
                Assert.AreSame(newConfig, Ambient.DefaultClientConfiguration);
            }
            finally
            {
                Ambient.DefaultClientConfiguration = oldConfig;
            }
        }

        [Test]
        public void DefaultConfiguration_ThrowsWhenValueSetToNull()
        {
            Assert.Throws<ArgumentNullException>(() => Ambient.DefaultClientConfiguration = null);
        }

        [Test]
        public void CanStoreAndDeleteAll()
        {
            Ambient.Data.Store(new Item());
            Assert.AreNotEqual(0, Ambient.Data.Query<Item>().Count());

            Ambient.Data.DeleteAll();
            Assert.AreEqual(0, Ambient.Data.Query<Item>().Count());
        }

        [Test]
        public void CanStoreAndQuery()
        {
            Ambient.Data.DeleteAll();

            Ambient.Data.Store(new Item { Name = "A", Value = 1 });
            Ambient.Data.Store(new Item { Name = "B", Value = 2 });

            IList<Item> items = Ambient.Data.Query<Item>();
            Assert.AreElementsEqualIgnoringOrder(new[] { new Item { Name = "A", Value = 1 }, new Item { Name = "B", Value = 2 } }, items);
        }

        [Test]
        public void CanStoreAndQueryWithPredicate()
        {
            Ambient.Data.DeleteAll();

            Ambient.Data.Store(new Item { Name = "A", Value = 1 });
            Ambient.Data.Store(new Item { Name = "B", Value = 2 });
            Ambient.Data.Store(new Item { Name = "C", Value = 3 });

            IList<Item> items = Ambient.Data.Query<Item>(x => x.Value > 1);
            Assert.AreElementsEqualIgnoringOrder(new[] { new Item { Name = "B", Value = 2 }, new Item { Name = "C", Value = 3 } }, items);
        }

        [Test]
        public void CanStoreAndDelete()
        {
            Ambient.Data.DeleteAll();

            Item b = new Item { Name = "B", Value = 2 };
            Ambient.Data.Store(new Item { Name = "A", Value = 1 });
            Ambient.Data.Store(b);

            Ambient.Data.Delete(b);

            IList<Item> items = Ambient.Data.Query<Item>();
            Assert.AreElementsEqualIgnoringOrder(new[] { new Item { Name = "A", Value = 1 } }, items);
        }

        [Test]
        public void DeleteDoesNothingIfElementNotFound()
        {
            Ambient.Data.DeleteAll();

            Ambient.Data.Delete(new Item { Name = "A", Value = 1 });

            IList<Item> items = Ambient.Data.Query<Item>();
            Assert.AreEqual(0, items.Count());
        }

        [Test]
        public void CanStoreAndQueryWithLinq()
        {
            Ambient.Data.DeleteAll();

            Ambient.Data.Store(new Item { Name = "A", Value = 1 });
            Ambient.Data.Store(new Item { Name = "B", Value = 4 });
            Ambient.Data.Store(new Item { Name = "B", Value = 2 });
            Ambient.Data.Store(new Item { Name = "C", Value = 3 });
            Ambient.Data.Store(new Item { Name = "B", Value = 3 });

            IList<Item> items = (from Item x in Ambient.Data where x.Value > 1 orderby x.Name orderby x.Value descending select x).ToList();
            Assert.AreElementsEqual(new[] 
            {
                new Item { Name = "B", Value = 4 },
                new Item { Name = "B", Value = 3 },
                new Item { Name = "B", Value = 2 },
                new Item { Name = "C", Value = 3 } 
            }, items);

            items = (from Item x in Ambient.Data where x.Value > 1 orderby x.Name descending orderby x.Value select x).ToList();
            Assert.AreElementsEqual(new[] 
            {
                new Item { Name = "C", Value = 3 },
                new Item { Name = "B", Value = 2 },
                new Item { Name = "B", Value = 3 },
                new Item { Name = "B", Value = 4 }
            }, items);

            Assert.AreEqual(4, items.Count());
        }
    }
}
