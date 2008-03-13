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
using System.Runtime.CompilerServices;
using System.Threading;

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
        private const int QuiescentState = 0;
        private const int RunningState = 1;
        private const int AbortingState = 2;
        private const int AbortedState = 3;

        private Thread thread;
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
        public ThreadAbortException Run(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (Interlocked.CompareExchange(ref thread, Thread.CurrentThread, null) != null)
                throw new InvalidOperationException("At most one action may be active within the scope at a time.");
            try
            {
                RunWithThreadAbort(action);
                return null;
            }
            catch (ThreadAbortException ex)
            {
                if (ex.ExceptionState == this)
                {
                    Thread.ResetAbort();
                    Thread.VolatileWrite(ref state, AbortedState);
                    return ex;
                }

                throw;
            }
            finally
            {
                thread = null;
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
                if (Interlocked.CompareExchange(ref state, AbortingState, RunningState) == RunningState)
                {
                    thread.Abort(this);
                    return;
                }
            }
        }

        /// <summary>
        /// Runs the action and guarantees that if an abort will occur, it must occur within this
        /// block and nowhere else.
        /// </summary>
        private void RunWithThreadAbort(Action action)
        {
            try
            {
                RuntimeHelpers.PrepareConstrainedRegions();
                try // must immediately follow PrepareConstrainedRegions
                {
                    // Preempt the action if it has been previously aborted.
                    int oldState = Interlocked.CompareExchange(ref state, RunningState, QuiescentState);
                    if (oldState == AbortedState)
                        thread.Abort(this);

                    // Run the action.
                    action();
                }
                finally
                {
                    // Reset the state.
                    // This is guaranteed to run even if an asynchronous exception occurs because it is in a constrained region.
                    Interlocked.CompareExchange(ref state, QuiescentState, RunningState);
                }

                WaitForThreadAbortIfAborting();
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception)
            {
                WaitForThreadAbortIfAborting();
                throw;
            }
        }

        /// <summary>
        /// Because a thread abort is delivered asynchronously, we need to synchronize with
        /// it to ensure that it gets delivered.  Otherwise the pending thread abort exception
        /// could occur outside of our special region.
        /// </summary>
        private void WaitForThreadAbortIfAborting()
        {
            while (Thread.VolatileRead(ref state) == AbortingState)
                Thread.Sleep(0);
        }
    }
}
