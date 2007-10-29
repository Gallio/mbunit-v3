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
    public class IProjectAdapterTest : MockTest
    {
        private IProjectAdapterView mockView;
        private IProjectAdapterModel mockModel;
        private IProjectPresenter mockPresenter;
        private ProjectAdapter projectAdapter;

        [SetUp]
        public void SetUp()
        {
            mockView = MockRepository.GenerateStub<IProjectAdapterView>();
            mockModel = mocks.CreateMock<IProjectAdapterModel>();
            projectAdapter = new ProjectAdapter(mockView, mockModel, new TestPackage());
        }

        [Test]
        public void AddAssembliesEventHandler_Test()
        {
            mockView.AddAssemblies += null;
            LastCall.IgnoreArguments();
            IEventRaiser raiseViewEvent = LastCall.GetEventRaiser();
            mocks.ReplayAll();
            raiseViewEvent.Raise(mockView, new AddAssembliesEventArgs(new string[] { "test.dll" }));
            Assert.IsTrue(projectAdapter.TestPackage.AssemblyFiles.Contains("test.dll"));
        }

        [Test]
        public void RemoveAssembliesEventHandler_Test()
        {
            projectAdapter.TestPackage.AssemblyFiles.Add("test.dll");
            mockView.RemoveAssemblies += null;
            LastCall.IgnoreArguments();
            IEventRaiser raiseViewEvent = LastCall.GetEventRaiser();
            mocks.ReplayAll();
            raiseViewEvent.Raise(mockView, EventArgs.Empty);
            Assert.IsTrue(projectAdapter.TestPackage.AssemblyFiles.Count == 0);
        }

        [Test]
        public void GetTestTreeEventHandler_Test()
        {
            mockView.GetTestTree += null;
            LastCall.IgnoreArguments();
            IEventRaiser raiseViewEvent = LastCall.GetEventRaiser();
            mockPresenter = MockRepository.GenerateStub<IProjectPresenter>();
            projectAdapter.GetTestTree += new EventHandler<ProjectEventArgs>(mockPresenter.GetTestTree);
            mockPresenter.GetTestTree(projectAdapter, new ProjectEventArgs(new TestPackage()));
            mocks.ReplayAll();
            raiseViewEvent.Raise(mockView, new GetTestTreeEventArgs("mode"));
        }

        [Test]
        public void RunTestsEventHandler_Test()
        {
            mockView.RunTests += null;
            LastCall.IgnoreArguments();
            IEventRaiser raiseViewEvent = LastCall.GetEventRaiser();
            mockPresenter = MockRepository.GenerateStub<IProjectPresenter>();
            projectAdapter.RunTests += new EventHandler<EventArgs>(mockPresenter.RunTests);
            mockPresenter.RunTests(projectAdapter, EventArgs.Empty);
            mocks.ReplayAll();
            raiseViewEvent.Raise(mockView, EventArgs.Empty);
        }

        [Test]
        public void StatusText_Test()
        {
            mockView.StatusText = "blah blah";
            mocks.ReplayAll();
            projectAdapter.StatusText = "blah blah";
            Assert.AreEqual("blah blah", mockView.StatusText);
        }

        [Test]
        public void CompletedWorkUnits_Test()
        {
            mockView.CompletedWorkUnits = 2;
            mocks.ReplayAll();
            projectAdapter.CompletedWorkUnits = 2;
            Assert.AreEqual(2, mockView.CompletedWorkUnits);
        }

        [Test]
        public void TotalWorkUnits_Test()
        {
            mockView.TotalWorkUnits = 5;
            mocks.ReplayAll();
            projectAdapter.TotalWorkUnits = 5;
            Assert.AreEqual(5, mockView.TotalWorkUnits);
        }

        [Test]
        public void DataBind_Test()
        {
            Expect.Call(mockModel.BuildAssemblyList(projectAdapter.TestPackage.AssemblyFiles)).Return(new ListViewItem[0]);
            Expect.Call(mockModel.BuildTestTree(projectAdapter.TestModel, "")).Return(new TreeNode[0]);
            Expect.Call(mockModel.CountTests(projectAdapter.TestModel)).Return(0);
            mocks.ReplayAll();
            projectAdapter.DataBind();
        }

        [Test]
        public void Passed_Test()
        {
            mockView.Passed("test1");
            mocks.ReplayAll();
            projectAdapter.Passed("test1");
        }

        [Test]
        public void Failed_Test()
        {
            mockView.Failed("test1");
            mocks.ReplayAll();
            projectAdapter.Failed("test1");
        }

        [Test]
        public void Skipped_Test()
        {
            mockView.Skipped("test1");
            mocks.ReplayAll();
            projectAdapter.Skipped("test1");
        }

        [Test]
        public void Ignored_Test()
        {
            mockView.Ignored("test1");
            mocks.ReplayAll();
            projectAdapter.Ignored("test1");
        }
    }
}