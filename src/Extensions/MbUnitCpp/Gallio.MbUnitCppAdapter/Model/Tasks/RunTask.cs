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

            if (rootTestCommand != null)
            {
                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
                {
                    using (subProgressMonitor.BeginTask("Running the tests.", rootTestCommand.TestCount))
                    {
                        RunTest(repository, rootTestCommand, null, subProgressMonitor);
                    }
                }
            }
        }

        private static TestResult RunTest(UnmanagedTestRepository repository, ITestCommand testCommand, TestStep parentTestStep, IProgressMonitor progressMonitor)
        {
            Test test = testCommand.Test;
            progressMonitor.SetStatus(test.Name);
            var mbUnitCppTest = test as MbUnitCppTest;
            return ((mbUnitCppTest == null) || mbUnitCppTest.TestInfoData.IsTestFixture)
                ? RunChildTests(repository, testCommand, parentTestStep, progressMonitor)
                : RunTestStep(repository, testCommand, mbUnitCppTest.TestInfoData, parentTestStep, progressMonitor);
        }

        private static TestResult RunChildTests(UnmanagedTestRepository repository, ITestCommand testCommand, TestStep parentTestStep, IProgressMonitor progressMonitor)
        {
            ITestContext testContext = testCommand.StartPrimaryChildStep(parentTestStep);
            TestOutcome combinedOutCome = TestOutcome.Passed;
            var duration = TimeSpan.Zero;

            foreach (ITestCommand child in testCommand.Children)
            {
                TestResult testResult = RunTest(repository, child, testContext.TestStep, progressMonitor);
                combinedOutCome = combinedOutCome.CombineWith(testResult.Outcome);
                duration += testResult.Duration;
            }

            return testContext.FinishStep(combinedOutCome, duration);
        }

        private static TestResult RunTestStep(UnmanagedTestRepository repository, ITestCommand testCommand, TestInfoData testStepInfo, TestStep parentTestStep, IProgressMonitor progressMonitor)
        {
            ITestContext testContext = testCommand.StartPrimaryChildStep(parentTestStep);
            var stopwatch = Stopwatch.StartNew();
            TestStepResult testStepResult = repository.RunTest(testStepInfo);
            stopwatch.Stop();
            ReportFailure(repository, testContext, testStepInfo, testStepResult);
            testContext.AddAssertCount(testStepResult.AssertCount);
            progressMonitor.Worked(1);
            return testContext.FinishStep(testStepResult.TestOutcome, stopwatch.Elapsed);
        }

        private static void ReportFailure(UnmanagedTestRepository repository, ITestContext testContext, TestInfoData testInfoData, TestStepResult testStepResult)
        {
            if (testStepResult.TestOutcome == TestOutcome.Failed)
            {
                MbUnitCppAssertionFailure failure = testStepResult.Failure;
                var builder = new AssertionFailureBuilder(failure.Description);

                if (failure.HasExpectedValue && failure.HasActualValue && failure.Diffing)
                {
                    builder.AddRawExpectedAndActualValuesWithDiffs(failure.ExpectedValue, failure.ActualValue);
                }
                else
                {
                    if (failure.HasExpectedValue)
                        builder.AddRawActualValue(failure.ExpectedValue);

                    if (failure.HasActualValue)
                        builder.AddRawActualValue(failure.ActualValue);
                }

                if (failure.Message.Length > 0)
                    builder.SetMessage(failure.Message);

                builder.SetStackTrace(testInfoData.GetStackTraceData());
                builder.ToAssertionFailure().WriteTo(testContext.LogWriter.Failures);
            }
        }
    }
}
