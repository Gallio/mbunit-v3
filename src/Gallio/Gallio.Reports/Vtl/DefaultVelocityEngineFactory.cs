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

namespace Gallio.Reports.Vtl
{
    internal class DefaultVelocityEngineFactory : IVelocityEngineFactory
    {
        private readonly string templateDirectory;

        public DefaultVelocityEngineFactory(string templateDirectory)
        {
            this.templateDirectory = templateDirectory;
        }

        public VelocityEngine CreateVelocityEngine()
        {
            var engine = new VelocityEngine();
            SetupVelocityEngine(engine);
            engine.Init();
            return engine;
        }

        protected virtual void SetupVelocityEngine(VelocityEngine engine)
        {
            engine.SetProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, Path.GetDirectoryName(templateDirectory));
        }

        public VelocityContext CreateVelocityContext(IReportWriter reportWriter)
        {
            var context = new VelocityContext();
            context.Put("report", reportWriter.Report);
            context.Put("helper", new FormatHelper());
            var resourcesPath = RuntimeAccessor.Instance.ResourceLocator.ResolveResourcePath(new Uri("plugin://Gallio.Reports/Resources/"));
            context.Put("resourceRoot", reportWriter.ReportContainer.ReportName);
            context.Put("passed", TestStatus.Passed);
            context.Put("failed", TestStatus.Failed);
            context.Put("skipped", TestStatus.Skipped);
            context.Put("inconclusive", TestStatus.Inconclusive);
            return context;
        }
    }
}
