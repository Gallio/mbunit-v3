using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Services.Interfaces;
using Gallio.Runner.Reports;

namespace Gallio.Icarus.Controllers
{
    class ReportController : IReportController
    {
        private readonly IReportService reportService;

        public ReportController(IReportService reportService)
        {
            this.reportService = reportService;    
        }

        public void GenerateReport(Report report, string reportDirectory)
        {
            reportService.GenerateReport(report, reportDirectory);
        }
    }
}
