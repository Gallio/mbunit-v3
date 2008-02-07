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

extern alias MbUnit2;

using System;
using System.Collections.Generic;
using System.Reflection;
using Gallio.Collections;
using Gallio.Reflection;
using MbUnit2::MbUnit.Framework;

namespace Gallio.Tests.Reflection
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

        /// <summary>
        /// Gets the reflection policy to use to obtain wrappers.
        /// </summary>
        protected abstract IReflectionPolicy ReflectionPolicy { get; }

        protected IAssemblyInfo GetAssembly(Assembly assembly)
        {
            return ReflectionPolicy.LoadAssembly(assembly.GetName());
        }

        protected ITypeInfo GetType(Type type)
        {
            return GetAssembly(type.Assembly).GetType(type.FullName);
        }

        protected IConstructorInfo GetConstructor(ConstructorInfo member)
        {
            return GetType(member.DeclaringType).GetConstructors(All)[0];
        }

        protected IMethodInfo GetMethod(MethodInfo member)
        {
            return FindByName(GetType(member.DeclaringType).GetMethods(All), member.Name);
        }

        protected IEventInfo GetEvent(EventInfo member)
        {
            return FindByName(GetType(member.DeclaringType).GetEvents(All), member.Name);
        }

        protected IFieldInfo GetField(FieldInfo member)
        {
            return FindByName(GetType(member.DeclaringType).GetFields(All), member.Name);
        }

        protected IPropertyInfo GetProperty(PropertyInfo member)
        {
            return FindByName(GetType(member.DeclaringType).GetProperties(All), member.Name);
        }

        protected IParameterInfo GetMethodParameter(ParameterInfo parameter)
        {
            if (parameter.Position == -1)
                return GetMethod((MethodInfo)parameter.Member).ReturnParameter;
            else
                return GetMethod((MethodInfo)parameter.Member).Parameters[parameter.Position];
        }

        protected IGenericParameterInfo GetGenericTypeParameter(Type parameter)
        {
            return GetType(parameter.DeclaringType).GenericParameters[parameter.GenericParameterPosition];
        }

        protected IGenericParameterInfo GetGenericMethodParameter(Type parameter)
        {
            return GetMethod((MethodInfo)parameter.DeclaringMethod).GenericParameters[parameter.GenericParameterPosition];
        }

        private static T FindByName<T>(IEnumerable<T> elements, string name)
            where T : ICodeElementInfo
        {
            return GenericUtils.Find(elements, delegate(T element)
            {
                return element.Name == name;
            });
        }

        [Test]
        public void AssemblyWrapper()
        {
            Assembly target = typeof(BaseReflectionPolicyTest).Assembly;
            IAssemblyInfo info = GetAssembly(target);

            WrapperAssert.AreEquivalent(target, info);
        }

        [Test]
        public void AssemblyWrapper_EqualityAndHashcode()
        {
            VerifyEqualityAndHashcodeContracts<Assembly, IAssemblyInfo>(
                typeof(BaseReflectionPolicyTest).Assembly,
                typeof(Reflector).Assembly,
                GetAssembly);
        }

        [Test]
        public void NamespaceWrapper()
        {
            string target = typeof(Class1).Namespace;
            INamespaceInfo info = GetType(typeof(Class1)).Namespace;

            WrapperAssert.AreEquivalent(target, info);
        }

        [Test]
        public void NamespaceWrapper_EqualityAndHashcode()
        {
            VerifyEqualityAndHashcodeContracts<Type, INamespaceInfo>(
                typeof(BaseReflectionPolicyTest),
                typeof(Reflector),
                delegate(Type type) { return GetType(type).Namespace; });
        }

        [RowTest]
        [Row(typeof(Class1))]
        [Row(typeof(Class3))]
        [Row(typeof(Struct1<int, string>))]
        [Row(typeof(Interface1))]
        public void TypeWrapper(Type target)
        {
            ITypeInfo info = GetType(target);

            WrapperAssert.AreEquivalent(target, info);
        }

        [Test]
        public void TypeWrapper_EqualityAndHashcode()
        {
            VerifyEqualityAndHashcodeContracts<Type, ITypeInfo>(
                typeof(Class1),
                typeof(Struct1<int, string>),
                GetType);
        }

        [RowTest]
        [Row(typeof(Class1), "Method1")]
        [Row(typeof(Class1), "Method2")]
        public void MethodWrapper(Type type, string methodName)
        {
            MethodInfo target = type.GetMethod(methodName, All);
            IMethodInfo info = GetMethod(target);

            WrapperAssert.AreEquivalent(target, info);
        }

        [Test]
        public void MethodWrapper_EqualityAndHashcode()
        {
            VerifyEqualityAndHashcodeContracts<MethodInfo, IMethodInfo>(
                typeof(Class1).GetMethod("Method1", All),
                typeof(Class1).GetMethod("Method2", All),
                GetMethod);
        }

        [RowTest]
        [Row(typeof(Class1), "Field1")]
        [Row(typeof(Class1), "Field2")]
        public void FieldWrapper(Type type, string fieldName)
        {
            FieldInfo target = type.GetField(fieldName, All);
            IFieldInfo info = GetField(target);

            WrapperAssert.AreEquivalent(target, info);
        }

        [Test]
        public void FieldWrapper_EqualityAndHashcode()
        {
            VerifyEqualityAndHashcodeContracts<FieldInfo, IFieldInfo>(
                typeof(Class1).GetField("Field1", All),
                typeof(Class1).GetField("Field2", All),
                GetField);
        }

        [RowTest]
        [Row(typeof(Class1), "Property1")]
        [Row(typeof(Class1), "Property2")]
        [Row(typeof(Class1), "Property3")]
        public void PropertyWrapper(Type type, string propertyName)
        {
            PropertyInfo target = type.GetProperty(propertyName, All);
            IPropertyInfo info = GetProperty(target);

            WrapperAssert.AreEquivalent(target, info);
        }

        [Test]
        public void PropertyWrapper_EqualityAndHashcode()
        {
            VerifyEqualityAndHashcodeContracts<PropertyInfo, IPropertyInfo>(
                typeof(Class1).GetProperty("Property1", All),
                typeof(Class1).GetProperty("Property2", All),
                GetProperty);
        }

        [RowTest]
        [Row(typeof(Class1), "Event1")]
        [Row(typeof(Class1), "Event2")]
        public void EventWrapper(Type type, string eventName)
        {
            EventInfo target = type.GetEvent(eventName, All);
            IEventInfo info = GetEvent(target);

            WrapperAssert.AreEquivalent(target, info);
        }

        [Test]
        public void EventWrapper_EqualityAndHashcode()
        {
            VerifyEqualityAndHashcodeContracts<EventInfo, IEventInfo>(
                typeof(Class1).GetEvent("Event1", All),
                typeof(Class1).GetEvent("Event2", All),
                GetEvent);
        }

        [RowTest]
        [Row(typeof(Class1))]
        [Row(typeof(Struct1<,>))]
        public void ConstructorWrapper(Type type)
        {
            ConstructorInfo target = type.GetConstructors(All)[0];
            IConstructorInfo info = GetConstructor(target);

            WrapperAssert.AreEquivalent(target, info);
        }

        [Test]
        public void ConstructorWrapper_EqualityAndHashcode()
        {
            VerifyEqualityAndHashcodeContracts<ConstructorInfo, IConstructorInfo>(
                typeof(Class1).GetConstructors(All)[0],
                typeof(Struct1<,>).GetConstructors(All)[0],
                GetConstructor);
        }

        [RowTest]
        [Row(0)]
        [Row(1)]
        [Row(-1)]
        public void ParameterWrapper(int position)
        {
            MethodInfo method = typeof(Interface1).GetMethod("Method1");
            ParameterInfo target = position == -1 ? method.ReturnParameter : method.GetParameters()[position];
            IParameterInfo info = GetMethodParameter(target);

            WrapperAssert.AreEquivalent(target, info);
        }

        [Test]
        public void ParameterWrapper_EqualityAndHashcode()
        {
            MethodInfo method = typeof(Interface1).GetMethod("Method1");

            VerifyEqualityAndHashcodeContracts<ParameterInfo, IParameterInfo>(
                method.GetParameters()[0],
                method.GetParameters()[1],
                GetMethodParameter);
        }

        [RowTest]
        [Row(0)]
        [Row(1)]
        public void GenericParameterWrapper(int position)
        {
            Type target = typeof(Struct1<,>).GetGenericArguments()[position];
            IGenericParameterInfo info = GetGenericTypeParameter(target);

            WrapperAssert.AreEquivalent(target, info);
        }

        [Test]
        public void GenericParameterWrapper_EqualityAndHashcode()
        {
            VerifyEqualityAndHashcodeContracts<Type, IGenericParameterInfo>(
                typeof(Struct1<,>).GetGenericArguments()[0],
                typeof(Struct1<,>).GetGenericArguments()[1],
                GetGenericTypeParameter);
        }

        [RowTest]
        [Row(typeof(Class3), 0)]
        [Row(typeof(Class3), 1)]
        [Row(typeof(Interface1), 0)]
        [Row(typeof(Interface2), 0)]
        public void AttributeWrapper(Type type, int index)
        {
            SampleAttribute target = (SampleAttribute) type.GetCustomAttributes(typeof(SampleAttribute), true)[index];
            IAttributeInfo info = GenericUtils.ToArray(GetType(type).GetAttributeInfos(true))[index];

            WrapperAssert.AreEquivalent(target, info);

            SampleAttribute resolvedAttrib = (SampleAttribute) info.Resolve();
            Assert.AreEqual(target.param, resolvedAttrib.param);
            Assert.AreEqual(target.Field, resolvedAttrib.Field);
            Assert.AreEqual(target.Property, resolvedAttrib.Property);

            try
            {
                WrapperAssert.AreEquivalent(typeof(SampleAttribute).GetConstructors()[0], info.Constructor);
            }
            catch (NotSupportedException)
            {
                // This is also acceptable behavior.
            }

            if (target.param == typeof(int))
            {
                try
                {
                    CollectionAssert.AreElementsEqual(new object[] { typeof(int) }, info.InitializedArgumentValues);
                }
                catch (NotSupportedException)
                {
                    // This is also acceptable behavior.
                }

                CollectionAssert.AreElementsEqual(new KeyValuePair<IFieldInfo, object>[] {
                    new KeyValuePair<IFieldInfo, object>(GetField(typeof(SampleAttribute).GetField("Field")), 0)
                }, info.InitializedFieldValues);

                CollectionAssert.AreElementsEqual(new KeyValuePair<IPropertyInfo, object>[] {
                    new KeyValuePair<IPropertyInfo, object>(GetProperty(typeof(SampleAttribute).GetProperty("Property")), null)
                }, info.InitializedPropertyValues);
            }
            else
            {
                try
                {
                    CollectionAssert.AreElementsEqual(new object[] { typeof(string[]) }, info.InitializedArgumentValues);
                }
                catch (NotSupportedException)
                {
                    // This is also acceptable behavior.
                }

                CollectionAssert.AreElementsEqual(new KeyValuePair<IFieldInfo, object>[] {
                    new KeyValuePair<IFieldInfo, object>(GetField(typeof(SampleAttribute).GetField("Field")), 2)
                }, info.InitializedFieldValues);

                CollectionAssert.AreElementsEqual(new KeyValuePair<IPropertyInfo, object>[] {
                    new KeyValuePair<IPropertyInfo, object>(GetProperty(typeof(SampleAttribute).GetProperty("Property")), "foo")
                }, info.InitializedPropertyValues);
            }
        }

        private static void VerifyEqualityAndHashcodeContracts<TTarget, TWrapper>(
            TTarget target1, TTarget target2, Func<TTarget, TWrapper> wrapperFactory)
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

        public abstract class Class1
        {
            static Class1() { }

            public void Method1<T>(T param) { Event1 += null; }
            protected abstract int Method2();

            public int Field1;
            internal object Field2 = null;

            public int Property1 { get { return 0; } }
            public int Property2 { get { return 0; } set { } }
            protected abstract string Property3 { set; }

            public event EventHandler Event1;
            protected abstract event EventHandler Event2;
        }

        [Sample(typeof(int))]
        private class Class2 : Class1
        {
            protected override event EventHandler Event2 { add { } remove { } }
            protected override string Property3 { set { } }
            protected override int Method2() { return 0; }
        }

        [Sample(typeof(string[]), Field = 2, Property = "foo")]
        private class Class3 : Class2
        {
        }

        private struct Struct1<S, T> : Interface1
        {
            public Struct1(S s, T t) { }

            string Interface1.Method1(string s, int x) { return ""; }
        }

        [Sample(typeof(string[]), Field=2, Property="foo")]
        public interface Interface1
        {
            string Method1(string s, int x);
        }

        [Sample(typeof(int))]
        public interface Interface2
        {
        }

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple=true, Inherited=true)]
        public class SampleAttribute : Attribute
        {
            internal Type param;
            private string property;

            public SampleAttribute(Type param)
            {
                this.param = param;
            }

            public int Field;
            public string Property { get { return property; } set { property = value; } }
        }
    }
}
