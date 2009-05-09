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
            StructuredDocument log = new StructuredDocument();
            Assert.IsEmpty(log.Streams);
            Assert.IsEmpty(log.Attachments);
        }

        [Test]
        public void GetAttachmentReturnsNamedAttachment()
        {
            StructuredDocument log = new StructuredDocument();
            AttachmentData data = new AttachmentData("foo", MimeTypes.Binary, AttachmentEncoding.Base64, null, new byte[0]);
            log.Attachments.Add(data);
            log.Attachments.Add(new AttachmentData("bar", MimeTypes.Binary, AttachmentEncoding.Base64, null, new byte[0]));

            Assert.AreSame(data, log.GetAttachment("foo"));
        }

        [Test]
        public void GetAttachmentReturnsNullIfAttachmentNotFound()
        {
            StructuredDocument log = new StructuredDocument();
            log.Attachments.Add(new AttachmentData("bar", MimeTypes.Binary, AttachmentEncoding.Base64, null, new byte[0]));

            Assert.IsNull(log.GetAttachment("foo"));
        }

        [Test]
        public void GetAttachmentThrowsIfNameIsNull()
        {
            StructuredDocument log = new StructuredDocument();
            Assert.Throws<ArgumentNullException>(() => log.GetAttachment(null));
        }

        [Test]
        public void GetStreamReturnsNamedStream()
        {
            StructuredDocument log = new StructuredDocument();
            StructuredStream stream = new StructuredStream("foo");
            log.Streams.Add(stream);
            log.Streams.Add(new StructuredStream("bar"));

            Assert.AreSame(stream, log.GetStream("foo"));
        }

        [Test]
        public void GetStreamReturnsNullIfStreamNotFound()
        {
            StructuredDocument log = new StructuredDocument();
            log.Streams.Add(new StructuredStream("bar"));

            Assert.IsNull(log.GetAttachment("foo"));
        }

        [Test]
        public void GetStreamThrowsIfNameIsNull()
        {
            StructuredDocument log = new StructuredDocument();
            Assert.Throws<ArgumentNullException>(() => log.GetStream(null));
        }

        [Test]
        public void ToStringPrintsTheContentsOfAllStreams()
        {
            StructuredDocumentWriter writer = new StructuredDocumentWriter();
            writer.Default.WriteLine("Foo");
            writer.ConsoleOutput.WriteLine("Bar");
            writer.Close();

            Assert.AreEqual("*** Log ***\n\nFoo\n\n*** ConsoleOutput ***\n\nBar\n", writer.Document.ToString());
        }

        [Test]
        public void WriteToThrowsIfWriterIsNull()
        {
            StructuredDocument log = new StructuredDocument();
            Assert.Throws<ArgumentNullException>((() => log.WriteTo(null)));
        }

        [Test]
        public void WriteToReproducesTheStructureOfTheLog()
        {
            StructuredDocumentWriter sourceWriter = new StructuredDocumentWriter();
            sourceWriter.Default.WriteLine("Foo");
            sourceWriter.ConsoleOutput.WriteLine("Bar");
            sourceWriter.ConsoleOutput.EmbedPlainText("foo", "bar");
            sourceWriter.Close();

            StructuredDocumentWriter targetWriter = new StructuredDocumentWriter();

            sourceWriter.Document.WriteTo(targetWriter);
            targetWriter.Close();

            Assert.AreEqual(targetWriter.Document.ToString(), sourceWriter.Document.ToString());
        }
    }
}
