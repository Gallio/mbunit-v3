// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Runtime.ProgressMonitoring;
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
    internal class XunitTestController : BaseTestController
    {
        /// <summary>
        /// The metadata key used for recording Xunit's internal test name with a step
        /// when it differs from what the adapter derived by itself.
        /// </summary>
        private const string XunitTestNameKey = "Xunit:TestName";

        /// <inheritdoc />
        protected override TestOutcome RunTestsImpl(ITestCommand rootTestCommand, ITestStep parentTestStep, TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            using (progressMonitor)
            {
                progressMonitor.BeginTask(Resources.XunitTestController_RunningXunitTests, rootTestCommand.TestCount);

                if (options.SkipTestExecution)
                {
                    SkipAll(rootTestCommand, parentTestStep);
                    return TestOutcome.Skipped;
                }
                else
                {
                    bool success = RunTest(rootTestCommand, parentTestStep, progressMonitor);
                    return success ? TestOutcome.Passed : TestOutcome.Failed;
                }
            }
        }

        private static bool RunTest(ITestCommand testCommand, ITestStep parentTestStep, IProgressMonitor progressMonitor)
        {
            ITest test = testCommand.Test;
            progressMonitor.SetStatus(test.Name);

            bool passed;
            XunitTest xunitTest = test as XunitTest;
            if (xunitTest == null)
            {
                passed = RunChildTests(testCommand, parentTestStep, progressMonitor);
            }
            else
            {
                passed = RunTestFixture(testCommand, xunitTest.TypeInfo, parentTestStep);
            }

            progressMonitor.Worked(1);
            return passed;
        }

        private static bool RunChildTests(ITestCommand testCommand, ITestStep parentTestStep, IProgressMonitor progressMonitor)
        {
            ITestContext testContext = testCommand.StartPrimaryChildStep(parentTestStep);

            bool passed = true;
            foreach (ITestCommand child in testCommand.Children)
                passed &= RunTest(child, testContext.TestStep, progressMonitor);

            testContext.FinishStep(passed ? TestOutcome.Passed : TestOutcome.Failed, null);
            return passed;
        }

        private static bool RunTestFixture(ITestCommand testCommand, XunitTypeInfoAdapter typeInfo,
            ITestStep parentTestStep)
        {
            ITestContext testContext = testCommand.StartPrimaryChildStep(parentTestStep);

            XunitTestClassCommand testClassCommand;
            try
            {
                testClassCommand = XunitTestClassCommandFactory.Make(typeInfo);
            }
            catch (Exception ex)
            {
                // Xunit can throw exceptions when making commands if the test is malformed.
                TestLogWriterUtils.WriteException(testContext.LogWriter, LogStreamNames.Failures, ex, "Internal Error");
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
                            testContext.TestStep);
                    }
                }

                // Run ClassFinish behavior, if applicable.
                testContext.LifecyclePhase = LifecyclePhases.TearDown;
                ex = testClassCommand.ClassFinish() ?? ex;

                if (ex != null)
                {
                    TestLogWriterUtils.WriteException(testContext.LogWriter, LogStreamNames.Failures, ex, "Internal Error");
                    passed = false;
                }

                testContext.FinishStep(passed ? TestOutcome.Passed : TestOutcome.Failed, null);
                return passed;
            }
            catch (Exception ex)
            {
                // Xunit probably shouldn't throw an exception in a test command.
                // But just in case...
                TestLogWriterUtils.WriteException(testContext.LogWriter, LogStreamNames.Failures, ex, "Internal Error");
                testContext.FinishStep(TestOutcome.Failed, null);
                return false;
            }
        }

        private static bool RunTestMethod(ITestCommand testCommand, MethodInfo methodInfo, XunitTestClassCommand testClassCommand,
            ITestStep parentTestStep)
        {
            List<XunitTestCommand> xunitTestCommands;
            try
            {
                xunitTestCommands = new List<XunitTestCommand>(XunitTestCommandFactory.Make(testClassCommand, methodInfo));
            }
            catch (Exception ex)
            {
                // Xunit can throw exceptions when making commands if the test is malformed.
                ITestContext testContext = testCommand.StartPrimaryChildStep(parentTestStep);
                TestLogWriterUtils.WriteException(testContext.LogWriter, LogStreamNames.Failures, ex, "Internal Error");
                testContext.FinishStep(TestOutcome.Failed, null);
                return false;
            }

            bool passed = true;
            foreach (XunitTestCommand xunitTestCommand in xunitTestCommands)
            {
                ITestContext testContext = testCommand.StartPrimaryChildStep(parentTestStep);
                passed &= RunTestCommandAndFinishStep(testContext, testClassCommand, xunitTestCommand);
            }

            return passed;
        }

        private static bool RunTestCommandAndFinishStep(ITestContext testContext, XunitTestClassCommand testClassCommand, XunitTestCommand testCommand)
        {
            try
            {
                testContext.LifecyclePhase = LifecyclePhases.Execute;

                XunitMethodResult result = testCommand.Execute(testClassCommand.ObjectUnderTest);
                return LogMethodResultAndFinishStep(testContext, result, false);
            }
            catch (Exception ex)
            {
                // Xunit probably shouldn't throw an exception in a test command.
                // But just in case...
                TestLogWriterUtils.WriteException(testContext.LogWriter, LogStreamNames.Failures, ex, "Internal Error");
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
            if (xunitTestName != testContext.TestStep.Name)
                testContext.AddMetadata(XunitTestNameKey, xunitTestName);

            if (result is XunitPassedResult)
            {
                testContext.FinishStep(TestOutcome.Passed, testTime);
                return true;
            }

            XunitFailedResult failedResult = result as XunitFailedResult;
            if (failedResult != null)
            {
                // Report the failure exception.
                testContext.LogWriter.BeginSection(LogStreamNames.Failures, "Exception");

                if (failedResult.Message != null)
                    testContext.LogWriter.Write(LogStreamNames.Failures, failedResult.Message + "\n");
                if (failedResult.StackTrace != null)
                    testContext.LogWriter.Write(LogStreamNames.Failures, failedResult.StackTrace);

                testContext.LogWriter.EndSection(LogStreamNames.Failures);

                testContext.FinishStep(TestOutcome.Failed, testTime);
                return false;
            }

            XunitSkipResult skipResult = result as XunitSkipResult;
            if (skipResult != null)
            {
                testContext.LogWriter.Write(LogStreamNames.Warnings, String.Format("The test was skipped.  Reason: {0}\n",
                    string.IsNullOrEmpty(skipResult.Reason) ? "<unspecified>" : skipResult.Reason));
                testContext.FinishStep(TestOutcome.Skipped, testTime);
                return true;
            }

            throw new NotSupportedException(String.Format("Unrecognized Xunit method result type: '{0}'.", result.GetType()));
        }
    }
}
