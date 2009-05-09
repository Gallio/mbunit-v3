// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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

using System.Collections.Specialized;
using Gallio.Common.Markup;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runner.Reports;
using Gallio.Tests;
using MbUnit.Framework;

namespace Gallio.Reports.Tests
{
    [TestFixture]
    [TestsOn(typeof(XmlReportFormatter))]
    public class XmlReportFormatterTest : BaseTestWithMocks
    {
        [Test]
        public void TheDefaultAttachmentContentDispositionIsAbsent()
        {
            XmlReportFormatter formatter = new XmlReportFormatter();
            Assert.AreEqual(AttachmentContentDisposition.Absent, formatter.DefaultAttachmentContentDisposition);
        }

        [Test]
        public void TheDefaultAttachmentContentDispositionCanBeChanged()
        {
            XmlReportFormatter formatter = new XmlReportFormatter();

            formatter.DefaultAttachmentContentDisposition = AttachmentContentDisposition.Inline;
            Assert.AreEqual(AttachmentContentDisposition.Inline, formatter.DefaultAttachmentContentDisposition);
        }

        [Test]
        public void FormatWritesTheReportWithTheDefaultAttachmentContentDispositionIfNoneSpecified()
        {
            IProgressMonitor progressMonitor = Mocks.Stub<IProgressMonitor>();
            IReportWriter writer = Mocks.StrictMock<IReportWriter>();

            using (Mocks.Record())
            {
                writer.SaveReport(AttachmentContentDisposition.Absent, progressMonitor);
            }

            using (Mocks.Playback())
            {
                XmlReportFormatter formatter = new XmlReportFormatter();
                var reportFormatterOptions = new ReportFormatterOptions();

                formatter.Format(writer, reportFormatterOptions, progressMonitor);
            }
        }

        [Test]
        public void FormatWritesTheReportWithTheSpecifiedAttachmentContentDisposition()
        {
            IProgressMonitor progressMonitor = Mocks.Stub<IProgressMonitor>();
            IReportWriter writer = Mocks.StrictMock<IReportWriter>();

            using (Mocks.Record())
            {
                writer.SaveReport(AttachmentContentDisposition.Link, progressMonitor);
            }

            using (Mocks.Playback())
            {
                XmlReportFormatter formatter = new XmlReportFormatter();
                ReportFormatterOptions options = new ReportFormatterOptions();
                options.Properties.Add(XmlReportFormatter.AttachmentContentDispositionOption, AttachmentContentDisposition.Link.ToString());

                formatter.Format(writer, options, progressMonitor);
            }
        }
    }
}
