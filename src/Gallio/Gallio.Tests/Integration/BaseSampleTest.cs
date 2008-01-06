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
using System.Text;
using Gallio.Collections;
using Gallio.Reflection;
using Gallio.Runner.Reports;
using Gallio.Model.Serialization;

namespace Gallio.Tests.Integration
{
    /// <summary>
    /// Abstract base class for integration tests based on test samples.
    /// These tests work by launching a nested test runner to generate a report
    /// from the execution of zero or more tests.  The test cases then verify
    /// the contents of this report.
    /// </summary>
    /// <remarks>
    /// For efficiency, it is recommended to run a whole bunch of fixtures as
    /// part of the TestFixtureSetUp phase and then to verify the expected
    /// results as separate test cases.
    /// </remarks>
    public abstract class BaseSampleTest
    {
        private SampleRunner runner;

        public SampleRunner Runner
        {
            get { return runner; }
        }

        public Report Report
        {
            get { return runner.Report; }
        }

        protected void RunFixtures(params Type[] fixtureTypes)
        {
            runner = new SampleRunner();

            foreach (Type fixtureType in fixtureTypes)
                runner.AddFixture(fixtureType);

            runner.Run();
        }

        protected TestData GetTestInfo(CodeReference codeReference)
        {
            foreach (TestData info in Report.TestModelData.Tests.Values)
            {
                if (Equals(info.CodeReference, codeReference))
                    return info;
            }

            return null;
        }

        protected TestInstanceRun GetFirstTestInstanceRun(CodeReference codeReference)
        {
            TestData data = GetTestInfo(codeReference);
            if (data != null)
            {
                return GenericUtils.Find(Report.PackageRun.TestInstanceRuns, delegate(TestInstanceRun run)
                {
                    return run.TestInstance.TestId == data.Id;
                });
            }

            return null;
        }

        protected string GetStreamText(CodeReference codeReference, string streamName)
        {
            TestInstanceRun run = GetFirstTestInstanceRun(codeReference);
            if (run == null)
                return null;

            StringBuilder contents = new StringBuilder();
            foreach (TestStepRun step in run.TestStepRuns)
            {
                ExecutionLogStream stream = step.ExecutionLog.GetStream(streamName);
                if (stream != null)
                    AppendText(contents, stream.Body);
            }

            return contents.ToString();
        }

        private static void AppendText(StringBuilder contents, ExecutionLogStreamTag tag)
        {
            ExecutionLogStreamTextTag textTag = tag as ExecutionLogStreamTextTag;
            if (textTag != null)
            {
                contents.Append(textTag.Text).AppendLine();
                return;
            }

            ExecutionLogStreamContainerTag containerTag = tag as ExecutionLogStreamContainerTag;
            if (containerTag != null)
            {
                foreach (ExecutionLogStreamTag childTag in containerTag.Contents)
                    AppendText(contents, childTag);
            }
        }
    }
}
