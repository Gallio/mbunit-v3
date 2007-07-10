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
//		MbUnit HomePage: http://www.mbunit.org
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
    
    public sealed class XmlAssert{
		private XmlAssert()
		{}

        public static void XmlEquals(TextReader controlTextReader, TextReader testTextReader) {
            XmlEquals(new XmlDiff(controlTextReader, testTextReader));
        }

        public static void XmlEquals(string controlText, string testText) {
           XmlEquals(new XmlDiff(controlText, testText));
        }

        public static void XmlEquals(XmlInput controlInput, XmlInput testInput) {
            XmlEquals(new XmlDiff(controlInput, testInput));
        }        
        
        public static void XmlIdentical(TextReader controlTextReader, TextReader testTextReader) {
            XmlIdentical(new XmlDiff(controlTextReader, testTextReader));
        }        

        public static void XmlIdentical(string controlText, string testText) {
            XmlIdentical(new XmlDiff(controlText, testText));
        }        
        
        public static void XmlIdentical(XmlInput controlInput, XmlInput testInput) {
            XmlIdentical(new XmlDiff(controlInput, testInput));
        }        
        
        public static void XmlEquals(XmlDiff xmlDiff) {
            XmlEquals(xmlDiff, true);
        }
        
        public static void XmlNotEquals(XmlDiff xmlDiff) {
            XmlEquals(xmlDiff, false);
        }

        private static void XmlEquals(XmlDiff xmlDiff, bool equal) 
		{
			if (equal)
				xmlDiff.Compare();
			else
			{
				try
				{
					xmlDiff.Compare();
				}
				catch(Exception ex)
				{
					if(ex.GetType()!=typeof(FlowControlException))
						throw;
				}
			}
        }
        
        public static void XmlIdentical(XmlDiff xmlDiff) {
            XmlIdentical(xmlDiff, true);
        }
        
        public static void XmlNotIdentical(XmlDiff xmlDiff) {
            XmlIdentical(xmlDiff, false);
        }
        
        private static void XmlIdentical(XmlDiff xmlDiff, bool identical) {
            DiffResult diffResult = xmlDiff.Compare();
            Assert.AreEqual(identical, diffResult.Identical,xmlDiff.OptionalDescription);
        }
        
        public static void XmlValid(string someXml) {
            XmlValid(new XmlInput(someXml));
        }
        
        public static void XmlValid(string someXml, string baseURI) {
            XmlValid(new XmlInput(someXml, baseURI));
        }
        
        public static void XmlValid(TextReader reader) {
            XmlValid(new XmlInput(reader));
        }
        
        public static void XmlValid(TextReader reader, string baseURI) {
            XmlValid(new XmlInput(reader, baseURI));
        }
        
        public static void XmlValid(XmlInput xmlInput) {
            Validator validator = new Validator(xmlInput);
            XmlValid(validator);
        }
        
        public static void XmlValid(Validator validator) {
            Assert.AreEqual(true, validator.IsValid,validator.ValidationMessage);
        }
        
        public static void XPathExists(string anXPathExpression, string inXml) {
            XPathExists(anXPathExpression, new XmlInput(inXml));
        }
        
        public static void XPathExists(string anXPathExpression, TextReader inXml) {
            XPathExists(anXPathExpression, new XmlInput(inXml));
        }
        
        public static void XPathExists(string anXPathExpression, XmlInput inXml) {
            XPath xpath = new XPath(anXPathExpression);
            Assert.AreEqual(true, xpath.XPathExists(inXml));
        }
        
        public static void XPathEvaluatesTo(string anXPathExpression, string inXml, 
                                                  string expectedValue) {
            XPathEvaluatesTo(anXPathExpression, new XmlInput(inXml), expectedValue);
        }
        
        public static void XPathEvaluatesTo(string anXPathExpression, TextReader inXml, 
                                                  string expectedValue) {
            XPathEvaluatesTo(anXPathExpression, new XmlInput(inXml), expectedValue);
        }
                                                  
        public static void XPathEvaluatesTo(string anXPathExpression, XmlInput inXml, 
                                                  string expectedValue) {
            XPath xpath = new XPath(anXPathExpression);
            Assert.AreEqual(expectedValue, xpath.EvaluateXPath(inXml));
        }
        
        public static void XslTransformResults(string xslTransform, string xmlToTransform, string expectedResult) {
            XmlInput xsl = new XmlInput(xslTransform);
            XmlInput xml2 = new XmlInput(xmlToTransform);
            XmlInput xmlEx = new XmlInput(expectedResult);
        	XslTransformResults(xsl, xml2, xmlEx);
        }
        
        public static void XslTransformResults(XmlInput xslTransform, XmlInput xmlToTransform, XmlInput expectedResult) {
        	Xslt xslt = new Xslt(xslTransform);
        	XmlOutput output = xslt.Transform(xmlToTransform);
        	XmlEquals(expectedResult, output.AsXml());
        }

    }
}
