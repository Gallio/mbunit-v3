using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Text;
using Gallio.Hosting.Channels;
using MbUnit.Framework;

namespace Gallio.Tests.Hosting.Channels
{
    [TestFixture]
    [TestsOn(typeof(BaseChannel))]
    public class BaseChannelTest : BaseUnitTest
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfChannelIsNull()
        {
            new StubChannel(null, new Uri("urn:foo"));
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfUriIsNull()
        {
            new StubChannel(Mocks.Stub<IChannel>(), null);
        }

        private class StubChannel : BaseChannel
        {
            public StubChannel(IChannel channel, Uri channelUri) : base(channel, channelUri)
            {
            }
        }
    }
}
