using System;
using Gallio.Icarus.Controllers;
using Gallio.Model;
using Gallio.Runtime;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Controllers
{
    [Category("Controllers"), Author("Graham Hay"), TestsOn(typeof(AboutController))]
    internal class AboutControllerTest
    {
        [Test]
        public void TestFrameworks_should_come_from_TestFrameworkManager()
        {
            var testFrameworkManager = RuntimeAccessor.ServiceLocator.Resolve<ITestFrameworkManager>();
            var aboutController = new AboutController(testFrameworkManager);

            Assert.AreEqual(testFrameworkManager.FrameworkHandles.Count, aboutController.TestFrameworks.Count);
        }

        [Test]
        public void Version_string_()
        {
            var testFrameworkManager = MockRepository.GenerateStub<ITestFrameworkManager>();
            var aboutController = new AboutController(testFrameworkManager);

            Assert.StartsWith(aboutController.Version, "Gallio Icarus - Version");
        }

        [Test]
        public void Ctor_should_throw_if_TestFrameworkManager_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new AboutController(null));
        }
    }
}
