// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.IO;
using System.Text;
using System.Xml;

using Gallio.Hosting.ProgressMonitoring;
using Gallio.Runner.Reports;
using System.Xml.Serialization;
using Gallio.Model.Serialization;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Reflection;

namespace Gallio.Icarus.Core.Reports
{
    public class TestStepReportWriter : DefaultReportWriter
    {
        private readonly TestStepRun testStepRun;

        public TestStepReportWriter(Report report, TestStepRun testStepRun, IReportContainer reportContainer)
            : base(report, reportContainer)
        {
            this.testStepRun = testStepRun;
        }

        public override void SerializeReport(XmlWriter xmlWriter, ExecutionLogAttachmentContentDisposition attachmentContentDisposition)
        {
            if (xmlWriter == null)
                throw new ArgumentNullException(@"xmlWriter");

            XmlAttributes ignoreAttributes = new XmlAttributes();
            ignoreAttributes.XmlIgnore = true;
            XmlAttributeOverrides overrides = new XmlAttributeOverrides();

            // Prune unnecessary ids that can be determined implicitly from the report structure.
            overrides.Add(typeof(TestStepData), @"ParentId", ignoreAttributes);
            overrides.Add(typeof(ExecutionLogAttachment), @"ContentPath", ignoreAttributes);

            // Serialize the step.
            XmlSerializer serializer = new XmlSerializer(typeof(TestStepRun), overrides);
            serializer.Serialize(xmlWriter, testStepRun);
        }
    }
}
