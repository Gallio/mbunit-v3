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
using Gallio.Common.Markup;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Markup
{
    [TestsOn(typeof(Attachment))]
    public class AttachmentTest
    {
        [Test]
        public void ToAttachmentData_Binary()
        {
            byte[] bytes = new byte[] { 1, 2, 3 };
            var attachment = new BinaryAttachment("name", MimeTypes.Binary, bytes);
            AttachmentData attachmentData = attachment.ToAttachmentData();
            Assert.AreEqual("name", attachmentData.Name);
            Assert.AreEqual(MimeTypes.Binary, attachmentData.ContentType);
            Assert.AreEqual(bytes, attachmentData.GetBytes());
            Assert.AreEqual(AttachmentContentDisposition.Inline, attachmentData.ContentDisposition);
            Assert.AreEqual(AttachmentType.Binary, attachmentData.Type);
        }

        [Test]
        public void ToAttachmentData_Text()
        {
            var attachment = new TextAttachment("name", MimeTypes.PlainText, "content");
            AttachmentData attachmentData = attachment.ToAttachmentData();
            Assert.AreEqual("name", attachmentData.Name);
            Assert.AreEqual(MimeTypes.PlainText, attachmentData.ContentType);
            Assert.AreEqual("content", attachmentData.GetText());
            Assert.AreEqual(AttachmentContentDisposition.Inline, attachmentData.ContentDisposition);
            Assert.AreEqual(AttachmentType.Text, attachmentData.Type);
        }

        [Test]
        public void FromAttachmentData_Binary()
        {
            byte[] bytes = new byte[] { 1, 2, 3 };
            var attachmentData = new AttachmentData("name", MimeTypes.Binary, AttachmentType.Binary, null, bytes);
            var attachment = (BinaryAttachment)Attachment.FromAttachmentData(attachmentData);
            Assert.AreEqual("name", attachment.Name);
            Assert.AreEqual(MimeTypes.Binary, attachment.ContentType);
            Assert.AreEqual(bytes, attachment.Bytes);
        }

        [Test]
        public void FromAttachmentData_Text()
        {
            var attachmentData = new AttachmentData("name", MimeTypes.PlainText, AttachmentType.Text, "content", null);
            var attachment = (TextAttachment)Attachment.FromAttachmentData(attachmentData);
            Assert.AreEqual("name", attachment.Name);
            Assert.AreEqual(MimeTypes.PlainText, attachment.ContentType);
            Assert.AreEqual("content", attachment.Text);
        }

        [Test]
        public void FromAttachmentData_ThrowsIfDataIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => Attachment.FromAttachmentData(null));
        }
    }
}
