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
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Gallio.Collections;
using Gallio.Utilities;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Assert = MbUnit.Framework.Assert;


namespace Gallio.Tests.Collections
{
    [TestFixture]
    [TestsOn(typeof(PropertyBag))]
    public class PropertyBagTest
    {
        private readonly XmlSerializer serializer = new XmlSerializer(typeof(PropertyBag));

        [VerifyContract]
        public readonly IContract Equality = new EqualityContract<PropertyBag>()
        {
            ImplementsOperatorOverloads = false,
            EquivalenceClasses =
            {
                { new PropertyBag() },
                { new PropertyBag() { { "key", "value" } }},
                { new PropertyBag() { { "key", "other value" } }},
                { new PropertyBag() { { "key", "value" }, { "key", "value2" } }, new PropertyBag() { { "key", "value2" }, { "key", "value" } }}, // ignores order
                { new PropertyBag() { { "key", "value" }, { "other key", "value" }, { "key", "value2" } }},
                { new PropertyBag() { { "key", "value" }, { "other key", "value" } }},
                { new PropertyBag() { { "key", "value" }, { "other key", "other value" } }},
            }
        };

        [Test]
        public void Copy()
        {
            PropertyBag original = new PropertyBag();
            original.Add("abc", "123");
            original.Add("abc", "456");
            original.Add("def", "");

            PropertyBag copy = original.Copy();

            Assert.AreNotSame(original, copy);
            AssertAreEqual(original, copy);
        }

        [Test]
        [Row(@"<?xml version=""1.0"" encoding=""utf-16""?>
<propertyBag xmlns=""http://www.gallio.org/"" />",
            new string[] { })]
        [Row(@"<?xml version=""1.0"" encoding=""utf-16""?>
<propertyBag xmlns=""http://www.gallio.org/"">
  <entry key=""a"">
    <value>1</value>
  </entry>
  <entry key=""b"">
    <value>2</value>
    <value>22</value>
  </entry>
</propertyBag>",
            new string[] { "a", "1", "b", "2", "b", "22" })]
        public void SerializeToXml(string expectedXml, string[] keyValuePairs)
        {
            PropertyBag map = new PropertyBag();
            for (int i = 0; i < keyValuePairs.Length; i += 2)
                map.Add(keyValuePairs[i], keyValuePairs[i + 1]);

            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, map);

            Assert.AreEqual(expectedXml, writer.ToString());
        }

        [Test]
        [Row(@"<?xml version=""1.0"" encoding=""utf-16""?>
<propertyBag xmlns=""http://www.gallio.org/"" />",
            new string[] { })]
        [Row(@"<?xml version=""1.0"" encoding=""utf-16""?>
<propertyBag xmlns=""http://www.gallio.org/"">
  <entry key=""a"">
    <value>1</value>
  </entry>
  <entry key=""b"">
    <value>2</value>
    <value>22</value>
  </entry>
</propertyBag>",
            new string[] { "a", "1", "b", "2", "b", "22" })]
        [Row(@"<?xml version=""1.0"" encoding=""utf-16""?>
<propertyBag xmlns=""http://www.gallio.org/"">
  <entry key=""a"" />
  <entry key=""b"">
    <value />
    <value>2</value>
  </entry>
</propertyBag>",
            new string[] { "b", "2" })]
        public void DeserializeFromXml(string xml, string[] expectedKeyValuePairs)
        {
            PropertyBag expectedMap = new PropertyBag();
            for (int i = 0; i < expectedKeyValuePairs.Length; i += 2)
                expectedMap.Add(expectedKeyValuePairs[i], expectedKeyValuePairs[i + 1]);

            PropertyBag actualMap = (PropertyBag) serializer.Deserialize(new StringReader(xml));
            AssertAreEqual(expectedMap, actualMap);
        }

        /// <summary>
        /// We had a case where the implementation of the metadata Xml reader was
        /// reading the final end element too agressively.  It was causing the
        /// remainder of the containing element to be discarded when an empty
        /// metadata section was found.
        /// </summary>
        [Test]
        public void DeserializeFromXml_RegressionTestForEmptyMetadataElement()
        {
            MetadataContainer container = new MetadataContainer();
            container.Metadata = new PropertyBag();
            container.FollowingElement = 1;

            XmlSerializer serializer = new XmlSerializer(typeof(MetadataContainer));
            StringWriter output = new StringWriter();
            serializer.Serialize(output, container);

            MetadataContainer result = (MetadataContainer)serializer.Deserialize(new StringReader(output.ToString()));
            Assert.AreEqual(0, result.Metadata.Count);
            Assert.AreEqual(1, result.FollowingElement);
        }

        [Test]
        public void AsReadOnly()
        {
            PropertyBag original = new PropertyBag();
            original.Add("abc", "123");

            PropertyBag readOnly = original.AsReadOnly();
            Assert.IsTrue(readOnly.IsReadOnly);
            AssertAreEqual(original, readOnly);

            MbUnit.Framework.Assert.Throws<NotSupportedException>(delegate { readOnly.Add("def", "456"); });
            MbUnit.Framework.Assert.Throws<NotSupportedException>(delegate { readOnly["abc"].Add("456"); });
        }

        private static void AssertAreEqual(PropertyBag expected, PropertyBag actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);

            foreach (KeyValuePair<string, IList<string>> entry in expected)
            {
                Assert.IsTrue(actual.Contains(entry));
            }
        }

        [XmlRoot("container", Namespace = XmlSerializationUtils.GallioNamespace)]
        [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
        public class MetadataContainer
        {
            [XmlElement("metadata")]
            public PropertyBag Metadata;

            [XmlElement("following")]
            public int FollowingElement;
        }
    }
}
