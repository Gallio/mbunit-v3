#pragma warning disable 618

namespace MbUnit.Tests.Compatibility.Framework.Xml {
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
