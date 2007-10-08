// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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

extern alias MbUnit2;
using System;
using MbUnit.Core.Reporting;
//using MbUnit.Framework.Kernel.Results;
using MbUnit2::MbUnit.Framework;

namespace MbUnit.Tests.Core.Reporting
{
    [TestFixture]
    [TestsOn(typeof(ReportUtils))]
    [Author("Vadim")]
    public class ReportUtilsTests
    {
        [RowTest]
        [Row(@"C:\dir\fiename.ext", @"C:\dir\fiename")]
        [Row(@"fiename", @"fiename.content")]
        [Row(@"fiename.ext", @"fiename")]
        public void GetContentDirectoryPathTest(string inputDir, string outputDir)
        {
            Assert.AreEqual(outputDir, ReportUtils.GetContentDirectoryPath(inputDir));
        }

        [Test]
        [Ignore("To properly test it need to mock Directory.")]
        public void SaveAttachmentContentsTest()
        {}

        [Test]
        [ExpectedArgumentNullException]
        public void LoadAttachmentContentsWithNullAttachementTest()
        {
            ReportUtils.LoadAttachmentContents(null, "pat");
        }

        [Test]
        [ExpectedArgumentNullException]
        public void LoadAttachmentContentsWithNullContentPathTest()
        {
            ReportUtils.LoadAttachmentContents(new ExecutionLogAttachment("", "", ExecutionLogAttachmentEncoding.Xml, "", null), null);
        }

        [Test]
        [Ignore("To properly test it need to abstract StreamReader and File.")]
        public void LoadAttachmentContentsTest()
        { }

        [Test]
        [ExpectedArgumentNullException]
        public void GetAttachmentFileNameWithNullAttachmentName()
        {
            ReportUtils.GetAttachmentFileName(null);
        }

        [Test]
        public void GetAttachmentFileNameTest()
        {
            Assert.AreEqual("tes_t", ReportUtils.GetAttachmentFileName("tes\\t"));
        }

        [Test]
        [ExpectedArgumentNullException]
        public void GetStepRunDirectoryNameWithNullStepId()
        {
            ReportUtils.GetStepRunDirectoryName(null);
        }

        [Test]
        public void GetStepRunDirectoryNameTest()
        {
            Assert.AreEqual("test", ReportUtils.GetStepRunDirectoryName("test"));
        }
    }
}