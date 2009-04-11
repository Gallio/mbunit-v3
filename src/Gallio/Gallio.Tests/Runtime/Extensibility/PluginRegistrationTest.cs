using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Collections;
using Gallio.Reflection;
using Gallio.Runtime.Extensibility;
using MbUnit.Framework;
using System.IO;
using Rhino.Mocks;

namespace Gallio.Tests.Runtime.Extensibility
{
    [TestsOn(typeof(PluginRegistration))]
    public class PluginRegistrationTest
    {
        [Test]
        public void Constructor_WhenPluginIdIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new PluginRegistration(null, new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\"));
            });
        }

        [Test]
        public void Constructor_WhenPluginTypeNameIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new PluginRegistration("pluginId", null, new DirectoryInfo(@"C:\"));
            });
        }

        [Test]
        public void Constructor_WhenBaseDirectoryIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), null);
            });
        }

        [Test]
        public void PluginId_Accessor_EnforcesConstraints()
        {
            var registration = new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\"));

            Assert.AreEqual("pluginId", registration.PluginId);
            Assert.Throws<ArgumentNullException>(() => { registration.PluginId = null; });

            registration.PluginId = "differentPluginId";

            Assert.AreEqual("differentPluginId", registration.PluginId);
        }

        [Test]
        public void PluginTypeName_Accessor_EnforcesConstraints()
        {
            var registration = new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\"));

            Assert.AreEqual(new TypeName("Plugin, Assembly"), registration.PluginTypeName);
            Assert.Throws<ArgumentNullException>(() => { registration.PluginTypeName = null; });

            registration.PluginTypeName = new TypeName("DifferentPlugin, Assembly");

            Assert.AreEqual(new TypeName("DifferentPlugin, Assembly"), registration.PluginTypeName);
        }

        [Test]
        public void BaseDirectory_Accessor_EnforcesConstraints()
        {
            var registration = new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\"));

            Assert.AreEqual(@"C:\", registration.BaseDirectory.ToString());
            Assert.Throws<ArgumentNullException>(() => { registration.BaseDirectory = null; });

            registration.BaseDirectory = new DirectoryInfo(@"D:\");

            Assert.AreEqual(@"D:\", registration.BaseDirectory.ToString());
        }

        [Test]
        public void PluginProperties_Accessor_EnforcesConstraints()
        {
            var registration = new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\"));

            Assert.IsEmpty(registration.PluginProperties);
            Assert.Throws<ArgumentNullException>(() => { registration.PluginProperties = null; });

            var differentProperties = new PropertySet();
            registration.PluginProperties = differentProperties;

            Assert.AreSame(differentProperties, registration.PluginProperties);
        }

        [Test]
        public void TraitsProperties_Accessor_EnforcesConstraints()
        {
            var registration = new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\"));

            Assert.IsEmpty(registration.TraitsProperties);
            Assert.Throws<ArgumentNullException>(() => { registration.TraitsProperties = null; });

            var differentProperties = new PropertySet();
            registration.TraitsProperties = differentProperties;

            Assert.AreSame(differentProperties, registration.TraitsProperties);
        }

        [Test]
        public void PluginHandlerFactory_Accessor_EnforcesConstraints()
        {
            var registration = new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\"));

            Assert.IsInstanceOfType<SingletonHandlerFactory>(registration.PluginHandlerFactory);
            Assert.Throws<ArgumentNullException>(() => { registration.PluginHandlerFactory = null; });

            var differentHandlerFactory = MockRepository.GenerateStub<IHandlerFactory>();
            registration.PluginHandlerFactory = differentHandlerFactory;

            Assert.AreSame(differentHandlerFactory, registration.PluginHandlerFactory);
        }
    }
}
