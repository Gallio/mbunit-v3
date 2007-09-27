using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using MbUnit.Framework.Reflection;

namespace MbUnit.Framework.Tests.Reflection
{
    [TestFixture]
    public class ReflectorTest
    {
        Reflector _reflector = null;

        [SetUp]
        public void Init()
        {
            SampleClass sc = new SampleClass();
            _reflector = new Reflector(sc);
        }

        [Test]
        public void PrivateTest()
        {
            object result = _reflector.RunPrivateMethod("PrivateIntWithNoParam");
            Assert.AreEqual(5, result);
        }

        [Test]
        public void PrivateTestWithNullTest()
        {
            object result = _reflector.RunPrivateMethod("PrivateIntWithNoParam", null);
            Assert.AreEqual(5, result);
        }

        [Test]
        public void PrivateTestWithIntParamTest()
        {
            object result = _reflector.RunPrivateMethod("PrivateIntWithIntParam", 5);
            Assert.AreEqual(5, result);
        }

        [Test]
        public void PrivateOverloadedTests()
        {
            string result = (string)_reflector.RunPrivateMethod("PrivateOverloaded");
            Assert.AreEqual("Test", result);

            string result2 = (string)_reflector.RunPrivateMethod("PrivateOverloaded", result);
            Assert.AreEqual(result, result2);
        }

        [Test]
        public void PrivateVoidWithNoParamTest()
        {
            object result = _reflector.RunPrivateMethod("PrivateVoidWithNoParam");
            Assert.IsNull(result);
        }

        [Test]
        public void PrivateIntWithDoubleParamTest()
        {
            object result = _reflector.RunPrivateMethod("PrivateIntWithDoubleParam", 5.05);
            Assert.AreEqual(5, result);
        }

        [Test]
        public void PrivateStaticBoolWithBoolParamTest()
        {
            bool result = (bool)_reflector.RunPrivateMethod("PrivateStaticBoolWithBoolParam", false);
            Assert.IsFalse(result);
        }

        [Test]
        public void InternalIntWithIntParamTest()
        {
            object result = _reflector.RunPrivateMethod("InternalIntWithIntParam", 25);
            Assert.AreEqual(25, result);
        }

        [Test]
        public void PrivateIntFieldTest()
        {
            object result = _reflector.GetNonPublicField("privateInt");
            Assert.AreEqual(7, result);
        }

        [Test]
        public void PrivateStringPropertyTest()
        {
            object result = _reflector.GetNonPublicProperty("PrivateStringProperty");
            Assert.AreEqual("Test", result);
        }
    }
}
