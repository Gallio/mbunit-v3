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

namespace Gallio.VisualStudio.Shell.Core
{
    /// <summary>
    /// Protects readers and writers of <see cref="IShell" /> services.
    /// </summary>
    public static class ShellLock
    {
        /// <summary>
        /// An action performed while the lock is acquired.
        /// </summary>
        public delegate void ProtectedAction();

        private static readonly ReaderWriterLockSlim @lock = new ReaderWriterLockSlim();

        /// <summary>
        /// Performs a protected action with the reader lock.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
        public static void WithReaderLock(ProtectedAction action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            @lock.EnterReadLock();
            try
            {
                action();
            }
            finally
            {
                @lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Performs a protected action with the writer lock.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
        public static void WithWriterLock(ProtectedAction action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            @lock.EnterWriteLock();
            try
            {
                action();
            }
            finally
            {
                @lock.ExitWriteLock();
            }
        }
    }
}
