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
using System.IO;
using Gallio.Common.IO;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Events;
using Gallio.Icarus.ProjectProperties;
using Gallio.Icarus.Properties;
using Gallio.Icarus.Utilities;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.ProjectProperties
{
    public class ControllerTest
    {
        private Controller controller;
        private IFileSystem fileSystem;
        private IProjectController projectController;
        private Icarus.ProjectProperties.Model model;
        private ITestRunnerExtensionFactory testRunnerExtensionFactory;

        [SetUp]
        public void SetUp()
        {
            fileSystem = MockRepository.GenerateStub<IFileSystem>();
            projectController = MockRepository.GenerateStub<IProjectController>();
            testRunnerExtensionFactory = MockRepository.GenerateStub<ITestRunnerExtensionFactory>();
            model = new Icarus.ProjectProperties.Model();
            controller = new Controller(model, fileSystem, projectController, 
                testRunnerExtensionFactory);
        }

        [Test]
        public void Add_hint_directory_throws_if_directory_does_not_exist()
        {
            const string hintDirectory = "hintDirectory";
            fileSystem.Stub(fs => fs.DirectoryExists(hintDirectory)).Return(false);

            var exception = Assert.Throws<Exception>(() => controller.AddHintDirectory(hintDirectory));

            Assert.AreEqual(Resources.Controller_Invalid_hint_directory_path_, exception.Message);
        }

        [Test]
        public void Add_hint_directory_does_not_throw_if_directory_is_empty_string()
        {
            const string hintDirectory = "";
            fileSystem.Stub(fs => fs.DirectoryExists(hintDirectory)).Return(false);

            controller.AddHintDirectory(hintDirectory);
        }

        [Test]
        public void Add_hint_directory_adds_it_to_the_project()
        {
            const string hintDirectory = "hintDirectory";
            fileSystem.Stub(fs => fs.DirectoryExists(hintDirectory)).Return(true);

            controller.AddHintDirectory(hintDirectory);

            projectController.AssertWasCalled(pc => pc.AddHintDirectory(hintDirectory));
        }

        [Test]
        public void Add_hint_directory_adds_it_to_the_model()
        {
            const string hintDirectory = "hintDirectory";
            fileSystem.Stub(fs => fs.DirectoryExists(hintDirectory)).Return(true);
            var flag = false;
            model.HintDirectories.PropertyChanged += (s, e) =>
            {
                if (model.HintDirectories.Contains(hintDirectory))
                    flag = true;
            };

            controller.AddHintDirectory(hintDirectory);

            Assert.IsTrue(flag);
        }

        [Test]
        public void Add_test_runner_extension_specification_checks_it_is_a_valid_specification()
        {
            const string testRunnerExtensionSpecification = "testRunnerExtensionSpecification";

            controller.AddTestRunnerExtensionSpecification(testRunnerExtensionSpecification);

            testRunnerExtensionFactory.AssertWasCalled(tref => 
                tref.CreateFromSpecification(testRunnerExtensionSpecification));
        }

        [Test]
        public void Add_test_runner_extension_specification_adds_it_to_the_project()
        {
            const string testRunnerExtensionSpecification = "testRunnerExtensionSpecification";

            controller.AddTestRunnerExtensionSpecification(testRunnerExtensionSpecification);

            projectController.AssertWasCalled(pc => pc.AddTestRunnerExtensionSpecification(testRunnerExtensionSpecification));
        }

        [Test]
        public void Add_test_runner_extension_specification_adds_it_to_the_model()
        {
            const string testRunnerExtensionSpecification = "testRunnerExtensionSpecification";
            var flag = false;
            model.TestRunnerExtensionSpecifications.PropertyChanged += (s, e) =>
            {
                if (model.TestRunnerExtensionSpecifications.Contains(testRunnerExtensionSpecification))
                    flag = true;
            };

            controller.AddTestRunnerExtensionSpecification(testRunnerExtensionSpecification);

            Assert.IsTrue(flag);
        }

        [Test]
        public void Load_binds_model()
        {
            projectController.Stub(pc => pc.HintDirectories).Return(new DirectoryInfo[0]);
            projectController.Stub(pc => pc.TestRunnerExtensionSpecifications).Return(new string[0]);
            var flag = false;
            model.ApplicationBaseDirectory.PropertyChanged += (s, e) =>
            {
                flag = true;
            };

            controller.Load();

            Assert.IsTrue(flag);
        }

        [Test]
        public void Remove_hint_directory_removes_it_from_the_project()
        {
            const string hintDirectory = "hintDirectory";

            controller.RemoveHintDirectory(hintDirectory);

            projectController.AssertWasCalled(pc => pc.RemoveHintDirectory(hintDirectory));
        }

        [Test]
        public void Remove_hint_directory_removes_it_from_the_the_model()
        {
            const string hintDirectory = "hintDirectory";
            model.HintDirectories.Add(hintDirectory);
            var flag = false;
            model.HintDirectories.PropertyChanged += (s, e) =>
            {
                if (model.HintDirectories.Contains(hintDirectory) == false)
                    flag = true;
            };

            controller.RemoveHintDirectory(hintDirectory);

            Assert.IsTrue(flag);
        }

        [Test]
        public void Remove_test_runner_extension_specification_removes_it_from_the_project()
        {
            const string testRunnerExtensionSpecification = "testRunnerExtensionSpecification";

            controller.RemoveTestRunnerExtensionSpecification(testRunnerExtensionSpecification);

            projectController.AssertWasCalled(pc => 
                pc.RemoveTestRunnerExtensionSpecification(testRunnerExtensionSpecification));
        }

        [Test]
        public void Remove_test_runner_extension_specification_removes_it_from_the_the_model()
        {
            const string testRunnerExtensionSpecification = "testRunnerExtensionSpecification";
            model.TestRunnerExtensionSpecifications.Add(testRunnerExtensionSpecification);
            var flag = false;
            model.TestRunnerExtensionSpecifications.PropertyChanged += (s, e) =>
            {
                if (model.TestRunnerExtensionSpecifications.Contains(testRunnerExtensionSpecification) == false)
                    flag = true;
            };

            controller.RemoveTestRunnerExtensionSpecification(testRunnerExtensionSpecification);

            Assert.IsTrue(flag);
        }

        [Test]
        public void Set_application_base_directory_throws_if_directory_does_not_exist()
        {
            const string applicationBaseDirectory = "applicationBaseDirectory";
            fileSystem.Stub(fs => fs.DirectoryExists(applicationBaseDirectory)).Return(false);

            var exception = Assert.Throws<Exception>(() => controller.SetApplicationBaseDirectory(applicationBaseDirectory));

            Assert.AreEqual(Resources.Controller_Invalid_application_base_directory, exception.Message);
        }

        [Test]
        public void Set_application_base_directory_does_not_throw_if_directory_is_empty_string()
        {
            const string applicationBaseDirectory = "";
            fileSystem.Stub(fs => fs.DirectoryExists(applicationBaseDirectory)).Return(false);

            controller.SetApplicationBaseDirectory(applicationBaseDirectory);
        }

        [Test]
        public void Set_application_base_directory_sets_it_on_the_project()
        {
            const string applicationBaseDirectory = "applicationBaseDirectory";
            fileSystem.Stub(fs => fs.DirectoryExists(applicationBaseDirectory)).Return(true);

            controller.SetApplicationBaseDirectory(applicationBaseDirectory);

            projectController.AssertWasCalled(pc => 
                pc.SetApplicationBaseDirectory(applicationBaseDirectory));
        }

        [Test]
        public void Set_report_name_format_sets_it_on_the_project()
        {
            const string reportNameFormat = "reportNameFormat";

            controller.SetReportNameFormat(reportNameFormat);

            projectController.AssertWasCalled(pc => pc.SetReportNameFormat(reportNameFormat));
        }

        [Test]
        public void Set_shadow_copy_sets_it_on_the_project()
        {
            const bool shadowCopy = false;

            controller.SetShadowCopy(shadowCopy);

            projectController.AssertWasCalled(pc => pc.SetShadowCopy(shadowCopy));
        }

        [Test]
        public void Set_working_directory_throws_if_directory_does_not_exist()
        {
            const string workingDirectory = "workingDirectory";
            fileSystem.Stub(fs => fs.DirectoryExists(workingDirectory)).Return(false);

            var exception = Assert.Throws<Exception>(() => controller.SetWorkingDirectory(workingDirectory));

            Assert.AreEqual(Resources.Controller_Invalid_working_directory, exception.Message);
        }

        [Test]
        public void Set_working_directory_does_not_throw_if_directory_is_empty_string()
        {
            const string workingDirectory = "";
            fileSystem.Stub(fs => fs.DirectoryExists(workingDirectory)).Return(false);

            controller.SetWorkingDirectory(workingDirectory);
        }

        [Test]
        public void Set_working_directory_sets_it_on_the_project()
        {
            const string workingDirectory = "workingDirectory";
            fileSystem.Stub(fs => fs.DirectoryExists(workingDirectory)).Return(true);

            controller.SetWorkingDirectory(workingDirectory);

            projectController.AssertWasCalled(pc => pc.SetWorkingDirectory(workingDirectory));
        }

        [Test]
        public void Handle_project_loaded_binds_model()
        {
            projectController.Stub(pc => pc.HintDirectories).Return(new DirectoryInfo[0]);
            projectController.Stub(pc => pc.TestRunnerExtensionSpecifications).Return(new string[0]);
            var flag = false;
            model.ApplicationBaseDirectory.PropertyChanged += (s, e) =>
            {
                flag = true;
            };

            controller.Handle(new ProjectLoaded(""));

            Assert.IsTrue(flag);
        }
    }
}
