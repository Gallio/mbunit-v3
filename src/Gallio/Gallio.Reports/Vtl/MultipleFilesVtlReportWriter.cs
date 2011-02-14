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
using NVelocity.App;
using NVelocity;
using Gallio.Runner.Reports;
using System.IO;

namespace Gallio.Reports.Vtl
{
    /// <summary>
    /// VTL engine-based report writer for multiple pages document.
    /// </summary>
    internal class MultipleFilesVtlReportWriter : VtlReportWriter
    {
        private readonly int pageSize;
        private readonly Encoding encoding = new UTF8Encoding(false);

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="velocityEngine">The velocity engine</param>
        /// <param name="velocityContext">The current velocity context.</param>
        /// <param name="reportWriter">The report writer</param>
        /// <param name="templatePath">The template path.</param>
        /// <param name="contentType">The content type of the report.</param>
        /// <param name="extension">The extension of the report file.</param>
        /// <param name="helper">The formatting helper class.</param>
        /// <param name="pageSize">The number of test steps displayed in one page.</param>
        public MultipleFilesVtlReportWriter(VelocityEngine velocityEngine, VelocityContext velocityContext, IReportWriter reportWriter, string templatePath, string contentType, string extension, FormatHelper helper, int pageSize)
            : base(velocityEngine, velocityContext, reportWriter, templatePath, contentType, extension, helper)
        {
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("size", "Must be greater than zero.");

            this.pageSize = pageSize;
        }

        /// <inheritdoc />
        public override void Run()
        {
            int pageCount = GetPageCount();
            VelocityContext.Put("pagingEnabled", true);
            VelocityContext.Put("pageSize", pageSize);
            VelocityContext.Put("pageCount", pageCount);
            Template template = VelocityEngine.GetTemplate(Path.GetFileName(TemplatePath), encoding.BodyName);

            for (int i = 0; i <= pageCount; i++)
            {
                GeneratePage(template, i);
            }
        }

        private void GeneratePage(Template template, int pageIndex)
        {
            VelocityContext.Put("pageIndex", pageIndex);
            string reportPath = Helper.Paging.GetReportPath(pageIndex);
            var stringBuilder = new StringBuilder();

            using (var stringWriter = new StringWriter(stringBuilder))
            {
                template.Merge(VelocityContext, stringWriter);

                using (var fileWriter = new StreamWriter(ReportWriter.ReportContainer.OpenWrite(reportPath, ContentType, encoding)))
                {
                    fileWriter.Write(FormatHtmlHelper.Flatten(stringBuilder.ToString()));
                }
            }

            ReportWriter.AddReportDocumentPath(reportPath);
        }

        private int GetPageCount()
        {
            int n = ReportWriter.Report.TestPackageRun.Statistics.TestCount;
            return (n / pageSize) + ((n % pageSize) != 0 ? 1 : 0);
        }
    }
}
