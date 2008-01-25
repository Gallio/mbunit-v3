using System;
using System.Collections;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading;
using Gallio.Hosting;
using Gallio.Hosting.Channels;
using Gallio.Utilities;
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
        private static readonly string PortName = typeof(BinaryIpcChannelTest).Name.ToLower();
        private const string ServiceName = "Test";

        [Test]
        public void RegisteredServiceCanBeAccessedWithGetService()
        {
            using (ManagedAppDomain domain = ManagedAppDomain.Create(GetType().Name))
            {
                domain.AppDomain.DoCallBack(RemoteCallback);

                using (BinaryIpcClientChannel clientChannel = new BinaryIpcClientChannel(PortName))
                {
                    TestService serviceProxy = (TestService)clientChannel.GetService(typeof(TestService), ServiceName);
                    Assert.AreEqual(42, serviceProxy.Add(23, 19));
                }
            }
        }

        private static void RemoteCallback()
        {
            using (BinaryIpcServerChannel serverChannel = new BinaryIpcServerChannel(PortName))
            {
                TestService serviceProvider = new TestService();
                serverChannel.RegisterService(ServiceName, serviceProvider);
            }
        }

        private class TestService : MarshalByRefObject
        {
            public int Add(int x, int y)
            {
                return x + y;
            }
        }
    }
}
