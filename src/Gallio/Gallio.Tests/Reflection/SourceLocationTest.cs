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
using System.Collections.Generic;
using System.Text;
using Gallio.Reflection;
using MbUnit.Framework.Xml;
using MbUnit2::MbUnit.Framework;

namespace Gallio.Tests.Reflection
{
    [TestFixture]
    [TestsOn(typeof(SourceLocation))]
    public class SourceLocationTest
    {
        [RowTest]
        [Row("file", 1, 1)]
        [Row("file", 1, 0)]
        [Row("file", 0, 1)]
        [Row("file", 0, 0)]
        [Row("file", -1, 1, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [Row("file", 1, -1, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [Row(null, 1, 1, ExpectedException = typeof(ArgumentNullException))]
        public void Constructor(string filename, int line, int column)
        {
            SourceLocation location = new SourceLocation(filename, line, column);
            Assert.AreEqual(filename, location.Filename);
            Assert.AreEqual(line, location.Line);
            Assert.AreEqual(column, location.Column);
        }

        [RowTest]
        [Row("file", 1, 1)]
        [Row("file", 1, 0)]
        [Row("file", 0, 1)]
        [Row("file", 0, 0)]
        [Row("file", -1, 1, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [Row("file", 1, -1, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [Row(null, 1, 1, ExpectedException = typeof(ArgumentNullException))]
        public void Setters(string filename, int line, int column)
        {
            SourceLocation location = new SourceLocation("", 0, 0);

            location.Filename = filename;
            location.Line = line;
            location.Column = column;

            Assert.AreEqual(filename, location.Filename);
            Assert.AreEqual(line, location.Line);
            Assert.AreEqual(column, location.Column);
        }

        [Test]
        public void TypeIsXmlSerializable()
        {
            XmlSerializationAssert.IsXmlSerializable(typeof(SourceLocation));
        }
    }
}
