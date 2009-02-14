// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
            AssertionFailure[] failures = Capture(() => Assert.IsXmlSerializableType(typeof(NotXmlSerializableClass)));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the type to support Xml serialization but an exception was thrown while constructing an XmlSerializer.", failures[0].Description);
        }

        [Test]
        public void IsXmlSerializableType_passes_if_type_is_serializable()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsXmlSerializableType(typeof(XmlSerializableClass)));
            Assert.AreEqual(0, failures.Length);
        }

        [Test, Pending]
        public void XmlSerialize()
        {
        }

        [Test, Pending]
        public void XmlDeserialize()
        {
        }

        [Test, Pending]
        public void XmlSerializeThenDeserialize()
        {
        }

        [Test]
        public void IsBinarySerializableType_throws_if_type_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => Assert.IsBinarySerializableType(null));
        }

        [Test]
        public void IsBinarySerializableType_fails_if_type_is_non_serializable()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsBinarySerializableType(typeof(NotBinarySerializableClass)));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the type to support binary serialization but it lacks the [Serializable] attribute.", failures[0].Description);
        }

        [Test]
        public void IsBinarySerializableType_passes_if_type_is_serializable()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsBinarySerializableType(typeof(BinarySerializableClass)));
            Assert.AreEqual(0, failures.Length);
        }

        [Test, Pending]
        public void BinarySerialize()
        {
        }

        [Test, Pending]
        public void BinaryDeserialize()
        {
        }

        [Test, Pending]
        public void BinarySerializeThenDeserialize()
        {
        }

        #region DummyClasses
        public class NotXmlSerializableClass
        {
            public NotXmlSerializableClass(string s)
            { }
        }

        public class XmlSerializableClass
        {
            public string Name = "Marc";
            public string LastName = "Paul";
        }

        public class NotBinarySerializableClass
        {
        }

        [Serializable]
        public class BinarySerializableClass
        {
            public string Name = "Marc";
            public string LastName = "Paul";
        }
        #endregion
    }
}