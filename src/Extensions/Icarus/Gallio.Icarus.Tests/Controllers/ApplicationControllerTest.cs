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
using System.ComponentModel;
using Gallio.Common.IO;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Icarus.Utilities;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Common.Policies;
using Gallio.UI.Events;
using Gallio.UI.ProgressMonitoring;
using MbUnit.Framework;
using Rhino.Mocks;
using Gallio.Icarus.Commands;

namespace Gallio.Icarus.Tests.Controllers
{
    [MbUnit.Framework.Category("Controllers"), TestsOn(typeof(ApplicationController))]
    public class ApplicationControllerTest
    {
        private ApplicationController applicationController;
        private IOptionsController optionsController;
        private IFileSystem fileSystem;
        private ITaskManager taskManager;
        private ITestController testController;
        private IProjectController projectController;
        private IUnhandledExceptionPolicy unhandledExceptionPolicy;
        private IEventAggregator eventAggregator;
        private ICommandFactory commandFactory;
        private ICommand command;

        [SetUp]
        public void SetUp()
        {
            optionsController = MockRepository.GenerateStub<IOptionsController>();
            fileSystem = MockRepository.GenerateStub<IFileSystem>();
            taskManager = MockRepository.GenerateStub<ITaskManager>();
            testController = MockRepository.GenerateStub<ITestController>();
            projectController = MockRepository.GenerateStub<IProjectController>();
            unhandledExceptionPolicy = MockRepository.GenerateStub<IUnhandledExceptionPolicy>();
            eventAggregator = MockRepository.GenerateStub<IEventAggregator>();
            commandFactory = MockRepository.GenerateStub<ICommandFactory>();
            applicationController = new ApplicationController(optionsController,fileSystem,taskManager,testController,
                                projectController,unhandledExceptionPolicy,eventAggregator,commandFactory);
            command = MockRepository.GenerateStub<ICommand>();
        }

        [Test]
        public void ProjectFileName_should_be_App_name_if_not_set()
        {
            Assert.AreEqual(Icarus.Properties.Resources.ApplicationName, 
                applicationController.Title);
        }
        
        [Test]
        public void ProjectFileName_should_be_App_name_followed_by_project_name_if_set()
        {
            const string projectName = "test";
            applicationController.Title = projectName;

            Assert.AreEqual(string.Format("{0} - {1}", projectName, Icarus.Properties.Resources.ApplicationName), 
                applicationController.Title);
        }
        
        [SyncTest]
        public void ProjectFileName_should_notify_when_set()
        {
            var propertyChangedFlag = false;
            applicationController.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                Assert.AreEqual("ProjectFileName", e.PropertyName);
                propertyChangedFlag = true;
            };

            applicationController.Title = "test";

            Assert.AreEqual(true, propertyChangedFlag);
        }

        [Test]
        public void Load_files_from_arguments_should_be_added_if_they_exist()
        {
            const string file1 = "test1.dll";
            const string file2 = "test2.dll";
            var files = new List<string>() {file1};
            var arguments = new IcarusArguments { Files = new[] { file1, file2 } };
            fileSystem.Stub(fs => fs.FileExists(file1)).Return(true);
            commandFactory.Stub(cf => cf.CreateAddFilesCommand(files)).Return(command);            

            applicationController.Arguments = arguments;
            applicationController.Load();

            taskManager.AssertWasCalled(t => t.QueueTask(command));
        }

        [Test]
        public void Load_project_files_from_arguments_should_be_loaded_if_they_exist()
        {
            const string projectFile = "test.gallio";
            var arguments = new IcarusArguments { Files = new[] { projectFile } };
            fileSystem.Stub(fs => fs.FileExists(projectFile)).Return(true);
            commandFactory.Stub(cf => cf.CreateOpenProjectCommand(projectFile)).Return(command);

            applicationController.Arguments = arguments;
            applicationController.Load();

            taskManager.AssertWasCalled(t => t.QueueTask(command));
        }
        
        [Test]
        public void Load_should_restore_last_project_used_if_required_and_no_args_supplied()
        {
            const string projectFile = "test.gallio";
            var arguments = new IcarusArguments();
            optionsController.RestorePreviousSettings = true;
            optionsController.Stub(oc => oc.RecentProjects).Return(new MRUList(new List<string>(new[] { projectFile }), 10));
            fileSystem.Stub(fs => fs.FileExists(projectFile)).Return(true);
            commandFactory.Stub(cf => cf.CreateOpenProjectCommand(projectFile)).Return(command);

            applicationController.Arguments = arguments;
            applicationController.Load();

            taskManager.AssertWasCalled(t => t.QueueTask(command));
        }

        [Test]
        public void Load_with_no_restore_and_no_args_should_create_new_project()
        {
            const string projectFile = "test.gallio";
            var arguments = new IcarusArguments();
            optionsController.RestorePreviousSettings = false;
            optionsController.Stub(oc => oc.RecentProjects).Return(new MRUList(new List<string>(new[] { projectFile }), 10));
            commandFactory.Stub(c => c.CreateNewProjectCommand()).Return(command);

            applicationController.Arguments = arguments;
            applicationController.Load();

            Assert.AreEqual(string.Format("Default - {0}", Icarus.Properties.Resources.ApplicationName),
                            applicationController.Title);
            taskManager.AssertWasCalled(t => t.QueueTask(command));
        }

        [Test]
        public void OpenProject_should_update_title()
        {
            const string projectName = "test";
            commandFactory.Stub(cf => cf.CreateOpenProjectCommand(projectName)).Return(command);

            applicationController.OpenProject(projectName);

            Assert.AreEqual(string.Format("{0} - {1}", projectName, Icarus.Properties.Resources.ApplicationName),
                applicationController.Title);
        }

        [Test]
        public void OpenProject_should_queue_task()
        {
            const string projectName = "test";
            commandFactory.Stub(cf => cf.CreateOpenProjectCommand(projectName)).Return(command);

            applicationController.OpenProject(projectName);

            taskManager.AssertWasCalled(t => t.QueueTask(command));
        }

        [Test]
        public void SaveProject_with_true_should_queue_task()
        {
            string projectName = string.Empty;
            commandFactory.Stub(cf => cf.CreateSaveProjectCommand(projectName)).Return(command);

            applicationController.SaveProject(true);

            taskManager.AssertWasCalled(t => t.QueueTask(command));
        }

        [Test]
        public void SaveProject_with_false_should_execute_command()
        {
            var mocks = new MockRepository();
            command = mocks.StrictMock<ICommand>();
            command.Stub(c => c.Execute(NullProgressMonitor.CreateInstance())).IgnoreArguments();
            
            string projectName = string.Empty;
            commandFactory.Stub(cf => cf.CreateSaveProjectCommand(projectName)).Return(command);

            mocks.ReplayAll();
            applicationController.SaveProject(false);
            
            command.VerifyAllExpectations();
        }

        [Test]
        public void SaveProject_with_false_and_execute_exception_should_report_error()
        {
            string projectName = string.Empty;
            commandFactory.Stub(cf => cf.CreateSaveProjectCommand(projectName)).Return(command);
            var exception = new Exception();
            command.Stub(c => c.Execute(NullProgressMonitor.CreateInstance())).IgnoreArguments().Throw(exception);

            applicationController.SaveProject(false);

            commandFactory.AssertWasCalled(cf => cf.CreateSaveProjectCommand(projectName));
            unhandledExceptionPolicy.AssertWasCalled(uep => uep.Report("Error saving project",exception));
        }      
        
        [Test]
        public void NewProject_should_queue_task()
        {
            commandFactory.Stub(c => c.CreateNewProjectCommand()).Return(command);

            applicationController.NewProject();
            
            Assert.AreEqual(string.Format("Default - {0}",  Icarus.Properties.Resources.ApplicationName),
                            applicationController.Title); 
            taskManager.AssertWasCalled(t => t.QueueTask(command));
        }

        [Test]
        public void Shutdown_should_save_options_and_project()
        {
            string projectName = string.Empty;
            commandFactory.Stub(cf => cf.CreateSaveProjectCommand(projectName)).Return(command);

            applicationController.Shutdown();

            optionsController.AssertWasCalled(oc => oc.Save());
            commandFactory.AssertWasCalled(cf => cf.CreateSaveProjectCommand(projectName));           

        }
    }
}
