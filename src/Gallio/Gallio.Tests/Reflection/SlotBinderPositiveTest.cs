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
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Gallio.Reflection;
using MbUnit.Framework;

namespace Gallio.Tests.Reflection
{
    /// <summary>
    /// Tests for <see cref="SlotBinder" /> in which the action succeeds.
    /// </summary>
    [TestFixture]
    [TestsOn(typeof(SlotBinder))]
    public class SlotBinderTest
    {
        private const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance;
        private const BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static;

        [SetUp]
        public void SetUp()
        {
            Class<int>.staticMethodParamValue = 0;
            Class<int>.staticMethodTypeValue = null;
        }

        [Test]
        public void CreateInstanceWithGenericClass()
        {
            ITypeInfo type = Reflector.Wrap(typeof(Class<>));
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add((IGenericParameterInfo)type.GenericArguments[0], typeof(int));
            slotValues.Add(type.GetConstructors(PublicInstance)[1].Parameters[0], 1);
            slotValues.Add(type.GetFields(PublicInstance)[0], 2);
            slotValues.Add(type.GetProperties(PublicInstance)[0], 3);

            Class<int> instance = (Class<int>)SlotBinder.CreateInstance(type, slotValues);
            Assert.AreEqual(1, instance.constructorParamValue);
            Assert.AreEqual(2, instance.fieldValue);
            Assert.AreEqual(3, instance.propertyValue);
        }

        [Test]
        public void CreateInstanceWithGenericClassDefaultConstructor()
        {
            ITypeInfo type = Reflector.Wrap(typeof(Class<>));
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add((IGenericParameterInfo)type.GenericArguments[0], typeof(int));
            slotValues.Add(type.GetFields(PublicInstance)[0], 2);
            slotValues.Add(type.GetProperties(PublicInstance)[0], 3);

            Class<int> instance = (Class<int>)SlotBinder.CreateInstance(type, slotValues);
            Assert.AreEqual(0, instance.constructorParamValue);
            Assert.AreEqual(2, instance.fieldValue);
            Assert.AreEqual(3, instance.propertyValue);
        }

        [Test]
        public void CreateInstanceWithGenericClassInstantiation()
        {
            ITypeInfo type = Reflector.Wrap(typeof(Class<int>));
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(type.GetConstructors(PublicInstance)[1].Parameters[0], 1);
            slotValues.Add(type.GetFields(PublicInstance)[0], 2);
            slotValues.Add(type.GetProperties(PublicInstance)[0], 3);

            Class<int> instance = (Class<int>)SlotBinder.CreateInstance(type, slotValues);
            Assert.AreEqual(1, instance.constructorParamValue);
            Assert.AreEqual(2, instance.fieldValue);
            Assert.AreEqual(3, instance.propertyValue);
        }

        [Test]
        public void CreateInstanceWithGenericClassInstantiationDefaultConstructor()
        {
            ITypeInfo type = Reflector.Wrap(typeof(Class<int>));
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(type.GetFields(PublicInstance)[0], 2);
            slotValues.Add(type.GetProperties(PublicInstance)[0], 3);

            Class<int> instance = (Class<int>)SlotBinder.CreateInstance(type, slotValues);
            Assert.AreEqual(0, instance.constructorParamValue);
            Assert.AreEqual(2, instance.fieldValue);
            Assert.AreEqual(3, instance.propertyValue);
        }

        [Test]
        public void CreateInstanceWithGenericStruct()
        {
            ITypeInfo type = Reflector.Wrap(typeof(Struct<>));
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add((IGenericParameterInfo)type.GenericArguments[0], typeof(int));
            slotValues.Add(type.GetConstructors(PublicInstance)[0].Parameters[0], 1);
            slotValues.Add(type.GetFields(PublicInstance)[0], 2);
            slotValues.Add(type.GetProperties(PublicInstance)[0], 3);

            Struct<int> instance = (Struct<int>)SlotBinder.CreateInstance(type, slotValues);
            Assert.AreEqual(1, instance.constructorParamValue);
            Assert.AreEqual(2, instance.fieldValue);
            Assert.AreEqual(3, instance.propertyValue);
        }

        [Test]
        public void CreateInstanceWithGenericStructDefaultConstructor()
        {
            ITypeInfo type = Reflector.Wrap(typeof(Struct<>));
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add((IGenericParameterInfo)type.GenericArguments[0], typeof(int));
            slotValues.Add(type.GetFields(PublicInstance)[0], 2);
            slotValues.Add(type.GetProperties(PublicInstance)[0], 3);

            Struct<int> instance = (Struct<int>)SlotBinder.CreateInstance(type, slotValues);
            Assert.AreEqual(0, instance.constructorParamValue);
            Assert.AreEqual(2, instance.fieldValue);
            Assert.AreEqual(3, instance.propertyValue);
        }

        [Test]
        public void CreateInstanceWithGenericStructInstantiation()
        {
            ITypeInfo type = Reflector.Wrap(typeof(Struct<int>));
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(type.GetConstructors(PublicInstance)[0].Parameters[0], 1);
            slotValues.Add(type.GetFields(PublicInstance)[0], 2);
            slotValues.Add(type.GetProperties(PublicInstance)[0], 3);

            Struct<int> instance = (Struct<int>)SlotBinder.CreateInstance(type, slotValues);
            Assert.AreEqual(1, instance.constructorParamValue);
            Assert.AreEqual(2, instance.fieldValue);
            Assert.AreEqual(3, instance.propertyValue);
        }

        [Test]
        public void CreateInstanceWithGenericStructInstantiationDefaultConstructor()
        {
            ITypeInfo type = Reflector.Wrap(typeof(Struct<int>));
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(type.GetFields(PublicInstance)[0], 2);
            slotValues.Add(type.GetProperties(PublicInstance)[0], 3);

            Struct<int> instance = (Struct<int>)SlotBinder.CreateInstance(type, slotValues);
            Assert.AreEqual(0, instance.constructorParamValue);
            Assert.AreEqual(2, instance.fieldValue);
            Assert.AreEqual(3, instance.propertyValue);
        }

        [Test]
        public void InvokeInstanceMethodWithGenericMethod()
        {
            ITypeInfo type = Reflector.Wrap(typeof(Class<int>));
            IMethodInfo method = ChooseByName(type.GetMethods(PublicInstance), "InstanceMethod");
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add((IGenericParameterInfo) method.GenericArguments[0], typeof(int));
            slotValues.Add(method.Parameters[0], 1);

            Class<int> instance = new Class<int>();
            SlotBinder.InvokeInstanceMethod(method, instance, slotValues);
            Assert.AreEqual(typeof(int), instance.instanceMethodTypeValue);
            Assert.AreEqual(1, instance.instanceMethodParamValue);
        }

        [Test]
        public void InvokeInstanceMethodWithGenericMethodInstantiation()
        {
            ITypeInfo type = Reflector.Wrap(typeof(Class<int>));
            IMethodInfo method = Reflector.Wrap(ChooseByName(type.GetMethods(PublicInstance), "InstanceMethod").Resolve(true).MakeGenericMethod(typeof(int)));
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(method.Parameters[0], 1);

            Class<int> instance = new Class<int>();
            SlotBinder.InvokeInstanceMethod(method, instance, slotValues);
            Assert.AreEqual(typeof(int), instance.instanceMethodTypeValue);
            Assert.AreEqual(1, instance.instanceMethodParamValue);
        }

        [Test]
        public void InvokeInstanceMethodWithGenericMethodOnGenericTypeDefn()
        {
            ITypeInfo type = Reflector.Wrap(typeof(Class<>));
            IMethodInfo method = ChooseByName(type.GetMethods(PublicInstance), "InstanceMethod");
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add((IGenericParameterInfo)method.GenericArguments[0], typeof(int));
            slotValues.Add(method.Parameters[0], 1);

            Class<int> instance = new Class<int>();
            SlotBinder.InvokeInstanceMethod(method, instance, slotValues);
            Assert.AreEqual(typeof(int), instance.instanceMethodTypeValue);
            Assert.AreEqual(1, instance.instanceMethodParamValue);
        }

        [Test]
        public void InvokeStaticMethodWithGenericMethod()
        {
            ITypeInfo type = Reflector.Wrap(typeof(Class<int>));
            IMethodInfo method = ChooseByName(type.GetMethods(PublicStatic), "StaticMethod");
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add((IGenericParameterInfo)method.GenericArguments[0], typeof(int));
            slotValues.Add(method.Parameters[0], 1);

            SlotBinder.InvokeStaticMethod(method, typeof(Class<int>), slotValues);
            Assert.AreEqual(typeof(int), Class<int>.staticMethodTypeValue);
            Assert.AreEqual(1, Class<int>.staticMethodParamValue);
        }

        [Test]
        public void InvokeStaticMethodWithGenericMethodInstantiation()
        {
            ITypeInfo type = Reflector.Wrap(typeof(Class<int>));
            IMethodInfo method = Reflector.Wrap(ChooseByName(type.GetMethods(PublicStatic), "StaticMethod").Resolve(true).MakeGenericMethod(typeof(int)));
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(method.Parameters[0], 1);

            SlotBinder.InvokeStaticMethod(method, typeof(Class<int>), slotValues);
            Assert.AreEqual(typeof(int), Class<int>.staticMethodTypeValue);
            Assert.AreEqual(1, Class<int>.staticMethodParamValue);
        }

        [Test]
        public void InvokeStaticMethodWithGenericMethodOnGenericTypeDefn()
        {
            ITypeInfo type = Reflector.Wrap(typeof(Class<>));
            IMethodInfo method = ChooseByName(type.GetMethods(PublicStatic), "StaticMethod");
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add((IGenericParameterInfo)method.GenericArguments[0], typeof(int));
            slotValues.Add(method.Parameters[0], 1);

            SlotBinder.InvokeStaticMethod(method, typeof(Class<int>), slotValues);
            Assert.AreEqual(typeof(int), Class<int>.staticMethodTypeValue);
            Assert.AreEqual(1, Class<int>.staticMethodParamValue);
        }

        public class Class<T>
        {
            internal T constructorParamValue;
            internal int propertyValue;

            internal int instanceMethodParamValue;
            internal Type instanceMethodTypeValue;

            internal static int staticMethodParamValue;
            internal static Type staticMethodTypeValue;

            public Class()
            {
            }

            public Class(T constructorParamValue)
            {
                this.constructorParamValue = constructorParamValue;
            }

            public int fieldValue = 0;

            public int Property { set { propertyValue = value; } }

            public void InstanceMethod<S>(int instanceMethodParamValue)
            {
                this.instanceMethodParamValue = instanceMethodParamValue;
                instanceMethodTypeValue = typeof(S);
            }

            public static void StaticMethod<S>(int staticMethodParamValue)
            {
                Class<T>.staticMethodParamValue = staticMethodParamValue;
                staticMethodTypeValue = typeof(S);
            }
        }

        public struct Struct<T>
        {
            internal T constructorParamValue;
            internal int propertyValue;

            public Struct(T constructorParamValue)
            {
                this.constructorParamValue = constructorParamValue;
                fieldValue = 0;
                propertyValue = 0;
            }

            public int fieldValue;

            public int Property { set { propertyValue = value; } }
        }

        private static T ChooseByName<T>(IEnumerable<T> elements, string name)
            where T : ICodeElementInfo
        {
            foreach (T element in elements)
                if (element.Name == name)
                    return element;

            throw new InvalidOperationException("Could not find code element.");
        }
    }
}
