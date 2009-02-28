// MbUnit Test Framework
// 
// Copyright (c) 2004 Jonathan de Halleux
//
// This software is provided 'as-is', without any express or implied warranty. 
// 
// In no event will the authors be held liable for any damages arising from 
// the use of this software.
// Permission is granted to anyone to use this software for any purpose, 
// including commercial applications, and to alter it and redistribute it 
// freely, subject to the following restrictions:
//
//		1. The origin of this software must not be misrepresented; 
//		you must not claim that you wrote the original software. 
//		If you use this software in a product, an acknowledgment in the product 
//		documentation would be appreciated but is not required.
//
//		2. Altered source versions must be plainly marked as such, and must 
//		not be misrepresented as being the original software.
//
//		3. This notice may not be removed or altered from any source 
//		distribution.
//		
//		MbUnit HomePage: http://www.mbunit.com
//		Author: Jonathan de Halleux


//	Original XmlUnit license
/*
******************************************************************
Copyright (c) 2001, Jeff Martin, Tim Bacon
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above
      copyright notice, this list of conditions and the following
      disclaimer in the documentation and/or other materials provided
      with the distribution.
    * Neither the name of the xmlunit.sourceforge.net nor the names
      of its contributors may be used to endorse or promote products
      derived from this software without specific prior written
      permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
POSSIBILITY OF SUCH DAMAGE.

******************************************************************
*/



namespace MbUnit.Framework
{
	using MbUnit.Framework.Xml;
    using System.IO;
	using System;


    /// <summary>
    /// Class containing generic assert methods for XML markup
    /// </summary>
    [Obsolete("The OldXmlAsserts are obsolete and will be replaced in a future version.")]
    public static class OldXmlAssert{
        /// <summary>
        /// Asserts that two pieces of XML are similar.
        /// </summary>
        /// <param name="controlTextReader">The control text reader.</param>
        /// <param name="testTextReader">The test text reader.</param>
        public static void XmlEquals(TextReader controlTextReader, TextReader testTextReader) {
            XmlEquals(new XmlDiff(controlTextReader, testTextReader));
        }


        /// <summary>
        /// Asserts that two pieces of XML are similar.
        /// </summary>
        /// <param name="controlText">The control text.</param>
        /// <param name="testText">The test text.</param>
        public static void XmlEquals(string controlText, string testText) {
           XmlEquals(new XmlDiff(controlText, testText));
        }


        /// <summary>
        /// Asserts that two pieces of XML are similar.
        /// </summary>
        /// <param name="controlInput">The control input.</param>
        /// <param name="testInput">The test input.</param>
        public static void XmlEquals(XmlInput controlInput, XmlInput testInput) {
            XmlEquals(new XmlDiff(controlInput, testInput));
        }


        /// <summary>
        /// Asserts that two pieces of XML are identical.
        /// </summary>
        /// <param name="controlTextReader">The control text reader.</param>
        /// <param name="testTextReader">The test text reader.</param>
        public static void XmlIdentical(TextReader controlTextReader, TextReader testTextReader) {
            XmlIdentical(new XmlDiff(controlTextReader, testTextReader));
        }


        /// <summary>
        /// Asserts that two pieces of XML are identical.
        /// </summary>
        /// <param name="controlText">The control text.</param>
        /// <param name="testText">The test text.</param>
        public static void XmlIdentical(string controlText, string testText) {
            XmlIdentical(new XmlDiff(controlText, testText));
        }


        /// <summary>
        /// Asserts that two pieces of XML are identical.
        /// </summary>
        /// <param name="controlInput">The control input.</param>
        /// <param name="testInput">The test input.</param>
        public static void XmlIdentical(XmlInput controlInput, XmlInput testInput) {
            XmlIdentical(new XmlDiff(controlInput, testInput));
        }

        /// <summary>
        /// Asserts that two pieces of XMl are similar given their diff
        /// </summary>
        /// <param name="xmlDiff">The XML diff.</param>
        public static void XmlEquals(XmlDiff xmlDiff) {
            XmlEquals(xmlDiff, true);
        }

        /// <summary>
        /// Asserts that two pieces of XMl are not similar given their diff
        /// </summary>
        /// <param name="xmlDiff">The XML diff.</param>
        public static void XmlNotEquals(XmlDiff xmlDiff) {
            XmlEquals(xmlDiff, false);
        }

        /// <summary>
        /// Asserts that two pieces of XML are similar (or not) given their diff and a boolean value
        /// </summary>
        /// <param name="xmlDiff">The XML diff.</param>
        /// <param name="equal">if set to <c>true</c> the assert passes if the XML is similar.
        /// if <c>false</c>, the assert passes if the XML is not similar</param>
        private static void XmlEquals(XmlDiff xmlDiff, bool equal)
        {
            DiffResult diffResult = xmlDiff.Compare();
            Assert.AreEqual(equal, diffResult.Equal, FailMessage(equal, xmlDiff.OptionalDescription));

        }

        private static string FailMessage(bool equal, string optionalDescription)
        {
            if (optionalDescription == null || optionalDescription.Equals("") || optionalDescription.Equals(DiffConfiguration.DEFAULT_DESCRIPTION))
                return (equal == true) ? "Xml does not match" : "Xml matches but should be different";
            else
                return optionalDescription;
        }


        /// <summary>
        /// Asserts that two pieces of XML are identical given their diff
        /// </summary>
        /// <param name="xmlDiff">The XML diff.</param>
        public static void XmlIdentical(XmlDiff xmlDiff) {
            XmlIdentical(xmlDiff, true);
        }

        /// <summary>
        /// Asserts that two pieces of XML are not identical given their diff
        /// </summary>
        /// <param name="xmlDiff">The XML diff.</param>
        public static void XmlNotIdentical(XmlDiff xmlDiff) {
            XmlIdentical(xmlDiff, false);
        }

        /// <summary>
        /// Asserts that two pieces of XML are identical (or not) given their diff and a boolean value
        /// </summary>
        /// <param name="xmlDiff">The XML diff.</param>
        /// <param name="identical">if set to <c>true</c> the assert passes if the XML is identical.
        /// if <c>false</c>, the assert passes if the XML is not identical</param>
        private static void XmlIdentical(XmlDiff xmlDiff, bool identical) {
            DiffResult diffResult = xmlDiff.Compare();
            Assert.AreEqual(identical, diffResult.Identical,xmlDiff.OptionalDescription);
        }


        /// <summary>
        /// Asserts that <paramref name="someXml"/> is valid XML.
        /// </summary>
        /// <param name="someXml">The XMl to test</param>
        public static void XmlValid(string someXml) {
            XmlValid(new XmlInput(someXml));
        }


        /// <summary>
        /// Asserts that <paramref name="someXml"/> is valid XML given a <paramref name="baseURI"/>
        /// </summary>
        /// <param name="someXml">The XML to test.</param>
        /// <param name="baseURI">The base URI.</param>
        public static void XmlValid(string someXml, string baseURI) {
            XmlValid(new XmlInput(someXml, baseURI));
        }


        /// <summary>
        /// Asserts that some XML is valid.
        /// </summary>
        /// <param name="reader">A <see cref="TextReader"/> pointing to the XML to test.</param>
        public static void XmlValid(TextReader reader) {
            XmlValid(new XmlInput(reader));
        }

        /// <summary>
        /// Asserts that some XML is valid given a <paramref name="baseURI"/>
        /// </summary>
        /// <param name="reader">A <see cref="TextReader"/> pointing to the XML to test.</param>
        /// <param name="baseURI">The base URI.</param>
        public static void XmlValid(TextReader reader, string baseURI) {
            XmlValid(new XmlInput(reader, baseURI));
        }


        /// <summary>
        /// Asserts that some XML is valid.
        /// </summary>
        /// <param name="xmlInput">The XML input.</param>
        public static void XmlValid(XmlInput xmlInput) {
            Validator validator = new Validator(xmlInput);
            XmlValid(validator);
        }


        /// <summary>
        /// Asserts that some XML is valid.
        /// </summary>
        /// <param name="validator">A <see cref="Validator"/> object containing the XML to validate</param>
        public static void XmlValid(Validator validator) {
            Assert.AreEqual(true, validator.IsValid,validator.ValidationMessage);
        }


        /// <summary>
        /// Assert that an XPath expression matches at least one node in someXml
        /// </summary>
        /// <param name="anXPathExpression">An X path expression.</param>
        /// <param name="inXml">The XML being tested.</param>
        public static void XPathExists(string anXPathExpression, string inXml) {
            XPathExists(anXPathExpression, new XmlInput(inXml));
        }


        /// <summary>
        /// Assert that an XPath expression matches at least one node in someXml
        /// </summary>
        /// <param name="anXPathExpression">An X path expression.</param>
        /// <param name="inXml">A reader ontot eh XML being tested</param>
        public static void XPathExists(string anXPathExpression, TextReader inXml) {
            XPathExists(anXPathExpression, new XmlInput(inXml));
        }


        /// <summary>
        /// Assert that an XPath expression matches at least one node in someXml
        /// </summary>
        /// <param name="anXPathExpression">An X path expression.</param>
        /// <param name="inXml">The XML to test.</param>
        public static void XPathExists(string anXPathExpression, XmlInput inXml) {
            XPath xpath = new XPath(anXPathExpression);
            Assert.AreEqual(true, xpath.XPathExists(inXml));
        }


        /// <summary>
        /// Asserts that the flattened String obtained by executing an Xpath on some XML is a particular value 
        /// </summary>
        /// <param name="anXPathExpression">An X path expression.</param>
        /// <param name="inXml">The XML to test.</param>
        /// <param name="expectedValue">The expected value.</param>
        public static void XPathEvaluatesTo(string anXPathExpression, string inXml, 
                                                  string expectedValue) {
            XPathEvaluatesTo(anXPathExpression, new XmlInput(inXml), expectedValue);
        }

        /// <summary>
        /// Asserts that the flattened String obtained by executing an Xpath on some XML is a particular value 
        /// </summary>
        /// <param name="anXPathExpression">An X path expression.</param>
        /// <param name="inXml">The XML to test.</param>
        /// <param name="expectedValue">The expected value.</param>
        public static void XPathEvaluatesTo(string anXPathExpression, TextReader inXml, 
                                                  string expectedValue) {
            XPathEvaluatesTo(anXPathExpression, new XmlInput(inXml), expectedValue);
        }

        /// <summary>
        /// Asserts that the flattened String obtained by executing an Xpath on some XML is a particular value 
        /// </summary>
        /// <param name="anXPathExpression">An X path expression.</param>
        /// <param name="inXml">The XML to test.</param>
        /// <param name="expectedValue">The expected value.</param>
        public static void XPathEvaluatesTo(string anXPathExpression, XmlInput inXml, 
                                                  string expectedValue) {
            XPath xpath = new XPath(anXPathExpression);
            Assert.AreEqual(expectedValue, xpath.EvaluateXPath(inXml));
        }


        /// <summary>
        /// Asserts that the results of an XSL transform on some XML are the expected result
        /// </summary>
        /// <param name="xslTransform">The XSL transform.</param>
        /// <param name="xmlToTransform">The XML to transform.</param>
        /// <param name="expectedResult">The expected result.</param>
        public static void XslTransformResults(string xslTransform, string xmlToTransform, string expectedResult) {
            XmlInput xsl = new XmlInput(xslTransform);
            XmlInput xml2 = new XmlInput(xmlToTransform);
            XmlInput xmlEx = new XmlInput(expectedResult);
        	XslTransformResults(xsl, xml2, xmlEx);
        }


        /// <summary>
        /// Asserts that the results of an XSL transform on some XML are the expected result
        /// </summary>
        /// <param name="xslTransform">The XSL transform.</param>
        /// <param name="xmlToTransform">The XML to transform.</param>
        /// <param name="expectedResult">The expected result.</param>
        public static void XslTransformResults(XmlInput xslTransform, XmlInput xmlToTransform, XmlInput expectedResult) {
        	Xslt xslt = new Xslt(xslTransform);
        	XmlOutput output = xslt.Transform(xmlToTransform);
        	XmlEquals(expectedResult, output.AsXml());
        }

    }
}
