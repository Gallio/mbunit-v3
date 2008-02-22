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
            TestOutcome outcome;
            try
            {
                PatternTest test = (PatternTest)testCommand.Test;

                string ignoreReason = test.Metadata.GetValue(MetadataKeys.IgnoreReason);
                string pendingReason = test.Metadata.GetValue(MetadataKeys.PendingReason);

                if (ignoreReason != null)
                {
                    outcome = TestOutcome.Ignored;
                    testContext.LogWriter[LogStreamNames.Warnings].WriteLine("The test was ignored.  Reason: {0}",
                        ignoreReason.Length != 0 ? ignoreReason : "<unspecified>");
                }
                else if (pendingReason != null)
                {
                    outcome = TestOutcome.Pending;
                    testContext.LogWriter[LogStreamNames.Warnings].WriteLine("The test has pending prerequisites.  Reason: {0}",
                        pendingReason.Length != 0 ? pendingReason : "<unspecified>");
                }
                else
                {
                    PatternTestState state = new PatternTestState(test);

                    outcome = InitializeTest(testContext, state, parentState);

                    if (outcome.Status == TestStatus.Passed)
                    {
                        bool childTestFailed = false;
                        outcome = outcome.CombineWith(RunSetup(testContext, state, parentState));

                        if (outcome.Status == TestStatus.Passed)
                        {
                            outcome = outcome.CombineWith(RunExecute(testContext, state));

                            if (outcome.Status == TestStatus.Passed)
                            {
                                foreach (ITestCommand child in testCommand.Children)
                                {
                                    if (RunTest(progressMonitor, child, testContext.TestStep.TestInstance, state).Status == TestStatus.Failed)
                                        childTestFailed = true;
                                }
                            }
                        }

                        outcome = outcome.CombineWith(RunTearDown(testContext, state, parentState));

                        // Note: Child test failures take the least precedence in determining the overall outcome.
                        if (childTestFailed)
                            outcome.CombineWith(TestOutcome.Failed);
                    }
                }
            }
            catch (Exception ex)
            {
                testContext.LogWriter[LogStreamNames.Failures].WriteException(ex, "A fatal test runner exception occurred.");
                outcome = TestOutcome.Error;
            }

            testContext.FinishStep(outcome, null);
            progressMonitor.Worked(1);
            return outcome;
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
            });
        }

        private TestOutcome RunSetup(ITestContext testContext, PatternTestState state, PatternTestState parentState)
        {
            return ExecuteSafely(testContext, LifecyclePhases.SetUp, delegate
            {
                if (parentState != null)
                    parentState.Test.BeforeChildChain.Action(parentState);

                state.Test.SetUpChain.Action(state);
            });
        }

        private TestOutcome RunExecute(ITestContext testContext, PatternTestState state)
        {
            return ExecuteSafely(testContext, LifecyclePhases.Execute, delegate
            {
                state.Test.ExecuteChain.Action(state);
            });
        }

        private TestOutcome RunTearDown(ITestContext testContext, PatternTestState state, PatternTestState parentState)
        {
            return ExecuteSafely(testContext, LifecyclePhases.TearDown, delegate
            {
                state.Test.TearDownChain.Action(state);

                if (parentState != null)
                    parentState.Test.AfterChildChain.Action(parentState);
            });
        }

        private TestOutcome ExecuteSafely(ITestContext testContext, string lifecyclePhase, Action action)
        {
            testContext.LifecyclePhase = lifecyclePhase;

            TestOutcome outcome = TestInvoker.Run(action, lifecyclePhase);
            if (testContext.Outcome.Status == TestStatus.Passed)
                testContext.Outcome = outcome;

            return outcome;
        }
    }
}