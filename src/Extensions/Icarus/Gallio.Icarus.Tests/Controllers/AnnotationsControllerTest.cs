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
using Gallio.Common.Concurrency;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Model;
using Gallio.Model.Serialization;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Controllers
{
    [Category("Controllers")]
    class AnnotationsControllerTest
    {
        [Test]
        public void ShowErrors_Test()
        {
            // Arrange
            var testController = SetupAnnotationData(AnnotationType.Error);
            var optionsController = MockRepository.GenerateStub<IOptionsController>();

            // Act
            var annotationsController = new AnnotationsController(testController, optionsController);
            testController.Raise(tc => tc.ExploreFinished += null, this, EventArgs.Empty);
            
            // Assert
            Assert.IsFalse(annotationsController.ShowErrors);
            Assert.AreEqual(0, annotationsController.Annotations.Count);
            annotationsController.ShowErrors = true;
            Assert.AreEqual(1, annotationsController.Annotations.Count);
            Assert.AreEqual("1 Error", annotationsController.ErrorsText);
        }

        [Test]
        public void ShowWarnings_Test()
        {
            // Arrange
            var testController = SetupAnnotationData(AnnotationType.Warning);
            var optionsController = MockRepository.GenerateStub<IOptionsController>();

            // Act
            var annotationsController = new AnnotationsController(testController, optionsController);
            testController.Raise(tc => tc.ExploreFinished += null, this, EventArgs.Empty);

            // Assert
            Assert.IsFalse(annotationsController.ShowWarnings);
            Assert.AreEqual(0, annotationsController.Annotations.Count);
            annotationsController.ShowWarnings = true;
            Assert.AreEqual(1, annotationsController.Annotations.Count);
            Assert.AreEqual("1 Warning", annotationsController.WarningsText);
        }

        [Test]
        public void ShowInfo_Test()
        {
            // Arrange
            var testController = SetupAnnotationData(AnnotationType.Info);
            var optionsController = MockRepository.GenerateStub<IOptionsController>();

            // Act
            var annotationsController = new AnnotationsController(testController, optionsController);
            testController.Raise(tc => tc.ExploreFinished += null, this, EventArgs.Empty);

            // Assert
            Assert.IsFalse(annotationsController.ShowInfos);
            Assert.AreEqual(0, annotationsController.Annotations.Count);
            annotationsController.ShowInfos = true;
            Assert.AreEqual(1, annotationsController.Annotations.Count);
            Assert.AreEqual("1 Info", annotationsController.InfoText);
        }

        private static ITestController SetupAnnotationData(AnnotationType annotationType)
        {
            var testModelData = new TestModelData();
            testModelData.Annotations.Add(new AnnotationData(annotationType, CodeLocation.Unknown,
                                                             new CodeReference(), "message", "details"));
            Report report = new Report
                                {
                                    TestModel = testModelData
                                };

            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(x => x.ReadReport(null)).IgnoreArguments().Do((Action<ReadAction<Report>>) (action => action(report)));
            return testController;
        }
    }
}
