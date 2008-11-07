using System;
using Gallio.Concurrency;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Model;
using Gallio.Model.Serialization;
using Gallio.Reflection;
using Gallio.Runner.Reports;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Gallio.Icarus.Tests.Controllers
{
    [Category("Controllers")]
    class AnnotationsControllerTest : MockTest
    {
        private AnnotationsController annotationsController;

        [SetUp]
        public void SetUp()
        {
            ITestController testController = mocks.StrictMock<ITestController>();
            testController.LoadFinished += null;
            IEventRaiser loadFinished = LastCall.IgnoreArguments().GetEventRaiser();
            Report report = new Report
            {
                TestModel = new TestModelData(new TestData("id", "name", "fullName"))
            };
            report.TestModel.Annotations.AddRange(new[]
            {
                new AnnotationData(AnnotationType.Error, CodeLocation.Unknown, new CodeReference(), "message", "details"),
                new AnnotationData(AnnotationType.Warning, CodeLocation.Unknown, new CodeReference(), "message", "details"), 
                new AnnotationData(AnnotationType.Info, CodeLocation.Unknown, new CodeReference(), "message", "details"), 
            });
            Expect.Call(testController.Report).Return(new LockBox<Report>(report)).Repeat.Twice();
            mocks.ReplayAll();
            annotationsController = new AnnotationsController(testController);
            loadFinished.Raise(testController, EventArgs.Empty);            
        }

        [Test]
        public void ShowErrors_Test()
        {
            Assert.IsTrue(annotationsController.ShowErrors);
            Assert.AreEqual(3, annotationsController.Annotations.Count);
            Assert.AreEqual("1 Errors", annotationsController.ErrorsText);
            annotationsController.ShowErrors = false;
            Assert.AreEqual(2, annotationsController.Annotations.Count);
        }

        [Test]
        public void ShowWarnings_Test()
        {
            Assert.IsTrue(annotationsController.ShowWarnings);
            Assert.AreEqual(3, annotationsController.Annotations.Count);
            Assert.AreEqual("1 Warnings", annotationsController.WarningsText);
            annotationsController.ShowWarnings = false;
            Assert.AreEqual(2, annotationsController.Annotations.Count);
        }

        [Test]
        public void ShowInfo_Test()
        {
            Assert.IsTrue(annotationsController.ShowInfo);
            Assert.AreEqual(3, annotationsController.Annotations.Count);
            Assert.AreEqual("1 Info", annotationsController.InfoText);
            annotationsController.ShowInfo = false;
            Assert.AreEqual(2, annotationsController.Annotations.Count);
        }
    }
}
