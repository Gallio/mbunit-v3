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

namespace Gallio.Hosting
{
    /// <summary>
    /// <para>
    /// An implementation of <see cref="IHost" /> intended to be accessed
    /// via .Net remoting from some other application context.
    /// </para>
    /// <para>
    /// This implementation wraps a <see cref="IRemoteHostService" /> with additional
    /// exception handling code and sends periodic heartbeat ping message.
    /// </para>
    /// </summary>
    public class RemoteHost : IHost
    {
        private IRemoteHostService remoteHostService;
        private readonly TimeSpan? pingInterval;

        private readonly object pingLock = new object();
        private Timer pingTimer;

        /// <summary>
        /// Creates a remote host as a wrapper of the specified 
        /// </summary>
        /// <param name="remoteHostService">The remote host service</param>
        /// <param name="pingInterval">The automatic ping interval, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="remoteHostService"/> is null</exception>
        public RemoteHost(IRemoteHostService remoteHostService, TimeSpan? pingInterval)
        {
            if (remoteHostService == null)
                throw new ArgumentNullException("remoteHostService");

            this.remoteHostService = remoteHostService;
            this.pingInterval = pingInterval;

            StartPingTimer();
        }

        /// <summary>
        /// Disposes of the host.
        /// </summary>
        ~RemoteHost()
        {
            Dispose(false);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public object Activate(string assemblyName, string typeName)
        {
            ThrowIfDisposed();

            try
            {
                return remoteHostService.Activate(assemblyName, typeName).Unwrap();
            }
            catch (Exception ex)
            {
                throw new RemotingException("The remote host service threw an exception or could not be reached.", ex);
            }
        }

        /// <inheritdoc />
        public void InitializeRuntime(RuntimeSetup runtimeSetup, ILogger logger)
        {
            ThrowIfDisposed();

            try
            {
                remoteHostService.InitializeRuntime(runtimeSetup, RemoteLogger.Wrap(logger));
            }
            catch (Exception ex)
            {
                throw new RemotingException("The remote host service threw an exception or could not be reached.", ex);
            }
        }

        /// <inheritdoc />
        public void ShutdownRuntime()
        {
            ThrowIfDisposed();

            try
            {
                remoteHostService.ShutdownRuntime();
            }
            catch (Exception ex)
            {
                throw new RemotingException("The remote host service threw an exception or could not be reached.", ex);
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
                    if (remoteHostService != null)
                        remoteHostService.Dispose();
                }
            }
            catch (Exception ex)
            {
                Panic.UnhandledException("Could not send Dispose message to remote host service.", ex);
            }
            finally
            {
                remoteHostService = null;
            }
        }

        private void ThrowIfDisposed()
        {
            if (remoteHostService == null)
                throw new ObjectDisposedException(GetType().Name);
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
                if (remoteHostService != null)
                    remoteHostService.Ping();
            }
            catch (Exception ex)
            {
                Panic.UnhandledException("Could not send Ping message to the remote host service.", ex);
            }
        }
    }
}
