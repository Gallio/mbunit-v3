// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Model.Logging;
using Gallio.Model.Logging.Tags;
using MbUnit.Framework;

namespace Gallio.Tests.Model.Logging
{
    [TestsOn(typeof(StructuredTestLogStream))]
    public class StructuredTestLogStreamTest
    {
        [Test]
        public void ConstructorThrowsIfNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new StructuredTestLogStream(null));
        }

        [Test]
        public void ConstructorCreatesAnEmptyStream()
        {
            StructuredTestLogStream stream = new StructuredTestLogStream("name");
            Assert.AreEqual("name", stream.Name);
            Assert.IsEmpty(stream.Body.Contents);
        }

        [Test]
        public void NameCanBeSetToNonNull()
        {
            StructuredTestLogStream stream = new StructuredTestLogStream("name");
            stream.Name = "foo";
            Assert.AreEqual("foo", stream.Name);
        }

        [Test]
        public void NameCannotBeSetToNull()
        {
            StructuredTestLogStream stream = new StructuredTestLogStream("name");
            Assert.Throws<ArgumentNullException>(() => stream.Name = null);
        }

        [Test]
        public void BodyCanBeSetToNonNull()
        {
            BodyTag newBody = new BodyTag();

            StructuredTestLogStream stream = new StructuredTestLogStream("name");
            stream.Body = newBody;
            Assert.AreSame(newBody, stream.Body);
        }

        [Test]
        public void BodyCannotBeSetToNull()
        {
            StructuredTestLogStream stream = new StructuredTestLogStream("name");
            Assert.Throws<ArgumentNullException>(() => stream.Body = null);
        }

        [Test]
        public void WriteToThrowsIfWriterIsNull()
        {
            StructuredTestLogStream stream = new StructuredTestLogStream("name");
            Assert.Throws<ArgumentNullException>(() => stream.WriteTo(null));
        }

        [Test]
        public void WriteToReproducesTheBodyOfTheStream()
        {
            StructuredTestLogStream stream = new StructuredTestLogStream("name")
            {
                Body = new BodyTag()
                {
                    Contents =
                    {
                        new TextTag("text")
                    }
                }
            };

            StructuredTextWriter writer = new StructuredTextWriter();
            stream.WriteTo(writer);
            writer.Close();

            Assert.AreEqual(stream.ToString(), writer.ToString());
        }

        [Test]
        public void ToStringPrintsTheBody()
        {
            StructuredTestLogStream stream = new StructuredTestLogStream("name")
            {
                Body = new BodyTag()
                {
                    Contents =
                    {
                        new TextTag("text")
                    }
                }
            };

            Assert.AreEqual("text", stream.ToString());
        }
    }
}
