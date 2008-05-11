// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Runtime.Remoting;

namespace Gallio.Runtime.Hosting
{
    /// <summary>
    /// Base implementation of <see cref="IHostService"/> that performs argument validation.
    /// </summary>
    public abstract class BaseHostService : LongLivedMarshalByRefObject, IHostService, IDisposable
    {
        private bool isDisposed;

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public void Ping()
        {
            ThrowIfDisposed();
            PingImpl();
        }

        /// <inheritdoc />
        public TResult Do<TArg, TResult>(Func<TArg, TResult> func, TArg arg)
        {
            if (func == null)
                throw new ArgumentNullException("func");

            ThrowIfDisposed();
            return DoImpl(func, arg);
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

        /// <summary>
        /// Internal implementation of <see cref="Do"/>.
        /// </summary>
        /// <param name="func">The action to perform, not null</param>
        /// <param name="arg">The argument value, if any</param>
        /// <returns>The result value, if any</returns>
        /// <typeparam name="TArg">The argument type</typeparam>
        /// <typeparam name="TResult">The result type</typeparam>
        protected virtual TResult DoImpl<TArg, TResult>(Func<TArg, TResult> func, TArg arg)
        {
            return func(arg);
        }

        /// <summary>
        /// Internal implementation of <see cref="Ping"/>.
        /// </summary>
        protected virtual void PingImpl()
        {
        }

        /// <summary>
        /// Internal implementation of <see cref="CreateInstance"/>.
        /// </summary>
        /// <param name="assemblyName">The assembly name, not null</param>
        /// <param name="typeName">The type name, not null</param>
        /// <returns>The created object handle</returns>
        protected virtual ObjectHandle CreateInstanceImpl(string assemblyName, string typeName)
        {
            return Activator.CreateInstance(assemblyName, typeName);
        }

        /// <summary>
        /// Internal implementation of <see cref="CreateInstanceFrom" />.
        /// </summary>
        /// <param name="assemblyPath">The assembly path, not null</param>
        /// <param name="typeName">The type name, not null</param>
        /// <returns>The created object handle</returns>
        protected virtual ObjectHandle CreateInstanceFromImpl(string assemblyPath, string typeName)
        {
            return Activator.CreateInstanceFrom(assemblyPath, typeName);
        }

        /// <summary>
        /// Disposes the host service.
        /// </summary>
        /// <param name="disposing">True if disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            isDisposed = true;
        }

        /// <summary>
        /// Throws an exception if the host service has been disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }
    }
}
