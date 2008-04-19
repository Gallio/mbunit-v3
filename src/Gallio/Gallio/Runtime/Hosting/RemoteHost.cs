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
    /// <summary>
    /// <para>
    /// An implementation of <see cref="IHost" /> intended to be accessed
    /// via .Net remoting from some other application context.
    /// </para>
    /// <para>
    /// This implementation wraps a <see cref="IHostService" /> with additional
    /// exception handling code and sends periodic heartbeat ping message.
    /// </para>
    /// </summary>
    public abstract class RemoteHost : BaseHost
    {
        private IHostService hostService;
        private TimeSpan? pingInterval;

        private readonly object pingLock = new object();
        private Timer pingTimer;
        private bool lastPingFailed;
        private bool pingInProgress;

        /// <summary>
        /// Creates an uninitialized host.
        /// </summary>
        /// <param name="hostSetup">The host setup</param>
        /// <param name="logger">The logger for host message output</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostSetup"/>
        /// or <paramref name="logger"/> is null</exception>
        protected RemoteHost(HostSetup hostSetup, ILogger logger)
            : base(hostSetup, logger)
        {
        }

        /// <inheritdoc />
        public override bool IsLocal
        {
            get { return false; }
        }

        /// <summary>
        /// Initializes the remote host and makes it ready for use.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the host has already been initialized.</exception>
        public void Initialize()
        {
            ThrowIfDisposed();

            if (hostService != null)
                throw new InvalidOperationException("The host has already been initialized.");

            InitializeImpl();

            if (hostService == null)
                throw new HostException("The subclass did not configure the host service.");
        }

        /// <inheritdoc />
        protected override void PingImpl()
        {
            ThrowIfNotInitialized();

            try
            {
                hostService.Ping();
            }
            catch (Exception ex)
            {
                throw new RemotingException(Resources.RemoteHost_RemoteException, ex);
            }
        }

        /// <inheritdoc />
        protected override void DoCallbackImpl(CrossAppDomainDelegate callback)
        {
            ThrowIfNotInitialized();

            try
            {
                hostService.DoCallback(callback);
            }
            catch (Exception ex)
            {
                throw new RemotingException(Resources.RemoteHost_RemoteException, ex);
            }
        }

        /// <inheritdoc />
        protected override ObjectHandle CreateInstanceImpl(string assemblyName, string typeName)
        {
            ThrowIfNotInitialized();

            try
            {
                return hostService.CreateInstance(assemblyName, typeName);
            }
            catch (Exception ex)
            {
                throw new RemotingException(Resources.RemoteHost_RemoteException, ex);
            }
        }

        /// <inheritdoc />
        protected override ObjectHandle CreateInstanceFromImpl(string assemblyPath, string typeName)
        {
            ThrowIfNotInitialized();

            try
            {
                return hostService.CreateInstanceFrom(assemblyPath, typeName);
            }
            catch (Exception ex)
            {
                throw new RemotingException(Resources.RemoteHost_RemoteException, ex);
            }
        }

        /// <inheritdoc />
        protected override void InitializeRuntimeImpl(RuntimeFactory runtimeFactory, RuntimeSetup runtimeSetup, ILogger logger)
        {
            ThrowIfNotInitialized();

            try
            {
                hostService.InitializeRuntime(runtimeFactory, runtimeSetup, new RemoteLogger(logger));
            }
            catch (Exception ex)
            {
                throw new RemotingException(Resources.RemoteHost_RemoteException, ex);
            }
        }

        /// <inheritdoc />
        protected override void ShutdownRuntimeImpl()
        {
            ThrowIfNotInitialized();

            try
            {
                hostService.ShutdownRuntime();
            }
            catch (Exception ex)
            {
                throw new RemotingException(Resources.RemoteHost_RemoteException, ex);
            }
        }

        /// <summary>
        /// Disposes the remote host.
        /// </summary>
        /// <param name="disposing">True if disposing</param>
        protected override void Dispose(bool disposing)
        {
            StopPingTimer();

            try
            {
                if (disposing)
                {
                    if (hostService != null)
                        hostService.Dispose();
                }
            }
            catch (RemotingException)
            {
                // Ignore remoting exceptions that are probably just signalling that
                // the remote link has already been severed.
            }
            catch (Exception ex)
            {
                UnhandledExceptionPolicy.Report("Could not send Dispose message to remote host service.", ex);
            }
            finally
            {
                hostService = null;
                base.Dispose(disposing);
            }
        }

        private void ThrowIfNotInitialized()
        {
            if (hostService == null)
                throw new InvalidOperationException("The host has not been initialized.");
        }

        /// <summary>
        /// Initializes the host.
        /// Must call <see cref="ConfigureHostService"/> to configure the host service.
        /// </summary>
        protected abstract void InitializeImpl();

        /// <summary>
        /// Configures the host service parameters.
        /// </summary>
        /// <param name="hostService">The remote host service</param>
        /// <param name="pingInterval">The automatic ping interval, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostService"/> is null</exception>
        protected void ConfigureHostService(IHostService hostService, TimeSpan? pingInterval)
        {
            if (hostService == null)
                throw new ArgumentNullException("hostService");

            this.hostService = hostService;
            this.pingInterval = pingInterval;

            StartPingTimer();
        }

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
    }
}
