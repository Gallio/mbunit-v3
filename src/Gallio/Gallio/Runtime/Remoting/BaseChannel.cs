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
using System.Threading;
using Gallio.Common.Platform;

namespace Gallio.Runtime.Remoting
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
            InitializeAppDomainUnloadAutomaticUnregistrationHack();
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

        private static void InitializeAppDomainUnloadAutomaticUnregistrationHack()
        {
            // HACK: When the AppDomain is being unloaded, we can encounter problems because the channel
            //       tries to keep communicating but fails.  In particular, there is a problem in the
            //       IpcPort implementation wherein it will receive a final overlapped I/O call
            //       from its last pending read after it has been disposed.  When this happens, it
            //       will continue to process the next message in the pipe.
            //
            //       This wreaks some havoc during the rather precarious time of AppDomain unloading.
            //       So as a precaution, we now ensure that all channels are promptly unregistered
            //       as the AppDomain gets unloaded.  This avoids the apparent race condition we
            //       are observing with the I/O completion port notification occurring after finalization.
            //
            //       A better solution would be to ensure that we dispose channels explicitly before
            //       unloading, but we might not actually be in control of the unload.  It would be
            //       even better if the .Net framework better tolerated active channels during unload.
            //       Oh well.
            //
            //       To reproduce, run the BinaryIpcChannelTest a few times with the following line commented.
            //       The problem will manifest as an unhandled exception due to an invalid cross-AppDomain
            //       call just as the AppDomain is being unloaded.
            //       -- Jeff.
            if (DotNetRuntimeSupport.IsUsingMono)
                return; // Don't need this hack on Mono.  Actually, it causes hangs.

            AppDomain.CurrentDomain.DomainUnload += delegate
            {
                foreach (IChannel channel in ChannelServices.RegisteredChannels)
                    ChannelServices.UnregisterChannel(channel);

                // Sleep a litle to drain any pending I/O requests.
                Thread.Sleep(10);
            };
        }
    }
}
