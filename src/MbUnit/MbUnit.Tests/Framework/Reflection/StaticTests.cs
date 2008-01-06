// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using MbUnit.Framework.Reflection;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework.Reflection
{
    [TestFixture]
    [TestsOn(typeof(Reflector))]
    [Author("Vadim")]
    public class ReflectorStaticTests
    {
        public static readonly string MSCorLibAssembly = typeof(Int32).Assembly.FullName;
        TestSample _sampleObject;

        [SetUp]
        public void Setup()
        {
            _sampleObject = new TestSample();    
        }

        #region Create Instance
        [Test]
        public void CreateInstanceByAssemblyNameAndClassWithDefaultConstructo()
        {
            string className = "System.Number";
            object obj = Reflector.CreateInstance(MSCorLibAssembly, className);
            Assert.IsNotNull(obj);
            Assert.AreEqual(true, Reflector.InvokeMethod(AccessModifier.Default, obj, "IsWhite", ' '));
            Assert.AreEqual(false, Reflector.InvokeMethod(AccessModifier.Default, obj, "IsWhite", 'V'));
        }

        [Test]
        public void CreateInstanceByAssemblyNameAndClassWithParametizedConstructor()
        {
            string className = "System.Collections.KeyValuePairs";
            object obj = Reflector.CreateInstance(MSCorLibAssembly, className, 1, 'A');
            Assert.IsNotNull(obj);
            Assert.AreEqual(1, Reflector.GetProperty(obj, "Key"));
            Assert.AreEqual('A', Reflector.GetProperty(obj, "Value"));
        }
        #endregion

        #region Field Tests
        [Test]
        public void GetPublicField_DefaultAccessibility()
        {
            Assert.AreEqual("MbUnit Rocks!!!", Reflector.GetField(new TestSample(), "publicString"));
        }

        [Test]
        public void GetPrivateFieldFromBaseClass()
        {
            Assert.AreEqual("Base var", Reflector.GetField(new TestSample(), "_baseString"));
        }

        [Test]
        [ExpectedException(typeof(ReflectionException))]
        public void TryToGetBaseClassFieldButSettingLookInBaseToFalse()
        {
            Reflector.GetField(AccessModifier.NonPublic, new TestSample(), "_baseString", false);
        }

        [Test]
        public void SetPrivateFieldInBaseClass()
        {
            Reflector.SetField(_sampleObject, "_baseString", "test base field");
            Assert.AreEqual("test base field", Reflector.GetField(_sampleObject, "_baseString"));
        }

        [Test]
        [ExpectedArgumentNullException()]
        public void SetPropertyWithNullObject()
        {
            Reflector.SetProperty(null, "somePropety", "value");
        }

        #endregion

        #region Property Tests
        [Test]
        public void GetPrivatePropertyFromBaseClass()
        {
            Assert.AreEqual(12, Reflector.GetProperty(new TestSample(), "BaseInteger"));
        }

        [Test]
        [ExpectedException(typeof(ReflectionException))]
        public void TryToGetBaseClassPropertyButSettingLookInBaseToFalse()
        {
            Reflector.GetProperty(AccessModifier.NonPublic, new TestSample(), "BaseInteger", false);
        }

        [Test]
        public void SetPrivatePropertyInBaseClass()
        {
            Reflector.SetProperty(_sampleObject, "BaseInteger", 7);
            Assert.AreEqual(7, Reflector.GetProperty(_sampleObject, "BaseInteger"));
        }
        #endregion

        #region InvokeMethod
        [Test]
        public void IvokePrivateMethodWithoutParametersFromBaseClass()
        {
            Assert.AreEqual("Wow!", Reflector.InvokeMethod(new TestSample(), "Wow"));
        }

        [Test]
        public void IvokePrivateMethodWithParameterFromBaseClass()
        {
            Assert.AreEqual("MbUnit. Oh, Yhea!", Reflector.InvokeMethod(new TestSample(), "OhYhea", "MbUnit."));
        }

        [Test]
        [ExpectedException(typeof(ReflectionException))]
        public void TryToInvokeBaseClassMethodButLookInBaseIsFalse()
        {
            Reflector.InvokeMethod(AccessModifier.NonPublic, new TestSample(), "OhYhea", false, "Fail");
        }
        #endregion
    }


}
