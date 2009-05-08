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
using System.Threading;
using Gallio.Tests;
using MbUnit.Framework;
using Action=Gallio.Common.Action;

namespace Gallio.AutoCAD.Tests
{
    [TestsOn(typeof(AcadThread))]
    public class AcadThreadTest : BaseTestWithMocks
    {
        private static readonly Action NoOp = delegate { };
        private static readonly Action Fail = new Action(Assert.Fail);

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void CallingInvokeBeforeRunThrowsException()
        {
            new AcadThread().Invoke(Fail);
        }

        [Test]
        public void SurrendersThreadAfterShutdown()
        {
            using (AcadThreadTestScope scope = new AcadThreadTestScope())
            {
                // scope.Dispose() calls AcadThread.Shutdown().
            }
        }

        [Test]
        public void CallbackHappensOnCorrectThread()
        {
            using (AcadThreadTestScope scope = new AcadThreadTestScope())
            {
                scope.AcadThread.Invoke(new Action(() => Assert.AreEqual(scope.ProcessingThread, Thread.CurrentThread)));
            }
        }

        [Test]
        public void ReentryDoesntCauseDeadlock()
        {
            using (AcadThreadTestScope scope = new AcadThreadTestScope())
            {
                scope.AcadThread.Invoke(new Action(() => scope.AcadThread.Invoke(NoOp)));
            }
        }

        [Test]
        public void InvokeReturnsValue()
        {
            using (AcadThreadTestScope scope = new AcadThreadTestScope())
            {
                object expected = new object();
                object result = scope.AcadThread.Invoke(new Common.Func<object>(delegate { return expected; }));
                Assert.AreSame(expected, result);
            }
        }

        [Test]
        public void ExceptionsThrownInCallbackAreThrownFromInvoke()
        {
            using (AcadThreadTestScope scope = new AcadThreadTestScope())
            {
                InvalidOperationException expected = new InvalidOperationException("This exception is expected.");
                try
                {
                    scope.AcadThread.Invoke(new Action(delegate { throw expected; }));
                    Assert.Fail();
                }
                catch (InvalidOperationException result)
                {
                    Assert.AreSame(expected, result);
                }
            }
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void CallingRunTwiceThrowsInvalidOperationException()
        {
            using (AcadThreadTestScope scope = new AcadThreadTestScope())
            {
                scope.AcadThread.Run();
            }
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void CallingInvokeAfterShutdownThrowsArgumentNullException()
        {
            AcadThreadTestScope scope = new AcadThreadTestScope();
            scope.Dispose();
            scope.AcadThread.Invoke(NoOp);
        }

        [Test, ExpectedArgumentNullException]
        public void CallingInvokeWithNullDelegateThrowsArgumentNullException()
        {
            using (AcadThreadTestScope scope = new AcadThreadTestScope())
            {
                scope.AcadThread.Invoke(null);
            }
        }

        [Test]
        public void OrphanedThreadsThrowThreadInterruptedException()
        {
            using (AcadThreadTestScope scope = new AcadThreadTestScope())
            {
                AutoResetEvent firstThreadRunning = new AutoResetEvent(false);
                AutoResetEvent completeFirstThread = new AutoResetEvent(false);

                Thread firstThread = new Thread(new ThreadStart(delegate
                    {
                        scope.AcadThread.Invoke(new Action(delegate
                            {
                                firstThreadRunning.Set();
                                completeFirstThread.WaitOne();
                            }));
                    }));
                firstThread.Start();
                firstThreadRunning.WaitOne();
                
                Thread orphanedThread = new Thread(new ThreadStart(delegate
                    {
                        try
                        {
                            scope.AcadThread.Invoke(Fail);
                        }
                        catch (ThreadInterruptedException)
                        {
                            return;
                        }

                        Assert.Fail();
                    }));
                orphanedThread.Start();
                Thread.Sleep(200); // Give it enough time to get into the queue.
                
                scope.AcadThread.Shutdown();

                completeFirstThread.Set();
                
                Assert.IsTrue(firstThread.Join(TimeSpan.FromSeconds(2)));
                Assert.IsTrue(orphanedThread.Join(TimeSpan.FromSeconds(2)));
            }
        }

        [Test]
        public void InvokePassesArgumentsCorrectly()
        {
            object first = new object();
            object second = new object();
            object third = new object();
            
            using (AcadThreadTestScope scope = new AcadThreadTestScope())
            {
                Assert.AreSame(first,  scope.AcadThread.Invoke(new Common.Func<object, object>(x => x), first));
                Assert.AreSame(second, scope.AcadThread.Invoke(new Common.Func<object, object, object>((x, y) => y), first, second));
                Assert.AreSame(third,  scope.AcadThread.Invoke(new Common.Func<object, object, object, object>((x, y, z) => z), first, second, third));
            }
        }

        public interface IArgumentCheckerTest
        {
            object ReturnArgument(object first);
            object ReturnSecondArgument(object first, object second);
            object ReturnThirdArgument(object first, object second, object third);
        }

        private class AcadThreadTestScope : IDisposable
        {
            internal AcadThread AcadThread;
            internal Thread ProcessingThread;

            public AcadThreadTestScope()
            {
                AcadThread = new AcadThread();
                ProcessingThread = new Thread(AcadThread.Run);
                ProcessingThread.Start();
                Thread.Sleep(TimeSpan.FromSeconds(0.5));
            }

            public void Dispose()
            {
                AcadThread.Shutdown();
                Assert.IsTrue(ProcessingThread.Join(TimeSpan.FromSeconds(2)), "AcadThread did not surrender the processing thread after Shutdown().");
            }
        }
    }
}
