using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Gallio.Framework;
using Gallio.Framework.Pattern;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Pattern
{
    [TestsOn(typeof(ParallelizableTestCaseScheduler))]
    public class ParallelizableTestCaseSchedulerTest
    {
        private int maxThreads;

        [Column(1, 2, 4, 8)]
        public ParallelizableTestCaseSchedulerTest(int maxThreads)
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

            var scheduler = new ParallelizableTestCaseScheduler(() => maxThreads);
            scheduler.Run(actions);

            for (int i = 0; i < numActions; i++)
                Assert.IsTrue(finished[i]);
        }

        [Test, Timeout(1000)]
        public void SupportsReentrance()
        {
            var scheduler = new ParallelizableTestCaseScheduler(() => maxThreads);
            Assert.AreEqual(21, Fibonnaci(scheduler, 8));
        }

        private static int Fibonnaci(ParallelizableTestCaseScheduler scheduler, int count)
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
