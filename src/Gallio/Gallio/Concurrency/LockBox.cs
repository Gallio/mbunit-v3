using System;
using System.Threading;

namespace Gallio.Concurrency
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
    public struct LockBox<T>
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
    }
}