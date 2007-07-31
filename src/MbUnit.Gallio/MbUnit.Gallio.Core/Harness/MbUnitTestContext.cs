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
using System.Diagnostics;
using System.IO;
using MbUnit.Framework;
using MbUnit.Framework.Services.ExecutionLogs;

namespace MbUnit.Core.Harness
{
    /// <summary>
    /// Sets up and tears down the environment used for running MbUnit tests.
    /// </summary>
    public class MbUnitTestContext
    {
        private TextReader oldConsoleIn;
        private TextWriter oldConsoleOut;
        private TextWriter oldConsoleError;

        private ContextualExecutionLogTraceListener debugListener;
        private ContextualExecutionLogTraceListener traceListener;

        public void SetUp()
        {
            // Save the old console streams.
            oldConsoleIn = Console.In;
            oldConsoleOut = Console.Out;
            oldConsoleError = Console.Error;

            // Inject debug and trace listeners.
            debugListener = new ContextualExecutionLogTraceListener(ExecutionLogStreams.Debug);
            traceListener = new ContextualExecutionLogTraceListener(ExecutionLogStreams.Trace);
            Debug.Listeners.Add(debugListener);
            Debug.AutoFlush = true;

            Trace.Listeners.Add(traceListener);
            Trace.AutoFlush = true;

            // Inject console streams.

        }

        public void TearDown()
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