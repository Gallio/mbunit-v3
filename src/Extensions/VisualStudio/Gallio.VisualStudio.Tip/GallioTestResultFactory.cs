// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Gallio.Model;
using Gallio.Model.Logging;
using Gallio.Runner.Reports;
using Gallio.Utilities;
using Microsoft.VisualStudio.TestTools.Common;
using GallioTestOutcome = Gallio.Model.TestOutcome;
using VSTestOutcome=Microsoft.VisualStudio.TestTools.Common.TestOutcome;

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
            result.TestStepRunXml = TestStepRunToXml(run);
            return result;
        }

        /// <summary>
        /// Gets the test step run associated with a test result, or null if it could not be loaded.
        /// </summary>
        public static TestStepRun GetTestStepRun(GallioTestResult result)
        {
            return TestStepRunFromXml(result.TestStepRunXml);
        }

        private static VSTestOutcome GetOutcome(GallioTestOutcome outcome)
        {
            if (outcome == GallioTestOutcome.Canceled)
                return VSTestOutcome.Aborted;
            if (outcome == GallioTestOutcome.Error)
                return VSTestOutcome.Error;
            if (outcome == GallioTestOutcome.Timeout)
                return VSTestOutcome.Timeout;
            if (outcome == GallioTestOutcome.Ignored || outcome == GallioTestOutcome.Pending || outcome == GallioTestOutcome.Explicit)
                return VSTestOutcome.NotRunnable;

            switch (outcome.Status)
            {
                case TestStatus.Passed:
                    return VSTestOutcome.Passed;

                case TestStatus.Inconclusive:
                    return VSTestOutcome.Inconclusive;

                case TestStatus.Skipped:
                    return VSTestOutcome.NotExecuted;

                case TestStatus.Failed:
                default:
                    return VSTestOutcome.Failed;
            }
        }

        private static string TestStepRunToXml(TestStepRun run)
        {
            StringWriter stringWriter = new StringWriter();
            XmlSerializer serializer = new XmlSerializer(typeof(TestStepRun));
            serializer.Serialize(stringWriter, run);
            return stringWriter.ToString();
        }

        private static TestStepRun TestStepRunFromXml(string xml)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TestStepRun));
                return (TestStepRun)serializer.Deserialize(new StringReader(xml));
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
