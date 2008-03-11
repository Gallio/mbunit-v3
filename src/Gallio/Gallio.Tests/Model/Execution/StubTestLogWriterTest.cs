// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Model.Execution;
using MbUnit.Framework;

namespace Gallio.Tests.Model.Execution
{
    [TestFixture]
    [TestsOn(typeof(StubTestLogWriter))]
    public class StubTestLogWriterTest
    {
        [Test]
        public void AttachText()
        {
            Assert.AreEqual("[Attachment 'foo': text/plain]\n",
                Do(delegate(StubTestLogWriter logWriter)
                {
                    logWriter.AttachText("foo", "text/plain", "don't care");
                }));
        }

        [Test]
        public void AttachBytes()
        {
            Assert.AreEqual("[Attachment 'foo': application/octet-stream]\n",
                Do(delegate(StubTestLogWriter logWriter)
                {
                    logWriter.AttachBytes("foo", "application/octet-stream", new byte[0]);
                }));
        }

        [Test]
        public void Write()
        {
            Assert.AreEqual("abcdef",
                Do(delegate(StubTestLogWriter logWriter)
                {
                    logWriter.Write("AnyStream", "abcdef");
                }));
        }

        [Test]
        public void Embed()
        {
            Assert.AreEqual("[Attachment 'foo': text/plain]\n[Embedded Attachment 'foo']\n",
                Do(delegate(StubTestLogWriter logWriter)
                {
                    logWriter.AttachText("foo", "text/plain", "don't care");
                    logWriter.Embed("AnyStream", "foo");
                }));
        }

        [Test]
        public void Sections()
        {
            Assert.AreEqual("[Begin Section 'foo']\nBar bar bar\n[End Section]\n",
                Do(delegate(StubTestLogWriter logWriter)
                {
                    logWriter.BeginSection("AnyStream", "foo");
                    logWriter.Write("AnyStream", "Bar bar bar");
                    logWriter.EndSection("AnyStream");
                }));
        }


        private static string Do(Action<StubTestLogWriter> action)
        {
            StringWriter writer = new StringWriter();
            writer.NewLine = "\n";

            TextWriter oldWriter = Console.Out;
            try
            {
                Console.SetOut(writer);

                StubTestLogWriter logWriter = new StubTestLogWriter();
                action(logWriter);

                logWriter.Close();
                return writer.ToString();
            }
            finally
            {
                Console.SetOut(oldWriter);
            }
        }
    }
}
