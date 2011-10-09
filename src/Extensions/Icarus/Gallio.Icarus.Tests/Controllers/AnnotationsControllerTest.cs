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
using Gallio.Common.Concurrency;
using Gallio.Icarus.Annotations;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Events;
using Gallio.Model;
using Gallio.Model.Schema;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Controllers
{
    [Category("Controllers")]
    public class AnnotationsControllerTest
    {
        private AnnotationsController annotationsController;
        private IOptionsController optionsController;
        private ITestController testController;

        [SetUp]
        public void SetUp()
        {
            testController = MockRepository.GenerateStub<ITestController>();
            optionsController = MockRepository.GenerateStub<IOptionsController>();
            annotationsController = new AnnotationsController(testController, optionsController);
        }

        [Test]
        public void ShowErrors_Test()
        {
            SetupAnnotationData(AnnotationType.Error);
            
            annotationsController.ShowErrors(true);

            Assert.Count(1, annotationsController.Annotations);
            Assert.AreEqual("1 Error", annotationsController.ErrorsText);
        }

        [Test]
        public void ShowWarnings_Test()
        {
            SetupAnnotationData(AnnotationType.Warning);

            annotationsController.ShowWarnings(true);

            Assert.Count(1, annotationsController.Annotations);
            Assert.AreEqual("1 Warning", annotationsController.WarningsText);
        }

        [Test]
        public void ShowInfo_Test()
        {
            SetupAnnotationData(AnnotationType.Info);

            annotationsController.ShowInfos(true);

            Assert.Count(1, annotationsController.Annotations);
            Assert.AreEqual("1 Info", annotationsController.InfoText);
        }

        private void SetupAnnotationData(AnnotationType annotationType)
        {
            var testModelData = new TestModelData();
            testModelData.Annotations.Add(new AnnotationData(annotationType, CodeLocation.Unknown, 
                new CodeReference(), "message", "details"));

            var report = new Report
            {
                TestModel = testModelData
            };

            testController
                .Stub(x => x.ReadReport(null))
                .IgnoreArguments()
                .Repeat.Any()
                .Do((Action<ReadAction<Report>>)(action => action(report)));
        }
    }
}
