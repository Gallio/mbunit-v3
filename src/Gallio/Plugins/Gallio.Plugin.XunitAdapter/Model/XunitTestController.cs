// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Reflection;
using Gallio.Hosting.ProgressMonitoring;
using Gallio.Logging;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Plugin.XunitAdapter.Properties;
using Xunit.Sdk;
using XunitMethodInfo = Xunit.Sdk.IMethodInfo;

namespace Gallio.Plugin.XunitAdapter.Model
{
    /// <summary>
    /// Controls the execution of Xunit tests.
    /// </summary>
    internal class XunitTestController : ITestController
    {
        /// <summary>
        /// The metadata key used for recording Xunit's internal test name with a step
        /// when it differs from what the adapter derived by itself.
        /// </summary>
        private const string XunitTestNameKey = "Xunit:TestName";

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public void RunTests(IProgressMonitor progressMonitor, ITestMonitor rootTestMonitor)
        {
            using (progressMonitor)
            {
                progressMonitor.BeginTask(Resources.XunitTestController_RunningXunitTests, rootTestMonitor.TestCount);

                RunTest(progressMonitor, rootTestMonitor);
            }
        }

        private static bool RunTest(IProgressMonitor progressMonitor, ITestMonitor testMonitor)
        {
            ITest test = testMonitor.Test;
            progressMonitor.SetStatus(String.Format(Resources.XunitTestController_StatusMessages_RunningTest, test.Name));

            bool passed;
            XunitTest xunitTest = test as XunitTest;
            if (xunitTest == null)
            {
                passed = RunChildTests(progressMonitor, testMonitor);
            }
            else
            {
                passed = RunTestFixture(testMonitor, xunitTest.TypeInfo);
            }

            progressMonitor.Worked(1);
            return passed;
        }

        private static bool RunChildTests(IProgressMonitor progressMonitor, ITestMonitor testMonitor)
        {
            ITestStepMonitor stepMonitor = testMonitor.StartTestInstance();

            bool passed = true;
            foreach (ITestMonitor child in testMonitor.Children)
                passed &= RunTest(progressMonitor, child);

            stepMonitor.FinishStep(TestStatus.Executed, passed ? TestOutcome.Passed : TestOutcome.Failed, null);
            return passed;
        }

        private static bool RunTestFixture(ITestMonitor testMonitor, XunitTypeInfoAdapter typeInfo)
        {
            ITestStepMonitor stepMonitor = testMonitor.StartTestInstance();

            ITestClassCommand testClassCommand;
            try
            {
                testClassCommand = TestClassCommandFactory.Make(typeInfo);
            }
            catch (Exception ex)
            {
                // Xunit can throw exceptions when making commands if the test is malformed.
                stepMonitor.LogWriter[LogStreamNames.Failures].WriteException(ex);
                stepMonitor.FinishStep(TestStatus.Executed, TestOutcome.Failed, null);
                return false;
            }

            return RunTestClassCommandAndFinishStep(testMonitor, stepMonitor, testClassCommand);
        }

        private static bool RunTestClassCommandAndFinishStep(ITestMonitor testMonitor, ITestStepMonitor stepMonitor, ITestClassCommand testClassCommand)
        {
            try
            {
                bool passed = true;

                // Run ClassStart behavior, if applicable.
                stepMonitor.LifecyclePhase = LifecyclePhases.SetUp;
                Exception ex = testClassCommand.ClassStart();

                // Run tests.
                if (ex == null)
                {
                    List<MethodInfo> testMethods = new List<MethodInfo>();
                    List<ITestMonitor> testMonitors = new List<ITestMonitor>();

                    foreach (ITestMonitor child in testMonitor.Children)
                    {
                        XunitTest test = child.Test as XunitTest;

                        if (test != null)
                        {
                            testMethods.Add(test.MethodInfo.Target.Resolve(false));
                            testMonitors.Add(child);
                        }
                    }

                    while (testMethods.Count != 0)
                    {
                        int nextTestIndex = testClassCommand.ChooseNextTest(testMethods.AsReadOnly());
                        ITestMonitor nextTestMonitor = testMonitors[nextTestIndex];
                        MethodInfo nextTestMethodInfo = testMethods[nextTestIndex];

                        testMethods.RemoveAt(nextTestIndex);
                        testMonitors.RemoveAt(nextTestIndex);

                        passed &= RunTestMethod(nextTestMonitor, nextTestMethodInfo, testClassCommand);
                    }
                }

                // Run ClassFinish behavior, if applicable.
                stepMonitor.LifecyclePhase = LifecyclePhases.TearDown;
                ex = testClassCommand.ClassFinish() ?? ex;

                if (ex != null)
                {
                    stepMonitor.LogWriter[LogStreamNames.Failures].WriteException(ex);
                    passed = false;
                }

                stepMonitor.FinishStep(TestStatus.Executed, passed ? TestOutcome.Passed : TestOutcome.Failed, null);
                return passed;
            }
            catch (Exception ex)
            {
                // Xunit probably shouldn't throw an exception in a test command.
                // But just in case...
                stepMonitor.LogWriter[LogStreamNames.Failures].WriteException(ex);
                stepMonitor.FinishStep(TestStatus.Executed, TestOutcome.Failed, null);
                return false;
            }
        }

        private static bool RunTestMethod(ITestMonitor testMonitor, MethodInfo methodInfo, ITestClassCommand testClassCommand)
        {

            List<ITestCommand> testCommands;
            try
            {
                testCommands = new List<ITestCommand>(TestCommandFactory.Make(testClassCommand, methodInfo));
            }
            catch (Exception ex)
            {
                // Xunit can throw exceptions when making commands if the test is malformed.
                ITestStepMonitor stepMonitor = testMonitor.StartTestInstance();
                stepMonitor.LogWriter[LogStreamNames.Failures].WriteException(ex);
                stepMonitor.FinishStep(TestStatus.Executed, TestOutcome.Failed, null);
                return false;
            }

            bool passed = true;
            foreach (ITestCommand testCommand in testCommands)
            {
                ITestStepMonitor stepMonitor = testMonitor.StartTestInstance();
                passed &= RunTestCommandAndFinishStep(stepMonitor, testClassCommand, testCommand);
            }

            return passed;
        }

        private static bool RunTestCommandAndFinishStep(ITestStepMonitor stepMonitor, ITestClassCommand testClassCommand, ITestCommand testCommand)
        {
            try
            {
                MethodResult result = testCommand.Execute(testClassCommand.ObjectUnderTest);
                return LogMethodResultAndFinishStep(stepMonitor, result, false);
            }
            catch (Exception ex)
            {
                // Xunit probably shouldn't throw an exception in a test command.
                // But just in case...
                stepMonitor.LogWriter[LogStreamNames.Failures].WriteException(ex);
                stepMonitor.FinishStep(TestStatus.Executed, TestOutcome.Failed, null);
                return false;
            }
        }

        private static bool LogMethodResultAndFinishStep(ITestStepMonitor stepMonitor, MethodResult result, bool useXunitTime)
        {
            TimeSpan? testTime = useXunitTime ? (TimeSpan?)TimeSpan.FromSeconds(result.TestTime) : null;

            // Record the method name as metadata if it's not at all present in the step name.
            // That can happen with data-driven tests.
            string xunitTestName = result.MethodName;
            if (xunitTestName != stepMonitor.Step.TestInstance.Name
                && xunitTestName != stepMonitor.Step.FullName
                && xunitTestName != stepMonitor.Step.Name)
                stepMonitor.AddMetadata(XunitTestNameKey, xunitTestName);

            if (result is PassedResult)
            {
                stepMonitor.FinishStep(TestStatus.Executed, TestOutcome.Passed, testTime);
                return true;
            }

            FailedResult failedResult = result as FailedResult;
            if (failedResult != null)
            {
                // Get the failure exception.
                LogStreamWriter failureStream = stepMonitor.LogWriter[LogStreamNames.Failures];
                using (failureStream.BeginSection("Exception"))
                {
                    if (failedResult.Message != null)
                        failureStream.WriteLine(failedResult.Message);

                    if (failedResult.StackTrace != null)
                        failureStream.Write(failedResult.StackTrace);
                }

                stepMonitor.FinishStep(TestStatus.Executed, TestOutcome.Failed, testTime);
                return false;
            }

            SkipResult skipResult = result as SkipResult;
            if (skipResult != null)
            {
                if (skipResult.Reason != null)
                    stepMonitor.LogWriter[LogStreamNames.Warnings].Write(skipResult.Reason);

                stepMonitor.FinishStep(TestStatus.Skipped, TestOutcome.Inconclusive, testTime);
                return true;
            }

            throw new NotSupportedException(String.Format("Unrecognized Xunit method result type: '{0}'.", result.GetType()));
        }
    }
}
