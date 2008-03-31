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
