using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Runtime.Extensibility;
using MbUnit.Framework;
using Gallio.Reflection;
using Rhino.Mocks;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Tests.Runtime.Extensibility
{
    [TestsOn(typeof(ServiceRegistration))]
    public class ServiceRegistrationTest
    {
        [Test]
        public void Constructor_WhenPluginIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new ServiceRegistration(null, "serviceId", new TypeName("Service, Assembly"));
            });
        }

        [Test]
        public void Constructor_WhenServiceIdIsNull_Throws()
        {
            var plugin = MockRepository.GenerateStub<IPluginDescriptor>();

            Assert.Throws<ArgumentNullException>(() =>
            {
                new ServiceRegistration(plugin, null, new TypeName("Service, Assembly"));
            });
        }

        [Test]
        public void Constructor_WhenServiceTypeNameIsNull_Throws()
        {
            var plugin = MockRepository.GenerateStub<IPluginDescriptor>();

            Assert.Throws<ArgumentNullException>(() =>
            {
                new ServiceRegistration(plugin, "serviceId", null);
            });
        }

        [Test]
        public void Plugin_Accessor_EnforcesConstraints()
        {
            var plugin = MockRepository.GenerateStub<IPluginDescriptor>();
            var registration = new ServiceRegistration(plugin, "serviceId", new TypeName("Service, Assembly"));

            Assert.AreSame(plugin, registration.Plugin);
            Assert.Throws<ArgumentNullException>(() => { registration.Plugin = null; });

            var differentPlugin = MockRepository.GenerateStub<IPluginDescriptor>();
            registration.Plugin = differentPlugin;

            Assert.AreSame(differentPlugin, registration.Plugin);
        }

        [Test]
        public void ServiceId_Accessor_EnforcesConstraints()
        {
            var plugin = MockRepository.GenerateStub<IPluginDescriptor>();
            var registration = new ServiceRegistration(plugin, "serviceId", new TypeName("Service, Assembly"));

            Assert.AreEqual("serviceId", registration.ServiceId);
            Assert.Throws<ArgumentNullException>(() => { registration.ServiceId = null; });

            registration.ServiceId = "differentServiceId";

            Assert.AreEqual("differentServiceId", registration.ServiceId);
        }

        [Test]
        public void ServiceTypeName_Accessor_EnforcesConstraints()
        {
            var plugin = MockRepository.GenerateStub<IPluginDescriptor>();
            var registration = new ServiceRegistration(plugin, "serviceId", new TypeName("Service, Assembly"));

            Assert.AreEqual(new TypeName("Service, Assembly"), registration.ServiceTypeName);
            Assert.Throws<ArgumentNullException>(() => { registration.ServiceTypeName = null; });

            registration.ServiceTypeName = new TypeName("DifferentService, Assembly");

            Assert.AreEqual(new TypeName("DifferentService, Assembly"), registration.ServiceTypeName);
        }

        [Test]
        public void TraitsHandlerFactory_Accessor_EnforcesConstraints()
        {
            var plugin = MockRepository.GenerateStub<IPluginDescriptor>();
            var registration = new ServiceRegistration(plugin, "serviceId", new TypeName("Service, Assembly"));

            Assert.IsInstanceOfType<SingletonHandlerFactory>(registration.TraitsHandlerFactory);
            Assert.Throws<ArgumentNullException>(() => { registration.TraitsHandlerFactory = null; });

            var differentHandlerFactory = MockRepository.GenerateStub<IHandlerFactory>();
            registration.TraitsHandlerFactory = differentHandlerFactory;

            Assert.AreSame(differentHandlerFactory, registration.TraitsHandlerFactory);
        }
    }
}
