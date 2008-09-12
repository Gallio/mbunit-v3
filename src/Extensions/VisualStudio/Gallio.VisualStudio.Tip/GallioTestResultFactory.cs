// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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
using Gallio.Model;
using Gallio.Model.Logging;
using Gallio.Runner.Reports;
using Microsoft.VisualStudio.TestTools.Common;
using GallioTestOutcome = Gallio.Model.TestOutcome;
using TestOutcome=Microsoft.VisualStudio.TestTools.Common.TestOutcome;

namespace Gallio.VisualStudio.Tip
{
    internal static class GallioTestResultFactory
    {
        public static GallioTestResult CreateTestResult(TestStepRun run, Guid runId, ITestElement test)
        {
            GallioTestResult result = new GallioTestResult(Environment.MachineName, runId, test);
            result.TestName = run.Step.FullName;
            result.Outcome = GetOutcome(run.Result.Outcome);

            foreach (StructuredTestLogStream stream in run.TestLog.Streams)
            {
                string contents = stream.ToString();

                if (stream.Name == TestLogStreamNames.DebugTrace)
                    result.DebugTrace += contents;
                else if (stream.Name == TestLogStreamNames.ConsoleOutput)
                    result.StdOut += contents;
                else if (stream.Name == TestLogStreamNames.ConsoleError)
                    result.StdErr += contents;
                else if (stream.Name == TestLogStreamNames.Failures || stream.Name == TestLogStreamNames.Warnings)
                    result.ErrorMessage += contents;
                else
                    result.DebugTrace += contents;
            }

            result.SetTimings(run.StartTime, run.EndTime, TimeSpan.FromSeconds(run.Result.Duration));
            return result;
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
