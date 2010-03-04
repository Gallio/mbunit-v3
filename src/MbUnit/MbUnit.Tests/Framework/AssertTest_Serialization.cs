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
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using Gallio.Framework.Assertions;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(Assert))]
    public class AssertTest_Serialization : BaseAssertTest
    {
        [Test]
        public void IsXmlSerializableType_throws_if_type_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => Assert.IsXmlSerializableType(null));
        }

        [Test]
        public void IsXmlSerializableType_fails_if_type_is_non_serializable()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsXmlSerializableType(typeof(NonXmlSerializableClass)));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the type to support Xml serialization but an exception was thrown while constructing an XmlSerializer.", failures[0].Description);
        }

        [Test]
        public void IsXmlSerializableType_passes_if_type_is_serializable()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsXmlSerializableType(typeof(XmlSerializableClass)));
            Assert.AreEqual(0, failures.Length);
        }

        [Test]
        public void XmlSerialize_fails_if_value_is_null()
        {
            AssertionFailure[] failures = Capture(() => Assert.XmlSerialize(null));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Could not serialize the value because it is null.", failures[0].Description);
        }

        [Test]
        public void XmlSerialize_fails_if_value_is_not_serializable()
        {
            AssertionFailure[] failures = Capture(() => Assert.XmlSerialize(new NonXmlSerializableClass(null)));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Could not serialize the value.", failures[0].Description);
        }

        [Test]
        public void XmlSerialize_passes_and_returns_xml_if_serializable()
        {
            string xml = Assert.XmlSerialize(new XmlSerializableClass() { Token = "Abc" });
            Assert.Xml.AreEqual(
                "<XmlSerializableClass xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Token>Abc</Token></XmlSerializableClass>",
                xml);
        }

        [Test]
        public void XmlDeserialize_fails_if_xml_is_null()
        {
            AssertionFailure[] failures = Capture(() => Assert.XmlDeserialize<XmlSerializableClass>(null));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Could not deserialize the value because the xml is null.", failures[0].Description);
        }

        [Test]
        public void XmlDeserialize_fails_if_value_is_not_deserializable()
        {
            AssertionFailure[] failures = Capture(() => Assert.XmlDeserialize<XmlSerializableClass>("<Root/>"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Could not deserialize the value.", failures[0].Description);
        }

        [Test]
        public void XmlDeserialize_passes_and_returns_value_if_deserializable()
        {
            var value = Assert.XmlDeserialize<XmlSerializableClass>("<XmlSerializableClass xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Token>Abc</Token></XmlSerializableClass>");
            Assert.AreEqual("Abc", value.Token);
        }

        [Test]
        public void XmlSerializeThenDeserialize_fails_if_value_is_null()
        {
            AssertionFailure[] failures = Capture(() => Assert.XmlSerializeThenDeserialize<object>(null));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Could not serialize the value because it is null.", failures[0].Description);
        }

        [Test]
        public void XmlSerializeThenDeserialize_fails_if_value_is_not_serializable()
        {
            AssertionFailure[] failures = Capture(() => Assert.XmlSerializeThenDeserialize(new NonXmlSerializableClass(null)));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Could not serialize the value.", failures[0].Description);
        }

        [Test]
        public void XmlSerializeThenDeserialize_passes_and_returns_value_if_serializable()
        {
            var value = Assert.XmlSerializeThenDeserialize(new XmlSerializableClass() { Token = "Abc" });
            Assert.AreEqual("Abc", value.Token);
        }

        [Test]
        public void IsSerializableType_throws_if_type_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => Assert.IsSerializableType(null));
        }

        [Test]
        public void IsSerializableType_fails_if_type_is_non_serializable()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsSerializableType(typeof(NonSerializableClass)));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the type to support binary serialization but it lacks the [Serializable] attribute.", failures[0].Description);
        }

        [Test]
        public void IsSerializableType_passes_if_type_is_serializable()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsSerializableType(typeof(SerializableClass)));
            Assert.AreEqual(0, failures.Length);
        }

        [Test]
        public void IsSerializableType_fails_if_type_is_custom_serializable_but_missing_deserialization_constructor()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsSerializableType(typeof(CustomBinarySerializableClassMissingDeserializationConstructor)));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the type to support binary serialization but it implements ISerializable and is missing a deserialization constructor with signature .ctor(SerializationInfo, StreamingContext).", failures[0].Description);
        }

        [Test]
        public void IsSerializableType_passes_if_type_is_custom_serializable_and_has_deserialization_constructor()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsSerializableType(typeof(CustomBinarySerializableClass)));
            Assert.AreEqual(0, failures.Length);
        }

        [Test]
        public void Serialize_throws_if_formatter_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => Assert.Serialize("value", null));
        }

        [Test]
        public void Serialize_fails_if_value_is_null()
        {
            AssertionFailure[] failures = Capture(() => Assert.Serialize(null, CreateObjectSerializationFormatter()));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Could not serialize the value because it is null.", failures[0].Description);
        }

        [Test]
        public void Serialize_fails_if_value_is_not_serializable()
        {
            AssertionFailure[] failures = Capture(() => Assert.Serialize(new NonSerializableClass(), CreateObjectSerializationFormatter()));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Could not serialize the value.", failures[0].Description);
        }

        [Test]
        public void Serialize_passes_and_returns_stream_if_serializable()
        {
            MemoryStream stream = Assert.Serialize(new SerializableClass() { Token = "Abc" }, CreateObjectSerializationFormatter());
            Assert.AreEqual(0, stream.Position);
            Assert.AreNotEqual(0, stream.Length);
        }

        [Test]
        public void Deserialize_throws_if_formatter_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => Assert.Deserialize<SerializableClass>(new MemoryStream(), null));
        }

        [Test]
        public void Deserialize_fails_if_stream_is_null()
        {
            AssertionFailure[] failures = Capture(() => Assert.Deserialize<SerializableClass>(null, CreateObjectSerializationFormatter()));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Could not deserialize the value because the stream is null.", failures[0].Description);
        }

        [Test]
        public void Deserialize_fails_if_value_is_not_deserializable()
        {
            AssertionFailure[] failures = Capture(() => Assert.Deserialize<SerializableClass>(new MemoryStream(), CreateObjectSerializationFormatter()));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Could not deserialize the value.", failures[0].Description);
        }

        [Test]
        public void Deserialize_passes_and_returns_value_if_deserializable()
        {
            MemoryStream stream = Assert.Serialize(new SerializableClass() { Token = "Abc" }, CreateObjectSerializationFormatter());

            var value = Assert.Deserialize<SerializableClass>(stream, CreateObjectSerializationFormatter());
            Assert.AreEqual("Abc", value.Token);
        }

        [Test]
        public void SerializeThenDeserialize_throws_if_formatter_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => Assert.SerializeThenDeserialize("value", null));
        }

        [Test]
        public void SerializeThenDeserialize_fails_if_value_is_null()
        {
            AssertionFailure[] failures = Capture(() => Assert.SerializeThenDeserialize<object>(null, CreateObjectSerializationFormatter()));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Could not serialize the value because it is null.", failures[0].Description);
        }

        [Test]
        public void SerializeThenDeserialize_fails_if_value_is_not_serializable()
        {
            AssertionFailure[] failures = Capture(() => Assert.SerializeThenDeserialize(new NonSerializableClass(), CreateObjectSerializationFormatter()));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Could not serialize the value.", failures[0].Description);
        }

        [Test]
        public void SerializeThenDeserialize_passes_and_returns_value_if_serializable()
        {
            var value = Assert.SerializeThenDeserialize(new SerializableClass() { Token = "Abc" }, CreateObjectSerializationFormatter());
            Assert.AreEqual("Abc", value.Token);
        }

        [Test]
        public void BinarySerialize_fails_if_value_is_null()
        {
            AssertionFailure[] failures = Capture(() => Assert.BinarySerialize(null));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Could not serialize the value because it is null.", failures[0].Description);
        }

        [Test]
        public void BinarySerialize_fails_if_value_is_not_serializable()
        {
            AssertionFailure[] failures = Capture(() => Assert.BinarySerialize(new NonSerializableClass()));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Could not serialize the value.", failures[0].Description);
        }

        [Test]
        public void BinarySerialize_passes_and_returns_stream_if_serializable()
        {
            MemoryStream stream = Assert.BinarySerialize(new SerializableClass() { Token = "Abc" });
            Assert.AreEqual(0, stream.Position);
            Assert.AreNotEqual(0, stream.Length);
        }

        [Test]
        public void BinaryDeserialize_fails_if_stream_is_null()
        {
            AssertionFailure[] failures = Capture(() => Assert.BinaryDeserialize<SerializableClass>(null));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Could not deserialize the value because the stream is null.", failures[0].Description);
        }

        [Test]
        public void BinaryDeserialize_fails_if_value_is_not_deserializable()
        {
            AssertionFailure[] failures = Capture(() => Assert.BinaryDeserialize<SerializableClass>(new MemoryStream()));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Could not deserialize the value.", failures[0].Description);
        }

        [Test]
        public void BinaryDeserialize_passes_and_returns_value_if_deserializable()
        {
            MemoryStream stream = Assert.BinarySerialize(new SerializableClass() { Token = "Abc" });

            var value = Assert.BinaryDeserialize<SerializableClass>(stream);
            Assert.AreEqual("Abc", value.Token);
        }

        [Test]
        public void BinarySerializeThenDeserialize_fails_if_value_is_null()
        {
            AssertionFailure[] failures = Capture(() => Assert.BinarySerializeThenDeserialize<object>(null));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Could not serialize the value because it is null.", failures[0].Description);
        }

        [Test]
        public void BinarySerializeThenDeserialize_fails_if_value_is_not_serializable()
        {
            AssertionFailure[] failures = Capture(() => Assert.BinarySerializeThenDeserialize(new NonSerializableClass()));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Could not serialize the value.", failures[0].Description);
        }

        [Test]
        public void BinarySerializeThenDeserialize_passes_and_returns_value_if_serializable()
        {
            var value = Assert.BinarySerializeThenDeserialize(new SerializableClass() { Token = "Abc" });
            Assert.AreEqual("Abc", value.Token);
        }

        #region Test classes
        public class NonXmlSerializableClass
        {
            public NonXmlSerializableClass(string s)
            { }
        }

        public class XmlSerializableClass
        {
            public string Token = "Value";
        }

        public class NonSerializableClass
        {
        }

        [Serializable]
        public class SerializableClass
        {
            public string Token = "Value";
        }

        [Serializable]
        public class CustomBinarySerializableClass : ISerializable
        {
            public string Token = "Value";

            public CustomBinarySerializableClass()
            {
            }

            public CustomBinarySerializableClass(SerializationInfo info, StreamingContext context)
            {
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
            }
        }

        [Serializable]
        public class CustomBinarySerializableClassMissingDeserializationConstructor : ISerializable
        {
            public string Token = "Value";

            public CustomBinarySerializableClassMissingDeserializationConstructor()
            {
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
            }
        }
        #endregion

        private static IFormatter CreateObjectSerializationFormatter()
        {
            return new BinaryFormatter();
        }
    }
}