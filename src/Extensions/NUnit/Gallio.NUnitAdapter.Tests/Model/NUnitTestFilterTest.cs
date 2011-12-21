using System.Collections.Generic;
using Gallio.Model.Commands;
using Gallio.NUnitAdapter.Model;
using MbUnit.Framework;
using NHamcrest.Core;
using NUnit.Core;
using Rhino.Mocks;

namespace Gallio.NUnitAdapter.Tests.Model
{
    public class NUnitTestFilterTest
    {
        private ITestCommand testCommand;
        private TestName testName;
        private NUnitTestFilter testFilter;
        private ITest test;

        [SetUp]
        public void SetUp()
        {
            testCommand = MockRepository.GenerateStub<ITestCommand>();
            testName = new TestName { TestID = new TestID(), FullName = "fullName" };
            var testCommandsByTestName = new Dictionary<TestName, ITestCommand> { { testName, testCommand } };
            testFilter = new NUnitTestFilter(testCommandsByTestName);
            test = MockRepository.GenerateStub<ITest>();
        }

        [Test]
        public void Match_returns_false_if_test_name_does_not_match_a_command()
        {
            test.Stub(t => t.TestName).Return(new TestName { TestID = new TestID(), FullName = "" });

            var result = testFilter.Match(test);

            Assert.That(result, Is.False());
        }

        [Test]
        [Row(RunState.Ignored)]
        [Row(RunState.NotRunnable)]
        [Row(RunState.Runnable)]
        [Row(RunState.Skipped)]
        public void Match_returns_true_if_run_state_is_not_explicit(RunState runState)
        {
            test.Stub(t => t.TestName).Return(testName);
            test.RunState = runState;

            var result = testFilter.Match(test);

            Assert.That(result, Is.True());
        }

        [Test]
        public void Match_returns_true_if_run_state_is_explicit_and_command_is_explicit()
        {
            test.Stub(t => t.TestName).Return(testName);
            test.RunState = RunState.Explicit;
            testCommand.Stub(tc => tc.IsExplicit).Return(true);

            var result = testFilter.Match(test);

            Assert.That(result, Is.True());
        }

        [Test]
        public void Match_returns_false_if_run_state_is_explicit_and_command_is_not_explicit()
        {
            test.Stub(t => t.TestName).Return(testName);
            test.RunState = RunState.Explicit;
            testCommand.Stub(tc => tc.IsExplicit).Return(false);

            var result = testFilter.Match(test);

            Assert.That(result, Is.False());
        }

        [Test]
        public void Pass_returns_false_if_test_name_does_not_match_a_command()
        {
            test.Stub(t => t.TestName).Return(new TestName { TestID = new TestID(), FullName = "" });

            var result = testFilter.Pass(test);

            Assert.That(result, Is.False());
        }

        [Test]
        [Row(RunState.Ignored)]
        [Row(RunState.NotRunnable)]
        [Row(RunState.Runnable)]
        [Row(RunState.Skipped)]
        public void Pass_returns_true_if_run_state_is_not_explicit(RunState runState)
        {
            test.Stub(t => t.TestName).Return(testName);
            test.RunState = runState;

            var result = testFilter.Pass(test);

            Assert.That(result, Is.True());
        }

        [Test]
        public void Pass_returns_true_if_run_state_is_explicit_and_command_is_explicit()
        {
            test.Stub(t => t.TestName).Return(testName);
            test.RunState = RunState.Explicit;
            testCommand.Stub(tc => tc.IsExplicit).Return(true);

            var result = testFilter.Pass(test);

            Assert.That(result, Is.True());
        }

        [Test]
        public void Pass_returns_false_if_run_state_is_explicit_and_command_is_not_explicit()
        {
            test.Stub(t => t.TestName).Return(testName);
            test.RunState = RunState.Explicit;
            testCommand.Stub(tc => tc.IsExplicit).Return(false);

            var result = testFilter.Pass(test);

            Assert.That(result, Is.False());
        }

        [Test]
        public void Is_empty_returns_false()
        {
            Assert.That(testFilter.IsEmpty, Is.False());
        }
    }
}