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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gallio.Common.Diagnostics;
using Gallio.Common.Messaging;
using Gallio.DLRIntegration.Model;
using Gallio.Model;
using Gallio.Model.Isolation;
using Gallio.Runtime;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using MbUnit.Framework;
using Microsoft.Scripting.Hosting;
using Rhino.Mocks;

namespace Gallio.DLRIntegration.Tests.Model
{
    [TestsOn(typeof(DLRTestDriver))]
    public class IronRubyIntegrationTest
    {
        private ILogger logger;
        private IronRubyTestDriver driver;
        private ITestIsolationContext testIsolationContext;
        private TestPackage testPackage;
        private TestExplorationOptions testExplorationOptions;
        private TestExecutionOptions testExecutionOptions;
        private IMessageSink messageSink;
        private IProgressMonitor progressMonitor;

        [SetUp]
        public void SetUp()
        {
            logger = MockRepository.GenerateStub<ILogger>();
            driver = new IronRubyTestDriver(logger);
            var testIsolationProvider = (ITestIsolationProvider)RuntimeAccessor.ServiceLocator.ResolveByComponentId("Gallio.LocalTestIsolationProvider");
            testIsolationContext = testIsolationProvider.CreateContext(new TestIsolationOptions(), logger);
            testPackage = new TestPackage();
            testExplorationOptions = new TestExplorationOptions();
            testExecutionOptions = new TestExecutionOptions();
            messageSink = MockRepository.GenerateStub<IMessageSink>();
            progressMonitor = NullProgressMonitor.CreateInstance();
        }

        [Test]
        public void Explore_DriverScriptCanWriteToStdOutAndStdErr()
        {
            driver.Explore(testIsolationContext, testPackage, testExplorationOptions, messageSink, progressMonitor);

            logger.AssertWasCalled(x => x.Log(LogSeverity.Info, "StdOut message", (ExceptionData)null));
            logger.AssertWasCalled(x => x.Log(LogSeverity.Error, "StdErr message", (ExceptionData)null));
        }

        [Test]
        public void Explore_DriverScriptCanWriteToLogger()
        {
            driver.Explore(testIsolationContext, testPackage, testExplorationOptions, messageSink, progressMonitor);

            logger.AssertWasCalled(x => x.Log(LogSeverity.Error, "Log error", (ExceptionData)null));
            logger.AssertWasCalled(x => x.Log(LogSeverity.Warning, "Log warning", (ExceptionData)null));
            logger.AssertWasCalled(x => x.Log(LogSeverity.Important, "Log important", (ExceptionData)null));
            logger.AssertWasCalled(x => x.Log(LogSeverity.Info, "Log info", (ExceptionData)null));
            logger.AssertWasCalled(x => x.Log(LogSeverity.Debug, "Log debug", (ExceptionData)null));
        }

        [Test]
        public void Explore_DriverScriptReceivesExploreVerb()
        {
            driver.Explore(testIsolationContext, testPackage, testExplorationOptions, messageSink, progressMonitor);

            logger.AssertWasCalled(x => x.Log(LogSeverity.Info, "Verb Explore", (ExceptionData)null));
        }

        [Test]
        public void Explore_DriverScriptReceivesTestPackage()
        {
            testPackage.AddFile(new FileInfo("foo.rb"));
            testPackage.AddFile(new FileInfo("bar.rb"));
            driver.Explore(testIsolationContext, testPackage, testExplorationOptions, messageSink, progressMonitor);

            logger.AssertWasCalled(x => x.Log(LogSeverity.Info, "File foo.rb", (ExceptionData)null));
            logger.AssertWasCalled(x => x.Log(LogSeverity.Info, "File bar.rb", (ExceptionData)null));
        }

        [Test]
        public void Run_DriverScriptReceivesRunVerb()
        {
            driver.Run(testIsolationContext, testPackage, testExplorationOptions, testExecutionOptions, messageSink, progressMonitor);

            logger.AssertWasCalled(x => x.Log(LogSeverity.Info, "Verb Run", (ExceptionData)null));
        }

        private class IronRubyTestDriver : DLRTestDriver
        {
            public IronRubyTestDriver(ILogger logger)
                : base(logger)
            {
            }

            protected override FileInfo GetTestDriverScriptFile(TestPackage testPackage)
            {
                return new FileInfo(@"..\Scripts\IronRuby\sample_driver.rb");
            }

            protected override void ConfigureIronRuby(LanguageSetup languageSetup, IList<string> libraryPaths)
            {
                libraryPaths.Add(Path.GetFullPath(@"..\Scripts\IronRuby"));
            }
        }
    }
}
