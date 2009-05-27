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
using System.Linq;
using System.Text;
using Gallio.Runtime.Extensibility;
using MbUnit.Framework;
using Gallio.Common.Reflection;
using Rhino.Mocks;
using Gallio.Common.Collections;

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
        public void ComponentTypeName_Accessor_CanBeSetInConstructorAndChanged()
        {
            var plugin = MockRepository.GenerateStub<IPluginDescriptor>();
            var service = MockRepository.GenerateStub<IServiceDescriptor>();
            var registration = new ComponentRegistration(plugin, service, "componentId", new TypeName("Component, Assembly"));

            Assert.AreEqual(new TypeName("Component, Assembly"), registration.ComponentTypeName);

            registration.ComponentTypeName = null;

            Assert.IsNull(registration.ComponentTypeName);
        }

        [Test]
        public void ComponentTypeName_Accessor_CanBeNullAndChanged()
        {
            var plugin = MockRepository.GenerateStub<IPluginDescriptor>();
            var service = MockRepository.GenerateStub<IServiceDescriptor>();
            var registration = new ComponentRegistration(plugin, service, "componentId", null);

            Assert.IsNull(registration.ComponentTypeName);

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
