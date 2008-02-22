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
using Gallio.XunitAdapter.Properties;
using XunitMethodInfo = Xunit.Sdk.IMethodInfo;
using XunitMethodResult = Xunit.Sdk.MethodResult;
using XunitPassedResult = Xunit.Sdk.PassedResult;
using XunitFailedResult = Xunit.Sdk.FailedResult;
using XunitSkipResult = Xunit.Sdk.SkipResult;
using XunitTestClassCommand = Xunit.Sdk.ITestClassCommand;
using XunitTestClassCommandFactory = Xunit.Sdk.TestClassCommandFactory;
using XunitTestCommand = Xunit.Sdk.ITestCommand;
using XunitTestCommandFactory = Xunit.Sdk.TestCommandFactory;

namespace Gallio.XunitAdapter.Model
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
        public void RunTests(IProgressMonitor progressMonitor, ITestCommand rootTestCommand,
            ITestInstance parentTestInstance)
        {
            using (progressMonitor)
            {
                progressMonitor.BeginTask(Resources.XunitTestController_RunningXunitTests, rootTestCommand.TestCount);

                RunTest(progressMonitor, rootTestCommand, parentTestInstance);
            }
        }

        private static bool RunTest(IProgressMonitor progressMonitor, ITestCommand testCommand,
            ITestInstance parentTestInstance)
        {
            ITest test = testCommand.Test;
            progressMonitor.SetStatus(String.Format(Resources.XunitTestController_StatusMessages_RunningTest, test.Name));

            bool passed;
            XunitTest xunitTest = test as XunitTest;
            if (xunitTest == null)
            {
                passed = RunChildTests(progressMonitor, testCommand, parentTestInstance);
            }
            else
            {
                passed = RunTestFixture(testCommand, xunitTest.TypeInfo, parentTestInstance);
            }

            progressMonitor.Worked(1);
            return passed;
        }

        private static bool RunChildTests(IProgressMonitor progressMonitor, ITestCommand testCommand,
            ITestInstance parentTestInstance)
        {
            ITestContext testContext = testCommand.StartRootStep(parentTestInstance);

            bool passed = true;
            foreach (ITestCommand child in testCommand.Children)
                passed &= RunTest(progressMonitor, child, testContext.TestStep.TestInstance);

            testContext.FinishStep(passed ? TestOutcome.Passed : TestOutcome.Failed, null);
            return passed;
        }

        private static bool RunTestFixture(ITestCommand testCommand, XunitTypeInfoAdapter typeInfo,
            ITestInstance parentTestInstance)
        {
            ITestContext testContext = testCommand.StartRootStep(parentTestInstance);

            XunitTestClassCommand testClassCommand;
            try
            {
                testClassCommand = XunitTestClassCommandFactory.Make(typeInfo);
            }
            catch (Exception ex)
            {
                // Xunit can throw exceptions when making commands if the test is malformed.
                testContext.LogWriter[LogStreamNames.Failures].WriteException(ex);
                testContext.FinishStep(TestOutcome.Failed, null);
                return false;
            }

            return RunTestClassCommandAndFinishStep(testCommand, testContext, testClassCommand);
        }

        private static bool RunTestClassCommandAndFinishStep(ITestCommand testCommand, ITestContext testContext, XunitTestClassCommand testClassCommand)
        {
            try
            {
                bool passed = true;

                // Run ClassStart behavior, if applicable.
                testContext.LifecyclePhase = LifecyclePhases.SetUp;
                Exception ex = testClassCommand.ClassStart();

                // Run tests.
                if (ex == null)
                {
                    List<MethodInfo> testMethods = new List<MethodInfo>();
                    List<ITestCommand> testCommands = new List<ITestCommand>();

                    foreach (ITestCommand child in testCommand.Children)
                    {
                        XunitTest test = child.Test as XunitTest;

                        if (test != null)
                        {
                            testMethods.Add(test.MethodInfo.Target.Resolve(false));
                            testCommands.Add(child);
                        }
                    }

                    while (testMethods.Count != 0)
                    {
                        int nextTestIndex = testClassCommand.ChooseNextTest(testMethods.AsReadOnly());
                        ITestCommand nextTestCommand = testCommands[nextTestIndex];
                        MethodInfo nextTestMethodInfo = testMethods[nextTestIndex];

                        testMethods.RemoveAt(nextTestIndex);
                        testCommands.RemoveAt(nextTestIndex);

                        passed &= RunTestMethod(nextTestCommand, nextTestMethodInfo, testClassCommand,
                            testContext.TestStep.TestInstance);
                    }
                }

                // Run ClassFinish behavior, if applicable.
                testContext.LifecyclePhase = LifecyclePhases.TearDown;
                ex = testClassCommand.ClassFinish() ?? ex;

                if (ex != null)
                {
                    testContext.LogWriter[LogStreamNames.Failures].WriteException(ex);
                    passed = false;
                }

                testContext.FinishStep(passed ? TestOutcome.Passed : TestOutcome.Failed, null);
                return passed;
            }
            catch (Exception ex)
            {
                // Xunit probably shouldn't throw an exception in a test command.
                // But just in case...
                testContext.LogWriter[LogStreamNames.Failures].WriteException(ex);
                testContext.FinishStep(TestOutcome.Failed, null);
                return false;
            }
        }

        private static bool RunTestMethod(ITestCommand testCommand, MethodInfo methodInfo, XunitTestClassCommand testClassCommand,
            ITestInstance parentTestInstance)
        {
            List<XunitTestCommand> xunitTestCommands;
            try
            {
                xunitTestCommands = new List<XunitTestCommand>(XunitTestCommandFactory.Make(testClassCommand, methodInfo));
            }
            catch (Exception ex)
            {
                // Xunit can throw exceptions when making commands if the test is malformed.
                ITestContext testContext = testCommand.StartRootStep(parentTestInstance);
                testContext.LogWriter[LogStreamNames.Failures].WriteException(ex);
                testContext.FinishStep(TestOutcome.Failed, null);
                return false;
            }

            bool passed = true;
            foreach (XunitTestCommand xunitTestCommand in xunitTestCommands)
            {
                ITestContext testContext = testCommand.StartRootStep(parentTestInstance);
                passed &= RunTestCommandAndFinishStep(testContext, testClassCommand, xunitTestCommand);
            }

            return passed;
        }

        private static bool RunTestCommandAndFinishStep(ITestContext testContext, XunitTestClassCommand testClassCommand, XunitTestCommand testCommand)
        {
            try
            {
                XunitMethodResult result = testCommand.Execute(testClassCommand.ObjectUnderTest);
                return LogMethodResultAndFinishStep(testContext, result, false);
            }
            catch (Exception ex)
            {
                // Xunit probably shouldn't throw an exception in a test command.
                // But just in case...
                testContext.LogWriter[LogStreamNames.Failures].WriteException(ex);
                testContext.FinishStep(TestOutcome.Failed, null);
                return false;
            }
        }

        private static bool LogMethodResultAndFinishStep(ITestContext testContext, XunitMethodResult result, bool useXunitTime)
        {
            TimeSpan? testTime = useXunitTime ? (TimeSpan?)TimeSpan.FromSeconds(result.TestTime) : null;

            // Record the method name as metadata if it's not at all present in the step name.
            // That can happen with data-driven tests.
            string xunitTestName = result.MethodName;
            if (xunitTestName != testContext.TestStep.TestInstance.Name
                && xunitTestName != testContext.TestStep.FullName
                && xunitTestName != testContext.TestStep.Name)
                testContext.AddMetadata(XunitTestNameKey, xunitTestName);

            if (result is XunitPassedResult)
            {
                testContext.FinishStep(TestOutcome.Passed, testTime);
                return true;
            }

            XunitFailedResult failedResult = result as XunitFailedResult;
            if (failedResult != null)
            {
                // Get the failure exception.
                LogStreamWriter failureStream = testContext.LogWriter[LogStreamNames.Failures];
                using (failureStream.BeginSection("Exception"))
                {
                    if (failedResult.Message != null)
                        failureStream.WriteLine(failedResult.Message);

                    if (failedResult.StackTrace != null)
                        failureStream.Write(failedResult.StackTrace);
                }

                testContext.FinishStep(TestOutcome.Failed, testTime);
                return false;
            }

            XunitSkipResult skipResult = result as XunitSkipResult;
            if (skipResult != null)
            {
                testContext.LogWriter[LogStreamNames.Warnings].WriteLine("The test was skipped.  Reason: {0}",
                    string.IsNullOrEmpty(skipResult.Reason) ? "<unspecified>" : skipResult.Reason);
                testContext.FinishStep(TestOutcome.Skipped, testTime);
                return true;
            }

            throw new NotSupportedException(String.Format("Unrecognized Xunit method result type: '{0}'.", result.GetType()));
        }
    }
}
