// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;

namespace Gallio.Hosting.Channels
{
    /// <summary>
    /// Abstract base class for channels implemented using the .Net remoting infrastructure.
    /// </summary>
    public abstract class BaseChannel : IDisposable
    {
        private IChannel channel;
        private Uri channelUri;

        static BaseChannel()
        {
            RemotingConfiguration.Configure(null, false);
        }

        /// <summary>
        /// Creates a channel.
        /// </summary>
        /// <param name="channel">The .Net remoting channel</param>
        /// <param name="channelUri">The root Uri associated with the channel</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="channel"/> or <paramref name="channelUri"/> is null</exception>
        protected BaseChannel(IChannel channel, Uri channelUri)
        {
            if (channel == null)
                throw new ArgumentNullException("channel");
            if (channelUri == null)
                throw new ArgumentNullException("channelUri");

            this.channel = channel;
            this.channelUri = channelUri;

            ChannelServices.RegisterChannel(channel, false);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the associated .Net remoting channel.
        /// </summary>
        public IChannel Channel
        {
            get { return channel; }
        }

        /// <summary>
        /// Gets the root Uri associated with the channel.
        /// </summary>
        public Uri ChannelUri
        {
            get { return channelUri; }
        }

        /// <inheritdoc />
        protected virtual void Dispose(bool disposing)
        {
            if (channel != null)
            {
                if (disposing)
                {
                    ChannelServices.UnregisterChannel(channel);
                }

                channel = null;
                channelUri = null;
            }
        }

        /// <summary>
        /// Gets the Uri of a service with the given name that can be accessed using this channel.
        /// </summary>
        /// <param name="serviceName">The service name</param>
        /// <returns>The service uri</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceName"/> is null</exception>
        public string GetServiceUri(string serviceName)
        {
            if (serviceName == null)
                throw new ArgumentNullException("serviceName");

            return new Uri(ChannelUri, serviceName).ToString();
        }
    }
}
