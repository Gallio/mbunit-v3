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
using MbUnit.Framework.Kernel.Model;
using MbUnit.Model.Execution;
using MbUnit.Core.ProgressMonitoring;
using MbUnit.Logging;
using MbUnit.Model;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// Controls the execution of MbUnit tests.
    /// </summary>
    /// <todo author="jeff">
    /// VERY INCOMPLETE!
    /// </todo>
    public class MbUnitTestController : ITestController
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
                progressMonitor.BeginTask("Running MbUnit tests.", rootTestMonitor.TestCount);

                RunTest(progressMonitor, rootTestMonitor, null);
            }
        }

        private bool RunTest(IProgressMonitor progressMonitor, ITestMonitor testMonitor, MbUnitTestState parentState)
        {
            progressMonitor.SetStatus(String.Format("Run test: {0}.", testMonitor.Test.Name));

            IStepMonitor stepMonitor = testMonitor.StartRootStep();
            try
            {
                MbUnitTest test = (MbUnitTest)testMonitor.Test;
                MbUnitTestState state = new MbUnitTestState(test);
                bool passed;

                passed = InitializeTest(stepMonitor, state, parentState);

                if (passed)
                {
                    passed &= RunSetup(stepMonitor, state, parentState);

                    if (passed)
                    {
                        passed &= RunExecute(stepMonitor, state);

                        if (passed)
                        {
                            foreach (ITestMonitor child in testMonitor.Children)
                                passed &= RunTest(progressMonitor, child, state);
                        }
                    }

                    passed &= RunTearDown(stepMonitor, state, parentState);
                }

                stepMonitor.FinishStep(TestStatus.Executed, passed ? TestOutcome.Passed : TestOutcome.Failed, null);
                return passed;
            }
            catch (Exception ex)
            {
                stepMonitor.LogWriter[LogStreamNames.Failures].WriteException(ex, "A fatal test runner exception occurred.");

                stepMonitor.FinishStep(TestStatus.Error, TestOutcome.Failed, null);
                return false;
            }
            finally
            {
                progressMonitor.Worked(1);
            }
        }

        private bool InitializeTest(IStepMonitor stepMonitor, MbUnitTestState state, MbUnitTestState parentState)
        {
            return ExecuteSafely(stepMonitor, LifecyclePhases.Initialize, delegate
            {
                // FIXME: HACK!!!!
                // The real implementation will involve the test in the initialization
                // of its own state.  Thus fixtures can decide how to create instances
                // of themselves and how to pass them to child tests and so on.

                MbUnitTypeTemplate typeTemplate = state.Test.TemplateBinding.Template as MbUnitTypeTemplate;
                if (typeTemplate != null)
                    state.FixtureInstance = Activator.CreateInstance(typeTemplate.Type);
                else if (parentState != null)
                    state.FixtureInstance = parentState.FixtureInstance;
            }, "An exception occurred during initialization.");
        }

        private bool RunSetup(IStepMonitor stepMonitor, MbUnitTestState state, MbUnitTestState parentState)
        {
            return ExecuteSafely(stepMonitor, LifecyclePhases.SetUp, delegate
            {
                if (parentState != null)
                    parentState.Test.BeforeChildChain.Action(parentState);

                state.Test.SetUpChain.Action(state);
            }, "An exception occurred during set up.");
        }

        private bool RunExecute(IStepMonitor stepMonitor, MbUnitTestState state)
        {
            return ExecuteSafely(stepMonitor, LifecyclePhases.Execute, delegate
            {
                state.Test.ExecuteChain.Action(state);
            }, "An exception occurred during execution.");
        }

        private bool RunTearDown(IStepMonitor stepMonitor, MbUnitTestState state, MbUnitTestState parentState)
        {
            return ExecuteSafely(stepMonitor, LifecyclePhases.TearDown, delegate
            {
                state.Test.TearDownChain.Action(state);

                if (parentState != null)
                    parentState.Test.AfterChildChain.Action(parentState);
            }, "An exception occurred during tear down.");
        }

        private bool ExecuteSafely(IStepMonitor stepMonitor, string lifecyclePhase, Block block, string failureHeading)
        {
            stepMonitor.LifecyclePhase = lifecyclePhase;

            try
            {
                block();
                return true;
            }
            catch (Exception ex)
            {
                stepMonitor.LogWriter[LogStreamNames.Failures].WriteException(ex, failureHeading);
                stepMonitor.SetInterimOutcome(TestOutcome.Failed);
                return false;
            }
        }
    }
}