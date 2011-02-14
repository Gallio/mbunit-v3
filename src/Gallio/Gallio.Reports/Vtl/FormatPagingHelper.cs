// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Text;
using NVelocity;
using NVelocity.App;
using Gallio.Runner.Reports;
using System.Text.RegularExpressions;
using Gallio.Model;
using Gallio.Runner.Reports.Schema;
using Gallio.Common.Collections;
using Gallio.Common.Markup.Tags;
using Gallio.Common.Markup;

namespace Gallio.Reports.Vtl
{
    /// <summary>
    /// Provides helper methods to ease HTML report paging from VTL template engine.
    /// </summary>
    internal class FormatPagingHelper
    {
        /// <summary>
        /// Gets or sets the report name.
        /// </summary>
        public string ReportName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the report file extension.
        /// </summary>
        public string Extension
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the name of paged report file.
        /// </summary>
        /// <param name="pageIndex">The index of the page.</param>
        /// <returns>The name of the paged report file.</returns>
        public string GetReportPath(int pageIndex)
        {
            var output = new StringBuilder(ReportName);

            if (pageIndex > 0)
                output.Append(".Page" + pageIndex);

            output.Append("." + Extension);
            return output.ToString();
        }

        /// <summary>
        /// Returns the page index of the test step at the specified index.
        /// </summary>
        /// <param name="index">The index of the test step.</param>
        /// <param name="pageSize">The sise of a page.</param>
        /// <returns>The page index.</returns>
        public int GetPageOf(int index, int pageSize)
        {
            return 1 + (index / pageSize);
        }
    }
}
