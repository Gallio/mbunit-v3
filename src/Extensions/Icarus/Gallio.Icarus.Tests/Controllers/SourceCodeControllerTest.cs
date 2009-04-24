using System;
using Gallio.Concurrency;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Model.Serialization;
using Gallio.Reflection;
using Gallio.Runner.Reports;
using MbUnit.Framework;
using Rhino.Mocks;
using Gallio.Icarus.Controllers.EventArgs;

namespace Gallio.Icarus.Tests.Controllers
{
    internal class SourceCodeControllerTest
    {
        [Test]
        public void ShowSourceCode_is_fired_if_code_location_is_available()
        {
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var testController = MockRepository.GenerateStub<ITestController>();
            var root = new TestData("root", "root", "root");
            var testData = new TestData("testId", "testId", "testId");
            var codeLocation = new CodeLocation("path", 15, 6);
            testData.CodeLocation = codeLocation;
            root.Children.Add(testData);
            var testModelData = new TestModelData(root);
            Report report = new Report { TestModel = testModelData };
            testController.Stub(x => x.ReadReport(null)).IgnoreArguments().Do((Action<ReadAction<Report>>)(action => action(report)));
            var sourceCodeController = new SourceCodeController(testController);
            var showSourceCodeFlag = false;
            sourceCodeController.ShowSourceCode += delegate(object sender, ShowSourceCodeEventArgs e)
            {
                Assert.AreEqual(codeLocation, e.CodeLocation);
                showSourceCodeFlag = true;
            };

            sourceCodeController.ViewSourceCode("testId", progressMonitor);
            Assert.AreEqual(true, showSourceCodeFlag);
        }

        [Test]
        public void ShowSourceCode_does_not_fire_if_code_location_is_unknown()
        {
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var testController = MockRepository.GenerateStub<ITestController>();
            var root = new TestData("root", "root", "root");
            var testData = new TestData("testId", "testId", "testId");
            testData.CodeLocation = CodeLocation.Unknown;
            root.Children.Add(testData);
            var testModelData = new TestModelData(root);
            Report report = new Report { TestModel = testModelData };
            testController.Stub(x => x.ReadReport(null)).IgnoreArguments().Do((Action<ReadAction<Report>>)(action => action(report)));
            var sourceCodeController = new SourceCodeController(testController);
            sourceCodeController.ShowSourceCode += (sender, e) => Assert.Fail();

            sourceCodeController.ViewSourceCode("testId", progressMonitor);
        }

        [Test]
        public void ShowSourceCode_does_not_fire_if_code_location_is_dll()
        {
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var testController = MockRepository.GenerateStub<ITestController>();
            var root = new TestData("root", "root", "root");
            var testData = new TestData("testId", "testId", "testId");
            testData.CodeLocation = new CodeLocation("test.dll", 1, 1);
            root.Children.Add(testData);
            var testModelData = new TestModelData(root);
            Report report = new Report { TestModel = testModelData };
            testController.Stub(x => x.ReadReport(null)).IgnoreArguments().Do((Action<ReadAction<Report>>)(action => action(report)));
            var sourceCodeController = new SourceCodeController(testController);
            sourceCodeController.ShowSourceCode += (sender, e) => Assert.Fail();

            sourceCodeController.ViewSourceCode("testId", progressMonitor);
        }

        [Test]
        public void ShowSourceCode_does_not_fire_if_code_location_is_exe()
        {
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var testController = MockRepository.GenerateStub<ITestController>();
            var root = new TestData("root", "root", "root");
            var testData = new TestData("testId", "testId", "testId");
            testData.CodeLocation = new CodeLocation("test.exe", 1, 1);
            root.Children.Add(testData);
            var testModelData = new TestModelData(root);
            Report report = new Report { TestModel = testModelData };
            testController.Stub(x => x.ReadReport(null)).IgnoreArguments().Do((Action<ReadAction<Report>>)(action => action(report)));
            var sourceCodeController = new SourceCodeController(testController);
            sourceCodeController.ShowSourceCode += (sender, e) => Assert.Fail();

            sourceCodeController.ViewSourceCode("testId", progressMonitor);
        }

        [Test]
        public void ShowSourceCode_does_not_fire_if_code_location_cannot_be_found()
        {
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var testController = MockRepository.GenerateStub<ITestController>();
            var testModelData = new TestModelData();
            Report report = new Report { TestModel = testModelData };
            testController.Stub(x => x.ReadReport(null)).IgnoreArguments().Do((Action<ReadAction<Report>>)(action => action(report)));
            var sourceCodeController = new SourceCodeController(testController);
            sourceCodeController.ShowSourceCode += (sender, e) => Assert.Fail();

            sourceCodeController.ViewSourceCode("testId", progressMonitor);
        }
    }
}
