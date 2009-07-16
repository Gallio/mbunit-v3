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
using System.Text;
using System.Threading;
using Gallio.Common;
using Gallio.Common.Policies;
using Gallio.Model;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Schedules actions related to test cases to be run in parallel up to a specified
    /// (variable) maximum number of threads.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The implementation is designed to support re-entrance while still maintaining an
    /// upper bound on active thread count.  To avoid blowing the limit on thread count
    /// due to re-entrance the caller's thread is also used to schedule work.
    /// </para>
    /// </remarks>
    internal sealed class ParallelizableTestCaseScheduler
    {
        private readonly WorkScheduler workScheduler;

        public ParallelizableTestCaseScheduler(Func<int> maxThreads)
        {
            workScheduler = new WorkScheduler(maxThreads);
        }

        /// <summary>
        /// Runs actions in parallel up to the desired degree of parallelism
        /// then waits for them all to complete.
        /// </summary>
        /// <param name="actions">The actions to run in parallel (up to the
        /// supported degree of parallelism).</param>
        public void Run(IList<Action> actions)
        {
            WorkSet workSet = workScheduler.BeginWorkSet(actions);
            workSet.RunToCompletion();
        }

        private sealed class WorkScheduler
        {
            private readonly object syncRoot = new object();
            private readonly Queue<WorkSet> pendingWorkSets;
            private readonly Func<int> getMaxThreads;

            private int activeThreads = 1;

            public WorkScheduler(Func<int> getMaxThreads)
            {
                this.getMaxThreads = getMaxThreads;

                pendingWorkSets = new Queue<WorkSet>();
            }

            public object SyncRoot
            {
                get { return syncRoot; }
            }

            public WorkSet BeginWorkSet(IEnumerable<Action> actions)
            {
                lock (syncRoot)
                {
                    WorkSet workSet = new WorkSet(this, actions);
                    pendingWorkSets.Enqueue(workSet);
                    return workSet;
                }
            }

            internal void SyncFork()
            {
                if (activeThreads < getMaxThreads())
                    ThreadPool.QueueUserWorkItem(BackgroundActionLoop);
            }

            private void BackgroundActionLoop(object dummy)
            {
                while (RunNextPendingAction())
                {
                }
            }

            private bool RunNextPendingAction()
            {
                WorkSet nextWorkSet;
                Action nextAction;

                lock (syncRoot)
                {
                    for (; ; )
                    {
                        if (pendingWorkSets.Count == 0)
                            return false;

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

                nextWorkSet.UnsyncRunPreparedAction(nextAction);
                return true;
            }
        }

        private sealed class WorkSet
        {
            private readonly WorkScheduler workScheduler;
            private readonly Queue<Action> actions;
            private int actionsInProgress;

            public WorkSet(WorkScheduler workScheduler, IEnumerable<Action> actions)
            {
                this.workScheduler = workScheduler;
                this.actions = new Queue<Action>(actions);
            }

            public void RunToCompletion()
            {
                for (; ; )
                {
                    Action nextAction;

                    lock (workScheduler.SyncRoot)
                    {
                        if (! SyncHasPendingActions())
                        {
                            if (actionsInProgress == 0)
                                return;

                            Monitor.Wait(workScheduler.SyncRoot);
                            continue;
                        }
                        else
                        {
                            nextAction = SyncPrepareNextAction();

                            if (SyncHasPendingActions())
                                workScheduler.SyncFork();
                        }
                    }

                    UnsyncRunPreparedAction(nextAction);
                }
            }

            internal bool SyncHasPendingActions()
            {
                return actions.Count != 0;
            }

            internal Action SyncPrepareNextAction()
            {
                Action action = actions.Dequeue();
                actionsInProgress += 1;
                return action;
            }

            internal void UnsyncRunPreparedAction(Action action)
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    UnhandledExceptionPolicy.Report("An unhandled exception occurred while running a work item.", ex);
                }
                finally
                {
                    lock (workScheduler.SyncRoot)
                    {
                        actionsInProgress -= 1;
                        Monitor.PulseAll(workScheduler.SyncRoot);
                    }
                }
            }
        }
    }
}
