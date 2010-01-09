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

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Common.Xml;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Assert = MbUnit.Framework.Assert;


namespace Gallio.Tests.Common.Collections
{
    [TestFixture]
    [TestsOn(typeof(PropertySet))]
    public class PropertySetTest
    {
        private readonly XmlSerializer serializer = new XmlSerializer(typeof(PropertySet));

        [VerifyContract]
        public readonly IContract Equality = new EqualityContract<PropertySet>()
        {
            ImplementsOperatorOverloads = false,
            EquivalenceClasses =
            {
                { new PropertySet() },
                { new PropertySet() { { "key", "value" } }},
                { new PropertySet() { { "key", "other value" } }},
                { new PropertySet() { { "key", "value" }, { "other key", "value" } }},
                { new PropertySet() { { "key", "value" }, { "other key", "other value" } }},
            }
        };

        [Test]
        public void Copy()
        {
            PropertySet original = new PropertySet();
            original.Add("abc", "123");
            original.Add("def", "");

            PropertySet copy = original.Copy();

            Assert.AreNotSame(original, copy);
            AssertAreEqual(original, copy);
        }

        [Test]
        [Row(@"<?xml version=""1.0"" encoding=""utf-16""?>
<propertySet xmlns=""http://www.gallio.org/"" />",
            new string[] { })]
        [Row(@"<?xml version=""1.0"" encoding=""utf-16""?>
<propertySet xmlns=""http://www.gallio.org/"">
  <entry key=""a"" value=""1"" />
  <entry key=""b"" value=""2"" />
</propertySet>",
            new string[] { "a", "1", "b", "2" })]
        public void SerializeToXml(string expectedXml, string[] keyValuePairs)
        {
            PropertySet map = new PropertySet();
            for (int i = 0; i < keyValuePairs.Length; i += 2)
                map.Add(keyValuePairs[i], keyValuePairs[i + 1]);

            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, map);

            Assert.AreEqual(expectedXml, writer.ToString());
        }

        [Test]
        [Row(@"<?xml version=""1.0"" encoding=""utf-16""?>
<propertySet xmlns=""http://www.gallio.org/"" />",
            new string[] { })]
        [Row(@"<?xml version=""1.0"" encoding=""utf-16""?>
<propertySet xmlns=""http://www.gallio.org/"">
  <entry key=""a"" value=""1"" />
  <entry key=""b"" value=""2"" />
</propertySet>",
            new string[] { "a", "1", "b", "2" })]
        [Row(@"<?xml version=""1.0"" encoding=""utf-16""?>
<propertySet xmlns=""http://www.gallio.org/"">
  <entry key=""a"" value=""""/>
</propertySet>",
            new string[] { "a", "" })]
        public void DeserializeFromXml(string xml, string[] expectedKeyValuePairs)
        {
            PropertySet expectedMap = new PropertySet();
            for (int i = 0; i < expectedKeyValuePairs.Length; i += 2)
                expectedMap.Add(expectedKeyValuePairs[i], expectedKeyValuePairs[i + 1]);

            PropertySet actualMap = (PropertySet) serializer.Deserialize(new StringReader(xml));
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
            container.Metadata = new PropertySet();
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
            PropertySet original = new PropertySet();
            original.Add("abc", "123");

            PropertySet readOnly = original.AsReadOnly();
            Assert.IsTrue(readOnly.IsReadOnly);
            AssertAreEqual(original, readOnly);

            MbUnit.Framework.Assert.Throws<NotSupportedException>(delegate { readOnly.Add("def", "456"); });
        }

        [Test]
        public void GetAndSetValue()
        {
            PropertySet set = new PropertySet();

            Assert.IsNull(set.GetValue("key"));

            set.SetValue("key", "value");
            Assert.AreEqual("value", set.GetValue("key"));

            set.SetValue("key", "different value");
            Assert.AreEqual("different value", set.GetValue("key"));

            set.SetValue("key", null);
            Assert.IsNull(set.GetValue("key"));

            set.Add("key", "value1");
            Assert.AreEqual("value1", set.GetValue("key"));
        }

        private static void AssertAreEqual(PropertySet expected, PropertySet actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);

            foreach (KeyValuePair<string, string> entry in expected)
                Assert.IsTrue(actual.Contains(entry));
        }

        [XmlRoot("propertySetContainer", Namespace = SchemaConstants.XmlNamespace)]
        [XmlType(Namespace = SchemaConstants.XmlNamespace)]
        public class MetadataContainer
        {
            [XmlElement("metadata")]
            public PropertySet Metadata;

            [XmlElement("following")]
            public int FollowingElement;
        }
    }
}
