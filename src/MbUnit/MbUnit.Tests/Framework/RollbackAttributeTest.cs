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
using System.Transactions;
using Gallio.Framework;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(RollbackAttribute))]
    [RunSample(typeof(RollbackSample))]
    public class RollbackAttributeTest : BaseTestWithSampleRunner
    {
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
}
