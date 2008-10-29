using System;
using Gallio.Model.Logging;
using MbUnit.Framework;

namespace Gallio.Tests.Model.Logging
{
    [TestsOn(typeof(Attachment))]
    public class AttachmentTest
    {
        [Test]
        public void ToAttachmentData_Binary()
        {
            byte[] bytes = new byte[] { 1, 2, 3 };
            BinaryAttachment attachment = new BinaryAttachment("name", MimeTypes.Binary, bytes);

            AttachmentData attachmentData = attachment.ToAttachmentData();
            Assert.AreEqual("name", attachmentData.Name);
            Assert.AreEqual(MimeTypes.Binary, attachmentData.ContentType);
            Assert.AreEqual(bytes, attachmentData.GetBytes());
            Assert.AreEqual(AttachmentContentDisposition.Inline, attachmentData.ContentDisposition);
            Assert.AreEqual(AttachmentEncoding.Base64, attachmentData.Encoding);
        }

        [Test]
        public void ToAttachmentData_Text()
        {
            TextAttachment attachment = new TextAttachment("name", MimeTypes.PlainText, "content");

            AttachmentData attachmentData = attachment.ToAttachmentData();
            Assert.AreEqual("name", attachmentData.Name);
            Assert.AreEqual(MimeTypes.PlainText, attachmentData.ContentType);
            Assert.AreEqual("content", attachmentData.GetText());
            Assert.AreEqual(AttachmentContentDisposition.Inline, attachmentData.ContentDisposition);
            Assert.AreEqual(AttachmentEncoding.Text, attachmentData.Encoding);
        }

        [Test]
        public void FromAttachmentData_Binary()
        {
            byte[] bytes = new byte[] { 1, 2, 3 };
            AttachmentData attachmentData = new AttachmentData("name", MimeTypes.Binary, AttachmentEncoding.Base64, null, bytes);

            BinaryAttachment attachment = (BinaryAttachment)Attachment.FromAttachmentData(attachmentData);
            Assert.AreEqual("name", attachment.Name);
            Assert.AreEqual(MimeTypes.Binary, attachment.ContentType);
            Assert.AreEqual(bytes, attachment.Bytes);
        }

        [Test]
        public void FromAttachmentData_Text()
        {
            AttachmentData attachmentData = new AttachmentData("name", MimeTypes.PlainText, AttachmentEncoding.Text, "content", null);

            TextAttachment attachment = (TextAttachment)Attachment.FromAttachmentData(attachmentData);
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
