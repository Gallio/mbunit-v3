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
