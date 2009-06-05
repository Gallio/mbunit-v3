// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using Gallio.Common.Collections;
using Gallio.Common.Reflection;
using Gallio.Runtime;
using Gallio.Runtime.Extensibility;
using MbUnit.Framework;
using Rhino.Mocks;
using System.Reflection;

namespace Gallio.Tests.Runtime.Extensibility
{
    [TestsOn(typeof(Registry))]
    public class RegistryTest
    {
        public class Invariants
        {
            [Test]
            public void ResourceLocator_ReturnsRegistryResourceLocator()
            {
                var registry = new Registry();

                Assert.IsInstanceOfType<RegistryResourceLocator>(registry.ResourceLocator);
            }

            [Test]
            public void ServiceLocator_ReturnsServiceResourceLocator()
            {
                var registry = new Registry();

                Assert.IsInstanceOfType<RegistryServiceLocator>(registry.ServiceLocator);
            }
        }

        public class Registration
        {
            [Test]
            public void RegistryPlugin_WhenRegistrationNull_Throws()
            {
                var registry = new Registry();

                Assert.Throws<ArgumentNullException>(() => registry.RegisterPlugin(null));
            }

            [Test]
            public void RegistryPlugin_WhenRegistrationContainsNullAssemblyBinding_Throws()
            {
                var registry = new Registry();
                var pluginRegistration = new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\"));
                pluginRegistration.AssemblyBindings.Add(null);

                var ex = Assert.Throws<ArgumentException>(() => registry.RegisterPlugin(pluginRegistration));
                Assert.Contains(ex.Message, "The assembly references must not contain a null reference.");
            }

            [Test]
            public void RegistryPlugin_WhenRegistrationContainsNullPluginDependency_Throws()
            {
                var registry = new Registry();
                var pluginRegistration = new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\"));
                pluginRegistration.PluginDependencies.Add(null);

                var ex = Assert.Throws<ArgumentException>(() => registry.RegisterPlugin(pluginRegistration));
                Assert.Contains(ex.Message, "The plugin dependencies must not contain a null reference.");
            }

            [Test]
            public void RegistryPlugin_WhenRegistrationContainsNullProbingPath_Throws()
            {
                var registry = new Registry();
                var pluginRegistration = new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\"));
                pluginRegistration.ProbingPaths.Add(null);

                var ex = Assert.Throws<ArgumentException>(() => registry.RegisterPlugin(pluginRegistration));
                Assert.Contains(ex.Message, "The probing paths must not contain a null reference.");
            }

            [Test]
            public void RegistryService_WhenRegistrationNull_Throws()
            {
                var registry = new Registry();

                Assert.Throws<ArgumentNullException>(() => registry.RegisterService(null));
            }

            [Test]
            public void RegistryComponent_WhenRegistrationNull_Throws()
            {
                var registry = new Registry();

                Assert.Throws<ArgumentNullException>(() => registry.RegisterComponent(null));
            }

            [Test]
            public void RegisterPlugin_WhenArgumentsValid_RegistersThePluginAndReturnsItsDescriptor()
            {
                var registry = new Registry();
                var dependentPlugin = registry.RegisterPlugin(new PluginRegistration("plugin1Id", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var handlerFactory = MockRepository.GenerateStub<IHandlerFactory>();
                var assemblyBindings = new[] { new AssemblyBinding(new AssemblyName("Gallio")) };
                var pluginDependencies = new[] { dependentPlugin };
                var probingPaths = new[] { "publicBin", "privateBin" };

                var plugin = registry.RegisterPlugin(new PluginRegistration("plugin2Id", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\"))
                {
                    PluginProperties = { { "ConfigName", "Value" } },
                    TraitsProperties = { { "TraitName", "Value" } },
                    PluginHandlerFactory = handlerFactory,
                    AssemblyBindings = assemblyBindings,
                    PluginDependencies = pluginDependencies,
                    ProbingPaths = probingPaths
                });

                Assert.Multiple(() =>
                {
                    Assert.AreEqual("plugin2Id", plugin.PluginId);
                    Assert.AreEqual(new TypeName("Plugin, Assembly"), plugin.PluginTypeName);
                    Assert.AreEqual(@"C:\", plugin.BaseDirectory.ToString());
                    Assert.AreEqual(new PropertySet() { { "ConfigName", "Value" } }, plugin.PluginProperties);
                    Assert.AreEqual(new PropertySet() { { "TraitName", "Value" } }, plugin.TraitsProperties);
                    Assert.AreSame(handlerFactory, plugin.PluginHandlerFactory);
                    Assert.AreElementsEqual(assemblyBindings, plugin.AssemblyBindings);
                    Assert.AreElementsEqual(pluginDependencies, plugin.PluginDependencies);
                    Assert.AreElementsEqual(probingPaths, plugin.ProbingPaths);
                });
            }

            [Test]
            public void RegisterService_WhenArgumentsValid_RegistersTheServiceAndReturnsItsDescriptor()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var handlerFactory = MockRepository.GenerateStub<IHandlerFactory>();

                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName("Service, Assembly"))
                {
                    TraitsHandlerFactory = handlerFactory
                });

                Assert.Multiple(() =>
                {
                    Assert.AreSame(plugin, service.Plugin);
                    Assert.AreEqual("serviceId", service.ServiceId);
                    Assert.AreEqual(new TypeName("Service, Assembly"), service.ServiceTypeName);
                    Assert.AreSame(handlerFactory, service.TraitsHandlerFactory);
                });
            }

            [Test]
            public void RegisterComponent_WhenArgumentsValid_RegistersTheComponentAndReturnsItsDescriptor()
            {
                var registry = new Registry();
                var plugin1 = registry.RegisterPlugin(new PluginRegistration("plugin1Id", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var plugin2 = registry.RegisterPlugin(new PluginRegistration("plugin2Id", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var service = registry.RegisterService(new ServiceRegistration(plugin2, "serviceId", new TypeName("Service, Assembly")));
                var handlerFactory = MockRepository.GenerateStub<IHandlerFactory>();

                var component = registry.RegisterComponent(new ComponentRegistration(plugin2, service, "componentId", new TypeName("Component, Assembly"))
                {
                    ComponentProperties = { { "ConfigName", "Value" } },
                    TraitsProperties = { { "TraitName", "Value" } },
                    ComponentHandlerFactory = handlerFactory
                });

                Assert.Multiple(() =>
                {
                    Assert.AreSame(plugin2, component.Plugin);
                    Assert.AreSame(service, component.Service);
                    Assert.AreEqual("componentId", component.ComponentId);
                    Assert.AreEqual(new TypeName("Component, Assembly"), component.ComponentTypeName);
                    Assert.AreEqual(new PropertySet() { { "ConfigName", "Value" } }, component.ComponentProperties);
                    Assert.AreEqual(new PropertySet() { { "TraitName", "Value" } }, component.TraitsProperties);
                    Assert.AreSame(handlerFactory, component.ComponentHandlerFactory);
                });
            }

            [Test]
            public void RegisterComponent_WhenArgumentsValidButNoComponentTypeNameProvided_UsesTheDefaultComponentTypeSepcifiedByTheService()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName("Service, Assembly"))
                    {
                        DefaultComponentTypeName = new TypeName("Component, Assembly")
                    });
                var handlerFactory = MockRepository.GenerateStub<IHandlerFactory>();

                var component = registry.RegisterComponent(new ComponentRegistration(plugin, service, "componentId", null));

                Assert.Multiple(() =>
                {
                    Assert.AreSame(plugin, component.Plugin);
                    Assert.AreSame(service, component.Service);
                    Assert.AreEqual("componentId", component.ComponentId);
                    Assert.AreEqual(new TypeName("Component, Assembly"), component.ComponentTypeName);
                });
            }

            [Test]
            public void RegisterPlugin_WhenPluginIdNotUnique_Throws()
            {
                var registry = new Registry();
                registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));

                var ex = Assert.Throws<ArgumentException>(() =>
                {
                    registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                });
                Assert.Contains(ex.Message, "There is already a plugin registered with id 'pluginId'.");
            }

            [Test]
            public void RegisterPlugin_WhenPluginDependencyBelongsToDifferentRegistry_Throws()
            {
                var registry = new Registry();
                var foreignRegistry = new Registry();
                var foreignPlugin = foreignRegistry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));

                var ex = Assert.Throws<ArgumentException>(() =>
                {
                    registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\"))
                    {
                        PluginDependencies = { foreignPlugin }
                    });
                });
                Assert.Contains(ex.Message, "One of the plugin dependencies does not belong to this registry.");
            }

            [Test]
            public void RegisterPlugin_WhenPluginDependencyListNotClosed_AutomaticallyAddsIndirectDependency()
            {
                var registry = new Registry();
                var plugin1 = registry.RegisterPlugin(new PluginRegistration("plugin1Id", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var plugin2 = registry.RegisterPlugin(new PluginRegistration("plugin2Id", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\"))
                    {
                        PluginDependencies = { plugin1 }
                    });

                var plugin3 = registry.RegisterPlugin(new PluginRegistration("plugin3Id", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\"))
                    {
                        PluginDependencies = { plugin2 }
                    });


                Assert.AreElementsEqual(new[] { plugin2, plugin1 }, plugin3.PluginDependencies);
            }

            [Test]
            public void RegisterService_WhenPluginBelongsToDifferentRegistry_Throws()
            {
                var registry = new Registry();
                var foreignRegistry = new Registry();
                var foreignPlugin = foreignRegistry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));

                var ex = Assert.Throws<ArgumentException>(() =>
                {
                    registry.RegisterService(new ServiceRegistration(foreignPlugin, "serviceId", new TypeName("Service, Assembly")));
                });
                Assert.Contains(ex.Message, "The specified plugin descriptor does not belong to this registry.");
            }

            [Test]
            public void RegisterService_WhenServiceIdNotUnique_Throws()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName("Service, Assembly")));

                var ex = Assert.Throws<ArgumentException>(() =>
                {
                    registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName("Service, Assembly"))); // same service id
                });
                Assert.Contains(ex.Message, "There is already a service registered with id 'serviceId'.");
            }

            [Test]
            public void RegisterService_WhenServiceTypeNameNotUnique_Throws()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName("Service, Assembly")));

                var ex = Assert.Throws<ArgumentException>(() =>
                {
                    registry.RegisterService(new ServiceRegistration(plugin, "differentServiceId", new TypeName("Service, Assembly"))); // same service type
                });
                Assert.Contains(ex.Message, "There is already a service registered with type name 'Service, Assembly'.  This service has id 'differentServiceId' and the other service has id 'serviceId'.");
            }

            [Test]
            public void RegisterComponent_WhenPluginBelongsToDifferentRegistry_Throws()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName("Service, Assembly")));
                var foreignRegistry = new Registry();
                var foreignPlugin = foreignRegistry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));

                var ex = Assert.Throws<ArgumentException>(() =>
                {
                    registry.RegisterComponent(new ComponentRegistration(foreignPlugin, service, "componentId", new TypeName("Component, Assembly")));
                });
                Assert.Contains(ex.Message, "The specified plugin descriptor does not belong to this registry.");
            }

            [Test]
            public void RegisterComponent_WhenServiceBelongsToDifferentRegistry_Throws()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var foreignRegistry = new Registry();
                var foreignPlugin = foreignRegistry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var foreignService = foreignRegistry.RegisterService(new ServiceRegistration(foreignPlugin, "serviceId", new TypeName("Service, Assembly")));

                var ex = Assert.Throws<ArgumentException>(() =>
                {
                    registry.RegisterComponent(new ComponentRegistration(plugin, foreignService, "componentId", new TypeName("Component, Assembly")));
                });
                Assert.Contains(ex.Message, "The specified service descriptor does not belong to this registry.");
            }

            [Test]
            public void RegisterComponent_WhenServiceBelongsToPluginNotIndicatedAsAPluginDependency_Throws()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var foreignPlugin = registry.RegisterPlugin(new PluginRegistration("foreignPluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var foreignService = registry.RegisterService(new ServiceRegistration(foreignPlugin, "foreignServiceId", new TypeName("Service, Assembly")));

                var ex = Assert.Throws<ArgumentException>(() =>
                {
                    registry.RegisterComponent(new ComponentRegistration(plugin, foreignService, "componentId", new TypeName("Component, Assembly")));
                });
                Assert.Contains(ex.Message, "The service belongs to a plugin that was not declared as a dependency of the plugin that provides this component.  Plugin 'pluginId' should declare a dependency on plugin 'foreignPluginId'.");
            }

            [Test]
            public void RegisterComponent_WhenComponentIdNotUnique_Throws()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName("Service, Assembly")));
                registry.RegisterComponent(new ComponentRegistration(plugin, service, "componentId", new TypeName("Component, Assembly")));

                var ex = Assert.Throws<ArgumentException>(() =>
                {
                    registry.RegisterComponent(new ComponentRegistration(plugin, service, "componentId", new TypeName("Component, Assembly")));
                });
                Assert.Contains(ex.Message, "There is already a component registered with id 'componentId'.");
            }

            [Test]
            public void RegisterComponent_WhenComponentMissingComponentTypeAndTheServiceDoesNotHaveADefaultComponentType_Throws()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName("Service, Assembly")));

                var ex = Assert.Throws<ArgumentException>(() =>
                {
                    registry.RegisterComponent(new ComponentRegistration(plugin, service, "componentId", null));
                });
                Assert.Contains(ex.Message, "The specified service descriptor does not have a default component type name so the component registration must specify a component type name but it does not.");
            }
        }

        public class PluginLookup
        {
            [Test]
            public void Indexer_WhenPluginIdIsNull_Throws()
            {
                var registry = new Registry();

                Assert.Throws<ArgumentNullException>(() => { var x = registry.Plugins[null]; });
            }

            [Test]
            public void Indexer_WhenPluginIdIsRegistered_ReturnsPlugin()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));

                var result = registry.Plugins["pluginId"];

                Assert.AreSame(plugin, result);
            }

            [Test]
            public void Indexer_WhenPluginIdIsNotRegistered_ReturnsNull()
            {
                var registry = new Registry();

                var result = registry.Plugins["pluginId"];

                Assert.IsNull(result);
            }
        }

        public class ServiceLookup
        {
            [Test]
            public void Indexer_WhenServiceIdIsNull_Throws()
            {
                var registry = new Registry();

                Assert.Throws<ArgumentNullException>(() => { var x = registry.Services[null]; });
            }

            [Test]
            public void Indexer_WhenServiceIdIsRegistered_ReturnsService()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName("Service, Assembly")));

                var result = registry.Services["serviceId"];

                Assert.AreSame(service, result);
            }

            [Test]
            public void Indexer_WhenServiceIdIsNotRegistered_ReturnsNull()
            {
                var registry = new Registry();

                var result = registry.Services["serviceId"];

                Assert.IsNull(result);
            }

            [Test]
            public void GetByServiceType_WhenServiceTypeIsNull_Throws()
            {
                var registry = new Registry();

                Assert.Throws<ArgumentNullException>(() => registry.Services.GetByServiceType(null));
            }

            [Test]
            public void GetByServiceType_WhenServiceTypeIsRegistered_ReturnsService()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName(typeof(DummyService))));

                var result = registry.Services.GetByServiceType(typeof(DummyService));

                Assert.AreSame(service, result);
            }

            [Test]
            public void GetByServiceType_WhenServiceTypeIsNotRegistered_ReturnsNull()
            {
                var registry = new Registry();

                var result = registry.Services.GetByServiceType(typeof(DummyService));

                Assert.IsNull(result);
            }

            [Test]
            public void GetByServiceTypeName_WhenServiceTypeNameIsNull_Throws()
            {
                var registry = new Registry();

                Assert.Throws<ArgumentNullException>(() => registry.Services.GetByServiceTypeName(null));
            }

            [Test]
            public void GetByServiceTypeName_WhenServiceTypeNameIsRegistered_ReturnsService()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName(typeof(DummyService))));

                var result = registry.Services.GetByServiceTypeName(new TypeName(typeof(DummyService)));

                Assert.AreSame(service, result);
            }

            [Test]
            public void GetByServiceTypeName_WhenServiceTypeNameIsNotRegistered_ReturnsNull()
            {
                var registry = new Registry();

                var result = registry.Services.GetByServiceTypeName(new TypeName(typeof(DummyService)));

                Assert.IsNull(result);
            }
        }

        public class ComponentLookup
        {
            [Test]
            public void Indexer_WhenComponentIdIsNull_Throws()
            {
                var registry = new Registry();

                Assert.Throws<ArgumentNullException>(() => { var x = registry.Components[null]; });
            }

            [Test]
            public void Indexer_WhenComponentIdIsRegistered_ReturnsComponent()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName("Service, Assembly")));
                var component = registry.RegisterComponent(new ComponentRegistration(plugin, service, "componentId", new TypeName("Component, Assembly")));

                var result = registry.Components["componentId"];

                Assert.AreSame(component, result);
            }

            [Test]
            public void Indexer_WhenComponentIdIsNotRegistered_ReturnsNull()
            {
                var registry = new Registry();

                var result = registry.Components["componentId"];

                Assert.IsNull(result);
            }

            [Test]
            public void FindByServiceId_WhenServiceIdIsNull_Throws()
            {
                var registry = new Registry();

                Assert.Throws<ArgumentNullException>(() => registry.Components.FindByServiceId(null));
            }

            [Test]
            public void FindByServiceId_WhenServiceIdNotRegistered_ReturnsEmptyList()
            {
                var registry = new Registry();

                var result = registry.Components.FindByServiceId("serviceId");

                Assert.IsEmpty(result);
            }

            [Test]
            public void FindByServiceId_WhenComponentsRegistered_ReturnsComponentList()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName("Service, Assembly")));
                var component1 = registry.RegisterComponent(new ComponentRegistration(plugin, service, "component1Id", new TypeName("Component, Assembly")));
                var component2 = registry.RegisterComponent(new ComponentRegistration(plugin, service, "component2Id", new TypeName("Component, Assembly")));

                var result = registry.Components.FindByServiceId("serviceId");

                Assert.AreElementsEqualIgnoringOrder(new[] { component1, component2 }, result);
            }

            [Test]
            public void FindByServiceId_WhenComponentsNotRegistered_ReturnsEmptyList()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName("Service, Assembly")));

                var result = registry.Components.FindByServiceId("serviceId");

                Assert.IsEmpty(result);
            }

            [Test]
            public void FindByServiceType_WhenServiceTypeIsNull_Throws()
            {
                var registry = new Registry();

                Assert.Throws<ArgumentNullException>(() => registry.Components.FindByServiceType(null));
            }

            [Test]
            public void FindByServiceType_WhenServiceTypeNotRegistered_ReturnsEmptyList()
            {
                var registry = new Registry();

                var result = registry.Components.FindByServiceType(typeof(DummyService));

                Assert.IsEmpty(result);
            }

            [Test]
            public void FindByServiceType_WhenComponentsRegistered_ReturnsComponentList()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName(typeof(DummyService))));
                var component1 = registry.RegisterComponent(new ComponentRegistration(plugin, service, "component1Id", new TypeName("Component, Assembly")));
                var component2 = registry.RegisterComponent(new ComponentRegistration(plugin, service, "component2Id", new TypeName("Component, Assembly")));

                var result = registry.Components.FindByServiceType(typeof(DummyService));

                Assert.AreElementsEqualIgnoringOrder(new[] { component1, component2 }, result);
            }

            [Test]
            public void FindByServiceType_WhenComponentsNotRegistered_ReturnsEmptyList()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName(typeof(DummyService))));

                var result = registry.Components.FindByServiceType(typeof(DummyService));

                Assert.IsEmpty(result);
            }

            [Test]
            public void FindByServiceTypeName_WhenServiceTypeNameIsNull_Throws()
            {
                var registry = new Registry();

                Assert.Throws<ArgumentNullException>(() => registry.Components.FindByServiceTypeName(null));
            }

            [Test]
            public void FindByServiceTypeName_WhenServiceTypeNameNotRegistered_ReturnsEmptyList()
            {
                var registry = new Registry();

                var result = registry.Components.FindByServiceTypeName(new TypeName(typeof(DummyService)));

                Assert.IsEmpty(result);
            }

            [Test]
            public void FindByServiceTypeName_WhenComponentsRegistered_ReturnsComponentList()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName(typeof(DummyService))));
                var component1 = registry.RegisterComponent(new ComponentRegistration(plugin, service, "component1Id", new TypeName("Component1, Assembly")));
                var component2 = registry.RegisterComponent(new ComponentRegistration(plugin, service, "component2Id", new TypeName("Component2, Assembly")));

                var result = registry.Components.FindByServiceTypeName(new TypeName(typeof(DummyService)));

                Assert.AreElementsEqualIgnoringOrder(new[] { component1, component2 }, result);
            }

            [Test]
            public void FindByServiceTypeName_WhenComponentsNotRegistered_ReturnsEmptyList()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName(typeof(DummyService))));

                var result = registry.Components.FindByServiceTypeName(new TypeName(typeof(DummyService)));

                Assert.IsEmpty(result);
            }
        }

        public class ServiceLocation
        {
            [Test]
            public void ServiceLocator_Always_ReturnsARegistryServiceLocatorForTheRegistry()
            {
                var registry = new Registry();

                IServiceLocator serviceLocator = registry.ServiceLocator;

                Assert.Multiple(() =>
                {
                    Assert.IsInstanceOfType<RegistryServiceLocator>(serviceLocator);
                    Assert.AreSame(registry, ((RegistryServiceLocator)serviceLocator).Registry);
                });
            }
        }

        public class PluginDescriptorSearchPaths
        {
            [Test]
            public void GetSearchPaths_WhenResourcePathIsEmpty_Throws()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("InvalidPlugin, Assembly"), new DirectoryInfo(@"C:\")));

                Assert.Throws<ArgumentException>(() => plugin.GetSearchPaths(""));
            }

            [Test]
            public void GetSearchPaths_WhenResourcePathIsNull_ReturnsBinAndProbingPaths()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("InvalidPlugin, Assembly"), new DirectoryInfo(@"C:\"))
                    {
                        ProbingPaths = { "ProbeA", "ProbeB" }
                    });

                IList<string> result = plugin.GetSearchPaths(null).ToList();

                Assert.AreElementsEqual(new[]
                {
                    @"C:\",
                    @"C:\bin",
                    @"C:\ProbeA",
                    @"C:\bin\ProbeA",
                    @"C:\ProbeB",
                    @"C:\bin\ProbeB"
                }, result);
            }

            [Test]
            public void GetSearchPaths_WhenResourcePathIsRelative_ReturnsBinAndProbingPathsWithResourcePathAppended()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("InvalidPlugin, Assembly"), new DirectoryInfo(@"C:\"))
                {
                    ProbingPaths = { "ProbeA", "ProbeB" }
                });

                IList<string> result = plugin.GetSearchPaths("resource.txt").ToList();

                Assert.AreElementsEqual(new[]
                {
                    @"C:\resource.txt",
                    @"C:\bin\resource.txt",
                    @"C:\ProbeA\resource.txt",
                    @"C:\bin\ProbeA\resource.txt",
                    @"C:\ProbeB\resource.txt",
                    @"C:\bin\ProbeB\resource.txt"
                }, result);
            }

            [Test]
            public void GetSearchPaths_WhenResourcePathIsAbsolute_ReturnsResourcePathItself()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("InvalidPlugin, Assembly"), new DirectoryInfo(@"C:\")));

                IList<string> result = plugin.GetSearchPaths(@"C:\SomePath\resource.txt").ToList();

                Assert.AreElementsEqual(new[]
                {
                    @"C:\SomePath\resource.txt"
                }, result);
            }
        }

        public class PluginDescriptorResolution
        {
            [Test]
            public void ResolvePluginType_WhenTypeIsInvalid_Throws()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("InvalidPlugin, Assembly"), new DirectoryInfo(@"C:\")));

                var ex = Assert.Throws<RuntimeException>(() => plugin.ResolvePluginType());
                Assert.Contains(ex.Message, "Could not resolve the plugin type of plugin 'pluginId'.");
            }

            [Test]
            public void ResolvePluginType_WhenTypeIsValid_ReturnsType()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName(typeof(DummyPlugin)), new DirectoryInfo(@"C:\")));

                var pluginType = plugin.ResolvePluginType();

                Assert.AreEqual(typeof(DummyPlugin), pluginType);
            }

            [Test]
            public void ResolvePluginHandler_WhenHandlerFactoryFails_Throws()
            {
                var registry = new Registry();
                var handlerFactory = MockRepository.GenerateMock<IHandlerFactory>();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName(typeof(DummyPlugin)), new DirectoryInfo(@"C:\"))
                {
                    PluginHandlerFactory = handlerFactory,
                    PluginProperties = { { "Name", "Value" } }
                });

                handlerFactory.Expect(x => x.CreateHandler(registry.ServiceLocator, registry.ResourceLocator,
                    typeof(IPlugin), typeof(DummyPlugin), new PropertySet() { { "Name", "Value" } }))
                    .Throw(new Exception("Boom"));

                var ex = Assert.Throws<RuntimeException>(() => plugin.ResolvePluginHandler());
                Assert.Contains(ex.Message, "Could not resolve the plugin handler of plugin 'pluginId'.");
                handlerFactory.VerifyAllExpectations();
            }

            [Test]
            public void ResolvePluginHandler_WhenHandlerFactorySucceeds_ReturnsHandler()
            {
                var registry = new Registry();
                var handlerFactory = MockRepository.GenerateMock<IHandlerFactory>();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName(typeof(DummyPlugin)), new DirectoryInfo(@"C:\"))
                {
                    PluginHandlerFactory = handlerFactory,
                    PluginProperties = { { "Name", "Value" } }
                });
                var handler = MockRepository.GenerateStub<IHandler>();

                handlerFactory.Expect(x => x.CreateHandler(registry.ServiceLocator, registry.ResourceLocator,
                    typeof(IPlugin), typeof(DummyPlugin), new PropertySet() { { "Name", "Value" } }))
                    .Return(handler);

                var result = plugin.ResolvePluginHandler();

                Assert.AreSame(handler, result);
                handlerFactory.VerifyAllExpectations();
            }

            [Test]
            public void ResolveTraitsHandler_WhenHandlerFactoryFails_Throws()
            {
                var registry = new Registry();
                var handlerFactory = MockRepository.GenerateMock<IHandlerFactory>();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName(typeof(DummyPlugin)), new DirectoryInfo(@"C:\"))
                {
                    TraitsProperties = { { "Name", "Value" } }
                });

                handlerFactory.Expect(x => x.CreateHandler(registry.ServiceLocator, registry.ResourceLocator,
                    typeof(PluginTraits), typeof(PluginTraits), new PropertySet() { { "Name", "Value" } }))
                    .Throw(new Exception("Boom"));

                PluginDescriptor.RunWithInjectedTraitsHandlerFactoryMock(handlerFactory, () =>
                {
                    var ex = Assert.Throws<RuntimeException>(() => plugin.ResolveTraitsHandler());
                    Assert.Contains(ex.Message, "Could not resolve the traits handler of plugin 'pluginId'.");
                });

                handlerFactory.VerifyAllExpectations();
            }

            [Test]
            public void ResolveTraitsHandler_WhenHandlerFactorySucceeds_ReturnsHandler()
            {
                var registry = new Registry();
                var handlerFactory = MockRepository.GenerateMock<IHandlerFactory>();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName(typeof(DummyPlugin)), new DirectoryInfo(@"C:\"))
                {
                    TraitsProperties = { { "Name", "Value" } }
                });
                var handler = MockRepository.GenerateStub<IHandler>();

                handlerFactory.Expect(x => x.CreateHandler(registry.ServiceLocator, registry.ResourceLocator,
                    typeof(PluginTraits), typeof(PluginTraits), new PropertySet() { { "Name", "Value" } }))
                    .Return(handler);

                PluginDescriptor.RunWithInjectedTraitsHandlerFactoryMock(handlerFactory, () =>
                {
                    var result = plugin.ResolveTraitsHandler();

                    Assert.AreSame(handler, result);
                });

                handlerFactory.VerifyAllExpectations();
            }

            [Test]
            public void ResolvePlugin_WhenWellFormed_ReturnsPluginObject()
            {
                var registry = new Registry();
                var handlerFactory = MockRepository.GenerateMock<IHandlerFactory>();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName(typeof(DummyPlugin)), new DirectoryInfo(@"C:\"))
                {
                    PluginHandlerFactory = handlerFactory,
                    PluginProperties = { { "Name", "Value" } }
                });
                var handler = MockRepository.GenerateMock<IHandler>();
                var pluginInstance = new DummyPlugin();

                handlerFactory.Expect(x => x.CreateHandler(registry.ServiceLocator, registry.ResourceLocator,
                    typeof(IPlugin), typeof(DummyPlugin), new PropertySet() { { "Name", "Value" } }))
                    .Return(handler);
                handler.Expect(x => x.Activate()).Return(pluginInstance);

                var result = (DummyPlugin) plugin.ResolvePlugin();

                handlerFactory.VerifyAllExpectations();
                handler.VerifyAllExpectations();
                Assert.AreSame(pluginInstance, result);
            }

            [Test]
            public void ResolvePlugin_WhenActivationFails_Throws()
            {
                var registry = new Registry();
                var handlerFactory = MockRepository.GenerateMock<IHandlerFactory>();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName(typeof(DummyPlugin)), new DirectoryInfo(@"C:\"))
                {
                    PluginHandlerFactory = handlerFactory,
                    PluginProperties = { { "Name", "Value" } }
                });
                var handler = MockRepository.GenerateMock<IHandler>();

                handlerFactory.Expect(x => x.CreateHandler(registry.ServiceLocator, registry.ResourceLocator,
                    typeof(IPlugin), typeof(DummyPlugin), new PropertySet() { { "Name", "Value" } }))
                    .Return(handler);
                handler.Expect(x => x.Activate()).Throw(new InvalidOperationException("Boom"));

                var ex = Assert.Throws<RuntimeException>(() => plugin.ResolvePlugin());
                Assert.AreEqual("Could not resolve instance of plugin 'pluginId'.", ex.Message);
                handlerFactory.VerifyAllExpectations();
                handler.VerifyAllExpectations();
            }

            [Test]
            public void ResolvePluginTraits_WhenWellFormed_ReturnsPluginTraitsObject()
            {
                var registry = new Registry();
                var handlerFactory = MockRepository.GenerateMock<IHandlerFactory>();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName(typeof(DummyPlugin)), new DirectoryInfo(@"C:\"))
                {
                    TraitsProperties = { { "Name", "Value" } }
                });
                var handler = MockRepository.GenerateMock<IHandler>();
                var traitsInstance = new PluginTraits("name");

                handlerFactory.Expect(x => x.CreateHandler(registry.ServiceLocator, registry.ResourceLocator,
                    typeof(PluginTraits), typeof(PluginTraits), new PropertySet() { { "Name", "Value" } }))
                    .Return(handler);
                handler.Expect(x => x.Activate()).Return(traitsInstance);

                PluginDescriptor.RunWithInjectedTraitsHandlerFactoryMock(handlerFactory, () =>
                {
                    var result = plugin.ResolveTraits();

                    Assert.AreSame(traitsInstance, result);
                });

                handlerFactory.VerifyAllExpectations();
                handler.VerifyAllExpectations();
            }

            [Test]
            public void ResolvePluginTraits_WhenActivationFails_Throws()
            {
                var registry = new Registry();
                var handlerFactory = MockRepository.GenerateMock<IHandlerFactory>();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName(typeof(DummyPlugin)), new DirectoryInfo(@"C:\"))
                {
                    TraitsProperties = { { "Name", "Value" } }
                });
                var handler = MockRepository.GenerateMock<IHandler>();

                handlerFactory.Expect(x => x.CreateHandler(registry.ServiceLocator, registry.ResourceLocator,
                    typeof(PluginTraits), typeof(PluginTraits), new PropertySet() { { "Name", "Value" } }))
                    .Return(handler);
                handler.Expect(x => x.Activate()).Throw(new InvalidOperationException("Boom"));

                PluginDescriptor.RunWithInjectedTraitsHandlerFactoryMock(handlerFactory, () =>
                {
                    var ex = Assert.Throws<RuntimeException>(() => plugin.ResolveTraits());
                    Assert.AreEqual("Could not resolve traits of plugin 'pluginId'.", ex.Message);
                });

                handlerFactory.VerifyAllExpectations();
                handler.VerifyAllExpectations();
            }
        }

        public class ServiceDescriptorResolution
        {
            [Test]
            public void ResolveServiceType_WhenTypeIsInvalid_Throws()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName("InvalidService, Assembly")));

                var ex = Assert.Throws<RuntimeException>(() => service.ResolveServiceType());
                Assert.Contains(ex.Message, "Could not resolve the service type of service 'serviceId'.");
            }

            [Test]
            public void ResolveServiceType_WhenTypeIsValid_ReturnsType()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName(typeof(DummyService))));

                var serviceType = service.ResolveServiceType();

                Assert.AreEqual(typeof(DummyService), serviceType);
            }

            [Test]
            public void ResolveTraitsType_WhenServiceTypeIsInvalid_Throws()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName("InvalidService, Assembly")));

                var ex = Assert.Throws<RuntimeException>(() => service.ResolveTraitsType());
                Assert.Contains(ex.Message, "Could not resolve the service type of service 'serviceId'.");
            }

            [Test]
            public void ResolveTraitsType_WhenServiceTypeHasTraitsAttribute_ReturnsIndicatedTraitsType()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName(typeof(DummyServiceWithTraits))));

                var traitsType = service.ResolveTraitsType();

                Assert.AreEqual(typeof(DummyTraits), traitsType);
            }

            [Test]
            public void ResolveTraitsType_WhenServiceTypeDoesNotHaveTraitsAttribute_ReturnsDefaultTraitsType()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName(typeof(DummyService))));

                var traitsType = service.ResolveTraitsType();

                Assert.AreEqual(typeof(Traits), traitsType);
            }

            [Test]
            public void ResolveTraitsType_WhenServiceTypeHasInvalidTraitsAttribute_Throws()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName(typeof(DummyServiceWithInvalidTraits))));

                var ex = Assert.Throws<RuntimeException>(() => service.ResolveTraitsType());
                Assert.Contains(ex.Message, "Could not resolve the traits type of service 'serviceId'.");
            }
        }

        public class ComponentDescriptorResolution
        {
            [Test]
            public void ResolveComponenType_WhenTypeIsInvalid_Throws()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName("Service, Assembly")));
                var component = registry.RegisterComponent(new ComponentRegistration(plugin, service, "componentId", new TypeName("InvalidComponent, Assembly")));

                var ex = Assert.Throws<RuntimeException>(() => component.ResolveComponentType());
                Assert.Contains(ex.Message, "Could not resolve the component type of component 'componentId'.");
            }

            [Test]
            public void ResolveComponentType_WhenTypeIsValid_ReturnsType()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName("Service, Assembly")));
                var component = registry.RegisterComponent(new ComponentRegistration(plugin, service, "componentId", new TypeName(typeof(DummyComponent))));

                var componentType = component.ResolveComponentType();

                Assert.AreEqual(typeof(DummyComponent), componentType);
            }

            [Test]
            public void ResolveComponentHandler_WhenHandlerFactoryFails_Throws()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName(typeof(DummyService))));
                var handlerFactory = MockRepository.GenerateMock<IHandlerFactory>();
                var component = registry.RegisterComponent(new ComponentRegistration(plugin, service, "componentId", new TypeName(typeof(DummyComponent)))
                {
                    ComponentHandlerFactory = handlerFactory,
                    ComponentProperties = { { "Name", "Value" } }
                });

                handlerFactory.Expect(x => x.CreateHandler(registry.ServiceLocator, registry.ResourceLocator,
                    typeof(DummyService), typeof(DummyComponent), new PropertySet() { { "Name", "Value" } }))
                    .Throw(new Exception("Boom"));

                var ex = Assert.Throws<RuntimeException>(() => component.ResolveComponentHandler());
                Assert.Contains(ex.Message, "Could not resolve the component handler of component 'componentId'.");
                handlerFactory.VerifyAllExpectations();
            }

            [Test]
            public void ResolveComponentHandler_WhenHandlerFactorySucceeds_ReturnsHandler()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName(typeof(DummyService))));
                var handlerFactory = MockRepository.GenerateMock<IHandlerFactory>();
                var component = registry.RegisterComponent(new ComponentRegistration(plugin, service, "componentId", new TypeName(typeof(DummyComponent)))
                {
                    ComponentHandlerFactory = handlerFactory,
                    ComponentProperties = { { "Name", "Value" } }
                });
                var handler = MockRepository.GenerateStub<IHandler>();

                handlerFactory.Expect(x => x.CreateHandler(registry.ServiceLocator, registry.ResourceLocator,
                    typeof(DummyService), typeof(DummyComponent), new PropertySet() { { "Name", "Value" } }))
                    .Return(handler);

                var result = component.ResolveComponentHandler();

                Assert.AreSame(handler, result);
                handlerFactory.VerifyAllExpectations();
            }

            [Test]
            public void ResolveTraitsHandler_WhenHandlerFactoryFails_Throws()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var handlerFactory = MockRepository.GenerateMock<IHandlerFactory>();
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName(typeof(DummyServiceWithTraits)))
                {
                    TraitsHandlerFactory = handlerFactory,
                });
                var component = registry.RegisterComponent(new ComponentRegistration(plugin, service, "componentId", new TypeName(typeof(DummyComponent)))
                {
                    TraitsProperties = { { "Name", "Value" } }
                });

                handlerFactory.Expect(x => x.CreateHandler(registry.ServiceLocator, registry.ResourceLocator,
                    typeof(Traits), typeof(DummyTraits), new PropertySet() { { "Name", "Value" } }))
                    .Throw(new Exception("Boom"));

                var ex = Assert.Throws<RuntimeException>(() => component.ResolveTraitsHandler());
                Assert.Contains(ex.Message, "Could not resolve the traits handler of component 'componentId'.");
                handlerFactory.VerifyAllExpectations();
            }

            [Test]
            public void ResolveTraitsHandler_WhenHandlerFactorySucceeds_ReturnsHandler()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var handlerFactory = MockRepository.GenerateMock<IHandlerFactory>();
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName(typeof(DummyServiceWithTraits)))
                {
                    TraitsHandlerFactory = handlerFactory,
                });
                var component = registry.RegisterComponent(new ComponentRegistration(plugin, service, "componentId", new TypeName(typeof(DummyComponent)))
                {
                    TraitsProperties = { { "Name", "Value" } }
                });
                var handler = MockRepository.GenerateStub<IHandler>();

                handlerFactory.Expect(x => x.CreateHandler(registry.ServiceLocator, registry.ResourceLocator,
                    typeof(Traits), typeof(DummyTraits), new PropertySet() { { "Name", "Value" } }))
                    .Return(handler);

                var result = component.ResolveTraitsHandler();

                Assert.AreSame(handler, result);
                handlerFactory.VerifyAllExpectations();
            }

            [Test]
            public void ResolveComponent_WhenWellFormed_ReturnsComponentObject()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName(typeof(DummyService))));
                var handlerFactory = MockRepository.GenerateMock<IHandlerFactory>();
                var component = registry.RegisterComponent(new ComponentRegistration(plugin, service, "componentId", new TypeName(typeof(DummyComponent)))
                {
                    ComponentHandlerFactory = handlerFactory,
                    ComponentProperties = { { "Name", "Value" } }
                });
                var handler = MockRepository.GenerateMock<IHandler>();
                var componentInstance = new DummyComponent();

                handlerFactory.Expect(x => x.CreateHandler(registry.ServiceLocator, registry.ResourceLocator,
                    typeof(DummyService), typeof(DummyComponent), new PropertySet() { { "Name", "Value" } }))
                    .Return(handler);
                handler.Expect(x => x.Activate()).Return(componentInstance);

                var result = (DummyComponent) component.ResolveComponent();

                Assert.AreSame(componentInstance, result);
                handlerFactory.VerifyAllExpectations();
                handler.VerifyAllExpectations();
            }

            [Test]
            public void ResolveComponent_WhenActivationFails_Throws()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName(typeof(DummyService))));
                var handlerFactory = MockRepository.GenerateMock<IHandlerFactory>();
                var component = registry.RegisterComponent(new ComponentRegistration(plugin, service, "componentId", new TypeName(typeof(DummyComponent)))
                {
                    ComponentHandlerFactory = handlerFactory,
                    ComponentProperties = { { "Name", "Value" } }
                });
                var handler = MockRepository.GenerateMock<IHandler>();

                handlerFactory.Expect(x => x.CreateHandler(registry.ServiceLocator, registry.ResourceLocator,
                    typeof(DummyService), typeof(DummyComponent), new PropertySet() { { "Name", "Value" } }))
                    .Return(handler);
                handler.Expect(x => x.Activate()).Throw(new InvalidOperationException("Boom"));

                var ex = Assert.Throws<RuntimeException>(() => component.ResolveComponent());
                Assert.AreEqual("Could not resolve instance of component 'componentId'.", ex.Message);
                handlerFactory.VerifyAllExpectations();
                handler.VerifyAllExpectations();
            }

            [Test]
            public void ResolveComponentTraits_WhenWellFormed_ReturnsComponentTraitsObject()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var handlerFactory = MockRepository.GenerateMock<IHandlerFactory>();
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName(typeof(DummyServiceWithTraits)))
                {
                    TraitsHandlerFactory = handlerFactory,
                });
                var component = registry.RegisterComponent(new ComponentRegistration(plugin, service, "componentId", new TypeName(typeof(DummyComponent)))
                {
                    TraitsProperties = { { "Name", "Value" } }
                });
                var handler = MockRepository.GenerateMock<IHandler>();
                var traitsInstance = new DummyTraits();

                handlerFactory.Expect(x => x.CreateHandler(registry.ServiceLocator, registry.ResourceLocator,
                    typeof(Traits), typeof(DummyTraits), new PropertySet() { { "Name", "Value" } }))
                    .Return(handler);
                handler.Expect(x => x.Activate()).Return(traitsInstance);

                var result = (DummyTraits)component.ResolveTraits();

                Assert.AreSame(traitsInstance, result);
                handlerFactory.VerifyAllExpectations();
                handler.VerifyAllExpectations();
            }

            [Test]
            public void ResolveComponentTraits_WhenActivationFails_Throws()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var handlerFactory = MockRepository.GenerateMock<IHandlerFactory>();
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName(typeof(DummyServiceWithTraits)))
                {
                    TraitsHandlerFactory = handlerFactory,
                });
                var component = registry.RegisterComponent(new ComponentRegistration(plugin, service, "componentId", new TypeName(typeof(DummyComponent)))
                {
                    TraitsProperties = { { "Name", "Value" } }
                });
                var handler = MockRepository.GenerateMock<IHandler>();

                handlerFactory.Expect(x => x.CreateHandler(registry.ServiceLocator, registry.ResourceLocator,
                    typeof(Traits), typeof(DummyTraits), new PropertySet() { { "Name", "Value" } }))
                    .Return(handler);
                handler.Expect(x => x.Activate()).Throw(new InvalidOperationException("Boom"));

                var ex = Assert.Throws<RuntimeException>(() => component.ResolveTraits());
                Assert.AreEqual("Could not resolve traits of component 'componentId'.", ex.Message);
                handlerFactory.VerifyAllExpectations();
                handler.VerifyAllExpectations();
            }
        }

        public class DisabledPlugins
        {
            [Test]
            public void Disable_WhenReasonIsNull_Throws()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));

                Assert.Throws<ArgumentNullException>(() => plugin.Disable(null));
            }

            [Test]
            public void Disable_WhenAlreadyDisabled_OverwritesTheReason()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                plugin.Disable("Old Reason");

                plugin.Disable("New Reason");

                Assert.AreEqual("New Reason", plugin.DisabledReason);
            }

            [Test]
            public void IsDisabled_WhenPluginNotDisabled_ReturnsFalse()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));

                Assert.IsFalse(plugin.IsDisabled);
            }

            [Test]
            public void IsDisabled_WhenPluginDisabledDirectly_ReturnsTrue()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));

                plugin.Disable("The reason");

                Assert.IsTrue(plugin.IsDisabled);
            }

            [Test]
            public void IsDisabled_WhenPluginDisabledIndirectlyViaDependency_ReturnsTrue()
            {
                var registry = new Registry();
                var plugin1 = registry.RegisterPlugin(new PluginRegistration("plugin1Id", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var plugin2 = registry.RegisterPlugin(new PluginRegistration("plugin2Id", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\"))
                    {
                        PluginDependencies = { plugin1 }
                    });

                plugin1.Disable("The reason");

                Assert.IsTrue(plugin2.IsDisabled);
            }

            [Test]
            public void DisabledReason_WhenPluginNotDisabled_Throws()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));

                var ex = Assert.Throws<InvalidOperationException>(() => { var x = plugin.DisabledReason; });
                Assert.AreEqual("The plugin has not been disabled.", ex.Message);
            }

            [Test]
            public void DisabledReason_WhenPluginDisabled_ReturnsTheReason()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));

                plugin.Disable("The reason");

                Assert.AreEqual("The reason", plugin.DisabledReason);
            }

            [Test]
            public void DisabledReason_WhenPluginDisabledIndirectlyViaDependency_ReturnsTheReason()
            {
                var registry = new Registry();
                var plugin1 = registry.RegisterPlugin(new PluginRegistration("plugin1Id", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var plugin2 = registry.RegisterPlugin(new PluginRegistration("plugin2Id", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\"))
                {
                    PluginDependencies = { plugin1 }
                });

                plugin1.Disable("The reason");

                Assert.AreEqual("The plugin depends on another disabled plugin.  Reason: The reason", plugin2.DisabledReason);
            }
        }

        public class DisabledServices
        {
            [Test]
            public void IsDisabled_WhenPluginNotDisabled_ReturnsFalse()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName("Service, Assembly")));

                Assert.IsFalse(service.IsDisabled);
            }

            [Test]
            public void IsDisabled_WhenPluginDisabled_ReturnsTrue()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName("Service, Assembly")));

                plugin.Disable("The reason");

                Assert.IsTrue(service.IsDisabled);
            }

            [Test]
            public void DisabledReason_WhenPluginNotDisabled_Throws()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName("Service, Assembly")));

                var ex = Assert.Throws<InvalidOperationException>(() => { var x = service.DisabledReason; });
                Assert.AreEqual("The service has not been disabled.", ex.Message);
            }

            [Test]
            public void DisabledReason_WhenPluginDisabled_ReturnsTheReason()
            {
                var registry = new Registry();
                var plugin = registry.RegisterPlugin(new PluginRegistration("pluginId", new TypeName("Plugin, Assembly"), new DirectoryInfo(@"C:\")));
                var service = registry.RegisterService(new ServiceRegistration(plugin, "serviceId", new TypeName("Service, Assembly")));

                plugin.Disable("The reason");

                Assert.AreEqual("The plugin that provides this service was disabled.  Reason: The reason", service.DisabledReason);
            }
        }

        public class DisabledComponents
        {
            [Test]
            public void IsDisabled_WhenPluginAndServiceNotDisabled_ReturnsFalse()
            {
                var registry = new Registry();
                var plugin1 = registry.RegisterPlugin(new PluginRegistration("plugin1Id", new TypeName("Plugin1, Assembly"), new DirectoryInfo(@"C:\")));
                var plugin2 = registry.RegisterPlugin(new PluginRegistration("plugin2Id", new TypeName("Plugin2, Assembly"), new DirectoryInfo(@"C:\"))
                {
                    PluginDependencies = { plugin1 }
                });
                var service = registry.RegisterService(new ServiceRegistration(plugin1, "serviceId", new TypeName("Service, Assembly")));
                var component = registry.RegisterComponent(new ComponentRegistration(plugin2, service, "componentId", new TypeName("Component, Assembly")));

                Assert.IsFalse(component.IsDisabled);
            }

            [Test]
            public void IsDisabled_WhenPluginDisabled_ReturnsTrue()
            {
                var registry = new Registry();
                var plugin1 = registry.RegisterPlugin(new PluginRegistration("plugin1Id", new TypeName("Plugin1, Assembly"), new DirectoryInfo(@"C:\")));
                var plugin2 = registry.RegisterPlugin(new PluginRegistration("plugin2Id", new TypeName("Plugin2, Assembly"), new DirectoryInfo(@"C:\"))
                {
                    PluginDependencies = { plugin1 }
                });
                var service = registry.RegisterService(new ServiceRegistration(plugin1, "serviceId", new TypeName("Service, Assembly")));
                var component = registry.RegisterComponent(new ComponentRegistration(plugin2, service, "componentId", new TypeName("Component, Assembly")));

                plugin2.Disable("The reason");

                Assert.IsTrue(component.IsDisabled);
            }

            [Test]
            public void IsDisabled_WhenServiceDisabled_ReturnsTrue()
            {
                var registry = new Registry();
                var plugin1 = registry.RegisterPlugin(new PluginRegistration("plugin1Id", new TypeName("Plugin1, Assembly"), new DirectoryInfo(@"C:\")));
                var plugin2 = registry.RegisterPlugin(new PluginRegistration("plugin2Id", new TypeName("Plugin2, Assembly"), new DirectoryInfo(@"C:\"))
                {
                    PluginDependencies = { plugin1 }
                });
                var service = registry.RegisterService(new ServiceRegistration(plugin1, "serviceId", new TypeName("Service, Assembly")));
                var component = registry.RegisterComponent(new ComponentRegistration(plugin2, service, "componentId", new TypeName("Component, Assembly")));

                plugin1.Disable("The reason");

                Assert.IsTrue(component.IsDisabled);
            }

            [Test]
            public void DisabledReason_WhenPluginAndServiceNotDisabled_Throws()
            {
                var registry = new Registry();
                var plugin1 = registry.RegisterPlugin(new PluginRegistration("plugin1Id", new TypeName("Plugin1, Assembly"), new DirectoryInfo(@"C:\")));
                var plugin2 = registry.RegisterPlugin(new PluginRegistration("plugin2Id", new TypeName("Plugin2, Assembly"), new DirectoryInfo(@"C:\"))
                    {
                        PluginDependencies = { plugin1 }
                    });
                var service = registry.RegisterService(new ServiceRegistration(plugin1, "serviceId", new TypeName("Service, Assembly")));
                var component = registry.RegisterComponent(new ComponentRegistration(plugin2, service, "componentId", new TypeName("Component, Assembly")));

                var ex = Assert.Throws<InvalidOperationException>(() => { var x = component.DisabledReason; });
                Assert.AreEqual("The component has not been disabled.", ex.Message);
            }

            [Test]
            public void DisabledReason_WhenPluginDisabled_ReturnsTheReason()
            {
                var registry = new Registry();
                var plugin1 = registry.RegisterPlugin(new PluginRegistration("plugin1Id", new TypeName("Plugin1, Assembly"), new DirectoryInfo(@"C:\")));
                var plugin2 = registry.RegisterPlugin(new PluginRegistration("plugin2Id", new TypeName("Plugin2, Assembly"), new DirectoryInfo(@"C:\"))
                    {
                        PluginDependencies = { plugin1 }
                    });
                var service = registry.RegisterService(new ServiceRegistration(plugin1, "serviceId", new TypeName("Service, Assembly")));
                var component = registry.RegisterComponent(new ComponentRegistration(plugin2, service, "componentId", new TypeName("Component, Assembly")));

                plugin2.Disable("The reason");

                Assert.AreEqual("The plugin that provides this component was disabled.  Reason: The reason", component.DisabledReason);
            }

            [Test]
            public void DisabledReason_WhenServiceDisabled_ReturnsTheReason()
            {
                var registry = new Registry();
                var plugin1 = registry.RegisterPlugin(new PluginRegistration("plugin1Id", new TypeName("Plugin1, Assembly"), new DirectoryInfo(@"C:\")));
                var plugin2 = registry.RegisterPlugin(new PluginRegistration("plugin2Id", new TypeName("Plugin2, Assembly"), new DirectoryInfo(@"C:\"))
                {
                    PluginDependencies = { plugin1 }
                });
                var service = registry.RegisterService(new ServiceRegistration(plugin1, "serviceId", new TypeName("Service, Assembly")));
                var component = registry.RegisterComponent(new ComponentRegistration(plugin2, service, "componentId", new TypeName("Component, Assembly")));

                plugin1.Disable("The reason");

                // Now that plugin dependencies have been implemented, we see a different message than before.
                // However if later we add a feature to disable individual services independently of a plugin
                // then we should see the following message:
                // Assert.AreEqual("The service implemented by this component was disabled.  Reason: The plugin that provides this service was disabled.  Reason: The reason", component.DisabledReason);

                Assert.AreEqual("The plugin that provides this component was disabled.  Reason: The plugin depends on another disabled plugin.  Reason: The reason", component.DisabledReason);
            }
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

        private class DummyPlugin : IPlugin
        {
        }
    }
}
