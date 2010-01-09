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
using System.Linq;
using System.Text;
using System.Threading;
using Gallio.Common.Concurrency;
using Gallio.Framework;
using MbUnit.Framework;
using Action=Gallio.Common.Action;

namespace Gallio.Tests.Common.Concurrency
{
    [TestsOn(typeof(WorkScheduler))]
    public class WorkSchedulerTest
    {
        private int maxThreads;

        [Column(1, 2, 4, 8)]
        public WorkSchedulerTest(int maxThreads)
        {
            this.maxThreads = maxThreads;
        }

        [Test, Timeout(1000)]
        public void ShouldWaitForAllActionsToFinishBeforeReturning(
            [Column(0, 1, 2, 7, 19)] int numActions)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            bool[] finished = new bool[numActions];
            Action[] actions = new Action[numActions];
            for (int i = 0; i < numActions; i++)
            {
                int actionIndex = i;
                actions[i] = () =>
                {
                    Thread.Sleep((actionIndex + 1) * 71 % 37);
                    TestLog.WriteLine("Iteration #{0} finished after {1}ms", actionIndex + 1, stopwatch.ElapsedMilliseconds);
                    finished[actionIndex] = true;
                };
            }

            var scheduler = new WorkScheduler(() => maxThreads);
            scheduler.Run(actions);

            for (int i = 0; i < numActions; i++)
                Assert.IsTrue(finished[i]);
        }

        [Test, Timeout(1000)]
        public void SupportsReentrance()
        {
            var scheduler = new WorkScheduler(() => maxThreads);
            Assert.AreEqual(21, Fibonnaci(scheduler, 8));
        }

        private static int Fibonnaci(WorkScheduler scheduler, int count)
        {
            if (count < 2)
                return count;

            int sum = 0;
            scheduler.Run(new Action[]
            {
                () => Interlocked.Add(ref sum, Fibonnaci(scheduler, count - 1)),
                () => Interlocked.Add(ref sum, Fibonnaci(scheduler, count - 2))
            });
            return sum;
        }
    }
}