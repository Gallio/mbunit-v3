using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Model.Logging;
using MbUnit.Framework;

namespace Gallio.Tests.Model.Logging
{
    [TestsOn(typeof(StructuredTestLog))]
    public class StructuredTestLogTest
    {
        [Test]
        public void TestLogIsInitiallyEmpty()
        {
            StructuredTestLog log = new StructuredTestLog();
            Assert.IsEmpty(log.Streams);
            Assert.IsEmpty(log.Attachments);
        }

        [Test]
        public void GetAttachmentReturnsNamedAttachment()
        {
            StructuredTestLog log = new StructuredTestLog();
            AttachmentData data = new AttachmentData("foo", MimeTypes.Binary, AttachmentEncoding.Base64, null, new byte[0]);
            log.Attachments.Add(data);
            log.Attachments.Add(new AttachmentData("bar", MimeTypes.Binary, AttachmentEncoding.Base64, null, new byte[0]));

            Assert.AreSame(data, log.GetAttachment("foo"));
        }

        [Test]
        public void GetAttachmentReturnsNullIfAttachmentNotFound()
        {
            StructuredTestLog log = new StructuredTestLog();
            log.Attachments.Add(new AttachmentData("bar", MimeTypes.Binary, AttachmentEncoding.Base64, null, new byte[0]));

            Assert.IsNull(log.GetAttachment("foo"));
        }

        [Test]
        public void GetAttachmentThrowsIfNameIsNull()
        {
            StructuredTestLog log = new StructuredTestLog();
            Assert.Throws<ArgumentNullException>(() => log.GetAttachment(null));
        }

        [Test]
        public void GetStreamReturnsNamedStream()
        {
            StructuredTestLog log = new StructuredTestLog();
            StructuredTestLogStream stream = new StructuredTestLogStream("foo");
            log.Streams.Add(stream);
            log.Streams.Add(new StructuredTestLogStream("bar"));

            Assert.AreSame(stream, log.GetStream("foo"));
        }

        [Test]
        public void GetStreamReturnsNullIfStreamNotFound()
        {
            StructuredTestLog log = new StructuredTestLog();
            log.Streams.Add(new StructuredTestLogStream("bar"));

            Assert.IsNull(log.GetAttachment("foo"));
        }

        [Test]
        public void GetStreamThrowsIfNameIsNull()
        {
            StructuredTestLog log = new StructuredTestLog();
            Assert.Throws<ArgumentNullException>(() => log.GetStream(null));
        }

        [Test]
        public void ToStringPrintsTheContentsOfAllStreams()
        {
            StructuredTestLogWriter writer = new StructuredTestLogWriter();
            writer.Default.WriteLine("Foo");
            writer.ConsoleOutput.WriteLine("Bar");
            writer.Close();

            Assert.AreEqual("*** Log ***\n\nFoo\n\n*** ConsoleOutput ***\n\nBar\n", writer.TestLog.ToString());
        }

        [Test]
        public void WriteToThrowsIfWriterIsNull()
        {
            StructuredTestLog log = new StructuredTestLog();
            Assert.Throws<ArgumentNullException>((() => log.WriteTo(null)));
        }

        [Test]
        public void WriteToReproducesTheStructureOfTheLog()
        {
            StructuredTestLogWriter sourceWriter = new StructuredTestLogWriter();
            sourceWriter.Default.WriteLine("Foo");
            sourceWriter.ConsoleOutput.WriteLine("Bar");
            sourceWriter.ConsoleOutput.EmbedPlainText("foo", "bar");
            sourceWriter.Close();

            StructuredTestLogWriter targetWriter = new StructuredTestLogWriter();

            sourceWriter.TestLog.WriteTo(targetWriter);
            targetWriter.Close();

            Assert.AreEqual(targetWriter.TestLog.ToString(), sourceWriter.TestLog.ToString());
        }
    }
}
