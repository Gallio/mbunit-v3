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
using System.Runtime.Remoting;
using System.Threading;
using Castle.Core.Logging;
using Gallio.Properties;

namespace Gallio.Hosting
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
    public abstract class RemoteHost : IHost
    {
        private readonly HostSetup hostSetup;
        private bool isDisposed;
        private IHostService hostService;
        private TimeSpan? pingInterval;

        private readonly object pingLock = new object();
        private Timer pingTimer;

        /// <summary>
        /// Creates an uninitialized host.
        /// </summary>
        /// <param name="hostSetup">The host setup</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostSetup"/> is null</exception>
        protected RemoteHost(HostSetup hostSetup)
        {
            if (hostSetup == null)
                throw new ArgumentNullException("hostSetup");

            this.hostSetup = hostSetup;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
        public HostSetup GetHostSetup()
        {
            return hostSetup.Copy();
        }

        /// <inheritdoc />
        public void Ping()
        {
            ThrowIfDisposed();
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
        public void DoCallback(CrossAppDomainDelegate callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            ThrowIfDisposed();
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
        public ObjectHandle CreateInstance(string assemblyName, string typeName)
        {
            if (assemblyName == null)
                throw new ArgumentNullException("assemblyName");
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            ThrowIfDisposed();
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
        public ObjectHandle CreateInstanceFrom(string assemblyPath, string typeName)
        {
            if (assemblyPath == null)
                throw new ArgumentNullException("assemblyPath");
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            ThrowIfDisposed();
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
        public void InitializeRuntime(RuntimeSetup runtimeSetup, ILogger logger)
        {
            if (runtimeSetup == null)
                throw new ArgumentNullException("runtimeSetup");
            if (logger == null)
                throw new ArgumentNullException("logger");

            ThrowIfDisposed();
            ThrowIfNotInitialized();

            try
            {
                hostService.InitializeRuntime(runtimeSetup, RemoteLogger.Wrap(logger));
            }
            catch (Exception ex)
            {
                throw new RemotingException(Resources.RemoteHost_RemoteException, ex);
            }
        }

        /// <inheritdoc />
        public void ShutdownRuntime()
        {
            ThrowIfDisposed();
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
        protected virtual void Dispose(bool disposing)
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
                Panic.UnhandledException("Could not send Dispose message to remote host service.", ex);
            }
            finally
            {
                hostService = null;
                isDisposed = true;
            }
        }

        private void ThrowIfDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        private void ThrowIfNotInitialized()
        {
            if (hostService == null)
                throw new InvalidOperationException("The host has not been initialized.");
        }

        /// <summary>
        /// Gets the internal host setup information without copying it.
        /// </summary>
        protected HostSetup HostSetup
        {
            get { return hostSetup; }
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
            try
            {
                if (hostService != null)
                    hostService.Ping();
            }
            catch (Exception ex)
            {
                Panic.UnhandledException("Could not send Ping message to the remote host service.", ex);
            }
        }
    }
}
