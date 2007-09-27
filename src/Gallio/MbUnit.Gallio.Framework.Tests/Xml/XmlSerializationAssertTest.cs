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