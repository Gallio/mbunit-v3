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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Gallio.Common.Markup;
using Gallio.Common.Markup.Tags;
using Gallio.Common.Text.RegularExpression;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Tests.Common.Markup.Tags
{
    public class TextTagTest : BaseTagTest<TextTag>
    {
        [VerifyContract]
        public readonly IContract EqualityTests = new EqualityContract<TextTag>
        {
            ImplementsOperatorOverloads = false,
            EquivalenceClasses = equivalenceClasses
        };

        [VerifyContract]
        public readonly IContract HashCodeTests = new HashCodeAcceptanceContract<TextTag>
        {
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances = DataGenerators.Strings.Random(100000, @"[A-Za-z0-9 ]{4,100}").Select(text => new TextTag(text)),
        };

        public override EquivalenceClassCollection<TextTag> GetEquivalenceClasses()
        {
            return equivalenceClasses;
        }

        private static readonly EquivalenceClassCollection<TextTag> equivalenceClasses = new EquivalenceClassCollection<TextTag>
        {
            new TextTag(""),
            new TextTag("text"),
            new TextTag("other"),
            new TextTag("   \nsomething\nwith  embedded  newlines and significant whitespace to\nencode\n  ")
        };

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfTextIsNull()
        {
            new TextTag(null);
        }

        [Test]
        [Row(
            "",
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<text xmlns=\"http://www.gallio.org/\" />",
            Description = "Empty text")]
        [Row(
            " ",
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<text xmlns=\"http://www.gallio.org/\"><![CDATA[ ]]></text>",
            Description = "Single space")]
        [Row(
            "\n",
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<text xmlns=\"http://www.gallio.org/\"><![CDATA[\n]]></text>",
            Description = "Single newline")]
        [Row(
            "text",
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<text xmlns=\"http://www.gallio.org/\"><![CDATA[text]]></text>",
            Description = "Single word")]
        [Row(
            "abc ]]> def ]]> ghi",
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<text xmlns=\"http://www.gallio.org/\"><![CDATA[abc ]]>]]&gt;<![CDATA[ def ]]>]]&gt;<![CDATA[ ghi]]></text>",
            Description = "Embedded CDATA terminator")]
        public void RoundTripToXml(string text, string expectedXml)
        {
            var originalTag = new TextTag(text);
            var serializer = new XmlSerializer(typeof(TextTag));
            var writer = new StringWriter();
            serializer.Serialize(writer, originalTag);
            string xml = writer.ToString();
            Assert.AreEqual(expectedXml, xml);
            var deserializedTag = (TextTag)serializer.Deserialize(new StringReader(xml));
            Assert.AreEqual(text, deserializedTag.Text);
        }
    }
}
