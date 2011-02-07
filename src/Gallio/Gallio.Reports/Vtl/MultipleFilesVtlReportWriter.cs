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
    /// 
    /// </summary>
    internal class MultipleFilesVtlReportWriter : VtlReportWriter
    {
        private readonly int size;
        private readonly Encoding encoding = new UTF8Encoding(false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="velocityEngine"></param>
        /// <param name="velocityContext"></param>
        /// <param name="reportWriter"></param>
        /// <param name="templatePath"></param>
        /// <param name="contentType"></param>
        /// <param name="extension"></param>
        /// <param name="helper"></param>
        /// <param name="size"></param>
        public MultipleFilesVtlReportWriter(VelocityEngine velocityEngine, VelocityContext velocityContext, IReportWriter reportWriter, string templatePath, string contentType, string extension, FormatHelper helper, int size)
            : base(velocityEngine, velocityContext, reportWriter, templatePath, contentType, extension, helper)
        {
            if (size <= 0)
                throw new ArgumentOutOfRangeException("size", "Must be greater than zero.");

            this.size = size;
        }

        /// <inheritdoc />
        public override void Run()
        {
            int pageCount = GetPageCount();
            VelocityContext.Put("pagingEnabled", true);
            VelocityContext.Put("pageSize", size);
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
            return (n / size) + ((n % size) != 0 ? 1 : 0);
        }
    }
}
