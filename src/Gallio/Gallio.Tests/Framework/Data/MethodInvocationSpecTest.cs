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
using System.Reflection;
using Gallio.Collections;
using Gallio.Runtime;
using Gallio.Framework.Data;
using Gallio.Framework.Conversions;
using Gallio.Framework.Formatting;
using Gallio.Reflection;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(MethodInvocationSpec))]
    public class MethodInvocationSpecTest : BaseUnitTest
    {
        private const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance;
        private const BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static;

        private static readonly ITypeInfo GenericClassDefInfo = Reflector.Wrap(typeof(GenericClass<>));

        private static readonly ITypeInfo NonGenericClassInfo = Reflector.Wrap(typeof(NonGenericClass));
        private static readonly IMethodInfo NonGenericClassMethodInfo = NonGenericClassInfo.GetMethod("Method", PublicInstance);
        private static readonly IMethodInfo NonGenericClassEmptyMethodInfo = NonGenericClassInfo.GetMethod("EmptyMethod", PublicInstance);
        private static readonly IMethodInfo NonGenericClassGenericMethodInfo = NonGenericClassInfo.GetMethod("GenericMethod", PublicInstance);
        private static readonly IMethodInfo NonGenericClassStaticMethodInfo = NonGenericClassInfo.GetMethod("StaticMethod", PublicStatic);

        [SetUp]
        new public void SetUp()
        {
            GenericClass<int>.staticMethodParamValue = 0;
            GenericClass<int>.staticMethodTypeValue = null;
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfResolvedTypeIsNull()
        {
            new MethodInvocationSpec(null, NonGenericClassEmptyMethodInfo,
                EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance, Mocks.Stub<IConverter>());                
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfMethodIsNull()
        {
            new MethodInvocationSpec(typeof(NonGenericClass), null,
                EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance, Mocks.Stub<IConverter>());

        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfSlotValuesIsNull()
        {
            new MethodInvocationSpec(typeof(NonGenericClass), NonGenericClassEmptyMethodInfo,
                null, Mocks.Stub<IConverter>());

        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfConverterIsNull()
        {
            new MethodInvocationSpec(typeof(NonGenericClass), NonGenericClassEmptyMethodInfo,
                EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance, null);
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfSlotValuesContainsANullSlot()
        {
            new MethodInvocationSpec(typeof(NonGenericClass), NonGenericClassEmptyMethodInfo,
                new KeyValuePair<ISlotInfo, object>[] { new KeyValuePair<ISlotInfo, object>(null, 42) },
                Mocks.Stub<IConverter>());
        }

        [Test, ExpectedArgumentException]
        public void ConstructorThrowsWhenMissingParamSlotForMethod()
        {
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            new MethodInvocationSpec(typeof(NonGenericClass), NonGenericClassMethodInfo, slotValues, NullConverter.Instance);
        }

        [Test, ExpectedArgumentException]
        public void ConstructorThrowsWhenMissingGenericParamSlotForGenericMethod()
        {
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(NonGenericClassGenericMethodInfo.Parameters[0], 42);

            new MethodInvocationSpec(typeof(NonGenericClass), NonGenericClassGenericMethodInfo, slotValues, NullConverter.Instance);
        }

        [Test, ExpectedArgumentException]
        public void ConstructorThrowsWhenGenericParamSlotDoesNotBelongToTheMethod()
        {
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add((ISlotInfo) GenericClassDefInfo.GenericArguments[0], typeof(int));
            slotValues.Add(NonGenericClassGenericMethodInfo.Parameters[0], 42);

            new MethodInvocationSpec(typeof(NonGenericClass), NonGenericClassGenericMethodInfo, slotValues, NullConverter.Instance);
        }

        [Test, ExpectedArgumentException]
        public void ConstructorThrowsWhenParamSlotBelongsToDifferentMethod()
        {
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(NonGenericClassStaticMethodInfo.Parameters[0], 42);

            new MethodInvocationSpec(typeof(NonGenericClass), NonGenericClassMethodInfo, slotValues, NullConverter.Instance);
        }

        [Test]
        public void InvokeInstanceMethodThrowsWhenInstanceIsOfWrongType()
        {
            GenericClass<int> instance = new GenericClass<int>();
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();

            MethodInvocationSpec spec = new MethodInvocationSpec(typeof(NonGenericClass), NonGenericClassEmptyMethodInfo, slotValues, NullConverter.Instance);
            InterimAssert.Throws<TargetException>(delegate { spec.Invoke(instance); });
        }

        [Test]
        public void InvokeInstanceMethodThrowsWhenInstanceIsNull()
        {
            MethodInvocationSpec spec = new MethodInvocationSpec(typeof(NonGenericClass), NonGenericClassEmptyMethodInfo,
                EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance, NullConverter.Instance);

            InterimAssert.Throws<ArgumentNullException>(delegate { spec.Invoke(null); });
        }

        [Test]
        public void InvokeInstanceMethodWithGenericMethod()
        {
            ITypeInfo type = Reflector.Wrap(typeof(GenericClass<int>));
            IMethodInfo method = type.GetMethod("InstanceMethod", PublicInstance);
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add((IGenericParameterInfo)method.GenericArguments[0], typeof(int));
            slotValues.Add(method.Parameters[0], 1);

            GenericClass<int> instance = new GenericClass<int>();
            MethodInvocationSpec spec = new MethodInvocationSpec(typeof(GenericClass<int>), method, slotValues, NullConverter.Instance);
            spec.Invoke(instance);
            Assert.AreEqual(typeof(int), instance.instanceMethodTypeValue);
            Assert.AreEqual(1, instance.instanceMethodParamValue);
        }

        [Test]
        public void InvokeInstanceMethodWithGenericMethodInstantiation()
        {
            ITypeInfo type = Reflector.Wrap(typeof(GenericClass<int>));
            IMethodInfo method = Reflector.Wrap(type.GetMethod("InstanceMethod", PublicInstance).Resolve(true).MakeGenericMethod(typeof(int)));
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(method.Parameters[0], 1);

            GenericClass<int> instance = new GenericClass<int>();
            MethodInvocationSpec spec = new MethodInvocationSpec(typeof(GenericClass<int>), method, slotValues, NullConverter.Instance);
            spec.Invoke(instance);
            Assert.AreEqual(typeof(int), instance.instanceMethodTypeValue);
            Assert.AreEqual(1, instance.instanceMethodParamValue);
        }

        [Test]
        public void InvokeInstanceMethodWithGenericMethodOnGenericTypeDefn()
        {
            ITypeInfo type = Reflector.Wrap(typeof(GenericClass<>));
            IMethodInfo method = type.GetMethod("InstanceMethod", PublicInstance);
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add((IGenericParameterInfo)method.GenericArguments[0], typeof(int));
            slotValues.Add(method.Parameters[0], 1);

            GenericClass<int> instance = new GenericClass<int>();
            MethodInvocationSpec spec = new MethodInvocationSpec(typeof(GenericClass<int>), method, slotValues, NullConverter.Instance);
            spec.Invoke(instance);
            Assert.AreEqual(typeof(int), instance.instanceMethodTypeValue);
            Assert.AreEqual(1, instance.instanceMethodParamValue);
        }

        [Test]
        public void InvokeStaticMethodWithGenericMethod()
        {
            ITypeInfo type = Reflector.Wrap(typeof(GenericClass<int>));
            IMethodInfo method = type.GetMethod("StaticMethod", PublicStatic);
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add((IGenericParameterInfo)method.GenericArguments[0], typeof(int));
            slotValues.Add(method.Parameters[0], 1);

            MethodInvocationSpec spec = new MethodInvocationSpec(typeof(GenericClass<int>), method, slotValues, NullConverter.Instance);
            spec.Invoke(null);
            Assert.AreEqual(typeof(int), GenericClass<int>.staticMethodTypeValue);
            Assert.AreEqual(1, GenericClass<int>.staticMethodParamValue);
        }

        [Test]
        public void InvokeStaticMethodWithGenericMethodInstantiation()
        {
            ITypeInfo type = Reflector.Wrap(typeof(GenericClass<int>));
            IMethodInfo method = Reflector.Wrap(type.GetMethod("StaticMethod", PublicStatic).Resolve(true).MakeGenericMethod(typeof(int)));
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(method.Parameters[0], 1);

            MethodInvocationSpec spec = new MethodInvocationSpec(typeof(GenericClass<int>), method, slotValues, NullConverter.Instance);
            spec.Invoke(null);
            Assert.AreEqual(typeof(int), GenericClass<int>.staticMethodTypeValue);
            Assert.AreEqual(1, GenericClass<int>.staticMethodParamValue);
        }

        [Test]
        public void InvokeStaticMethodWithGenericMethodOnGenericTypeDefn()
        {
            ITypeInfo type = Reflector.Wrap(typeof(GenericClass<>));
            IMethodInfo method = type.GetMethod("StaticMethod", PublicStatic);
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add((IGenericParameterInfo)method.GenericArguments[0], typeof(int));
            slotValues.Add(method.Parameters[0], 1);

            MethodInvocationSpec spec = new MethodInvocationSpec(typeof(GenericClass<int>), method, slotValues, NullConverter.Instance);
            spec.Invoke(null);
            Assert.AreEqual(typeof(int), GenericClass<int>.staticMethodTypeValue);
            Assert.AreEqual(1, GenericClass<int>.staticMethodParamValue);
        }


        [Test]
        public void InvokeThrowsUnwrappedException()
        {
            ITypeInfo type = Reflector.Wrap(typeof(NonGenericClass));
            IMethodInfo method = type.GetMethod("StaticMethodThatThrows", PublicStatic);
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();

            MethodInvocationSpec spec = new MethodInvocationSpec(typeof(NonGenericClass), method, slotValues, NullConverter.Instance);
            InterimAssert.Throws<InvalidOperationException>(delegate { spec.Invoke(null); });
        }

        [Test]
        public void SpecPropertiesDescribeTheMethod()
        {
            ITypeInfo type = Reflector.Wrap(typeof(GenericClass<>));
            IMethodInfo method = type.GetMethod("StaticMethod", PublicStatic);
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add((IGenericParameterInfo)method.GenericArguments[0], typeof(int));
            slotValues.Add(method.Parameters[0], 1);

            MethodInvocationSpec spec = new MethodInvocationSpec(typeof(GenericClass<int>), method, slotValues, NullConverter.Instance);
            Assert.AreEqual(typeof(GenericClass<int>), spec.ResolvedType);
            Assert.AreSame(method, spec.Method);
            Assert.AreSame(slotValues, spec.SlotValues);
            Assert.AreSame(NullConverter.Instance, spec.Converter);
            Assert.AreEqual("Void StaticMethod[Int32](Int32)", spec.ResolvedMethod.ToString());
            CollectionAssert.AreElementsEqual(new object[] { 1 }, spec.ResolvedArguments);
        }

        [Test]
        public void FormatThrowsIfEntityIsNull()
        {
            MethodInvocationSpec spec = new MethodInvocationSpec(typeof(NonGenericClass), NonGenericClassEmptyMethodInfo,
                EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance, Mocks.Stub<IConverter>());

            InterimAssert.Throws<ArgumentNullException>(delegate { spec.Format(null, Mocks.Stub<IFormatter>()); });
        }

        [Test]
        public void FormatThrowsIfFormatterIsNull()
        {
            MethodInvocationSpec spec = new MethodInvocationSpec(typeof(NonGenericClass), NonGenericClassEmptyMethodInfo,
                EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance, Mocks.Stub<IConverter>());

            InterimAssert.Throws<ArgumentNullException>(delegate { spec.Format("Foo", null); });
        }

        [Test]
        public void FormatDescribesTheMethod()
        {
            ITypeInfo type = Reflector.Wrap(typeof(GenericClass<>));
            IMethodInfo method = type.GetMethod("StaticMethod", PublicStatic);
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add((IGenericParameterInfo)method.GenericArguments[0], typeof(int));
            slotValues.Add(method.Parameters[0], 1);

            MethodInvocationSpec spec = new MethodInvocationSpec(typeof(GenericClass<int>), method, slotValues, NullConverter.Instance);
            Assert.AreEqual("Foo<int>(1)", spec.Format("Foo", RuntimeAccessor.Instance.Resolve<IFormatter>()));
        }

        [Test]
        public void FormatStringIsJustTheEntityIfThereAreNoSlots()
        {
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();

            MethodInvocationSpec spec = new MethodInvocationSpec(typeof(NonGenericClass), NonGenericClassEmptyMethodInfo, slotValues, NullConverter.Instance);
            Assert.AreEqual("Foo", spec.Format("Foo", RuntimeAccessor.Instance.Resolve<IFormatter>()));
        }

        public class NonGenericClass
        {
            public void EmptyMethod()
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

            public static void StaticMethodThatThrows()
            {
                throw new InvalidOperationException("Boom!");
            }
        }

        public class GenericClass<T>
        {
            internal int instanceMethodParamValue;
            internal Type instanceMethodTypeValue;

            internal static int staticMethodParamValue;
            internal static Type staticMethodTypeValue;

            public void InstanceMethod<S>(int instanceMethodParamValue)
            {
                this.instanceMethodParamValue = instanceMethodParamValue;
                instanceMethodTypeValue = typeof(S);
            }

            public static void StaticMethod<S>(int staticMethodParamValue)
            {
                GenericClass<T>.staticMethodParamValue = staticMethodParamValue;
                staticMethodTypeValue = typeof(S);
            }
        }
    }
}