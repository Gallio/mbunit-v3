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
using Gallio.Icarus.Mediator.Interfaces;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Icarus.Utilities;
using MbUnit.Framework;
using Rhino.Mocks;

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
            var applicationController = new ApplicationController(new IcarusArguments(), 
                MockRepository.GenerateStub<IMediator>(), MockRepository.GenerateStub<IFileSystem>());

            Assert.AreEqual(Icarus.Properties.Resources.ApplicationName, 
                applicationController.ProjectFileName);
        }

        [Test]
        public void ProjectFileName_should_be_App_name_followed_by_project_name_if_set()
        {
            var applicationController = new ApplicationController(new IcarusArguments(),
                MockRepository.GenerateStub<IMediator>(), MockRepository.GenerateStub<IFileSystem>());
            const string projectFileName = "test";
            applicationController.ProjectFileName = projectFileName;

            Assert.AreEqual(string.Format("{0} - {1}", projectFileName, Icarus.Properties.Resources.ApplicationName), 
                applicationController.ProjectFileName);
        }

        [Test]
        public void ProjectFileName_should_notify_when_set()
        {
            var applicationController = new ApplicationController(new IcarusArguments(),
                MockRepository.GenerateStub<IMediator>(), MockRepository.GenerateStub<IFileSystem>());
            var propertyChangedFlag = false;
            applicationController.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                Assert.AreEqual("ProjectFileName", e.PropertyName);
                propertyChangedFlag = true;
            };
            applicationController.SynchronizationContext = new TestSynchronizationContext();

            applicationController.ProjectFileName = "test";

            Assert.AreEqual(true, propertyChangedFlag);
        }

        [Test]
        public void Load_assemblies_from_arguments_should_be_added_if_they_exist()
        {
            const string assembly1 = "test1.dll";
            const string assembly2 = "test2.dll";
            var arguments = new IcarusArguments { Assemblies = new[] { assembly1, assembly2 } };
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            fileSystem.Stub(fs => fs.FileExists(assembly1)).Return(true);
            var mediator = MockRepository.GenerateMock<IMediator>();
            var applicationController = new ApplicationController(arguments,
                mediator, fileSystem);

            applicationController.Load();

            mediator.AssertWasCalled(m => m.AddAssemblies(Arg<IList<string>>.Matches(list => 
                (list.Count == 1 && list[0] == assembly1))));
        }

        [Test]
        public void Load_project_files_from_arguments_should_be_loaded_if_they_exist()
        {
            const string projectFile = "test.gallio";
            var arguments = new IcarusArguments { Assemblies = new[] { projectFile } };
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            fileSystem.Stub(fs => fs.FileExists(projectFile)).Return(true);
            var mediator = MockRepository.GenerateMock<IMediator>();
            var applicationController = new ApplicationController(arguments,
                mediator, fileSystem);

            applicationController.Load();

            mediator.AssertWasCalled(m => m.OpenProject(projectFile));
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
            var applicationController = new ApplicationController(new IcarusArguments(), mediator, 
                MockRepository.GenerateStub<IFileSystem>());

            applicationController.Load();

            mediator.AssertWasCalled(m => m.OpenProject(projectFile));            
        }

        [Test]
        public void OpenProject_should_call_mediator()
        {
            const string projectFile = "test.gallio";
            var mediator = MockRepository.GenerateMock<IMediator>();
            var applicationController = new ApplicationController(new IcarusArguments(), mediator,
                MockRepository.GenerateStub<IFileSystem>());

            applicationController.OpenProject(projectFile);

            mediator.AssertWasCalled(m => m.OpenProject(projectFile));
        }

        [Test]
        public void OpenProject_should_notify_filename_has_changed()
        {
            var applicationController = new ApplicationController(new IcarusArguments(),
                MockRepository.GenerateStub<IMediator>(), MockRepository.GenerateStub<IFileSystem>());
            var propertyChangedFlag = false;
            applicationController.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                Assert.AreEqual("ProjectFileName", e.PropertyName);
                propertyChangedFlag = true;
            };
            applicationController.SynchronizationContext = new TestSynchronizationContext();
            
            applicationController.OpenProject("test.gallio");

            Assert.AreEqual(true, propertyChangedFlag);
        }

        [Test]
        public void SaveProject_should_call_mediator()
        {
            const string projectFile = "test.gallio";
            var mediator = MockRepository.GenerateMock<IMediator>();
            var applicationController = new ApplicationController(new IcarusArguments(), mediator,
                MockRepository.GenerateStub<IFileSystem>()) { ProjectFileName = projectFile };

            applicationController.SaveProject();

            mediator.AssertWasCalled(m => m.SaveProject(projectFile));
        }

        [Test]
        public void NewProject_should_call_mediator()
        {
            var mediator = MockRepository.GenerateMock<IMediator>();
            var applicationController = new ApplicationController(new IcarusArguments(), mediator,
                MockRepository.GenerateStub<IFileSystem>());

            applicationController.NewProject();

            mediator.AssertWasCalled(m => m.NewProject());
        }
    }
}
