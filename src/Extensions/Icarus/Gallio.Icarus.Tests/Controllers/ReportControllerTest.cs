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

using System.Collections.Generic;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Reports;
using Gallio.Icarus.Services.Interfaces;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Runner.Reports.Schema;
using Gallio.Runtime.ProgressMonitoring;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Controllers
{
    [Category("Controllers"), TestsOn(typeof(ReportController))]
    internal class ReportControllerTest
    {
        private ReportController reportController;
        private IReportService reportService;

        [SetUp]
        public void EstablishContext()
        {
            reportService = MockRepository.GenerateMock<IReportService>();
            reportController = new ReportController(reportService);
        }

        [Test]
        public void GenerateReport_Test()
        {
            var report = new Report();
            var reportOptions = new ReportOptions("reportDirectory", "reportNameFormat");

            reportController.GenerateReport(report, reportOptions, MockProgressMonitor.Instance);

            reportService.AssertWasCalled(rs => rs.SaveReportAs(Arg.Is(report), Arg<string>.Is.Anything, Arg.Is("xml"),
                Arg<IProgressMonitor>.Is.Anything));
        }

        [Test]
        public void ReportTypes_Test()
        {
            var list = new List<string>(new[] {"test"});
            reportService.Stub(rs => rs.ReportTypes).Return(list);
            Assert.AreEqual(list, reportController.ReportTypes);
        }

        [Test]
        public void ShowReport_Test()
        {
            var report = new Report();
            const string reportType = "test";
            const string reportName = "reportName";
            reportService.Stub(rs => rs.SaveReportAs(Arg.Is(report), Arg<string>.Is.Anything, 
                Arg.Is(reportType), Arg<IProgressMonitor>.Is.Anything)).Return(reportName);
            
            Assert.AreEqual(reportName, reportController.ShowReport(report, reportType, 
                MockProgressMonitor.Instance));
        }

        [Test]
        public void ConvertSavedReport_should_call_same_on_ReportService()
        {
            const string fileName = "fileName";
            const string format = "format";
            var progressMonitor = MockProgressMonitor.Instance;

            reportController.ConvertSavedReport(fileName, format, progressMonitor);

            reportService.AssertWasCalled(rs => rs.ConvertSavedReport(fileName, format, progressMonitor));
        }
    }
}
