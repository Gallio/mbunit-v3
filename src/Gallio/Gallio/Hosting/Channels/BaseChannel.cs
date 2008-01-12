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

        /// <summary>
        /// Disposes of the channel.
        /// </summary>
        ~BaseChannel()
        {
            Dispose(false);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the .Net remoting channel.
        /// </summary>
        protected IChannel Channel
        {
            get { return channel; }
        }

        /// <summary>
        /// Gets the root Uri associated with the channel.
        /// </summary>
        protected Uri ChannelUri
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
    }
}
