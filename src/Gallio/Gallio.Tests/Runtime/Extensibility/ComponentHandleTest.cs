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
using Gallio.Reflection;
using Gallio.Runtime.Extensibility;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Runtime.Extensibility
{
    [TestsOn(typeof(ComponentHandle))]
    [TestsOn(typeof(ComponentHandle<,>))]
    public class ComponentHandleTest
    {
        [Test]
        public void CreateInstance_WhenDescriptorIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => ComponentHandle.CreateInstance(null));
        }

        [Test]
        public void CreateInstanceGeneric_WhenDescriptorIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => ComponentHandle.CreateInstance<DummyService, DummyTraits>(null));
        }

        [Test]
        public void CreateInstance_WhenArgumentsValid_ReturnsTypedComponentHandle()
        {
            var componentDescriptor = CreateStubComponentDescriptor<DummyService, DummyTraits>();

            var componentHandle = ComponentHandle.CreateInstance(componentDescriptor);

            Assert.Multiple(() =>
            {
                Assert.IsInstanceOfType(typeof(ComponentHandle<DummyService, DummyTraits>), componentHandle);
                Assert.AreSame(componentDescriptor, componentHandle.Descriptor);
                Assert.AreEqual(typeof(DummyService), componentHandle.ServiceType);
                Assert.AreEqual(typeof(DummyTraits), componentHandle.TraitsType);
            });
        }

        [Test]
        public void CreateInstanceGeneric_WhenServiceTypeDoesNotMatchDescriptor_Throws()
        {
            var componentDescriptor = CreateStubComponentDescriptor<DummyService, DummyTraits>();

            var ex = Assert.Throws<ArgumentException>(() => ComponentHandle.CreateInstance<object, DummyTraits>(componentDescriptor));
            Assert.Contains(ex.Message, "The component descriptor is not compatible with the requested component handle type because it has a different service type or traits type.");
        }

        [Test]
        public void CreateInstanceGeneric_WhenTraitsTypeDoesNotMatchDescriptor_Throws()
        {
            var componentDescriptor = CreateStubComponentDescriptor<DummyService, DummyTraits>();

            var ex = Assert.Throws<ArgumentException>(() => ComponentHandle.CreateInstance<ServiceDescriptor, Traits>(componentDescriptor));
            Assert.Contains(ex.Message, "The component descriptor is not compatible with the requested component handle type because it has a different service type or traits type.");
        }

        [Test]
        public void CreateInstanceGeneric_WhenArgumentsValid_ReturnsTypedComponentHandle()
        {
            var componentDescriptor = CreateStubComponentDescriptor<DummyService, DummyTraits>();

            var componentHandle = ComponentHandle.CreateInstance<DummyService, DummyTraits>(componentDescriptor);

            Assert.Multiple(() =>
            {
                Assert.AreSame(componentDescriptor, componentHandle.Descriptor);
                Assert.AreEqual(typeof(DummyService), componentHandle.ServiceType);
                Assert.AreEqual(typeof(DummyTraits), componentHandle.TraitsType);
            });
        }

        [Test]
        public void CreateStub_WhenComponentIdIsNull_Throws()
        {
            var component = MockRepository.GenerateStub<DummyService>();
            var traits = new DummyTraits();

            Assert.Throws<ArgumentNullException>(() => ComponentHandle.CreateStub<DummyService, DummyTraits>(null, component, traits));
        }

        [Test]
        public void CreateStub_WhenComponentIsNull_Throws()
        {
            var componentId = "componentId";
            var traits = new DummyTraits();

            Assert.Throws<ArgumentNullException>(() => ComponentHandle.CreateStub<DummyService, DummyTraits>(componentId, null, traits));
        }

        [Test]
        public void CreateStub_WhenTraitsIsNull_Throws()
        {
            var component = MockRepository.GenerateStub<DummyService>();
            var traits = new DummyTraits();

            Assert.Throws<ArgumentNullException>(() => ComponentHandle.CreateStub<DummyService, DummyTraits>(null, component, traits));
        }

        [Test]
        public void CreateStub_WhenArgumentsValid_ReturnsHandleWithStubbedDescriptor()
        {
            var componentId = "componentId";
            var component = MockRepository.GenerateStub<DummyService>();
            var traits = new DummyTraits();

            var handle = ComponentHandle.CreateStub<DummyService, DummyTraits>(componentId, component, traits);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(componentId, handle.Id);
                Assert.AreEqual(typeof(DummyService), handle.ServiceType);
                Assert.AreEqual(typeof(DummyTraits), handle.TraitsType);
                Assert.AreSame(component, handle.GetComponent());
                Assert.AreSame(traits, handle.GetTraits());

                object x = null;
                Assert.Throws<NotSupportedException>(() => x = handle.Descriptor.Plugin);
                Assert.Throws<NotSupportedException>(() => x = handle.Descriptor.Service);
                Assert.AreEqual(componentId, handle.Descriptor.ComponentId);
                Assert.AreEqual(new TypeName(component.GetType()), handle.Descriptor.ComponentTypeName);
                Assert.Throws<NotSupportedException>(() => x = handle.Descriptor.ComponentHandlerFactory);
                Assert.Throws<NotSupportedException>(() => x = handle.Descriptor.ComponentProperties);
                Assert.Throws<NotSupportedException>(() => x = handle.Descriptor.TraitsProperties);
                Assert.IsFalse(handle.Descriptor.IsDisabled);
                Assert.Throws<InvalidOperationException>(() => x = handle.Descriptor.DisabledReason);
                Assert.AreEqual(component.GetType(), handle.Descriptor.ResolveComponentType());
                Assert.Throws<NotSupportedException>(() => handle.Descriptor.ResolveComponentHandler());
                Assert.AreSame(component, handle.Descriptor.ResolveComponent());
                Assert.Throws<NotSupportedException>(() => handle.Descriptor.ResolveTraitsHandler());
                Assert.AreSame(traits, handle.Descriptor.ResolveTraits());
            });
        }

        [Test]
        public void IsComponentHandleType_WhenTypeIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => ComponentHandle.IsComponentHandleType(null));
        }

        [Test]
        public void IsComponentHandleType_WhenTypeIsNonGenericComponentHandle_ReturnsTrue()
        {
            Assert.IsTrue(ComponentHandle.IsComponentHandleType(typeof(ComponentHandle)));
        }

        [Test]
        public void IsComponentHandleType_WhenTypeIsGenericComponentHandle_ReturnsTrue()
        {
            Assert.IsTrue(ComponentHandle.IsComponentHandleType(typeof(ComponentHandle<DummyService, DummyTraits>)));
        }

        [Test]
        public void IsComponentHandleType_WhenTypeIsNotAComponentHandleComponentHandle_ReturnsFalse()
        {
            Assert.IsFalse(ComponentHandle.IsComponentHandleType(typeof(object)));
        }

        [Test]
        public void GetComponentUntyped_WhenComponentCanBeResolved_ReturnsItAndMemoizesIt()
        {
            var component = MockRepository.GenerateStub<DummyService>();
            var componentDescriptor = CreateStubComponentDescriptor<DummyService, DummyTraits>();
            var componentHandle = ComponentHandle.CreateInstance(componentDescriptor);
            componentDescriptor.Expect(x => x.ResolveComponent()).Return(component);

            // first time
            object result = componentHandle.GetComponent();
            Assert.AreSame(component, result);

            // second time should be same but method only called once
            result = componentHandle.GetComponent();
            Assert.AreSame(component, result);

            componentDescriptor.VerifyAllExpectations();
        }

        [Test]
        public void GetTraitsUntyped_WhenComponentCanBeResolved_ReturnsItAndMemoizesIt()
        {
            var traits = new DummyTraits();
            var componentDescriptor = CreateStubComponentDescriptor<DummyService, DummyTraits>();
            var componentHandle = ComponentHandle.CreateInstance(componentDescriptor);
            componentDescriptor.Expect(x => x.ResolveTraits()).Return(traits);

            // first time
            Traits result = componentHandle.GetTraits();
            Assert.AreSame(traits, result);

            // second time should be same but method only called once
            result = componentHandle.GetTraits();
            Assert.AreSame(traits, result);

            componentDescriptor.VerifyAllExpectations();
        }

        [Test]
        public void GetComponentTyped_WhenComponentCanBeResolved_ReturnsAndMemoizesIt()
        {
            var component = MockRepository.GenerateStub<DummyService>();
            var componentDescriptor = CreateStubComponentDescriptor<DummyService, DummyTraits>();
            var componentHandle = ComponentHandle.CreateInstance<DummyService, DummyTraits>(componentDescriptor);
            componentDescriptor.Expect(x => x.ResolveComponent()).Return(component);

            // first time
            DummyService result = componentHandle.GetComponent();
            Assert.AreSame(component, result);

            // second time should be same but method only called once
            result = componentHandle.GetComponent();
            Assert.AreSame(component, result);

            componentDescriptor.VerifyAllExpectations();
        }

        [Test]
        public void GetTraitsTyped_WhenComponentCanBeResolved_ReturnsAndMemoizesIt()
        {
            var traits = new DummyTraits();
            var componentDescriptor = CreateStubComponentDescriptor<DummyService, DummyTraits>();
            var componentHandle = ComponentHandle.CreateInstance<DummyService, DummyTraits>(componentDescriptor);
            componentDescriptor.Expect(x => x.ResolveTraits()).Return(traits);

            // first time
            DummyTraits result = componentHandle.GetTraits();
            Assert.AreSame(traits, result);

            // second time should be same but method only called once
            result = componentHandle.GetTraits();
            Assert.AreSame(traits, result);

            componentDescriptor.VerifyAllExpectations();
        }

        [Test]
        public void Id_ReturnsComponentId()
        {
            var componentDescriptor = CreateStubComponentDescriptor<DummyService, DummyTraits>();
            componentDescriptor.Stub(x => x.ComponentId).Return("componentId");
            var componentHandle = ComponentHandle.CreateInstance<DummyService, DummyTraits>(componentDescriptor);

            Assert.AreEqual("componentId", componentHandle.Id);
        }

        [Test]
        public void ToString_ReturnsComponentId()
        {
            var componentDescriptor = CreateStubComponentDescriptor<DummyService, DummyTraits>();
            componentDescriptor.Stub(x => x.ComponentId).Return("componentId");
            var componentHandle = ComponentHandle.CreateInstance<DummyService, DummyTraits>(componentDescriptor);

            Assert.AreEqual("componentId", componentHandle.ToString());
        }

        private IComponentDescriptor CreateStubComponentDescriptor<TService, TTraits>()
            where TTraits : Traits
        {
            var serviceDescriptor = MockRepository.GenerateMock<IServiceDescriptor>();
            serviceDescriptor.Stub(x => x.ResolveServiceType()).Return(typeof(TService));
            serviceDescriptor.Stub(x => x.ResolveTraitsType()).Return(typeof(TTraits));

            var componentDescriptor = MockRepository.GenerateMock<IComponentDescriptor>();
            componentDescriptor.Stub(x => x.Service).Return(serviceDescriptor);
            return componentDescriptor;
        }

        [Traits(typeof(DummyTraits))]
        public interface DummyService
        {
        }

        public class DummyTraits : Traits
        {
        }
    }
}
