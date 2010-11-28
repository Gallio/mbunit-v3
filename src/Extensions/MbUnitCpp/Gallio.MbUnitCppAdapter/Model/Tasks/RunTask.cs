// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Common.Messaging;
using Gallio.MbUnitCppAdapter.Model.Bridge;
using Gallio.Model;
using Gallio.Model.Commands;
using Gallio.Model.Contexts;
using Gallio.Model.Messages;
using Gallio.Model.Messages.Execution;
using Gallio.Model.Schema;
using Gallio.Model.Tree;
using Gallio.Runtime.Extensibility.Schema;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Common.Markup;
using Gallio.Framework.Assertions;
using Gallio.Common.Diagnostics;

namespace Gallio.MbUnitCppAdapter.Model.Tasks
{
    /// <summary>
    /// Isolated task which run the tests in a MbUnitCpp unmanaged test repository.
    /// </summary>
    internal class RunTask : ExploreTask
    {
        private AssertionFailureReporter reporter;
        private UnmanagedTestRepository repository;

        /// <inheritdoc />
        protected override void Execute(UnmanagedTestRepository repository, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Running " + repository.FileName, 4))
            {
                progressMonitor.SetStatus("Building the test model.");
                BuildTestModel(repository, progressMonitor);
                progressMonitor.Worked(1);

                progressMonitor.SetStatus("Running the tests.");
                RunTests(repository, progressMonitor);
                progressMonitor.Worked(3);
            }
        }

        private void RunTests(UnmanagedTestRepository repository, IProgressMonitor progressMonitor)
        {
            ITestContextManager testContextManager = new ObservableTestContextManager(TestContextTrackerAccessor.Instance, MessageSink);
            ITestCommandFactory testCommandFactory = new DefaultTestCommandFactory();
            ITestCommand rootTestCommand = testCommandFactory.BuildCommands(TestModel, TestExecutionOptions.FilterSet, TestExecutionOptions.ExactFilter, testContextManager);
            reporter = new AssertionFailureReporter(repository);
            this.repository = repository;

            if (rootTestCommand != null)
            {
                if (TestExecutionOptions.SkipTestExecution)
                {
                    SkipAll(rootTestCommand, null);
                }
                else
                {
                    RunAll(progressMonitor, rootTestCommand);
                }
            }
        }

        private void RunAll(IProgressMonitor progressMonitor, ITestCommand rootTestCommand)
        {
            using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
            {
                using (subProgressMonitor.BeginTask("Running the tests.", rootTestCommand.TestCount))
                {
                    RunTest(rootTestCommand, null, subProgressMonitor);
                }
            }
        }

        private static TestResult SkipAll(ITestCommand testCommand, TestStep parentTestStep)
        {
            ITestContext context = testCommand.StartPrimaryChildStep(parentTestStep);

            foreach (ITestCommand child in testCommand.Children)
                SkipAll(child, context.TestStep);

            return context.FinishStep(TestOutcome.Skipped, null);
        }


        private TestResult RunTest(ITestCommand testCommand, TestStep parentTestStep, IProgressMonitor progressMonitor)
        {
            Test test = testCommand.Test;
            progressMonitor.SetStatus(test.Name);
            var mbUnitCppTest = test as MbUnitCppTest;
            return ((mbUnitCppTest == null) || mbUnitCppTest.TestInfoData.HasChildren)
                ? RunChildTests(testCommand, parentTestStep, progressMonitor)
                : RunTestStep(testCommand, mbUnitCppTest.TestInfoData, parentTestStep, progressMonitor);
        }

        private TestResult RunChildTests(ITestCommand testCommand, TestStep parentTestStep, IProgressMonitor progressMonitor)
        {
            ITestContext testContext = testCommand.StartPrimaryChildStep(parentTestStep);
            TestOutcome combinedOutCome = TestOutcome.Passed;
            var duration = TimeSpan.Zero;

            foreach (ITestCommand child in testCommand.Children)
            {
                TestResult testResult = RunTest(child, testContext.TestStep, progressMonitor);
                combinedOutCome = combinedOutCome.CombineWith(testResult.Outcome);
                duration += testResult.Duration;
            }

            return testContext.FinishStep(combinedOutCome, duration);
        }

        private TestResult RunTestStep(ITestCommand testCommand, TestInfoData testStepInfo, TestStep parentTestStep, IProgressMonitor progressMonitor)
        {
            ITestContext testContext = testCommand.StartPrimaryChildStep(parentTestStep);
            var stopwatch = Stopwatch.StartNew();
            TestStepResult testStepResult = repository.RunTest(testStepInfo);
            stopwatch.Stop();
            reporter.Run(testContext, testStepInfo, testStepResult);
            WriteToTestLog(testContext, testStepResult);
            testContext.AddAssertCount(testStepResult.AssertCount);
            progressMonitor.Worked(1);
            return testContext.FinishStep(testStepResult.TestOutcome, stopwatch.Elapsed);
        }

        private static void WriteToTestLog(ITestContext testContext, TestStepResult testStepResult)
        {
            if (testStepResult.TestLogContents.Length > 0)
                testContext.LogWriter.Default.Write(testStepResult.TestLogContents);
        }
    }
}
