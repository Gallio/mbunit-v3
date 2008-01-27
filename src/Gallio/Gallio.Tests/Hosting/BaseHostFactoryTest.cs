// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using Gallio.Hosting;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Hosting
{
    [TestFixture]
    [TestsOn(typeof(BaseHostFactory))]
    public class BaseHostFactoryTest
    {
        [Test, ExpectedArgumentNullException]
        public void CreateHostThrowsIfHostSetupIsNull()
        {
            StubHostFactory factory = new StubHostFactory();
            factory.CreateHost(null);
        }

        [Test]
        public void CreateHostDelegatesToCreateHostImplWithACanonicalizedHostSetup()
        {
            StubHostFactory factory = new StubHostFactory();

            HostSetup originalHostSetup = new HostSetup();
            Assert.IsNotNull(factory.CreateHost(originalHostSetup));

            Assert.AreNotSame(originalHostSetup, factory.HostSetup);
            Assert.AreEqual(Environment.CurrentDirectory, factory.HostSetup.WorkingDirectory);
        }

        private class StubHostFactory : BaseHostFactory
        {
            public HostSetup HostSetup;

            protected override IHost CreateHostImpl(HostSetup hostSetup)
            {
                HostSetup = hostSetup;
                return new MockRepository().Stub<IHost>();
            }
        }
    }
}