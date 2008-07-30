// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Xml;
using Gallio.Model.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime;
using Gallio.Framework;
using Gallio.Reflection;
using Gallio.Runner.Reports;
using Gallio.Tests;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace Gallio.Reports.Tests
{
    [TestFixture]
    [TestsOn(typeof(XsltReportFormatter))]
    public class XsltReportFormatterTest : BaseUnitTest
    {
        private delegate void SerializeReportDelegate(XmlWriter writer, TestLogAttachmentContentDisposition contentDisposition);

        [Test, ExpectedArgumentNullException]
        public void RuntimeCannotBeNull()
        {
            new XsltReportFormatter(null, "SomeName", "description", "ext", MimeTypes.PlainText, "file://content", "xslt", new string[0]);
        }

        [Test, ExpectedArgumentNullException]
        public void NameCannotBeNull()
        {
            new XsltReportFormatter(Mocks.Stub<IRuntime>(), null, "description", "ext", MimeTypes.PlainText, "file://content", "xslt", new string[0]);
        }

        [Test, ExpectedArgumentNullException]
        public void DescriptionCannotBeNull()
        {
            new XsltReportFormatter(Mocks.Stub<IRuntime>(), "name", null, "ext", MimeTypes.PlainText, "file://content", "xslt", new string[0]);
        }

        [Test, ExpectedArgumentNullException]
        public void ContentTypeCannotBeNull()
        {
            new XsltReportFormatter(Mocks.Stub<IRuntime>(), "name", "description", "ext", null, "file://content", "xslt", new string[0]);
        }

        [Test, ExpectedArgumentNullException]
        public void ExtensionCannotBeNull()
        {
            new XsltReportFormatter(Mocks.Stub<IRuntime>(), "SomeName", "description", null, MimeTypes.PlainText, "file://content", "xslt", new string[0]);
        }

        [Test, ExpectedArgumentNullException]
        public void ContentPathCannotBeNull()
        {
            new XsltReportFormatter(Mocks.Stub<IRuntime>(), "SomeName", "description", "ext", MimeTypes.PlainText, null, "xslt", new string[0]);
        }

        [Test, ExpectedArgumentNullException]
        public void XsltPathCannotBeNull()
        {
            new XsltReportFormatter(Mocks.Stub<IRuntime>(), "SomeName", "description", "ext", MimeTypes.PlainText, "file://content", null, new string[0]);
        }

        [Test, ExpectedArgumentNullException]
        public void ResourcePathsCannotBeNull()
        {
            new XsltReportFormatter(Mocks.Stub<IRuntime>(), "SomeName", "description", "ext", MimeTypes.PlainText, "file://content", "xslt", null);
        }

        [Test, ExpectedArgumentNullException]
        public void ResourcePathsCannotContainNulls()
        {
            new XsltReportFormatter(Mocks.Stub<IRuntime>(), "SomeName", "description", "ext", MimeTypes.PlainText, "file://content", "xslt", new string[] { null });
        }

        [Test]
        public void NameIsTheSameAsWasSpecifiedInTheConstructor()
        {
            XsltReportFormatter formatter = new XsltReportFormatter(Mocks.Stub<IRuntime>(), "SomeName", "description", "ext", MimeTypes.PlainText, "file://content", "xslt", new string[] { "res1", "res2" });
            Assert.AreEqual("SomeName", formatter.Name);
        }

        [Test]
        public void DescriptionIsTheSameAsWasSpecifiedInTheConstructor()
        {
            XsltReportFormatter formatter = new XsltReportFormatter(Mocks.Stub<IRuntime>(), "SomeName", "description", "ext", MimeTypes.PlainText, "file://content", "xslt", new string[] { "res1", "res2" });
            Assert.AreEqual("description", formatter.Description);
        }

        [Test]
        public void TheDefaultAttachmentContentDispositionIsAbsent()
        {
            XsltReportFormatter formatter = new XsltReportFormatter(Mocks.Stub<IRuntime>(), "SomeName", "description", "ext", MimeTypes.PlainText, "file://content", "xslt", new string[] { "res1", "res2" });
            Assert.AreEqual(TestLogAttachmentContentDisposition.Absent, formatter.DefaultAttachmentContentDisposition);
        }

        [Test]
        public void TheDefaultAttachmentContentDispositionCanBeChanged()
        {
            XsltReportFormatter formatter = new XsltReportFormatter(Mocks.Stub<IRuntime>(), "SomeName", "description", "ext", MimeTypes.PlainText, "file://content", "xslt", new string[] { "res1", "res2" });

            formatter.DefaultAttachmentContentDisposition = TestLogAttachmentContentDisposition.Inline;
            Assert.AreEqual(TestLogAttachmentContentDisposition.Inline, formatter.DefaultAttachmentContentDisposition);
        }

        [Test]
        public void FormatWritesTheTransformedReport()
        {
            string resourcePath = Path.Combine(Path.GetDirectoryName(AssemblyUtils.GetAssemblyLocalPath(GetType().Assembly)), @"..\Resources");

            IRuntime runtime = Mocks.CreateMock<IRuntime>();
            IReportWriter reportWriter = Mocks.CreateMock<IReportWriter>();
            IReportContainer reportContainer = Mocks.CreateMock<IReportContainer>();
            IProgressMonitor progressMonitor = NullProgressMonitor.CreateInstance();

            string reportPath = Path.GetTempFileName();
            using (Stream tempFileStream = File.OpenWrite(reportPath))
            {
                using (Mocks.Record())
                {
                    SetupResult.For(reportWriter.ReportContainer).Return(reportContainer);

                    Expect.Call(runtime.MapUriToLocalPath(null))
                        .Constraints(Is.Equal(new Uri("file://content")))
                        .Return(resourcePath);

                    reportWriter.SerializeReport(null, TestLogAttachmentContentDisposition.Link);
                    LastCall.Constraints(Is.NotNull(), Is.Equal(TestLogAttachmentContentDisposition.Link))
                        .Do((SerializeReportDelegate)delegate(XmlWriter writer, TestLogAttachmentContentDisposition contentDisposition)
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.InnerXml = "<report>The report.</report>";
                            doc.Save(writer);
                        });

                    SetupResult.For(reportContainer.ReportName).Return("Foo");
                    Expect.Call(reportContainer.OpenWrite("Foo.ext", MimeTypes.PlainText, Encoding.UTF8))
                        .Return(tempFileStream);
                    reportWriter.AddReportDocumentPath("Foo.ext");

                    Expect.Call(reportContainer.OpenWrite(@"Foo\MbUnitLogo.png", MimeTypes.Png, null)).Return(new MemoryStream());

                    reportWriter.SaveReportAttachments(null);
                    LastCall.Constraints(Is.NotNull());
                }

                using (Mocks.Playback())
                {
                    XsltReportFormatter formatter = new XsltReportFormatter(runtime, "SomeName", "description", "ext", MimeTypes.PlainText, "file://content", "Diagnostic.xslt", new string[] { "MbUnitLogo.png" });
                    NameValueCollection options = new NameValueCollection();
                    options.Add(XsltReportFormatter.AttachmentContentDispositionOption, TestLogAttachmentContentDisposition.Link.ToString());

                    formatter.Format(reportWriter, options, progressMonitor);

                    string reportContents = File.ReadAllText(reportPath);
                    Log.EmbedXml("Diagnostic report contents", reportContents);
                    Assert.Contains(reportContents, "<resourceRoot>Foo</resourceRoot>");
                    Assert.Contains(reportContents, "The report.");

                    File.Delete(reportPath);
                }
            }
        }
    }
}