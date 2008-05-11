// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Text;
using Gallio.Runtime.Remoting;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.Remoting
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

        [Test]
        public void RegistersTheChannelDuringConstructionAndUnregistersItOnDispose()
        {
            IpcClientChannel channel = new IpcClientChannel();
            Uri uri = new Uri("ipc://foo");

            using (StubChannel stub = new StubChannel(channel, uri))
            {
                CollectionAssert.Contains(ChannelServices.RegisteredChannels, channel);

                Assert.AreSame(channel, stub.Channel);
                Assert.AreEqual(uri, stub.ChannelUri);
            }

            CollectionAssert.DoesNotContain(ChannelServices.RegisteredChannels, channel);
        }

        [Test]
        public void GetServiceUriCombinesTheServiceNameWithTheChannelUri()
        {
            using (StubChannel stub = new StubChannel(new IpcClientChannel(), new Uri("ipc://foo")))
                Assert.AreEqual(new Uri("ipc://foo/Service"), stub.GetServiceUri("Service"));
        }

        [Test, ExpectedArgumentNullException]
        public void GetServiceUriThrowsIfNameIsNull()
        {
            using (StubChannel stub = new StubChannel(new IpcClientChannel(), new Uri("ipc://foo")))
                stub.GetServiceUri(null);
        }

        private class StubChannel : BaseChannel
        {
            public StubChannel(IChannel channel, Uri channelUri) : base(channel, channelUri)
            {
            }
        }
    }
}
