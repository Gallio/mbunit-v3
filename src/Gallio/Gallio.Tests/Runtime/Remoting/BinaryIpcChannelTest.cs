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
using Gallio.Framework;
using Gallio.Model.Logging;
using Gallio.Runtime;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Remoting;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.Remoting
{
    [TestFixture]
    [TestsOn(typeof(BinaryIpcClientChannel))]
    [TestsOn(typeof(BinaryIpcServerChannel))]
    [DependsOn(typeof(BaseClientChannelTest))]
    [DependsOn(typeof(BaseServerChannelTest))]
    public class BinaryIpcChannelTest
    {
        private static readonly string PortName = typeof(BinaryIpcChannelTest).Name;
        private const string ServiceName = "Test";

        [Test, ExpectedArgumentNullException]
        public void BinaryIpcClientChannelConstructorThrowsIfPortNameIsNull()
        {
            new BinaryIpcClientChannel(null);
        }

        [Test, ExpectedArgumentNullException]
        public void BinaryIpcServerChannelConstructorThrowsIfPortNameIsNull()
        {
            new BinaryIpcServerChannel(null);
        }

        [Test]
        public void RegisteredServiceCanBeAccessedWithGetService()
        {
            var hostFactory = (IsolatedAppDomainHostFactory)RuntimeAccessor.Registry.ResolveByComponentId(IsolatedAppDomainHostFactory.ComponentId);
            using (IHost host = hostFactory.CreateHost(new HostSetup(), new TestLogStreamLogger(TestLog.Default)))
            {
                HostAssemblyResolverHook.InstallCallback(host);

                host.GetHostService().Do<object, object>(RemoteCallback, null);

                using (BinaryIpcClientChannel clientChannel = new BinaryIpcClientChannel(PortName))
                {
                    TestService serviceProxy =
                        (TestService)clientChannel.GetService(typeof(TestService), ServiceName);
                    Assert.AreEqual(42, serviceProxy.Add(23, 19));
                }
            }
        }

        public static object RemoteCallback(object dummy)
        {
            BinaryIpcServerChannel serverChannel = new BinaryIpcServerChannel(PortName);
            TestService serviceProvider = new TestService();
            serverChannel.RegisterService(ServiceName, serviceProvider);
            return null;
        }

        public class TestService : MarshalByRefObject
        {
            public int Add(int x, int y)
            {
                return x + y;
            }
        }
    }
}
