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
using System.Collections.Generic;
using System.IO;

using Gallio.Icarus.Core.Model;
using Gallio.Icarus.Core.Interfaces;
using Gallio.Icarus.Core.Presenter;
using Gallio.Icarus.Tests;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Runner.Events;
using MbUnit.Framework;

using Rhino.Mocks;
using Gallio.Runner;
using Gallio.Runner.Reports;
using Gallio.Model.Serialization;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Model.Execution;
using Gallio.Runtime;
using System.Collections.Specialized;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Interfaces;

namespace Gallio.Icarus.Core.Model.Tests
{
    [TestFixture]
    public class TestRunnerModelTest : MockTest
    {
        private TestRunnerModel testRunnerModel;
        private ITestRunner testRunner;
        private ITestRunnerEvents testRunnerEvents;
        private IReportManager reportManager;

        [SetUp]
        public void SetUp()
        {
            // set up test runner mock
            testRunner = mocks.CreateMock<ITestRunner>();
            testRunnerEvents = mocks.DynamicMock<ITestRunnerEvents>();
            SetupResult.For(testRunner.Events).Return(testRunnerEvents);

            reportManager = mocks.CreateMock<IReportManager>();
        }

        [Test, ExpectedArgumentNullException("projectPresenter")]
        public void SetProjectPresenterNull_Test()
        {
            mocks.ReplayAll();
            testRunnerModel = new TestRunnerModel(testRunner, reportManager);
            testRunnerModel.ProjectPresenter = null;
        }

        [Test]
        public void Explore_Test()
        {
            testRunner.Explore(null, null);
            LastCall.IgnoreArguments();
            TestModelData testModelData = new TestModelData(new TestData("test", "test", "test"));
            Report report = new Report();
            report.TestModel = testModelData;
            Expect.Call(testRunner.Report).Return(report);
            mocks.ReplayAll();
            testRunnerModel = new TestRunnerModel(testRunner, reportManager);
            TestModelData tmd = testRunnerModel.Explore();
            Assert.AreEqual(testModelData, tmd);
        }

        [Test]
        public void Load_Test()
        {
            TestPackageConfig testPackageConfig = new TestPackageConfig();

            Expect.Call(testRunner.Report).Return(null);
            testRunner.Unload(null);
            LastCall.IgnoreArguments();
            testRunner.Load(testPackageConfig, null);
            LastCall.IgnoreArguments();

            mocks.ReplayAll();

            testRunnerModel = new TestRunnerModel(testRunner, reportManager);
            testRunnerModel.Load(testPackageConfig);
        }

        [Test]
        public void GetExecutionLog_Test()
        {
            Expect.Call(testRunner.Report).Return(new Report());
            mocks.ReplayAll();
            testRunnerModel = new TestRunnerModel(testRunner, reportManager);
            Assert.IsNull(testRunnerModel.GetExecutionLog("test", new TestModelData(new TestData("test", "test", "test"))));
        }

        [Test]
        public void GetReportTypes_Test()
        {
            IList<string> reportTypes = new List<string>();
            reportTypes.Add("test");
            IRegisteredComponentResolver<IReportFormatter> formatterResolver = mocks.CreateMock<IRegisteredComponentResolver<IReportFormatter>>();
            Expect.Call(formatterResolver.GetNames()).Return(reportTypes);
            Expect.Call(reportManager.FormatterResolver).Return(formatterResolver);
            mocks.ReplayAll();
            testRunnerModel = new TestRunnerModel(testRunner, reportManager);
            Assert.AreEqual(reportTypes, testRunnerModel.GetReportTypes());
        }

        [Test]
        public void GetTestFrameworks_Test()
        {
            mocks.ReplayAll();
            testRunnerModel = new TestRunnerModel(testRunner, reportManager);
            IList<string> testFrameworks = testRunnerModel.GetTestFrameworks();
        }

        [Test]
        public void GenerateReport_Test()
        {
            IReportWriter reportWriter = mocks.CreateMock<IReportWriter>();
            IList<string> reportDocumentPaths = new List<string>();
            reportDocumentPaths.Add("test");
            Expect.Call(reportWriter.ReportDocumentPaths).Return(reportDocumentPaths);
            Expect.Call(reportManager.CreateReportWriter(null, null)).Return(reportWriter);
            LastCall.IgnoreArguments();
            reportManager.Format(reportWriter, "html", new NameValueCollection(), null);
            LastCall.IgnoreArguments();
            Report report = new Report();
            Expect.Call(testRunner.Report).Return(report).Repeat.Twice();
            mocks.ReplayAll();
            testRunnerModel = new TestRunnerModel(testRunner, reportManager);
            testRunnerModel.ReportFolder = @"c:\";
            Assert.AreEqual(@"c:\test", testRunnerModel.GenerateReport());
        }

        [Test]
        public void RunTests_Test()
        {
            IProjectPresenter projectPresenter = mocks.CreateMock<IProjectPresenter>();
            Expect.Call(projectPresenter.CompletedWorkUnits = 0).Repeat.AtLeastOnce();
            projectPresenter.StatusText = string.Empty;
            testRunner.Run(null, null);
            LastCall.IgnoreArguments();
            mocks.ReplayAll();
            testRunnerModel = new TestRunnerModel(testRunner, reportManager);
            testRunnerModel.ProjectPresenter = projectPresenter;
            testRunnerModel.Run();
        }

        [Test]
        public void SaveReportAs_Test()
        {
            IReportWriter reportWriter = mocks.CreateMock<IReportWriter>();
            Expect.Call(reportManager.CreateReportWriter(null, null)).Return(reportWriter);
            LastCall.IgnoreArguments();
            string format = "format";
            reportManager.Format(reportWriter, format, new NameValueCollection(), null);
            LastCall.IgnoreArguments();
            Report report = new Report();
            Expect.Call(testRunner.Report).Return(report);
            mocks.ReplayAll();
            testRunnerModel = new TestRunnerModel(testRunner, reportManager);
            string fileName = Path.GetTempFileName();
            testRunnerModel.SaveReportAs(fileName, format);
        }

        [Test]
        public void SetFilter_Test()
        {
            TestExecutionOptions testExecutionOptions = new TestExecutionOptions();
            Filter<ITest> filter = new NoneFilter<ITest>();
            testExecutionOptions.Filter = filter;

            Expect.Call(delegate { testRunner.Run(null, null); })
                .Constraints(Is.NotNull(), Is.NotNull())
                .Do((Action<TestExecutionOptions, IProgressMonitor>)
                delegate(TestExecutionOptions options, IProgressMonitor progressMonitor)
                {
                    Assert.AreSame(filter, options.Filter);
                });

            mocks.ReplayAll();

            testRunnerModel = new TestRunnerModel(testRunner, reportManager);
            testRunnerModel.SetFilter(filter);

            testRunnerModel.Run();
        }

        [Test]
        public void StopTests_Test()
        {
            mocks.ReplayAll();
            testRunnerModel = new TestRunnerModel(testRunner, reportManager);
            testRunnerModel.StopTests();
        }

        [Test]
        public void UnloadTestPackage_Test()
        {
            Expect.Call(testRunner.Report).Return(null);
            testRunner.Unload(null);
            LastCall.IgnoreArguments();

            mocks.ReplayAll();

            testRunnerModel = new TestRunnerModel(testRunner, reportManager);
            testRunnerModel.Unload();
        }
    }
}