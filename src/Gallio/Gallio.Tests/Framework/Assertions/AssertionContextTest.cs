// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Collections;
using Gallio.Framework;
using Gallio.Framework.Assertions;
using Gallio.Model.Diagnostics;
using Gallio.Model.Logging;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Assertions
{
    [TestsOn(typeof(AssertionContext))]
    public class AssertionContextTest
    {
        [Test]
        public void CurrentAssertionContextIsAssociatedWithTheCurrentTestContext()
        {
            AssertionContext current = AssertionContext.CurrentContext;
            Assert.AreSame(TestContext.CurrentContext, current.TestContext);
        }

        [Test]
        public void CurrentAssertionContextIsPreservedAcrossMultipleRequests()
        {
            Assert.AreSame(AssertionContext.CurrentContext, AssertionContext.CurrentContext);
        }

        [Test]
        public void DifferentTestContextsHaveDifferentAssertionContexts()
        {
            AssertionContext a = null, b = null;
            TestStep.RunStep("A", () => a = AssertionContext.CurrentContext);
            TestStep.RunStep("B", () => b = AssertionContext.CurrentContext);

            Assert.IsNotNull(a);
            Assert.IsNotNull(b);
            Assert.AreNotSame(a, b);
        }

        [Test]
        public void InitialAssertionFailureBehaviorIsLogAndThrow()
        {
            Assert.AreEqual(AssertionFailureBehavior.LogAndThrow, AssertionContext.CurrentContext.AssertionFailureBehavior);
        }

        public class WhenAssertionFailureBehaviorIsLogAndThrow
        {
            [Test]
            public void TheAssertionIsLoggedAndCapturedButExecutionEnds()
            {
                StubAssertionFailure failure1 = new StubAssertionFailure();
                StubAssertionFailure failure2 = new StubAssertionFailure();
                bool completed = false;

                AssertionFailure[] failures = AssertionContext.CurrentContext.CaptureFailures(delegate
                {
                    AssertionContext.CurrentContext.SubmitFailure(failure1);
                    AssertionContext.CurrentContext.SubmitFailure(failure2);
                    completed = true;
                }, AssertionFailureBehavior.LogAndThrow, false);

                ArrayAssert.AreEqual(new[] { failure1 }, failures);
                Assert.IsTrue(failure1.WasWriteToCalled);
                Assert.IsFalse(failure2.WasWriteToCalled);
                Assert.IsFalse(completed);
            }

            [Test]
            public void AnExceptionMayBeReifiedAsAnAssertionFailure()
            {
                AssertionFailure[] failures = AssertionContext.CurrentContext.CaptureFailures(delegate
                {
                    throw new InvalidOperationException("Boom");
                }, AssertionFailureBehavior.LogAndThrow, true);

                Assert.AreEqual(1, failures.Length);
                Assert.AreEqual("An exception occurred.", failures[0].Description);
                Assert.AreEqual(1, failures[0].Exceptions.Count);
                Assert.Contains(failures[0].Exceptions[0].ToString(), "Boom");
            }

            [Test]
            public void AnExceptionMayEscapeTheBlock()
            {
                NewAssert.Throws<InvalidOperationException>(delegate
                {
                    AssertionContext.CurrentContext.CaptureFailures(delegate
                    {
                        throw new InvalidOperationException("Boom");
                    }, AssertionFailureBehavior.LogAndThrow, false);
                });
            }
        }

        public class WhenAssertionFailureBehaviorIsThrow
        {
            [Test]
            public void TheAssertionIsNotLoggedButItIsCapturedButExecutionEnds()
            {
                StubAssertionFailure failure1 = new StubAssertionFailure();
                StubAssertionFailure failure2 = new StubAssertionFailure();
                bool completed = false;

                AssertionFailure[] failures = AssertionContext.CurrentContext.CaptureFailures(delegate
                {
                    AssertionContext.CurrentContext.SubmitFailure(failure1);
                    AssertionContext.CurrentContext.SubmitFailure(failure2);
                    completed = true;
                }, AssertionFailureBehavior.Throw, false);

                ArrayAssert.AreEqual(new[] { failure1 }, failures);
                Assert.IsFalse(failure1.WasWriteToCalled);
                Assert.IsFalse(failure2.WasWriteToCalled);
                Assert.IsFalse(completed);
            }
        }

        public class WhenAssertionFailureBehaviorIsLog
        {
            [Test]
            public void TheAssertionIsLoggedAndCapturedAndExecutionContinues()
            {
                StubAssertionFailure failure1 = new StubAssertionFailure();
                StubAssertionFailure failure2 = new StubAssertionFailure();
                bool completed = false;

                AssertionFailure[] failures = AssertionContext.CurrentContext.CaptureFailures(delegate
                {
                    AssertionContext.CurrentContext.SubmitFailure(failure1);
                    AssertionContext.CurrentContext.SubmitFailure(failure2);
                    completed = true;
                }, AssertionFailureBehavior.Log, false);

                ArrayAssert.AreEqual(new[] { failure1, failure2 }, failures);
                Assert.IsTrue(failure1.WasWriteToCalled);
                Assert.IsTrue(failure2.WasWriteToCalled);
                Assert.IsTrue(completed);
            }
        }

        public class WhenAssertionFailureBehaviorIsDefer
        {
            [Test]
            public void TheAssertionIsNotLoggedButIsCapturedAndExecutionContinues()
            {
                StubAssertionFailure failure1 = new StubAssertionFailure();
                StubAssertionFailure failure2 = new StubAssertionFailure();
                bool completed = false;

                AssertionFailure[] failures = AssertionContext.CurrentContext.CaptureFailures(delegate
                {
                    AssertionContext.CurrentContext.SubmitFailure(failure1);
                    AssertionContext.CurrentContext.SubmitFailure(failure2);
                    completed = true;
                }, AssertionFailureBehavior.Defer, false);

                ArrayAssert.AreEqual(new[] { failure1, failure2 }, failures);
                Assert.IsFalse(failure1.WasWriteToCalled);
                Assert.IsFalse(failure2.WasWriteToCalled);
                Assert.IsTrue(completed);
            }
        }

        public class WhenAssertionFailureBehaviorIsIgnore
        {
            [Test]
            public void NothingHappens()
            {
                StubAssertionFailure failure1 = new StubAssertionFailure();
                StubAssertionFailure failure2 = new StubAssertionFailure();
                bool completed = false;

                AssertionFailure[] failures = AssertionContext.CurrentContext.CaptureFailures(delegate
                {
                    AssertionContext.CurrentContext.SubmitFailure(failure1);
                    AssertionContext.CurrentContext.SubmitFailure(failure2);
                    completed = true;
                }, AssertionFailureBehavior.Ignore, false);

                Assert.IsEmpty(failures);
                Assert.IsFalse(failure1.WasWriteToCalled);
                Assert.IsFalse(failure2.WasWriteToCalled);
                Assert.IsTrue(completed);
            }
        }

        private sealed class StubAssertionFailure : AssertionFailure
        {
            private bool wasWriteToCalled;

            public StubAssertionFailure() :
                base("Description", "Message", "Stack", EmptyArray<LabeledValue>.Instance,
                EmptyArray<ExceptionData>.Instance)
            {
            }

            public bool WasWriteToCalled
            {
                get { return wasWriteToCalled; }
            }

            public override void WriteTo(TestLogStreamWriter writer)
            {
                wasWriteToCalled = true;
                base.WriteTo(writer);
            }
        }
    }
}
