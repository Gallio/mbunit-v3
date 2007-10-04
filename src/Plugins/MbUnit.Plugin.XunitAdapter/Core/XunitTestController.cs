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

            IStepMonitor stepMonitor = testMonitor.StartRootStep();

            bool passed;
            XunitTest xunitTest = test as XunitTest;
            if (xunitTest == null)
            {
                passed = RunChildTests(progressMonitor, testMonitor);
            }
            else if (xunitTest.MethodInfo != null)
            {
                passed = RunTestMethod(stepMonitor, xunitTest.MethodInfo);
            }
            else
            {
                passed = RunTestFixture(progressMonitor, testMonitor, stepMonitor, xunitTest.TypeInfo);
            }

            stepMonitor.FinishStep(TestStatus.Executed, passed ? TestOutcome.Passed : TestOutcome.Failed, null);
            progressMonitor.Worked(1);
            return passed;
        }

        private static bool RunChildTests(IProgressMonitor progressMonitor, ITestMonitor testMonitor)
        {
            bool passed = true;
            foreach (ITestMonitor child in testMonitor.Children)
                passed &= RunTest(progressMonitor, child);
            return passed;
        }

        private static bool RunTestFixture(IProgressMonitor progressMonitor, ITestMonitor testMonitor, IStepMonitor stepMonitor, Type typeInfo)
        {
            if (TypeReflectionUtilities.HasRunWith(typeInfo))
            {
                // TODO: A general-purpose Xunit ITestClassCommand is not as easy to integrate.
                // When we do so we will probably observe some degraded functionality because we
                // can't monitor when tests start and finish.
                stepMonitor.LogWriter[LogStreamNames.Failures].WriteLine("Xunit RunWith not currently supported.");
                return false;
            }

            bool passed;
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
                    passed = RunChildTests(progressMonitor, testMonitor);

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

            return passed;
        }

        private static bool RunTestMethod(IStepMonitor stepMonitor, MethodInfo methodInfo)
        {
            List<ITestCommand> commands;
            try
            {
                commands = new List<ITestCommand>(TestCommandFactory.Make(methodInfo));
            }
            catch (Exception ex)
            {
                // Xunit can throw exceptions when making commands if the test is malformed.
                stepMonitor.LogWriter[LogStreamNames.Failures].WriteException(ex);
                return false;
            }

            // For simple tests, run them as one step.
            if (commands.Count == 1)
                return RunTestCommand(commands[0], stepMonitor);

            // For data-driven tests, run them as multiple nested steps.
            bool passed = true;
            int step = 0;
            foreach (ITestCommand command in commands)
            {
                IStepMonitor nestedStepMonitor = stepMonitor.StartChildStep(String.Format("Xunit Test Step {0}", ++step),
                    CodeReference.CreateFromMember(methodInfo));

                bool stepPassed = RunTestCommand(command, nestedStepMonitor);

                nestedStepMonitor.FinishStep(TestStatus.Executed, stepPassed ? TestOutcome.Passed : TestOutcome.Failed, null);

                passed &= stepPassed;
            }
            return passed;
        }

        private static bool RunTestCommand(ITestCommand command, IStepMonitor stepMonitor)
        {
            try
            {
                MethodResult result = command.Execute();

                // Record the method name.
                string xunitTestName = result.MethodName;
                if (xunitTestName != stepMonitor.Step.Test.Name)
                    stepMonitor.AddMetadata(XunitTestNameKey, xunitTestName);

                // Get the failure exception if any.
                FailedResult failedResult = result as FailedResult;
                if (failedResult == null)
                    return true;

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
            }
            catch (Exception ex)
            {
                // Xunit probably shouldn't throw an exception in a test command.
                // But just in case...
                stepMonitor.LogWriter[LogStreamNames.Failures].WriteException(ex);
            }

            return false;
        }
    }
}
