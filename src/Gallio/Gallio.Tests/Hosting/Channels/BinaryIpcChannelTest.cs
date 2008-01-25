using System;
using System.Runtime.Remoting;
using System.Threading;
using Gallio.Hosting;
using Gallio.Hosting.Channels;
using MbUnit.Framework;

namespace Gallio.Tests.Hosting.Channels
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

        [Test]
        public void RegisteredServiceCanBeAccessedWithGetService()
        {
            using (IHost host = new IsolatedAppDomainHostFactory().CreateHost(new HostSetup()))
            {
                HostAssemblyResolverHook.Install(host);

                host.DoCallback(RemoteCallback);

                using (BinaryIpcClientChannel clientChannel = new BinaryIpcClientChannel(PortName))
                {
                    TestService serviceProxy =
                        (TestService)clientChannel.GetService(typeof(TestService), ServiceName);
                    Assert.AreEqual(42, serviceProxy.Add(23, 19));
                }
            }
        }

        public static void RemoteCallback()
        {
            BinaryIpcServerChannel serverChannel = new BinaryIpcServerChannel(PortName);
            TestService serviceProvider = new TestService();
            serverChannel.RegisterService(ServiceName, serviceProvider);
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
