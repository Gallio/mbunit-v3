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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Common.Reflection;
using Gallio.Runtime;
using Gallio.Runtime.Extensibility;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Runtime.Extensibility
{
    [TestsOn(typeof(DefaultObjectDependencyResolver))]
    public class DefaultObjectDependencyResolverTest
    {
        public class ArgumentValidations
        {
            [Test]
            public void Constructor_WhenServiceLocatorIsNull_Throws()
            {
                var resourceLocator = MockRepository.GenerateStub<IResourceLocator>();

                Assert.Throws<ArgumentNullException>(() => new DefaultObjectDependencyResolver(null, resourceLocator));
            }

            [Test]
            public void Constructor_WhenResourceLocatorIsNull_Throws()
            {
                var serviceLocator = MockRepository.GenerateStub<IServiceLocator>();

                Assert.Throws<ArgumentNullException>(() => new DefaultObjectDependencyResolver(serviceLocator, null));
            }

            [Test]
            public void ResolveDependency_WhenParameterNameIsNull_Throws()
            {
                var serviceLocator = MockRepository.GenerateStub<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateStub<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);

                Assert.Throws<ArgumentNullException>(() => dependencyResolver.ResolveDependency(null, typeof(string), "value"));
            }

            [Test]
            public void ResolveDependency_WhenParameterTypeIsNull_Throws()
            {
                var serviceLocator = MockRepository.GenerateStub<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateStub<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);

                Assert.Throws<ArgumentNullException>(() => dependencyResolver.ResolveDependency("name", null, "value"));
            }
        }

        public class ResolveByServiceLocation
        {
            [Test]
            public void ResolveDependency_WhenArgumentIsNullAndTypeIsScalar_ResolvesDependencyWithServiceLocatorByType()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);
                var service = MockRepository.GenerateStub<IService>();
                serviceLocator.Expect(x => x.HasService(typeof(IService))).Return(true);
                serviceLocator.Expect(x => x.Resolve(typeof(IService))).Return(service);

                var result = dependencyResolver.ResolveDependency("service", typeof(IService), null);

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);
                    Assert.IsInstanceOfType<IService>(result.Value);
                    Assert.AreSame(service, result.Value);
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenArgumentIsNullAndTypeIsScalarOfGenericComponentHandle_ResolvesDependencyWithServiceLocatorByType()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);
                var componentHandle = CreateStubComponentHandle<IService, Traits>();
                serviceLocator.Expect(x => x.HasService(typeof(IService))).Return(true);
                serviceLocator.Expect(x => x.ResolveHandle(typeof(IService))).Return(componentHandle);

                var result = dependencyResolver.ResolveDependency("service", typeof(ComponentHandle<IService, Traits>), null);

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);
                    Assert.AreSame(componentHandle, result.Value);
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenArgumentIsNullAndTypeIsScalarOfNonGenericComponentHandle_Throws()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);

                var ex = Assert.Throws<RuntimeException>(() => dependencyResolver.ResolveDependency("service", typeof(ComponentHandle), null));
                Assert.AreEqual("Could not detect service type from non-generic component handle.", ex.Message);
                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenArgumentIsNullAndTypeIsArray_ResolvesDependencyWithServiceLocatorByTypeAndReturnsAllMatches()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);
                var service1 = MockRepository.GenerateStub<IService>();
                var service2 = MockRepository.GenerateStub<IService>();
                serviceLocator.Expect(x => x.HasService(typeof(IService))).Return(true);
                serviceLocator.Expect(x => x.ResolveAll(typeof(IService))).Return(new object[] { service1, service2 });

                var result = dependencyResolver.ResolveDependency("service", typeof(IService[]), null);

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);
                    Assert.IsInstanceOfType<IService[]>(result.Value);

                    IService[] resultValue = (IService[])result.Value;
                    Assert.AreEqual(2, resultValue.Length);
                    Assert.AreSame(service1, resultValue[0]);
                    Assert.AreSame(service2, resultValue[1]);
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenArgumentIsNullAndTypeIsArrayOfGenericComponentHandle_ResolvesDependencyWithServiceLocatorByTypeAndReturnsAllMatches()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);
                var componentHandle1 = CreateStubComponentHandle<IService, Traits>();
                var componentHandle2 = CreateStubComponentHandle<IService, Traits>();
                serviceLocator.Expect(x => x.HasService(typeof(IService))).Return(true);
                serviceLocator.Expect(x => x.ResolveAllHandles(typeof(IService))).Return(new ComponentHandle[] { componentHandle1, componentHandle2 });

                var result = dependencyResolver.ResolveDependency("service", typeof(ComponentHandle<IService, Traits>[]), null);

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);

                    var resultValue = (ComponentHandle<IService, Traits>[])result.Value;
                    Assert.AreEqual(2, resultValue.Length);
                    Assert.AreSame(componentHandle1, resultValue[0]);
                    Assert.AreSame(componentHandle2, resultValue[1]);
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenArgumentIsNullAndTypeIsArrayOfNonGenericComponentHandle_Throws()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);

                var ex = Assert.Throws<RuntimeException>(() => dependencyResolver.ResolveDependency("service", typeof(ComponentHandle[]), null));
                Assert.AreEqual("Could not detect service type from non-generic component handle.", ex.Message);
                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenArgumentIsNullAndTypeIsScalarButServiceNotRegistered_ReturnsUnsatisfiedDependency()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);
                serviceLocator.Expect(x => x.HasService(typeof(IService))).Return(false);

                var result = dependencyResolver.ResolveDependency("service", typeof(IService), null);

                Assert.IsFalse(result.IsSatisfied);

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenArgumentIsNullAndTypeIsArrayButServiceNotRegistered_ReturnsUnsatisfiedDependency()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);
                serviceLocator.Expect(x => x.HasService(typeof(IService))).Return(false);

                var result = dependencyResolver.ResolveDependency("service", typeof(IService[]), null);

                Assert.IsFalse(result.IsSatisfied);

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }
        }

        public class ResolveByConfiguration
        {
            [Test]
            public void ResolveDependency_WhenArgumentSpecifiesComponentId_ResolvesDependencyWithServiceLocatorUsingComponentId()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);
                var service = MockRepository.GenerateStub<IService>();
                serviceLocator.Expect(x => x.ResolveByComponentId("componentId")).Return(service);

                var result = dependencyResolver.ResolveDependency("service", typeof(IService), "${componentId}");

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);
                    Assert.IsInstanceOfType<IService>(result.Value);
                    Assert.AreSame(service, result.Value);
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenArgumentSpecifiesComponentIdAndRequestedHandle_ResolvesDependencyWithServiceLocatorUsingComponentId()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);
                var componentHandle = CreateStubComponentHandle<IService, Traits>();
                serviceLocator.Expect(x => x.ResolveHandleByComponentId("componentId")).Return(componentHandle);

                var result = dependencyResolver.ResolveDependency("service", typeof(ComponentHandle), "${componentId}");

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);
                    Assert.AreSame(componentHandle, result.Value);
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenArgumentSpecifiesComponentIdThatDoesNotActuallyImplementServiceType_Throws()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);
                var service = new object();
                serviceLocator.Expect(x => x.ResolveByComponentId("componentId")).Return(service);

                var ex = Assert.Throws<RuntimeException>(() => dependencyResolver.ResolveDependency("service", typeof(IService), "${componentId}"));
                Assert.AreEqual(string.Format("Could not inject component with id 'componentId' into a dependency of type '{0}' because it is of the wrong type even though the component was explicitly specified using the '${{component.id}}' property value syntax.",
                    typeof(IService)), ex.Message);
                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenDependencyIsOfArrayType_SplitsAndParsesPropertyStringComponentsIndependently()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);

                var result = dependencyResolver.ResolveDependency("array", typeof(string[]), "abc;def;ghi");

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);
                    Assert.IsInstanceOfType<string[]>(result.Value);
                    Assert.AreEqual(new[] { "abc", "def", "ghi" }, result.Value);
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenDependencyIsOfEnumType_ParsesPropertyStringToEnumValueCaseInsensitively()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);

                var result = dependencyResolver.ResolveDependency("enum", typeof(YesNo), "no");

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);
                    Assert.IsInstanceOfType<YesNo>(result.Value);
                    Assert.AreEqual(YesNo.No, result.Value);
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenDependencyIsOfTypeVersion_ConvertsPropertyStringToVersion()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);

                var result = dependencyResolver.ResolveDependency("version", typeof(Version), "1.2.3.4");

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);
                    Assert.IsInstanceOfType<Version>(result.Value);
                    Assert.AreEqual(new Version(1, 2, 3, 4), result.Value);
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenDependencyIsOfTypeAssemblyName_ConvertsPropertyStringToAssemblyName()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);

                var result = dependencyResolver.ResolveDependency("assemblyName", typeof(AssemblyName), "Name");

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);
                    Assert.IsInstanceOfType<AssemblyName>(result.Value);
                    Assert.AreEqual("Name", ((AssemblyName)result.Value).Name);
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenDependencyIsOfTypeAssemblySignature_ConvertsPropertyStringToAssemblySignature()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);

                var result = dependencyResolver.ResolveDependency("assemblySig", typeof(AssemblySignature), "Name, Version=1.0.0.0-2.0.0.0");

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);
                    Assert.IsInstanceOfType<AssemblySignature>(result.Value);
                    Assert.AreEqual("Name, Version=1.0.0.0-2.0.0.0", result.Value.ToString());
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenDependencyIsOfTypeGuid_ConvertsPropertyStringToGuid()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);

                var result = dependencyResolver.ResolveDependency("guid", typeof(Guid), "{6DAA03EC-D603-43d4-A3E1-1607375A50A1}");

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);
                    Assert.IsInstanceOfType<Guid>(result.Value);
                    Assert.AreEqual(new Guid("{6DAA03EC-D603-43d4-A3E1-1607375A50A1}"), result.Value);
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenDependencyIsOfTypeCondition_ConvertsPropertyStringToCondition()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);

                var result = dependencyResolver.ResolveDependency("condition", typeof(Condition), "${env:VariableName}");

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);
                    Assert.IsInstanceOfType<Condition>(result.Value);
                    Assert.AreEqual("${env:VariableName}", result.Value.ToString());
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenDependencyIsOfTypeInt_ConvertsPropertyStringToInt()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);

                var result = dependencyResolver.ResolveDependency("int", typeof(int), "42");

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);
                    Assert.IsInstanceOfType<int>(result.Value);
                    Assert.AreEqual(42, result.Value);
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenDependencyIsOfTypeImage_LoadsImageFromResource()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);
                var uri = new Uri("file://SampleImage.png");
                resourceLocator.Expect(x => x.ResolveResourcePath(uri)).Return(@"..\Resources\SampleImage.png");

                var result = dependencyResolver.ResolveDependency("image", typeof(Image), uri.ToString());

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);
                    Assert.IsInstanceOfType<Image>(result.Value);
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenDependencyIsOfTypeIcon_LoadsIconFromResource()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);
                var uri = new Uri("file://SampleIcon.ico");
                resourceLocator.Expect(x => x.ResolveResourcePath(uri)).Return(@"..\Resources\SampleIcon.ico");

                var result = dependencyResolver.ResolveDependency("icon", typeof(Icon), uri.ToString());

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);
                    Assert.IsInstanceOfType<Icon>(result.Value);
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenDependencyIsOfTypeFileInfo_CreatesFileInfoForResource()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);
                var uri = new Uri("file://SampleFile.png");
                resourceLocator.Expect(x => x.ResolveResourcePath(uri)).Return(@"C:\file.txt");

                var result = dependencyResolver.ResolveDependency("fileInfo", typeof(FileInfo), uri.ToString());

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);
                    Assert.IsInstanceOfType<FileInfo>(result.Value);
                    Assert.AreEqual(@"C:\file.txt", result.Value.ToString());
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void ResolveDependency_WhenDependencyIsOfTypeDirectoryInfo_CreatesDirectoryInfoForResource()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);
                var uri = new Uri("file://Directory");
                resourceLocator.Expect(x => x.ResolveResourcePath(uri)).Return(@"C:\dir");

                var result = dependencyResolver.ResolveDependency("directoryInfo", typeof(DirectoryInfo), uri.ToString());

                Assert.Multiple(() =>
                {
                    Assert.IsTrue(result.IsSatisfied);
                    Assert.IsInstanceOfType<DirectoryInfo>(result.Value);
                    Assert.AreEqual(@"C:\dir", result.Value.ToString());
                });

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }
        }

        public interface IService
        {
        }

        public enum YesNo
        {
            Yes, No
        }

        private static ComponentHandle<TService, TTraits> CreateStubComponentHandle<TService, TTraits>()
            where TTraits : Traits
        {
            var serviceDescriptor = MockRepository.GenerateMock<IServiceDescriptor>();
            serviceDescriptor.Stub(x => x.ResolveServiceType()).Return(typeof(TService));
            serviceDescriptor.Stub(x => x.ResolveTraitsType()).Return(typeof(TTraits));

            var componentDescriptor = MockRepository.GenerateMock<IComponentDescriptor>();
            componentDescriptor.Stub(x => x.Service).Return(serviceDescriptor);
            return ComponentHandle.CreateInstance<TService, TTraits>(componentDescriptor);
        }
    }
}
