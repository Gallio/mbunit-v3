using System;
using System.Collections.Generic;
using System.Text;
using Castle.Core;
using MbUnit.Framework.Kernel.Events;

namespace MbUnit.Core.Reporting
{
    /// <summary>
    /// The default implementation of <see cref="IReportManager" />.
    /// </summary>
    [Singleton]
    public class DefaultReportManager : IReportManager
    {
        /// <inheritdoc />
        public IList<string> GetFormatterNames()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IReportFormatter GetFormatter(string name)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IList<string> Format(string formatterName, Report report, string outputFilename,
                                    Dictionary<string, string> options, IProgressMonitor progressMonitor)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void SaveReport(Report report, string filename, IProgressMonitor progressMonitor)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Report LoadReport(string filename, IProgressMonitor progressMonitor)
        {
            throw new NotImplementedException();
        }
    }
}
