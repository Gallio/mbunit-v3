// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Framework.Utilities;
using Gallio.Common.Markup;
using Gallio.Runner.Reports.Schema;
using MbUnit.Framework;

namespace Gallio.Tests
{
    /// <summary>
    /// Abstract base class for integration tests based on test samples.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This fixture runs the samples specified by <see cref="RunSampleAttribute" />
    /// and <see cref="RunSampleFileAttribute" /> then presents the results from the runner
    /// via the <see cref="Runner" /> and <see cref="Report"/> properties.  Test cases should
    /// then verify the results from the samples.
    /// </para>
    /// </remarks>
    public abstract class BaseTestWithSampleRunner
    {
        private readonly SampleRunner runner = new SampleRunner();
        private static bool isSampleRunning;

        /// <summary>
        /// Gets the sample runner for the fixture.
        /// </summary>
        public SampleRunner Runner
        {
            get { return runner; }
        }

        /// <summary>
        /// Gets the report for the tests that ran.
        /// </summary>
        public Report Report
        {
            get { return Runner.Report; }
        }

        [FixtureSetUp]
        public void RunDeclaredSamples()
        {
            // Protect the sample runner from re-entrance when we are asked to run
            // samples that are defined in nested classes of the fixture.
            if (isSampleRunning)
                return;

            try
            {
                isSampleRunning = true;
                ConfigureRunner();

                if (runner.TestPackage.Files.Count != 0)
                    runner.Run();
            }
            finally
            {
                isSampleRunning = false;
            }
        }

        /// <summary>
        /// Configures the test runner.
        /// </summary>
        protected virtual void ConfigureRunner()
        {
            foreach (RunSampleAttribute attrib in GetType().GetCustomAttributes(typeof(RunSampleAttribute), true))
            {
                if (attrib.MethodName == null)
                    runner.AddFixture(attrib.FixtureType);
                else
                    runner.AddMethod(attrib.FixtureType, attrib.MethodName);
            }

            foreach (RunSampleFileAttribute attrib in GetType().GetCustomAttributes(typeof(RunSampleFileAttribute), true))
            {
                runner.AddFile(new FileInfo(attrib.FilePath));
            }
        }

        protected static void AssertLogContains(TestStepRun run, string expectedOutput)
        {
            AssertLogContains(run, expectedOutput, MarkupStreamNames.Default);
        }

        protected static void AssertLogContains(TestStepRun run, string expectedOutput, string streamName)
        {
            Assert.Contains(run.TestLog.GetStream(streamName).ToString(), expectedOutput);
        }

        protected static void AssertLogDoesNotContain(TestStepRun run, string expectedOutput)
        {
            AssertLogDoesNotContain(run, expectedOutput, MarkupStreamNames.Default);
        }

        protected static void AssertLogDoesNotContain(TestStepRun run, string expectedOutput, string streamName)
        {
            Assert.DoesNotContain(run.TestLog.GetStream(streamName).ToString(), expectedOutput);
        }
    }
}