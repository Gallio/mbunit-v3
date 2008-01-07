using System.Collections.Specialized;
using Gallio.Core.ProgressMonitoring;
using Gallio.Runner.Reports;
using Gallio.Tests;
using MbUnit.Framework;

namespace Gallio.Plugin.Reports.Tests
{
    [TestFixture]
    [TestsOn(typeof(XmlReportFormatter))]
    public class XmlReportFormatterTest : BaseUnitTest
    {
        [Test, ExpectedArgumentNullException]
        public void NameCannotBeNull()
        {
            new XmlReportFormatter(null);
        }

        [Test]
        public void NameIsTheSameAsWasSpecifiedInTheConstructor()
        {
            XmlReportFormatter formatter = new XmlReportFormatter("SomeName");
            Assert.AreEqual("SomeName", formatter.Name);
        }

        [Test]
        public void TheDefaultAttachmentContentDispositionIsAbsent()
        {
            XmlReportFormatter formatter = new XmlReportFormatter("Xml");
            Assert.AreEqual(ExecutionLogAttachmentContentDisposition.Absent, formatter.DefaultAttachmentContentDisposition);
        }

        [Test]
        public void TheDefaultAttachmentContentDispositionCanBeChanged()
        {
            XmlReportFormatter formatter = new XmlReportFormatter("Xml");

            formatter.DefaultAttachmentContentDisposition = ExecutionLogAttachmentContentDisposition.Inline;
            Assert.AreEqual(ExecutionLogAttachmentContentDisposition.Inline, formatter.DefaultAttachmentContentDisposition);
        }

        [Test]
        public void FormatWritesTheReportWithTheDefaultAttachmentContentDispositionIfNoneSpecified()
        {
            IProgressMonitor progressMonitor = Mocks.Stub<IProgressMonitor>();
            IReportWriter writer = Mocks.CreateMock<IReportWriter>();

            using (Mocks.Record())
            {
                writer.SaveReport(ExecutionLogAttachmentContentDisposition.Absent, progressMonitor);
            }

            using (Mocks.Playback())
            {
                XmlReportFormatter formatter = new XmlReportFormatter("Xml");
                NameValueCollection options = new NameValueCollection();

                formatter.Format(writer, options, progressMonitor);
            }
        }

        [Test]
        public void FormatWritesTheReportWithTheSpecifiedAttachmentContentDisposition()
        {
            IProgressMonitor progressMonitor = Mocks.Stub<IProgressMonitor>();
            IReportWriter writer = Mocks.CreateMock<IReportWriter>();

            using (Mocks.Record())
            {
                writer.SaveReport(ExecutionLogAttachmentContentDisposition.Link, progressMonitor);
            }

            using (Mocks.Playback())
            {
                XmlReportFormatter formatter = new XmlReportFormatter("Xml");
                NameValueCollection options = new NameValueCollection();
                options.Add(XmlReportFormatter.AttachmentContentDispositionOption, ExecutionLogAttachmentContentDisposition.Link.ToString());

                formatter.Format(writer, options, progressMonitor);
            }
        }
    }
}
