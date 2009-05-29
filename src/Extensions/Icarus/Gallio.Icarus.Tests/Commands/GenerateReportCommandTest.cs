using System;
using Gallio.Common.Concurrency;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Reports;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Runner.Reports;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [Category("Commands"), TestsOn(typeof(GenerateReportCommand))]
    internal class GenerateReportCommandTest
    {
        [Test]
        public void Execute_should_call_GenerateReport_on_ReportController()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            var report = new Report();
            testController.Stub(tc => tc.ReadReport(null)).IgnoreArguments().Do(
                (Action<ReadAction<Report>>)(action => action(report)));
            var reportController = MockRepository.GenerateStub<IReportController>();
            var reportOptions = new ReportOptions("", "");
            var generateReportCommand = new GenerateReportCommand(testController, reportController)
                                            { ReportOptions = reportOptions };
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            
            generateReportCommand.Execute(progressMonitor);

            reportController.AssertWasCalled(rc => rc.GenerateReport(report, reportOptions, 
                progressMonitor));
        }

        [Test]
        public void ReportOptions_should_return_set_value()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            var reportController = MockRepository.GenerateStub<IReportController>();
            var reportOptions = new ReportOptions("", "");
            var generateReportCommand = new GenerateReportCommand(testController, reportController) 
                { ReportOptions = reportOptions };

            Assert.AreEqual(reportOptions, generateReportCommand.ReportOptions);
        }
    }
}
