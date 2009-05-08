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
using Gallio.Common.Collections;
using Gallio.Framework;
using Gallio.Framework.Assertions;
using Gallio.Model;
using Gallio.Common.Reflection;
using MbUnit.Framework;

namespace Gallio.Tests.Framework
{
    [TestFixture]
    public class TestStepTest
    {
        [Test]
        public void EachTestStepHasItsOwnChildContext()
        {
            TestContext parentContext = TestContext.CurrentContext;
            TestContext stepRunningContext = null;

            TestContext stepFinishedContext = TestStep.RunStepAndVerifyOutcome("Name", () =>
            {
                stepRunningContext = TestContext.CurrentContext;
            }, TestOutcome.Passed);

            Assert.AreNotSame(parentContext, stepFinishedContext);
            Assert.AreSame(stepFinishedContext, stepRunningContext, "Should return same context on exit from step as was active during step.");
            Assert.AreSame(parentContext, stepFinishedContext.Parent, "Parent context should be containing test.");
        }

        [Test, Description("This test marks a change in behavior between v3.0.6 and v3.0.5 which previously inherited context data.")]
        public void EachTestStepHasItsOwnContextDataNotInheritedFromItsParent()
        {
            Key<string> key = new Key<string>("key");
            TestContext.CurrentContext.Data.SetValue(key, "value");
            Assert.AreEqual("value", TestContext.CurrentContext.Data.GetValue(key));

            TestStep.RunStepAndVerifyOutcome("Name", () =>
            {
                Assert.IsFalse(TestContext.CurrentContext.Data.HasValue(key));

                TestContext.CurrentContext.Data.SetValue(key, "new value");
                Assert.AreEqual("new value", TestContext.CurrentContext.Data.GetValue(key));
            }, TestOutcome.Passed);

            Assert.AreEqual("value", TestContext.CurrentContext.Data.GetValue(key));
        }

        [Test]
        public void CurrentStepHasCorrectTestName()
        {
            Assert.Like(TestStep.CurrentStep.FullName, "Gallio.Tests/TestStepTest/CurrentStepHasCorrectTestName$");

            string step1Name = null, step2Name = null;
            TestStep.RunStepAndVerifyOutcome("Step1", delegate
            {
                step1Name = TestStep.CurrentStep.FullName;

                TestStep.RunStepAndVerifyOutcome("Step2", delegate
                {
                    step2Name = TestStep.CurrentStep.FullName;
                }, TestOutcome.Passed);
            }, TestOutcome.Passed);

            Assert.Like(step1Name, "Gallio.Tests/TestStepTest/CurrentStepHasCorrectTestName/Step1$");
            Assert.Like(step2Name, "Gallio.Tests/TestStepTest/CurrentStepHasCorrectTestName/Step1/Step2$");
        }

        [Test]
        public void MetadataAdditionsAreVisibleInStepInfo()
        {
            Assert.IsNull(TestStep.CurrentStep.Metadata.GetValue("New"));

            TestStep.AddMetadata("New", "And improved!");
            Assert.AreEqual("And improved!", TestStep.CurrentStep.Metadata.GetValue("New"));

            TestStep.AddMetadata("New", "Now with less sugar.");
            Assert.AreElementsEqual(new string[] { "And improved!", "Now with less sugar." },
                TestStep.CurrentStep.Metadata["New"]);
        }

        [Test, MultipleAsserts]
        public void TestStepsAreIndependentFromContainingMultipleAssertsContext()
        {
            bool failedFast = false;
            TestStep.RunStepAndVerifyOutcome("Fail", () =>
            {
                failedFast = true;
                Assert.Fail("First failure.");

                failedFast = false;
                Assert.Fail("Second failure.");
            }, TestOutcome.Failed);

            Assert.IsTrue(failedFast);
        }

        [Test]
        public void RunStep_NameAndAction_ArgumentValidation()
        {
            Assert.Throws<ArgumentNullException>(() => TestStep.RunStep(null, () => { }));
            Assert.Throws<ArgumentNullException>(() => TestStep.RunStep("Abc", null));
        }

        [Test]
        public void RunStep_NameAndAction_Execution()
        {
            bool ran = false;
            TestContext context = TestStep.RunStep("Abc", () =>
            {
                ran = true;
                Assert.Inconclusive();
            });
            Assert.IsTrue(ran, "Verify that the step ran.");
            Assert.AreEqual(TestOutcome.Inconclusive, context.Outcome,
                "Verify that the step has the expected outcome.");
            Assert.AreEqual("Abc", context.TestStep.Name);
            Assert.AreEqual("RunStep_NameAndAction_Execution", context.TestStep.CodeElement.CodeReference.MemberName);
            Assert.IsFalse(context.TestStep.IsTestCase);
        }

        [Test]
        public void RunStep_NameActionAndTimeout_ArgumentValidation()
        {
            Assert.Throws<ArgumentNullException>(() => TestStep.RunStep(null, () => { }, TimeSpan.FromSeconds(60)));
            Assert.Throws<ArgumentNullException>(() => TestStep.RunStep("Abc", null, TimeSpan.FromSeconds(60)));
            Assert.Throws<ArgumentOutOfRangeException>(() => TestStep.RunStep("Abc", () => { }, TimeSpan.FromSeconds(-1)));
        }

        [Test]
        public void RunStep_NameActionAndTimeout_Execution()
        {
            bool ran = false;
            TestContext context = TestStep.RunStep("Abc", () =>
            {
                ran = true;
                Assert.TerminateSilently(TestOutcome.Skipped);
            }, null);
            Assert.IsTrue(ran, "Verify that the step ran.");
            Assert.AreEqual(TestOutcome.Skipped, context.Outcome,
                "Verify that the step has the expected outcome.");
            Assert.AreEqual("Abc", context.TestStep.Name);
            Assert.AreEqual("RunStep_NameActionAndTimeout_Execution", context.TestStep.CodeElement.CodeReference.MemberName);
            Assert.IsFalse(context.TestStep.IsTestCase);
        }

        [Test]
        public void RunStep_NameActionAndTimeout_ExecutionWithUnexpiredTimeout()
        {
            bool ran = false;
            TestContext context = TestStep.RunStep("Abc", () =>
            {
                ran = true;
                Thread.Sleep(100);
                Assert.TerminateSilently(TestOutcome.Pending);
            }, TimeSpan.FromSeconds(60));
            Assert.IsTrue(ran, "Verify that the step ran.");
            Assert.AreEqual(TestOutcome.Pending, context.Outcome,
                "Verify that the step has the expected outcome.");
            Assert.AreEqual("Abc", context.TestStep.Name);
            Assert.AreEqual("RunStep_NameActionAndTimeout_ExecutionWithUnexpiredTimeout", context.TestStep.CodeElement.CodeReference.MemberName);
            Assert.IsFalse(context.TestStep.IsTestCase);
        }

        [Test]
        public void RunStep_NameActionAndTimeout_ExecutionWithExpiredTimeout()
        {
            bool ran = false;
            bool shouldNotGetHere = false;
            TestContext context = TestStep.RunStep("Abc", () =>
            {
                ran = true;
                Thread.Sleep(1000);
                shouldNotGetHere = true;
                Assert.TerminateSilently(TestOutcome.Skipped);
            }, TimeSpan.FromMilliseconds(100));
            Assert.IsTrue(ran, "Verify that the step ran.");
            Assert.IsFalse(shouldNotGetHere, "Verify that the step aborted prematurely due to the timeout.");
            Assert.AreEqual(TestOutcome.Timeout, context.Outcome,
                "Verify that the step has the expected outcome.");
            Assert.AreEqual("Abc", context.TestStep.Name);
            Assert.AreEqual("RunStep_NameActionAndTimeout_ExecutionWithExpiredTimeout", context.TestStep.CodeElement.CodeReference.MemberName);
            Assert.IsFalse(context.TestStep.IsTestCase);
        }

        [Test]
        public void RunStep_NameActionTimeoutIsTestCaseAndCodeElement_ArgumentValidation()
        {
            Assert.Throws<ArgumentNullException>(() => TestStep.RunStep(null, () => { }, TimeSpan.FromSeconds(60), false, Reflector.Wrap(typeof(TestStepTest))));
            Assert.Throws<ArgumentNullException>(() => TestStep.RunStep("Abc", null, TimeSpan.FromSeconds(60), false, Reflector.Wrap(typeof(TestStepTest))));
            Assert.Throws<ArgumentOutOfRangeException>(() => TestStep.RunStep("Abc", () => { }, TimeSpan.FromSeconds(-1), false, Reflector.Wrap(typeof(TestStepTest))));
        }

        [Test]
        public void RunStep_NameActionTimeoutIsTestCaseAndCodeElement_Execution()
        {
            bool ran = false;
            TestContext context = TestStep.RunStep("Abc", () =>
            {
                ran = true;
                Assert.TerminateSilently(TestOutcome.Skipped);
            }, null, false, null);
            Assert.IsTrue(ran, "Verify that the step ran.");
            Assert.AreEqual(TestOutcome.Skipped, context.Outcome,
                "Verify that the step has the expected outcome.");
            Assert.AreEqual("Abc", context.TestStep.Name);
            Assert.IsNull(context.TestStep.CodeElement);
            Assert.IsFalse(context.TestStep.IsTestCase);
        }

        [Test]
        public void RunStep_NameActionTimeoutIsTestCaseAndCodeElement_ExecutionWithCodeElement()
        {
            bool ran = false;
            TestContext context = TestStep.RunStep("Abc", () =>
            {
                ran = true;
                Assert.TerminateSilently(TestOutcome.Skipped);
            }, null, false, Reflector.Wrap(typeof(TestStep)));
            Assert.IsTrue(ran, "Verify that the step ran.");
            Assert.AreEqual(TestOutcome.Skipped, context.Outcome,
                "Verify that the step has the expected outcome.");
            Assert.AreEqual("Abc", context.TestStep.Name);
            Assert.EndsWith(context.TestStep.CodeElement.CodeReference.TypeName, "TestStep");
            Assert.IsNull(context.TestStep.CodeElement.CodeReference.MemberName);
            Assert.IsFalse(context.TestStep.IsTestCase);
        }

        [Test]
        public void RunStep_NameActionTimeoutIsTestCaseAndCodeElement_ExecutionWithIsTestCaseTrue()
        {
            bool ran = false;
            TestContext context = TestStep.RunStep("Abc", () =>
            {
                ran = true;
            }, null, true, null);
            Assert.IsTrue(ran, "Verify that the step ran.");
            Assert.AreEqual(TestOutcome.Passed, context.Outcome,
                "Verify that the step has the expected outcome.");
            Assert.AreEqual("Abc", context.TestStep.Name);
            Assert.IsNull(context.TestStep.CodeElement);
            Assert.IsTrue(context.TestStep.IsTestCase);
        }

        [Test]
        public void RunStepAndVerifyOutcome_ExecutionWithCorrectOutcome()
        {
            bool ran = false;
            TestContext context = TestStep.RunStepAndVerifyOutcome("Abc", () =>
            {
                ran = true;
                Assert.Inconclusive();
            }, TestOutcome.Inconclusive);
            Assert.IsTrue(ran, "Verify that the step ran.");
            Assert.AreEqual(TestOutcome.Inconclusive, context.Outcome,
                "Verify that the step has the expected outcome.");
            Assert.AreEqual("Abc", context.TestStep.Name);
            Assert.AreEqual("RunStepAndVerifyOutcome_ExecutionWithCorrectOutcome", context.TestStep.CodeElement.CodeReference.MemberName);
            Assert.IsFalse(context.TestStep.IsTestCase);
        }

        [Test]
        public void RunStepAndVerifyOutcome_ExecutionWithIncorrectOutcome()
        {
            bool ran = false;
            TestContext context = null;
            AssertionFailure[] failures = AssertionHelper.Eval(() =>
            {
                context = TestStep.RunStepAndVerifyOutcome("Abc", () =>
                {
                    ran = true;
                    Assert.Inconclusive();
                }, TestOutcome.Passed);
            }, AssertionFailureBehavior.CaptureAndContinue);

            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("The test step did not produce the expected outcome.", failures[0].Description);
            Assert.AreEqual("Expected Outcome", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("passed", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Actual Outcome", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("inconclusive", failures[0].LabeledValues[1].FormattedValue.ToString());

            Assert.IsTrue(ran, "Verify that the step ran.");
            Assert.AreEqual(TestOutcome.Inconclusive, context.Outcome,
                "Verify that the step has the expected outcome.");
            Assert.AreEqual("Abc", context.TestStep.Name);
            Assert.Contains(context.TestStep.CodeElement.CodeReference.MemberName, "RunStepAndVerifyOutcome_ExecutionWithIncorrectOutcome");
            Assert.IsFalse(context.TestStep.IsTestCase);
        }
    }
}