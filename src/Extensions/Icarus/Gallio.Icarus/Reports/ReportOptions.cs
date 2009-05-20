namespace Gallio.Icarus.Reports
{
    public class ReportOptions
    {
        public string ReportDirectory
        {
            get;
            private set;
        }

        public string ReportNameFormat
        {
            get;
            private set;
        }

        public ReportOptions(string reportDirectory, string reportNameFormat)
        {
            ReportDirectory = reportDirectory;
            ReportNameFormat = reportNameFormat;
        }
    }
}
