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

extern alias MbUnit2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using MbUnit.Framework.Kernel.Metadata;
using MbUnit2::MbUnit.Framework;
using Assert = MbUnit2::MbUnit.Framework.Assert;


namespace MbUnit._Framework.Tests.Kernel.Model
{
    [TestFixture]
    [TestsOn(typeof(MetadataMap))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class MetadataMapTest
    {
        private XmlSerializer serializer = new XmlSerializer(typeof(MetadataMap));

        [Test]
        public void Copy()
        {
            MetadataMap original = new MetadataMap();
            original.Entries.Add("abc", "123");
            original.Entries.Add("abc", "456");
            original.Entries.Add("def", "");

            MetadataMap copy = original.Copy();

            Assert.AreNotSame(original, copy);
            AssertAreEqual(original, copy);
        }

        [RowTest]
        [Row(@"<?xml version=""1.0"" encoding=""utf-16""?>
<metadata xmlns=""http://www.mbunit.com/gallio"" />",
            new string[] { })]
        [Row(@"<?xml version=""1.0"" encoding=""utf-16""?>
<metadata xmlns=""http://www.mbunit.com/gallio"">
  <entry key=""a"">
    <value>1</value>
  </entry>
  <entry key=""b"">
    <value>2</value>
    <value>22</value>
  </entry>
</metadata>",
            new string[] { "a", "1", "b", "2", "b", "22" })]
        public void SerializeToXml(string expectedXml, string[] keyValuePairs)
        {
            MetadataMap map = new MetadataMap();
            for (int i = 0; i < keyValuePairs.Length; i += 2)
                map.Entries.Add(keyValuePairs[i], keyValuePairs[i + 1]);

            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, map);

            Assert.AreEqual(expectedXml, writer.ToString());
        }

        [RowTest]
        [Row(@"<?xml version=""1.0"" encoding=""utf-16""?>
<metadata xmlns=""http://www.mbunit.com/gallio"" />",
            new string[] { })]
        [Row(@"<?xml version=""1.0"" encoding=""utf-16""?>
<metadata xmlns=""http://www.mbunit.com/gallio"">
  <entry key=""a"">
    <value>1</value>
  </entry>
  <entry key=""b"">
    <value>2</value>
    <value>22</value>
  </entry>
</metadata>",
            new string[] { "a", "1", "b", "2", "b", "22" })]
        [Row(@"<?xml version=""1.0"" encoding=""utf-16""?>
<metadata xmlns=""http://www.mbunit.com/gallio"">
  <entry key=""a"" />
  <entry key=""b"">
    <value />
    <value>2</value>
  </entry>
</metadata>",
            new string[] { "b", "2" })]
        public void DeserializeFromXml(string xml, string[] expectedKeyValuePairs)
        {
            MetadataMap expectedMap = new MetadataMap();
            for (int i = 0; i < expectedKeyValuePairs.Length; i += 2)
                expectedMap.Entries.Add(expectedKeyValuePairs[i], expectedKeyValuePairs[i + 1]);

            MetadataMap actualMap = (MetadataMap) serializer.Deserialize(new StringReader(xml));
            AssertAreEqual(expectedMap, actualMap);
        }

        private static void AssertAreEqual(MetadataMap expected, MetadataMap actual)
        {
            Assert.AreEqual(expected.Entries.Count, actual.Entries.Count);

            foreach (KeyValuePair<string, IList<string>> entry in expected.Entries)
            {
                Assert.IsTrue(actual.Entries.Contains(entry));
            }
        }
    }
}
