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
    public class ReflectorStaticTests
    {
        public static readonly string MSCorLibAssembly = Environment.GetEnvironmentVariable("SystemRoot")
            + @"\Microsoft.NET\Framework\v2.0.50727\mscorlib.dll";
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
            Assert.AreEqual("MbUnit Rocks!!!", Reflector.GetField(_sampleObject, "publicString"));
        }

        [Test]
        public void GetPrivateFieldFromBaseClass()
        {
            Assert.AreEqual("Base var", Reflector.GetField(_sampleObject, "_baseString"));
        }

        [Test]
        [ExpectedException(typeof(ReflectionException))]
        public void TryToGetBaseClassFieldButSettingLookInBaseToFalse()
        {
            Reflector.GetField(AccessModifier.NonPublic, _sampleObject, "_baseString", false);
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
            Assert.AreEqual(12, Reflector.GetProperty(_sampleObject, "BaseInteger"));
        }

        [Test]
        [ExpectedException(typeof(ReflectionException))]
        public void TryToGetBaseClassPropertyButSettingLookInBaseToFalse()
        {
            Reflector.GetProperty(AccessModifier.NonPublic, _sampleObject, "BaseInteger", false);
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
            Assert.AreEqual("Wow!", Reflector.InvokeMethod(_sampleObject, "Wow"));
        }

        [Test]
        public void IvokePrivateMethodWithParameterFromBaseClass()
        {
            Assert.AreEqual("MbUnit. Oh, Yhea!", Reflector.InvokeMethod(_sampleObject, "OhYhea", "MbUnit."));
        }

        [Test]
        [ExpectedException(typeof(ReflectionException))]
        public void TryToInvokeBaseClassMethodButLookInBaseIsFalse()
        {
            Reflector.InvokeMethod(AccessModifier.NonPublic, _sampleObject, "OhYhea", false, "Fail");
        }
        #endregion
    }


}
