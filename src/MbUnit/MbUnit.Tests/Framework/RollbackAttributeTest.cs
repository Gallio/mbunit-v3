using System;
using System.Transactions;
using Gallio.Framework;
using Gallio.Reflection;
using Gallio.Runner.Reports;
using Gallio.Tests.Integration;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(RollbackAttribute))]
    public class RollbackAttributeTest : BaseSampleTest
    {
        [FixtureSetUp]
        public void RunSamples()
        {
            RunFixtures(typeof(RollbackSample));
        }

        [Test]
        public void NoRollback()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(RollbackSample).GetMethod("NoRollback")));
            AssertLogContains(run, "SetUp: Prepare\nSetUp: Commit\nTest: Prepare\nTest: Commit\nTearDown: Prepare\nTearDown: Commit");
        }

        [Test]
        public void IncludeSetUpAndTearDown()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(RollbackSample).GetMethod("IncludeSetUpAndTearDown")));
            AssertLogContains(run, "SetUp: Rollback\nTest: Rollback\nTearDown: Rollback");
        }

        [Test]
        public void ExcludeSetUpAndTearDown()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(RollbackSample).GetMethod("ExcludeSetUpAndTearDown")));
            AssertLogContains(run, "SetUp: Prepare\nSetUp: Commit\nTest: Rollback\nTearDown: Prepare\nTearDown: Commit");
        }
    }

    [Explicit("Sample")]
    public class RollbackSample
    {
        [SetUp]
        public void SetUp()
        {
            EnlistAndCompleteScope("SetUp");
        }

        [TearDown]
        public void TearDown()
        {
            EnlistAndCompleteScope("TearDown");
        }

        [Test, Rollback(IncludeSetUpAndTearDown = true)]
        public void IncludeSetUpAndTearDown()
        {
            EnlistAndCompleteScope("Test");
        }

        [Test, Rollback]
        public void ExcludeSetUpAndTearDown()
        {
            EnlistAndCompleteScope("Test");
        }

        [Test]
        public void NoRollback()
        {
            EnlistAndCompleteScope("Test");
        }

        private static void EnlistAndCompleteScope(string phase)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                Enlist(phase);
                scope.Complete();
            }
        }

        private static void Enlist(string phase)
        {
            Transaction.Current.EnlistVolatile(new StubEnlistmentNotification(phase), EnlistmentOptions.EnlistDuringPrepareRequired);
        }

        private class StubEnlistmentNotification : IEnlistmentNotification
        {
            private readonly string phase;

            public StubEnlistmentNotification(string phase)
            {
                this.phase = phase;
            }

            public void Prepare(PreparingEnlistment preparingEnlistment)
            {
                TestLog.WriteLine("{0}: Prepare", phase);
                preparingEnlistment.Prepared();
            }

            public void Commit(Enlistment enlistment)
            {
                TestLog.WriteLine("{0}: Commit", phase);
                enlistment.Done();
            }

            public void Rollback(Enlistment enlistment)
            {
                TestLog.WriteLine("{0}: Rollback", phase);
                enlistment.Done();
            }

            public void InDoubt(Enlistment enlistment)
            {
                TestLog.WriteLine("{0}: InDoubt", phase);
                enlistment.Done();
            }
        }
    }
}