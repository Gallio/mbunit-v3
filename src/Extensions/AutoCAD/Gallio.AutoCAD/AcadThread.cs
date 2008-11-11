using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Gallio.Utilities;

namespace Gallio.AutoCAD
{
    /// <summary>
    /// Serializes calls to the AutoCAD UI thread.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The AutoCAD API can only be accessed by a single thread at a time.
    /// Furthermore, parts of it (notably <c>AcDbObject</c> destructors) can
    /// only be accessed from the thread AutoCAD runs commands, reactors or
    /// custom objects on.
    /// </para>
    /// <para>
    /// The <c>AcadThread</c> type allows the AutoCAD thread to be blocked
    /// by calling <see cref="Run"/> and then used to execute arbitrary code
    /// through delegates passed by other threads to <see cref="Invoke"/>.
    /// The AutoCAD thread remains blocked until another thread calls the
    /// <see cref="Shutdown"/> method.
    /// </para>
    /// </remarks>
    public class AcadThread : IDisposable
    {
        private static class State
        {
            public const int Starting = 0;
            public const int Processing = 1;
            public const int Shutdown = 2;
        }

        private LinkedList<ThreadEntry> queuedThreads;
        private ManualResetEvent workReady;
        private int state;
        private Thread waitThread;

        /// <summary>
        /// Initializes a new instance of <see cref="AcadThread"/>.
        /// </summary>
        public AcadThread()
        {
            queuedThreads = new LinkedList<ThreadEntry>();
            workReady = new ManualResetEvent(false);
            state = State.Starting;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the <see cref="AcadThread"/>.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (workReady != null)
                {
                    workReady.Close();
                    workReady = null;
                }
            }
        }

        /// <summary>
        /// Calls the specified delegate method on the AutoCAD UI thread.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="callback"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If the AutoCAD thread is not currently executing
        /// in the <see cref="Run"/> method.
        /// </exception>
        public object Invoke(Delegate callback, params object[] args)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            if (state == State.Processing)
            {
                if (!InvokeRequired)
                {
                    return ExecuteDelegate(callback, args);
                }
                else
                {
                    ThreadEntry entry = new ThreadEntry(callback, true, args);
                    EnqueueThread(entry);
                    entry.waitHandle.WaitOne();
                    if (entry.exception != null)
                    {
                        throw entry.exception;
                    }
                    return entry.retVal;
                }
            }

            throw new InvalidOperationException("The processing thread is not available.");
        }

        /// <summary>
        /// Returns <c>true</c> if the calling thread is same thread currently executing
        /// in the <see cref="Run"/> method; otherwise, <c>false</c>.
        /// </summary>
        public bool InvokeRequired
        {
            get
            {
                return state == State.Processing && waitThread != Thread.CurrentThread;
            }
        }

        /// <summary>
        /// Tells the <see cref="AcadThread"/> to stop servicing <see cref="Invoke"/> requests
        /// and to stop blocking the thread executing in the <see cref="Run"/> method.
        /// </summary>
        public void Shutdown()
        {
            if (state != State.Shutdown)
            {
                if (Interlocked.Exchange(ref state, State.Shutdown) != State.Shutdown)
                {
                    EnqueueThread(new ThreadEntry(new Action(delegate() { /* no-op */ }), false));
                }
            }
        }

        /// <summary>
        /// <para>
        /// Begins processing incoming <see cref="Invoke"/> calls on the calling thread.
        /// </para>
        /// <para>
        /// The calling thread is blocked until <see cref="Shutdown"/> is called.
        /// </para>
        /// </summary>
        /// <devnote>
        /// We might consider passing a callback in here that we could hand the AutoCAD
        /// thread off to every once in a while. It could be used to pump messages for
        /// AutoCAD, update a progress bar, check for acedUsrBrk(), etc.
        /// </devnote>
        public void Run()
        {
            if (state == State.Shutdown)
                return;

            if (waitThread == null)
            {
                Interlocked.CompareExchange<Thread>(ref waitThread, Thread.CurrentThread, null);
            }
            if (waitThread != Thread.CurrentThread)
            {
                throw new InvalidOperationException("Already running on a different thread.");
            }

            try
            {
                WaitForIncomingThreads();
            }
            finally
            {
                ProcessOrphanedThreads(queuedThreads);
                waitThread = null;
            }
        }

        private void WaitForIncomingThreads()
        {
            for (Interlocked.CompareExchange(ref state, State.Processing, State.Starting);
                state == State.Processing;
                workReady.WaitOne())
            {
                while (state == State.Processing)
                {
                    var nextThread = DeqeueueThread();
                    if (nextThread == null)
                        break;
                    ProcessThread(nextThread);
                }

                lock (queuedThreads)
                {
                    if (state == State.Processing && queuedThreads.Count == 0)
                    {
                        workReady.Reset();
                    }
                }
            }
        }

        private void EnqueueThread(ThreadEntry workItem)
        {
            lock (queuedThreads)
            {
                queuedThreads.AddLast(workItem);
            }

            workReady.Set();
        }

        // NB: Any dequeued ThreadEntry *must* be acted upon.
        //     There is a thread out there waiting for the
        //     entry to be either processed or orphaned. Either
        //     way, we can't leave that thread waiting indefinitely.
        private ThreadEntry DeqeueueThread()
        {
            lock (queuedThreads)
            {
                if (queuedThreads.Count == 0)
                    return null;
                
                var retVal = queuedThreads.First.Value;
                queuedThreads.RemoveFirst();
                return retVal;
            }
        }

        private static void ProcessThread(ThreadEntry entry)
        {
            try
            {
                entry.retVal = ExecuteDelegate(entry.callback, entry.args);
            }
            catch (Exception e)
            {
                entry.exception = e;
            }
            finally
            {
                entry.waitHandle.Set();
            }
        }

        private static object ExecuteDelegate(Delegate callback, object[] args)
        {
            try
            {
                return callback.DynamicInvoke(args);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionUtils.RethrowWithNoStackTraceLoss(ex.InnerException);

                // Should never reach this.
                throw;
            }
        }

        private static void ProcessOrphanedThreads(LinkedList<ThreadEntry> entries)
        {
            for (; entries.Count > 0; entries.RemoveFirst())
            {
                var entry = entries.First.Value;
                entry.exception = new ThreadInterruptedException("The processing thread has been shut down.");
                entry.waitHandle.Set();
            }
        }

        private class ThreadEntry
        {
            public ThreadEntry(Delegate callback, bool synchronous, params object[] args)
            {
                waitHandle = new ManualResetEvent(!synchronous);
                this.callback = callback;
                this.args = args;
            }

            internal ManualResetEvent waitHandle;
            internal Delegate callback;
            internal object[] args;
            internal object retVal;
            internal Exception exception;
        }
    }
}
