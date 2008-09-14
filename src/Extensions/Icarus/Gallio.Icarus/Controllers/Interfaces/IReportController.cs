using Gallio.Runner.Reports;

namespace Gallio.Icarus.Controllers.Interfaces
{
    public interface IReportController
    {
        void GenerateReport(Report report, string reportDirectory);
    }
}
