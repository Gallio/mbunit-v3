using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Gallio.Collections;
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
            public void Constructor_WhenServiceLocatorIsNull_Throws()
            {
                var resourceLocator = MockRepository.GenerateStub<IResourceLocator>();

                Assert.Throws<ArgumentNullException>(() => new ObjectFactory(null, resourceLocator, typeof(object), new PropertySet()));
            }

            [Test]
            public void Constructor_WhenResourceLocatorIsNull_Throws()
            {
                var serviceLocator = MockRepository.GenerateStub<IServiceLocator>();

                Assert.Throws<ArgumentNullException>(() => new ObjectFactory(serviceLocator, null, typeof(object), new PropertySet()));
            }

            [Test]
            public void Constructor_WhenObjectTypeIsNull_Throws()
            {
                var serviceLocator = MockRepository.GenerateStub<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateStub<IResourceLocator>();

                Assert.Throws<ArgumentNullException>(() => new ObjectFactory(serviceLocator, resourceLocator, null, new PropertySet()));
            }

            [Test]
            public void Constructor_WhenPropertySetIsNull_Throws()
            {
                var serviceLocator = MockRepository.GenerateStub<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateStub<IResourceLocator>();

                Assert.Throws<ArgumentNullException>(() => new ObjectFactory(serviceLocator, resourceLocator, typeof(object), null));
            }
        }

        public class DependencyInjection
        {
            [Test]
            public void CreateInstance_WhenComponentHasNoDependencies_InstantiatesItUsingDefaultConstructor()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var objectFactory = new ObjectFactory(serviceLocator, resourceLocator, typeof(Component), new PropertySet());

                var component = (Component)objectFactory.CreateInstance();
                Assert.IsNotNull(component);

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void CreateInstance_WhenComponentHasRequiredDependencyOnService_ResolvesDependencyWithServiceLocator()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var objectFactory = new ObjectFactory(serviceLocator, resourceLocator, typeof(ComponentWithRequiredDependencyOnService), new PropertySet());
                var service = MockRepository.GenerateStub<IService>();

                serviceLocator.Expect(x => x.CanResolve(typeof(IService))).Return(true);
                serviceLocator.Expect(x => x.Resolve(typeof(IService))).Return(service);

                var component = (ComponentWithRequiredDependencyOnService)objectFactory.CreateInstance();
                Assert.AreSame(service, component.Service);

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void CreateInstance_WhenComponentHasOptionalDependencyOnService_ResolvesDependencyWithServiceLocator()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var objectFactory = new ObjectFactory(serviceLocator, resourceLocator, typeof(ComponentWithOptionalDependencyOnService), new PropertySet());
                var service = MockRepository.GenerateStub<IService>();

                serviceLocator.Expect(x => x.CanResolve(typeof(IService))).Return(true);
                serviceLocator.Expect(x => x.Resolve(typeof(IService))).Return(service);

                var component = (ComponentWithOptionalDependencyOnService)objectFactory.CreateInstance();
                Assert.AreSame(service, component.Service);

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void CreateInstance_WhenComponentHasRequiredDependencyOnProperty_ResolvesDependencyWithPropertySetCaseInsensitively()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var objectFactory = new ObjectFactory(serviceLocator, resourceLocator, typeof(ComponentWithRequiredDependencyOnProperty), new PropertySet()
                {
                    { "prOpERtY", "value" }
                });

                var component = (ComponentWithRequiredDependencyOnProperty)objectFactory.CreateInstance();
                Assert.AreEqual("value", component.Property);

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void CreateInstance_WhenComponentHasOptionalDependencyOnProperty_ResolvesDependencyWithPropertySetCaseInsensitvely()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var objectFactory = new ObjectFactory(serviceLocator, resourceLocator, typeof(ComponentWithOptionalDependencyOnProperty), new PropertySet()
                {
                    { "prOpERtY", "value" }
                });

                var component = (ComponentWithOptionalDependencyOnProperty)objectFactory.CreateInstance();
                Assert.AreEqual("value", component.Property);

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void CreateInstance_WhenComponentHasRequiredDependencyOnServiceThatCannotBeResolved_Throws()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var objectFactory = new ObjectFactory(serviceLocator, resourceLocator, typeof(ComponentWithRequiredDependencyOnService), new PropertySet());

                serviceLocator.Expect(x => x.CanResolve(typeof(IService))).Return(false);

                var ex = Assert.Throws<RuntimeException>(() => objectFactory.CreateInstance());
                Assert.AreEqual(string.Format("Could not resolve required dependency '{0}' of type '{1}'.", "service", typeof(IService)), ex.Message);

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void CreateInstance_WhenComponentHasRequiredDependencyOnServiceThatThrowsDuringResolution_RethrowsWithMessage()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var objectFactory = new ObjectFactory(serviceLocator, resourceLocator, typeof(ComponentWithRequiredDependencyOnService), new PropertySet());

                serviceLocator.Expect(x => x.CanResolve(typeof(IService))).Return(true);
                serviceLocator.Expect(x => x.Resolve(typeof(IService))).Throw(new InvalidOperationException("boom"));

                var ex = Assert.Throws<RuntimeException>(() => objectFactory.CreateInstance());
                Assert.AreEqual(string.Format("Could not resolve required dependency '{0}' of type '{1}' due to an exception.", "service", typeof(IService)), ex.Message);
                Assert.IsInstanceOfType<InvalidOperationException>(ex.InnerException, "Should contain rethrown exception.");

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void CreateInstance_WhenComponentHasOptionalDependencyOnServiceThatCannotBeResolved_IgnoresIt()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var objectFactory = new ObjectFactory(serviceLocator, resourceLocator, typeof(ComponentWithOptionalDependencyOnService), new PropertySet());

                serviceLocator.Expect(x => x.CanResolve(typeof(IService))).Return(false);

                var component = (ComponentWithOptionalDependencyOnService)objectFactory.CreateInstance();
                Assert.IsNull(component.Service);

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void CreateInstance_WhenComponentHasOptionalDependencyOnServiceThatThrowsDuringResolution_RethrowsWithMessage()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var objectFactory = new ObjectFactory(serviceLocator, resourceLocator, typeof(ComponentWithOptionalDependencyOnService), new PropertySet());

                serviceLocator.Expect(x => x.CanResolve(typeof(IService))).Return(true);
                serviceLocator.Expect(x => x.Resolve(typeof(IService))).Throw(new InvalidOperationException("boom"));

                var ex = Assert.Throws<RuntimeException>(() => objectFactory.CreateInstance());
                Assert.AreEqual(string.Format("Could not resolve optional dependency '{0}' of type '{1}' due to an exception.", "Service", typeof(IService)), ex.Message);
                Assert.IsInstanceOfType<InvalidOperationException>(ex.InnerException, "Should contain rethrown exception.");

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void CreateInstance_WhenComponentHasMultipleConstructors_UsesPublicConstructorWithMostParameters()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var objectFactory = new ObjectFactory(serviceLocator, resourceLocator, typeof(ComponentWithMultipleConstructors), new PropertySet()
                {
                    { "prOpERtY", "value" }
                });
                var service = MockRepository.GenerateStub<IService>();

                serviceLocator.Expect(x => x.CanResolve(typeof(IService))).Return(true);
                serviceLocator.Expect(x => x.Resolve(typeof(IService))).Return(service);

                var component = (ComponentWithMultipleConstructors)objectFactory.CreateInstance();
                Assert.AreSame(service, component.Service);
                Assert.AreEqual("value", component.Property);

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void CreateInstance_WhenComponentHasNoPublicConstructors_Throws()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var objectFactory = new ObjectFactory(serviceLocator, resourceLocator, typeof(ComponentWithNoPublicConstructors), new PropertySet());

                var ex = Assert.Throws<RuntimeException>(() => objectFactory.CreateInstance());
                Assert.AreEqual(string.Format("Type '{0}' does not have any public constructors.", typeof(ComponentWithNoPublicConstructors)), ex.Message);

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void CreateInstance_WhenComponentIsAbstract_Throws()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var objectFactory = new ObjectFactory(serviceLocator, resourceLocator, typeof(AbstractComponent), new PropertySet());

                var ex = Assert.Throws<RuntimeException>(() => objectFactory.CreateInstance());
                Assert.AreEqual(string.Format("Type '{0}' is abstract and cannot be instantiated.", typeof(AbstractComponent)), ex.Message);

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }
        }

        public class PropertyConversions
        {
            [Test]
            public void CreateInstance_WhenPropertySpecifiesComponentId_ResolvesDependencyWithServiceLocatorUsingComponentId()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var objectFactory = new ObjectFactory(serviceLocator, resourceLocator, typeof(ComponentWithRequiredDependencyOnService), new PropertySet()
                    {
                        { "service", "${componentId}" }
                    });
                var service = MockRepository.GenerateStub<IService>();

                serviceLocator.Expect(x => x.ResolveByComponentId("componentId")).Return(service);

                var component = (ComponentWithRequiredDependencyOnService)objectFactory.CreateInstance();
                Assert.AreSame(service, component.Service);

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void CreateInstance_WhenDependencyIsOfTypeInt_ConvertsPropertyStringToInt()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var objectFactory = new ObjectFactory(serviceLocator, resourceLocator, typeof(ComponentWithPropertiesOfMultipleTypes), new PropertySet()
                    {
                        { "int", "42" }
                    });

                var component = (ComponentWithPropertiesOfMultipleTypes)objectFactory.CreateInstance();
                Assert.AreEqual(42, component.Int);

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void CreateInstance_WhenDependencyIsOfTypeImage_LoadsImageFromResource()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var objectFactory = new ObjectFactory(serviceLocator, resourceLocator, typeof(ComponentWithPropertiesOfMultipleTypes), new PropertySet()
                    {
                        { "image", "SampleImage.png" }
                    });

                resourceLocator.Expect(x => x.GetFullPath("SampleImage.png")).Return(@"..\Resources\SampleImage.png");

                var component = (ComponentWithPropertiesOfMultipleTypes)objectFactory.CreateInstance();
                Assert.IsNotNull(component.Image);

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void CreateInstance_WhenDependencyIsOfTypeIcon_LoadsIconFromResource()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var objectFactory = new ObjectFactory(serviceLocator, resourceLocator, typeof(ComponentWithPropertiesOfMultipleTypes), new PropertySet()
                    {
                        { "icon", "SampleIcon.ico" }
                    });

                resourceLocator.Expect(x => x.GetFullPath("SampleIcon.ico")).Return(@"..\Resources\SampleIcon.ico");

                var component = (ComponentWithPropertiesOfMultipleTypes)objectFactory.CreateInstance();
                Assert.IsNotNull(component.Icon);

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void CreateInstance_WhenDependencyIsOfTypeFileInfo_CreatesFileInfoForResource()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var objectFactory = new ObjectFactory(serviceLocator, resourceLocator, typeof(ComponentWithPropertiesOfMultipleTypes), new PropertySet()
                    {
                        { "fileInfo", "file.txt" }
                    });

                resourceLocator.Expect(x => x.GetFullPath("file.txt")).Return(@"C:\file.txt");

                var component = (ComponentWithPropertiesOfMultipleTypes)objectFactory.CreateInstance();
                Assert.AreEqual(@"C:\file.txt", component.FileInfo.ToString());

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
            }

            [Test]
            public void CreateInstance_WhenDependencyIsOfTypeDirectoryInfo_CreatesDirectoryInfoForResource()
            {
                var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
                var resourceLocator = MockRepository.GenerateMock<IResourceLocator>();
                var objectFactory = new ObjectFactory(serviceLocator, resourceLocator, typeof(ComponentWithPropertiesOfMultipleTypes), new PropertySet()
                    {
                        { "directoryInfo", "dir" }
                    });

                resourceLocator.Expect(x => x.GetFullPath("dir")).Return(@"C:\dir");

                var component = (ComponentWithPropertiesOfMultipleTypes)objectFactory.CreateInstance();
                Assert.AreEqual(@"C:\dir", component.DirectoryInfo.ToString());

                serviceLocator.VerifyAllExpectations();
                resourceLocator.VerifyAllExpectations();
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

        private class ComponentWithOptionalDependencyOnProperty
        {
            public string Property { get; set; }
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

        public class ComponentWithPropertiesOfMultipleTypes
        {
            public int Int { get; set; }

            public Image Image { get; set; }

            public Icon Icon { get; set; }

            public FileInfo FileInfo { get; set; }

            public DirectoryInfo DirectoryInfo { get; set; }
        }

        private abstract class AbstractComponent
        {
        }
    }
}
