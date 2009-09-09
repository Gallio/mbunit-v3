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
