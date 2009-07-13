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
using Gallio.Common.Reflection;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Model.Schema;
using Gallio.Runner.Reports.Schema;
using MbUnit.Framework;
using Rhino.Mocks;

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

        [Test]
        public void ShowSourceCode_does_not_fire_if_TestModel_is_null()
        {
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var testController = MockRepository.GenerateStub<ITestController>();
            Report report = new Report();
            testController.Stub(x => x.ReadReport(null)).IgnoreArguments().Do((Action<ReadAction<Report>>)(action => action(report)));
            var sourceCodeController = new SourceCodeController(testController);
            sourceCodeController.ShowSourceCode += (sender, e) => Assert.Fail();

            sourceCodeController.ViewSourceCode("testId", progressMonitor);
        }
    }
}
