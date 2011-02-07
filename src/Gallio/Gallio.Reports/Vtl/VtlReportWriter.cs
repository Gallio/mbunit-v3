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

namespace Gallio.Reports.Vtl
{
    /// <summary>
    /// 
    /// </summary>
    internal abstract class VtlReportWriter
    {
        private readonly VelocityEngine velocityEngine;
        private readonly VelocityContext velocityContext;
        private readonly IReportWriter reportWriter;
        private readonly string templatePath;
        private readonly string contentType;
        private readonly string extension;
        private readonly FormatHelper helper;

        /// <summary>
        /// 
        /// </summary>
        protected VelocityEngine VelocityEngine
        {
            get { return velocityEngine;  }
        }

        /// <summary>
        /// 
        /// </summary>
        protected VelocityContext VelocityContext
        {
            get { return velocityContext; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected IReportWriter ReportWriter
        {
            get { return reportWriter; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string TemplatePath
        {
            get { return templatePath; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string ContentType
        {
            get { return contentType; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string Extension
        {
            get { return extension; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected FormatHelper Helper
        {
            get { return helper; }
        }

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
        protected VtlReportWriter(VelocityEngine velocityEngine, VelocityContext velocityContext, IReportWriter reportWriter, 
            string templatePath, string contentType, string extension, FormatHelper helper)
        {
            if (velocityEngine == null)
                throw new ArgumentNullException("velocityEngine");
            if (velocityContext == null)
                throw new ArgumentNullException("velocityContext");
            if (reportWriter == null)
                throw new ArgumentNullException("reportWriter");
            if (templatePath == null)
                throw new ArgumentNullException("templatePath");
            if (contentType == null)
                throw new ArgumentNullException("contentType");
            if (extension == null)
                throw new ArgumentNullException("extension");
            if (helper == null)
                throw new ArgumentNullException("helper");

            this.velocityEngine = velocityEngine;
            this.velocityContext = velocityContext;
            this.reportWriter = reportWriter;
            this.templatePath = templatePath;
            this.contentType = contentType;
            this.extension = extension;
            this.helper = helper;
            InitializeHelper();
        }

        private void InitializeHelper()
        {
            Helper.Paging.Extension = extension;
            Helper.Paging.ReportName = reportWriter.ReportContainer.ReportName;
        }

        /// <summary>
        /// 
        /// </summary>
        public abstract void Run();
    }
}
