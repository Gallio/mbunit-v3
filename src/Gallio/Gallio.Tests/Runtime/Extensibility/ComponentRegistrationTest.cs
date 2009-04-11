using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Runtime.Extensibility;
using MbUnit.Framework;
using Gallio.Reflection;
using Rhino.Mocks;
using Gallio.Collections;

namespace Gallio.Tests.Runtime.Extensibility
{
    [TestsOn(typeof(ComponentRegistration))]
    public class ComponentRegistrationTest
    {
        [Test]
        public void Constructor_WhenPluginIsNull_Throws()
        {
            var service = MockRepository.GenerateStub<IServiceDescriptor>();

            Assert.Throws<ArgumentNullException>(() =>
            {
                new ComponentRegistration(null, service, "componentId", new TypeName("Component, Assembly"));
            });
        }

        [Test]
        public void Constructor_WhenServiceIsNull_Throws()
        {
            var plugin = MockRepository.GenerateStub<IPluginDescriptor>();
            var service = MockRepository.GenerateStub<IServiceDescriptor>();

            Assert.Throws<ArgumentNullException>(() =>
            {
                new ComponentRegistration(plugin, null, "componentId", new TypeName("Component, Assembly"));
            });
        }

        [Test]
        public void Constructor_WhenComponentIdIsNull_Throws()
        {
            var plugin = MockRepository.GenerateStub<IPluginDescriptor>();
            var service = MockRepository.GenerateStub<IServiceDescriptor>();

            Assert.Throws<ArgumentNullException>(() =>
            {
                new ComponentRegistration(plugin, service, null, new TypeName("Component, Assembly"));
            });
        }

        [Test]
        public void Constructor_WhenComponentTypeNameIsNull_Throws()
        {
            var plugin = MockRepository.GenerateStub<IPluginDescriptor>();
            var service = MockRepository.GenerateStub<IServiceDescriptor>();

            Assert.Throws<ArgumentNullException>(() =>
            {
                new ComponentRegistration(plugin, service, "componentId", null);
            });
        }

        [Test]
        public void Plugin_Accessor_EnforcesConstraints()
        {
            var plugin = MockRepository.GenerateStub<IPluginDescriptor>();
            var service = MockRepository.GenerateStub<IServiceDescriptor>();
            var registration = new ComponentRegistration(plugin, service, "componentId", new TypeName("Component, Assembly"));

            Assert.AreSame(plugin, registration.Plugin);
            Assert.Throws<ArgumentNullException>(() => { registration.Plugin = null; });

            var differentPlugin = MockRepository.GenerateStub<IPluginDescriptor>();
            registration.Plugin = differentPlugin;

            Assert.AreSame(differentPlugin, registration.Plugin);
        }

        [Test]
        public void Service_Accessor_EnforcesConstraints()
        {
            var plugin = MockRepository.GenerateStub<IPluginDescriptor>();
            var service = MockRepository.GenerateStub<IServiceDescriptor>();
            var registration = new ComponentRegistration(plugin, service, "componentId", new TypeName("Component, Assembly"));

            Assert.AreSame(service, registration.Service);
            Assert.Throws<ArgumentNullException>(() => { registration.Service = null; });

            var differentService = MockRepository.GenerateStub<IServiceDescriptor>();
            registration.Service = differentService;

            Assert.AreSame(differentService, registration.Service);
        }

        [Test]
        public void ComponentId_Accessor_EnforcesConstraints()
        {
            var plugin = MockRepository.GenerateStub<IPluginDescriptor>();
            var service = MockRepository.GenerateStub<IServiceDescriptor>();
            var registration = new ComponentRegistration(plugin, service, "componentId", new TypeName("Component, Assembly"));

            Assert.AreEqual("componentId", registration.ComponentId);
            Assert.Throws<ArgumentNullException>(() => { registration.ComponentId = null; });

            registration.ComponentId = "differentComponentId";

            Assert.AreEqual("differentComponentId", registration.ComponentId);
        }

        [Test]
        public void ComponentTypeName_Accessor_EnforcesConstraints()
        {
            var plugin = MockRepository.GenerateStub<IPluginDescriptor>();
            var service = MockRepository.GenerateStub<IServiceDescriptor>();
            var registration = new ComponentRegistration(plugin, service, "componentId", new TypeName("Component, Assembly"));

            Assert.AreEqual(new TypeName("Component, Assembly"), registration.ComponentTypeName);
            Assert.Throws<ArgumentNullException>(() => { registration.ComponentTypeName = null; });

            registration.ComponentTypeName = new TypeName("DifferentComponent, Assembly");

            Assert.AreEqual(new TypeName("DifferentComponent, Assembly"), registration.ComponentTypeName);
        }

        [Test]
        public void ComponentProperties_Accessor_EnforcesConstraints()
        {
            var plugin = MockRepository.GenerateStub<IPluginDescriptor>();
            var service = MockRepository.GenerateStub<IServiceDescriptor>();
            var registration = new ComponentRegistration(plugin, service, "componentId", new TypeName("Component, Assembly"));

            Assert.IsEmpty(registration.ComponentProperties);
            Assert.Throws<ArgumentNullException>(() => { registration.ComponentProperties = null; });

            var differentProperties = new PropertySet();
            registration.ComponentProperties = differentProperties;

            Assert.AreSame(differentProperties, registration.ComponentProperties);
        }

        [Test]
        public void TraitsProperties_Accessor_EnforcesConstraints()
        {
            var plugin = MockRepository.GenerateStub<IPluginDescriptor>();
            var service = MockRepository.GenerateStub<IServiceDescriptor>();
            var registration = new ComponentRegistration(plugin, service, "componentId", new TypeName("Component, Assembly"));

            Assert.IsEmpty(registration.TraitsProperties);
            Assert.Throws<ArgumentNullException>(() => { registration.TraitsProperties = null; });

            var differentProperties = new PropertySet();
            registration.TraitsProperties = differentProperties;

            Assert.AreSame(differentProperties, registration.TraitsProperties);
        }

        [Test]
        public void ComponentHandlerFactory_Accessor_EnforcesConstraints()
        {
            var plugin = MockRepository.GenerateStub<IPluginDescriptor>();
            var service = MockRepository.GenerateStub<IServiceDescriptor>();
            var registration = new ComponentRegistration(plugin, service, "componentId", new TypeName("Component, Assembly"));

            Assert.IsInstanceOfType<SingletonHandlerFactory>(registration.ComponentHandlerFactory);
            Assert.Throws<ArgumentNullException>(() => { registration.ComponentHandlerFactory = null; });

            var differentHandlerFactory = MockRepository.GenerateStub<IHandlerFactory>();
            registration.ComponentHandlerFactory = differentHandlerFactory;

            Assert.AreSame(differentHandlerFactory, registration.ComponentHandlerFactory);
        }
    }
}
