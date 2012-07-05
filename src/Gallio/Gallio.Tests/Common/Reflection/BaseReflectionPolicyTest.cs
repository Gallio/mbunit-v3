// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Common.Collections;
using Gallio.Common.Reflection;
using Gallio.Model.Tree;
using MbUnit.TestResources;
using MbUnit.TestResources.Reflection;
using MbUnit.Framework;
using Test=Gallio.Model.Tree.Test;

namespace Gallio.Tests.Common.Reflection
{
    /// <summary>
    /// An abstract base class that can be used to test reflection wrappers 
    /// provided by a reflection policy on the assumption that wrappers can be
    /// obtained that are equivalent to native reflection objects.
    /// </summary>
    /// <todo>
    /// We still need to test constructed types.
    /// </todo>
    [TestsOn(typeof(IReflectionPolicy))]
    public abstract class BaseReflectionPolicyTest
    {
        private const BindingFlags All = BindingFlags.Public | BindingFlags.NonPublic
            | BindingFlags.Instance | BindingFlags.Static
            | BindingFlags.FlattenHierarchy;
        private WrapperAssert wrapperAssert;

        /// <summary>
        /// Gets the reflection policy to use to obtain wrappers.
        /// </summary>
        protected abstract IReflectionPolicy ReflectionPolicy { get; }

        protected WrapperAssert WrapperAssert
        {
            get { return wrapperAssert; }
        }

        [SetUp]
        public virtual void SetUp()
        {
            wrapperAssert = new WrapperAssert();
        }

        [TearDown]
        public virtual void TearDown()
        {
            wrapperAssert = null;
        }

        protected IAssemblyInfo GetAssembly(Assembly assembly)
        {
            IAssemblyInfo wrapper = ReflectionPolicy.LoadAssembly(assembly.GetName());
            Assert.IsNotNull(wrapper, "Could not find assembly '{0}'.", assembly);
            return wrapper;
        }

        protected ITypeInfo GetType(Type type)
        {
            IAssemblyInfo assembly = GetAssembly(type.Assembly);

            ITypeInfo wrapper;
            if (type.IsGenericType && ! type.IsGenericTypeDefinition)
            {
                wrapper = assembly.GetType(type.GetGenericTypeDefinition().FullName);
                if (wrapper != null)
                {
                    ITypeInfo[] genericArguments = GenericCollectionUtils.ConvertAllToArray<Type, ITypeInfo>(type.GetGenericArguments(), Reflector.Wrap);
                    wrapper = wrapper.MakeGenericType(genericArguments);
                }
            }
            else
            {
                wrapper = assembly.GetType(type.FullName);
            }

            Assert.IsNotNull(wrapper, "Could not find type '{0}'.", type);
            return wrapper;
        }

        protected IConstructorInfo GetConstructor(ConstructorInfo member)
        {
            IConstructorInfo wrapper = GetType(member.DeclaringType).GetConstructors(All)[0];
            Assert.IsNotNull(wrapper, "Could not find constructor '{0}'.", member);
            return wrapper;
        }

        protected IMethodInfo GetMethod(MethodInfo member)
        {
            IMethodInfo wrapper = FindByName(GetType(member.DeclaringType).GetMethods(All), member.Name);
            Assert.IsNotNull(wrapper, "Could not find method '{0}'.", member);
            return wrapper;
        }

        protected IEventInfo GetEvent(EventInfo member)
        {
            IEventInfo wrapper = FindByName(GetType(member.DeclaringType).GetEvents(All), member.Name);
            Assert.IsNotNull(wrapper, "Could not find event '{0}'.", member);
            return wrapper;
        }

        protected IFieldInfo GetField(FieldInfo member)
        {
            IFieldInfo wrapper = FindByName(GetType(member.DeclaringType).GetFields(All), member.Name);
            Assert.IsNotNull(wrapper, "Could not find field '{0}'.", member);
            return wrapper;
        }

        protected IPropertyInfo GetProperty(PropertyInfo member)
        {
            IPropertyInfo wrapper = FindByName(GetType(member.DeclaringType).GetProperties(All), member.Name);
            Assert.IsNotNull(wrapper, "Could not find property '{0}'.", member);
            return wrapper;
        }

        protected IParameterInfo GetMethodParameter(ParameterInfo parameter)
        {
            IParameterInfo wrapper;
            if (parameter.Position == -1)
                wrapper = GetMethod((MethodInfo)parameter.Member).ReturnParameter;
            else
                wrapper = GetMethod((MethodInfo)parameter.Member).Parameters[parameter.Position];

            Assert.IsNotNull(wrapper, "Could not find parameter '{0}'.", parameter);
            return wrapper;
        }

        protected IGenericParameterInfo GetGenericTypeParameter(Type parameter)
        {
            IGenericParameterInfo wrapper = (IGenericParameterInfo)GetType(parameter.DeclaringType).GenericArguments[parameter.GenericParameterPosition];
            Assert.IsNotNull(wrapper, "Could not find generic type parameter '{0}'.", parameter);
            return wrapper;
        }

        protected IGenericParameterInfo GetGenericMethodParameter(Type parameter)
        {
            IGenericParameterInfo wrapper = (IGenericParameterInfo)GetMethod((MethodInfo)parameter.DeclaringMethod).GenericArguments[parameter.GenericParameterPosition];
            Assert.IsNotNull(wrapper, "Could not find generic method parameter '{0}'.", parameter);
            return wrapper;
        }

        private static T FindByName<T>(IEnumerable<T> elements, string name)
            where T : ICodeElementInfo
        {
            return GenericCollectionUtils.Find(elements, delegate(T element)
            {
                return element.Name == name;
            });
        }

        [Test]
        public void AssemblyWrapper()
        {
            Assembly target = typeof(ReflectionPolicySample).Assembly;
            IAssemblyInfo info = GetAssembly(target);

            WrapperAssert.AreEquivalent(target, info, false);
        }

        [Test]
        public void AssemblyWrapper_EqualityAndHashcode()
        {
            VerifyEqualityAndHashcodeContracts<Assembly, IAssemblyInfo>(
                typeof(ReflectionPolicySample).Assembly,
                typeof(Test).Assembly,
                GetAssembly);
        }

        [Test]
        public void NamespaceWrapper()
        {
            string target = typeof(ReflectionPolicySample.Class1).Namespace;
            INamespaceInfo info = GetType(typeof(ReflectionPolicySample.Class1)).Namespace;

            WrapperAssert.AreEquivalent(target, info);
        }

        [Test]
        public void NamespaceWrapper_EqualityAndHashcode()
        {
            VerifyEqualityAndHashcodeContracts<Type, INamespaceInfo>(
                typeof(ReflectionPolicySample),
                typeof(SimpleTest),
                delegate(Type type) { return GetType(type).Namespace; });
        }

        [Test]
        [Row(typeof(ReflectionPolicySample.Class1))]
        [Row(typeof(ReflectionPolicySample.Class3))]
        [Row(typeof(ReflectionPolicySample.Struct1<int, string>))]
        [Row(typeof(ReflectionPolicySample.Struct1<,>))]
        [Row(typeof(ReflectionPolicySample.Interface1))]
        [Row(typeof(ReflectionPolicySample.TortureTest<int>.NestedType))]
        [Row(typeof(ReflectionPolicySample.TortureTest<>.NestedType))]
        [Row(typeof(ReflectionPolicySample.TortureTest<int>.GenericDoublyNestedType<string>))]
        [Row(typeof(ReflectionPolicySample.TortureTest<>.GenericDoublyNestedType<>))]
        public void TypeWrapper(Type target)
        {
            if (WrapperAssert.IsUnsupportedType(target))
                Assert.Inconclusive("Test involves a type that is not supported due to known issues.");

            ITypeInfo info = GetType(target);

            WrapperAssert.AreEquivalent(target, info, false);
        }

        [Test]
        public void TypeWrapper_EqualityAndHashcode()
        {
            VerifyEqualityAndHashcodeContracts<Type, ITypeInfo>(
                typeof(ReflectionPolicySample.Class1),
                typeof(ReflectionPolicySample.Struct1<int, string>),
                GetType);
        }

        [Test]
        [Row(typeof(ReflectionPolicySample.Class1), "Method1")]
        [Row(typeof(ReflectionPolicySample.Class1), "Method2")]
        [Row(typeof(ReflectionPolicySample.Class1), "op_Equality")]
        [Row(typeof(ReflectionPolicySample.Class1), "op_Inequality")]
        public void MethodWrapper(Type type, string methodName)
        {
            MethodInfo target = type.GetMethod(methodName, All);
            IMethodInfo info = GetMethod(target);

            WrapperAssert.AreEquivalent(target, info, false);
        }

        [Test]
        public void MethodWrapper_EqualityAndHashcode()
        {
            VerifyEqualityAndHashcodeContracts<MethodInfo, IMethodInfo>(
                typeof(ReflectionPolicySample.Class1).GetMethod("Method1", All),
                typeof(ReflectionPolicySample.Class1).GetMethod("Method2", All),
                GetMethod);
        }

        [Test]
        [Row(typeof(ReflectionPolicySample.Class1), "Field1")]
        [Row(typeof(ReflectionPolicySample.Class1), "Field2")]
        public void FieldWrapper(Type type, string fieldName)
        {
            FieldInfo target = type.GetField(fieldName, All);
            IFieldInfo info = GetField(target);

            WrapperAssert.AreEquivalent(target, info, false);
        }

        [Test]
        public void FieldWrapper_EqualityAndHashcode()
        {
            VerifyEqualityAndHashcodeContracts<FieldInfo, IFieldInfo>(
                typeof(ReflectionPolicySample.Class1).GetField("Field1", All),
                typeof(ReflectionPolicySample.Class1).GetField("Field2", All),
                GetField);
        }

        [Test]
        [Row(typeof(ReflectionPolicySample.Class1), "Property1")]
        [Row(typeof(ReflectionPolicySample.Class1), "Property2")]
        [Row(typeof(ReflectionPolicySample.Class1), "Property3")]
        public void PropertyWrapper(Type type, string propertyName)
        {
            PropertyInfo target = type.GetProperty(propertyName, All);
            IPropertyInfo info = GetProperty(target);

            WrapperAssert.AreEquivalent(target, info, false);
        }

        [Test]
        public void PropertyWrapper_EqualityAndHashcode()
        {
            VerifyEqualityAndHashcodeContracts<PropertyInfo, IPropertyInfo>(
                typeof(ReflectionPolicySample.Class1).GetProperty("Property1", All),
                typeof(ReflectionPolicySample.Class1).GetProperty("Property2", All),
                GetProperty);
        }

        [Test]
        [Row(typeof(ReflectionPolicySample.Class1), "Event1")]
        [Row(typeof(ReflectionPolicySample.Class1), "Event2")]
        [Row(typeof(ReflectionPolicySample.Class2), "Event2")]
        public void EventWrapper(Type type, string eventName)
        {
            EventInfo target = type.GetEvent(eventName, All);
            IEventInfo info = GetEvent(target);

            WrapperAssert.AreEquivalent(target, info, false);
        }

        [Test]
        public void EventWrapper_EqualityAndHashcode()
        {
            VerifyEqualityAndHashcodeContracts<EventInfo, IEventInfo>(
                typeof(ReflectionPolicySample.Class1).GetEvent("Event1", All),
                typeof(ReflectionPolicySample.Class1).GetEvent("Event2", All),
                GetEvent);
        }

        [Test]
        [Row(typeof(ReflectionPolicySample.Class1))]
        [Row(typeof(ReflectionPolicySample.Struct1<,>))]
        public void ConstructorWrapper(Type type)
        {
            ConstructorInfo target = type.GetConstructors(All)[0];
            IConstructorInfo info = GetConstructor(target);

            WrapperAssert.AreEquivalent(target, info, false);
        }

        [Test]
        public void ConstructorWrapper_EqualityAndHashcode()
        {
            VerifyEqualityAndHashcodeContracts<ConstructorInfo, IConstructorInfo>(
                typeof(ReflectionPolicySample.Class1).GetConstructors(All)[0],
                typeof(ReflectionPolicySample.Struct1<,>).GetConstructors(All)[0],
                GetConstructor);
        }

        [Test]
        [Row(0)]
        [Row(1)]
        [Row(-1)]
        public void ParameterWrapper(int position)
        {
            MethodInfo method = typeof(ReflectionPolicySample.Interface1).GetMethod("Method1");
            ParameterInfo target = position == -1 ? method.ReturnParameter : method.GetParameters()[position];
            IParameterInfo info = GetMethodParameter(target);

            WrapperAssert.AreEquivalent(target, info, false);
        }

        [Test]
        public void ParameterWrapper_EqualityAndHashcode()
        {
            MethodInfo method = typeof(ReflectionPolicySample.Interface1).GetMethod("Method1");

            VerifyEqualityAndHashcodeContracts<ParameterInfo, IParameterInfo>(
                method.GetParameters()[0],
                method.GetParameters()[1],
                GetMethodParameter);
        }

        [Test]
        [Row(0)]
        [Row(1)]
        public void GenericParameterWrapper(int position)
        {
            Type target = typeof(ReflectionPolicySample.Struct1<,>).GetGenericArguments()[position];
            IGenericParameterInfo info = GetGenericTypeParameter(target);

            WrapperAssert.AreEquivalent(target, info, false);
        }

        [Test]
        public void GenericParameterWrapper_EqualityAndHashcode()
        {
            VerifyEqualityAndHashcodeContracts<Type, IGenericParameterInfo>(
                typeof(ReflectionPolicySample.Struct1<,>).GetGenericArguments()[0],
                typeof(ReflectionPolicySample.Struct1<,>).GetGenericArguments()[1],
                GetGenericTypeParameter);
        }

        [Test]
        [Row(typeof(ReflectionPolicySample.Class3), 0)]
        [Row(typeof(ReflectionPolicySample.Class3), 1)]
        [Row(typeof(ReflectionPolicySample.Interface1), 0)]
        [Row(typeof(ReflectionPolicySample.Interface2), 0)]
        public void AttributeWrapper(Type type, int index)
        {
            SampleAttribute target = (SampleAttribute)type.GetCustomAttributes(typeof(SampleAttribute), true)[index];
            IAttributeInfo info = GenericCollectionUtils.ToArray(GetType(type).GetAttributeInfos(Reflector.Wrap(typeof(SampleAttribute)), true))[index];

            WrapperAssert.AreEquivalent(target, info, false);

            SampleAttribute resolvedAttrib = (SampleAttribute)info.Resolve(true);
            Assert.AreEqual(target.param, resolvedAttrib.param);
            Assert.AreEqual(target.Field, resolvedAttrib.Field);
            Assert.AreEqual(target.Property, resolvedAttrib.Property);

            try
            {
                WrapperAssert.AreEquivalent(typeof(SampleAttribute).GetConstructors()[0], info.Constructor, false);
            }
            catch (NotSupportedException)
            {
                // This is also acceptable behavior.
            }
            Dictionary<IFieldInfo, object> fieldValues = new Dictionary<IFieldInfo, object>();
            foreach (KeyValuePair<IFieldInfo, ConstantValue> entry in info.InitializedFieldValues)
                fieldValues.Add(entry.Key, entry.Value.Resolve(true));

            Dictionary<IPropertyInfo, object> propertyValues = new Dictionary<IPropertyInfo, object>();
            foreach (KeyValuePair<IPropertyInfo, ConstantValue> entry in info.InitializedPropertyValues)
                propertyValues.Add(entry.Key, entry.Value.Resolve(true));

            if (target.param == typeof(int))
            {
                try
                {
                    object[] values = GenericCollectionUtils.ConvertAllToArray<ConstantValue, object>(info.InitializedArgumentValues,
                        delegate(ConstantValue constantValue) { return constantValue.Resolve(true); });

                    Assert.AreElementsEqual(new object[] { typeof(int) }, values);
                }
                catch (NotSupportedException)
                {
                    // This is also acceptable behavior.
                }

                if (fieldValues.Count != 0)
                {
                    Assert.AreEqual(1, fieldValues.Count, "The implementation may return values for uninitialized fields, but there is only one such field.");
                    Assert.AreEqual(0, fieldValues[GetField(typeof(SampleAttribute).GetField("Field"))]);
                }

                if (propertyValues.Count != 0)
                {
                    Assert.AreEqual(1, propertyValues.Count, "The implementation may return values uninitialized properties, but there is only one such field.");
                    Assert.AreEqual(null, propertyValues[GetProperty(typeof(SampleAttribute).GetProperty("Property"))]);
                }
            }
            else
            {
                try
                {
                    object[] values = GenericCollectionUtils.ConvertAllToArray<ConstantValue, object>(info.InitializedArgumentValues,
                        delegate(ConstantValue constantValue) { return constantValue.Resolve(true); });

                    Assert.AreElementsEqual(new object[] { typeof(string[]) }, values);
                }
                catch (NotSupportedException)
                {
                    // This is also acceptable behavior.
                }


                Assert.AreElementsEqual(new KeyValuePair<IFieldInfo, object>[] {
                    new KeyValuePair<IFieldInfo, object>(GetField(typeof(SampleAttribute).GetField("Field")), 2)
                }, fieldValues);

                Assert.AreElementsEqual(new KeyValuePair<IPropertyInfo, object>[] {
                    new KeyValuePair<IPropertyInfo, object>(GetProperty(typeof(SampleAttribute).GetProperty("Property")), "foo")
                }, propertyValues);
            }
        }

        [Test]
        public void ArraysOfRank1()
        {
            ITypeInfo type = GetType(typeof(ReflectionPolicySample));
            WrapperAssert.AreEquivalent(typeof(ReflectionPolicySample).MakeArrayType(), type.MakeArrayType(1), false);
        }

        [Test]
        public void ArraysOfRank3()
        {
            ITypeInfo type = GetType(typeof(ReflectionPolicySample));
            WrapperAssert.AreEquivalent(typeof(ReflectionPolicySample).MakeArrayType(3), type.MakeArrayType(3), false);
        }

        [Test]
        public void ByRefTypes()
        {
            ITypeInfo type = GetType(typeof(ReflectionPolicySample));
            WrapperAssert.AreEquivalent(typeof(ReflectionPolicySample).MakeByRefType(), type.MakeByRefType(), false);
        }

        [Test]
        public void PointerTypes()
        {
            ITypeInfo type = GetType(typeof(ReflectionPolicySample));
            WrapperAssert.AreEquivalent(typeof(ReflectionPolicySample).MakePointerType(), type.MakePointerType(), false);
        }

        [Test]
        public void GenericTypeInstantiations()
        {
            ITypeInfo type = GetType(typeof(ReflectionPolicySample.Struct1<,>));
            WrapperAssert.AreEquivalent(typeof(ReflectionPolicySample.Struct1<int,string>),
                type.MakeGenericType(new ITypeInfo[] { GetType(typeof(int)), GetType(typeof(string)) }), false);
        }

        [Test]
        public void GenericMethodInstantiations()
        {
            MethodInfo target = typeof(ReflectionPolicySample.Class1).GetMethod("Method1");
            IMethodInfo method = GetMethod(target);
            WrapperAssert.AreEquivalent(target.MakeGenericMethod(typeof(int)),
                method.MakeGenericMethod(new ITypeInfo[] { GetType(typeof(int)) }), false);
        }

        [Test]
        public void IsSubclassOf_WhenTypeIsNull_ReturnsFalse()
        {
            ITypeInfo type = GetType(typeof(ReflectionPolicySample.Class2));

            Assert.IsFalse(type.IsSubclassOf(null));
        }

        [Test]
        public void IsSubclassOf_WhenTypeIsSelf_ReturnsFalse()
        {
            ITypeInfo type = GetType(typeof(ReflectionPolicySample.Class2));

            Assert.IsFalse(type.IsSubclassOf(type));
        }

        [Test]
        public void IsSubclassOf_WhenTypeIsSubclass_ReturnsFalse()
        {
            ITypeInfo type = GetType(typeof(ReflectionPolicySample.Class2));
            ITypeInfo subclass = GetType(typeof(ReflectionPolicySample.Class3));

            Assert.IsFalse(type.IsSubclassOf(subclass));
        }

        [Test]
        public void IsSubclassOf_WhenTypeIsSuperclass_ReturnsTrue()
        {
            ITypeInfo type = GetType(typeof(ReflectionPolicySample.Class2));
            ITypeInfo superclass = GetType(typeof(ReflectionPolicySample.Class1));

            Assert.IsTrue(type.IsSubclassOf(superclass));
        }

        [Test]
        public void IsSubclassOf_WhenTypeIsObject_ReturnsTrue()
        {
            ITypeInfo type = GetType(typeof(ReflectionPolicySample.Class2));
            ITypeInfo objectType = GetType(typeof(Object));

            Assert.IsTrue(type.IsSubclassOf(objectType));
        }

        [Test]
        public void IsAssignableFrom_WhenTypeIsNull_ReturnsFalse()
        {
            ITypeInfo type = GetType(typeof(ReflectionPolicySample.Class2));

            Assert.IsFalse(type.IsAssignableFrom(null));
        }

        [Test]
        public void IsAssignableFrom_WhenTypeIsSelf_ReturnsTrue()
        {
            ITypeInfo type = GetType(typeof(ReflectionPolicySample.Class2));

            Assert.IsTrue(type.IsAssignableFrom(type));
        }

        [Test]
        public void IsAssignableFrom_WhenTypeIsSubclass_ReturnsTrue()
        {
            ITypeInfo type = GetType(typeof(ReflectionPolicySample.Class2));
            ITypeInfo subclass = GetType(typeof(ReflectionPolicySample.Class3));

            Assert.IsTrue(type.IsAssignableFrom(subclass));
        }

        [Test]
        public void IsAssignableFrom_WhenTypeIsSuperclass_ReturnsFalse()
        {
            ITypeInfo type = GetType(typeof(ReflectionPolicySample.Class2));
            ITypeInfo superclass = GetType(typeof(ReflectionPolicySample.Class1));

            Assert.IsFalse(type.IsAssignableFrom(superclass));
        }

        [Test]
        public void IsAssignableFrom_WhenTypeImplementsInterface_ReturnsTrue()
        {
            ITypeInfo type = GetType(typeof(ReflectionPolicySample.Interface1));
            ITypeInfo implementor = GetType(typeof(ReflectionPolicySample.Struct1<int, int>));

            Assert.IsTrue(type.IsAssignableFrom(implementor));
        }

        [Test]
        public void IsAssignableFrom_WhenTypeDoesNotImplementInterface_ReturnsFalse()
        {
            ITypeInfo type = GetType(typeof(ReflectionPolicySample.Interface1));
            ITypeInfo nonImplementor = GetType(typeof(ReflectionPolicySample.Class1));

            Assert.IsFalse(type.IsAssignableFrom(nonImplementor));
        }

        [Test]
        [Row(typeof(ReflectionPolicySample.TortureTest<string>))]
        [Row(typeof(ReflectionPolicySample.TortureTest2<string>))]
        [Row(typeof(ReflectionPolicySampleInGlobalNamespace))]
        public void TortureTest(Type target)
        {
            ITypeInfo info = GetType(target);

            WrapperAssert.AreEquivalent(target, info, true);
        }

        [Test]
        public void ExhaustiveComparison()
        {
            Assembly assembly = typeof(ReflectionPolicySample).Assembly;
            IAssemblyInfo info =GetAssembly(assembly);

            WrapperAssert.AreEquivalent(assembly, info, true);
        }

        [Test]
        public void NamespaceOfGlobalTypeIsEmpty()
        {
            ITypeInfo info = GetType(typeof(ReflectionPolicySampleInGlobalNamespace));
            Assert.IsNotNull(info.Namespace);
            Assert.AreEqual("", info.Namespace.Name);
            Assert.AreEqual("", info.NamespaceName);
        }

        [Test]
        public void AssemblyCodeLocation()
        {
            Assembly assembly = typeof(ReflectionPolicySample).Assembly;
            IAssemblyInfo info = GetAssembly(assembly);

            var codeLocation = info.GetCodeLocation();
            Assert.AreEqual(info.Path, codeLocation.Path);
        }

        [Test]
        public void TypeCodeLocation()
        {
            Type target = typeof(ReflectionPolicySample.Class1);
            ITypeInfo info = GetType(target);

            var codeLocation = info.GetCodeLocation();
            if (codeLocation != CodeLocation.Unknown)
                Assert.EndsWith(codeLocation.Path, "ReflectionPolicySample.cs");
        }

        [Test]
        public void MethodCodeLocation()
        {
            MethodInfo target = typeof(ReflectionPolicySample.Class1).GetMethod("Method1");
            IMethodInfo info = GetMethod(target);

            var codeLocation = info.GetCodeLocation();
            if (codeLocation != CodeLocation.Unknown)
                Assert.EndsWith(codeLocation.Path, "ReflectionPolicySample.cs");
        }

        protected static void VerifyEqualityAndHashcodeContracts<TTarget, TWrapper>(
            TTarget target1, TTarget target2, Gallio.Common.Func<TTarget, TWrapper> wrapperFactory)
        {
            Assert.AreNotEqual(target1, target2, "Precondition for this method is that targets are different.");

            Assert.IsTrue(wrapperFactory(target1).Equals((object)wrapperFactory(target1)));
            Assert.IsFalse(wrapperFactory(target1).Equals((object)wrapperFactory(target2)));
            Assert.IsFalse(wrapperFactory(target1).Equals((object) null));

            Assert.AreEqual(wrapperFactory(target1).GetHashCode(), wrapperFactory(target1).GetHashCode());

            foreach (Type @interface in typeof(TWrapper).GetInterfaces())
            {
                if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IEquatable<>))
                {
                    MethodInfo equals = @interface.GetMethod("Equals");

                    Assert.IsTrue((bool)equals.Invoke(wrapperFactory(target1), new object[] { wrapperFactory(target1) }));
                    Assert.IsFalse((bool)equals.Invoke(wrapperFactory(target1), new object[] { wrapperFactory(target2) }));
                    Assert.IsFalse((bool)equals.Invoke(wrapperFactory(target1), new object[] { null }));
                }
            }
        }

        [Test]
        public void GetTypesIncludesNestedTypes()
        {
            Assembly assembly = typeof(ReflectionPolicySample).Assembly;
            IAssemblyInfo info = GetAssembly(assembly);
            ITypeInfo expectedType = GetType(typeof(ReflectionPolicySample.Class1));

            IList<ITypeInfo> types = info.GetTypes();

            Assert.IsTrue(types.Contains(expectedType));
        }
    }
}
