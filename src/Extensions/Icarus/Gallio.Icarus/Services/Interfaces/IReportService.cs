using System.Collections.Generic;
using Gallio.Runner.Reports;

namespace Gallio.Icarus.Services.Interfaces
{
    interface IReportService
    {
        IList<string> ReportTypes { get; }

        void GenerateReport(Report report, string reportFolder);
        void SaveReportAs(Report report, string fileName, string format);
    }
}
