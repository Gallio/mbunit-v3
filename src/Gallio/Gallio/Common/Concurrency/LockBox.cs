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
using System.Threading;

namespace Gallio.Common.Concurrency
{
    /// <summary>
    /// <para>
    /// A lock box object provides protected read / write access to a shared object
    /// that may be accessed concurrently by multiple threads.
    /// </para>
    /// <para>
    /// Clients are expected to use the <see cref="Read" /> and <see cref="Write" /> methods
    /// to acquire and release a lock of the appropriate type prior to manipulating
    /// the contents.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of object inside the lock box</typeparam>
    public class LockBox<T>
    {
        private readonly T obj;
        private readonly ReaderWriterLock @lock;

        /// <summary>
        /// Creates a lock box for the specified object.
        /// </summary>
        /// <param name="obj">The object to protect</param>
        public LockBox(T obj)
        {
            this.obj = obj;
            @lock = new ReaderWriterLock();
        }

        /// <summary>
        /// Acquires a read lock and invokes the action with the object inside the lock box.
        /// </summary>
        /// <param name="action">The action to invoke</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        public void Read(ReadAction<T> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            @lock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                action(obj);
            }
            finally
            {
                @lock.ReleaseReaderLock();
            }
        }

        /// <summary>
        /// Acquires a write lock and invokes the action with the object inside the lock box.
        /// </summary>
        /// <param name="action">The action to invoke</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        public void Write(WriteAction<T> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            @lock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                action(obj);
            }
            finally
            {
                @lock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Acquires a read lock and invokes the function with the object inside the lock box.
        /// </summary>
        /// <param name="func">The action to invoke</param>
        /// <returns>The value returned by the function</returns>
        /// <typeparam name="TResult">The function result type</typeparam>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="func"/> is null</exception>
        public TResult Read<TResult>(ReadFunc<T, TResult> func)
        {
            if (func == null)
                throw new ArgumentNullException("func");

            @lock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                return func(obj);
            }
            finally
            {
                @lock.ReleaseReaderLock();
            }
        }

        /// <summary>
        /// Acquires a write lock and invokes the function with the object inside the lock box.
        /// </summary>
        /// <param name="func">The action to invoke</param>
        /// <returns>The value returned by the function</returns>
        /// <typeparam name="TResult">The function result type</typeparam>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="func"/> is null</exception>
        public TResult Write<TResult>(WriteFunc<T, TResult> func)
        {
            if (func == null)
                throw new ArgumentNullException("func");

            @lock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                return func(obj);
            }
            finally
            {
                @lock.ReleaseWriterLock();
            }
        }
    }
}
