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
using Gallio;
using Gallio.Reflection;
using Gallio.Model.Execution;
using Gallio.Hosting.ProgressMonitoring;
using Gallio.Logging;
using Gallio.Model;

namespace Gallio.Framework.Explorer
{
    /// <summary>
    /// Controls the execution of <see cref="PatternTest" /> instances.
    /// </summary>
    /// <seealso cref="PatternTestFramework"/>
    /// <todo author="jeff">
    /// VERY INCOMPLETE!
    /// </todo>
    public class PatternTestController : ITestController
    {
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
                progressMonitor.BeginTask("Running tests.", rootTestCommand.TestCount);

                RunTest(progressMonitor, rootTestCommand, parentTestInstance, null);
            }
        }

        private TestOutcome RunTest(IProgressMonitor progressMonitor, ITestCommand testCommand, ITestInstance parentTestInstance,
            PatternTestState parentState)
        {
            progressMonitor.SetStatus(String.Format("Run test: {0}.", testCommand.Test.Name));

            ITestContext testContext = testCommand.StartRootStep(parentTestInstance);
            try
            {
                PatternTest test = (PatternTest)testCommand.Test;

                string ignoreReason = test.Metadata.GetValue(MetadataKeys.IgnoreReason);
                if (ignoreReason != null)
                {
                    testContext.LogWriter[LogStreamNames.Warnings].WriteLine("Ignored: {0}", ignoreReason);
                    testContext.FinishStep(TestStatus.Ignored, TestOutcome.Inconclusive, null);
                    return TestOutcome.Inconclusive;
                }
                else
                {
                    PatternTestState state = new PatternTestState(test);

                    TestOutcome outcome = InitializeTest(testContext, state, parentState);

                    if (outcome == TestOutcome.Passed)
                    {
                        CombineOutcome(ref outcome, RunSetup(testContext, state, parentState));

                        if (outcome == TestOutcome.Passed)
                        {
                            CombineOutcome(ref outcome, RunExecute(testContext, state));

                            if (outcome == TestOutcome.Passed)
                            {
                                foreach (ITestCommand child in testCommand.Children)
                                {
                                    if (RunTest(progressMonitor, child, testContext.TestStep.TestInstance, state) == TestOutcome.Failed)
                                        outcome = TestOutcome.Failed;
                                }
                            }
                        }

                        CombineOutcome(ref outcome, RunTearDown(testContext, state, parentState));
                    }

                    testContext.FinishStep(TestStatus.Executed, outcome, null);
                    return outcome;
                }
            }
            catch (Exception ex)
            {
                testContext.LogWriter[LogStreamNames.Failures].WriteException(ex, "A fatal test runner exception occurred.");

                testContext.FinishStep(TestStatus.Error, TestOutcome.Failed, null);
                return TestOutcome.Failed;
            }
            finally
            {
                progressMonitor.Worked(1);
            }
        }

        private TestOutcome InitializeTest(ITestContext testContext, PatternTestState state, PatternTestState parentState)
        {
            return ExecuteSafely(testContext, LifecyclePhases.Initialize, delegate
            {
                // FIXME: HACK!!!!
                // The real implementation will involve the test in the initialization
                // of its own state.  Thus fixtures can decide how to create instances
                // of themselves and how to pass them to child tests and so on.

                ITypeInfo type = state.Test.CodeElement as ITypeInfo;
                if (type != null)
                    state.FixtureInstance = Activator.CreateInstance(type.Resolve(true));
                else if (parentState != null)
                    state.FixtureInstance = parentState.FixtureInstance;
            }, "An exception occurred during initialization.");
        }

        private TestOutcome RunSetup(ITestContext testContext, PatternTestState state, PatternTestState parentState)
        {
            return ExecuteSafely(testContext, LifecyclePhases.SetUp, delegate
            {
                if (parentState != null)
                    parentState.Test.BeforeChildChain.Action(parentState);

                state.Test.SetUpChain.Action(state);
            }, "An exception occurred during set up.");
        }

        private TestOutcome RunExecute(ITestContext testContext, PatternTestState state)
        {
            return ExecuteSafely(testContext, LifecyclePhases.Execute, delegate
            {
                state.Test.ExecuteChain.Action(state);
            }, "An exception occurred during execution.");
        }

        private TestOutcome RunTearDown(ITestContext testContext, PatternTestState state, PatternTestState parentState)
        {
            return ExecuteSafely(testContext, LifecyclePhases.TearDown, delegate
            {
                state.Test.TearDownChain.Action(state);

                if (parentState != null)
                    parentState.Test.AfterChildChain.Action(parentState);
            }, "An exception occurred during tear down.");
        }

        private TestOutcome ExecuteSafely(ITestContext testContext, string lifecyclePhase, Action action, string failureHeading)
        {
            testContext.LifecyclePhase = lifecyclePhase;

            try
            {
                action();
                return TestOutcome.Passed;
            }
            catch (Exception ex)
            {
                TestOutcome outcome = TestOutcome.Failed;

                if (ex is ClientException)
                    ex = ex.InnerException;
                if (ex is TestException)
                    outcome = ((TestException)ex).Outcome;

                LogException(testContext, ex, outcome, failureHeading);
                return outcome;
            }
        }

        private static void LogException(ITestContext testContext, Exception ex, TestOutcome outcome, string failureHeading)
        {
            string streamName;
            switch (outcome)
            {
                default:
                case TestOutcome.Passed:
                    streamName = LogStreamNames.Default;
                    break;
                case TestOutcome.Inconclusive:
                    streamName = LogStreamNames.Warnings;
                    break;
                case TestOutcome.Failed:
                    streamName = LogStreamNames.Failures;
                    break;
            }

            testContext.LogWriter[streamName].WriteException(ex, failureHeading);

            CombineOutcome(ref outcome, testContext.Outcome);
            testContext.Outcome = outcome;
        }

        private static void CombineOutcome(ref TestOutcome result, TestOutcome other)
        {
            if (other == TestOutcome.Failed)
                result = TestOutcome.Failed;
            else if (result != TestOutcome.Failed && other == TestOutcome.Inconclusive)
                result = TestOutcome.Inconclusive;
        }
    }
}