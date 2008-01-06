// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Core.ProgressMonitoring;
using Gallio.Hosting;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// The default implementation of <see cref="IReportManager" />.
    /// </summary>
    [Singleton]
    public class DefaultReportManager : IReportManager
    {
        private readonly IRuntime runtime;

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
        public void Format(IReportWriter reportWriter, string formatterName, NameValueCollection formatterOptions,
            IProgressMonitor progressMonitor)
        {
            if (reportWriter == null)
                throw new ArgumentNullException(@"reportWriter");
            if (formatterName == null)
                throw new ArgumentNullException(@"formatterName");
            if (formatterOptions == null)
                throw new ArgumentNullException(@"formatterOptions");
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");

            IReportFormatter formatter = GetFormatter(formatterName);
            if (formatter == null)
                throw new InvalidOperationException(String.Format("There is no report formatter named '{0}'.", formatterName));

            formatter.Format(reportWriter, formatterOptions, progressMonitor);
        }

        /// <inheritdoc />
        public IReportReader CreateReportReader(IReportContainer reportContainer)
        {
            return new DefaultReportReader(reportContainer);
        }

        /// <inheritdoc />
        public IReportWriter CreateReportWriter(Report report, IReportContainer reportContainer)
        {
            return new DefaultReportWriter(report, reportContainer);
        }
    }
}
