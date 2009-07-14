// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using MbUnit.Framework;
using Rhino.Mocks;
using Gallio.Icarus.Commands;

namespace Gallio.Icarus.Tests.Controllers
{
    internal class ApplicationControllerTest
    {
        [Test]
        public void Ctor_should_throw_if_args_is_null()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new ApplicationController(null, 
                MockRepository.GenerateStub<IMediator>(), MockRepository.GenerateStub<IFileSystem>()));
            Assert.AreEqual("arguments", ex.ParamName);
        }

        [Test]
        public void Ctor_should_throw_if_mediator_is_null()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new ApplicationController(new IcarusArguments(),
                null, MockRepository.GenerateStub<IFileSystem>()));
            Assert.AreEqual("mediator", ex.ParamName);
        }

        [Test]
        public void Ctor_should_throw_if_filesystem_is_null()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new ApplicationController(new IcarusArguments(),
                MockRepository.GenerateStub<IMediator>(), null));
            Assert.AreEqual("fileSystem", ex.ParamName);
        }

        [Test]
        public void ProjectFileName_should_be_App_name_if_not_set()
        {
            var mediator = MockRepository.GenerateStub<IMediator>();
            mediator.OptionsController = MockRepository.GenerateStub<IOptionsController>();
            var applicationController = new ApplicationController(new IcarusArguments(), 
                mediator, MockRepository.GenerateStub<IFileSystem>());

            Assert.AreEqual(Icarus.Properties.Resources.ApplicationName, 
                applicationController.Title);
        }

        [Test]
        public void ProjectFileName_should_be_App_name_followed_by_project_name_if_set()
        {
            var mediator = MockRepository.GenerateStub<IMediator>();
            mediator.OptionsController = MockRepository.GenerateStub<IOptionsController>();
            var applicationController = new ApplicationController(new IcarusArguments(),
                mediator, MockRepository.GenerateStub<IFileSystem>()); 
            const string projectFileName = "test";
            applicationController.Title = projectFileName;

            Assert.AreEqual(string.Format("{0} - {1}", projectFileName, Icarus.Properties.Resources.ApplicationName), 
                applicationController.Title);
        }

        [SyncTest]
        public void ProjectFileName_should_notify_when_set()
        {
            var mediator = MockRepository.GenerateStub<IMediator>();
            mediator.OptionsController = MockRepository.GenerateStub<IOptionsController>();
            var applicationController = new ApplicationController(new IcarusArguments(),
                mediator, MockRepository.GenerateStub<IFileSystem>());
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
            var arguments = new IcarusArguments { Files = new[] { file1, file2 } };
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            fileSystem.Stub(fs => fs.FileExists(file1)).Return(true);
            var mediator = MockRepository.GenerateStub<IMediator>();
            mediator.OptionsController = MockRepository.GenerateStub<IOptionsController>();
            var taskManager = new TestTaskManager();
            mediator.TaskManager = taskManager;
            var applicationController = new ApplicationController(arguments, mediator, fileSystem);

            applicationController.Load();

            Assert.AreEqual(1, taskManager.Queue.Count);
            Assert.IsInstanceOfType(typeof(AddFilesCommand), taskManager.Queue[0]);
        }

        [Test]
        public void Load_project_files_from_arguments_should_be_loaded_if_they_exist()
        {
            const string projectFile = "test.gallio";
            var arguments = new IcarusArguments { Files = new[] { projectFile } };
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            fileSystem.Stub(fs => fs.FileExists(projectFile)).Return(true);
            var mediator = MockRepository.GenerateStub<IMediator>();
            mediator.OptionsController = MockRepository.GenerateStub<IOptionsController>();
            var taskManager = new TestTaskManager();
            mediator.TaskManager = taskManager;
            var applicationController = new ApplicationController(arguments, mediator, fileSystem);

            applicationController.Load();

            Assert.AreEqual(1, taskManager.Queue.Count);
            Assert.IsInstanceOfType(typeof(OpenProjectCommand), taskManager.Queue[0]);
            var command = (OpenProjectCommand)taskManager.Queue[0];
            Assert.AreEqual(projectFile, command.FileName);
        }

        [Test]
        public void Load_should_restore_last_project_used_if_required_and_no_args_supplied()
        {
            const string projectFile = "test.gallio";
            var mediator = MockRepository.GenerateMock<IMediator>();
            mediator.Stub(m => m.OptionsController).Return(MockRepository.GenerateStub<IOptionsController>());
            mediator.OptionsController.RestorePreviousSettings = true;
            mediator.OptionsController.Stub(oc => oc.RecentProjects).Return(new MRUList(
                new List<string>(new[] { projectFile }), 10));
            var taskManager = new TestTaskManager();
            mediator.TaskManager = taskManager;
            var applicationController = new ApplicationController(new IcarusArguments(), mediator, 
                MockRepository.GenerateStub<IFileSystem>());

            applicationController.Load();

            Assert.AreEqual(1, taskManager.Queue.Count);
            Assert.IsInstanceOfType(typeof(OpenProjectCommand), taskManager.Queue[0]);
            var command = (OpenProjectCommand)taskManager.Queue[0];
            Assert.AreEqual(projectFile, command.FileName);
        }

        [Test]
        public void OpenProject_should_call_mediator()
        {
            const string projectFile = "test.gallio";
            var mediator = MockRepository.GenerateStub<IMediator>();
            mediator.OptionsController = MockRepository.GenerateStub<IOptionsController>();
            var taskManager = new TestTaskManager();
            mediator.TaskManager = taskManager;
            var applicationController = new ApplicationController(new IcarusArguments(),
                mediator, MockRepository.GenerateStub<IFileSystem>());

            applicationController.OpenProject(projectFile);

            Assert.AreEqual(1, taskManager.Queue.Count);
            Assert.IsInstanceOfType(typeof(OpenProjectCommand), taskManager.Queue[0]);
            var command = (OpenProjectCommand)taskManager.Queue[0];
            Assert.AreEqual(projectFile, command.FileName);
        }

        [SyncTest]
        public void OpenProject_should_notify_filename_has_changed()
        {
            var mediator = MockRepository.GenerateMock<IMediator>();
            mediator.Stub(m => m.OptionsController).Return(MockRepository.GenerateStub<IOptionsController>());
            var applicationController = new ApplicationController(new IcarusArguments(),
                mediator, MockRepository.GenerateStub<IFileSystem>()); 
            var propertyChangedFlag = false;
            applicationController.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                Assert.AreEqual("ProjectFileName", e.PropertyName);
                propertyChangedFlag = true;
            };
            
            applicationController.OpenProject("test.gallio");

            Assert.AreEqual(true, propertyChangedFlag);
        }

        [Test]
        public void SaveProject_should_call_mediator()
        {
            const string projectFile = "test.gallio";
            var mediator = MockRepository.GenerateMock<IMediator>();
            mediator.Stub(m => m.OptionsController).Return(MockRepository.GenerateStub<IOptionsController>());
            var applicationController = new ApplicationController(new IcarusArguments(),
                mediator, MockRepository.GenerateStub<IFileSystem>())
                {
                    Title = projectFile 
                };

            applicationController.SaveProject();

            mediator.AssertWasCalled(m => m.SaveProject(projectFile));
        }

        [Test]
        public void NewProject_should_call_mediator()
        {
            var mediator = MockRepository.GenerateMock<IMediator>();
            mediator.Stub(m => m.OptionsController).Return(MockRepository.GenerateStub<IOptionsController>());
            var applicationController = new ApplicationController(new IcarusArguments(),
                mediator, MockRepository.GenerateStub<IFileSystem>());

            applicationController.NewProject();

            mediator.AssertWasCalled(m => m.NewProject());
        }
    }
}
