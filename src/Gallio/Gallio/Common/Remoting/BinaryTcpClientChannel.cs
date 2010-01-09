// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace Gallio.Common.Remoting
{
    /// <summary>
    /// A client channel based on an <see cref="TcpClientChannel" /> that uses a
    /// <see cref="BinaryClientFormatterSinkProvider" />.
    /// </summary>
    public class BinaryTcpClientChannel : BaseClientChannel
    {
        /// <summary>
        /// Creates a channel.
        /// </summary>
        /// <param name="hostName">The host name to connect to.</param>
        /// <param name="portNumber">The port number to connect to.</param>
        /// <param name="requestTimeout">The request timeout or null if none.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostName"/> is null.</exception>
        public BinaryTcpClientChannel(string hostName, int portNumber, TimeSpan? requestTimeout)
            : base(CreateChannel(hostName, portNumber, requestTimeout), new Uri(@"tcp://" + hostName + ":" + portNumber))
        {
        }

        private static IChannel CreateChannel(string hostName, int portNumber, TimeSpan? requestTimeout)
        {
            if (hostName == null)
                throw new ArgumentNullException("hostName");

            IDictionary channelProperties = new Hashtable();
            channelProperties[@"name"] = @"tcp-client:" + hostName + ":" + portNumber;
            channelProperties[@"secure"] = false;
            channelProperties[@"timeout"] = requestTimeout.HasValue ? (int)requestTimeout.Value.TotalMilliseconds : -1;

            return new TcpClientChannel(channelProperties, CreateClientChannelSinkProvider());
        }

        private static IClientChannelSinkProvider CreateClientChannelSinkProvider()
        {
            IDictionary formatterProperties = new Hashtable();
            formatterProperties[@"includeVersions"] = false;

            return new BinaryClientFormatterSinkProvider(formatterProperties, null);
        }
    }
}
