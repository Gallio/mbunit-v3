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
using MbUnit.Framework;
using Gallio.Runner;
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
        }

        [Test]
        public void AddAssembliesEventHandler_Test()
        {
            mocks.ReplayAll();

            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackage());
            addAssembliesEvent.Raise(mockView, new AddAssembliesEventArgs(new string[] { "test.dll" }));
            Assert.IsTrue(projectAdapter.TestPackage.AssemblyFiles.Contains("test.dll"));
            projectAdapter = null;
        }

        [Test]
        public void RemoveAssembliesEventHandler_Test()
        {
            mocks.ReplayAll();

            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackage());
            projectAdapter.TestPackage.AssemblyFiles.Add("test.dll");
            Assert.IsTrue(projectAdapter.TestPackage.AssemblyFiles.Count == 1);
            removeAssembliesEvent.Raise(mockView, EventArgs.Empty);
            Assert.IsTrue(projectAdapter.TestPackage.AssemblyFiles.Count == 0);
            projectAdapter = null;
        }

        [Test]
        public void RemoveAssemblyEventHandler_Test()
        {
            mocks.ReplayAll();

            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackage());
            projectAdapter.TestPackage.AssemblyFiles.Add("test.dll");
            Assert.IsTrue(projectAdapter.TestPackage.AssemblyFiles.Count == 1);
            removeAssemblyEvent.Raise(mockView, new SingleStringEventArgs("test2.dll"));
            Assert.IsTrue(projectAdapter.TestPackage.AssemblyFiles.Count == 1);
            removeAssemblyEvent.Raise(mockView, new SingleStringEventArgs("test.dll"));
            Assert.IsTrue(projectAdapter.TestPackage.AssemblyFiles.Count == 0);
            projectAdapter = null;
        }

        [Test]
        public void GetTestTreeEventHandler_Test()
        {
            mockPresenter = MockRepository.GenerateStub<IProjectPresenter>();
            mockPresenter.GetTestTree(projectAdapter, new ProjectEventArgs(new TestPackage()));
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackage());
            projectAdapter.GetTestTree += new EventHandler<ProjectEventArgs>(mockPresenter.GetTestTree);
            getTestTreeEvent.Raise(mockView, new SingleStringEventArgs("mode"));
            projectAdapter = null;
        }

        [Test]
        public void RunTestsEventHandler_Test()
        {
            mockPresenter = MockRepository.GenerateStub<IProjectPresenter>();
            mockPresenter.RunTests(projectAdapter, EventArgs.Empty);
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackage());
            projectAdapter.RunTests += new EventHandler<EventArgs>(mockPresenter.RunTests);
            runTestsEvent.Raise(mockView, EventArgs.Empty);
            projectAdapter = null;
        }

        [Test]
        public void StatusText_Test()
        {
            mockView.StatusText = "blah blah";
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackage());
            projectAdapter.StatusText = "blah blah";
            projectAdapter = null;
        }

        [Test]
        public void CompletedWorkUnits_Test()
        {
            mockView.CompletedWorkUnits = 2;
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackage());
            projectAdapter.CompletedWorkUnits = 2;
            projectAdapter = null;
        }

        [Test]
        public void TotalWorkUnits_Test()
        {
            mockView.TotalWorkUnits = 5;
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackage());
            projectAdapter.TotalWorkUnits = 5;
            projectAdapter = null;
        }

        [Test]
        public void DataBind_Test()
        {
            TestPackage testPackage = new TestPackage();
            mockView.Assemblies = null;
            LastCall.IgnoreArguments();
            Expect.Call(mockModel.BuildAssemblyList(testPackage.AssemblyFiles)).Return(new ListViewItem[0]);
            mockView.TestTreeCollection = null;
            LastCall.IgnoreArguments();
            Expect.Call(mockModel.BuildTestTree(null, "")).IgnoreArguments().Return(new TreeNode[0]);
            mockView.TotalTests(null);
            LastCall.IgnoreArguments();
            Expect.Call(mockModel.CountTests(null)).IgnoreArguments().Return(0);
            mockView.DataBind();
            
            mocks.ReplayAll();

            projectAdapter = new ProjectAdapter(mockView, mockModel, testPackage);
            projectAdapter.DataBind();
            projectAdapter = null;
        }

        [Test]
        public void Passed_Test()
        {
            mockView.Passed("test1");
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackage());
            projectAdapter.Passed("test1");
            projectAdapter = null;
        }

        [Test]
        public void Failed_Test()
        {
            mockView.Failed("test1");
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackage());
            projectAdapter.Failed("test1");
            projectAdapter = null;
        }

        [Test]
        public void Skipped_Test()
        {
            mockView.Skipped("test1");
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackage());
            projectAdapter.Skipped("test1");
            projectAdapter = null;
        }

        [Test]
        public void Ignored_Test()
        {
            mockView.Ignored("test1");
            mocks.ReplayAll();
            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackage());
            projectAdapter.Ignored("test1");
            projectAdapter = null;
        }
    }
}