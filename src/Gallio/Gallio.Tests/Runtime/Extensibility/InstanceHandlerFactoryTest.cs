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
using Gallio.Common.Collections;
using Gallio.Runtime;
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
