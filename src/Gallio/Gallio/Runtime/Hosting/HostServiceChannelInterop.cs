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
using Gallio.Runtime.Remoting;

namespace Gallio.Runtime.Hosting
{
    /// <summary>
    /// Provides utilities to interact with a <see cref="RemoteHostService" /> over a <see cref="IClientChannel" />
    /// or <see cref="IServerChannel" />.
    /// </summary>
    public static class HostServiceChannelInterop
    {
        /// <summary>
        /// Gets the name used to register the <see cref="RemoteHostService" />.
        /// </summary>
        public const string ServiceName = "HostService";

        /// <summary>
        /// Gets a remote host service using the specified channel.
        /// </summary>
        /// <param name="channel">The channel</param>
        /// <returns>The remote host service</returns>
        public static IHostService GetRemoteHostService(IClientChannel channel)
        {
            if (channel == null)
                throw new ArgumentNullException("channel");

            return (IHostService)channel.GetService(typeof(IHostService), ServiceName);
        }

        /// <summary>
        /// Registers the host service with a channel.
        /// </summary>
        /// <param name="hostService">The remote host service</param>
        /// <param name="channel">The channel</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostService"/> or 
        /// <paramref name="channel"/> is null</exception>
        public static void RegisterWithChannel(RemoteHostService hostService, IServerChannel channel)
        {
            if (hostService == null)
                throw new ArgumentNullException("hostService");
            if (channel == null)
                throw new ArgumentNullException("channel");

            channel.RegisterService(ServiceName, hostService);
        }
    }
}
