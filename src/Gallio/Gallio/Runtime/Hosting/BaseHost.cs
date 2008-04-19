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
using Gallio.Runtime.Logging;

namespace Gallio.Runtime.Hosting
{
    /// <summary>
    /// Base implementation of <see cref="IHost"/> that performs argument validation.
    /// </summary>
    public abstract class BaseHost : IHost
    {
        private readonly HostSetup hostSetup;
        private readonly ILogger logger;
        private bool isDisposed;

        /// <summary>
        /// Creates a host.
        /// </summary>
        /// <param name="hostSetup">The host setup</param>
        /// <param name="logger">The logger for host message output</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostSetup"/>
        /// or <paramref name="logger"/> is null</exception>
        protected BaseHost(HostSetup hostSetup, ILogger logger)
        {
            if (hostSetup == null)
                throw new ArgumentNullException("hostSetup");
            if (logger == null)
                throw new ArgumentNullException("logger");

            this.hostSetup = hostSetup;
            this.logger = logger;
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

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public HostSetup GetHostSetup()
        {
            return hostSetup.Copy();
        }

        /// <inheritdoc />
        public abstract bool IsLocal { get; }

        /// <inheritdoc />
        public void DoCallback(CrossAppDomainDelegate callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            ThrowIfDisposed();
            DoCallbackImpl(callback);
        }

        /// <inheritdoc />
        public void Ping()
        {
            ThrowIfDisposed();
            PingImpl();
        }

        /// <inheritdoc />
        public ObjectHandle CreateInstance(string assemblyName, string typeName)
        {
            if (assemblyName == null)
                throw new ArgumentNullException("assemblyName");
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            ThrowIfDisposed();
            return CreateInstanceImpl(assemblyName, typeName);
        }

        /// <inheritdoc />
        public ObjectHandle CreateInstanceFrom(string assemblyPath, string typeName)
        {
            if (assemblyPath == null)
                throw new ArgumentNullException("assemblyPath");
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            ThrowIfDisposed();
            return CreateInstanceFromImpl(assemblyPath, typeName);
        }

        /// <inheritdoc />
        public void InitializeRuntime(RuntimeFactory runtimeFactory, RuntimeSetup runtimeSetup, ILogger logger)
        {
            if (runtimeFactory == null)
                throw new ArgumentNullException("runtimeFactory");
            if (runtimeSetup == null)
                throw new ArgumentNullException("runtimeSetup");
            if (logger == null)
                throw new ArgumentNullException("logger");

            ThrowIfDisposed();
            InitializeRuntimeImpl(runtimeFactory, runtimeSetup, logger);
        }

        /// <inheritdoc />
        public void ShutdownRuntime()
        {
            ThrowIfDisposed();
            ShutdownRuntimeImpl();
        }

        /// <summary>
        /// Internal implementation of <see cref="DoCallback"/>.
        /// </summary>
        /// <param name="callback">The callback to invoke within the host, not null</param>
        protected abstract void DoCallbackImpl(CrossAppDomainDelegate callback);

        /// <summary>
        /// Internal implementation of <see cref="Ping"/>.
        /// </summary>
        protected abstract void PingImpl();

        /// <summary>
        /// Internal implementation of <see cref="CreateInstance"/>.
        /// </summary>
        /// <param name="assemblyName">The assembly name, not null</param>
        /// <param name="typeName">The type name, not null</param>
        /// <returns>The created object handle</returns>
        protected abstract ObjectHandle CreateInstanceImpl(string assemblyName, string typeName);

        /// <summary>
        /// Internal implementation of <see cref="CreateInstanceFrom" />.
        /// </summary>
        /// <param name="assemblyPath">The assembly path, not null</param>
        /// <param name="typeName">The type name, not null</param>
        /// <returns>The created object handle</returns>
        protected abstract ObjectHandle CreateInstanceFromImpl(string assemblyPath, string typeName);

        /// <summary>
        /// Internal implementation of <see cref="InitializeRuntime" />.
        /// </summary>
        /// <param name="runtimeFactory">The runtime factory, not null</param>
        /// <param name="runtimeSetup">The runtime setup, not null</param>
        /// <param name="logger">The logger, not null</param>
        protected abstract void InitializeRuntimeImpl(RuntimeFactory runtimeFactory, RuntimeSetup runtimeSetup, ILogger logger);

        /// <summary>
        /// Internal implementation of <see cref="ShutdownRuntime" />.
        /// </summary>
        protected abstract void ShutdownRuntimeImpl();

        /// <summary>
        /// Disposes the host.
        /// </summary>
        /// <param name="disposing">True if disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            isDisposed = true;
        }

        /// <summary>
        /// Throws an exception if the host has been disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }
    }
}
