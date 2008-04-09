// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Runtime;
using Gallio.Model.Execution;

namespace Gallio.Runner.Harness
{
    /// <summary>
    /// A contextual log trace listener messages to a named log stream in the log associated
    /// with the test execution context that is active at the time each message is written.
    /// </summary>
    public sealed class ContextualLogTraceListener : TraceListener
    {
        private readonly string streamName;

        /// <summary>
        /// Creates a trace listener that writes to the specified execution log stream.
        /// </summary>
        /// <param name="streamName">The execution log stream name</param>
        public ContextualLogTraceListener(string streamName)
        {
            this.streamName = streamName;
        }

        /// <inheritdoc />
        public override void Write(string message)
        {
            if (message == null)
                return;

            WriteIndentIfNeeded();

            try
            {
                ITestContext context = TestContextTrackerAccessor.GetInstance().CurrentContext;
                if (context == null)
                    return;

                context.LogWriter.Write(streamName, message);
            }
            catch (Exception ex)
            {
                UnhandledExceptionPolicy.Report(String.Format("Could not write to the {0} log stream.", streamName), ex);
            }
        }

        /// <inheritdoc />
        public override void WriteLine(string message)
        {
            Write(message + "\n");
        }

        /// <inheritdoc />
        public override bool IsThreadSafe
        {
            get { return true; }
        }

        private void WriteIndentIfNeeded()
        {
            if (NeedIndent)
                WriteIndent();
        }
    }
}