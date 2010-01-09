// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gallio.Common.Reflection;
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
        public void Registry_Always_ReturnsRegistry()
        {
            var registry = MockRepository.GenerateStub<IRegistry>();
            var serviceLocator = new RegistryServiceLocator(registry);

            Assert.AreSame(registry, serviceLocator.Registry);
        }

        public class ResolvingInstances
        {
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
                var serviceDescriptor = registry.RegisterService(typeof(DummyService));
                registry.RegisterComponent(serviceDescriptor, "component1Id", typeof(DummyComponent));
                registry.RegisterComponent(serviceDescriptor, "component2Id", typeof(DummyComponent2));

                var ex = Assert.Throws<RuntimeException>(() => serviceLocator.Resolve(typeof(DummyService)));
                Assert.Contains(ex.Message, "Could not resolve component for service type '" + typeof(DummyService) + "' because there are more than one of them registered and enabled so the request is ambiguous.");
            }

            [Test]
            public void Resolve_WhenServiceTypeMapsToExactlyOneComponent_ReturnsResolvedComponent()
            {
                var registry = new FakeRegistry();
                var serviceLocator = new RegistryServiceLocator(registry);
                var serviceDescriptor = registry.RegisterService(typeof(DummyService));
                registry.RegisterComponent(serviceDescriptor, "componentId", typeof(DummyComponent));

                var component = (DummyComponent)serviceLocator.Resolve(typeof(DummyService));

                Assert.IsNotNull(component);
            }

            [Test]
            public void Resolve_WhenServiceTypeMapsToExactlyOneComponentButItIsDisabled_Throws()
            {
                var registry = new FakeRegistry();
                var serviceLocator = new RegistryServiceLocator(registry);
                var serviceDescriptor = registry.RegisterService(typeof(DummyService));
                registry.RegisterDisabledComponent(serviceDescriptor, "componentId", typeof(DummyComponent));

                var ex = Assert.Throws<RuntimeException>(() => serviceLocator.Resolve(typeof(DummyService)));
                Assert.Contains(ex.Message, "Could not resolve component for service type '" + typeof(DummyService) + "' because there do not appear to be any components registered and enabled for that service type.");
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
                var serviceDescriptor = registry.RegisterService(typeof(DummyService));
                registry.RegisterComponent(serviceDescriptor, "component1Id", typeof(DummyComponent));
                registry.RegisterComponent(serviceDescriptor, "component2Id", typeof(DummyComponent2));

                var ex = Assert.Throws<RuntimeException>(() => serviceLocator.Resolve<DummyService>());
                Assert.Contains(ex.Message, "Could not resolve component for service type '" + typeof(DummyService) + "' because there are more than one of them registered and enabled so the request is ambiguous.");
            }

            [Test]
            public void ResolveGeneric_WhenServiceTypeMapsToExactlyOneComponent_ReturnsResolvedComponent()
            {
                var registry = new FakeRegistry();
                var serviceLocator = new RegistryServiceLocator(registry);
                var serviceDescriptor = registry.RegisterService(typeof(DummyService));
                registry.RegisterComponent(serviceDescriptor, "componentId", typeof(DummyComponent));

                var component = (DummyComponent)serviceLocator.Resolve<DummyService>();

                Assert.IsNotNull(component);
            }

            [Test]
            public void ResolveGeneric_WhenServiceTypeMapsToExactlyOneComponentButItIsDisabled_Throws()
            {
                var registry = new FakeRegistry();
                var serviceLocator = new RegistryServiceLocator(registry);
                var serviceDescriptor = registry.RegisterService(typeof(DummyService));
                registry.RegisterDisabledComponent(serviceDescriptor, "componentId", typeof(DummyComponent));

                var ex = Assert.Throws<RuntimeException>(() => serviceLocator.Resolve<DummyService>());
                Assert.Contains(ex.Message, "Could not resolve component for service type '" + typeof(DummyService) + "' because there do not appear to be any components registered and enabled for that service type.");
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
                var serviceDescriptor = registry.RegisterService(typeof(DummyService));
                registry.RegisterComponent(serviceDescriptor, "component1Id", typeof(DummyComponent));
                registry.RegisterComponent(serviceDescriptor, "component2Id", typeof(DummyComponent2));

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
                var serviceDescriptor = registry.RegisterService(typeof(DummyService));
                registry.RegisterComponent(serviceDescriptor, "component1Id", typeof(DummyComponent));
                registry.RegisterComponent(serviceDescriptor, "component2Id", typeof(DummyComponent2));

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
                var serviceDescriptor = registry.RegisterService(typeof(DummyService));
                registry.RegisterComponent(serviceDescriptor, "componentId", typeof(DummyComponent));

                var component = (DummyComponent)serviceLocator.ResolveByComponentId("componentId");

                Assert.IsNotNull(component);
            }

            [Test]
            public void ResolveByComponentId_WhenComponentRegisteredButItIsDisabled_ReturnsResolvedComponent()
            {
                var registry = new FakeRegistry();
                var serviceLocator = new RegistryServiceLocator(registry);
                var serviceDescriptor = registry.RegisterService(typeof(DummyService));
                registry.RegisterDisabledComponent(serviceDescriptor, "componentId", typeof(DummyComponent));

                var ex = Assert.Throws<RuntimeException>(() => serviceLocator.ResolveByComponentId("componentId"));
                Assert.Contains(ex.Message, "Could not resolve component with id 'componentId' because it has been disabled.  Reason: ");
            }
        }

        public class ResolvingHandles
        {
            [Test]
            public void ResolveHandle_WhenServiceTypeIsNull_Throws()
            {
                var registry = new FakeRegistry();
                var serviceLocator = new RegistryServiceLocator(registry);

                Assert.Throws<ArgumentNullException>(() => serviceLocator.ResolveHandle(null));
            }

            [Test]
            public void ResolveHandle_WhenServiceTypeNotRegistered_Throws()
            {
                var registry = new FakeRegistry();
                var serviceLocator = new RegistryServiceLocator(registry);

                var ex = Assert.Throws<RuntimeException>(() => serviceLocator.ResolveHandle(typeof(DummyService)));
                Assert.Contains(ex.Message, "Could not resolve component for service type '" + typeof(DummyService) + "' because there do not appear to be any components registered and enabled for that service type.");
            }

            [Test]
            public void ResolveHandle_WhenServiceTypeRegisteredButNoComponents_Throws()
            {
                var registry = new FakeRegistry();
                var serviceLocator = new RegistryServiceLocator(registry);
                registry.RegisterService(typeof(DummyService));

                var ex = Assert.Throws<RuntimeException>(() => serviceLocator.ResolveHandle(typeof(DummyService)));
                Assert.Contains(ex.Message, "Could not resolve component for service type '" + typeof(DummyService) + "' because there do not appear to be any components registered and enabled for that service type.");
            }

            [Test]
            public void ResolveHandle_WhenServiceTypeRegisteredButMoreThanOneComponent_Throws()
            {
                var registry = new FakeRegistry();
                var serviceLocator = new RegistryServiceLocator(registry);
                var serviceDescriptor = registry.RegisterService(typeof(DummyService));
                registry.RegisterComponent(serviceDescriptor, "component1Id", typeof(DummyComponent));
                registry.RegisterComponent(serviceDescriptor, "component2Id", typeof(DummyComponent2));

                var ex = Assert.Throws<RuntimeException>(() => serviceLocator.ResolveHandle(typeof(DummyService)));
                Assert.Contains(ex.Message, "Could not resolve component for service type '" + typeof(DummyService) + "' because there are more than one of them registered and enabled so the request is ambiguous.");
            }

            [Test]
            public void ResolveHandle_WhenServiceTypeMapsToExactlyOneComponent_ReturnsComponentHandle()
            {
                var registry = new FakeRegistry();
                var serviceLocator = new RegistryServiceLocator(registry);
                var serviceDescriptor = registry.RegisterService(typeof(DummyService));
                var componentDescriptor = registry.RegisterComponent(serviceDescriptor, "componentId", typeof(DummyComponent));

                var componentHandle = serviceLocator.ResolveHandle(typeof(DummyService));

                Assert.AreSame(componentDescriptor, componentHandle.Descriptor);
            }

            [Test]
            public void ResolveHandle_WhenServiceTypeMapsToExactlyOneComponentButItIsDisabled_Throws()
            {
                var registry = new FakeRegistry();
                var serviceLocator = new RegistryServiceLocator(registry);
                var serviceDescriptor = registry.RegisterService(typeof(DummyService));
                registry.RegisterDisabledComponent(serviceDescriptor, "componentId", typeof(DummyComponent));

                var ex = Assert.Throws<RuntimeException>(() => serviceLocator.ResolveHandle(typeof(DummyService)));
                Assert.Contains(ex.Message, "Could not resolve component for service type '" + typeof(DummyService) + "' because there do not appear to be any components registered and enabled for that service type.");
            }

            [Test]
            public void ResolveHandleGeneric_WhenServiceTypeNotRegistered_Throws()
            {
                var registry = new FakeRegistry();
                var serviceLocator = new RegistryServiceLocator(registry);

                var ex = Assert.Throws<RuntimeException>(() => serviceLocator.ResolveHandle<DummyService, DummyTraits>());
                Assert.Contains(ex.Message, "Could not resolve component for service type '" + typeof(DummyService) + "' because there do not appear to be any components registered and enabled for that service type.");
            }

            [Test]
            public void ResolveHandleGeneric_WhenServiceTypeRegisteredButNoComponents_Throws()
            {
                var registry = new FakeRegistry();
                var serviceLocator = new RegistryServiceLocator(registry);
                registry.RegisterService(typeof(DummyService));

                var ex = Assert.Throws<RuntimeException>(() => serviceLocator.ResolveHandle<DummyService, DummyTraits>());
                Assert.Contains(ex.Message, "Could not resolve component for service type '" + typeof(DummyService) + "' because there do not appear to be any components registered and enabled for that service type.");
            }

            [Test]
            public void ResolveHandleGeneric_WhenServiceTypeRegisteredButMoreThanOneComponent_Throws()
            {
                var registry = new FakeRegistry();
                var serviceLocator = new RegistryServiceLocator(registry);
                var serviceDescriptor = registry.RegisterService(typeof(DummyService));
                registry.RegisterComponent(serviceDescriptor, "component1Id", typeof(DummyComponent));
                registry.RegisterComponent(serviceDescriptor, "component2Id", typeof(DummyComponent2));

                var ex = Assert.Throws<RuntimeException>(() => serviceLocator.ResolveHandle<DummyService, DummyTraits>());
                Assert.Contains(ex.Message, "Could not resolve component for service type '" + typeof(DummyService) + "' because there are more than one of them registered and enabled so the request is ambiguous.");
            }

            [Test]
            public void ResolveHandleGeneric_WhenServiceTypeMapsToExactlyOneComponent_ReturnsComponentHandle()
            {
                var registry = new FakeRegistry();
                var serviceLocator = new RegistryServiceLocator(registry);
                var serviceDescriptor = registry.RegisterService(typeof(DummyService));
                var componentDescriptor = registry.RegisterComponent(serviceDescriptor, "componentId", typeof(DummyComponent));

                var componentHandle = serviceLocator.ResolveHandle<DummyService, DummyTraits>();

                Assert.AreSame(componentDescriptor, componentHandle.Descriptor);
            }

            [Test]
            public void ResolveHandleGeneric_WhenServiceTypeMapsToExactlyOneComponentButItIsDisabled_Throws()
            {
                var registry = new FakeRegistry();
                var serviceLocator = new RegistryServiceLocator(registry);
                var serviceDescriptor = registry.RegisterService(typeof(DummyService));
                registry.RegisterDisabledComponent(serviceDescriptor, "componentId", typeof(DummyComponent));

                var ex = Assert.Throws<RuntimeException>(() => serviceLocator.ResolveHandle<DummyService, DummyTraits>());
                Assert.Contains(ex.Message, "Could not resolve component for service type '" + typeof(DummyService) + "' because there do not appear to be any components registered and enabled for that service type.");
            }

            [Test]
            public void ResolveAllHandles_WhenServiceTypeIsNull_Throws()
            {
                var registry = new FakeRegistry();
                var serviceLocator = new RegistryServiceLocator(registry);

                Assert.Throws<ArgumentNullException>(() => serviceLocator.ResolveAllHandles(null));
            }

            [Test]
            public void ResolveAllHandles_WhenServiceTypeNotRegistered_ReturnsAnEmptyList()
            {
                var registry = new FakeRegistry();
                var serviceLocator = new RegistryServiceLocator(registry);

                var result = serviceLocator.ResolveAllHandles(typeof(DummyService));

                Assert.IsEmpty(result);
            }

            [Test]
            public void ResolveAllHandles_WhenServiceTypeRegisteredButNoComponents_ReturnsAnEmptyList()
            {
                var registry = new FakeRegistry();
                var serviceLocator = new RegistryServiceLocator(registry);
                registry.RegisterService(typeof(DummyService));

                var result = serviceLocator.ResolveAllHandles(typeof(DummyService));

                Assert.IsEmpty(result);
            }

            [Test]
            public void ResolveAllHandles_WhenServiceTypeRegisteredAndAtLeastOneComponent_ReturnsAllComponentHandles()
            {
                var registry = new FakeRegistry();
                var serviceLocator = new RegistryServiceLocator(registry);
                var serviceDescriptor = registry.RegisterService(typeof(DummyService));
                var component1Descriptor = registry.RegisterComponent(serviceDescriptor, "component1Id", typeof(DummyComponent));
                var component2Descriptor = registry.RegisterComponent(serviceDescriptor, "component2Id", typeof(DummyComponent2));

                var result = serviceLocator.ResolveAllHandles(typeof(DummyService));

                Assert.AreEqual(2, result.Count);
                Assert.AreSame(component1Descriptor, result[0].Descriptor);
                Assert.AreSame(component2Descriptor, result[1].Descriptor);
            }

            [Test]
            public void ResolveAllHandlesGeneric_WhenServiceTypeNotRegistered_ReturnsAnEmptyList()
            {
                var registry = new FakeRegistry();
                var serviceLocator = new RegistryServiceLocator(registry);

                var result = serviceLocator.ResolveAllHandles<DummyService, DummyTraits>();

                Assert.IsEmpty(result);
            }

            [Test]
            public void ResolveAllHandlesGeneric_WhenServiceTypeRegisteredButNoComponents_ReturnsAnEmptyList()
            {
                var registry = new FakeRegistry();
                var serviceLocator = new RegistryServiceLocator(registry);
                registry.RegisterService(typeof(DummyService));

                var result = serviceLocator.ResolveAllHandles<DummyService, DummyTraits>();

                Assert.IsEmpty(result);
            }

            [Test]
            public void ResolveAllHandlesGeneric_WhenServiceTypeRegisteredAndAtLeastOneComponent_ReturnsAllComponentHandles()
            {
                var registry = new FakeRegistry();
                var serviceLocator = new RegistryServiceLocator(registry);
                var serviceDescriptor = registry.RegisterService(typeof(DummyService));
                var component1Descriptor = registry.RegisterComponent(serviceDescriptor, "component1Id", typeof(DummyComponent));
                var component2Descriptor = registry.RegisterComponent(serviceDescriptor, "component2Id", typeof(DummyComponent2));

                var result = serviceLocator.ResolveAllHandles<DummyService, DummyTraits>();

                Assert.AreEqual(2, result.Count);
                Assert.AreSame(component1Descriptor, result[0].Descriptor);
                Assert.AreSame(component2Descriptor, result[1].Descriptor);
            }

            [Test]
            public void ResolveHandleByComponentId_WhenComponentIdIsNull_Throws()
            {
                var registry = new FakeRegistry();
                var serviceLocator = new RegistryServiceLocator(registry);

                Assert.Throws<ArgumentNullException>(() => serviceLocator.ResolveHandleByComponentId(null));
            }

            [Test]
            public void ResolveHandleByComponentId_WhenComponentNotRegistered_Throws()
            {
                var registry = new FakeRegistry();
                var serviceLocator = new RegistryServiceLocator(registry);

                var ex = Assert.Throws<RuntimeException>(() => serviceLocator.ResolveHandleByComponentId("componentId"));
                Assert.Contains(ex.Message, "Could not resolve component with id 'componentId' because it does not appear to be registered.");
            }

            [Test]
            public void ResolveHandleByComponentId_WhenComponentRegistered_ReturnsComponentHandle()
            {
                var registry = new FakeRegistry();
                var serviceLocator = new RegistryServiceLocator(registry);
                var serviceDescriptor = registry.RegisterService(typeof(DummyService));
                var componentDescriptor = registry.RegisterComponent(serviceDescriptor, "componentId", typeof(DummyComponent));

                var componentHandle = serviceLocator.ResolveHandleByComponentId("componentId");

                Assert.AreSame(componentDescriptor, componentHandle.Descriptor);
            }

            [Test]
            public void ResolveHandleByComponentId_WhenComponentRegisteredButItIsDisabled_ReturnsComponentHandle()
            {
                var registry = new FakeRegistry();
                var serviceLocator = new RegistryServiceLocator(registry);
                var serviceDescriptor = registry.RegisterService(typeof(DummyService));
                registry.RegisterDisabledComponent(serviceDescriptor, "componentId", typeof(DummyComponent));

                var ex = Assert.Throws<RuntimeException>(() => serviceLocator.ResolveHandleByComponentId("componentId"));
                Assert.Contains(ex.Message, "Could not resolve component with id 'componentId' because it has been disabled.  Reason: ");
            }
        }

        public class ExistanceQueries
        {
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
        }

        [Traits(typeof(DummyTraits))]
        private interface DummyService
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
