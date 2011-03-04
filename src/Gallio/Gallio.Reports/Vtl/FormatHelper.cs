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
    /// A general purpose class that helps in formatting stuff for the VTL template engine.
    /// </summary>
    internal class FormatHelper
    {
        private readonly FormatTextHelper text = new FormatTextHelper();
        private readonly FormatHtmlHelper html = new FormatHtmlHelper();
        private readonly FormatPagingHelper paging = new FormatPagingHelper();
        private readonly FormatContentsHelper contents = new FormatContentsHelper();

        /// <summary>
        /// Provides helper methods to ease text formating from VTL template engine.
        /// </summary>
        public FormatTextHelper Text
        { 
            get { return text; } 
        }

        /// <summary>
        /// Provides helper methods to ease HTML formating from VTL template engine.
        /// </summary>
        public FormatHtmlHelper Html
        {
            get { return html; }
        }

        /// <summary>
        /// Provides helper methods to ease HTML report paging from VTL template engine.
        /// </summary>
        public FormatPagingHelper Paging
        {
            get { return paging; }
        }

        /// <summary>
        /// Provides helper methods to sort and prepare the contents of the report.
        /// </summary>
        public FormatContentsHelper Contents
        {
            get { return contents; }
        }
    }
}
