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

using Gallio.Framework.Assertions;
using MbUnit.Compatibility.Tests.Framework.Xml;
using MbUnit.Framework;
using MbUnit.Framework.Xml;
using System.IO;

#pragma warning disable 618

namespace MbUnit.Compatibility.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(OldXmlAssert))]
    public class OldXmlAssertTest
    {
        private string _xmlTrueTest;
        private string _xmlFalseTest;

        [TestFixtureSetUp]
        public void StartTest()
        {
            _xmlTrueTest = "<assert>true</assert>";
            _xmlFalseTest = "<assert>false</assert>";
        }

        #region XmlEquals
        [Test]
        public void XmlEqualsWithTextReader()
        {
            OldXmlAssert.XmlEquals(new StringReader(_xmlTrueTest), new StringReader(_xmlTrueTest));
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void XmlEqualsWithTextReaderFail()
        {
            OldXmlAssert.XmlEquals(new StringReader(_xmlTrueTest), new StringReader(_xmlFalseTest));
        }

        [Test]
        public void XmlEqualsWithString()
        {
            OldXmlAssert.XmlEquals(_xmlTrueTest, _xmlTrueTest);
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void XmlEqualsWithStringFail()
        {
            OldXmlAssert.XmlEquals(_xmlTrueTest, _xmlFalseTest);
        }

        [Test]
        public void XmlEqualsWithXmlInput()
        {
            OldXmlAssert.XmlEquals(new XmlInput(_xmlTrueTest), new XmlInput(_xmlTrueTest));
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void XmlEqualsWithXmlInputFail()
        {
            OldXmlAssert.XmlEquals(new XmlInput(_xmlTrueTest), new XmlInput(_xmlFalseTest));
        }

        [Test]
        public void XmlEqualsWithXmlDiff()
        {
            OldXmlAssert.XmlEquals(new XmlDiff(_xmlTrueTest, _xmlTrueTest));
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void XmlEqualsWithXmlDiffFail()
        {
            OldXmlAssert.XmlEquals(new XmlDiff(new XmlInput(_xmlTrueTest), new XmlInput(_xmlFalseTest)));
        }

        [RowTest]
        [Row("Optional Description", "Optional Description")]
        [Row("", "Xml does not match")]
        [Row("XmlDiff", "Xml does not match")]
        public void XmlEqualsWithXmlDiffFail_WithDiffConfiguration(string optionalDesciption, string expectedMessage)
        {
            try
            {
                OldXmlAssert.XmlEquals(new XmlDiff(new XmlInput(_xmlTrueTest), new XmlInput(_xmlFalseTest), new DiffConfiguration(optionalDesciption)));
            }
            catch (AssertionException e)
            {
                Assert.AreEqual(true, e.Message.StartsWith(expectedMessage));
            }
        }

        [Test]
        public void XmlEqualsWithXmlDiffFail_WithNullOptionalDescription()
        {
            try
            {
                OldXmlAssert.XmlEquals(new XmlDiff(new XmlInput(_xmlTrueTest), new XmlInput(_xmlFalseTest), new DiffConfiguration(null)));
            }
            catch (AssertionException e)
            {
                Assert.AreEqual(true, e.Message.StartsWith("Xml does not match"));
            }
        }
        #endregion

        [Test] 
		public void AssertStringEqualAndIdenticalToSelf() 
		{
            string control = _xmlTrueTest;
            string test = _xmlTrueTest;
            OldXmlAssert.XmlIdentical(control, test);
            OldXmlAssert.XmlEquals(control, test);
        }

        [Test]
		public void AssertDifferentStringsNotEqualNorIdentical() {
            string control = "<assert>true</assert>";
            string test = "<assert>false</assert>";
            XmlDiff xmlDiff = new XmlDiff(control, test);
            OldXmlAssert.XmlNotIdentical(xmlDiff);
            OldXmlAssert.XmlNotEquals(xmlDiff);
        }        
        
        [Test] 
		public void AssertXmlIdenticalUsesOptionalDescription() 
		{
            string description = "An Optional Description";
            try {
                XmlDiff diff = new XmlDiff(new XmlInput("<a/>"), new XmlInput("<b/>"), 
                                           new DiffConfiguration(description));
                OldXmlAssert.XmlIdentical(diff);
            } catch (AssertionException e) {
                Assert.AreEqual(true, e.Message.StartsWith(description));
            }
        }
        
        [Test]      
        public void AssertXmlEqualsUsesOptionalDescription() {
            string description = "Another Optional Description";
            try {
                XmlDiff diff = new XmlDiff(new XmlInput("<a/>"), new XmlInput("<b/>"), 
                                           new DiffConfiguration(description));
                OldXmlAssert.XmlEquals(diff);
            } catch (AssertionException e) {
                Assert.AreEqual(true, e.Message.StartsWith(description));
            }
        }
        
        [Test] 
        public void AssertXmlValidTrueForValidFile() {
            StreamReader reader = new StreamReader(ValidatorTests.ValidFile);
            try {
                OldXmlAssert.XmlValid(reader);
            } finally {
                reader.Close();
            }
        }
        
        [Test] 
		public void AssertXmlValidFalseForInvalidFile() {
            StreamReader reader = new StreamReader(ValidatorTests.InvalidFile);
            try {
                OldXmlAssert.XmlValid(reader);
                Assert.Fail("Expected assertion failure");
            } catch(AssertionException e) {
                AvoidUnusedVariableCompilerWarning(e);
            } finally {
                reader.Close();
            }
        }
        
        private static readonly string MY_SOLAR_SYSTEM = "<solar-system><planet name='Earth' position='3' supportsLife='yes'/><planet name='Venus' position='4'/></solar-system>";
        
        [Test] public void AssertXPathExistsWorksForExistentXPath() {
            OldXmlAssert.XPathExists("//planet[@name='Earth']", 
                                           MY_SOLAR_SYSTEM);
        }
        
        [Test] public void AssertXPathExistsFailsForNonExistentXPath() {
            try {
                OldXmlAssert.XPathExists("//star[@name='alpha centauri']", 
                                               MY_SOLAR_SYSTEM);
                Assert.Fail("Expected assertion failure");
            } catch (AssertionException e) {
                AvoidUnusedVariableCompilerWarning(e);
            }
        }
        
        [Test] public void AssertXPathEvaluatesToWorksForMatchingExpression() {
            OldXmlAssert.XPathEvaluatesTo("//planet[@position='3']/@supportsLife", 
                                                MY_SOLAR_SYSTEM,
                                                "yes");
        }
        
        [Test] public void AssertXPathEvaluatesToWorksForNonMatchingExpression() {
            OldXmlAssert.XPathEvaluatesTo("//planet[@position='4']/@supportsLife", 
                                                MY_SOLAR_SYSTEM,
                                                "");
        }
        
        [Test] public void AssertXPathEvaluatesToWorksConstantExpression() {
            OldXmlAssert.XPathEvaluatesTo("true()", 
                                                MY_SOLAR_SYSTEM,
                                                "True");
            OldXmlAssert.XPathEvaluatesTo("false()", 
                                                MY_SOLAR_SYSTEM,
                                                "False");
        }
        
        [Test] 
        public void AssertXslTransformResultsWorksWithStrings() {
        	string xslt = XsltTests.IDENTITY_TRANSFORM;
        	string someXml = "<a><b>c</b><b/></a>";
        	OldXmlAssert.XslTransformResults(xslt, someXml, someXml);
        }
        
        [Test] 
        public void AssertXslTransformResultsWorksWithXmlInput() {
        	StreamReader xsl = ValidatorTests.GetTestReader("animal.xsl");
        	XmlInput xslt = new XmlInput(xsl);
        	StreamReader xml = ValidatorTests.GetTestReader("testAnimal.xml");
        	XmlInput xmlToTransform = new XmlInput(xml);
        	XmlInput expectedXml = new XmlInput("<dog/>");
        	OldXmlAssert.XslTransformResults(xslt, xmlToTransform, expectedXml);
        }
        
        [Test] 
        public void AssertXslTransformResultsCatchesFalsePositive() {
        	StreamReader xsl = ValidatorTests.GetTestReader("animal.xsl");
        	XmlInput xslt = new XmlInput(xsl);
        	StreamReader xml = ValidatorTests.GetTestReader("testAnimal.xml");
        	XmlInput xmlToTransform = new XmlInput(xml);
        	XmlInput expectedXml = new XmlInput("<cat/>");
        	bool exceptionExpected = true;
        	try {
        		OldXmlAssert.XslTransformResults(xslt, xmlToTransform, expectedXml);
        		exceptionExpected = false;
        		Assert.Fail("Expected dog not cat!");
        	} catch (AssertionException e) {
        		AvoidUnusedVariableCompilerWarning(e);
        		if (!exceptionExpected) {
        			throw e;
        		}
        	}
        }

        private void AvoidUnusedVariableCompilerWarning(AssertionException e) {
            string msg = e.Message;
        }
    }
}
