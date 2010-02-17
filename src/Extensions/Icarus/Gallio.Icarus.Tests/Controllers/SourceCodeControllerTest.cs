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
using Gallio.Common.Reflection;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Model.Schema;
using Gallio.Runner.Reports.Schema;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Controllers
{
    public class SourceCodeControllerTest
    {
        private SourceCodeController sourceCodeController;
        private ITestController testController;
        private const string testId = "testId";

        [SetUp]
        public void SetUp()
        {
            testController = MockRepository.GenerateStub<ITestController>();
            sourceCodeController = new SourceCodeController(testController);
        }

        [Test]
        public void ShowSourceCode_is_fired_if_code_location_is_available()
        {
            StubReport(CreateReport(new CodeLocation("path", 15, 6)));
            var showSourceCodeFlag = false;
            sourceCodeController.ShowSourceCode += (s, e) =>
            {
                Assert.AreEqual(new CodeLocation("path", 15, 6), e.CodeLocation);
                showSourceCodeFlag = true;
            };

            sourceCodeController.ViewSourceCode(testId, MockProgressMonitor.Instance);
            Assert.AreEqual(true, showSourceCodeFlag);
        }

        private void StubReport(Report report) {
            testController.Stub(x => x.ReadReport(null)).IgnoreArguments()
                .Do((Action<ReadAction<Report>>)(action => action(report)));
        }

        [Test]
        public void ShowSourceCode_does_not_fire_if_code_location_is_unknown()
        {
            var report = CreateReport(CodeLocation.Unknown);
            StubReport(report);
            sourceCodeController.ShowSourceCode += (sender, e) => Assert.Fail();

            sourceCodeController.ViewSourceCode(testId, MockProgressMonitor.Instance);
        }

        private static Report CreateReport(CodeLocation codeLocation)
        {
            var root = new TestData("root", "root", "root");
            var testData = new TestData(testId, "name", "fullName")
            {
                CodeLocation = codeLocation
            };
            root.Children.Add(testData);
            var testModelData = new TestModelData(root);
            return new Report { TestModel = testModelData };
        }

        [Test]
        public void ShowSourceCode_does_not_fire_if_code_location_is_dll()
        {
            StubReport(CreateReport(new CodeLocation("test.dll", 1, 1)));
            sourceCodeController.ShowSourceCode += (sender, e) => Assert.Fail();

            sourceCodeController.ViewSourceCode(testId, MockProgressMonitor.Instance);
        }

        [Test]
        public void ShowSourceCode_does_not_fire_if_code_location_is_DLL()
        {
            StubReport(CreateReport(new CodeLocation("test.DLL", 1, 1)));
            sourceCodeController.ShowSourceCode += (sender, e) => Assert.Fail();

            sourceCodeController.ViewSourceCode(testId, MockProgressMonitor.Instance);
        }

        [Test]
        public void ShowSourceCode_does_not_fire_if_code_location_is_exe()
        {
            StubReport(CreateReport(new CodeLocation("test.exe", 1, 1)));
            sourceCodeController.ShowSourceCode += (sender, e) => Assert.Fail();

            sourceCodeController.ViewSourceCode(testId, MockProgressMonitor.Instance);
        }

        [Test]
        public void ShowSourceCode_does_not_fire_if_code_location_is_EXE()
        {
            StubReport(CreateReport(new CodeLocation("test.EXE", 1, 1)));
            sourceCodeController.ShowSourceCode += (sender, e) => Assert.Fail();

            sourceCodeController.ViewSourceCode(testId, MockProgressMonitor.Instance);
        }

        [Test]
        public void ShowSourceCode_does_not_fire_if_code_location_cannot_be_found()
        {
            StubReport(new Report { TestModel = new TestModelData() });
            sourceCodeController.ShowSourceCode += (sender, e) => Assert.Fail();

            sourceCodeController.ViewSourceCode("testId", MockProgressMonitor.Instance);
        }

        [Test]
        public void ShowSourceCode_does_not_fire_if_TestModel_is_null()
        {
            StubReport(new Report());
            sourceCodeController.ShowSourceCode += (sender, e) => Assert.Fail();

            sourceCodeController.ViewSourceCode("testId", MockProgressMonitor.Instance);
        }
    }
}
