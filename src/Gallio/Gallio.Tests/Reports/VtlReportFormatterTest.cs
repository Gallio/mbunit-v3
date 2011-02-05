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
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Xml;
using Gallio.Common.Markup;
using Gallio.Common.Policies;
using Gallio.Reports;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime;
using Gallio.Framework;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports;
using Gallio.Tests;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using Gallio.Common.Collections;
using Gallio.Runner.Reports.Schema;
using NVelocity.App;
using NVelocity.Runtime;
using System.Reflection;
using Gallio.Reports.Vtl;
using Gallio.Model.Schema;
using Commons.Collections;

namespace Gallio.Tests.Reports
{
    [TestFixture]
    [TestsOn(typeof(VtlReportFormatter))]
    public class VtlReportFormatterTest
    {
        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_extension_should_throw_exception()
        {
            new VtlReportFormatter(null, MimeTypes.PlainText, new DirectoryInfo("content"), "vm", EmptyArray<string>.Instance);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_contentType_should_throw_exception()
        {
            new VtlReportFormatter("ext", null, new DirectoryInfo("content"), "vm", EmptyArray<string>.Instance);
        }   

        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_resourceDirectory_should_throw_exception()
        {
            new VtlReportFormatter("ext", MimeTypes.PlainText, null, "vm", EmptyArray<string>.Instance);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_templatePath_should_throw_exception()
        {
            new VtlReportFormatter("ext", MimeTypes.PlainText, new DirectoryInfo("content"), null, EmptyArray<string>.Instance);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_resourcePaths_should_throw_exception()
        {
            new VtlReportFormatter("ext", MimeTypes.PlainText, new DirectoryInfo("content"), "vm", null);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_resource_containing_null_element_should_throw_exception()
        {
            new VtlReportFormatter("ext", MimeTypes.PlainText, new DirectoryInfo("content"), "vm", new string[] { null });
        }

        [Test, Explicit("Can't make it pass on the build server!")]
        public void Format()
        {
            var mockReportWriter = MockRepository.GenerateStub<IReportWriter>();
            var mockReportContainer = MockRepository.GenerateStub<IReportContainer>();
            var mockProgressMonitor = NullProgressMonitor.CreateInstance();
            var output = new StringBuilder();
            var stream = new StringStream(output);
            var fakeReport = new Report();
            mockReportWriter.Stub(x => x.Report).Return(fakeReport);
            mockReportWriter.Stub(x => x.ReportContainer).Return(mockReportContainer);
            mockReportContainer.Stub(x => x.ReportName).Return("output");
            mockReportContainer.Stub(x => x.OpenWrite(null, null, null)).IgnoreArguments().Return(stream);
            fakeReport.TestPackageRun = new TestPackageRun();
            fakeReport.TestPackageRun.RootTestStepRun = new TestStepRun(new TestStepData("", "", "", ""));
            fakeReport.TestPackageRun.Statistics.RunCount = 123;
            var formatter = new VtlReportFormatter("ext", MimeTypes.PlainText, new DirectoryInfo("content"), "Gallio.Tests.Reports.SampleTemplate.vm", EmptyArray<string>.Instance);
            formatter.VelocityEngineFactory = new ResourceVelocityEngineFactory();
            formatter.Format(mockReportWriter, new ReportFormatterOptions(), mockProgressMonitor);
            Assert.AreEqual("This is the test report (123)", output.ToString());
        }

        // Simple stream that stored the input UTF8 text into a string builder.
        private class StringStream : Stream 
        {
            private readonly StringBuilder builder;

            public StringStream(StringBuilder builder)
            {
                this.builder = builder;
            }

            #region Mandatory but not used

            public override bool CanRead
            {
                get { return false; }
            }

            public override bool CanSeek
            {
                get { return false; }
            }

            public override bool CanWrite
            {
                get { return true; }
            }

            public override void Flush()
            {
            }

            public override long Length
            {
                get { return builder.Length; }
            }

            public override long Position
            {
                get
                {
                    return builder.Length;
                }

                set
                {
                    throw new NotSupportedException();
                }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            #endregion

            public override void Write(byte[] buffer, int offset, int count)
            {
                string text = Encoding.UTF8.GetString(buffer, offset, count);
                builder.Append(text);
            }
        }

        private class ResourceVelocityEngineFactory : DefaultVelocityEngineFactory
        {
            public ResourceVelocityEngineFactory()
                : base(null)
            {
            }

            protected override void SetupVelocityEngine(ExtendedProperties properties)
            {
                properties.SetProperty("resource.loader", "assembly");
                properties.SetProperty("assembly.resource.loader.class", "NVelocity.Runtime.Resource.Loader.AssemblyResourceLoader");
                properties.SetProperty("assembly.resource.loader.assembly", Assembly.GetExecutingAssembly().GetName().Name);
            }
        }
    }
}