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
using System.Threading;
using Gallio.Runtime.Logging;
using Gallio.Runtime;
using Gallio.Properties;

namespace Gallio.Runtime.Hosting
{
    /// <para>
    /// An implementation of <see cref="IHost" /> that communicates with a 
    /// <see cref="RemoteHostService" /> that resides in a different context
    /// using .Net remoting.
    /// </para>
    /// <para>
    /// This implementation also provides a mechanism for periodically polling
    /// the remote service to keep it alive and to detect abrupt disconnection.
    /// </para>
    public abstract class RemoteHost : BaseHost
    {
        private readonly TimeSpan? pingInterval;
        private readonly object pingLock = new object();
        private Timer pingTimer;
        private bool lastPingFailed;
        private bool pingInProgress;

        /// <summary>
        /// Creates an uninitialized host.
        /// </summary>
        /// <param name="hostSetup">The host setup</param>
        /// <param name="logger">The logger for host message output</param>
        /// <param name="pingInterval">The automatic ping interval, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostSetup"/>
        /// or <paramref name="logger"/> is null</exception>
        protected RemoteHost(HostSetup hostSetup, ILogger logger, TimeSpan? pingInterval)
            : base(hostSetup, logger)
        {
            this.pingInterval = pingInterval;
        }

        /// <inheritdoc />
        public override bool IsLocal
        {
            get { return false; }
        }

        /// <inheritdoc />
        protected sealed override IHostService AcquireHostService()
        {
            IRemoteHostService remoteHostService = AcquireRemoteHostService();
            if (remoteHostService == null)
                return null;

            StartPingTimer();
            return new ProxyHostService(remoteHostService);
        }

        /// <inheritdoc />
        protected sealed override void ReleaseHostService(IHostService hostService)
        {
            StopPingTimer();

            ProxyHostService proxyHostService = (ProxyHostService)hostService;
            proxyHostService.Dispose();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            StopPingTimer();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Connects to the remote host service.
        /// </summary>
        /// <returns>The remote host service</returns>
        protected abstract IRemoteHostService AcquireRemoteHostService();

        private void StartPingTimer()
        {
            if (!pingInterval.HasValue)
                return;

            lock (pingLock)
            {
                pingTimer = new Timer(PingTimerElapsed, null, pingInterval.Value, pingInterval.Value);
            }
        }

        private void StopPingTimer()
        {
            lock (pingLock)
            {
                if (pingTimer != null)
                {
                    pingTimer.Dispose();
                    pingTimer = null;
                }
            }
        }

        private void PingTimerElapsed(object state)
        {
            bool pinged = false;
            try
            {
                lock (pingLock)
                {
                    if (pingInProgress || pingTimer == null)
                        return;

                    pinged = true;
                    pingInProgress = true;
                }

#if DEBUG // FIXME: For debugging the remoting starvation issue.  See Google Code issue #147.  Remove when fixed.
                RuntimeAccessor.Logger.Log(LogSeverity.Debug, String.Format("[Ping] {0:o}", DateTime.Now));
#endif
                IHostService hostService = HostService;
                if (hostService != null)
                    hostService.Ping();

                lastPingFailed = false;
            }
            catch (Exception ex)
            {
                if (!lastPingFailed)
                {
                    UnhandledExceptionPolicy.Report("Could not send Ping message to the remote host service.", ex);
                    lastPingFailed = true;
                }
            }
            finally
            {
                if (pinged)
                    pingInProgress = false;
            }
        }

        private sealed class ProxyHostService : BaseHostService
        {
            private IRemoteHostService hostService;

            public ProxyHostService(IRemoteHostService hostService)
            {
                this.hostService = hostService;
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (hostService != null)
                {
                    try
                    {
                        hostService.Shutdown();
                    }
                    catch (Exception)
                    {
                        // Ignore the exception since we're shutting down anyways.
                    }
                }

                hostService = null;
            }

            protected override void PingImpl()
            {
                ThrowIfNotConnected();

                try
                {
                    hostService.Ping();
                }
                catch (Exception ex)
                {
                    throw new HostException(Resources.RemoteHost_RemoteException, ex);
                }
            }

            protected override void DoCallbackImpl(CrossAppDomainDelegate callback)
            {
                ThrowIfNotConnected();

                try
                {
                    hostService.DoCallback(callback);
                }
                catch (Exception ex)
                {
                    throw new HostException(Resources.RemoteHost_RemoteException, ex);
                }
            }

            protected override ObjectHandle CreateInstanceImpl(string assemblyName, string typeName)
            {
                ThrowIfNotConnected();

                try
                {
                    return hostService.CreateInstance(assemblyName, typeName);
                }
                catch (Exception ex)
                {
                    throw new HostException(Resources.RemoteHost_RemoteException, ex);
                }
            }

            protected override ObjectHandle CreateInstanceFromImpl(string assemblyPath, string typeName)
            {
                ThrowIfNotConnected();

                try
                {
                    return hostService.CreateInstanceFrom(assemblyPath, typeName);
                }
                catch (Exception ex)
                {
                    throw new HostException(Resources.RemoteHost_RemoteException, ex);
                }
            }

            private void ThrowIfNotConnected()
            {
                if (hostService == null)
                    throw new InvalidOperationException("The remote host service is not connected.");
            }
        }
    }
}
