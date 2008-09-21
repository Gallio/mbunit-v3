using System.Collections.Generic;
using System.IO;
using System.Threading;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.ProgressMonitoring.EventArgs;
using Gallio.Icarus.Services.Interfaces;
using Gallio.Runner.Reports;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Gallio.Icarus.Tests.Controllers
{
    [Category("Controllers")]
    class ReportControllerTest : MockTest
    {
        private IEventRaiser progressUpdate;

        [Test]
        public void GenerateReport_Test()
        {
            Report report = new Report();
            IReportService reportService = SetupReportService();
            Expect.Call(reportService.SaveReportAs(report, string.Empty, "xml")).Return(string.Empty);
            LastCall.IgnoreArguments();
            mocks.ReplayAll();
            ReportController reportController = new ReportController(reportService);
            string reportDirectory = Path.Combine(Path.GetTempPath(), "GallioReport");
            reportController.GenerateReport(report, reportDirectory);
            Thread.Sleep(100);
        }

        [Test]
        public void ProgressUpdate_Test()
        {
            IReportService reportService = SetupReportService();
            mocks.ReplayAll();
            ReportController reportController = new ReportController(reportService);
            bool flag = false;
            reportController.ProgressUpdate += delegate { flag = true; };
            Assert.IsFalse(flag);
            progressUpdate.Raise(reportController, new ProgressUpdateEventArgs("taskName", "subTaskName", 0, 0));
            Assert.IsTrue(flag);
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
            Expect.Call(reportService.SaveReportAs(report, string.Empty, reportType)).Return(string.Empty);
            LastCall.IgnoreArguments();
            mocks.ReplayAll();
            ReportController reportController = new ReportController(reportService);
            reportController.ShowReport(report, reportType);
            Thread.Sleep(100);
        }

        IReportService SetupReportService()
        {
            IReportService reportService = mocks.CreateMock<IReportService>();
            reportService.ProgressUpdate += null;
            progressUpdate = LastCall.IgnoreArguments().GetEventRaiser();
            return reportService;
        }
    }
}
