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
using Gallio.Model;
using Gallio.Reflection;
using MbUnit.TestResources;
using MbUnit.TestResources.Reflection;
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
            IAssemblyInfo wrapper = ReflectionPolicy.LoadAssembly(assembly.GetName());
            Assert.IsNotNull(wrapper, "Could not find assembly '{0}'.", assembly);
            return wrapper;
        }

        protected ITypeInfo GetType(Type type)
        {
            ITypeInfo wrapper = GetAssembly(type.Assembly).GetType(type.FullName);
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
            IMethodInfo wrapper =  FindByName(GetType(member.DeclaringType).GetMethods(All), member.Name);
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
            return GenericUtils.Find(elements, delegate(T element)
            {
                return element.Name == name;
            });
        }

        [Test]
        public void AssemblyWrapper()
        {
            Assembly target = typeof(ReflectionPolicySample).Assembly;
            IAssemblyInfo info = GetAssembly(target);

            WrapperAssert.AreEquivalent(target, info);
        }

        [Test]
        public void AssemblyWrapper_EqualityAndHashcode()
        {
            VerifyEqualityAndHashcodeContracts<Assembly, IAssemblyInfo>(
                typeof(ReflectionPolicySample).Assembly,
                typeof(ITest).Assembly,
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

        [RowTest]
        [Row(typeof(ReflectionPolicySample.Class1))]
        [Row(typeof(ReflectionPolicySample.Class3))]
        [Row(typeof(ReflectionPolicySample.Struct1<int, string>))]
        [Row(typeof(ReflectionPolicySample.Interface1))]
        public void TypeWrapper(Type target)
        {
            ITypeInfo info = GetType(target);

            WrapperAssert.AreEquivalent(target, info);
        }

        [Test]
        public void TypeWrapper_EqualityAndHashcode()
        {
            VerifyEqualityAndHashcodeContracts<Type, ITypeInfo>(
                typeof(ReflectionPolicySample.Class1),
                typeof(ReflectionPolicySample.Struct1<int, string>),
                GetType);
        }

        [RowTest]
        [Row(typeof(ReflectionPolicySample.Class1), "Method1")]
        [Row(typeof(ReflectionPolicySample.Class1), "Method2")]
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
                typeof(ReflectionPolicySample.Class1).GetMethod("Method1", All),
                typeof(ReflectionPolicySample.Class1).GetMethod("Method2", All),
                GetMethod);
        }

        [RowTest]
        [Row(typeof(ReflectionPolicySample.Class1), "Field1")]
        [Row(typeof(ReflectionPolicySample.Class1), "Field2")]
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
                typeof(ReflectionPolicySample.Class1).GetField("Field1", All),
                typeof(ReflectionPolicySample.Class1).GetField("Field2", All),
                GetField);
        }

        [RowTest]
        [Row(typeof(ReflectionPolicySample.Class1), "Property1")]
        [Row(typeof(ReflectionPolicySample.Class1), "Property2")]
        [Row(typeof(ReflectionPolicySample.Class1), "Property3")]
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
                typeof(ReflectionPolicySample.Class1).GetProperty("Property1", All),
                typeof(ReflectionPolicySample.Class1).GetProperty("Property2", All),
                GetProperty);
        }

        [RowTest]
        [Row(typeof(ReflectionPolicySample.Class1), "Event1")]
        [Row(typeof(ReflectionPolicySample.Class1), "Event2")]
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
                typeof(ReflectionPolicySample.Class1).GetEvent("Event1", All),
                typeof(ReflectionPolicySample.Class1).GetEvent("Event2", All),
                GetEvent);
        }

        [RowTest]
        [Row(typeof(ReflectionPolicySample.Class1))]
        [Row(typeof(ReflectionPolicySample.Struct1<,>))]
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
                typeof(ReflectionPolicySample.Class1).GetConstructors(All)[0],
                typeof(ReflectionPolicySample.Struct1<,>).GetConstructors(All)[0],
                GetConstructor);
        }

        [RowTest]
        [Row(0)]
        [Row(1)]
        [Row(-1)]
        public void ParameterWrapper(int position)
        {
            MethodInfo method = typeof(ReflectionPolicySample.Interface1).GetMethod("Method1");
            ParameterInfo target = position == -1 ? method.ReturnParameter : method.GetParameters()[position];
            IParameterInfo info = GetMethodParameter(target);

            WrapperAssert.AreEquivalent(target, info);
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

        [RowTest]
        [Row(0)]
        [Row(1)]
        public void GenericParameterWrapper(int position)
        {
            Type target = typeof(ReflectionPolicySample.Struct1<,>).GetGenericArguments()[position];
            IGenericParameterInfo info = GetGenericTypeParameter(target);

            WrapperAssert.AreEquivalent(target, info);
        }

        [Test]
        public void GenericParameterWrapper_EqualityAndHashcode()
        {
            VerifyEqualityAndHashcodeContracts<Type, IGenericParameterInfo>(
                typeof(ReflectionPolicySample.Struct1<,>).GetGenericArguments()[0],
                typeof(ReflectionPolicySample.Struct1<,>).GetGenericArguments()[1],
                GetGenericTypeParameter);
        }

        [RowTest]
        [Row(typeof(ReflectionPolicySample.Class3), 0)]
        [Row(typeof(ReflectionPolicySample.Class3), 1)]
        [Row(typeof(ReflectionPolicySample.Interface1), 0)]
        [Row(typeof(ReflectionPolicySample.Interface2), 0)]
        public void AttributeWrapper(Type type, int index)
        {
            SampleAttribute target = (SampleAttribute)type.GetCustomAttributes(typeof(SampleAttribute), true)[index];
            IAttributeInfo info = GenericUtils.ToArray(GetType(type).GetAttributeInfos(Reflector.Wrap(typeof(SampleAttribute)), true))[index];

            WrapperAssert.AreEquivalent(target, info);

            SampleAttribute resolvedAttrib = (SampleAttribute)info.Resolve();
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

        protected static void VerifyEqualityAndHashcodeContracts<TTarget, TWrapper>(
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
    }
}
