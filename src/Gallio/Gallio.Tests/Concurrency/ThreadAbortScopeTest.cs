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
        public void RunThrowsIfCalledReentrantlyOnSameThread()
        {
            ThreadAbortScope scope = new ThreadAbortScope();

            scope.Run(delegate
            {
                Assert.Throws<InvalidOperationException>(delegate
                {
                    scope.Run(delegate { });
                });
            });
        }

        [Test]
        public void RunThrowsIfCalledReentrantlyOnDifferentThread()
        {
            ThreadAbortScope scope = new ThreadAbortScope();

            scope.Run(delegate
            {
                Tasks.StartThreadTask("Reentrant Call to Scope.Run", () =>
                {
                    Assert.Throws<InvalidOperationException>(delegate
                    {
                        scope.Run(delegate { });
                    });
                });
                Tasks.JoinAndVerify(TimeSpan.FromMilliseconds(500));
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

                count += 1; // should not run
            }));

            Assert.AreEqual(1, count);

            Tasks.JoinAndVerify(TimeSpan.FromMilliseconds(100));
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
        [Repeat(3)]
        public void TryToAsynchronouslyHitARunningActionAtRandomTimes()
        {
            const int Iterations = 50;

            for (int i = 0; i < Iterations; i++)
            {
                ThreadAbortScope scope = new ThreadAbortScope();

                Tasks.StartThreadTask("Background Abort", delegate
                {
                    Thread.Sleep(i);
                    scope.Abort();
                });

                Stopwatch timeout = Stopwatch.StartNew();
                while (scope.Run(delegate { Thread.SpinWait(i % 13); }) == null)
                {
                    if (timeout.ElapsedMilliseconds > Iterations + 500)
                        Assert.Fail("The scope failed to stop the run during iteration {0}.", i);
                }

                Tasks.JoinAndVerify(TimeSpan.FromMilliseconds(100));
            }
        }

        [Test]
        public void ProtectThrowsIfActionIsNull()
        {
            ThreadAbortScope scope = new ThreadAbortScope();
            Assert.Throws<ArgumentNullException>(() => scope.Protect(null));
        }

        [Test]
        public void ProtectOutsideOfScopeJustRunsTheAction()
        {
            ThreadAbortScope scope = new ThreadAbortScope();

            bool ran = false;
            scope.Protect(() => ran = true);

            Assert.IsTrue(ran, "Should have run the action.");
        }

        [Test]
        public void ProtectInAnotherThreadJustRunsTheAction()
        {
            ThreadAbortScope scope = new ThreadAbortScope();

            bool ran = false;

            Tasks.StartThreadTask("Different thread", () =>
            {
                scope.Protect(() => ran = true);
            });
            Tasks.JoinAndVerify(TimeSpan.FromMilliseconds(100));

            Assert.IsTrue(ran, "Should have run the action.");
        }

        [Test]
        public void AbortFromWithinProtectedScopeDoesNotOccurUntilTheProtectedScopeExits()
        {
            bool ranToCompletion = false;
            ThreadAbortScope scope = new ThreadAbortScope();
            ThreadAbortException ex = scope.Run(() =>
            {
                scope.Protect(() =>
                {
                    scope.Abort();
                    ranToCompletion = true;
                });
            });

            Assert.IsNotNull(ex, "Should have aborted.");
            Assert.IsTrue(ranToCompletion, "Should have run the action.");
        }

        [Test]
        public void AbortWaitsUntilProtectedScopeEndsBeforeOccurring()
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

                scope.Protect(() =>
                {
                    count += 1;
                    barrier.Set();
                    Thread.Sleep(5000);
                    count += 1;
                });

                count += 1; // should not run
            }));

            Assert.AreEqual(3, count);

            Tasks.JoinAndVerify(TimeSpan.FromMilliseconds(100));
        }
    }
}
