using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Collections;
using Gallio.Runtime.Extensibility;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Runtime.Extensibility
{
    [TestsOn(typeof(InstanceHandlerFactory))]
    public class InstanceHandlerFactoryTest
    {
        [Test]
        public void Constructor_WhenInstanceIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new InstanceHandlerFactory(null));
        }

        [Test]
        public void CreateHandler_WhenInstanceIsNotOfRequestedType_Throws()
        {
            string instance = "";
            var instanceHandlerFactory = new InstanceHandlerFactory(instance);
            var serviceLocator = MockRepository.GenerateStub<IServiceLocator>();
            var resourceLocator = MockRepository.GenerateStub<IResourceLocator>();
            var contractType = typeof(int); // not a string as the instance is
            var objectType = typeof(int);

            var ex = Assert.Throws<RuntimeException>(() => instanceHandlerFactory.CreateHandler(serviceLocator, resourceLocator,
                contractType, objectType, new PropertySet()));
            Assert.AreEqual("Could not satisfy contract of type 'System.Int32' using pre-manufactured instance of type 'System.String'.", ex.Message);
        }

        [Test]
        public void CreateHandler_WhenInstanceIsOfCorrectType_ReturnsAHandlerThatYieldsTheInstance()
        {
            string instance = "";
            var instanceHandlerFactory = new InstanceHandlerFactory(instance);
            var serviceLocator = MockRepository.GenerateStub<IServiceLocator>();
            var resourceLocator = MockRepository.GenerateStub<IResourceLocator>();
            var contractType = typeof(string);
            var objectType = typeof(string);

            IHandler handler = instanceHandlerFactory.CreateHandler(serviceLocator, resourceLocator,
                contractType, objectType, new PropertySet());

            Assert.AreSame(instance, handler.Activate());
        }
    }
}
