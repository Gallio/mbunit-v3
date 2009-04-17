using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gallio.Collections;
using Gallio.Reflection;
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
            var plugin = new Plugin("pluginId")
            {
                Parameters = { { "Parameter", "Value" } },
                Traits = { { "Trait", "Value" } }
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
                Assert.AreEqual(new TypeName(typeof(DefaultPlugin)), pluginRegistrations[0].PluginTypeName);
                Assert.AreEqual(new PropertySet() { { "Parameter", "Value" } }, pluginRegistrations[0].PluginProperties);
                Assert.AreEqual(new PropertySet() { { "Trait", "Value" } }, pluginRegistrations[0].TraitsProperties);
            });
        }

        [Test]
        public void ApplyTo_WhenPluginTypeIsNotNull_RegistersThePluginWithSpecifiedPluginType()
        {
            var catalog = new PluginCatalog();
            var plugin = new Plugin("pluginId")
            {
                PluginType = "Plugin, Assembly",
                Parameters = { { "Parameter", "Value" } },
                Traits = { { "Trait", "Value" } }
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
                Assert.AreEqual(new TypeName("Plugin, Assembly"), pluginRegistrations[0].PluginTypeName);
                Assert.AreEqual(new PropertySet() { { "Parameter", "Value" } }, pluginRegistrations[0].PluginProperties);
                Assert.AreEqual(new PropertySet() { { "Trait", "Value" } }, pluginRegistrations[0].TraitsProperties);
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
                Services = { new Service("serviceId", "Service, Assembly") }
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
                        Parameters = { { "Parameter", "Value" } },
                        Traits = { { "Trait", "Value" } }
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

        private static Func<T, bool> Enlist<T>(ICollection<T> list)
        {
            return value =>
            {
                list.Add(value);
                return true;
            };
        }
    }
}
