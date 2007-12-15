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

using System;
using System.Windows.Forms;
using Gallio.Icarus.Adapter;
using Gallio.Icarus.Core.CustomEventArgs;
using Gallio.Icarus.Core.Interfaces;
using Gallio.Icarus.Interfaces;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Serialization;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Gallio.Icarus.Tests
{
    [TestFixture]
    public class ProjectAdapterTest : MockTest
    {
        private IProjectAdapterView mockView;
        private IProjectAdapterModel mockModel;
        private IProjectPresenter mockPresenter;
        private ProjectAdapter projectAdapter;

        private IEventRaiser addAssembliesEvent;
        private IEventRaiser removeAssembliesEvent;
        private IEventRaiser removeAssemblyEvent;
        private IEventRaiser getTestTreeEvent;
        private IEventRaiser runTestsEvent;
        private IEventRaiser stopTestsEvent;
        private IEventRaiser setFilterEvent;
        private IEventRaiser getLogStreamEvent;
        private IEventRaiser generateReportEvent;

        [SetUp]
        public void SetUp()
        {
            mockView = mocks.CreateMock<IProjectAdapterView>();
            mockModel = mocks.CreateMock<IProjectAdapterModel>();
            
            mockView.AddAssemblies += null;
            LastCall.IgnoreArguments();
            addAssembliesEvent = LastCall.GetEventRaiser();
            
            mockView.RemoveAssemblies += null;
            LastCall.IgnoreArguments();
            removeAssembliesEvent = LastCall.GetEventRaiser();

            mockView.RemoveAssembly += null;
            LastCall.IgnoreArguments();
            removeAssemblyEvent = LastCall.GetEventRaiser();

            mockView.GetTestTree += null;
            LastCall.IgnoreArguments();
            getTestTreeEvent = LastCall.GetEventRaiser();

            mockView.RunTests += null;
            LastCall.IgnoreArguments();
            runTestsEvent = LastCall.GetEventRaiser();

            mockView.StopTests += null;
            LastCall.IgnoreArguments();
            stopTestsEvent = LastCall.GetEventRaiser();

            mockView.SetFilter += null;
            LastCall.IgnoreArguments();
            setFilterEvent = LastCall.GetEventRaiser();

            mockView.GetLogStream += null;
            LastCall.IgnoreArguments();
            getLogStreamEvent = LastCall.GetEventRaiser();

            mockView.GenerateReport += null;
            LastCall.IgnoreArguments();
            generateReportEvent = LastCall.GetEventRaiser();
        }

        [Test]
        public void AddAssembliesEventHandler_Test()
        {
            mocks.ReplayAll();

            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackageConfig());
            addAssembliesEvent.Raise(mockView, new AddAssembliesEventArgs(new string[] { "test.dll" }));
            Assert.IsTrue(projectAdapter.TestPackageConfig.AssemblyFiles.Contains("test.dll"));
        }

        [Test]
        public void RemoveAssembliesEventHandler_Test()
        {
            mocks.ReplayAll();

            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackageConfig());
            projectAdapter.TestPackageConfig.AssemblyFiles.Add("test.dll");
            Assert.IsTrue(projectAdapter.TestPackageConfig.AssemblyFiles.Count == 1);
            removeAssembliesEvent.Raise(mockView, EventArgs.Empty);
            Assert.IsTrue(projectAdapter.TestPackageConfig.AssemblyFiles.Count == 0);
        }

        [Test]
        public void RemoveAssemblyEventHandler_Test()
        {
            mocks.ReplayAll();

            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackageConfig());
            projectAdapter.TestPackageConfig.AssemblyFiles.Add("test.dll");
            Assert.IsTrue(projectAdapter.TestPackageConfig.AssemblyFiles.Count == 1);
            removeAssemblyEvent.Raise(mockView, new SingleStringEventArgs("test2.dll"));
            Assert.IsTrue(projectAdapter.TestPackageConfig.AssemblyFiles.Count == 1);
            removeAssemblyEvent.Raise(mockView, new SingleStringEventArgs("test.dll"));
            Assert.IsTrue(projectAdapter.TestPackageConfig.AssemblyFiles.Count == 0);
        }

        [Test]
        public void GetTestTreeEventHandler_Test()
        {
            mockPresenter = MockRepository.GenerateStub<IProjectPresenter>();
            mockPresenter.GetTestTree(projectAdapter, new ProjectEventArgs(new TestPackageConfig()));
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackageConfig());
            projectAdapter.GetTestTree += new EventHandler<ProjectEventArgs>(mockPresenter.GetTestTree);
            getTestTreeEvent.Raise(mockView, new SingleStringEventArgs("mode"));
        }

        [Test]
        public void RunTestsEventHandler_Test()
        {
            mockPresenter = MockRepository.GenerateStub<IProjectPresenter>();
            mockPresenter.RunTests(projectAdapter, EventArgs.Empty);
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackageConfig());
            projectAdapter.RunTests += new EventHandler<EventArgs>(mockPresenter.RunTests);
            runTestsEvent.Raise(mockView, EventArgs.Empty);
        }

        [Test]
        public void StopTestsEventHandler_Test()
        {
            mockPresenter = mocks.CreateMock<IProjectPresenter>();
            mockPresenter.StopTests(projectAdapter, EventArgs.Empty);
            LastCall.IgnoreArguments();
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackageConfig());
            projectAdapter.StopTests += new EventHandler<EventArgs>(mockPresenter.StopTests);
            stopTestsEvent.Raise(mockView, EventArgs.Empty);
        }

        [Test]
        public void SetFilterEventHandler_Test()
        {
            mockPresenter = mocks.CreateMock<IProjectPresenter>();
            mockPresenter.SetFilter(projectAdapter, new SetFilterEventArgs(new NoneFilter<ITest>()));
            LastCall.IgnoreArguments();
            Expect.Call(mockModel.GetFilter(null)).Return(null);
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackageConfig());
            projectAdapter.SetFilter += new EventHandler<SetFilterEventArgs>(mockPresenter.SetFilter);
            setFilterEvent.Raise(mockView, new SetFilterEventArgs((TreeNodeCollection)null));
        }

        [Test]
        public void GetLogStreamEventHandler_Test()
        {
            mockPresenter = mocks.CreateMock<IProjectPresenter>();
            mockPresenter.GetLogStream(projectAdapter, new SingleStringEventArgs(""));
            LastCall.IgnoreArguments();
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackageConfig());
            projectAdapter.GetLogStream += new EventHandler<SingleStringEventArgs>(mockPresenter.GetLogStream);
            getLogStreamEvent.Raise(mockView, new SingleStringEventArgs(""));
        }

        [Test]
        public void GenerateReportEventHandler_Test()
        {
            mockPresenter = mocks.CreateMock<IProjectPresenter>();
            mockPresenter.GenerateReport(projectAdapter, new SingleStringEventArgs("generateReportTest"));
            LastCall.IgnoreArguments();
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackageConfig());
            projectAdapter.GenerateReport += new EventHandler<SingleStringEventArgs>(mockPresenter.GenerateReport);
            generateReportEvent.Raise(mockView, new SingleStringEventArgs("generateReportTest"));
        }

        [Test]
        public void StatusText_Test()
        {
            mockView.StatusText = "blah blah";
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackageConfig());
            projectAdapter.StatusText = "blah blah";
        }

        [Test]
        public void CompletedWorkUnits_Test()
        {
            mockView.CompletedWorkUnits = 2;
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackageConfig());
            projectAdapter.CompletedWorkUnits = 2;
        }

        [Test]
        public void TotalWorkUnits_Test()
        {
            mockView.TotalWorkUnits = 5;
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackageConfig());
            projectAdapter.TotalWorkUnits = 5;
        }

        [Test]
        public void LogBody_Test()
        {
            mockView.LogBody = "blah blah";
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackageConfig());
            projectAdapter.LogBody = "blah blah";
        }

        [Test]
        public void TestModel_Test()
        {
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackageConfig());
            Assert.IsNull(projectAdapter.TestModelData);
            TestModelData testModelData = new TestModelData(new TestData("id", "name"));
            projectAdapter.TestModelData = testModelData;
            Assert.AreEqual(testModelData, projectAdapter.TestModelData);
        }

        [Test]
        public void TestPackage_Test()
        {
            mocks.ReplayAll();
            TestPackageConfig testPackageConfig = new TestPackageConfig();
            projectAdapter = new ProjectAdapter(mockView, mockModel, testPackageConfig);
            Assert.AreEqual(testPackageConfig, projectAdapter.TestPackageConfig);
        }

        [Test]
        public void DataBind_Test()
        {
            TestPackageConfig testPackageConfig = new TestPackageConfig();
            mockView.Assemblies = null;
            LastCall.IgnoreArguments();
            Expect.Call(mockModel.BuildAssemblyList(testPackageConfig.AssemblyFiles)).Return(new ListViewItem[0]);
            mockView.TestTreeCollection = null;
            LastCall.IgnoreArguments();
            Expect.Call(mockModel.BuildTestTree(null, "")).IgnoreArguments().Return(new TreeNode[0]);
            mockView.TotalTests(null);
            LastCall.IgnoreArguments();
            Expect.Call(mockModel.CountTests(null)).IgnoreArguments().Return(0);
            mockView.DataBind();
            
            mocks.ReplayAll();

            projectAdapter = new ProjectAdapter(mockView, mockModel, testPackageConfig);
            projectAdapter.DataBind();
        }

        [Test]
        public void Passed_Test()
        {
            mockView.Passed("test1");
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackageConfig());
            projectAdapter.Passed("test1");
        }

        [Test]
        public void Failed_Test()
        {
            mockView.Failed("test1");
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackageConfig());
            projectAdapter.Failed("test1");
        }

        [Test]
        public void Skipped_Test()
        {
            mockView.Skipped("test1");
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackageConfig());
            projectAdapter.Skipped("test1");
        }

        [Test]
        public void Ignored_Test()
        {
            mockView.Ignored("test1");
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackageConfig());
            projectAdapter.Ignored("test1");
        }
    }
}