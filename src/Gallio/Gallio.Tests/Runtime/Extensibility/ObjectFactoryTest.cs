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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Gallio.Common.Collections;
using Gallio.Runtime;
using Gallio.Runtime.Extensibility;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Runtime.Extensibility
{
    [TestsOn(typeof(ObjectFactory))]
    public class ObjectFactoryTest
    {
        public class ArgumentValidation
        {
            [Test]
            public void Constructor_WhenObjectDependencyResolverIsNull_Throws()
            {
                Assert.Throws<ArgumentNullException>(() => new ObjectFactory(null, typeof(object), new PropertySet()));
            }

            [Test]
            public void Constructor_WhenObjectTypeIsNull_Throws()
            {
                var dependencyResolver = MockRepository.GenerateStub<IObjectDependencyResolver>();

                Assert.Throws<ArgumentNullException>(() => new ObjectFactory(dependencyResolver, null, new PropertySet()));
            }

            [Test]
            public void Constructor_WhenPropertySetIsNull_Throws()
            {
                var dependencyResolver = MockRepository.GenerateStub<IObjectDependencyResolver>();

                Assert.Throws<ArgumentNullException>(() => new ObjectFactory(dependencyResolver, typeof(object), null));
            }
        }

        public class DependencyInjection
        {
            [Test]
            public void CreateInstance_WhenComponentHasNoDependencies_InstantiatesItUsingDefaultConstructor()
            {
                var dependencyResolver = MockRepository.GenerateMock<IObjectDependencyResolver>();
                var objectFactory = new ObjectFactory(dependencyResolver, typeof(Component), new PropertySet());

                var component = (Component)objectFactory.CreateInstance();
                Assert.IsNotNull(component);

                dependencyResolver.VerifyAllExpectations();
            }

            [Test]
            public void CreateInstance_WhenComponentHasRequiredDependencyWithNoParameterValueAndItIsSatisfied_BuildsTheObjectWithTheResolvedDependency()
            {
                var dependencyResolver = MockRepository.GenerateMock<IObjectDependencyResolver>();
                var objectFactory = new ObjectFactory(dependencyResolver, typeof(ComponentWithRequiredDependencyOnService), new PropertySet());
                var service = MockRepository.GenerateStub<IService>();
                dependencyResolver.Expect(x => x.ResolveDependency("service", typeof(IService), null)).Return(DependencyResolution.Satisfied(service));

                var component = (ComponentWithRequiredDependencyOnService)objectFactory.CreateInstance();
                Assert.AreSame(service, component.Service);

                dependencyResolver.VerifyAllExpectations();
            }

            [Test]
            public void CreateInstance_WhenComponentHasRequiredDependencyWithNoParameterValueAndItIsNotSatisfied_Throws()
            {
                var dependencyResolver = MockRepository.GenerateMock<IObjectDependencyResolver>();
                var objectFactory = new ObjectFactory(dependencyResolver, typeof(ComponentWithRequiredDependencyOnService), new PropertySet());
                dependencyResolver.Expect(x => x.ResolveDependency("service", typeof(IService), null)).Return(DependencyResolution.Unsatisfied());

                var ex = Assert.Throws<RuntimeException>(() => objectFactory.CreateInstance());
                Assert.AreEqual(string.Format("Could not resolve required dependency 'service' of type '{0}'.", typeof(IService)), ex.Message);

                dependencyResolver.VerifyAllExpectations();
            }

            [Test]
            public void CreateInstance_WhenComponentHasRequiredDependencyWithNoParameterValueAndItIsNotSatisfiedDueToException_Throws()
            {
                var dependencyResolver = MockRepository.GenerateMock<IObjectDependencyResolver>();
                var objectFactory = new ObjectFactory(dependencyResolver, typeof(ComponentWithRequiredDependencyOnService), new PropertySet());
                dependencyResolver.Expect(x => x.ResolveDependency("service", typeof(IService), null)).Throw(new InvalidOperationException("Boom"));

                var ex = Assert.Throws<RuntimeException>(() => objectFactory.CreateInstance());
                Assert.AreEqual(string.Format("Could not resolve required dependency 'service' of type '{0}' due to an exception.", typeof(IService)), ex.Message);
                Assert.IsInstanceOfType<InvalidOperationException>(ex.InnerException, "Should contain rethrown exception.");

                dependencyResolver.VerifyAllExpectations();
            }

            [Test]
            public void CreateInstance_WhenComponentHasOptionalDependencyWithNoParameterValueAndItIsSatisfied_BuildsTheObjectWithTheResolvedDependency()
            {
                var dependencyResolver = MockRepository.GenerateMock<IObjectDependencyResolver>();
                var objectFactory = new ObjectFactory(dependencyResolver, typeof(ComponentWithOptionalDependencyOnService), new PropertySet());
                var service = MockRepository.GenerateStub<IService>();
                dependencyResolver.Expect(x => x.ResolveDependency("Service", typeof(IService), null)).Return(DependencyResolution.Satisfied(service));

                var component = (ComponentWithOptionalDependencyOnService)objectFactory.CreateInstance();
                Assert.AreSame(service, component.Service);

                dependencyResolver.VerifyAllExpectations();
            }

            [Test]
            public void CreateInstance_WhenComponentHasOptionalDependencyWithNoParameterValueAndItIsNotSatisfied_BuildsTheObjectWithoutTheResolvedDependency()
            {
                var dependencyResolver = MockRepository.GenerateMock<IObjectDependencyResolver>();
                var objectFactory = new ObjectFactory(dependencyResolver, typeof(ComponentWithOptionalDependencyOnService), new PropertySet());
                dependencyResolver.Expect(x => x.ResolveDependency("Service", typeof(IService), null)).Return(DependencyResolution.Unsatisfied());

                var component = (ComponentWithOptionalDependencyOnService)objectFactory.CreateInstance();
                Assert.IsNull(component.Service);

                dependencyResolver.VerifyAllExpectations();
            }

            [Test]
            public void CreateInstance_WhenComponentHasOptionalDependencyWithNoParameterValueAndItIsNotSatisfiedDueToException_Throws()
            {
                var dependencyResolver = MockRepository.GenerateMock<IObjectDependencyResolver>();
                var objectFactory = new ObjectFactory(dependencyResolver, typeof(ComponentWithOptionalDependencyOnService), new PropertySet());
                dependencyResolver.Expect(x => x.ResolveDependency("Service", typeof(IService), null)).Throw(new InvalidOperationException("Boom"));

                var ex = Assert.Throws<RuntimeException>(() => objectFactory.CreateInstance());
                Assert.AreEqual(string.Format("Could not resolve optional dependency 'Service' of type '{0}' due to an exception.", typeof(IService)), ex.Message);
                Assert.IsInstanceOfType<InvalidOperationException>(ex.InnerException, "Should contain rethrown exception.");

                dependencyResolver.VerifyAllExpectations();
            }

            [Test]
            public void CreateInstance_WhenComponentHasRequiredDependencyWithAParameterValueAndItIsSatisfied_BuildsTheObjectWithTheResolvedDependency()
            {
                var dependencyResolver = MockRepository.GenerateMock<IObjectDependencyResolver>();
                var objectFactory = new ObjectFactory(dependencyResolver, typeof(ComponentWithRequiredDependencyOnProperty), new PropertySet()
                {
                    { "prOpERtY", "value" }
                });
                dependencyResolver.Expect(x => x.ResolveDependency("property", typeof(string), "value")).Return(DependencyResolution.Satisfied("value"));

                var component = (ComponentWithRequiredDependencyOnProperty)objectFactory.CreateInstance();
                Assert.AreEqual("value", component.Property);

                dependencyResolver.VerifyAllExpectations();
            }

            [Test]
            public void CreateInstance_WhenComponentHasMultipleConstructors_UsesPublicConstructorWithMostParameters()
            {
                var dependencyResolver = MockRepository.GenerateMock<IObjectDependencyResolver>();
                var objectFactory = new ObjectFactory(dependencyResolver, typeof(ComponentWithMultipleConstructors), new PropertySet()
                {
                    { "prOpERtY", "value" }
                });
                var service = MockRepository.GenerateStub<IService>();
                dependencyResolver.Expect(x => x.ResolveDependency("service", typeof(IService), null)).Return(DependencyResolution.Satisfied(service));
                dependencyResolver.Expect(x => x.ResolveDependency("property", typeof(string), "value")).Return(DependencyResolution.Satisfied("value"));

                var component = (ComponentWithMultipleConstructors)objectFactory.CreateInstance();
                Assert.AreSame(service, component.Service);
                Assert.AreEqual("value", component.Property);

                dependencyResolver.VerifyAllExpectations();
            }

            [Test]
            public void CreateInstance_WhenComponentHasNoPublicConstructors_Throws()
            {
                var dependencyResolver = MockRepository.GenerateMock<IObjectDependencyResolver>();
                var objectFactory = new ObjectFactory(dependencyResolver, typeof(ComponentWithNoPublicConstructors), new PropertySet());

                var ex = Assert.Throws<RuntimeException>(() => objectFactory.CreateInstance());
                Assert.AreEqual(string.Format("Type '{0}' does not have any public constructors.", typeof(ComponentWithNoPublicConstructors)), ex.Message);

                dependencyResolver.VerifyAllExpectations();
            }

            [Test]
            public void CreateInstance_WhenComponentIsAbstract_Throws()
            {
                var dependencyResolver = MockRepository.GenerateMock<IObjectDependencyResolver>();
                var objectFactory = new ObjectFactory(dependencyResolver, typeof(AbstractComponent), new PropertySet());

                var ex = Assert.Throws<RuntimeException>(() => objectFactory.CreateInstance());
                Assert.AreEqual(string.Format("Type '{0}' is abstract and cannot be instantiated.", typeof(AbstractComponent)), ex.Message);

                dependencyResolver.VerifyAllExpectations();
            }
        }

        public interface IService
        {
        }

        private class Component
        {
        }

        private class ComponentWithRequiredDependencyOnService
        {
            public ComponentWithRequiredDependencyOnService(IService service)
            {
                this.Service = service;
            }

            public IService Service { get; private set; }
        }

        private class ComponentWithOptionalDependencyOnService
        {
            public IService Service { get; set; }
        }

        private class ComponentWithRequiredDependencyOnProperty
        {
            public ComponentWithRequiredDependencyOnProperty(string property)
            {
                Property = property;
            }

            public string Property { get; private set; }
        }

        private class ComponentWithMultipleConstructors
        {
            public ComponentWithMultipleConstructors()
                : this(null)
            {
            }

            public ComponentWithMultipleConstructors(IService service)
                : this(service, null)
            {
            }

            public ComponentWithMultipleConstructors(IService service, string property)
                : this(service, property, true)
            {
                Property = property;
            }

            private ComponentWithMultipleConstructors(IService service, string property, bool internalFlag)
            {
                Property = property;
                Service = service;
            }

            public IService Service { get; private set; }
            public string Property { get; private set; }
        }

        private class ComponentWithNoPublicConstructors
        {
            private ComponentWithNoPublicConstructors()
            {
            }
        }

        private abstract class AbstractComponent
        {
        }
    }
}
