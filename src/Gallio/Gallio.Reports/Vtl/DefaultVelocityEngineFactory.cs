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
using NVelocity.Runtime;
using Gallio.Runtime;
using Gallio.Model;
using System.IO;
using Gallio.Runner.Reports.Schema;
using Commons.Collections;

namespace Gallio.Reports.Vtl
{
    /// <inheritdoc />
    internal class DefaultVelocityEngineFactory : IVelocityEngineFactory
    {
        private readonly string templateDirectory;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="templateDirectory">The directory where the VTL templates files (.vm) are located.</param>
        public DefaultVelocityEngineFactory(string templateDirectory)
        {
            this.templateDirectory = templateDirectory;
        }

        /// <inheritdoc />
        public VelocityEngine CreateVelocityEngine()
        {
            var properties = new ExtendedProperties();
            SetupVelocityEngine(properties);
            var engine = new VelocityEngine(properties);
            engine.Init();
            return engine;
        }

        /// <summary>
        /// Sets internal properties of the Velocity engine.
        /// </summary>
        /// <param name="properties">The velocity engine extended properties.</param>
        protected virtual void SetupVelocityEngine(ExtendedProperties properties)
        {
            properties.AddProperty("file.resource.loader.path", Path.GetDirectoryName(templateDirectory));
        }

        /// <inheritdoc />
        public VelocityContext CreateVelocityContext(IReportWriter reportWriter)
        {
            var context = new VelocityContext();
            context.Put("report", reportWriter.Report);
            var helper = new FormatHelper();
            helper.BuildParentMap(reportWriter.Report.TestPackageRun.RootTestStepRun);
            context.Put("helper", helper);
            context.Put("resourceRoot", reportWriter.ReportContainer.ReportName);
            context.Put("passed", TestStatus.Passed);
            context.Put("failed", TestStatus.Failed);
            context.Put("skipped", TestStatus.Skipped);
            context.Put("inconclusive", TestStatus.Inconclusive);
            return context;
        }
    }
}
