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

using System.ComponentModel;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Mediator.Interfaces;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Utilities;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Controllers
{
    internal class ApplicationControllerTest
    {
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

        //[Test]
        //public void Load_assemblies_from_arguments_should_be_added_if_they_exist()
        //{
        //    var fileSystem = MockRepository.GenerateStub<IFileSystem>();
        //    var applicationController = new ApplicationController(new IcarusArguments(),
        //        MockRepository.GenerateStub<IMediator>(), fileSystem);
        //}
    }
}
