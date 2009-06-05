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
using Gallio.Schema.Plugins;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Runtime.Extensibility
{
    [TestsOn(typeof(PluginCatalog))]
    public class PluginCatalogTest
    {
        public class AddingPlugins
        {
            [Test]
            public void AddPlugin_WhenPluginIsNull_Throws()
            {
                var catalog = new PluginCatalog();
                var baseDirectory = new DirectoryInfo(@"C:\");

                Assert.Throws<ArgumentNullException>(() => catalog.AddPlugin(null, baseDirectory));
            }

            [Test]
            public void AddPlugin_WhenBaseDirectoryIsNull_Throws()
            {
                var catalog = new PluginCatalog();
                var plugin = new Plugin("pluginId");

                Assert.Throws<ArgumentNullException>(() => catalog.AddPlugin(plugin, null));
            }
        }

        public class ApplyingConfigurationToRegistry
        {
            [Test]
            public void ApplyTo_WhenRegistryIsNull_Throws()
            {
                var catalog = new PluginCatalog();

                Assert.Throws<ArgumentNullException>(() => catalog.ApplyTo(null));
            }

            [Test]
            public void ApplyTo_WhenPluginTypeIsNull_RegistersThePluginWithDefaultPluginType()
            {
                var catalog = new PluginCatalog();
                var codeBase = AssemblyUtils.GetFriendlyAssemblyCodeBase(typeof(IRuntime).Assembly);
                var plugin = new Plugin("pluginId")
                {
                    Parameters = new KeyValueTable() { PropertySet = { { "Parameter", "Value" } } },
                    Traits = new KeyValueTable() { PropertySet = { { "Trait", "Value" } } },
                    Assemblies = { new Assembly("Gallio") { CodeBase = codeBase } }
                };
                var baseDirectory = new DirectoryInfo(@"C:\");

                var registry = MockRepository.GenerateMock<IRegistry>();
                var pluginDescriptor = MockRepository.GenerateStub<IPluginDescriptor>();
                var pluginRegistrations = new List<PluginRegistration>();
                registry.Expect(x => x.RegisterPlugin(null)).Callback(Enlist(pluginRegistrations)).Return(pluginDescriptor);

                catalog.AddPlugin(plugin, baseDirectory);

                catalog.ApplyTo(registry);

                registry.VerifyAllExpectations();
                Assert.Multiple(() =>
                {
                    Assert.AreEqual("pluginId", pluginRegistrations[0].PluginId);
                    Assert.AreEqual(baseDirectory, pluginRegistrations[0].BaseDirectory);
                    Assert.AreEqual(new TypeName(typeof(DefaultPlugin)), pluginRegistrations[0].PluginTypeName);
                    Assert.AreEqual(new PropertySet() { { "Parameter", "Value" } }, pluginRegistrations[0].PluginProperties);
                    Assert.AreEqual(new PropertySet() { { "Trait", "Value" } }, pluginRegistrations[0].TraitsProperties);
                    Assert.AreEqual("Gallio", pluginRegistrations[0].AssemblyBindings[0].AssemblyName.Name);
                    Assert.AreEqual(new Uri(codeBase), pluginRegistrations[0].AssemblyBindings[0].CodeBase);
                });
            }

            [Test]
            public void ApplyTo_WhenPluginTypeIsNotNull_RegistersThePluginWithSpecifiedPluginType()
            {
                var catalog = new PluginCatalog();
                var codeBase = AssemblyUtils.GetFriendlyAssemblyCodeBase(typeof(IRuntime).Assembly);
                var plugin = new Plugin("pluginId")
                {
                    PluginType = "Plugin, Assembly",
                    Parameters = new KeyValueTable() { PropertySet = { { "Parameter", "Value" } } },
                    Traits = new KeyValueTable() { PropertySet = { { "Trait", "Value" } } },
                    Assemblies = { new Assembly("Gallio") { CodeBase = codeBase } }
                };
                var baseDirectory = new DirectoryInfo(@"C:\");

                var registry = MockRepository.GenerateMock<IRegistry>();
                var pluginDescriptor = MockRepository.GenerateStub<IPluginDescriptor>();
                var pluginRegistrations = new List<PluginRegistration>();
                registry.Expect(x => x.RegisterPlugin(null)).Callback(Enlist(pluginRegistrations)).Return(pluginDescriptor);

                catalog.AddPlugin(plugin, baseDirectory);

                catalog.ApplyTo(registry);

                registry.VerifyAllExpectations();
                Assert.Multiple(() =>
                {
                    Assert.AreEqual("pluginId", pluginRegistrations[0].PluginId);
                    Assert.AreEqual(baseDirectory, pluginRegistrations[0].BaseDirectory);
                    Assert.AreEqual(new TypeName("Plugin, Assembly"), pluginRegistrations[0].PluginTypeName);
                    Assert.AreEqual(new PropertySet() { { "Parameter", "Value" } }, pluginRegistrations[0].PluginProperties);
                    Assert.AreEqual(new PropertySet() { { "Trait", "Value" } }, pluginRegistrations[0].TraitsProperties);
                    Assert.AreEqual("Gallio", pluginRegistrations[0].AssemblyBindings[0].AssemblyName.Name);
                    Assert.AreEqual(new Uri(codeBase), pluginRegistrations[0].AssemblyBindings[0].CodeBase);
                });
            }

            [Test]
            public void ApplyTo_WhenPluginRegistrationFails_Throws()
            {
                var catalog = new PluginCatalog();
                var plugin = new Plugin("pluginId");
                var baseDirectory = new DirectoryInfo(@"C:\");

                var registry = MockRepository.GenerateMock<IRegistry>();
                registry.Expect(x => x.RegisterPlugin(null)).IgnoreArguments().Throw(new InvalidOperationException("Boom"));

                catalog.AddPlugin(plugin, baseDirectory);

                var ex = Assert.Throws<RuntimeException>(() => catalog.ApplyTo(registry));
                Assert.AreEqual("Could not register plugin 'pluginId'.", ex.Message);

                registry.VerifyAllExpectations();
            }

            [Test]
            public void ApplyTo_WhenServiceRegistrationWellFormed_RegistersTheService()
            {
                var catalog = new PluginCatalog();
                var plugin = new Plugin("pluginId")
                {
                    Services = { new Service("serviceId", "Service, Assembly")
                    {
                        DefaultComponentType = "DefaultComponent, Assembly"
                    }}
                };
                var baseDirectory = new DirectoryInfo(@"C:\");

                var registry = MockRepository.GenerateMock<IRegistry>();
                var pluginDescriptor = MockRepository.GenerateStub<IPluginDescriptor>();
                var serviceRegistrations = new List<ServiceRegistration>();
                registry.Expect(x => x.RegisterPlugin(null)).IgnoreArguments().Return(pluginDescriptor);
                registry.Expect(x => x.RegisterService(null)).Callback(Enlist(serviceRegistrations)).Return(MockRepository.GenerateStub<IServiceDescriptor>());

                catalog.AddPlugin(plugin, baseDirectory);

                catalog.ApplyTo(registry);

                registry.VerifyAllExpectations();
                Assert.Multiple(() =>
                {
                    Assert.AreSame(pluginDescriptor, serviceRegistrations[0].Plugin);
                    Assert.AreEqual("serviceId", serviceRegistrations[0].ServiceId);
                    Assert.AreEqual(new TypeName("Service, Assembly"), serviceRegistrations[0].ServiceTypeName);
                    Assert.AreEqual(new TypeName("DefaultComponent, Assembly"), serviceRegistrations[0].DefaultComponentTypeName);
                });
            }

            [Test]
            public void ApplyTo_WhenServiceRegistrationFails_Throws()
            {
                var catalog = new PluginCatalog();
                var plugin = new Plugin("pluginId")
                {
                    Services = { new Service("serviceId", "Service, Assembly") }
                };
                var baseDirectory = new DirectoryInfo(@"C:\");

                var registry = MockRepository.GenerateMock<IRegistry>();
                registry.Expect(x => x.RegisterPlugin(null)).IgnoreArguments().Return(MockRepository.GenerateStub<IPluginDescriptor>());
                registry.Expect(x => x.RegisterService(null)).IgnoreArguments().Throw(new InvalidOperationException("Boom"));

                catalog.AddPlugin(plugin, baseDirectory);

                var ex = Assert.Throws<RuntimeException>(() => catalog.ApplyTo(registry));
                Assert.AreEqual("Could not register service 'serviceId' of plugin 'pluginId'.", ex.Message);

                registry.VerifyAllExpectations();
            }

            [Test]
            public void ApplyTo_WhenComponentRegistrationWellFormed_RegistersTheComponent()
            {
                var catalog = new PluginCatalog();
                var plugin = new Plugin("pluginId")
                {
                    Components =
                {
                    new Component("componentId", "serviceId", "Component, Assembly")
                    {
                        Parameters = new KeyValueTable()
                        {
                            PropertySet = { { "Parameter", "Value" } }
                        },
                        Traits = new KeyValueTable()
                        {
                            PropertySet = { { "Trait", "Value" } }
                        }
                    }
                }
                };
                var baseDirectory = new DirectoryInfo(@"C:\");

                var registry = MockRepository.GenerateMock<IRegistry>();
                var services = MockRepository.GenerateMock<IServices>();
                var pluginDescriptor = MockRepository.GenerateStub<IPluginDescriptor>();
                var serviceDescriptor = MockRepository.GenerateStub<IServiceDescriptor>();
                var componentRegistrations = new List<ComponentRegistration>();
                registry.Stub(x => x.Services).Return(services);
                registry.Expect(x => x.RegisterPlugin(null)).IgnoreArguments().Return(pluginDescriptor);
                registry.Expect(x => x.RegisterComponent(null)).Callback(Enlist(componentRegistrations)).Return(MockRepository.GenerateStub<IComponentDescriptor>());
                services.Expect(x => x["serviceId"]).Return(serviceDescriptor);

                catalog.AddPlugin(plugin, baseDirectory);

                catalog.ApplyTo(registry);

                registry.VerifyAllExpectations();
                services.VerifyAllExpectations();
                Assert.Multiple(() =>
                {
                    Assert.AreSame(pluginDescriptor, componentRegistrations[0].Plugin);
                    Assert.AreSame(serviceDescriptor, componentRegistrations[0].Service);
                    Assert.AreEqual("componentId", componentRegistrations[0].ComponentId);
                    Assert.AreEqual(new TypeName("Component, Assembly"), componentRegistrations[0].ComponentTypeName);
                    Assert.AreEqual(new PropertySet() { { "Parameter", "Value" } }, componentRegistrations[0].ComponentProperties);
                    Assert.AreEqual(new PropertySet() { { "Trait", "Value" } }, componentRegistrations[0].TraitsProperties);
                });
            }

            [Test]
            public void ApplyTo_WhenComponentRegistrationFails_Throws()
            {
                var catalog = new PluginCatalog();
                var plugin = new Plugin("pluginId")
                {
                    Components = { new Component("componentId", "serviceId", "Component, Assembly") }
                };
                var baseDirectory = new DirectoryInfo(@"C:\");

                var registry = MockRepository.GenerateMock<IRegistry>();
                var services = MockRepository.GenerateMock<IServices>();
                registry.Stub(x => x.Services).Return(services);
                registry.Expect(x => x.RegisterPlugin(null)).IgnoreArguments().Return(MockRepository.GenerateStub<IPluginDescriptor>());
                registry.Expect(x => x.RegisterComponent(null)).IgnoreArguments().Throw(new InvalidOperationException("Boom"));
                services.Expect(x => x["serviceId"]).Return(MockRepository.GenerateStub<IServiceDescriptor>());

                catalog.AddPlugin(plugin, baseDirectory);

                var ex = Assert.Throws<RuntimeException>(() => catalog.ApplyTo(registry));
                Assert.AreEqual("Could not register component 'componentId' of plugin 'pluginId'.", ex.Message);

                registry.VerifyAllExpectations();
                services.VerifyAllExpectations();
            }

            [Test]
            public void ApplyTo_WhenComponentRefersToUnregisteredService_Throws()
            {
                var catalog = new PluginCatalog();
                var plugin = new Plugin("pluginId")
                {
                    Components = { new Component("componentId", "serviceId", "Component, Assembly") }
                };
                var baseDirectory = new DirectoryInfo(@"C:\");

                var registry = MockRepository.GenerateMock<IRegistry>();
                var services = MockRepository.GenerateMock<IServices>();
                registry.Stub(x => x.Services).Return(services);
                registry.Expect(x => x.RegisterPlugin(null)).IgnoreArguments().Return(MockRepository.GenerateStub<IPluginDescriptor>());
                services.Expect(x => x["serviceId"]).Return(null);

                catalog.AddPlugin(plugin, baseDirectory);

                var ex = Assert.Throws<RuntimeException>(() => catalog.ApplyTo(registry));
                Assert.AreEqual("Could not register component 'componentId' of plugin 'pluginId' because it implements service 'serviceId' which was not found in the registry.", ex.Message);

                registry.VerifyAllExpectations();
                services.VerifyAllExpectations();
            }

            [Test]
            public void ApplyTo_WhenWellFormed_RegistersAllPluginsThenAllServicesThenAllComponents()
            {
                var catalog = new PluginCatalog();

                catalog.AddPlugin(new Plugin("plugin1Id")
                {
                    Services =
                {
                    new Service("service1Id", "Service1, Assembly"),
                    new Service("service2Id", "Service2, Assembly")
                },
                    Components =
                {
                    new Component("component1Id", "service2Id", "Component1, Assembly"),
                    new Component("component2Id", "service3Id", "Component2, Assembly"),
                }
                }, new DirectoryInfo(@"C:\Plugin1"));

                catalog.AddPlugin(new Plugin("plugin2Id")
                {
                    Services =
                {
                    new Service("service3Id", "Service3, Assembly")
                },
                    Components =
                {
                    new Component("component3Id", "service1Id", "Component3, Assembly"),
                    new Component("component4Id", "service3Id", "Component4, Assembly"),
                }
                }, new DirectoryInfo(@"C:\Plugin2"));

                var registry = MockRepository.GenerateMock<IRegistry>();
                var pluginRegistrations = new List<PluginRegistration>();
                var serviceRegistrations = new List<ServiceRegistration>();
                var componentRegistrations = new List<ComponentRegistration>();

                var services = MockRepository.GenerateMock<IServices>();

                registry.Expect(x => x.RegisterPlugin(null)).Callback(Enlist(pluginRegistrations))
                    .Return(MockRepository.GenerateStub<IPluginDescriptor>())
                    .Repeat.Twice();
                registry.Expect(x => x.RegisterService(null)).Callback(Enlist(serviceRegistrations))
                    .Return(MockRepository.GenerateStub<IServiceDescriptor>())
                    .Repeat.Times(3);
                registry.Expect(x => x.RegisterComponent(null)).Callback(Enlist(componentRegistrations))
                    .Return(MockRepository.GenerateStub<IComponentDescriptor>())
                    .Repeat.Times(4);

                registry.Stub(x => x.Services).Return(services);
                services.Expect(x => x["service2Id"]).Return(MockRepository.GenerateStub<IServiceDescriptor>());
                services.Expect(x => x["service3Id"]).Return(MockRepository.GenerateStub<IServiceDescriptor>());
                services.Expect(x => x["service1Id"]).Return(MockRepository.GenerateStub<IServiceDescriptor>());
                services.Expect(x => x["service3Id"]).Return(MockRepository.GenerateStub<IServiceDescriptor>());

                catalog.ApplyTo(registry);

                registry.VerifyAllExpectations();
                services.VerifyAllExpectations();
                Assert.Multiple(() =>
                {
                    Assert.AreEqual("plugin1Id", pluginRegistrations[0].PluginId);
                    Assert.AreEqual("plugin2Id", pluginRegistrations[1].PluginId);

                    Assert.AreEqual("service1Id", serviceRegistrations[0].ServiceId);
                    Assert.AreEqual("service2Id", serviceRegistrations[1].ServiceId);
                    Assert.AreEqual("service3Id", serviceRegistrations[2].ServiceId);

                    Assert.AreEqual("component1Id", componentRegistrations[0].ComponentId);
                    Assert.AreEqual("component2Id", componentRegistrations[1].ComponentId);
                    Assert.AreEqual("component3Id", componentRegistrations[2].ComponentId);
                    Assert.AreEqual("component4Id", componentRegistrations[3].ComponentId);
                });
            }
        }

        public class ResolvingPluginDependencies
        {
            [Test]
            public void ApplyTo_WhenPluginDependenciesPresent_RegistersPluginsInTopologicallySortedOrder()
            {
                var catalog = new PluginCatalog();
                var plugin1 = new Plugin("plugin1Id") { Dependencies = { new Dependency("plugin3Id"), new Dependency("plugin2Id") } };
                var plugin2 = new Plugin("plugin2Id") { Dependencies = { new Dependency("plugin4Id") } };
                var plugin3 = new Plugin("plugin3Id") { Dependencies = { new Dependency("plugin4Id") } };
                var plugin4 = new Plugin("plugin4Id") { Dependencies = { } };
                var baseDirectory = new DirectoryInfo(@"C:\");

                var registry = MockRepository.GenerateMock<IRegistry>();
                var pluginRegistrations = new List<PluginRegistration>();
                var plugins = MockRepository.GenerateMock<IPlugins>();
                registry.Expect(x => x.RegisterPlugin(null)).Callback(Enlist(pluginRegistrations)).Return(MockRepository.GenerateStub<IPluginDescriptor>()).Repeat.Times(4);
                registry.Stub(x => x.Plugins).Return(plugins);
                plugins.Expect(x => x[null]).IgnoreArguments().Return(MockRepository.GenerateStub<IPluginDescriptor>());

                catalog.AddPlugin(plugin2, baseDirectory);
                catalog.AddPlugin(plugin4, baseDirectory);
                catalog.AddPlugin(plugin1, baseDirectory);
                catalog.AddPlugin(plugin3, baseDirectory);

                catalog.ApplyTo(registry);

                registry.VerifyAllExpectations();
                plugins.VerifyAllExpectations();
                Assert.AreElementsEqual(new[] { "plugin4Id", "plugin2Id", "plugin3Id", "plugin1Id" },
                    pluginRegistrations.ConvertAll(p => p.PluginId), "Plugins should appear in sorted dependency order.");
            }

            [Test]
            public void ApplyTo_WhenPluginDependencyCycleExists_Throws()
            {
                var catalog = new PluginCatalog();
                var plugin1 = new Plugin("plugin1Id") { Dependencies = { new Dependency("plugin3Id"), new Dependency("plugin2Id") } };
                var plugin2 = new Plugin("plugin2Id") { Dependencies = { new Dependency("plugin1Id") } };
                var plugin3 = new Plugin("plugin3Id") { Dependencies = { } };
                var baseDirectory = new DirectoryInfo(@"C:\");

                var registry = MockRepository.GenerateMock<IRegistry>();

                catalog.AddPlugin(plugin2, baseDirectory);
                catalog.AddPlugin(plugin1, baseDirectory);
                catalog.AddPlugin(plugin3, baseDirectory);

                var ex = Assert.Throws<RuntimeException>(() => catalog.ApplyTo(registry));
                Assert.AreEqual("Could not topologically sort the following plugins either due to dependency cycles or duplicate dependencies: 'plugin2Id', 'plugin1Id'.", ex.Message);

                registry.VerifyAllExpectations();
            }

            [Test]
            public void ApplyTo_WhenPluginDependencyCannotBeResolved_DisablesAffectedPLugin()
            {
                var catalog = new PluginCatalog();
                var plugin = new Plugin("pluginId") { Dependencies = { new Dependency("unresolvedPluginId") } };
                var baseDirectory = new DirectoryInfo(@"C:\");

                var registry = MockRepository.GenerateMock<IRegistry>();
                var pluginDescriptor = MockRepository.GenerateMock<IPluginDescriptor>();
                var pluginRegistrations = new List<PluginRegistration>();
                var plugins = MockRepository.GenerateMock<IPlugins>();
                registry.Expect(x => x.RegisterPlugin(null)).Callback(Enlist(pluginRegistrations)).Return(pluginDescriptor);
                pluginDescriptor.Expect(x => x.Disable("Could not find plugin 'unresolvedPluginId' upon which this plugin depends."));
                registry.Stub(x => x.Plugins).Return(plugins);
                plugins.Expect(x => x["unresolvedPluginId"]).Return(null);

                catalog.AddPlugin(plugin, baseDirectory);

                catalog.ApplyTo(registry);

                registry.VerifyAllExpectations();
                pluginDescriptor.VerifyAllExpectations();
                plugins.VerifyAllExpectations();
            }
        }

        public class ProbingForAssembliesAndDisablingPlugins
        {
            [Test]
            public void ApplyTo_WhenAssemblyCodeBaseIsRootedAndExists_DoesNotDisablePlugin()
            {
                var catalog = new PluginCatalog();
                var codeBase = AssemblyUtils.GetFriendlyAssemblyCodeBase(typeof(IRuntime).Assembly);
                var plugin = new Plugin("pluginId")
                {
                    Assemblies = { new Assembly("Gallio") { CodeBase = codeBase } }
                };
                var baseDirectory = new DirectoryInfo(@"C:\");

                var registry = MockRepository.GenerateMock<IRegistry>();
                var pluginDescriptor = MockRepository.GenerateMock<IPluginDescriptor>();
                var pluginRegistrations = new List<PluginRegistration>();
                registry.Expect(x => x.RegisterPlugin(null)).Callback(Enlist(pluginRegistrations)).Return(pluginDescriptor);

                catalog.AddPlugin(plugin, baseDirectory);

                catalog.ApplyTo(registry);

                registry.VerifyAllExpectations();
                pluginDescriptor.VerifyAllExpectations();
            }

            [Test]
            public void ApplyTo_WhenAssemblyCodeBaseIsRelativeToPluginBaseDirectoryAndExists_DoesNotDisablePlugin()
            {
                var catalog = new PluginCatalog();
                var plugin = new Plugin("pluginId")
                {
                    Assemblies = { new Assembly("Gallio") { CodeBase = "Gallio.dll" } }
                };
                var baseDirectory = new DirectoryInfo(Path.GetDirectoryName(AssemblyUtils.GetFriendlyAssemblyCodeBase(typeof(IRuntime).Assembly)));

                var registry = MockRepository.GenerateMock<IRegistry>();
                var pluginDescriptor = MockRepository.GenerateMock<IPluginDescriptor>();
                var pluginRegistrations = new List<PluginRegistration>();
                registry.Expect(x => x.RegisterPlugin(null)).Callback(Enlist(pluginRegistrations)).Return(pluginDescriptor);

                catalog.AddPlugin(plugin, baseDirectory);

                catalog.ApplyTo(registry);

                registry.VerifyAllExpectations();
                pluginDescriptor.VerifyAllExpectations();
            }

            [Test]
            public void ApplyTo_WhenAssemblyCodeBaseIsRelativeToProbingPathAndExists_DoesNotDisablePlugin()
            {
                var catalog = new PluginCatalog();
                var plugin = new Plugin("pluginId")
                {
                    Assemblies = { new Assembly("Gallio") { CodeBase = "Gallio.dll" } },
                    ProbingPaths = { Path.GetDirectoryName(AssemblyUtils.GetFriendlyAssemblyCodeBase(typeof(IRuntime).Assembly)) }
                };
                var baseDirectory = new DirectoryInfo(@"C:\");

                var registry = MockRepository.GenerateMock<IRegistry>();
                var pluginDescriptor = MockRepository.GenerateMock<IPluginDescriptor>();
                var pluginRegistrations = new List<PluginRegistration>();
                registry.Expect(x => x.RegisterPlugin(null)).Callback(Enlist(pluginRegistrations)).Return(pluginDescriptor);

                catalog.AddPlugin(plugin, baseDirectory);

                catalog.ApplyTo(registry);

                registry.VerifyAllExpectations();
                pluginDescriptor.VerifyAllExpectations();
            }

            [Test]
            public void ApplyTo_WhenAssemblyHasNoCodeBaseAndIsFoundInTheGAC_DoesNotDisablePlugin()
            {
                var catalog = new PluginCatalog();
                var plugin = new Plugin("pluginId")
                {
                    Assemblies = { new Assembly("System") }
                };
                var baseDirectory = new DirectoryInfo(@"C:\");

                var registry = MockRepository.GenerateMock<IRegistry>();
                var pluginDescriptor = MockRepository.GenerateMock<IPluginDescriptor>();
                var pluginRegistrations = new List<PluginRegistration>();
                registry.Expect(x => x.RegisterPlugin(null)).Callback(Enlist(pluginRegistrations)).Return(pluginDescriptor);

                catalog.AddPlugin(plugin, baseDirectory);

                catalog.ApplyTo(registry);

                registry.VerifyAllExpectations();
                pluginDescriptor.VerifyAllExpectations();
            }

            [Test]
            public void ApplyTo_WhenAssemblyHasNoCodeBaseAndIsNotInTheGAC_DisablesPlugin()
            {
                var catalog = new PluginCatalog();
                var plugin = new Plugin("pluginId")
                {
                    Assemblies = { new Assembly("ThisAssemblyDoesNotExistInTheGAC") }
                };
                var baseDirectory = new DirectoryInfo(@"C:\");

                var registry = MockRepository.GenerateMock<IRegistry>();
                var pluginDescriptor = MockRepository.GenerateMock<IPluginDescriptor>();
                var pluginRegistrations = new List<PluginRegistration>();
                registry.Expect(x => x.RegisterPlugin(null)).Callback(Enlist(pluginRegistrations)).Return(pluginDescriptor);
                pluginDescriptor.Expect(x => x.Disable("Could not find assembly 'ThisAssemblyDoesNotExistInTheGAC' in the global assembly cache."));

                catalog.AddPlugin(plugin, baseDirectory);

                catalog.ApplyTo(registry);

                registry.VerifyAllExpectations();
                pluginDescriptor.VerifyAllExpectations();
            }

            [Test]
            public void ApplyTo_WhenAssemblyHasACodeBaseButIsNotFoundByProbing_DisablesPlugin()
            {
                var catalog = new PluginCatalog();
                var plugin = new Plugin("pluginId")
                {
                    Assemblies = { new Assembly("ThisAssemblyCannotBeFoundByProbing") { CodeBase = "ThisAssemblyCannotBeFoundByProbing.dll" } },
                    ProbingPaths = { "probing" }
                };
                var baseDirectory = new DirectoryInfo(@"C:\");

                var registry = MockRepository.GenerateMock<IRegistry>();
                var pluginDescriptor = MockRepository.GenerateMock<IPluginDescriptor>();
                var pluginRegistrations = new List<PluginRegistration>();
                registry.Expect(x => x.RegisterPlugin(null)).Callback(Enlist(pluginRegistrations)).Return(pluginDescriptor);
                pluginDescriptor.Expect(x => x.Disable("Could not find assembly 'ThisAssemblyCannotBeFoundByProbing' after probing for its code base in 'C:\\ThisAssemblyCannotBeFoundByProbing.dll', 'C:\\bin\\ThisAssemblyCannotBeFoundByProbing.dll', 'C:\\probing\\ThisAssemblyCannotBeFoundByProbing.dll', 'C:\\bin\\probing\\ThisAssemblyCannotBeFoundByProbing.dll'."));

                catalog.AddPlugin(plugin, baseDirectory);

                catalog.ApplyTo(registry);

                registry.VerifyAllExpectations();
                pluginDescriptor.VerifyAllExpectations();
            }
        }

        private static Gallio.Common.Func<T, bool> Enlist<T>(ICollection<T> list)
        {
            return value =>
            {
                list.Add(value);
                return true;
            };
        }
    }
}
