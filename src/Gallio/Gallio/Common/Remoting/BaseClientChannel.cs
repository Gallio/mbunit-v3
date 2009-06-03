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
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;

namespace Gallio.Common.Remoting
{
    /// <summary>
    /// Abstract base class for client channels implemented using the .Net
    /// remoting infrastructure.
    /// </summary>
    public abstract class BaseClientChannel : BaseChannel, IClientChannel
    {
        /// <summary>
        /// Creates a channel.
        /// </summary>
        /// <param name="channel">The .Net remoting channel.</param>
        /// <param name="channelUri">The root Uri associated with the channel.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="channel"/> or <paramref name="channelUri"/> is null.</exception>
        protected BaseClientChannel(IChannel channel, Uri channelUri)
            : base(channel, channelUri)
        {
        }

        /// <inheritdoc />
        public object GetService(Type serviceType, string serviceName)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");
            if (serviceName == null)
                throw new ArgumentNullException("serviceName");

            return RemotingServices.Connect(serviceType, GetServiceUri(serviceName));
        }
    }
}
