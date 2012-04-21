// Copyright 2012 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.IO;
using Gallio.Common;
using Gallio.Runner.Reports;
using MbUnit.Framework;
using NHamcrest.Core;
using Rhino.Mocks;

namespace Gallio.Tests.Runner.Reports
{
    public class AttachmentPathResolverTests
    {
        private IReportContainer reportContainer;
        private AttachmentPathResolver attachmentPathResolver;

        [SetUp]
        public void SetUp()
        {
            reportContainer = MockRepository.GenerateStub<IReportContainer>();
            reportContainer.Stub(rc => rc.EncodeFileName(Arg<string>.Is.Anything)).Do(new Func<string, string>(s => s));
            attachmentPathResolver = new AttachmentPathResolver(reportContainer);
        }

        [Test]
        public void Attachment_folder_is_report_path_then_step_id()
        {
            const string reportPath = @"C:\Reports\Report1";
            reportContainer.Stub(rc => rc.ReportName).Return(reportPath);
            const string testStepId = "testStepId";

            var attachmentPath = attachmentPathResolver.GetAttachmentPath(testStepId, "attachmentName", "");

            Assert.That(Path.GetDirectoryName(attachmentPath), Is.EqualTo(Path.Combine(reportPath, testStepId)));
        }

        [Test]
        public void Attachment_name()
        {
            reportContainer.Stub(rc => rc.ReportName).Return(@"C:\Reports\Report1");
            const string attachmentName = "attachmentName";

            var attachmentPath = attachmentPathResolver.GetAttachmentPath("testStepId", attachmentName, "");

            Assert.That(Path.GetFileName(attachmentPath), Is.EqualTo(attachmentName));
        }

        [Test]
        public void Attachment_extension()
        {
            reportContainer.Stub(rc => rc.ReportName).Return(@"C:\Reports\Report1");

            var attachmentPath = attachmentPathResolver.GetAttachmentPath("testStepId", "attachmentName", "text/xml");

            Assert.That(Path.GetExtension(attachmentPath), Is.EqualTo(".xml"));
        }
    }
}