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
using System.Collections.Generic;
using System.Reflection;
using Gallio.Common.Collections;
using Gallio.Common.Diagnostics;
using Gallio.Common.Reflection;
using Gallio.Model.Commands;
using Gallio.Model.Contexts;
using Gallio.Model.Helpers;
using Gallio.Model.Tree;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Model;
using Gallio.XunitAdapter.Properties;
using XunitReflector = Xunit.Sdk.Reflector;
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
    internal class XunitTestController : TestController
    {
        protected override TestResult RunImpl(ITestCommand rootTestCommand, TestStep parentTestStep, TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask(Resources.XunitTestController_RunningXunitTests, rootTestCommand.TestCount))
            {
                if (options.SkipTestExecution)
                {
                    return SkipAll(rootTestCommand, parentTestStep);
                }
                else
                {
                    return RunTest(rootTestCommand, parentTestStep, progressMonitor);
                }
            }
        }

        private static TestResult RunTest(ITestCommand testCommand, TestStep parentTestStep, IProgressMonitor progressMonitor)
        {
            Test test = testCommand.Test;
            progressMonitor.SetStatus(test.Name);

            TestResult result;
            XunitTest xunitTest = test as XunitTest;
            if (xunitTest == null)
            {
                result = RunChildTests(testCommand, parentTestStep, progressMonitor);
            }
            else
            {
                result = RunTestFixture(testCommand, xunitTest.TypeInfo, parentTestStep);
            }

            progressMonitor.Worked(1);
            return result;
        }

        private static TestResult RunChildTests(ITestCommand testCommand, TestStep parentTestStep, IProgressMonitor progressMonitor)
        {
            ITestContext testContext = testCommand.StartPrimaryChildStep(parentTestStep);

            bool passed = true;
            foreach (ITestCommand child in testCommand.Children)
                passed &= RunTest(child, testContext.TestStep, progressMonitor).Outcome.Status == TestStatus.Passed;

            return testContext.FinishStep(passed ? TestOutcome.Passed : TestOutcome.Failed, null);
        }

        private static TestResult RunTestFixture(ITestCommand testCommand, XunitTypeInfoAdapter typeInfo,
            TestStep parentTestStep)
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
                testContext.LogWriter.Failures.WriteException(ex, "Internal Error");
                return testContext.FinishStep(TestOutcome.Failed, null);
            }

            return RunTestClassCommandAndFinishStep(testCommand, testContext, testClassCommand);
        }

        private static TestResult RunTestClassCommandAndFinishStep(ITestCommand testCommand, ITestContext testContext, XunitTestClassCommand testClassCommand)
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
                        XunitMethodInfo[] xunitTestMethods = GenericCollectionUtils.ConvertAllToArray(
                            testMethods, x => XunitReflector.Wrap(x));

                        int nextTestIndex = testClassCommand.ChooseNextTest(xunitTestMethods);
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
                    testContext.LogWriter.Failures.WriteException(ex, "Internal Error");
                    passed = false;
                }

                return testContext.FinishStep(passed ? TestOutcome.Passed : TestOutcome.Failed, null);
            }
            catch (Exception ex)
            {
                // Xunit probably shouldn't throw an exception in a test command.
                // But just in case...
                testContext.LogWriter.Failures.WriteException(ex, "Internal Error");
                return testContext.FinishStep(TestOutcome.Failed, null);
            }
        }

        private static bool RunTestMethod(ITestCommand testCommand, MethodInfo methodInfo, XunitTestClassCommand testClassCommand,
            TestStep parentTestStep)
        {
            List<XunitTestCommand> xunitTestCommands;
            try
            {
                xunitTestCommands = new List<XunitTestCommand>(XunitTestCommandFactory.Make(testClassCommand,
                    XunitReflector.Wrap(methodInfo)));
            }
            catch (Exception ex)
            {
                // Xunit can throw exceptions when making commands if the test is malformed.
                ITestContext testContext = testCommand.StartPrimaryChildStep(parentTestStep);
                testContext.LogWriter.Failures.WriteException(ex, "Internal Error");
                testContext.FinishStep(TestOutcome.Failed, null);
                return false;
            }

            if (xunitTestCommands.Count == 0)
                return true;

            if (xunitTestCommands.Count == 1)
                return RunTestCommands(testCommand, testClassCommand, xunitTestCommands, parentTestStep, true);

            // Introduce a common primary test step for theories.
            ITestContext primaryTestContext = testCommand.StartPrimaryChildStep(parentTestStep);
            bool result = RunTestCommands(testCommand, testClassCommand, xunitTestCommands, primaryTestContext.TestStep, false);
            primaryTestContext.FinishStep(result ? TestOutcome.Passed : TestOutcome.Failed, null);
            return result;
        }

        private static bool RunTestCommands(ITestCommand testCommand, XunitTestClassCommand testClassCommand,
            IEnumerable<XunitTestCommand> xunitTestCommands, TestStep parentTestStep, bool isPrimary)
        {
            bool passed = true;
            foreach (XunitTestCommand xunitTestCommand in xunitTestCommands)
            {
                TestStep testStep = new TestStep(testCommand.Test, parentTestStep,
                    testCommand.Test.Name, testCommand.Test.CodeElement, isPrimary);
                testStep.IsDynamic = !isPrimary;

                string displayName = xunitTestCommand.DisplayName;
                if (displayName != null)
                    testStep.Name = StripTypeNamePrefixFromDisplayName(testCommand.Test.CodeElement, displayName);

                ITestContext testContext = testCommand.StartStep(testStep);
                passed &= RunTestCommandAndFinishStep(testContext, testClassCommand, xunitTestCommand);
            }

            return passed;
        }

        private static string StripTypeNamePrefixFromDisplayName(ICodeElementInfo codeElement, string displayName)
        {
            IMemberInfo member = codeElement as IMemberInfo;
            if (member != null)
            {
                ITypeInfo type = member.ReflectedType;
                if (type != null)
                {
                    string typeNamePlusDot = type.FullName + ".";
                    if (displayName.StartsWith(typeNamePlusDot))
                        return displayName.Substring(typeNamePlusDot.Length);
                }
            }

            return displayName;
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
                testContext.LogWriter.Failures.WriteException(ex, "Internal Error");
                testContext.FinishStep(TestOutcome.Failed, null);
                return false;
            }
        }

        private static bool LogMethodResultAndFinishStep(ITestContext testContext, XunitMethodResult result, bool useXunitTime)
        {
            TimeSpan? testTime = useXunitTime ? (TimeSpan?)TimeSpan.FromSeconds(result.ExecutionTime) : null;

            if (!string.IsNullOrEmpty(result.Output))
                testContext.LogWriter.ConsoleOutput.Write(result.Output);

            if (result is XunitPassedResult)
            {
                testContext.FinishStep(TestOutcome.Passed, testTime);
                return true;
            }

            XunitFailedResult failedResult = result as XunitFailedResult;
            if (failedResult != null)
            {
                // Report the failure exception.
                testContext.LogWriter.Failures.WriteException(
                    new ExceptionData(failedResult.ExceptionType ?? "", failedResult.Message ?? "", failedResult.StackTrace ?? "", null), "Exception");

                testContext.FinishStep(TestOutcome.Failed, testTime);
                return false;
            }

            XunitSkipResult skipResult = result as XunitSkipResult;
            if (skipResult != null)
            {
                testContext.LogWriter.Warnings.Write("The test was skipped.  Reason: {0}\n",
                    string.IsNullOrEmpty(skipResult.Reason) ? "<unspecified>" : skipResult.Reason);
                testContext.FinishStep(TestOutcome.Skipped, testTime);
                return true;
            }

            throw new NotSupportedException(String.Format("Unrecognized Xunit method result type: '{0}'.", result.GetType()));
        }
    }
}
