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
using System.Collections.Generic;
using System.IO;
using Castle.Core.Logging;
using Gallio.Contexts;
using Gallio.Model;
using Gallio.Runner.Reports;
using Gallio.Runner;
using Gallio.Logging;
using Gallio.Model.Filters;
using MbUnit.Framework;

namespace Gallio.Tests.Integration
{
    /// <summary>
    /// Runs a list of sample test fixtures and provides the results back
    /// for verification.  The idea is to make it easier to write tests that
    /// verify the end-to-end behavior of the test runner.
    /// </summary>
    /// <todo author="jeff">
    /// Use some kind of in-memory filesystem to avoid writing reports to disk.
    /// </todo>
    public class SampleRunner
    {
        private readonly TestPackageConfig packageConfig;
        private readonly List<Filter<ITest>> filters;

        private TestLauncherResult result;

        public SampleRunner()
        {
            packageConfig = new TestPackageConfig();
            filters = new List<Filter<ITest>>();
        }

        public Report Report
        {
            get { return result.Report; }
        }

        public TestLauncherResult Result
        {
            get { return result; }
        }


        public void AddFixture(Type fixtureType)
        {
            filters.Add(new TypeFilter<ITest>(new EqualityFilter<string>(fixtureType.AssemblyQualifiedName), false));

            string assemblyFile = fixtureType.Assembly.Location;
            if (!packageConfig.AssemblyFiles.Contains(assemblyFile))
                packageConfig.AssemblyFiles.Add(assemblyFile);
        }

        public void Run()
        {
            LogStreamWriter logStreamWriter = Log.ConsoleOutput;

            using (TestLauncher launcher = new TestLauncher())
            {
                launcher.TestPackageConfig = packageConfig;
                launcher.Logger = new DebugLogger(logStreamWriter);
                launcher.Filter = new OrFilter<ITest>(filters.ToArray());
                launcher.TestRunnerFactoryName = StandardTestRunnerFactoryNames.LocalAppDomain;

                string reportDirectory = Path.GetTempPath();
                launcher.ReportDirectory = reportDirectory;
                launcher.ReportNameFormat = "SampleRunnerReport";
                launcher.ReportFormatOptions.Add(@"SaveAttachmentContents", @"false");
                launcher.ReportFormats.Add(@"Text");

                using (logStreamWriter.BeginSection("Debug Output"))
                {
                    Context.RunWithContext(null, delegate
                    {
                        result = launcher.Run();
                    });
                }

                using (logStreamWriter.BeginSection("Text Report"))
                {
                    foreach (string reportPath in result.ReportDocumentPaths)
                    {
                        logStreamWriter.WriteLine(File.ReadAllText(reportPath));
                    }
                }
            }
        }

        /// <summary>
        /// Castle's StreamLogger doesn't accept arbitrary TextWriters!
        /// </summary>
        private sealed class DebugLogger : LevelFilteredLogger
        {
            private readonly TextWriter writer;

            public DebugLogger(TextWriter writer)
            {
                this.writer = writer;
                Level = LoggerLevel.Debug;
            }

            public override ILogger CreateChildLogger(string name)
            {
                return this;
            }

            protected override void Log(LoggerLevel level, string name, string message, Exception exception)
            {
                writer.WriteLine(message);

                if (exception != null)
                {
                    writer.Write(">>> ");
                    writer.WriteLine(exception);
                }
            }
        }
    }
}
