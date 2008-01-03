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

using System;
using System.IO;
using Gallio.Logging;
using MbUnit.Framework;

namespace Gallio.Tests.Logging
{
    [TestFixture]
    [TestsOn(typeof(TextLogWriter))]
    public class TextLogWriterTest
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfTextWriterIsNull()
        {
            new TextLogWriter(null);
        }

        [Test]
        public void Attach()
        {
            Assert.AreEqual("[Attach 'foo': text/plain]\n",
                Do(delegate(TextLogWriter logWriter)
                {
                    logWriter.AttachPlainText("foo", "don't care");
                }));
        }

        [Test]
        public void Stream_Write()
        {
            Assert.AreEqual("abcdef",
                Do(delegate(TextLogWriter logWriter)
                {
                    logWriter["Any"].Write("abcdef");
                }));
        }

        [Test]
        public void Stream_Embed()
        {
            Assert.AreEqual("[Embed 'foo': text/plain]\n",
                Do(delegate(TextLogWriter logWriter)
                {
                    logWriter["Any"].EmbedPlainText("foo", "don't care");
                }));
        }

        [Test]
        public void Stream_EmbedExisting()
        {
            Assert.AreEqual("[Embed 'foo']\n",
                Do(delegate(TextLogWriter logWriter)
                {
                    logWriter["Any"].EmbedExisting("foo");
                }));
        }

        [Test]
        public void Stream_Sections()
        {
            Assert.AreEqual("[Begin Section 'foo']\nBar bar bar\n[End Section]\n",
                Do(delegate(TextLogWriter logWriter)
                {
                    using (logWriter["Any"].BeginSection("foo"))
                        logWriter["Any"].WriteLine("Bar bar bar");
                }));
        }

        private string Do(Action<TextLogWriter> action)
        {
            StringWriter writer = new StringWriter();
            writer.NewLine = "\n";
            TextLogWriter logWriter = new TextLogWriter(writer);

            action(logWriter);

            logWriter.Close();
            return writer.ToString();
        }
    }
}
