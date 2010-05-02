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

using System.Collections.Generic;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Events;
using Gallio.Icarus.Models;
using Gallio.Icarus.TestExplorer;
using Gallio.Model;
using Gallio.UI.Events;
using Gallio.UI.ProgressMonitoring;
using MbUnit.Framework;
using Rhino.Mocks;
using SortOrder=Gallio.Icarus.Models.SortOrder;

namespace Gallio.Icarus.Tests.TestExplorer
{
    [TestsOn(typeof(Controller))]
    public class ControllerTest
    {
        private IEventAggregator eventAggregator;
        private Controller controller;
        private Icarus.TestExplorer.Model model;
        private ITaskManager taskManager;
        private ICommandFactory commandFactory;

        [SetUp]
        public void SetUp()
        {
            eventAggregator = MockRepository.GenerateStub<IEventAggregator>();
            model = new Icarus.TestExplorer.Model(MockRepository.GenerateStub<ISortedTreeModel>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var projectController = MockRepository.GenerateStub<IProjectController>();
            taskManager = MockRepository.GenerateStub<ITaskManager>();
            commandFactory = MockRepository.GenerateStub<ICommandFactory>();
            controller = new Controller(model, eventAggregator, optionsController, projectController, taskManager, commandFactory);
        }

        [Test]
        public void SortTree_should_send_a_SortTreeEvent()
        {
            const SortOrder sortOrder = SortOrder.Ascending;
            controller.SortTree(sortOrder);

            eventAggregator.AssertWasCalled(ea => ea.Send(Arg<SortTreeEvent>.Matches(ste => 
                ste.SortOrder == sortOrder)));
        }

        [Test]
        public void Filter_passed_status_should_update_the_model_when_not_set()
        {
            Assert.IsFalse(model.FilterPassed);

            controller.FilterStatus(TestStatus.Passed);

            Assert.IsTrue(model.FilterPassed);
        }

        [Test]
        public void Filter_passed_status_should_update_the_model_when_already_set()
        {
            model.FilterPassed.Value = true;

            controller.FilterStatus(TestStatus.Passed);
            
            Assert.IsFalse(model.FilterPassed);
        }

        [Test]
        public void Filter_failed_status_should_update_the_model_when_not_set()
        {
            Assert.IsFalse(model.FilterFailed);

            controller.FilterStatus(TestStatus.Failed);

            Assert.IsTrue(model.FilterFailed);
        }

        [Test]
        public void Filter_failed_status_should_update_the_model_when_already_set()
        {
            model.FilterFailed.Value = true;

            controller.FilterStatus(TestStatus.Failed);

            Assert.IsFalse(model.FilterFailed);
        }

        [Test]
        public void Filter_inconclusive_status_should_update_the_model_when_not_set()
        {
            Assert.IsFalse(model.FilterInconclusive);

            controller.FilterStatus(TestStatus.Inconclusive);

            Assert.IsTrue(model.FilterInconclusive);
        }

        [Test]
        public void Filter_inconclusive_status_should_update_the_model_when_already_set()
        {
            model.FilterInconclusive.Value = true;

            controller.FilterStatus(TestStatus.Inconclusive);

            Assert.IsFalse(model.FilterInconclusive);
        }

        [Test]
        public void Filter_status_should_send_FilterTestStatusEvent()
        {
            const TestStatus testStatus = TestStatus.Inconclusive;
            controller.FilterStatus(testStatus);

            eventAggregator.AssertWasCalled(ea => ea.Send(Arg<FilterTestStatusEvent>.Matches(ftse => 
                ftse.TestStatus == testStatus)));
        }

        [Test]
        public void AddFiles_should_retrieve_a_command_from_the_factory()
        {
            var fileNames = new[] { "test" };

            controller.AddFiles(fileNames);

            commandFactory.AssertWasCalled(cf => cf.CreateAddFilesCommand(fileNames));
        }

        [Test]
        public void AddFiles_should_queue_command()
        {
            var command = MockRepository.GenerateStub<ICommand>();
            commandFactory.Stub(cf => cf.CreateAddFilesCommand(Arg<string[]>.Is.Anything))
                .Return(command);
            
            controller.AddFiles(new string[0]);

            taskManager.AssertWasCalled(tm => tm.QueueTask(command));
        }

        [Test]
        public void RemoveAllFiles_should_retrieve_a_command_from_the_factory()
        {
            controller.RemoveAllFiles();

            commandFactory.AssertWasCalled(cf => cf.CreateRemoveAllFilesCommand());
        }

        [Test]
        public void RemoveAllFiles_should_queue_command()
        {
            var command = MockRepository.GenerateStub<ICommand>();
            commandFactory.Stub(cf => cf.CreateRemoveAllFilesCommand()).Return(command);

            controller.RemoveAllFiles();

            taskManager.AssertWasCalled(tm => tm.QueueTask(command));
        }

        [Test]
        public void RemoveFile_should_retrieve_a_command_from_the_factory()
        {
            const string fileName = "fileName";

            controller.RemoveFile(fileName);

            commandFactory.AssertWasCalled(cf => cf.CreateRemoveFileCommand(fileName));
        }

        [Test]
        public void RemoveFile_should_queue_command()
        {
            var command = MockRepository.GenerateStub<ICommand>();
            commandFactory.Stub(cf => cf.CreateRemoveFileCommand(Arg<string>.Is.Anything))
                .Return(command);
            
            controller.RemoveFile("fileName");

            taskManager.AssertWasCalled(tm => tm.QueueTask(command));
        }

        [Test]
        public void RefreshTree_should_retrieve_a_command_from_the_factory()
        {
            controller.ChangeTreeCategory(pm => { });

            commandFactory.AssertWasCalled(cf => cf.CreateRefreshTestTreeCommand());
        }

        [Test]
        public void RefreshTree_should_queue_command()
        {
            var command = MockRepository.GenerateStub<ICommand>();
            commandFactory.Stub(cf => cf.CreateRefreshTestTreeCommand()).Return(command);

            controller.ChangeTreeCategory(pm => { });

            taskManager.AssertWasCalled(tm => tm.QueueTask(command));
        }

        [Test]
        public void ShowSourceCode_should_retrieve_a_command_from_the_factory()
        {
            const string testId = "testId";
            
            controller.ShowSourceCode(testId);

            commandFactory.AssertWasCalled(cf => cf.CreateViewSourceCodeCommand(testId));
        }

        [Test]
        public void ShowSourceCode_should_queue_command()
        {
            var command = MockRepository.GenerateStub<ICommand>();
            commandFactory.Stub(cf => cf.CreateViewSourceCodeCommand(Arg<string>.Is.Anything))
                .Return(command);

            controller.ShowSourceCode("testId");

            taskManager.AssertWasCalled(tm => tm.QueueTask(command));
        }

        [Test]
        public void ResetTests_should_retrieve_a_command_from_the_factory()
        {
            controller.ResetTests();

            commandFactory.AssertWasCalled(cf => cf.CreateResetTestsCommand());
        }

        [Test]
        public void ResetTests_should_queue_command()
        {
            var command = MockRepository.GenerateStub<ICommand>();
            commandFactory.Stub(cf => cf.CreateResetTestsCommand())
                .Return(command);

            controller.ResetTests();

            taskManager.AssertWasCalled(tm => tm.QueueTask(command));
        }

        [Test]
        public void SetTreeSelection_should_send_an_event()
        {
            var nodes = new List<TestTreeNode>();

            controller.SetTreeSelection(nodes);

            eventAggregator.AssertWasCalled(ea => ea.Send(Arg<TestSelectionChanged>.Matches(tsc => 
                CheckElements(nodes, tsc.Nodes))));
        }

        private static bool CheckElements(IEnumerable<TestTreeNode> nodes1, 
            IEnumerable<TestTreeNode> nodes2)
        {
            try
            {
                Assert.AreElementsEqual(nodes1, nodes2);
            }
            catch
            {
                return false;
            }
            return true;
        }

        [Test]
        public void ApplicationShutdown_should_trigger_SaveState_event()
        {
            var flag = false;
            controller.SaveState += (s, e) => flag = true;
            commandFactory.Stub(cf => cf.CreateSaveFilterCommand(Arg<string>.Is.Anything))
                .Return(MockRepository.GenerateStub<ICommand>());

            controller.Handle(new ApplicationShutdown());

            Assert.IsTrue(flag);
        }

        [Test]
        public void ApplicationShutdown_should_retrieve_a_command_from_the_factory()
        {
            commandFactory.Stub(cf => cf.CreateSaveFilterCommand("AutoSave"))
                .Return(MockRepository.GenerateStub<ICommand>());

            controller.Handle(new ApplicationShutdown());
        }

        [Test]
        public void Reload_should_trigger_SaveState_event()
        {
            var flag = false;
            controller.SaveState += (s, e) => flag = true;

            controller.Handle(new Reloading());

            Assert.IsTrue(flag);
        }

        [Test]
        public void RunStarted_should_disable_editing_tree()
        {
            var flag = false;
            model.CanEditTree.PropertyChanged += (s, e) => 
            {
                Assert.IsFalse(model.CanEditTree); 
                flag = true; 
            };

            controller.Handle(new RunStarted());

            Assert.IsTrue(flag);
        }

        [Test]
        public void RunFinished_should_enable_editing_tree()
        {
            var flag = false;
            model.CanEditTree.PropertyChanged += (s, e) =>
            {
                Assert.IsTrue(model.CanEditTree);
                flag = true;
            };

            controller.Handle(new RunFinished());

            Assert.IsTrue(flag);
        }

        [Test]
        public void ExploreStarted_should_disable_editing_tree()
        {
            var flag = false;
            model.CanEditTree.PropertyChanged += (s, e) =>
            {
                Assert.IsFalse(model.CanEditTree);
                flag = true;
            };

            controller.Handle(new ExploreStarted());

            Assert.IsTrue(flag);
        }

        [Test]
        public void ExploreFinished_should_trigger_RestoreState_event()
        {
            var flag = false;
            controller.RestoreState += (s, e) =>
            {
                flag = true;
            };

            controller.Handle(new ExploreFinished());

            Assert.IsTrue(flag);
        }

        [Test]
        public void ExploreFinished_should_enable_editing_tree()
        {
            var flag = false;
            model.CanEditTree.PropertyChanged += (s, e) =>
            {
                Assert.IsTrue(model.CanEditTree);
                flag = true;
            };

            controller.Handle(new ExploreFinished());

            Assert.IsTrue(flag);
        }
    }
}
