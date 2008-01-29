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
        public void RunTests(IProgressMonitor progressMonitor, ITestMonitor rootTestMonitor)
        {
            using (progressMonitor)
            {
                progressMonitor.BeginTask("Running tests.", rootTestMonitor.TestCount);

                RunTest(progressMonitor, rootTestMonitor, null);
            }
        }

        private TestOutcome RunTest(IProgressMonitor progressMonitor, ITestMonitor testMonitor, PatternTestState parentState)
        {
            progressMonitor.SetStatus(String.Format("Run test: {0}.", testMonitor.Test.Name));

            ITestStepMonitor stepMonitor = testMonitor.StartTestInstance();
            try
            {
                PatternTest test = (PatternTest)testMonitor.Test;

                string ignoreReason = test.Metadata.GetValue(MetadataKeys.IgnoreReason);
                if (ignoreReason != null)
                {
                    stepMonitor.LogWriter[LogStreamNames.Warnings].WriteLine("Ignored: {0}", ignoreReason);
                    stepMonitor.FinishStep(TestStatus.Ignored, TestOutcome.Inconclusive, null);
                    return TestOutcome.Inconclusive;
                }
                else
                {
                    PatternTestState state = new PatternTestState(test);

                    TestOutcome outcome = InitializeTest(stepMonitor, state, parentState);

                    if (outcome == TestOutcome.Passed)
                    {
                        CombineOutcome(ref outcome, RunSetup(stepMonitor, state, parentState));

                        if (outcome == TestOutcome.Passed)
                        {
                            CombineOutcome(ref outcome, RunExecute(stepMonitor, state));

                            if (outcome == TestOutcome.Passed)
                            {
                                foreach (ITestMonitor child in testMonitor.Children)
                                {
                                    if (RunTest(progressMonitor, child, state) == TestOutcome.Failed)
                                        outcome = TestOutcome.Failed;
                                }
                            }
                        }

                        CombineOutcome(ref outcome, RunTearDown(stepMonitor, state, parentState));
                    }

                    stepMonitor.FinishStep(TestStatus.Executed, outcome, null);
                    return outcome;
                }
            }
            catch (Exception ex)
            {
                stepMonitor.LogWriter[LogStreamNames.Failures].WriteException(ex, "A fatal test runner exception occurred.");

                stepMonitor.FinishStep(TestStatus.Error, TestOutcome.Failed, null);
                return TestOutcome.Failed;
            }
            finally
            {
                progressMonitor.Worked(1);
            }
        }

        private TestOutcome InitializeTest(ITestStepMonitor stepMonitor, PatternTestState state, PatternTestState parentState)
        {
            return ExecuteSafely(stepMonitor, LifecyclePhases.Initialize, delegate
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

        private TestOutcome RunSetup(ITestStepMonitor stepMonitor, PatternTestState state, PatternTestState parentState)
        {
            return ExecuteSafely(stepMonitor, LifecyclePhases.SetUp, delegate
            {
                if (parentState != null)
                    parentState.Test.BeforeChildChain.Action(parentState);

                state.Test.SetUpChain.Action(state);
            }, "An exception occurred during set up.");
        }

        private TestOutcome RunExecute(ITestStepMonitor stepMonitor, PatternTestState state)
        {
            return ExecuteSafely(stepMonitor, LifecyclePhases.Execute, delegate
            {
                state.Test.ExecuteChain.Action(state);
            }, "An exception occurred during execution.");
        }

        private TestOutcome RunTearDown(ITestStepMonitor stepMonitor, PatternTestState state, PatternTestState parentState)
        {
            return ExecuteSafely(stepMonitor, LifecyclePhases.TearDown, delegate
            {
                state.Test.TearDownChain.Action(state);

                if (parentState != null)
                    parentState.Test.AfterChildChain.Action(parentState);
            }, "An exception occurred during tear down.");
        }

        private TestOutcome ExecuteSafely(ITestStepMonitor stepMonitor, string lifecyclePhase, Block block, string failureHeading)
        {
            stepMonitor.LifecyclePhase = lifecyclePhase;

            try
            {
                block();
                return TestOutcome.Passed;
            }
            catch (Exception ex)
            {
                TestOutcome outcome = TestOutcome.Failed;

                if (ex is ClientException)
                    ex = ex.InnerException;
                if (ex is TestException)
                    outcome = ((TestException)ex).Outcome;

                LogException(stepMonitor, ex, outcome, failureHeading);
                return outcome;
            }
        }

        private static void LogException(ITestStepMonitor stepMonitor, Exception ex, TestOutcome outcome, string failureHeading)
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

            stepMonitor.LogWriter[streamName].WriteException(ex, failureHeading);

            CombineOutcome(ref outcome, stepMonitor.Outcome);
            stepMonitor.SetInterimOutcome(outcome);
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