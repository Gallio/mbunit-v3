using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Hosting.Channels;
using MbUnit.Framework;

namespace Gallio.Tests.Hosting.Channels
{
    [TestFixture]
    [TestsOn(typeof(BaseServerChannel))]
    [DependsOn(typeof(BaseChannelTest))]
    public class BaseServerChannelTest : BaseUnitTest
    {
        [Test, ExpectedArgumentNullException]
        public void RegisterServiceThrowsIfServiceNameIsNull()
        {
            using (BinaryIpcServerChannel channel = new BinaryIpcServerChannel("port"))
                channel.RegisterService(null, Mocks.Stub<MarshalByRefObject>());
        }

        [Test, ExpectedArgumentNullException]
        public void RegisterServiceThrowsIfComponentIsNull()
        {
            using (BinaryIpcServerChannel channel = new BinaryIpcServerChannel("port"))
                channel.RegisterService("service", null);
        }
    }
}
