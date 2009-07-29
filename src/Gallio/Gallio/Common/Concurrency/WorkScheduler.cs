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
        private readonly Queue<WorkSet> pendingWorkSets;
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

            pendingWorkSets = new Queue<WorkSet>();
        }

        /// <summary>
        /// Runs a set of actions in parallel up to the current degree of parallelism.
        /// </summary>
        /// <param name="actions">The actions to run.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="actions"/> is null
        /// or contains null.</exception>
        [DebuggerHidden]
        public void Run(IEnumerable<Action> actions)
        {
            // NOTE: This method has been optimized to minimize the total stack depth of the action
            //       by inlining blocks on the critical path that had previously been factored out.

            if (actions == null)
                throw new ArgumentNullException("actions");

            var actionQueue = new Queue<Action>();
            foreach (Action action in actions)
            {
                if (action == null)
                    throw new ArgumentNullException("actions");
                actionQueue.Enqueue(action);
            }

            WorkSet workSet;
            lock (syncRoot)
            {
                workSet = new WorkSet(actionQueue);
                pendingWorkSets.Enqueue(workSet);
            }

            for (; ; )
            {
                Action nextAction;

                lock (syncRoot)
                {
                    if (!workSet.SyncHasPendingActions())
                    {
                        if (! workSet.SyncHasActionsInProgress())
                            return;

                        Monitor.Wait(syncRoot);
                        continue;
                    }
                    else
                    {
                        nextAction = workSet.SyncPrepareNextAction();

                        if (workSet.SyncHasPendingActions())
                        {
                            if (activeThreads < GetDegreeOfParallelism())
                            {
                                activeThreads += 1;
                                ThreadPool.QueueUserWorkItem(BackgroundActionLoop);
                            }
                        }
                    }
                }

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
                    lock (syncRoot)
                    {
                        workSet.SyncActionFinished();
                        Monitor.PulseAll(syncRoot);
                    }
                }
            }
        }

        private int GetDegreeOfParallelism()
        {
            return Math.Max(degreeOfParallelismProvider(), 1);
        }

        [DebuggerHidden]
        private void BackgroundActionLoop(object dummy)
        {
            // NOTE: This method has been optimized to minimize the total stack depth of the action
            //       by inlining blocks on the critical path that had previously been factored out.

            for (; ; )
            {
                WorkSet nextWorkSet;
                Action nextAction;

                lock (syncRoot)
                {
                    for (; ; )
                    {
                        if (pendingWorkSets.Count == 0
                            || activeThreads > GetDegreeOfParallelism())
                        {
                            activeThreads -= 1;
                            return;
                        }

                        nextWorkSet = pendingWorkSets.Peek();
                        if (!nextWorkSet.SyncHasPendingActions())
                        {
                            pendingWorkSets.Dequeue();
                        }
                        else
                        {
                            nextAction = nextWorkSet.SyncPrepareNextAction();
                            break;
                        }
                    }
                }

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
                    lock (syncRoot)
                    {
                        nextWorkSet.SyncActionFinished();
                        Monitor.PulseAll(syncRoot);
                    }
                }
            }
        }

        private static void ReportUnhandledException(Exception ex)
        {
            UnhandledExceptionPolicy.Report("An unhandled exception occurred while running a parallelizable action.", ex);
        }

        private sealed class WorkSet
        {
            private readonly Queue<Action> actionQueue;
            private int actionsInProgress;

            public WorkSet(Queue<Action> actionQueue)
            {
                this.actionQueue = actionQueue;
            }

            public bool SyncHasPendingActions()
            {
                return actionQueue.Count != 0;
            }

            public bool SyncHasActionsInProgress()
            {
                return actionsInProgress != 0;
            }

            public Action SyncPrepareNextAction()
            {
                Action action = actionQueue.Dequeue();
                actionsInProgress += 1;
                return action;
            }

            public void SyncActionFinished()
            {
                actionsInProgress -= 1;
            }
        }
    }
}
