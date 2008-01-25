using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Hosting.Channels;
using MbUnit.Framework;

namespace Gallio.Tests.Hosting.Channels
{
    [TestFixture]
    [TestsOn(typeof(BaseClientChannel))]
    [DependsOn(typeof(BaseChannelTest))]
    public class BaseClientChannelTest
    {
        [Test, ExpectedArgumentNullException]
        public void GetServiceThrowsIfServiceTypeIsNull()
        {
            using (BinaryIpcClientChannel channel = new BinaryIpcClientChannel("port"))
                channel.GetService(null, "service");
        }

        [Test, ExpectedArgumentNullException]
        public void GetServiceThrowsIfServiceNameIsNull()
        {
            using (BinaryIpcClientChannel channel = new BinaryIpcClientChannel("port"))
                channel.GetService(typeof(int), null);
        }
    }
}
