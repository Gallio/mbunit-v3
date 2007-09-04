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
using System.Diagnostics;
using System.IO;
using System.Text;
using MbUnit.Framework.Kernel.Events;
using MbUnit.Framework.Kernel.ExecutionLogs;
using MbUnit.Framework.Kernel.Results;

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
        private TextReader oldConsoleIn;
        private TextWriter oldConsoleOut;
        private TextWriter oldConsoleError;

        private ContextualExecutionLogTraceListener debugListener;
        private ContextualExecutionLogTraceListener traceListener;

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public void Run(IProgressMonitor progressMonitor, TestExecutionOptions options, IEventListener listener,
            IList<ITest> tests)
        {
            using (progressMonitor)
            {
                progressMonitor.BeginTask("Running MbUnit tests.", tests.Count + 2);

                // HACK to get this running quickly...
                foreach (MbUnitTest test in tests)
                {
                    MbUnitMethodTemplate methodTemplate = test.TemplateBinding.Template as MbUnitMethodTemplate;
                    if (methodTemplate != null)
                    {
                        RunTest(test, listener);
                    }
                }
            }
        }

        /// <summary>
        /// Applies a topological sort to all tests so that parent tests appear
        /// before children and fully encapsulate
        /// </summary>
        /// <param name="tests"></param>
        /// <returns></returns>
        private IList<ITest> SortTests(IList<ITest> tests)
        {
            return tests;
        }

        private void RunTest(MbUnitTest test, IEventListener listener)
        {
            IStep step = new BaseStep(test);
            bool passed = false;

            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                MbUnitTestState state = new MbUnitTestState(test);

                listener.NotifyLifecycleEvent(LifecycleEventArgs.CreateStartEvent(new StepInfo(step)));
                try
                {
                    listener.NotifyLifecycleEvent(LifecycleEventArgs.CreatePhaseEvent(step.Id, LifecyclePhase.SetUp));

                    if (InitializeFixture(test, state, step, listener)
                        && RunSetup(test, state, step, listener))
                    {
                        listener.NotifyLifecycleEvent(LifecycleEventArgs.CreatePhaseEvent(step.Id, LifecyclePhase.Execute));

                        passed = RunExecute(test, state, step, listener);                        
                    }
                }
                finally
                {
                    listener.NotifyLifecycleEvent(LifecycleEventArgs.CreatePhaseEvent(step.Id, LifecyclePhase.TearDown));

                    passed = RunTearDown(test, state, step, listener) && passed;
                }
            }
            finally
            {
                TestResult result = new TestResult();
                result.Outcome = passed ? TestOutcome.Passed : TestOutcome.Failed;
                result.State = TestState.Executed;
                result.Duration = stopwatch.Elapsed.TotalSeconds;
                result.AssertCount = 0; // TODO.

                listener.NotifyLifecycleEvent(LifecycleEventArgs.CreateFinishEvent(step.Id, result));
            }
        }

        private bool InitializeFixture(MbUnitTest test, MbUnitTestState state, IStep step, IEventListener listener)
        {
            return ExecuteSafely(step, listener, delegate
            {
                // HACK!!!!
                MbUnitMethodTemplate methodTemplate = test.TemplateBinding.Template as MbUnitMethodTemplate;

                if (methodTemplate != null)
                    state.FixtureInstance = Activator.CreateInstance(methodTemplate.TypeTemplate.Type);
            }, "An exception occurred while initializing the test fixture.");
        }

        private bool RunSetup(MbUnitTest test, MbUnitTestState state, IStep step, IEventListener listener)
        {
            return ExecuteSafely(step, listener, delegate
            {
                test.SetUpChain.Action(state);
            }, "An exception occurred while setting up the test.");
        }

        private bool RunExecute(MbUnitTest test, MbUnitTestState state, IStep step, IEventListener listener)
        {
            return ExecuteSafely(step, listener, delegate
            {
                test.ExecuteChain.Action(state);
            }, "An exception occurred while executing the test.");
        }

        private bool RunTearDown(MbUnitTest test, MbUnitTestState state, IStep step, IEventListener listener)
        {
            return ExecuteSafely(step, listener, delegate
            {
                test.TearDownChain.Action(state);
            }, "An exception occurred while tearing down the test.");
        }

        private bool ExecuteSafely(IStep step, IEventListener listener, Block block, string failureHeading)
        {
            try
            {
                block();
                return true;
            }
            catch (Exception ex)
            {
                listener.NotifyExecutionLogEvent(ExecutionLogEventArgs.CreateWriteTextEvent(step.Id,
                    ExecutionLogStreamName.Failures, failureHeading + "\n" + ex.ToString()));
                return false;
            }
        }

        private void SetUpFramework()
        {
            // Save the old console streams.
            oldConsoleIn = Console.In;
            oldConsoleOut = Console.Out;
            oldConsoleError = Console.Error;

            // Inject debug and trace listeners.
            debugListener = new ContextualExecutionLogTraceListener(ExecutionLogStreamName.Debug);
            traceListener = new ContextualExecutionLogTraceListener(ExecutionLogStreamName.Trace);
            Debug.Listeners.Add(debugListener);
            Debug.AutoFlush = true;

            Trace.Listeners.Add(traceListener);
            Trace.AutoFlush = true;

            // Inject console streams.
            Console.SetIn(TextReader.Null);
            Console.SetOut(new ContextualExecutionLogTextWriter(ExecutionLogStreamName.ConsoleOutput));
            Console.SetError(new ContextualExecutionLogTextWriter(ExecutionLogStreamName.ConsoleError));
        }

        private void TearDownFramework()
        {
            // Remove debug and trace listeners.
            Debug.Listeners.Remove(debugListener);
            Trace.Listeners.Remove(traceListener);

            // Restore the old console streams.
            Console.SetIn(oldConsoleIn);
            Console.SetOut(oldConsoleOut);
            Console.SetError(oldConsoleError);
        }

        private class ExecutionTreeNode
        {
            private readonly List<ITest> tests;


        }
    }
}
