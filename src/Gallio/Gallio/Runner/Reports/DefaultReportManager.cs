// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Gallio.Common.Collections;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// The default implementation of <see cref="IReportManager" />.
    /// </summary>
    public class DefaultReportManager : IReportManager
    {
        private readonly IList<ComponentHandle<IReportFormatter, ReportFormatterTraits>> formatterHandles;

        /// <summary>
        /// Creates a report manager.
        /// </summary>
        /// <param name="formatterHandles">The report formatter handles.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatterHandles"/> is null.</exception>
        public DefaultReportManager(ComponentHandle<IReportFormatter, ReportFormatterTraits>[] formatterHandles)
        {
            if (formatterHandles == null)
                throw new ArgumentNullException("formatterResolver");

            this.formatterHandles = formatterHandles;
        }

        /// <inheritdoc />
        public IList<ComponentHandle<IReportFormatter, ReportFormatterTraits>> FormatterHandles
        {
            get { return new ReadOnlyCollection<ComponentHandle<IReportFormatter, ReportFormatterTraits>>(formatterHandles); }
        }

        /// <inheritdoc />
        public IReportFormatter GetReportFormatter(string formatterName)
        {
            if (formatterName == null)
                throw new ArgumentNullException("name");

            ComponentHandle<IReportFormatter, ReportFormatterTraits> handle
                = GenericCollectionUtils.Find(formatterHandles, h => string.Compare(h.GetTraits().Name, formatterName, true) == 0);
            return handle != null ? handle.GetComponent() : null;
        }

        /// <inheritdoc />
        public void Format(IReportWriter reportWriter, string formatterName, ReportFormatterOptions formatterOptions,
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

            IReportFormatter formatter = GetReportFormatter(formatterName);
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
