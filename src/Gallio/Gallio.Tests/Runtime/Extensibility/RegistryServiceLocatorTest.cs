using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gallio.Reflection;
using Gallio.Runtime;
using Gallio.Runtime.Extensibility;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Runtime.Extensibility
{
    [TestsOn(typeof(RegistryServiceLocator))]
    public class RegistryServiceLocatorTest
    {
        [Test]
        public void Constructor_WhenRegistryIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new RegistryServiceLocator(null));
        }

        [Test]
        public void Resolve_WhenServiceTypeIsNull_Throws()
        {
            var registry = new FakeRegistry();
            var serviceLocator = new RegistryServiceLocator(registry);

            Assert.Throws<ArgumentNullException>(() => serviceLocator.Resolve(null));
        }

        [Test]
        public void Resolve_WhenServiceTypeNotRegistered_Throws()
        {
            var registry = new FakeRegistry();
            var serviceLocator = new RegistryServiceLocator(registry);

            var ex = Assert.Throws<RuntimeException>(() => serviceLocator.Resolve(typeof(DummyService)));
            Assert.Contains(ex.Message, "Could not resolve component for service type '" + typeof(DummyService) + "' because there do not appear to be any components registered and enabled for that service type.");
        }

        [Test]
        public void Resolve_WhenServiceTypeRegisteredButNoComponents_Throws()
        {
            var registry = new FakeRegistry();
            var serviceLocator = new RegistryServiceLocator(registry);
            registry.RegisterService(typeof(DummyService));

            var ex = Assert.Throws<RuntimeException>(() => serviceLocator.Resolve(typeof(DummyService)));
            Assert.Contains(ex.Message, "Could not resolve component for service type '" + typeof(DummyService) + "' because there do not appear to be any components registered and enabled for that service type.");
        }

        [Test]
        public void Resolve_WhenServiceTypeRegisteredButMoreThanOneComponent_Throws()
        {
            var registry = new FakeRegistry();
            var serviceLocator = new RegistryServiceLocator(registry);
            var service = registry.RegisterService(typeof(DummyService));
            registry.RegisterComponent(service, "component1Id", typeof(DummyComponent));
            registry.RegisterComponent(service, "component2Id", typeof(DummyComponent2));

            var ex = Assert.Throws<RuntimeException>(() => serviceLocator.Resolve(typeof(DummyService)));
            Assert.Contains(ex.Message, "Could not resolve component for service type '" + typeof(DummyService) + "' because there are more than one of them registered and enabled so the request is ambiguous.");
        }

        [Test]
        public void Resolve_WhenServiceTypeMapsToExactlyOneComponent_ReturnsResolvedComponent()
        {
            var registry = new FakeRegistry();
            var serviceLocator = new RegistryServiceLocator(registry);
            var service = registry.RegisterService(typeof(DummyService));
            registry.RegisterComponent(service, "componentId", typeof(DummyComponent));

            var component = (DummyComponent)serviceLocator.Resolve(typeof(DummyService));

            Assert.IsNotNull(component);
        }

        [Test]
        public void ResolveGeneric_WhenServiceTypeNotRegistered_Throws()
        {
            var registry = new FakeRegistry();
            var serviceLocator = new RegistryServiceLocator(registry);

            var ex = Assert.Throws<RuntimeException>(() => serviceLocator.Resolve<DummyService>());
            Assert.Contains(ex.Message, "Could not resolve component for service type '" + typeof(DummyService) + "' because there do not appear to be any components registered and enabled for that service type.");
        }

        [Test]
        public void ResolveGeneric_WhenServiceTypeRegisteredButNoComponents_Throws()
        {
            var registry = new FakeRegistry();
            var serviceLocator = new RegistryServiceLocator(registry);
            registry.RegisterService(typeof(DummyService));

            var ex = Assert.Throws<RuntimeException>(() => serviceLocator.Resolve<DummyService>());
            Assert.Contains(ex.Message, "Could not resolve component for service type '" + typeof(DummyService) + "' because there do not appear to be any components registered and enabled for that service type.");
        }

        [Test]
        public void ResolveGeneric_WhenServiceTypeRegisteredButMoreThanOneComponent_Throws()
        {
            var registry = new FakeRegistry();
            var serviceLocator = new RegistryServiceLocator(registry);
            var service = registry.RegisterService(typeof(DummyService));
            registry.RegisterComponent(service, "component1Id", typeof(DummyComponent));
            registry.RegisterComponent(service, "component2Id", typeof(DummyComponent2));

            var ex = Assert.Throws<RuntimeException>(() => serviceLocator.Resolve<DummyService>());
            Assert.Contains(ex.Message, "Could not resolve component for service type '" + typeof(DummyService) + "' because there are more than one of them registered and enabled so the request is ambiguous.");
        }

        [Test]
        public void ResolveGeneric_WhenServiceTypeMapsToExactlyOneComponent_ReturnsResolvedComponent()
        {
            var registry = new FakeRegistry();
            var serviceLocator = new RegistryServiceLocator(registry);
            var service = registry.RegisterService(typeof(DummyService));
            registry.RegisterComponent(service, "componentId", typeof(DummyComponent));

            var component = (DummyComponent)serviceLocator.Resolve<DummyService>();

            Assert.IsNotNull(component);
        }

        [Test]
        public void ResolveAll_WhenServiceTypeIsNull_Throws()
        {
            var registry = new FakeRegistry();
            var serviceLocator = new RegistryServiceLocator(registry);

            Assert.Throws<ArgumentNullException>(() => serviceLocator.ResolveAll(null));
        }

        [Test]
        public void ResolveAll_WhenServiceTypeNotRegistered_ReturnsAnEmptyList()
        {
            var registry = new FakeRegistry();
            var serviceLocator = new RegistryServiceLocator(registry);
            var result = serviceLocator.ResolveAll(typeof(DummyService));

            Assert.IsEmpty(result);
        }

        [Test]
        public void ResolveAll_WhenServiceTypeRegisteredButNoComponents_ReturnsAnEmptyList()
        {
            var registry = new FakeRegistry();
            var serviceLocator = new RegistryServiceLocator(registry);
            registry.RegisterService(typeof(DummyService));

            var result = serviceLocator.ResolveAll(typeof(DummyService));

            Assert.IsEmpty(result);
        }

        [Test]
        public void ResolveAll_WhenServiceTypeRegisteredAndAtLeastOneComponent_ReturnsAllResolvedComponents()
        {
            var registry = new FakeRegistry();
            var serviceLocator = new RegistryServiceLocator(registry);
            var service = registry.RegisterService(typeof(DummyService));
            registry.RegisterComponent(service, "component1Id", typeof(DummyComponent));
            registry.RegisterComponent(service, "component2Id", typeof(DummyComponent2));

            var result = serviceLocator.ResolveAll(typeof(DummyService));

            Assert.AreEqual(2, result.Count);
            Assert.IsInstanceOfType<DummyComponent>(result[0]);
            Assert.IsInstanceOfType<DummyComponent2>(result[1]);
        }

        [Test]
        public void ResolveAllGeneric_WhenServiceTypeNotRegistered_ReturnsAnEmptyList()
        {
            var registry = new FakeRegistry();
            var serviceLocator = new RegistryServiceLocator(registry);

            var result = serviceLocator.ResolveAll<DummyService>();

            Assert.IsEmpty(result);
        }

        [Test]
        public void ResolveAllGeneric_WhenServiceTypeRegisteredButNoComponents_ReturnsAnEmptyList()
        {
            var registry = new FakeRegistry();
            var serviceLocator = new RegistryServiceLocator(registry);
            registry.RegisterService(typeof(DummyService));

            var result = serviceLocator.ResolveAll<DummyService>();

            Assert.IsEmpty(result);
        }

        [Test]
        public void ResolveAllGeneric_WhenServiceTypeRegisteredAndAtLeastOneComponent_ReturnsAllResolvedComponents()
        {
            var registry = new FakeRegistry();
            var serviceLocator = new RegistryServiceLocator(registry);
            var service = registry.RegisterService(typeof(DummyService));
            registry.RegisterComponent(service, "component1Id", typeof(DummyComponent));
            registry.RegisterComponent(service, "component2Id", typeof(DummyComponent2));

            var result = serviceLocator.ResolveAll<DummyService>();

            Assert.AreEqual(2, result.Count);
            Assert.IsInstanceOfType<DummyComponent>(result[0]);
            Assert.IsInstanceOfType<DummyComponent2>(result[1]);
        }

        [Test]
        public void ResolveByComponentId_WhenComponentIdIsNull_Throws()
        {
            var registry = new FakeRegistry();
            var serviceLocator = new RegistryServiceLocator(registry);

            Assert.Throws<ArgumentNullException>(() => serviceLocator.ResolveByComponentId(null));
        }

        [Test]
        public void ResolveByComponentId_WhenComponentNotRegistered_Throws()
        {
            var registry = new FakeRegistry();
            var serviceLocator = new RegistryServiceLocator(registry);

            var ex = Assert.Throws<RuntimeException>(() => serviceLocator.ResolveByComponentId("componentId"));
            Assert.Contains(ex.Message, "Could not resolve component with id 'componentId' because it does not appear to be registered.");
        }

        [Test]
        public void ResolveByComponentId_WhenComponentRegistered_ReturnsResolvedComponent()
        {
            var registry = new FakeRegistry();
            var serviceLocator = new RegistryServiceLocator(registry);
            var service = registry.RegisterService(typeof(DummyService));
            registry.RegisterComponent(service, "componentId", typeof(DummyComponent));

            var component = (DummyComponent)serviceLocator.ResolveByComponentId("componentId");

            Assert.IsNotNull(component);
        }

        [Test]
        public void HasService_WhenServiceTypeIsNull_Throws()
        {
            var registry = new FakeRegistry();
            var serviceLocator = new RegistryServiceLocator(registry);

            Assert.Throws<ArgumentNullException>(() => serviceLocator.HasService(null));
        }

        [Test]
        public void HasService_WhenServiceTypeNotRegistered_ReturnsFalse()
        {
            var registry = new FakeRegistry();
            var serviceLocator = new RegistryServiceLocator(registry);

            var result = serviceLocator.HasService(typeof(DummyService));

            Assert.IsFalse(result);
        }

        [Test]
        public void HasService_WhenServiceTypeRegistered_ReturnsTrue()
        {
            var registry = new FakeRegistry();
            var serviceLocator = new RegistryServiceLocator(registry);
            registry.RegisterService(typeof(DummyService));

            var result = serviceLocator.HasService(typeof(DummyService));

            Assert.IsTrue(result);
        }

        [Test]
        public void HasService_WhenServiceTypeRegisteredButDisabled_ReturnsFalse()
        {
            var registry = new FakeRegistry();
            var serviceLocator = new RegistryServiceLocator(registry);
            registry.RegisterDisabledService(typeof(DummyService));

            var result = serviceLocator.HasService(typeof(DummyService));

            Assert.IsFalse(result);
        }

        [Test]
        public void HasComponent_WhenComponentIdIsNull_Throws()
        {
            var registry = new FakeRegistry();
            var serviceLocator = new RegistryServiceLocator(registry);

            Assert.Throws<ArgumentNullException>(() => serviceLocator.HasComponent(null));
        }

        [Test]
        public void HasComponent_WhenComponentNotRegistered_ReturnsFalse()
        {
            var registry = new FakeRegistry();
            var serviceLocator = new RegistryServiceLocator(registry);

            var result = serviceLocator.HasComponent("componentId");

            Assert.IsFalse(result);
        }

        [Test]
        public void HasComponent_WhenComponentRegistered_ReturnsTrue()
        {
            var registry = new FakeRegistry();
            var serviceLocator = new RegistryServiceLocator(registry);
            var service = registry.RegisterService(typeof(DummyService));
            registry.RegisterComponent(service, "componentId", typeof(DummyComponent));

            var result = serviceLocator.HasComponent("componentId");

            Assert.IsTrue(result);
        }

        [Test]
        public void HasComponent_WhenComponentRegisteredButDisabled_ReturnsFalse()
        {
            var registry = new FakeRegistry();
            var serviceLocator = new RegistryServiceLocator(registry);
            var service = registry.RegisterService(typeof(DummyService));
            registry.RegisterDisabledComponent(service, "componentId", typeof(DummyComponent2));

            var result = serviceLocator.HasComponent("componentId");

            Assert.IsFalse(result);
        }

        private interface DummyService
        {
        }

        [Traits(typeof(DummyTraits))]
        private interface DummyServiceWithTraits
        {
        }

        [Traits(typeof(object))]
        private interface DummyServiceWithInvalidTraits
        {
        }

        private class DummyTraits : Traits
        {
        }

        private class DummyComponent : DummyService
        {
        }

        private class DummyComponent2 : DummyService
        {
        }

        private class FakeRegistry : Registry
        {
            private int nextPluginId = 1;
            private int nextServiceId = 1;

            public IPluginDescriptor RegisterPlugin(params IPluginDescriptor[] pluginDependencies)
            {
                var pluginId = "plugin" + (nextPluginId++);
                var plugin = RegisterPlugin(new PluginRegistration(pluginId, new TypeName(typeof(DefaultPlugin)), new DirectoryInfo(@"C:\"))
                    {
                        PluginDependencies = pluginDependencies
                    });
                return plugin;
            }

            public IServiceDescriptor RegisterService(Type serviceType)
            {
                var plugin = RegisterPlugin();
                var serviceId = "service" + (nextServiceId++);
                var service = RegisterService(new ServiceRegistration(plugin, serviceId, new TypeName(serviceType)));
                return service;
            }

            public IServiceDescriptor RegisterDisabledService(Type serviceType)
            {
                var service = RegisterService(serviceType);
                service.Plugin.Disable("Disabled service.");
                return service;
            }

            public IComponentDescriptor RegisterComponent(IServiceDescriptor service, string componentId, Type componentType)
            {
                var plugin = RegisterPlugin(service.Plugin);
                var component = RegisterComponent(new ComponentRegistration(plugin, service, componentId, new TypeName(componentType)));
                return component;
            }

            public IComponentDescriptor RegisterDisabledComponent(IServiceDescriptor service, string componentId, Type componentType)
            {
                var component = RegisterComponent(service, componentId, componentType);
                component.Plugin.Disable("Disabled component.");
                return component;
            }
        }
    }
}
