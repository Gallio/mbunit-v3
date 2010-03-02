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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Common.Markup;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Markup
{
    [TestsOn(typeof(StructuredDocument))]
    public class StructuredDocumentTest
    {
        [Test]
        public void TestLogIsInitiallyEmpty()
        {
            var log = new StructuredDocument();
            Assert.IsEmpty(log.Streams);
            Assert.IsEmpty(log.Attachments);
        }

        [Test]
        public void GetAttachmentReturnsNamedAttachment()
        {
            var log = new StructuredDocument();
            var data = new AttachmentData("foo", MimeTypes.Binary, AttachmentType.Binary, null, new byte[0]);
            log.Attachments.Add(data);
            log.Attachments.Add(new AttachmentData("bar", MimeTypes.Binary, AttachmentType.Binary, null, new byte[0]));
            Assert.AreSame(data, log.GetAttachment("foo"));
        }

        [Test]
        public void GetAttachmentReturnsNullIfAttachmentNotFound()
        {
            var log = new StructuredDocument();
            log.Attachments.Add(new AttachmentData("bar", MimeTypes.Binary, AttachmentType.Binary, null, new byte[0]));
            Assert.IsNull(log.GetAttachment("foo"));
        }

        [Test]
        public void GetAttachmentThrowsIfNameIsNull()
        {
            var log = new StructuredDocument();
            Assert.Throws<ArgumentNullException>(() => log.GetAttachment(null));
        }

        [Test]
        public void GetStreamReturnsNamedStream()
        {
            var log = new StructuredDocument();
            var stream = new StructuredStream("foo");
            log.Streams.Add(stream);
            log.Streams.Add(new StructuredStream("bar"));

            Assert.AreSame(stream, log.GetStream("foo"));
        }

        [Test]
        public void GetStreamReturnsNullIfStreamNotFound()
        {
            var log = new StructuredDocument();
            log.Streams.Add(new StructuredStream("bar"));
            Assert.IsNull(log.GetAttachment("foo"));
        }

        [Test]
        public void GetStreamThrowsIfNameIsNull()
        {
            var log = new StructuredDocument();
            Assert.Throws<ArgumentNullException>(() => log.GetStream(null));
        }

        [Test]
        public void ToStringPrintsTheContentsOfAllStreams()
        {
            var writer = new StructuredDocumentWriter();
            writer.Default.WriteLine("Foo");
            writer.ConsoleOutput.WriteLine("Bar");
            writer.Close();
            Assert.AreEqual("*** Log ***\n\nFoo\n\n*** ConsoleOutput ***\n\nBar\n", writer.Document.ToString());
        }

        [Test]
        public void WriteToThrowsIfWriterIsNull()
        {
            var log = new StructuredDocument();
            Assert.Throws<ArgumentNullException>((() => log.WriteTo(null)));
        }

        [Test]
        public void WriteToReproducesTheStructureOfTheLog()
        {
            var sourceWriter = new StructuredDocumentWriter();
            sourceWriter.Default.WriteLine("Foo");
            sourceWriter.ConsoleOutput.WriteLine("Bar");
            sourceWriter.ConsoleOutput.EmbedPlainText("foo", "bar");
            sourceWriter.Close();
            var targetWriter = new StructuredDocumentWriter();
            sourceWriter.Document.WriteTo(targetWriter);
            targetWriter.Close();
            Assert.AreEqual(targetWriter.Document.ToString(), sourceWriter.Document.ToString());
        }
    }
}
