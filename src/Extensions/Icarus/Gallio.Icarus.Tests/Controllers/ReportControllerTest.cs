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

using System.Collections.Generic;
using System.IO;
using Gallio.Common.IO;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Services.Interfaces;
using Gallio.Runner.Reports;
using Gallio.Runtime.ProgressMonitoring;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Controllers
{
    [Category("Controllers"), TestsOn(typeof(ReportController))]
    internal class ReportControllerTest
    {
        [Test]
        public void GenerateReport_Test()
        {
            Report report = new Report();
            var reportService = MockRepository.GenerateMock<IReportService>();
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            ReportController reportController = new ReportController(reportService, fileSystem);
            string reportDirectory = Path.Combine(Path.GetTempPath(), "GallioReport");
            
            reportController.GenerateReport(report, reportDirectory, progressMonitor);

            reportService.AssertWasCalled(rs => rs.SaveReportAs(Arg.Is(report), Arg<string>.Is.Anything, Arg.Is("xml"),
                Arg.Is(progressMonitor)));
        }

        [Test]
        public void ReportTypes_Test()
        {
            var reportService = MockRepository.GenerateMock<IReportService>();
            var list = new List<string>(new[] {"test"});
            reportService.Stub(rs => rs.ReportTypes).Return(list);
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var reportController = new ReportController(reportService, fileSystem);
            Assert.AreEqual(list, reportController.ReportTypes);
        }

        [Test]
        public void ShowReport_Test()
        {
            var report = new Report();
            const string reportType = "test";
            const string reportName = "reportName";
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var reportService = MockRepository.GenerateMock<IReportService>();
            reportService.Stub(rs => rs.SaveReportAs(Arg.Is(report), Arg<string>.Is.Anything, Arg.Is(reportType),
                Arg.Is(progressMonitor))).Return(reportName);
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var reportController = new ReportController(reportService, fileSystem);
            
            Assert.AreEqual(reportName, reportController.ShowReport(report, reportType, progressMonitor));
        }

        [Test]
        public void ConvertSavedReport_should_call_same_on_ReportService()
        {
            const string fileName = "fileName";
            const string format = "format";
            var reportService = MockRepository.GenerateMock<IReportService>();
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var reportController = new ReportController(reportService, fileSystem);

            reportController.ConvertSavedReport(fileName, format, progressMonitor);

            reportService.AssertWasCalled(rs => rs.ConvertSavedReport(fileName, format, progressMonitor));
        }

        [Test]
        public void DeleteFile_should_pass_through_to_FileSystem()
        {
            const string fileName = "fileName";
            var reportService = MockRepository.GenerateMock<IReportService>();
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var reportController = new ReportController(reportService, fileSystem);

            reportController.DeleteReport(fileName, progressMonitor);

            fileSystem.AssertWasCalled(fs => fs.DeleteFile(fileName));
        }
    }
}
