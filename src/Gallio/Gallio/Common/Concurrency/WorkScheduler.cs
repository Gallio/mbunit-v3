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

        // Invariant: This list only contains work sets that have at least one pending action.
        private readonly LinkedList<WorkSet> pendingWorkSets;

        private readonly DegreeOfParallelismProvider degreeOfParallelismProvider;

        private volatile int activeThreads;

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

            pendingWorkSets = new LinkedList<WorkSet>();
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
            // NOTE: This method has been optimized to minimize the total stack depth of the action
            //       by inlining blocks on the critical path that had previously been factored out.

            if (actions == null)
                throw new ArgumentNullException("actions");

            // Copy the queue of actions to process from the enumeration.
            var actionQueue = new Queue<Action>();
            foreach (Action action in actions)
            {
                if (action == null)
                    throw new ArgumentNullException("actions");
                actionQueue.Enqueue(action);
            }

            // Short-circuit when no actions to run to satisfy our invariant for pending work sets.
            if (actionQueue.Count == 0)
                return;

            // Add this work set to the list of pending work sets.
            WorkSet workSet;
            lock (syncRoot)
            {
                workSet = new WorkSet(actionQueue);

                pendingWorkSets.AddLast(workSet);
                activeThreads += 1;
            }

            // Loop until all actions in this work set are finished.
            for (; ; )
            {
                Action nextAction;

                lock (syncRoot)
                {
                    // Synchronize and exit if this work set is finished.
                    if (!workSet.SyncHasPendingActions())
                    {
                        activeThreads -= 1;

                        while (workSet.SyncHasActionsInProgress())
                            Monitor.Wait(syncRoot);

                        return;
                    }

                    // Prepare next action from this work set only.
                    nextAction = workSet.SyncPrepareNextAction();

                    // Remove the work set from the list of pending work sets if it has no other pending actions.
                    if (!workSet.SyncHasPendingActions())
                        pendingWorkSets.Remove(workSet);

                    // Spawn more threads if needed for pending work sets.
                    SyncSpawnBackgroundActionLoopIfNeeded();
                }

                // Run the next action.
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

        private void SyncSpawnBackgroundActionLoopIfNeeded()
        {
            if (pendingWorkSets.Count != 0
                && activeThreads < GetDegreeOfParallelism())
            {
                activeThreads += 1;
                ThreadPool.QueueUserWorkItem(BackgroundActionLoop);
            }
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
                    // Exit if no pending work sets remain or if there are too many threads running.
                    if (pendingWorkSets.Count == 0
                        || activeThreads > GetDegreeOfParallelism())
                    {
                        activeThreads -= 1;
                        return;
                    }

                    // Prepare next action.
                    nextWorkSet = pendingWorkSets.First.Value;
                    nextAction = nextWorkSet.SyncPrepareNextAction();

                    // Remove the work set from the list of pending work sets if it has no other pending actions.
                    if (!nextWorkSet.SyncHasPendingActions())
                        pendingWorkSets.RemoveFirst();

                    // Spawn more threads if needed for pending work sets.
                    SyncSpawnBackgroundActionLoopIfNeeded();
                }

                // Run the next action.
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
