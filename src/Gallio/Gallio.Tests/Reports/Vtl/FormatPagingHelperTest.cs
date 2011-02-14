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
using System.IO;
using System.Text;
using System.Xml;
using Gallio.Common.Markup;
using Gallio.Common.Policies;
using Gallio.Reports;
using Gallio.Reports.Vtl;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime;
using Gallio.Framework;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports;
using Gallio.Tests;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using Gallio.Common.Collections;
using Gallio.Runner.Reports.Schema;
using NVelocity.App;
using NVelocity.Runtime;
using System.Reflection;
using Gallio.Common.Markup.Tags;
using Gallio.Model.Schema;

namespace Gallio.Tests.Reports.Vtl
{
    [TestFixture]
    [TestsOn(typeof(FormatPagingHelper))]
    public class FormatPagingHelperTest
    {
        [Test]
        [Row(0, @"C:\Path\ReportName.ext")]
        [Row(1, @"C:\Path\ReportName.Page1.ext")]
        [Row(123, @"C:\Path\ReportName.Page123.ext")]
        public void GetReportPath(int pageIndex, string expectedPath)
        {
            var helper = new FormatPagingHelper
            {
                ReportName = @"C:\Path\ReportName",
                Extension = "ext",
            };

            string actualPath = helper.GetReportPath(pageIndex);
            Assert.AreEqual(expectedPath, actualPath);
        }

        [Test]
        [Row(0, 1000, 1)]
        [Row(123, 1000, 1)]
        [Row(1234, 1000, 2)]
        [Row(12345, 1000, 13)]
        public void GetPageOf(int index, int pageSize, int expectedPage)
        {
            var helper = new FormatPagingHelper();
            int actualPage = helper.GetPageOf(index, pageSize);
            Assert.AreEqual(expectedPage, actualPage);
        }
    }
}