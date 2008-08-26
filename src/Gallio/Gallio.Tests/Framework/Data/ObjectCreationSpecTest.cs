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
    [TestsOn(typeof(ObjectCreationSpec))]
    public class ObjectCreationSpecTest : BaseUnitTest
    {
        private const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance;

        private static readonly ITypeInfo EmptyClassInfo = Reflector.Wrap(typeof(EmptyClass));

        private static readonly ITypeInfo AbstractClassInfo = Reflector.Wrap(typeof(AbstractClass));
        private static readonly IConstructorInfo AbstractClassOneParamConstructorInfo = ChooseByParameterCount(AbstractClassInfo.GetConstructors(PublicInstance), 1);

        private static readonly ITypeInfo GenericClassDefInfo = Reflector.Wrap(typeof(GenericClass<>));
        private static readonly ITypeInfo GenericClassInstInfo = Reflector.Wrap(typeof(GenericClass<int>));
        private static readonly IGenericParameterInfo GenericClassParamInfo = (IGenericParameterInfo)GenericClassDefInfo.GenericArguments[0];

        private static readonly ITypeInfo NonGenericClassInfo = Reflector.Wrap(typeof(NonGenericClass));
        private static readonly IConstructorInfo NonGenericClassOneParamConstructorInfo = ChooseByParameterCount(NonGenericClassInfo.GetConstructors(PublicInstance), 1);
        private static readonly IConstructorInfo NonGenericClassTwoParamConstructorInfo = ChooseByParameterCount(NonGenericClassInfo.GetConstructors(PublicInstance), 2);
        private static readonly IFieldInfo NonGenericClassFieldInfo = NonGenericClassInfo.GetFields(PublicInstance)[0];
        private static readonly IPropertyInfo NonGenericClassPropertyInfo = NonGenericClassInfo.GetProperties(PublicInstance)[0];
        private static readonly IMethodInfo NonGenericClassMethodInfo = NonGenericClassInfo.GetMethod("Method", PublicInstance);

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfTypeIsNull()
        {
            new ObjectCreationSpec(null, EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance, Mocks.Stub<IConverter>());

        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfSlotValuesIsNull()
        {
            new ObjectCreationSpec(EmptyClassInfo, null, Mocks.Stub<IConverter>());

        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfConverterIsNull()
        {
            new ObjectCreationSpec(EmptyClassInfo, EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance, null);
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfSlotValuesContainsANullSlot()
        {
            new ObjectCreationSpec(EmptyClassInfo,
                new KeyValuePair<ISlotInfo, object>[] { new KeyValuePair<ISlotInfo, object>(null, 42) },
                Mocks.Stub<IConverter>());
        }

        [Test, ExpectedArgumentException]
        public void ConstructorThrowsIfTypeIsAGenericParameter()
        {
            new ObjectCreationSpec(GenericClassDefInfo.GenericArguments[0], EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance, Mocks.Stub<IConverter>());
        }

        [Test, ExpectedArgumentException]
        public void ConstructorThrowsIfTypeIsAnArray()
        {
            new ObjectCreationSpec(EmptyClassInfo.MakeArrayType(1), EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance, Mocks.Stub<IConverter>());
        }

        [Test, ExpectedArgumentException]
        public void ConstructorThrowsIfTypeIsAPointer()
        {
            new ObjectCreationSpec(EmptyClassInfo.MakePointerType(), EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance, Mocks.Stub<IConverter>());
        }

        [Test, ExpectedArgumentException]
        public void ConstructorThrowsIfTypeIsAByRef()
        {
            new ObjectCreationSpec(EmptyClassInfo.MakeByRefType(), EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance, Mocks.Stub<IConverter>());
        }

        [Test, ExpectedArgumentException]
        public void ConstructorThrowsWhenConstructorParametersBelongToDifferentConstructor()
        {
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(NonGenericClassOneParamConstructorInfo.Parameters[0], 42);
            slotValues.Add(NonGenericClassTwoParamConstructorInfo.Parameters[0], 42);

            new ObjectCreationSpec(NonGenericClassInfo, slotValues, NullConverter.Instance);
        }

        [Test, ExpectedArgumentException]
        public void ConstructorThrowsWhenNotAllConstructorParametersHaveValues()
        {
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(NonGenericClassTwoParamConstructorInfo.Parameters[0], 42);

            new ObjectCreationSpec(NonGenericClassInfo, slotValues, NullConverter.Instance);
        }

        [Test, ExpectedArgumentException]
        public void ConstructorThrowsWhenConcreteClassHasNoConstructorParametersAndThereIsNoDefaultConstructor()
        {
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();

            new ObjectCreationSpec(NonGenericClassInfo, slotValues, NullConverter.Instance);
        }

        [Test, ExpectedArgumentException]
        public void ConstructorThrowsWhenAbstractClassProvidedAndThereAreConstructorParameters()
        {
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(AbstractClassOneParamConstructorInfo.Parameters[0], 42);

            new ObjectCreationSpec(AbstractClassInfo, slotValues, NullConverter.Instance);
        }

        [Test, ExpectedArgumentException]
        public void ConstructorThrowsWhenConstructorParameterSlotBelongsToDifferentType()
        {
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(NonGenericClassOneParamConstructorInfo.Parameters[0], 42);

            new ObjectCreationSpec(GenericClassDefInfo, slotValues, NullConverter.Instance);
        }

        [Test, ExpectedArgumentException]
        public void ConstructorThrowsWhenFieldSlotBelongsToDifferentType()
        {
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(NonGenericClassFieldInfo, 42);

            new ObjectCreationSpec(GenericClassDefInfo, slotValues, NullConverter.Instance);
        }

        [Test, ExpectedArgumentException]
        public void ConstructorThrowsWhenPropertySlotBelongsToDifferentType()
        {
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(NonGenericClassPropertyInfo, 42);

            new ObjectCreationSpec(GenericClassDefInfo, slotValues, NullConverter.Instance);
        }

        [Test, ExpectedArgumentException]
        public void ConstructorThrowsWhenGenericClassParamSlotBelongsToDifferentType()
        {
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(GenericClassParamInfo, 42);

            new ObjectCreationSpec(GenericClassInstInfo, slotValues, NullConverter.Instance);
        }

        [Test, ExpectedArgumentException]
        public void ConstructorThrowsWhenMethodParamSlotUsedInsteadOfAConstructorParam()
        {
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(NonGenericClassMethodInfo.Parameters[0], 42);

            new ObjectCreationSpec(NonGenericClassInfo, slotValues, NullConverter.Instance);
        }

        [Test, ExpectedArgumentException]
        public void ConstructorThrowsWhenMissingGenericParamSlotForGenericClassDef()
        {
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();

            new ObjectCreationSpec(GenericClassDefInfo, slotValues, NullConverter.Instance);
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsWhenGenericParamSlotForGenericClassDefIsNull()
        {
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(GenericClassParamInfo, null);

            new ObjectCreationSpec(GenericClassDefInfo, slotValues, NullConverter.Instance);
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void ConstructorThrowsWhenGenericParamSlotForGenericClassDefIsNotAssignedToAType()
        {
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(GenericClassParamInfo, 42);

            new ObjectCreationSpec(GenericClassDefInfo, slotValues, NullConverter.Instance);
        }

        [Test]
        public void CreateInstanceWithNonInstantiableClassThrows()
        {
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();

            ObjectCreationSpec spec = new ObjectCreationSpec(AbstractClassInfo, slotValues, NullConverter.Instance);
            InterimAssert.Throws<InvalidOperationException>(delegate { spec.CreateInstance(); });
        }

        [Test]
        public void CreateInstanceWithGenericClass()
        {
            ITypeInfo type = Reflector.Wrap(typeof(GenericClass<>));
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add((IGenericParameterInfo)type.GenericArguments[0], typeof(int));
            slotValues.Add(type.GetConstructors(PublicInstance)[1].Parameters[0], 1);
            slotValues.Add(type.GetFields(PublicInstance)[0], 2);
            slotValues.Add(type.GetProperties(PublicInstance)[0], 3);

            ObjectCreationSpec spec = new ObjectCreationSpec(type, slotValues, NullConverter.Instance);
            GenericClass<int> instance = (GenericClass<int>) spec.CreateInstance();
            Assert.AreEqual(1, instance.constructorParamValue);
            Assert.AreEqual(2, instance.fieldValue);
            Assert.AreEqual(3, instance.propertyValue);
        }

        [Test]
        public void CreateInstanceWithGenericClassDefaultConstructor()
        {
            ITypeInfo type = Reflector.Wrap(typeof(GenericClass<>));
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add((IGenericParameterInfo)type.GenericArguments[0], typeof(int));
            slotValues.Add(type.GetFields(PublicInstance)[0], 2);
            slotValues.Add(type.GetProperties(PublicInstance)[0], 3);

            ObjectCreationSpec spec = new ObjectCreationSpec(type, slotValues, NullConverter.Instance);
            GenericClass<int> instance = (GenericClass<int>)spec.CreateInstance();
            Assert.AreEqual(0, instance.constructorParamValue);
            Assert.AreEqual(2, instance.fieldValue);
            Assert.AreEqual(3, instance.propertyValue);
        }

        [Test]
        public void CreateInstanceWithGenericClassInstantiation()
        {
            ITypeInfo type = Reflector.Wrap(typeof(GenericClass<int>));
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(type.GetConstructors(PublicInstance)[1].Parameters[0], 1);
            slotValues.Add(type.GetFields(PublicInstance)[0], 2);
            slotValues.Add(type.GetProperties(PublicInstance)[0], 3);

            ObjectCreationSpec spec = new ObjectCreationSpec(type, slotValues, NullConverter.Instance);
            GenericClass<int> instance = (GenericClass<int>)spec.CreateInstance();
            Assert.AreEqual(1, instance.constructorParamValue);
            Assert.AreEqual(2, instance.fieldValue);
            Assert.AreEqual(3, instance.propertyValue);
        }

        [Test]
        public void CreateInstanceWithGenericClassInstantiationDefaultConstructor()
        {
            ITypeInfo type = Reflector.Wrap(typeof(GenericClass<int>));
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(type.GetFields(PublicInstance)[0], 2);
            slotValues.Add(type.GetProperties(PublicInstance)[0], 3);

            ObjectCreationSpec spec = new ObjectCreationSpec(type, slotValues, NullConverter.Instance);
            GenericClass<int> instance = (GenericClass<int>)spec.CreateInstance();
            Assert.AreEqual(0, instance.constructorParamValue);
            Assert.AreEqual(2, instance.fieldValue);
            Assert.AreEqual(3, instance.propertyValue);
        }

        [Test]
        public void CreateInstanceWithGenericStruct()
        {
            ITypeInfo type = Reflector.Wrap(typeof(GenericStruct<>));
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add((IGenericParameterInfo)type.GenericArguments[0], typeof(int));
            slotValues.Add(type.GetConstructors(PublicInstance)[0].Parameters[0], 1);
            slotValues.Add(type.GetFields(PublicInstance)[0], 2);
            slotValues.Add(type.GetProperties(PublicInstance)[0], 3);

            ObjectCreationSpec spec = new ObjectCreationSpec(type, slotValues, NullConverter.Instance);
            GenericStruct<int> instance = (GenericStruct<int>)spec.CreateInstance();
            Assert.AreEqual(1, instance.constructorParamValue);
            Assert.AreEqual(2, instance.fieldValue);
            Assert.AreEqual(3, instance.propertyValue);
        }

        [Test]
        public void CreateInstanceWithGenericStructDefaultConstructor()
        {
            ITypeInfo type = Reflector.Wrap(typeof(GenericStruct<>));
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add((IGenericParameterInfo)type.GenericArguments[0], typeof(int));
            slotValues.Add(type.GetFields(PublicInstance)[0], 2);
            slotValues.Add(type.GetProperties(PublicInstance)[0], 3);

            ObjectCreationSpec spec = new ObjectCreationSpec(type, slotValues, NullConverter.Instance);
            GenericStruct<int> instance = (GenericStruct<int>)spec.CreateInstance();
            Assert.AreEqual(0, instance.constructorParamValue);
            Assert.AreEqual(2, instance.fieldValue);
            Assert.AreEqual(3, instance.propertyValue);
        }

        [Test]
        public void CreateInstanceWithGenericStructInstantiation()
        {
            ITypeInfo type = Reflector.Wrap(typeof(GenericStruct<int>));
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(type.GetConstructors(PublicInstance)[0].Parameters[0], 1);
            slotValues.Add(type.GetFields(PublicInstance)[0], 2);
            slotValues.Add(type.GetProperties(PublicInstance)[0], 3);

            ObjectCreationSpec spec = new ObjectCreationSpec(type, slotValues, NullConverter.Instance);
            GenericStruct<int> instance = (GenericStruct<int>)spec.CreateInstance();
            Assert.AreEqual(1, instance.constructorParamValue);
            Assert.AreEqual(2, instance.fieldValue);
            Assert.AreEqual(3, instance.propertyValue);
        }

        [Test]
        public void CreateInstanceWithGenericStructInstantiationDefaultConstructor()
        {
            ITypeInfo type = Reflector.Wrap(typeof(GenericStruct<int>));
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(type.GetFields(PublicInstance)[0], 2);
            slotValues.Add(type.GetProperties(PublicInstance)[0], 3);

            ObjectCreationSpec spec = new ObjectCreationSpec(type, slotValues, NullConverter.Instance);
            GenericStruct<int> instance = (GenericStruct<int>)spec.CreateInstance();
            Assert.AreEqual(0, instance.constructorParamValue);
            Assert.AreEqual(2, instance.fieldValue);
            Assert.AreEqual(3, instance.propertyValue);
        }

        [Test]
        public void SpecPropertiesDescribeTheObject()
        {
            ITypeInfo type = Reflector.Wrap(typeof(GenericClass<>));
            IConstructorInfo constructor = type.GetConstructors(PublicInstance)[1];
            IParameterInfo constructorParameter = constructor.Parameters[0];
            IFieldInfo field = type.GetFields(PublicInstance)[0];
            IPropertyInfo property = type.GetProperties(PublicInstance)[0];

            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add((IGenericParameterInfo)type.GenericArguments[0], typeof(int));
            slotValues.Add(constructorParameter, 1);
            slotValues.Add(field, 2);
            slotValues.Add(property, 3);

            ObjectCreationSpec spec = new ObjectCreationSpec(type, slotValues, NullConverter.Instance);
            Assert.AreSame(type, spec.Type);
            Assert.AreSame(slotValues, spec.SlotValues);
            Assert.AreSame(NullConverter.Instance, spec.Converter);
            Assert.AreEqual(typeof(GenericClass<int>), spec.ResolvedType);

            Assert.AreEqual(constructor, Reflector.Wrap(spec.ResolvedConstructor));
            OldCollectionAssert.AreElementsEqual(new object[] { 1 }, spec.ResolvedConstructorArguments);

            List<KeyValuePair<FieldInfo, object>> fieldValues = new List<KeyValuePair<FieldInfo,object>>(spec.ResolvedFieldValues);
            Assert.AreEqual(1, fieldValues.Count);
            Assert.AreEqual(field, Reflector.Wrap(fieldValues[0].Key));
            Assert.AreEqual(2, fieldValues[0].Value);

            List<KeyValuePair<PropertyInfo, object>> propertyValues = new List<KeyValuePair<PropertyInfo, object>>(spec.ResolvedPropertyValues);
            Assert.AreEqual(1, propertyValues.Count);
            Assert.AreEqual(property, Reflector.Wrap(propertyValues[0].Key));
            Assert.AreEqual(3, propertyValues[0].Value);
        }

        [Test]
        public void SpecContainsNullConstructorIfNonInstantiableTypeUsed()
        {
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();

            ObjectCreationSpec spec = new ObjectCreationSpec(AbstractClassInfo, slotValues, NullConverter.Instance);
            Assert.IsNull(spec.ResolvedConstructor);
            Assert.AreEqual(0, spec.ResolvedConstructorArguments.Length);
        }

        [Test]
        public void FormatThrowsIfEntityIsNull()
        {
            ObjectCreationSpec spec = new ObjectCreationSpec(EmptyClassInfo,
                EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance, Mocks.Stub<IConverter>());

            InterimAssert.Throws<ArgumentNullException>(delegate { spec.Format(null, Mocks.Stub<IFormatter>()); });
        }

        [Test]
        public void FormatThrowsIfFormatterIsNull()
        {
            ObjectCreationSpec spec = new ObjectCreationSpec(EmptyClassInfo,
                EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance, Mocks.Stub<IConverter>());

            InterimAssert.Throws<ArgumentNullException>(delegate { spec.Format("Foo", null); });
        }

        [Test]
        public void FormatDescribesTheObject()
        {
            ITypeInfo type = Reflector.Wrap(typeof(GenericClass<>));
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add((IGenericParameterInfo)type.GenericArguments[0], typeof(int));
            slotValues.Add(type.GetConstructors(PublicInstance)[1].Parameters[0], 1);
            slotValues.Add(type.GetFields(PublicInstance)[0], 2);
            slotValues.Add(type.GetProperties(PublicInstance)[0], 3);

            ObjectCreationSpec spec = new ObjectCreationSpec(type, slotValues, NullConverter.Instance);
            Assert.AreEqual("Foo<int>(1): fieldValue=2, Property=3", spec.Format("Foo", RuntimeAccessor.Instance.Resolve<IFormatter>()));
        }

        [Test]
        public void FormatStringIsJustTheEntityIfThereAreNoSlots()
        {
            ITypeInfo type = Reflector.Wrap(typeof(EmptyClass));
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();

            ObjectCreationSpec spec = new ObjectCreationSpec(type, slotValues, NullConverter.Instance);
            Assert.AreEqual("Foo", spec.Format("Foo", RuntimeAccessor.Instance.Resolve<IFormatter>()));
        }

        [Test]
        public void FormatStringShowCommaDelimitedParameters()
        {
            ITypeInfo type = Reflector.Wrap(typeof(NonGenericClass));
            Dictionary<ISlotInfo, object> slotValues = new Dictionary<ISlotInfo, object>();
            slotValues.Add(type.GetConstructors(PublicInstance)[1].Parameters[0], 1);
            slotValues.Add(type.GetConstructors(PublicInstance)[1].Parameters[1], "abc");

            ObjectCreationSpec spec = new ObjectCreationSpec(type, slotValues, NullConverter.Instance);
            Assert.AreEqual("Foo(1, \"abc\")", spec.Format("Foo", RuntimeAccessor.Instance.Resolve<IFormatter>()));
        }

        public class EmptyClass
        {
        }

        public abstract class AbstractClass
        {
            public AbstractClass()
            {
            }

            public AbstractClass(int value)
            {
            }
        }

        public class NonGenericClass
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
        }

        public class GenericClass<T>
        {
            internal T constructorParamValue;
            internal int propertyValue;

            public GenericClass()
            {
            }

            public GenericClass(T constructorParamValue)
            {
                this.constructorParamValue = constructorParamValue;
            }

            public int fieldValue = 0;

            public int Property { set { propertyValue = value; } }
        }

        public struct GenericStruct<T>
        {
            internal T constructorParamValue;
            internal int propertyValue;

            public GenericStruct(T constructorParamValue)
            {
                this.constructorParamValue = constructorParamValue;
                fieldValue = 0;
                propertyValue = 0;
            }

            public int fieldValue;

            public int Property { set { propertyValue = value; } }
        }

        private static T ChooseByParameterCount<T>(IEnumerable<T> functions, int count)
            where T : IFunctionInfo
        {
            foreach (T function in functions)
                if (function.Parameters.Count == count)
                    return function;

            throw new InvalidOperationException("Could not find function.");
        }
    }
}
