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

using MbUnit.Framework.Xml;

namespace MbUnit.Framework.Tests.Xml
{
    [TestFixture]
    [TestsOn(typeof(XmlSerializationAssert))]
    public class XmlSerializationAssertTest
    {
        #region IsXmlSerializable
        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void IsXmlSerilizableNullType()
        {
            XmlSerializationAssert.IsXmlSerializable(null);
        }

        [Test]
        public void IsXmlSerilizable()
        {
            XmlSerializationAssert.IsXmlSerializable(typeof(SerializableClass));
        }
        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void IsXmlSerilizableFail()
        {
            XmlSerializationAssert.IsXmlSerializable(typeof(NotSerializableClass));
        }
        #endregion

        /*
        #region OneWaySerialization
        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void OneWaySerializationNullType()
        {
            SerialAssert.OneWaySerialization(null);      
        }
        [Test]
        [Ignore("Problem due to separate AppDomain")]
        public void OneWaySerialization()
        {
            SerialAssert.OneWaySerialization(new SerializableClass());      
        }
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void OneWaySerializationFail()
        {
            SerialAssert.OneWaySerialization(new NotSerializableClass("hello"));      
        }
        #endregion

        #region TwoWaySerialization
        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void TwoWaySerializationNullType()
        {
            SerialAssert.OneWaySerialization(null);      
        }
        [Test]
        [Ignore("Problem due to separate AppDomain")]
        public void TwoWaySerialization()
        {
            SerialAssert.TwoWaySerialization(new SerializableClass());      
        }
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TwoWaySerializationFail()
        {
            SerialAssert.TwoWaySerialization(new NotSerializableClass("hello"));      
        }
        #endregion
         */

        #region DummyClasses
        public class NotSerializableClass
        {
            public NotSerializableClass(string s)
            { }
        }

        public class SerializableClass
        {
            public string Name = "Marc";
            public string LastName = "Paul";
        }
        #endregion
    }
}