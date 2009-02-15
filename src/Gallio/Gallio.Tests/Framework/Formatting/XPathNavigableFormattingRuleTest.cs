// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Xml;
using System.Xml.XPath;
using Gallio.Framework.Formatting;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Formatting
{
    [TestFixture]
    [TestsOn(typeof(XPathNavigableFormattingRule))]
    public class XPathNavigableFormattingRuleTest : BaseFormattingRuleTest<XPathNavigableFormattingRule>
    {
        [Test]
        [Row("<root>abc</root>", "<root>abc</root>")]
        public void Format(XmlDocument value, string expectedResult)
        {
            Assert.AreEqual(expectedResult, Formatter.Format(value));
        }

        [Test]
        [Row(typeof(IXPathNavigable), FormattingRulePriority.Better)]
        [Row(typeof(XmlDocument), FormattingRulePriority.Better)]
        [Row(typeof(XmlNode), FormattingRulePriority.Better)]
        [Row(typeof(XPathNavigator), FormattingRulePriority.Better)]
        [Row(typeof(string), null)]
        public void GetPriority(Type type, int? expectedPriority)
        {
            Assert.AreEqual(expectedPriority, FormattingRule.GetPriority(type));
        }
    }
}