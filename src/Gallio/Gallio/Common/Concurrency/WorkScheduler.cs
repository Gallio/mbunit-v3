// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Gallio.Common.Policies;

namespace Gallio.Common.Concurrency
{
    /// <summary>
    /// Schedules actions to be run in parallel up to a specified (variable)
    /// maximum number of threads.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The implementation is designed to support re-entrance while still maintaining an
    /// upper bound on active thread count.  To avoid blowing the limit on thread count
    /// due to re-entrance the caller's thread is also used to schedule work.
    /// </para>
    /// <para>
    /// The work scheduler supports dynamically adjusting the degree of parallelism
    /// as new work is added.
    /// </para>
    /// </remarks>
    public sealed class WorkScheduler
    {
        private readonly object syncRoot = new object();

        private readonly DegreeOfParallelismProvider degreeOfParallelismProvider;

        private volatile int activeThreads = 1;

        /// <summary>
        /// Creates a work scheduler.
        /// </summary>
        /// <param name="degreeOfParallelismProvider">A function that determines
        /// the current degree of parallelism which may change over time.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="degreeOfParallelismProvider"/>
        /// is null.</exception>
        public WorkScheduler(DegreeOfParallelismProvider degreeOfParallelismProvider)
        {
            if (degreeOfParallelismProvider == null)
                throw new ArgumentNullException("degreeOfParallelismProvider");

            this.degreeOfParallelismProvider = degreeOfParallelismProvider;
        }

        /// <summary>
        /// Runs a set of actions in parallel up to the current degree of parallelism.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The implementation guarantees that the current thread will only be used to
        /// run actions from the provided work set; it will not be used to run actions
        /// from other concurrently executing work sets.  Other threads will be used
        /// to run actions from this work set and from other work sets in the order in
        /// which they were enqueued.  Thus this function will return as soon as all
        /// of the specified actions have completed even if other work sets have
        /// pending work.
        /// </para>
        /// </remarks>
        /// <param name="actions">The actions to run.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="actions"/> is null
        /// or contains null.</exception>
        [DebuggerHidden]
        public void Run(IEnumerable<Action> actions)
        {
            if (actions == null)
                throw new ArgumentNullException("actions");

            int localActiveThreads = 0;

            foreach (var action in actions)
            {
                // Get block local copy for when it gets captured in lambda below
                var nextAction = action;

                if (this.activeThreads >= this.GetDegreeOfParallelism())
                {
                    try
                    {
                        nextAction();
                    }
                    catch (Exception ex)
                    {
                        ReportUnhandledException(ex);
                    }
                }
                else
                {
                    lock (this.syncRoot)
                    {
                        while (this.activeThreads >= this.GetDegreeOfParallelism())
                        {
                            Monitor.Wait(this.syncRoot);
                        }

                        this.activeThreads++;
                        localActiveThreads++;
                    }

                    ThreadPool.QueueUserWorkItem(state =>
                    {
                        try
                        {
                            nextAction();
                        }
                        catch (Exception ex)
                        {
                            ReportUnhandledException(ex);
                        }
                        finally
                        {
                            lock (this.syncRoot)
                            {
                                this.activeThreads--;
                                localActiveThreads--;

                                Monitor.PulseAll(this.syncRoot);
                            }
                        }
                    });
                }
            }

            lock (this.syncRoot)
            {
                while (localActiveThreads > 0)
                {
                    Monitor.Wait(this.syncRoot);
                }
            }
        }

        private int GetDegreeOfParallelism()
        {
            return Math.Max(degreeOfParallelismProvider(), 1);
        }

        private static void ReportUnhandledException(Exception ex)
        {
            UnhandledExceptionPolicy.Report("An unhandled exception occurred while running a parallelizable action.", ex);
        }
    }
}
