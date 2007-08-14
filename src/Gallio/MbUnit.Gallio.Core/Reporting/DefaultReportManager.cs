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
