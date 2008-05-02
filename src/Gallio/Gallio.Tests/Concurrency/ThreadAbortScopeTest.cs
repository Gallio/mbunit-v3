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
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Gallio.Concurrency;
using Gallio.Framework;
using MbUnit.Framework;

namespace Gallio.Tests.Concurrency
{
    [TestFixture]
    [TestsOn(typeof(ThreadAbortScope))]
    public class ThreadAbortScopeTest
    {
        [Test, ExpectedArgumentNullException]
        public void RunThrowsIfActionIsNull()
        {
            ThreadAbortScope scope = new ThreadAbortScope();

            scope.Run(null);
        }

        [Test]
        public void RunThrowsIfCalledReentrantly()
        {
            ThreadAbortScope scope = new ThreadAbortScope();

            scope.Run(delegate
            {
                InterimAssert.Throws<InvalidOperationException>(delegate
                {
                    scope.Run(delegate { });
                });
            });
        }

        [Test]
        public void RunCanBeCalledRepeatedly()
        {
            ThreadAbortScope scope = new ThreadAbortScope();
            int count = 0;

            for (int i = 0; i < 3; i++)
            {
                Assert.IsNull(scope.Run(delegate { count += 1; }));
            }

            Assert.AreEqual(3, count);
        }

        [Test]
        public void AbortBeforeRunCausesImmediateAbortion()
        {
            ThreadAbortScope scope = new ThreadAbortScope();
            int count = 0;

            scope.Abort();
            Assert.IsNotNull(scope.Run(delegate { count += 1; }));

            Assert.AreEqual(0, count);
        }

        [Test]
        public void AbortAfterRunCausesImmediateAbortionOfNextRun()
        {
            ThreadAbortScope scope = new ThreadAbortScope();

            int count = 0;
            Assert.IsNull(scope.Run(delegate { count += 1; }));

            scope.Abort();
            Assert.IsNotNull(scope.Run(delegate { count += 1; }));

            Assert.AreEqual(1, count);
        }

        [Test]
        public void AbortDuringRunOnSameThreadUnwindsGracefully()
        {
            ThreadAbortScope scope = new ThreadAbortScope();

            int count = 0;
            Assert.IsNotNull(scope.Run(delegate
            {
                count += 1;
                scope.Abort();
                count += 1;
            }));

            Assert.AreEqual(1, count);
        }

        [Test]
        public void AbortFromADifferentThreadUnwindsGracefully()
        {
            ThreadAbortScope scope = new ThreadAbortScope();

            ManualResetEvent barrier = new ManualResetEvent(false);
            Tasks.StartThreadTask("Background Abort", delegate
            {
                barrier.WaitOne();
                scope.Abort(); 
            });

            int count = 0;
            Assert.IsNotNull(scope.Run(delegate
            {
                count += 1;
                barrier.Set();
                Thread.Sleep(5000);
                count += 1;
            }));

            Assert.AreEqual(1, count);
        }

        [Test]
        [Description("Here we use a pair of nested scopes and abort the outer one to verify that the inner scope will only try to intercept its own aborts.")]
        public void AbortFromADifferentSourceUnwindsGracefully()
        {
            ThreadAbortScope outerScope = new ThreadAbortScope();

            Assert.IsNotNull(outerScope.Run(delegate
            {
                ThreadAbortScope innerScope = new ThreadAbortScope();
                innerScope.Run(delegate { outerScope.Abort(); });
            }));
        }

        [Test]
        [Ignore("This test fails occasionally.  Exact cause is non-deterministic and has yet to be determined.")]
        public void TryToAsynchronouslyHitARunningActionRandomlyInAllPossibleWays()
        {
            // Like sitting ducks...
            for (int i = 0; i < 1000; i++)
            {
                ThreadAbortScope scope = new ThreadAbortScope();

                Tasks.StartThreadTask("Background Abort", delegate
                {
                    Thread.Sleep(i);
                    scope.Abort();
                });

                Stopwatch timeout = Stopwatch.StartNew();
                while (scope.Run(delegate { }) == null)
                {
                    if (timeout.ElapsedMilliseconds > 200)
                        Assert.Fail("The scope failed to stop the run.");
                }
            }
        }
    }
}
