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
            Assert.Throws<ArgumentNullException>(() => ComponentHandle<DummyService, DummyTraits>.CreateInstance(null));
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
        public void IsCompomentHandleType_WhenTypeIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => ComponentHandle.IsComponentHandleType(null));
        }

        [Test]
        public void IsCompomentHandleType_WhenTypeIsNonGenericComponentHandle_ReturnsTrue()
        {
            Assert.IsTrue(ComponentHandle.IsComponentHandleType(typeof(ComponentHandle)));
        }

        [Test]
        public void IsCompomentHandleType_WhenTypeIsGenericComponentHandle_ReturnsTrue()
        {
            Assert.IsTrue(ComponentHandle.IsComponentHandleType(typeof(ComponentHandle<DummyService, DummyTraits>)));
        }

        [Test]
        public void IsCompomentHandleType_WhenTypeIsNotAComponentHandleComponentHandle_ReturnsFalse()
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
