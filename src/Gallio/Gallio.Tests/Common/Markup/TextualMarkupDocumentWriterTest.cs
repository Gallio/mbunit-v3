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
using System.IO;
using Gallio.Common.Markup;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Markup
{
    [TestFixture]
    [TestsOn(typeof(TextualMarkupDocumentWriter))]
    [TestsOn(typeof(StringMarkupDocumentWriter))]
    public class TextualMarkupDocumentWriterTest
    {
        [Test]
        [Row(true, "[Attachment 'foo': text/plain]\n")]
        [Row(false, "")]
        public void AttachText(bool verbose, string expectedOutput)
        {
            StringMarkupDocumentWriter logWriter = new StringMarkupDocumentWriter(verbose);
            logWriter.Attach(new TextAttachment("foo", "text/plain", "don't care"));

            Assert.AreEqual(expectedOutput, logWriter.ToString());
        }

        [Test]
        [Row(true, "[Attachment 'foo': application/octet-stream]\n")]
        [Row(false, "")]
        public void AttachBytes(bool verbose, string expectedOutput)
        {
            StringMarkupDocumentWriter logWriter = new StringMarkupDocumentWriter(verbose);
            logWriter.Attach(new BinaryAttachment("foo", "application/octet-stream", new byte[0])); 

            Assert.AreEqual(expectedOutput, logWriter.ToString());
        }

        [Test]
        [Row(true, "abcdef")]
        [Row(false, "abcdef")]
        public void Write(bool verbose, string expectedOutput)
        {
            StringMarkupDocumentWriter logWriter = new StringMarkupDocumentWriter(verbose);
            logWriter["AnyStream"].Write("abcdef");

            Assert.AreEqual(expectedOutput, logWriter.ToString());
        }

        [Test]
        [Row(true, "[Attachment 'foo': text/plain]\n[Embedded Attachment 'foo']\n")]
        [Row(false, "")]
        public void Embed(bool verbose, string expectedOutput)
        {
            StringMarkupDocumentWriter logWriter = new StringMarkupDocumentWriter(verbose);
            logWriter["AnyStream"].Embed(new TextAttachment("foo", "text/plain", "don't care"));

            Assert.AreEqual(expectedOutput, logWriter.ToString());
        }

        [Test]
        [Row(true, "[Section 'foo']\nBar bar bar\n[End]\n")]
        [Row(false, "foo\nBar bar bar\n")]
        public void Sections(bool verbose, string expectedOutput)
        {
            StringMarkupDocumentWriter logWriter = new StringMarkupDocumentWriter(verbose);
            using (logWriter["AnyStream"].BeginSection("foo"))
                logWriter["AnyStream"].Write("Bar bar bar");

            Assert.AreEqual(expectedOutput, logWriter.ToString());
        }

        [Test]
        [Row(true, "[Marker 'foo']Bar bar bar[End]")]
        [Row(false, "Bar bar bar")]
        public void Markers(bool verbose, string expectedOutput)
        {
            StringMarkupDocumentWriter logWriter = new StringMarkupDocumentWriter(verbose);
            using (logWriter["AnyStream"].BeginMarker(new Marker("foo")))
                logWriter["AnyStream"].Write("Bar bar bar");

            Assert.AreEqual(expectedOutput, logWriter.ToString());
        }
    }
}
