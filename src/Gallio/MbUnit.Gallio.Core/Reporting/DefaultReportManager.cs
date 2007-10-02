// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Castle.Core;
using MbUnit.Core.ProgressMonitoring;
using MbUnit.Framework.Kernel.RuntimeSupport;

namespace MbUnit.Core.Reporting
{
    /// <summary>
    /// The default implementation of <see cref="IReportManager" />.
    /// </summary>
    [Singleton]
    public class DefaultReportManager : IReportManager
    {
        private IRuntime runtime;

        /// <summary>
        /// Creates a report manager.
        /// </summary>
        /// <param name="runtime">The runtime</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="runtime"/> is null</exception>
        public DefaultReportManager(IRuntime runtime)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");

            this.runtime = runtime;
        }

        /// <inheritdoc />
        public IList<string> GetFormatterNames()
        {
            List<string> names = new List<string>();
            foreach (IReportFormatter formatter in runtime.ResolveAll<IReportFormatter>())
            {
                names.Add(formatter.Name);
            }

            return names;
        }

        /// <inheritdoc />
        public IReportFormatter GetFormatter(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            foreach (IReportFormatter formatter in runtime.ResolveAll<IReportFormatter>())
            {
                if (String.Equals(name, formatter.Name, StringComparison.CurrentCultureIgnoreCase))
                    return formatter;
            }

            return null;
        }

        /// <inheritdoc />
        public void Format(string formatterName, Report report, string reportPath,
            NameValueCollection options, IList<string> filesWritten, IProgressMonitor progressMonitor)
        {
            if (formatterName == null)
                throw new ArgumentNullException("formatterName");
            if (report == null)
                throw new ArgumentNullException("report");
            if (reportPath == null)
                throw new ArgumentNullException("reportPath");
            if (options == null)
                throw new ArgumentNullException("options");
            if (filesWritten == null)
                throw new ArgumentNullException("filesWritten");
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            IReportFormatter formatter = GetFormatter(formatterName);
            formatter.Format(report, reportPath, options, filesWritten, progressMonitor);
        }

        /// <inheritdoc />
        public void SaveReport(Report report, string reportPath, IProgressMonitor progressMonitor)
        {
            ReportUtils.SaveReport(report, reportPath, true, false, null, progressMonitor);
        }

        /// <inheritdoc />
        public Report LoadReport(string reportPath, IProgressMonitor progressMonitor)
        {
            return ReportUtils.LoadReport(reportPath, true, progressMonitor);
        }
    }
}
