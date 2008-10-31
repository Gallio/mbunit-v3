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
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Gallio.Properties;
using ThreadState=System.Threading.ThreadState;

namespace Gallio.Concurrency
{
    /// <summary>
    /// <para>
    /// A <see cref="ThreadAbortScope" /> executes a block of code inside a special
    /// scope that is designed to issue and safely handle <see cref="Thread.Abort(object)" />
    /// on demand.
    /// </para>
    /// <para>
    /// This class may be used as a primitive for implementing higher-level protected
    /// scopes for the purpose of asynchronous cancelation and time-limited execution.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// The implementation distinguishes between <see cref="Thread.Abort(object)" />s that
    /// are initated using the <see cref="ThreadAbortScope.Abort" /> method and those
    /// that may be initiated by other system components.  Foreign aborts are not handled
    /// so they are allowed to continue to propagate.  This is to ensure that critical aborts
    /// such as those that are initiated by <see cref="AppDomain.Unload" /> can unwind the stack
    /// without interference.
    /// </para>
    /// </remarks>
    public class ThreadAbortScope
    {
        /// <summary>
        /// Indicates that the thread scope is not currently running abortable code.
        /// It could either not be entered, or it could be entered and running a
        /// protected block.
        /// </summary>
        private const int QuiescentState = 0;

        /// <summary>
        /// Indicates that the thread scope is running abortable code.
        /// </summary>
        private const int RunningState = 1;

        /// <summary>
        /// Indicates that <see cref="Thread.Abort(object)" /> has been requested but
        /// perhaps not yet signaled.
        /// </summary>
        private const int AbortRequestedState = 2;

        /// <summary>
        /// Indicates that the scope has been completely aborted.
        /// </summary>
        private const int AbortedState = 3;

        private object activeThread;
        private int state;

        /// <summary>
        /// Runs an action inside of the scope.
        /// </summary>
        /// <remarks>
        /// At most one action may be in progress at any time.  This method is not
        /// reentrant and cannot be called concurrently from multiple threads.
        /// </remarks>
        /// <param name="action">The action to run</param>
        /// <returns>The <see cref="ThreadAbortException" /> that was caught if the action
        /// was aborted, or null if the action completed normally</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if an action is already running in this scope</exception>
        /// <exception cref="Exception">Any other exception thrown by <paramref name="action"/> itself</exception>
        [DebuggerHidden]
        public ThreadAbortException Run(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            // First, ensure that this call is not re-entrant wrt. the current thread.
            if (Thread.VolatileRead(ref activeThread) == Thread.CurrentThread)
                throw new InvalidOperationException(Resources.ThreadAbortScope_ReentranceException);

            RuntimeHelpers.PrepareConstrainedRegions(); // must immediately precede try
            try
            {
                // Lock the scope for use by this thread only.
                if (Interlocked.CompareExchange(ref activeThread, Thread.CurrentThread, null) != null)
                    throw new InvalidOperationException(Resources.ThreadAbortScope_ReentranceException);

                // Proceed.
                return HandleThreadAbort(action);
            }
            finally
            {
                // Release the scope from this thread's control.
                Interlocked.CompareExchange(ref activeThread, null, Thread.CurrentThread);
            }
        }

        /// <summary>
        /// Runs an action inside of a protected context wherein it cannot receive
        /// a thread abort from this <see cref="ThreadAbortScope"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method enables critical system code to be protected from aborts that
        /// may affect the scope.  This call cannot be nested.
        /// </para>
        /// </remarks>
        /// <param name="action">The action to run</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        [DebuggerHidden]
        public void Protect(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            bool mustResetToRunningState = false;

            RuntimeHelpers.PrepareConstrainedRegions(); // must immediately precede try
            try
            {
                if (Thread.VolatileRead(ref activeThread) == Thread.CurrentThread)
                {
                    if (Interlocked.CompareExchange(ref state, QuiescentState, RunningState) == RunningState)
                    {
                        mustResetToRunningState = true;
                    }
                    else
                    {
                        WaitForThreadAbortIfAborting();
                    }
                }

                action();
            }
            finally
            {
                if (mustResetToRunningState)
                {
                    if (Interlocked.CompareExchange(ref state, RunningState, QuiescentState) == AbortedState)
                        Thread.CurrentThread.Abort(this);
                }
            }
        }

        /// <summary>
        /// <para>
        /// Aborts the currently running action and prevents any further actions from running
        /// inside of this scope.
        /// </para>
        /// </summary>
        public void Abort()
        {
            for (;;)
            {
                // Move from the quiescent state directly to aborted, if possible.
                // We have no work to do if we're not running.
                int oldState = Interlocked.CompareExchange(ref state, AbortedState, QuiescentState);
                if (oldState != RunningState)
                    return;

                // If we're running, try to move to the aborting state.
                // When successful, then actually perform the abort.
                if (Interlocked.CompareExchange(ref state, AbortRequestedState, RunningState) == RunningState)
                {
                    ((Thread) Thread.VolatileRead(ref activeThread)).Abort(this);
                    return;
                }
            }
        }

        /// <summary>
        /// Runs the action given that the current thread is now the active thread.
        /// </summary>
        [DebuggerHidden]
        private ThreadAbortException HandleThreadAbort(Action action)
        {
            try
            {
                // Run the action.  If an abort occurs, it will occur within this block.
                ConstrainThreadAbort(action);
                return null;
            }
            catch (ThreadAbortException ex)
            {
                // Unwind the aborted state.
                if (ex.ExceptionState == this)
                {
                    Thread.ResetAbort();
                    Thread.VolatileWrite(ref state, AbortedState);
                    return ex;
                }

                throw;
            }
        }

        /// <summary>
        /// Runs the action and guarantees that if an abort will occur, it must occur
        /// within this block and nowhere else.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method works by atomically modifying the state of the scope such that
        /// it can only transition into <see cref="AbortRequestedState"/> 
        /// </para>
        /// </remarks>
        [DebuggerHidden]
        private void ConstrainThreadAbort(Action action)
        {
            try
            {
                RuntimeHelpers.PrepareConstrainedRegions(); // must immediately precede try
                try
                {
                    // Preempt the action if it has been previously aborted.
                    int oldState = Interlocked.CompareExchange(ref state, RunningState, QuiescentState);
                    if (oldState == AbortedState)
                        Thread.CurrentThread.Abort(this);

                    // Run the action.
                    action();
                }
                finally
                {
                    // Reset the state.
                    Interlocked.CompareExchange(ref state, QuiescentState, RunningState);
                }
            }
            finally
            {
                WaitForThreadAbortIfAborting();
            }
        }

        /// <summary>
        /// Because a thread abort is delivered asynchronously, we need to synchronize with
        /// it to ensure that it gets delivered.  Otherwise the pending thread abort exception
        /// could occur outside of our special region.
        /// </summary>
        [DebuggerHidden]
        private void WaitForThreadAbortIfAborting()
        {
            while (Thread.VolatileRead(ref state) == AbortRequestedState)
            {
                if (Thread.CurrentThread.ThreadState == ThreadState.AbortRequested)
                    Thread.Sleep(0);
                else
                    Thread.CurrentThread.Abort(this);
            }
        }
    }
}
