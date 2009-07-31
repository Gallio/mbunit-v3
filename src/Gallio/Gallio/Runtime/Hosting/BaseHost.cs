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
using System.Diagnostics;
using Gallio.Common.Policies;
using Gallio.Runtime.Debugging;
using Gallio.Runtime.Logging;

namespace Gallio.Runtime.Hosting
{
    /// <summary>
    /// Base implementation of <see cref="IHost"/> that performs argument validation.
    /// </summary>
    public abstract class BaseHost : IHost
    {
        private readonly object syncRoot = new object();

        private readonly HostSetup hostSetup;
        private readonly ILogger logger;
        private IHostService hostService;
        private event EventHandler disconnectedHandlers;

        private bool wasInitialized;
        private bool isDisposed;

        private IDebugger debugger;
        private Process debuggedProcess;

        /// <summary>
        /// Creates a host.
        /// </summary>
        /// <param name="hostSetup">The host setup.</param>
        /// <param name="logger">The logger for host message output.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostSetup"/>
        /// or <paramref name="logger"/> is null.</exception>
        protected BaseHost(HostSetup hostSetup, ILogger logger)
        {
            if (hostSetup == null)
                throw new ArgumentNullException("hostSetup");
            if (logger == null)
                throw new ArgumentNullException("logger");

            this.hostSetup = hostSetup;
            this.logger = logger;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public event EventHandler Disconnected
        {
            add
            {
                lock (syncRoot)
                {
                    ThrowIfDisposed();

                    if (hostService != null)
                    {
                        disconnectedHandlers += value;
                        return;
                    }
                }

                EventHandlerPolicy.SafeInvoke(value, this, EventArgs.Empty);
            }
            remove
            {
                lock (syncRoot)
                {
                    ThrowIfDisposed();

                    disconnectedHandlers -= value;
                }
            }
        }

        /// <inheritdoc />
        public abstract bool IsLocal { get; }

        /// <inheritdoc />
        public bool IsConnected
        {
            get
            {
                lock (syncRoot)
                {
                    ThrowIfDisposed();

                    return hostService != null;
                }
            }
        }

        /// <inheritdoc />
        public HostSetup GetHostSetup()
        {
            lock (syncRoot)
            {
                ThrowIfDisposed();
                return hostSetup.Copy();
            }
        }

        /// <summary>
        /// Initializes the host and connects to the host service.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the host has already been initialized.</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the host has been disposed.</exception>
        /// <exception cref="HostException">Thrown if an exception occurred while connecting to the host.</exception>
        public void Connect()
        {
            lock (syncRoot)
            {
                ThrowIfDisposed();
                if (wasInitialized)
                    throw new InvalidOperationException("The host has already been initialized.");

                wasInitialized = true;
            }

            try
            {
                hostService = AcquireHostService();
            }
            catch (Exception ex)
            {
                throw new HostException("An exception occurred while connecting to the host service.", ex);
            }
            finally
            {
                if (hostService == null)
                    NotifyDisconnected();
            }
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            IHostService cachedHostService;
            lock (syncRoot)
            {
                ThrowIfDisposed();
                if (hostService == null)
                    return;

                cachedHostService = hostService;
            }

            if (cachedHostService != null)
            {
                try
                {
                    ReleaseHostService(cachedHostService);
                }
                catch (Exception ex)
                {
                    Logger.Log(LogSeverity.Warning, "An exception occurred while disconnecting from the host service.", ex);
                }
            }

            NotifyDisconnected();
        }

        /// <inheritdoc />
        public IHostService GetHostService()
        {
            lock (syncRoot)
            {
                ThrowIfDisposed();

                if (hostService == null)
                    throw new InvalidOperationException("The host has been disconnected.");
                return hostService;
            }
        }

        /// <summary>
        /// Gets an reference to the host service, or null if not connected.
        /// </summary>
        protected IHostService HostService
        {
            get { return hostService; }
        }

        /// <summary>
        /// Gets the internal host setup information without copying it.
        /// </summary>
        protected HostSetup HostSetup
        {
            get { return hostSetup; }
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger Logger
        {
            get { return logger; }
        }

        /// <summary>
        /// Gets the host service.
        /// </summary>
        /// <returns>The host service, or null if the host service was not available.</returns>
        protected abstract IHostService AcquireHostService();

        /// <summary>
        /// Releases the host service.
        /// </summary>
        /// <param name="hostService">The host service that is being released, not null.</param>
        protected virtual void ReleaseHostService(IHostService hostService)
        {
        }

        /// <summary>
        /// Disposes the host.
        /// </summary>
        /// <param name="disposing">True if disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                Disconnect();
                isDisposed = true;
            }
        }

        /// <summary>
        /// Throws an exception if the host has been disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        /// <summary>
        /// Sets the state of the host to disconnected and notifies clients.
        /// </summary>
        protected void NotifyDisconnected()
        {
            EventHandler cachedDisconnectedHandlers;
            lock (syncRoot)
            {
                if (hostService == null)
                    return;

                cachedDisconnectedHandlers = disconnectedHandlers;
                disconnectedHandlers = null;
                hostService = null;
            }

            EventHandlerPolicy.SafeInvoke(cachedDisconnectedHandlers, this, EventArgs.Empty);
        }

        /// <summary>
        /// Attaches the debugger to a process if the host settings require it.
        /// </summary>
        protected void AttachDebuggerIfNeeded(IDebuggerManager debuggerManager, Process debuggedProcess)
        {
            if (HostSetup.DebuggerSetup != null)
            {
                IDebugger debugger = debuggerManager.GetDebugger(HostSetup.DebuggerSetup, Logger);

                if (! Debugger.IsAttached)
                {
                    Logger.Log(LogSeverity.Important, "Attaching debugger to the host.");

                    AttachDebuggerResult result = debugger.AttachToProcess(debuggedProcess);
                    if (result == AttachDebuggerResult.Attached)
                    {
                        this.debugger = debugger;
                        this.debuggedProcess = debuggedProcess;
                    }
                    else if (result == AttachDebuggerResult.CouldNotAttach)
                    {
                        Logger.Log(LogSeverity.Warning, "Could not attach debugger to the host.");
                    }
                }
            }
        }

        /// <summary>
        /// Detaches the debugger from a process if the host settings require it.
        /// </summary>
        protected void DetachDebuggerIfNeeded()
        {
            if (debugger != null && debuggedProcess != null)
            {
                Logger.Log(LogSeverity.Important, "Detaching debugger from the host.");

                DetachDebuggerResult result = debugger.DetachFromProcess(debuggedProcess);
                if (result == DetachDebuggerResult.CouldNotDetach)
                    Logger.Log(LogSeverity.Warning, "Could not detach debugger from the host.");

                debuggedProcess = null;
                debugger = null;
            }
        }
    }
}
