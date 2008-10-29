using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Gallio.Framework;
using Gallio.Framework.Assertions;
using Gallio.Model;
using Gallio.Model.Logging;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Framework
{
    [TestsOn(typeof(Sandbox))]
    public class SandboxTest
    {
        [Test]
        public void RunThrowsIfTestLogWriterIsNull()
        {
            Sandbox sandbox = new Sandbox();
            Assert.Throws<ArgumentNullException>(() =>
                sandbox.Run(null, delegate { }, "description"));
        }

        [Test]
        public void RunThrowsIfActionIsNull()
        {
            Sandbox sandbox = new Sandbox();
            Assert.Throws<ArgumentNullException>(() =>
                sandbox.Run(MockRepository.GenerateStub<TestLogWriter>(), null, "description"));
        }

        [Test]
        public void WhenActionCompletesNormally_RunReturnsPassedAndLogsNothing()
        {
            StructuredTestLogWriter writer = new StructuredTestLogWriter();
            Sandbox sandbox = new Sandbox();
            Assert.AreEqual(TestOutcome.Passed, sandbox.Run(writer, delegate { }, null));

            Assert.AreEqual("", writer.ToString());
        }

        [Test]
        public void WhenActionThrowsANonTestException_RunReturnsFailedAndLogsTheException()
        {
            StructuredTestLogWriter writer = new StructuredTestLogWriter();

            Sandbox sandbox = new Sandbox();
            Assert.AreEqual(TestOutcome.Failed, sandbox.Run(writer, delegate { throw new InvalidOperationException("Foo"); }, null));

            Assert.Contains(writer.ToString(), "InvalidOperationException");
        }

        [Test]
        public void WhenActionThrowsTestExceptionWithExcludedStackTraceAndDefaultMessage_RunReturnsOutputAndLogsNothing()
        {
            StructuredTestLogWriter writer = new StructuredTestLogWriter();

            Sandbox sandbox = new Sandbox();
            Assert.AreEqual(TestOutcome.Canceled, sandbox.Run(writer, delegate { throw new SilentTestException(TestOutcome.Canceled); }, null));

            Assert.AreEqual("", writer.ToString());
        }

        [Test]
        public void WhenActionThrowsTestExceptionWithExcludedStackTraceAndNonDefaultMessage_RunReturnsOutputAndLogsTheMessageButNotTheException()
        {
            StructuredTestLogWriter writer = new StructuredTestLogWriter();

            Sandbox sandbox = new Sandbox();
            Assert.AreEqual(TestOutcome.Error, sandbox.Run(writer, delegate { throw new SilentTestException(TestOutcome.Error, "Message."); }, null));

            Assert.DoesNotContain(writer.ToString(), "SilentTestException");
            Assert.Contains(writer.ToString(), "Message.");
        }

        [Test]
        public void WhenActionThrowsTestExceptionWithIncludedStackTraceAndNonDefaultMessage_RunReturnsOutputAndLogsTheException()
        {
            StructuredTestLogWriter writer = new StructuredTestLogWriter();

            Sandbox sandbox = new Sandbox();
            Assert.AreEqual(TestOutcome.Failed, sandbox.Run(writer, delegate { throw new AssertionException("Reason."); }, null));

            Assert.Contains(writer.ToString(), "AssertionException");
            Assert.Contains(writer.ToString(), "Reason.");
        }

        [Test]
        public void WhenActionFailsAndADescriptionWasProvided_TheDescriptionAppearsInTheLog()
        {
            StructuredTestLogWriter writer = new StructuredTestLogWriter();

            Sandbox sandbox = new Sandbox();
            Assert.AreEqual(TestOutcome.Failed, sandbox.Run(writer, delegate { throw new InvalidOperationException("Foo"); }, "SetUp"));

            Assert.Contains(writer.ToString(), "InvalidOperationException");
            Assert.Contains(writer.ToString(), "SetUp");
        }

        [Test]
        public void RunCanBeAbortedInProgress()
        {
            StructuredTestLogWriter writer = new StructuredTestLogWriter();
            ManualResetEvent ready = new ManualResetEvent(false);
            bool completed = false;

            Sandbox sandbox = new Sandbox();

            Tasks.StartThreadTask("Background abort.", () =>
            {
                ready.WaitOne();
                sandbox.Abort(TestOutcome.Canceled, "Test was canceled.");
            });

            TestOutcome outcome = sandbox.Run(writer, () =>
            {
                ready.Set();
                Thread.Sleep(10000);
                completed = true;
            }, "Run Description");

            Assert.IsFalse(completed, "The action should have been aborted prior to completion.");

            Assert.AreEqual(TestOutcome.Canceled, outcome);
            Assert.AreEqual(TestOutcome.Canceled, sandbox.AbortOutcome);
            Assert.AreEqual("Test was canceled.", sandbox.AbortMessage);
            Assert.IsTrue(sandbox.WasAborted);

            Assert.Contains(writer.ToString(), "Run Description");
            Assert.Contains(writer.ToString(), "Test was canceled.");
        }

        [Test]
        public void RunWillBeAbortedByTimeoutIfItExpiresBeforeItIsFinished()
        {
            RunWithTimeout(TimeSpan.FromMilliseconds(10000), TimeSpan.FromMilliseconds(100));
        }

        [Test]
        public void RunWillNotBeAbortedByTimeoutIfItExpiresAfterItIsFinished()
        {
            RunWithTimeout(TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(10000));
        }

        [Test]
        public void RunWillNotBeAbortedByTimeoutIfItIsNull()
        {
            RunWithTimeout(TimeSpan.FromMilliseconds(1), null);
        }

        private static void RunWithTimeout(TimeSpan waitTime, TimeSpan? timeout)
        {
            StructuredTestLogWriter writer = new StructuredTestLogWriter();
            bool completed = false;

            Sandbox sandbox = new Sandbox();

            TestOutcome outcome = TestOutcome.Error;
            sandbox.UseTimeout(timeout, () =>
            {
                outcome = sandbox.Run(writer, () =>
                {
                    Thread.Sleep(waitTime);
                    completed = true;
                }, "Run Description");
            });

            if (timeout.HasValue && timeout.Value < waitTime)
            {
                Assert.IsFalse(completed, "The action should have been aborted prior to completion.");

                Assert.AreEqual(TestOutcome.Timeout, outcome);
                Assert.AreEqual(TestOutcome.Timeout, sandbox.AbortOutcome);
                Assert.AreEqual(String.Format("The test timed out after {0} seconds.", timeout.Value.TotalSeconds),
                    sandbox.AbortMessage);
                Assert.IsTrue(sandbox.WasAborted);

                Assert.Contains(writer.ToString(), "Run Description");
                Assert.Contains(writer.ToString(),
                    String.Format("The test timed out after {0} seconds.", timeout.Value.TotalSeconds));
            }
            else
            {
                Assert.IsTrue(completed, "The action should have completed because no timeout occurred.");

                Assert.AreEqual(TestOutcome.Passed, outcome);
                Assert.IsFalse(sandbox.WasAborted);
            }
        }

        public class WhenAborted
        {
            private readonly Sandbox abortedSandbox;

            [Row(true, Description="Abort once.")]
            [Row(false, Description="Abort twice.  Second call should have no effect.")]
            public WhenAborted(bool abortTwice)
            {
                abortedSandbox = new Sandbox();
                abortedSandbox.Abort(TestOutcome.Canceled, "Abort message.");

                if (abortTwice)
                    abortedSandbox.Abort(TestOutcome.Passed, "A different message.");
            }

            [Test]
            public void WasAbortedIsTrue()
            {
                Assert.IsTrue(abortedSandbox.WasAborted);
            }

            [Test]
            public void AbortMessageContainsSuppliedMessage()
            {
                Assert.AreEqual("Abort message.", abortedSandbox.AbortMessage);
            }

            [Test]
            public void AbortOutcomeContainsSuppliedOutcome()
            {
                Assert.AreEqual(TestOutcome.Canceled, abortedSandbox.AbortOutcome);
            }

            [Test]
            public void RunExitsImmediatelyAndLogsMessage()
            {
                StructuredTestLogWriter writer = new StructuredTestLogWriter();
                bool actionWasRun = false;
                abortedSandbox.Run(writer, () => actionWasRun = true, "Description");

                Assert.IsFalse(actionWasRun, "The action should not be run because the sandbox already aborted.");
                Assert.AreEqual("", writer.ToString());
            }
        }
    }
}
