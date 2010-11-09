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

namespace Gallio.MbUnitCppAdapter.Model.Tasks
{
    internal class RunTask : ExploreTask
    {
        protected override void Execute(UnmanagedTestRepository repository, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Running " + repository.FileName, 1))
            {
                BuildTestModel(repository, progressMonitor);
                RunTests(repository, progressMonitor);
            }
        }

        private void RunTests(UnmanagedTestRepository repository, IProgressMonitor progressMonitor)
        {
            ITestContextManager testContextManager = new ObservableTestContextManager(TestContextTrackerAccessor.Instance, MessageSink);
            ITestCommandFactory testCommandFactory = new DefaultTestCommandFactory();
            ITestCommand rootTestCommand = testCommandFactory.BuildCommands(TestModel, TestExecutionOptions.FilterSet, 
                TestExecutionOptions.ExactFilter, testContextManager);

            if (rootTestCommand != null)
            {
                RunTest(repository, rootTestCommand, null, progressMonitor);
            }
        }

        private static TestResult RunTest(UnmanagedTestRepository repository, ITestCommand testCommand, TestStep parentTestStep, IProgressMonitor progressMonitor)
        {
            Test test = testCommand.Test;
            progressMonitor.SetStatus(test.Name);
            TestResult result;
            var mbUnitCppTest = test as MbUnitCppTest;

            if ((mbUnitCppTest == null) || mbUnitCppTest.TestInfoData.IsTestFixture)
            {
                result = RunChildTests(repository, testCommand, parentTestStep, progressMonitor);
            }
            else
            {
                result = RunTestStep(repository, testCommand, mbUnitCppTest.TestInfoData, parentTestStep, progressMonitor);
            }

            return result;
        }

        private static TestResult RunChildTests(UnmanagedTestRepository repository, ITestCommand testCommand, TestStep parentTestStep, IProgressMonitor progressMonitor)
        {
            ITestContext testContext = testCommand.StartPrimaryChildStep(parentTestStep);
            TestOutcome combinedOutCome = TestOutcome.Passed;
            var duration = TimeSpan.Zero;
            int assertCount = 0;

            foreach (ITestCommand child in testCommand.Children)
            {
                TestResult testResult = RunTest(repository, child, testContext.TestStep, progressMonitor);
                combinedOutCome.CombineWith(testResult.Outcome);
                duration += testResult.Duration;
                assertCount += testResult.AssertCount;
            }

            TestResult parentTestResult = testContext.FinishStep(combinedOutCome, duration);
            parentTestResult.AssertCount = assertCount;
            return parentTestResult;
        }

        private static TestResult RunTestStep(UnmanagedTestRepository repository, ITestCommand testCommand, TestInfoData testStepInfo, 
            TestStep parentTestStep, IProgressMonitor progressMonitor)
        {
            ITestContext testContext = testCommand.StartPrimaryChildStep(parentTestStep);
            var stopwatch = Stopwatch.StartNew();
            TestStepResult testStepResult = repository.RunTest(testStepInfo);
            stopwatch.Stop();

            if (testStepResult.NativeOutcome == NativeOutcome.FAILED)
            {
                testContext.LogWriter.Failures.WriteLine(testStepResult.Message);
            }

            TestResult testResult = testContext.FinishStep(testStepResult.TestOutcome, stopwatch.Elapsed);
            testResult.AssertCount = testStepResult.AssertCount;
            return testResult;
        }
    }
}
