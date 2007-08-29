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
            // HACK to get this running quickly...
            foreach (MbUnitTest test in tests)
            {
                MbUnitMethodTemplate methodTemplate = test.TemplateBinding.Template as MbUnitMethodTemplate;
                if (methodTemplate != null)
                {
                    MbUnitTestState state = new MbUnitTestState(test);
                    state.FixtureInstance = Activator.CreateInstance(methodTemplate.FixtureTemplate.FixtureType);

                    Execute(test, state, listener);
                }
            }
        }

        private void Execute(MbUnitTest test, MbUnitTestState state, IEventListener listener)
        {
            IStep step = BaseStep.CreateRootStep(test);
            try
            {
                listener.NotifyLifecycleEvent(LifecycleEventArgs.CreateStartEvent(new StepInfo(step)));
                try
                {
                    listener.NotifyLifecycleEvent(LifecycleEventArgs.CreatePhaseEvent(step.Id, LifecyclePhase.SetUp));
                    test.SetUpChain.Action(state);

                    listener.NotifyLifecycleEvent(LifecycleEventArgs.CreatePhaseEvent(step.Id, LifecyclePhase.Execute));
                    test.ExecuteChain.Action(state);
                }
                finally
                {
                    listener.NotifyLifecycleEvent(LifecycleEventArgs.CreatePhaseEvent(step.Id, LifecyclePhase.TearDown));
                    test.TearDownChain.Action(state);
                }
            }
            finally
            {
                TestResult result = new TestResult();
                listener.NotifyLifecycleEvent(LifecycleEventArgs.CreateFinishEvent(step.Id, result));
            }
        }

        private void SetUp()
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

        private void TearDown()
        {
            // Remove debug and trace listeners.
            Debug.Listeners.Remove(debugListener);
            Trace.Listeners.Remove(traceListener);

            // Restore the old console streams.
            Console.SetIn(oldConsoleIn);
            Console.SetOut(oldConsoleOut);
            Console.SetError(oldConsoleError);
        }
    }
}
