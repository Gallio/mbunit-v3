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

using System.Diagnostics;
using MbUnit.Framework.Services.Reports;

namespace MbUnit.Framework.Services.Reports
{
    /// <summary>
    /// A contextual report stream trace listener writes trace messages to a stream
    /// in the report associated with the current test execution context at the time
    /// the message was written.
    /// </summary>
    public class ContextualReportTraceListener : TraceListener
    {
        private string reportStreamName;

        /// <summary>
        /// Creates a trace listener that writes to the specified report stream.
        /// </summary>
        /// <param name="reportStreamName">The report stream name</param>
        public ContextualReportTraceListener(string reportStreamName)
        {
            this.reportStreamName = reportStreamName;
        }

        public override void Write(string message)
        {
            WriteIndentIfNeeded();

            CurrentStream.Write(message);
        }

        public override void WriteLine(string message)
        {
            WriteIndentIfNeeded();

            CurrentStream.WriteLine(message);
        }

        private IReportStream CurrentStream
        {
            get { return Report.Streams[reportStreamName]; }
        }

        private void WriteIndentIfNeeded()
        {
            if (NeedIndent)
                WriteIndent();
        }
    }
}