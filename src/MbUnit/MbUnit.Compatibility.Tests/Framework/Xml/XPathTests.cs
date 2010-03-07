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
    using MbUnit.Framework;
    using MbUnit.Framework.Xml;
    
    [TestFixture]
    public class XpathTests {
        private static readonly string SIMPLE_XML = "<a><b><c>one two</c></b></a>";
        private static readonly string EXISTENT_XPATH = "/a/b/c";
        private static readonly string NONEXISTENT_XPATH = "/a/b/c/d";
        
        private static readonly string MORE_COMPLEX_XML = "<a><b>one</b><b>two</b></a>";
        private static readonly string MULTI_NODE_XPATH = "//b";
        private static readonly string COUNT_XPATH = "count(//b)";
        [Test] public void XpathExistsTrueForXpathThatExists() {
            XPath xpath = new XPath(EXISTENT_XPATH);
            OldAssert.AreEqual(true, 
                                   xpath.XPathExists(SIMPLE_XML));
        }
        
        [Test] public void XpathExistsFalseForUnmatchedExpression() {
            XPath xpath = new XPath(NONEXISTENT_XPATH);
            OldAssert.AreEqual(false, 
                                   xpath.XPathExists(SIMPLE_XML));
        }
        
        [Test] public void XpathEvaluatesToTextValueForSimpleString() {
            string expectedValue = "one two";
            XPath xpath = new XPath(EXISTENT_XPATH);
            OldAssert.AreEqual(expectedValue, 
                                   xpath.EvaluateXPath(SIMPLE_XML));
        }
        
        [Test] public void XpathEvaluatesToEmptyStringForUnmatchedExpression() {
            string expectedValue = "";
            XPath xpath = new XPath(NONEXISTENT_XPATH);
            OldAssert.AreEqual(expectedValue, 
                                   xpath.EvaluateXPath(SIMPLE_XML));
        }
        [Test] public void XpathEvaluatesCountExpression() {
            string expectedValue = "2";
            XPath xpath = new XPath(COUNT_XPATH);
            OldAssert.AreEqual(expectedValue, 
                                   xpath.EvaluateXPath(MORE_COMPLEX_XML));
        }
        [Test] public void XpathEvaluatesMultiNodeExpression() {
            string expectedValue = "onetwo";
            XPath xpath = new XPath(MULTI_NODE_XPATH);
            OldAssert.AreEqual(expectedValue, 
                                   xpath.EvaluateXPath(MORE_COMPLEX_XML));
        }
    }
}
