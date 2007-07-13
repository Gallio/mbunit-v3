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
    
    [TestFixture]
    public class XsltTests {
        public static readonly string XML_DECLARATION =
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            
        public static readonly string XSLT_START =
            "<xsl:stylesheet xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"1.0\">";
    
        public static readonly string XSLT_TEXT_OUTPUT_NOINDENT =
            "<xsl:output method=\"xml\" encoding=\"UTF-8\" omit-xml-declaration=\"yes\" indent=\"no\"/>";
    
        public static readonly string XSLT_IDENTITY_TEMPLATE =
            "<xsl:template match=\"/\"><xsl:copy-of select=\".\"/></xsl:template>";
    
        public static readonly string XSLT_END = "</xsl:stylesheet>";
    	
        public static readonly string IDENTITY_TRANSFORM = XML_DECLARATION
                + XSLT_START + XSLT_TEXT_OUTPUT_NOINDENT
                + XSLT_IDENTITY_TEMPLATE
                + XSLT_END;
        
        [Test] public void CanPerformTransform() {
            Xslt xslt = new Xslt(IDENTITY_TRANSFORM);
            string input = "<qwerty>uiop</qwerty>";
            string output = new string(input.ToCharArray());
            Assert.AreEqual(output, xslt.Transform(input).AsString());
            Assert.AreEqual(output, xslt.Transform(input).AsString());
        }                      
    }
}
