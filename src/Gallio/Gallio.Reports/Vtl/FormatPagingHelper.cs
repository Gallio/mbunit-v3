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
        /// 
        /// </summary>
        public string ReportName
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Extension
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public string GetReportPath(int pageIndex)
        {
            var output = new StringBuilder(ReportName);

            if (pageIndex > 0)
                output.Append(".Page" + pageIndex);

            output.Append("." + Extension);
            return output.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public bool IsVisible(int index, int pageIndex, int pageSize)
        {
            if (pageIndex == 0)
                return true;

            int start = (pageIndex - 1) * pageSize;
            return (index >= start) && (index < start + pageSize);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public int GetPageOf(int index, int pageSize)
        {
            return 1 + (index / pageSize);
        }
    }
}
