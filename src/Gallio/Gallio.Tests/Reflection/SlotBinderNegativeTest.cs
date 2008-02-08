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
using Gallio.Collections;
using Gallio.Reflection;
using MbUnit.Framework;

namespace Gallio.Tests.Reflection
{
    /// <summary>
    /// Tests for <see cref="SlotBinder" /> in which the action fails due to argument validations.
    /// </summary>
    [TestFixture]
    [TestsOn(typeof(SlotBinder))]
    public class SlotBinderNegativeTest
    {
        public static readonly ITypeInfo AbstractClassInfo = Reflector.Wrap(typeof(AbstractClass));
        public static readonly ITypeInfo InterfaceInfo = Reflector.Wrap(typeof(Interface));

        public static readonly ITypeInfo GenericClassDefInfo = Reflector.Wrap(typeof(GenericClass<>));
        public static readonly ITypeInfo GenericClassInstInfo = Reflector.Wrap(typeof(GenericClass<int>));
        public static readonly IGenericParameterInfo GenericClassParamInfo = (IGenericParameterInfo)GenericClassDefInfo.GenericArguments[0];

        public static readonly ITypeInfo NonGenericClassInfo = Reflector.Wrap(typeof(NonGenericClass));
        public static readonly IConstructorInfo NonGenericClassOneParamConstructorInfo = ChooseByParameterCount(NonGenericClassInfo.GetConstructors(BindingFlags.Public | BindingFlags.Instance), 1);
        public static readonly IConstructorInfo NonGenericClassTwoParamConstructorInfo = ChooseByParameterCount(NonGenericClassInfo.GetConstructors(BindingFlags.Public | BindingFlags.Instance), 2);
        public static readonly IFieldInfo NonGenericClassFieldInfo = NonGenericClassInfo.GetFields(BindingFlags.Instance | BindingFlags.Public)[0];
        public static readonly IPropertyInfo NonGenericClassPropertyInfo = NonGenericClassInfo.GetProperties(BindingFlags.Instance | BindingFlags.Public)[0];
        public static readonly IMethodInfo NonGenericClassMethodInfo = ChooseByName(NonGenericClassInfo.GetMethods(BindingFlags.Instance | BindingFlags.Public), "Method");
        public static readonly IMethodInfo NonGenericClassGenericMethodInfo = ChooseByName(NonGenericClassInfo.GetMethods(BindingFlags.Instance | BindingFlags.Public), "GenericMethod");
        public static readonly IMethodInfo NonGenericClassStaticMethodInfo = ChooseByName(NonGenericClassInfo.GetMethods(BindingFlags.Static | BindingFlags.Public), "StaticMethod");

        [Test, ExpectedArgumentNullException]
        public void CreateInstanceThrowsWhenTypeIsNull()
        {
            SlotBinder.CreateInstance(null, EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance);
        }

        [Test, ExpectedArgumentNullException]
        public void CreateInstanceThrowsWhenSlotValuesIsNull()
        {
            SlotBinder.CreateInstance(NonGenericClassInfo, null);
        }

        [Test, ExpectedArgumentNullException]
        public void CreateInstanceThrowsWhenSlotValuesContainsANullSlot()
        {
            SlotBinder.CreateInstance(NonGenericClassInfo, new KeyValuePair<ISlotInfo, object>[]
                {
                    new KeyValuePair<ISlotInfo, object>(null, 42)
                });
        }

        [Test, ExpectedArgumentException]
        public void CreateInstanceThrowsWhenTypeIsAbstract()
        {
            SlotBinder.CreateInstance(AbstractClassInfo, EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance);
        }

        [Test, ExpectedArgumentException]
        public void CreateInstanceThrowsWhenTypeIsAnInterface()
        {
            SlotBinder.CreateInstance(InterfaceInfo, EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance);
        }

        [Test, ExpectedArgumentException]
        public void CreateInstanceThrowsWhenTypeIsAGenericClassParameter()
        {
            SlotBinder.CreateInstance(GenericClassParamInfo, EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance);
        }

        [Test, ExpectedArgumentException]
        public void CreateInstanceThrowsWhenTypeIsAnArray()
        {
            ITypeInfo type = Reflector.Wrap(typeof(NonGenericClass[]));
            SlotBinder.CreateInstance(type, EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance);
        }

        [Test, ExpectedArgumentException]
        public void CreateInstanceThrowsWhenTypeIsAPointer()
        {
            ITypeInfo type = Reflector.Wrap(typeof(NonGenericClass).MakePointerType());
            SlotBinder.CreateInstance(type, EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance);
        }

        [Test, ExpectedArgumentException]
        public void CreateInstanceThrowsWhenTypeIsAByRef()
        {
            ITypeInfo type = Reflector.Wrap(typeof(NonGenericClass).MakeByRefType());
            SlotBinder.CreateInstance(type, EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance);
        }

        [Test, ExpectedArgumentException]
        public void CreateInstanceThrowsWhenConstructorParametersBelongToDifferentConstructor()
        {
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(NonGenericClassOneParamConstructorInfo.Parameters[0], 42);
            slotValues.Add(NonGenericClassTwoParamConstructorInfo.Parameters[0], 42);

            SlotBinder.CreateInstance(NonGenericClassInfo, slotValues);
        }

        [Test, ExpectedArgumentException]
        public void CreateInstanceThrowsWhenNotAllConstructorParametersHaveValues()
        {
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(NonGenericClassTwoParamConstructorInfo.Parameters[0], 42);

            SlotBinder.CreateInstance(NonGenericClassInfo, slotValues);
        }

        [Test, ExpectedArgumentException]
        public void CreateInstanceThrowsWhenNoConstructorParametersAndThereIsNoDefaultConstructor()
        {
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();

            SlotBinder.CreateInstance(NonGenericClassInfo, slotValues);
        }

        [Test, ExpectedArgumentException]
        public void CreateInstanceThrowsWhenConstructorParameterSlotBelongsToDifferentType()
        {
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(NonGenericClassOneParamConstructorInfo.Parameters[0], 42);

            SlotBinder.CreateInstance(GenericClassDefInfo, slotValues);
        }

        [Test, ExpectedArgumentException]
        public void CreateInstanceThrowsWhenFieldSlotBelongsToDifferentType()
        {
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(NonGenericClassFieldInfo, 42);

            SlotBinder.CreateInstance(GenericClassDefInfo, slotValues);
        }

        [Test, ExpectedArgumentException]
        public void CreateInstanceThrowsWhenPropertySlotBelongsToDifferentType()
        {
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(NonGenericClassPropertyInfo, 42);

            SlotBinder.CreateInstance(GenericClassDefInfo, slotValues);
        }

        [Test, ExpectedArgumentException]
        public void CreateInstanceThrowsWhenGenericClassParamSlotBelongsToDifferentType()
        {
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(GenericClassParamInfo, 42);

            SlotBinder.CreateInstance(GenericClassInstInfo, slotValues);
        }

        [Test, ExpectedArgumentException]
        public void CreateInstanceThrowsWhenMethodParamSlotUsedInsteadOfAConstructorParam()
        {
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(NonGenericClassMethodInfo.Parameters[0], 42);

            SlotBinder.CreateInstance(NonGenericClassInfo, slotValues);
        }

        [Test, ExpectedArgumentException]
        public void CreateInstanceThrowsWhenMissingGenericParamSlotForGenericClassDef()
        {
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();

            SlotBinder.CreateInstance(GenericClassDefInfo, slotValues);
        }

        [Test, ExpectedArgumentException]
        public void CreateInstanceThrowsWhenGenericParamSlotForGenericClassDefIsNull()
        {
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(GenericClassParamInfo, null);

            SlotBinder.CreateInstance(GenericClassDefInfo, slotValues);
        }

        [Test, ExpectedArgumentException]
        public void CreateInstanceThrowsWhenGenericParamSlotForGenericClassDefIsNotAssignedToAType()
        {
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(GenericClassParamInfo, 42);

            SlotBinder.CreateInstance(GenericClassDefInfo, slotValues);
        }

        [Test, ExpectedArgumentNullException]
        public void InvokeInstanceMethodThrowsWhenMethodIsNull()
        {
            SlotBinder.InvokeInstanceMethod(null, new NonGenericClass(42), EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance);
        }

        [Test, ExpectedArgumentNullException]
        public void InvokeInstanceMethodThrowsWhenInstanceIsNull()
        {
            SlotBinder.InvokeInstanceMethod(NonGenericClassMethodInfo, null, EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance);
        }

        [Test, ExpectedArgumentNullException]
        public void InvokeInstanceMethodThrowsWhenValuesIsNull()
        {
            SlotBinder.InvokeInstanceMethod(NonGenericClassMethodInfo, new NonGenericClass(42), null);
        }

        [Test, ExpectedArgumentException]
        public void InvokeInstanceMethodThrowsWhenMethodIsStatic()
        {
            SlotBinder.InvokeInstanceMethod(NonGenericClassStaticMethodInfo, new NonGenericClass(42), EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance);
        }

        [Test, ExpectedArgumentNullException]
        public void InvokeStaticMethodThrowsWhenMethodIsNull()
        {
            SlotBinder.InvokeStaticMethod(null, typeof(NonGenericClass), EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance);
        }

        [Test, ExpectedArgumentNullException]
        public void InvokeStaticMethodThrowsWhenTypeInstantiationIsNull()
        {
            SlotBinder.InvokeStaticMethod(NonGenericClassStaticMethodInfo, null, EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance);
        }

        [Test, ExpectedArgumentNullException]
        public void InvokeStaticMethodThrowsWhenValuesIsNull()
        {
            SlotBinder.InvokeStaticMethod(NonGenericClassStaticMethodInfo, typeof(NonGenericClass), null);
        }

        [Test, ExpectedArgumentException]
        public void InvokeStaticMethodThrowsWhenMethodIsNotStatic()
        {
            SlotBinder.InvokeStaticMethod(NonGenericClassMethodInfo, typeof(NonGenericClass), EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance);
        }

        [Test, ExpectedArgumentNullException]
        public void InvokeMethodThrowsWhenValuesConstainsANullSlot()
        {
            NonGenericClass instance = new NonGenericClass(42);
            ITypeInfo type = Reflector.Wrap(typeof(NonGenericClass));
            IMethodInfo method = type.GetMethod("Method", BindingFlags.Public | BindingFlags.Instance);
            SlotBinder.InvokeInstanceMethod(method, instance, new KeyValuePair<ISlotInfo, object>[]
                {
                    new KeyValuePair<ISlotInfo, object>(null, 42)
                });
        }

        [Test, ExpectedArgumentException]
        public void InvokeMethodThrowsWhenMissingParamSlotForMethod()
        {
            NonGenericClass instance = new NonGenericClass(42);
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();

            SlotBinder.InvokeInstanceMethod(NonGenericClassMethodInfo, instance, slotValues);
        }

        [Test, ExpectedArgumentException]
        public void InvokeMethodThrowsWhenMissingGenericParamSlotForGenericMethod()
        {
            NonGenericClass instance = new NonGenericClass(42);
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(NonGenericClassGenericMethodInfo.Parameters[0], 42);

            SlotBinder.InvokeInstanceMethod(NonGenericClassGenericMethodInfo, instance, slotValues);
        }

        [Test, ExpectedArgumentException]
        public void InvokeMethodThrowsWhenGenericParamSlotDoesNotBelongToTheMethod()
        {
            NonGenericClass instance = new NonGenericClass(42);
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(GenericClassParamInfo, typeof(int));
            slotValues.Add(NonGenericClassGenericMethodInfo.Parameters[0], 42);

            SlotBinder.InvokeInstanceMethod(NonGenericClassGenericMethodInfo, instance, slotValues);
        }

        [Test, ExpectedArgumentException]
        public void InvokeMethodThrowsWhenParamSlotBelongsToDifferentMethod()
        {
            NonGenericClass instance = new NonGenericClass(42);
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(NonGenericClassOneParamConstructorInfo.Parameters[0], 42);

            SlotBinder.InvokeInstanceMethod(NonGenericClassMethodInfo, instance, slotValues);
        }

        [Test, ExpectedArgumentException]
        public void InvokeInstanceMethodThrowsWhenInstanceIsOfWrongType()
        {
            GenericClass<int> instance = new GenericClass<int>();
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(NonGenericClassMethodInfo.Parameters[0], 42);
            slotValues.Add(NonGenericClassMethodInfo.Parameters[1], "42");

            SlotBinder.InvokeInstanceMethod(NonGenericClassMethodInfo, instance, slotValues);
        }

        private interface Interface
        {
        }

        private abstract class AbstractClass
        {
        }

        private class NonGenericClass
        {
            public int Field = 0;

            public int Property { get { return 0; } set { } }

            public NonGenericClass(int intValue)
            {
            }

            public NonGenericClass(int intValue, string stringValue)
            {
            }

            public void Method(int intValue, string stringValue)
            {
            }

            public void GenericMethod<T>(int intValue)
            {
            }

            public static void StaticMethod(int intValue)
            {
            }
        }

        private class GenericClass<T>
        {
        }

        private static T ChooseByParameterCount<T>(IEnumerable<T> functions, int count)
            where T : IFunctionInfo
        {
            foreach (T function in functions)
                if (function.Parameters.Count == count)
                    return function;

            throw new InvalidOperationException("Could not find function.");
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
