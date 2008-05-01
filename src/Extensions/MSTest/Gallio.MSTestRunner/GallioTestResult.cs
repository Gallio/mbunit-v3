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
using System.Runtime.Serialization;
using Gallio.Loader;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Runner.Reports;
using Microsoft.VisualStudio.TestTools.Common;
using GallioTestOutcome = Gallio.Model.TestOutcome;
using TestOutcome=Microsoft.VisualStudio.TestTools.Common.TestOutcome;
using TestResult=Microsoft.VisualStudio.TestTools.Common.TestResult;

namespace Gallio.MSTestRunner
{
    // TODO: Save all step run details and provide a custom result viewer.
    [Serializable]
    internal sealed class GallioTestResult : TestResult
    {
        static GallioTestResult()
        {
            GallioAssemblyResolver.Install(typeof(GallioPackage).Assembly);
        }

        public GallioTestResult(GallioTestResult result)
            : base(result)
        {
        }

        public GallioTestResult(TestResult result)
            : base(result)
        {
        }

        public GallioTestResult(TestStepRun run, Guid runId, ITestElement test)
            : base(Environment.MachineName, runId, test)
        {
            Outcome = GetOutcome(run.Result.Outcome);

            foreach (ExecutionLogStream stream in run.ExecutionLog.Streams)
            {
                string contents = stream.ToString();

                if (stream.Name == LogStreamNames.DebugTrace)
                    DebugTrace += contents;
                else if (stream.Name == LogStreamNames.ConsoleOutput)
                    StdOut += contents;
                else if (stream.Name == LogStreamNames.ConsoleError)
                    StdErr += contents;
                else if (stream.Name == LogStreamNames.Failures || stream.Name == LogStreamNames.Warnings)
                    ErrorMessage += contents;
                else
                    DebugTrace += contents;
            }

            m_duration = TimeSpan.FromSeconds(run.Result.Duration);
            m_startTime = run.StartTime;
            m_endTime = run.EndTime;
            m_testName = run.Step.FullName;
        }

        private GallioTestResult(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override object Clone()
        {
            return new GallioTestResult(this);
        }

        public void MergeFrom(GallioTestResult source)
        {
        }

        private static TestOutcome GetOutcome(GallioTestOutcome outcome)
        {
            if (outcome == GallioTestOutcome.Canceled)
                return TestOutcome.Aborted;
            if (outcome == GallioTestOutcome.Error)
                return TestOutcome.Error;
            if (outcome == GallioTestOutcome.Timeout)
                return TestOutcome.Timeout;
            if (outcome == GallioTestOutcome.Ignored || outcome == GallioTestOutcome.Pending || outcome == GallioTestOutcome.Explicit)
                return TestOutcome.NotRunnable;

            switch (outcome.Status)
            {
                case TestStatus.Passed:
                    return TestOutcome.Passed;

                case TestStatus.Inconclusive:
                    return TestOutcome.Inconclusive;

                case TestStatus.Skipped:
                    return TestOutcome.NotExecuted;

                case TestStatus.Failed:
                default:
                    return TestOutcome.Failed;
            }
        }
    }
}
