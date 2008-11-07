// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Threading;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Services.Interfaces;
using Gallio.Runner.Reports;
using Gallio.Runtime.ProgressMonitoring;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Controllers
{
    [Category("Controllers")]
    class ReportControllerTest : MockTest
    {
        [Test]
        public void GenerateReport_Test()
        {
            Report report = new Report();
            IReportService reportService = SetupReportService();
            Expect.Call(reportService.SaveReportAs(report, string.Empty, "xml", null)).Return(string.Empty);
            LastCall.IgnoreArguments();
            IProgressMonitor progressMonitor = mocks.StrictMock<IProgressMonitor>();
            mocks.ReplayAll();
            ReportController reportController = new ReportController(reportService);
            string reportDirectory = Path.Combine(Path.GetTempPath(), "GallioReport");
            reportController.GenerateReport(report, reportDirectory, progressMonitor);
            Thread.Sleep(100);
        }

        [Test]
        public void ReportTypes_Test()
        {
            IReportService reportService = SetupReportService();
            List<string> list = new List<string>(new[] {"test"});
            Expect.Call(reportService.ReportTypes).Return(list);
            mocks.ReplayAll();
            ReportController reportController = new ReportController(reportService);
            Assert.AreEqual(list, reportController.ReportTypes);
        }

        [Test]
        public void ShowReport_Test()
        {
            Report report = new Report();
            const string reportType = "test";
            IReportService reportService = SetupReportService();
            Expect.Call(reportService.SaveReportAs(report, string.Empty, reportType, null)).Return(string.Empty);
            LastCall.IgnoreArguments();
            IProgressMonitor progressMonitor = mocks.StrictMock<IProgressMonitor>();
            mocks.ReplayAll();
            ReportController reportController = new ReportController(reportService);
            reportController.ShowReport(report, reportType, progressMonitor);
            Thread.Sleep(500);
        }

        IReportService SetupReportService()
        {
            IReportService reportService = mocks.StrictMock<IReportService>();
            return reportService;
        }
    }
}
