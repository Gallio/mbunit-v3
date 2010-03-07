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

#pragma warning disable 618

namespace MbUnit.Compatibility.Tests.Framework.Xml {
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;
    using MbUnit.Framework;
    using MbUnit.Framework.Xml;
    
    [TestFixture]
    public class XmlInputTests {     
        private static readonly string INPUT = "<abc><q>werty</q><u>iop</u></abc> ";
        private string _expected; 
        
        [SetUp] public void SetExpected() {
            _expected = ReadOuterXml(new XmlTextReader(new StringReader(INPUT)));
        }
        
        [Test] public void StringInputTranslatesToXmlReader() {
            XmlInput input = new XmlInput(INPUT);
            string actual = ReadOuterXml(input.CreateXmlReader());
            OldAssert.AreEqual(_expected, actual);
        }
        
        [Test] public void TextReaderInputTranslatesToXmlReader() {
            XmlInput input = new XmlInput(new StringReader(INPUT));
            string actual = ReadOuterXml(input.CreateXmlReader());
            OldAssert.AreEqual(_expected, actual);
        }
        
        [Test] public void StreamInputTranslatesToXmlReader() {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream, Encoding.Default);
            writer.WriteLine(INPUT);
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            XmlInput input = new XmlInput(stream);
            string actual = ReadOuterXml(input.CreateXmlReader());
            try {
                OldAssert.AreEqual(_expected, actual);
            } finally {
                writer.Close();
            }
        }
        
        private string ReadOuterXml(XmlReader forReader) {
            try {
                forReader.MoveToContent();
                return forReader.ReadOuterXml();
            } finally {
                forReader.Close();
            }
        }
        
        [Test] public void NotEqualsNull() {
            XmlInput input = new XmlInput(INPUT);
            OldAssert.AreEqual(false, input.Equals(null));
        }
        
        [Test] public void NotEqualsADifferentClass() {
            XmlInput input = new XmlInput(INPUT);
            OldAssert.AreEqual(false, input.Equals(INPUT));
        }
        
        [Test] public void EqualsSelf() {
            XmlInput input = new XmlInput(INPUT);
            OldAssert.AreEqual(input, input);
        }
        
        [Test] public void EqualsCopyOfSelf() {
            XmlInput input = new XmlInput(INPUT);
            OldAssert.AreEqual(new XmlInput(INPUT), input);
        }
        
        [Test] public void HashCodeEqualsHashCodeOfInput() {
            XmlInput input = new XmlInput(INPUT);
            OldAssert.AreEqual(INPUT.GetHashCode(), input.GetHashCode());
        }
        
        [Test] public void HashCodeEqualsHashCodeOfCopy() {
            XmlInput input = new XmlInput(INPUT);
            OldAssert.AreEqual(new XmlInput(INPUT).GetHashCode(), input.GetHashCode());
        }
        
    }
}
