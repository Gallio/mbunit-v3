// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using MbUnit.Core.Model;
using MbUnit.Core.ProgressMonitoring;
using MbUnit.Framework.Kernel.ExecutionLogs;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Plugin.XunitAdapter.Properties;
using Xunit;
using Xunit.Runner;
using Xunit.Sdk;

namespace MbUnit.Plugin.XunitAdapter.Core
{
    /// <summary>
    /// Controls the execution of Xunit tests.
    /// </summary>
    public class XunitTestController : ITestController
    {
        /// <summary>
        /// The metadata key used for recording Xunit's internal test name with a step
        /// when it differs from what MbUnit derived.
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
            else if (xunitTest.MethodInfo != null)
            {
                passed = RunTestMethod(testMonitor, xunitTest.MethodInfo);
            }
            else
            {
                // Previously we marked the fixture as a test case if it had
                // an associated RunWith command.
                if (xunitTest.IsTestCase)
                    passed = RunTestFixtureWithTestClassCommand(testMonitor, xunitTest.TypeInfo);
                else
                    passed = RunTestFixture(progressMonitor, testMonitor, xunitTest.TypeInfo);
            }

            progressMonitor.Worked(1);
            return passed;
        }

        private static bool RunChildTests(IProgressMonitor progressMonitor, ITestMonitor testMonitor)
        {
            IStepMonitor stepMonitor = testMonitor.StartRootStep();

            bool passed = true;
            foreach (ITestMonitor child in testMonitor.Children)
                passed &= RunTest(progressMonitor, child);

            stepMonitor.FinishStep(TestStatus.Executed, passed ? TestOutcome.Passed : TestOutcome.Failed, null);
            return passed;
        }

        private static bool RunTestFixture(IProgressMonitor progressMonitor, ITestMonitor testMonitor, Type typeInfo)
        {
            IStepMonitor stepMonitor = testMonitor.StartRootStep();

            bool passed = true;
            try
            {
                ITestFixture testFixture = null;
                try
                {
                    // Run BeforeAllTests behavior, if applicable.
                    if (typeof(ITestFixture).IsAssignableFrom(typeInfo))
                    {
                        testFixture = (ITestFixture)Activator.CreateInstance(typeInfo);
                        testFixture.BeforeAllTests();
                    }
                    
                    // Run tests.
                    foreach (ITestMonitor child in testMonitor.Children)
                        passed &= RunTest(progressMonitor, child);

                    // Run AfterAllTests behavior, if applicable.
                    if (testFixture != null)
                        testFixture.AfterAllTests();
                }
                finally
                {
                    // Run dispose behavior, if applicable.
                    IDisposable disposable = testFixture as IDisposable;
                    if (disposable != null)
                        disposable.Dispose();
                }
            }
            catch (TargetInvocationException ex)
            {
                stepMonitor.LogWriter[LogStreamNames.Failures].WriteException(ex.InnerException);
                passed = false;
            }
            catch (Exception ex)
            {
                stepMonitor.LogWriter[LogStreamNames.Failures].WriteException(ex);
                passed = false;
            }

            stepMonitor.FinishStep(TestStatus.Executed, passed ? TestOutcome.Passed : TestOutcome.Failed, null);
            return passed;
        }

        private static bool RunTestMethod(ITestMonitor testMonitor, MethodInfo methodInfo)
        {
            IStepMonitor stepMonitor = testMonitor.StartRootStep();

            List<ITestCommand> commands;
            try
            {
                commands = new List<ITestCommand>(TestCommandFactory.Make(methodInfo));
            }
            catch (Exception ex)
            {
                // Xunit can throw exceptions when making commands if the test is malformed.
                stepMonitor.LogWriter[LogStreamNames.Failures].WriteException(ex);
                stepMonitor.FinishStep(TestStatus.Executed, TestOutcome.Failed, null);
                return false;
            }

            // For simple tests, run them as one step.
            if (commands.Count == 1)
                return RunTestCommandAndFinishStep(stepMonitor, commands[0]);

            // For data-driven tests, run them as multiple nested steps.
            bool passed = true;
            int step = 0;
            foreach (ITestCommand command in commands)
            {
                IStepMonitor nestedStepMonitor = stepMonitor.StartChildStep(String.Format("Xunit Test Step {0}", ++step),
                    CodeReference.CreateFromMember(methodInfo));

                bool stepPassed = RunTestCommandAndFinishStep(nestedStepMonitor, command);
                if (!stepPassed)
                    passed = false;
            }

            stepMonitor.FinishStep(TestStatus.Executed, passed ? TestOutcome.Passed : TestOutcome.Failed, null);
            return passed;
        }

        private static bool RunTestCommandAndFinishStep(IStepMonitor stepMonitor, ITestCommand command)
        {
            try
            {
                MethodResult result = command.Execute();
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

        private static bool LogMethodResultAndFinishStep(IStepMonitor stepMonitor, MethodResult result, bool useXunitTime)
        {
            TimeSpan? testTime = useXunitTime ? (TimeSpan?) TimeSpan.FromSeconds(result.TestTime) : null;

            // Record the method name if it's not at all present in the step name.
            // That can happen with data-driven tests.
            string xunitTestName = result.MethodName;
            if (xunitTestName != stepMonitor.Step.Test.Name
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
                // Get the failure exception if any.
                LogStreamWriter failureStream = stepMonitor.LogWriter[LogStreamNames.Failures];
                if (failedResult.Exception != null)
                {
                    failureStream.WriteException(failedResult.Exception);
                }
                else
                {
                    using (failureStream.BeginSection("Exception"))
                    {
                        if (failedResult.Message != null)
                            failureStream.WriteLine(failedResult.Message);

                        if (failedResult.StackTrace != null)
                            failureStream.Write(failedResult.StackTrace);
                    }
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

        private static bool RunTestFixtureWithTestClassCommand(ITestMonitor testMonitor, Type typeInfo)
        {
            IStepMonitor stepMonitor = testMonitor.StartRootStep();

            // Use more general-purpose support base on ITestClassCommand for RunWith.
            // The user experience is not quite as good because we are limited in
            // terms of the amount of declarative information we can derive early on.
            ITestClassCommand command;
            try
            {
                command = TestClassCommandFactory.Make(typeInfo);
            }
            catch (Exception ex)
            {
                // Xunit can throw exceptions when making commands if the test is malformed.
                stepMonitor.LogWriter[LogStreamNames.Failures].WriteException(ex);
                stepMonitor.FinishStep(TestStatus.Executed, TestOutcome.Failed, null);
                return false;
            }

            return RunTestClassCommandAndFinishStep(stepMonitor, command);
        }

        private static bool RunTestClassCommandAndFinishStep(IStepMonitor stepMonitor, ITestClassCommand command)
        {
            try
            {
                bool passed = true;
                ClassResult classResult = command.Execute(delegate(ITestResult result)
                {
                    MethodResult methodResult = result as MethodResult;
                    if (methodResult != null)
                    {
                        IStepMonitor methodStepMonitor = stepMonitor.StartChildStep(methodResult.MethodName, CodeReference.Unknown);

                        foreach (KeyValuePair<string, string> entry in methodResult.Properties)
                            methodStepMonitor.AddMetadata(entry.Key, entry.Value ?? @"");

                        passed &= LogMethodResultAndFinishStep(methodStepMonitor, methodResult, true);
                    }
                });

                if (classResult.FixtureException != null)
                {
                    stepMonitor.LogWriter[LogStreamNames.Failures].WriteException(classResult.FixtureException);
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
    }
}
