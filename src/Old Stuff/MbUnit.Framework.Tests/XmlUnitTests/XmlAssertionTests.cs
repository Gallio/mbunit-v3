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

namespace MbUnit.Tests.XmlUnit {
    using MbUnit.Framework;
    using MbUnit.Framework.Xml;
    using TestDriven.UnitTesting.Exceptions;
	using System.IO;
    
    [TestFixture]
    public class XmlAssertionTests {        
        [Test] 
		public void AssertStringEqualAndIdenticalToSelf() 
		{
            string control = "<assert>true</assert>";
            string test = "<assert>true</assert>";
            XmlAssert.XmlIdentical(control, test);
            XmlAssert.XmlEquals(control, test);
        }        
        
        [Test] 
		public void AssertDifferentStringsNotEqualNorIdentical() {
            string control = "<assert>true</assert>";
            string test = "<assert>false</assert>";
            XmlDiff xmlDiff = new XmlDiff(control, test);
            XmlAssert.XmlNotIdentical(xmlDiff);
            XmlAssert.XmlNotEquals(xmlDiff);
        }        
        
        [Test] 
		public void AssertXmlIdenticalUsesOptionalDescription() 
		{
            string description = "An Optional Description";
            try {
                XmlDiff diff = new XmlDiff(new XmlInput("<a/>"), new XmlInput("<b/>"), 
                                           new DiffConfiguration(description));
                XmlAssert.XmlIdentical(diff);
            } catch (AssertionException e) {
                Assert.AreEqual(true, e.Message.StartsWith(description));
            }
        }
        
        [Test] public void AssertXmlEqualsUsesOptionalDescription() {
            string description = "Another Optional Description";
            try {
                XmlDiff diff = new XmlDiff(new XmlInput("<a/>"), new XmlInput("<b/>"), 
                                           new DiffConfiguration(description));
                XmlAssert.XmlEquals(diff);
            } catch (AssertionException e) {
                Assert.AreEqual(true, e.Message.StartsWith(description));
            }
        }
        
        [Test] public void AssertXmlValidTrueForValidFile() {
            StreamReader reader = new StreamReader(ValidatorTests.ValidFile);
            try {
                XmlAssert.XmlValid(reader);
            } finally {
                reader.Close();
            }
        }
        
        [Test] 
		public void AssertXmlValidFalseForInvalidFile() {
            StreamReader reader = new StreamReader(ValidatorTests.InvalidFile);
            try {
                XmlAssert.XmlValid(reader);
                Assert.Fail("Expected assertion failure");
            } catch(AssertionException e) {
                AvoidUnusedVariableCompilerWarning(e);
            } finally {
                reader.Close();
            }
        }
        
        private static readonly string MY_SOLAR_SYSTEM = "<solar-system><planet name='Earth' position='3' supportsLife='yes'/><planet name='Venus' position='4'/></solar-system>";
        
        [Test] public void AssertXPathExistsWorksForExistentXPath() {
            XmlAssert.XPathExists("//planet[@name='Earth']", 
                                           MY_SOLAR_SYSTEM);
        }
        
        [Test] public void AssertXPathExistsFailsForNonExistentXPath() {
            try {
                XmlAssert.XPathExists("//star[@name='alpha centauri']", 
                                               MY_SOLAR_SYSTEM);
                Assert.Fail("Expected assertion failure");
            } catch (AssertionException e) {
                AvoidUnusedVariableCompilerWarning(e);
            }
        }
        
        [Test] public void AssertXPathEvaluatesToWorksForMatchingExpression() {
            XmlAssert.XPathEvaluatesTo("//planet[@position='3']/@supportsLife", 
                                                MY_SOLAR_SYSTEM,
                                                "yes");
        }
        
        [Test] public void AssertXPathEvaluatesToWorksForNonMatchingExpression() {
            XmlAssert.XPathEvaluatesTo("//planet[@position='4']/@supportsLife", 
                                                MY_SOLAR_SYSTEM,
                                                "");
        }
        
        [Test] public void AssertXPathEvaluatesToWorksConstantExpression() {
            XmlAssert.XPathEvaluatesTo("true()", 
                                                MY_SOLAR_SYSTEM,
                                                "True");
            XmlAssert.XPathEvaluatesTo("false()", 
                                                MY_SOLAR_SYSTEM,
                                                "False");
        }
        
        [Test] public void AssertXslTransformResultsWorksWithStrings() {
        	string xslt = XsltTests.IDENTITY_TRANSFORM;
        	string someXml = "<a><b>c</b><b/></a>";
        	XmlAssert.XslTransformResults(xslt, someXml, someXml);
        }
        
        [Test] public void AssertXslTransformResultsWorksWithXmlInput() {
        	StreamReader xsl = ValidatorTests.GetTestReader("animal.xsl");
        	XmlInput xslt = new XmlInput(xsl);
        	StreamReader xml = ValidatorTests.GetTestReader("testAnimal.xml");
        	XmlInput xmlToTransform = new XmlInput(xml);
        	XmlInput expectedXml = new XmlInput("<dog/>");
        	XmlAssert.XslTransformResults(xslt, xmlToTransform, expectedXml);
        }
        
        [Test] public void AssertXslTransformResultsCatchesFalsePositive() {
        	StreamReader xsl = ValidatorTests.GetTestReader("animal.xsl");
        	XmlInput xslt = new XmlInput(xsl);
        	StreamReader xml = ValidatorTests.GetTestReader("testAnimal.xml");
        	XmlInput xmlToTransform = new XmlInput(xml);
        	XmlInput expectedXml = new XmlInput("<cat/>");
        	bool exceptionExpected = true;
        	try {
        		XmlAssert.XslTransformResults(xslt, xmlToTransform, expectedXml);
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
