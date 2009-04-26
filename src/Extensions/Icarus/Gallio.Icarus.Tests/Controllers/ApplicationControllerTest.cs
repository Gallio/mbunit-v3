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
