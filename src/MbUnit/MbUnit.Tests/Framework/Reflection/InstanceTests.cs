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
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using MbUnit.Framework.Reflection;

namespace MbUnit.Tests.Framework.Reflection
{
    [TestFixture]
    public class InstanceTests
    {
        TestSample _sampleObject;
        Reflector _reflect;

        [SetUp]
        public void Setup()
        {
            _sampleObject = new TestSample();
            _reflect = new Reflector(_sampleObject);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorWithNullArgument()
        {
            Reflector reflector = new Reflector(null);
        }

        [Test]
        public void CreateInstanceWithDefaultConstructor()
        {
            string className = "System.Number";
            Reflector reflector = new Reflector(ReflectorStaticTests.MSCorLibAssembly, className);
            Assert.IsNotNull(reflector);
            Assert.AreEqual(true, reflector.InvokeMethod("IsWhite", ' '));
            Assert.AreEqual(false, reflector.InvokeMethod("IsWhite", 'V'));
        }

        [Test]
        public void CreateInstanceWithParametizedConstructor()
        {
            string className = "System.Collections.KeyValuePairs";
            Reflector reflector = new Reflector(ReflectorStaticTests.MSCorLibAssembly, className, 1, 'A');
            Assert.IsNotNull(reflector);
            Assert.AreEqual(1, reflector.GetProperty("Key"));
            Assert.AreEqual('A', reflector.GetProperty("Value"));
        }

        #region GetField Tests
        [Test]
        public void GetPublicField_DefaultAccessibility()
        {
            Assert.AreEqual("MbUnit Rocks!!!", _reflect.GetField("publicString"));
        }

        [Test]
        public void GetPublicField_PublicAccessibility()
        {
            Assert.AreEqual("MbUnit Rocks!!!", _reflect.GetField("publicString", AccessModifier.Public));
        }

        [Test]
        public void GetNonPublicField_DefaultAccessibility()
        {
            Assert.AreEqual(DateTime.Today, _reflect.GetField("privateDateTime"));
        }

        [Test]
        public void GetNonPublicField_NonPublicAccessibility()
        {
            Assert.AreEqual(DateTime.Today, _reflect.GetField("privateDateTime", AccessModifier.NonPublic));
        }

        [Test]
        public void GetStaticField_DefaultAccessibility()
        {
            Assert.AreEqual(7, _reflect.GetField("staticNum"));
        }

        [Test]
        public void GetStaticField_StaticAccessibility()
        {
            Assert.AreEqual(7, _reflect.GetField("staticNum", AccessModifier.Static | AccessModifier.NonPublic));
        }

        [Test]
        public void GetBaseClassField()
        {
            Assert.AreEqual("Base var", _reflect.GetField("_baseString"));
        }

        [Test]
        [ExpectedException(typeof(ReflectionException))]
        public void UnAccessibleField()
        {
            _reflect.GetField("staticNum", AccessModifier.Public);
        }

        [Test]
        [ExpectedException(typeof(ReflectionException))]
        public void TryToGetBaseClassFieldButSettingLookInBaseToFalse()
        {
            _reflect.GetField("_baseString", AccessModifier.NonPublic, false);
        }

        #endregion

        #region SetField Tests

        [Test]
        public void SetPublicField_DefaultAccessibility()
        {
            string fieldName = "publicString";
            string newValue = "Just mbunit";
            _reflect.SetField(fieldName, newValue);
            Assert.AreEqual(newValue, _reflect.GetField(fieldName));
        }

        [Test]
        public void SetPublicField_PublicAccessibility()
        {
            string fieldName = "publicString";
            string newValue = "Just mbunit";
            _reflect.SetField(AccessModifier.Public, fieldName, newValue);
            Assert.AreEqual(newValue, _reflect.GetField(fieldName));
        }

        [Test]
        public void SetNonPublicField_DefaultAccessibility()
        {
            string fieldName = "privateDateTime";
            DateTime dt = new DateTime(2008, 1, 1);
            _reflect.SetField(fieldName, dt);
            Assert.AreEqual(dt, _reflect.GetField(fieldName));
        }

        [Test]
        public void SetNonPublicField_NonPublicAccessibility()
        {
            string fieldName = "privateDateTime";
            DateTime dt = new DateTime(2008, 1, 1);
            _reflect.SetField(AccessModifier.NonPublic, fieldName, dt);
            Assert.AreEqual(dt, _reflect.GetField(fieldName));
        }

        [Test]
        public void SetStaticPublicField_DefaultAccessibility()
        {
            string fieldName = "staticNum";
            int newValue = 10;
            int originalValue = (int)_reflect.GetField(fieldName);
            _reflect.SetField(fieldName, newValue);
            Assert.AreEqual(newValue, _reflect.GetField(fieldName));

            // It's a static field, we need to restore to original value; othwerwise,
            // some other tests will fail.
            _reflect.SetField(fieldName, originalValue);
            Assert.AreEqual(originalValue, _reflect.GetField(fieldName));
        }

        [Test]
        public void SetStaticPublicField_StaticNonPublicAccessibility()
        {
            string fieldName = "staticNum";
            int newValue = 10;
            int originalValue = (int)_reflect.GetField(fieldName);
            _reflect.SetField(AccessModifier.Static | AccessModifier.NonPublic, fieldName, newValue);
            Assert.AreEqual(newValue, _reflect.GetField(fieldName));

            // It's a static field, we need to restore to original value; othwerwise,
            // some other tests will fail.
            _reflect.SetField(AccessModifier.Static | AccessModifier.NonPublic, fieldName, originalValue);
            Assert.AreEqual(originalValue, _reflect.GetField(fieldName));
        }

        [Test]
        public void SetBaseClassField()
        {
            _reflect.SetField("_baseString", "Test Field");
            Assert.AreEqual("Test Field", _reflect.GetField("_baseString"));
        }

        #endregion

        #region GetProperty Tests

        [Test]
        public void GetPublicProperty_DefaultAccessibility()
        {
            Assert.AreEqual("MbUnit Rocks!!!", _reflect.GetProperty("PublicProperty"));
        }

        [Test]
        public void GetPublicProperty_PublicAccessibility()
        {
            Assert.AreEqual("MbUnit Rocks!!!", _reflect.GetProperty(AccessModifier.Public, "PublicProperty"));
        }

        [Test]
        public void GetNonPublicProperty_DefaultAccessibility()
        {
            Assert.AreEqual(DateTime.Today, _reflect.GetProperty("InternalProperty"));
        }

        [Test]
        public void GetNonPublicProperty_NonPublicAccessibility()
        {
            Assert.AreEqual(DateTime.Today, _reflect.GetProperty(AccessModifier.NonPublic, "InternalProperty"));
        }

        [Test]
        public void GetStaticProperty_DefaultAccessibility()
        {
            Assert.AreEqual(7, _reflect.GetProperty("StaticProperty"));
        }

        [Test]
        public void GetStaticProperty_StaticAccessibility()
        {
            Assert.AreEqual(7, _reflect.GetProperty(AccessModifier.Static | AccessModifier.NonPublic, "StaticProperty"));
        }

        #endregion

        #region SetProperty Tests

        [Test]
        public void SetPublicPropert_DefaultAccessibility()
        {
            string propertyName = "PublicProperty";
            string newValue = "Just mbunit";
            _reflect.SetProperty(propertyName, newValue);
            Assert.AreEqual(newValue, _reflect.GetProperty(propertyName));
        }

        [Test]
        public void SetPublicPropert_PublicAccessibility()
        {
            string propertyName = "PublicProperty";
            string newValue = "Just mbunit";
            _reflect.SetProperty(AccessModifier.Public, propertyName, newValue);
            Assert.AreEqual(newValue, _reflect.GetProperty(propertyName));
        }

        [Test]
        public void SetNonPublicProperty_DefaultAccessibility()
        {
            string propertyName = "InternalProperty";
            DateTime dt = new DateTime(2008, 1, 1);
            _reflect.SetProperty(propertyName, dt);
            Assert.AreEqual(dt, _reflect.GetProperty(propertyName));
        }

        [Test]
        public void SetNonPublicProperty_NonPublicAccessibility()
        {
            string propertyName = "InternalProperty";
            DateTime dt = new DateTime(2008, 1, 1);
            _reflect.SetProperty(AccessModifier.NonPublic, propertyName, dt);
            Assert.AreEqual(dt, _reflect.GetProperty(propertyName));
        }

        [Test]
        public void SetStaticProperty_DefaultAccessibility()
        {
            string propertyName = "StaticProperty";
            int newValue = 10;
            int originalValue = (int)_reflect.GetProperty(propertyName);
            _reflect.SetProperty(propertyName, newValue);
            Assert.AreEqual(newValue, _reflect.GetProperty(propertyName));

            // It's a static field, we need to restore to original value; othwerwise,
            // some other tests will fail.
            _reflect.SetProperty(propertyName, originalValue);
            Assert.AreEqual(originalValue, _reflect.GetProperty(propertyName));
        }

        [Test]
        public void SetStaticProperty_StaticNonPublicAccessibility()
        {
            string propertyName = "StaticProperty";
            int newValue = 10;
            int originalValue = (int)_reflect.GetProperty(propertyName);
            _reflect.SetProperty(AccessModifier.Static | AccessModifier.NonPublic, propertyName, newValue);
            Assert.AreEqual(newValue, _reflect.GetProperty(propertyName));

            // It's a static field, we need to restore to original value; othwerwise,
            // some other tests will fail.
            _reflect.SetProperty(propertyName, originalValue);
            Assert.AreEqual(originalValue, _reflect.GetProperty(propertyName));
        }

        #endregion

        #region InvokeMethod

        [Test]
        public void PublicMethodWithNoParameters_DefaultAccessibility()
        {
            Assert.AreEqual("MbUnit Rocks!!!", _reflect.InvokeMethod("PraiseMe"));
        }

        [Test]
        public void PublicMethod_DefaultAccessibility()
        {
            Assert.AreEqual(25, _reflect.InvokeMethod("Pow", 5));
        }

        [Test]
        public void PublicMethod_PublicAccessibility()
        {
            Assert.AreEqual(25, _reflect.InvokeMethod(AccessModifier.Public, "Pow", 5));
        }

        [Test]
        public void NonPublicMethod_DefaultAccessiblity()
        {
            Assert.AreEqual(15, _reflect.InvokeMethod("Multiply", 5, 3));

        }

        [Test]
        public void StaticPublicMethod_StaticAccessiblity()
        {
            Assert.AreEqual(7, _reflect.InvokeMethod(AccessModifier.Static | AccessModifier.Public, "Add", 1, 6));
        }

        #endregion
    }
}
