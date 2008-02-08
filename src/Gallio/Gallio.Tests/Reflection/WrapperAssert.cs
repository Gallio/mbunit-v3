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
using Gallio.Hosting;
using Gallio.Reflection;
using Gallio.Reflection.Impl;
using MbUnit.Framework;

namespace Gallio.Tests.Reflection
{
    /// <summary>
    /// Assertions for testing the reflection wrappers.
    /// </summary>
    public static class WrapperAssert
    {
        private const BindingFlags All = BindingFlags.Public | BindingFlags.NonPublic
            | BindingFlags.Instance | BindingFlags.Static
            | BindingFlags.FlattenHierarchy;

        public static void AreEquivalent(string namespaceName, INamespaceInfo info)
        {
            Assert.AreEqual(CodeElementKind.Namespace, info.Kind);

            Assert.AreEqual(CodeReference.CreateFromNamespace(namespaceName), info.CodeReference);
            Assert.AreEqual(namespaceName, info.Name);

            Assert.IsFalse(info.GetAttributeInfos(false).GetEnumerator().MoveNext());
            Assert.IsFalse(info.GetAttributes(false).GetEnumerator().MoveNext());
            Assert.IsNull(info.GetCodeLocation());
            Assert.IsNull(info.GetXmlDocumentation());
            Assert.IsFalse(info.HasAttribute(typeof(Attribute), false));
        }

        public static void AreEquivalent(Assembly target, IAssemblyInfo info)
        {
            Assert.AreEqual(CodeElementKind.Assembly, info.Kind);

            Assert.AreEqual(target.FullName, info.ToString());

            Assert.AreEqual(CodeReference.CreateFromAssembly(target), info.CodeReference);
            Assert.AreEqual(target.FullName, info.FullName);
            Assert.AreEqual(target.GetName().Name, info.Name);
            StringAssert.AreEqualIgnoreCase(Loader.GetAssemblyLocalPath(target), info.Path);

            CodeLocation infoLocation = info.GetCodeLocation();
            StringAssert.AreEqualIgnoreCase(Loader.GetAssemblyLocalPath(target), infoLocation.Path);
            Assert.AreEqual(0, infoLocation.Line);
            Assert.AreEqual(0, infoLocation.Column);

            Assert.IsNull(info.GetXmlDocumentation());

            Assert.AreEqual(target.GetName().FullName, info.GetName().FullName);
            InterimAssert.AreElementsEqualIgnoringOrder(
                target.GetReferencedAssemblies(),
                info.GetReferencedAssemblies(),
                delegate(AssemblyName expected, AssemblyName actual) { return expected.FullName == actual.FullName; });

            foreach (Type type in target.GetTypes())
                AreEqualWhenResolved(type, info.GetType(type.FullName));

            AreElementsEqualWhenResolved(target.GetTypes(), info.GetTypes());
            AreElementsEqualWhenResolved(target.GetExportedTypes(), info.GetExportedTypes());

            Assert.AreEqual(target, info.Resolve());

            AreAttributeProvidersEquivalent(target, info);
        }

        public static void AreEquivalent(Attribute target, IAttributeInfo info)
        {
            AreEqualWhenResolved(target.GetType(), info.Type);
            AreEqual(target.GetType(), info.Resolve().GetType());

            foreach (FieldInfo field in target.GetType().GetFields())
                Assert.AreEqual(field.GetValue(target), info.GetFieldValue(field.Name));

            foreach (PropertyInfo property in target.GetType().GetProperties())
                if (property.CanRead && property.CanWrite)
                    Assert.AreEqual(property.GetValue(target, null), info.GetPropertyValue(property.Name));
        }

        public static void AreEquivalent(ConstructorInfo target, IConstructorInfo info)
        {
            Assert.AreEqual(CodeElementKind.Constructor, info.Kind);

            AreFunctionsEquivalent(target, info);
            AreEqual(target, info.Resolve(true));
        }

        public static void AreEquivalent(MethodInfo target, IMethodInfo info)
        {
            Assert.AreEqual(CodeElementKind.Method, info.Kind);

            AreFunctionsEquivalent(target, info);
            AreEqual(target, info.Resolve(true));

            Assert.AreEqual(target.ContainsGenericParameters, info.ContainsGenericParameters);
            Assert.AreEqual(target.IsGenericMethod, info.IsGenericMethod);
            Assert.AreEqual(target.IsGenericMethodDefinition, info.IsGenericMethodDefinition);
            AreElementsEqualWhenResolved(target.GetGenericArguments(), info.GenericArguments);
            AreEqualWhenResolved(target.IsGenericMethod ? target.GetGenericMethodDefinition() : null, info.GenericMethodDefinition);

            AreEqualWhenResolved(target.ReturnType, info.ReturnType);
            AreEqualWhenResolved(target.ReturnParameter, info.ReturnParameter);
        }

        public static void AreEquivalent(EventInfo target, IEventInfo info)
        {
            Assert.AreEqual(CodeElementKind.Event, info.Kind);

            AreMembersEquivalent(target, info);
            AreEqual(target, info.Resolve(true));

            AreEqualWhenResolved(target.GetAddMethod(), info.AddMethod);
            AreEqualWhenResolved(target.GetRemoveMethod(), info.RemoveMethod);
            AreEqualWhenResolved(target.GetRaiseMethod(), info.RaiseMethod);
        }

        public static void AreEquivalent(FieldInfo target, IFieldInfo info)
        {
            Assert.AreEqual(CodeElementKind.Field, info.Kind);

            AreMembersEquivalent(target, info);
            AreEqual(target, info.Resolve(true));

            AreEqualWhenResolved(target.FieldType, info.ValueType);
            Assert.AreEqual(0, info.Position);

            Assert.AreEqual(target.Attributes, info.FieldAttributes);
            Assert.AreEqual(target.IsLiteral, info.IsLiteral);
            Assert.AreEqual(target.IsPublic, info.IsPublic);
            Assert.AreEqual(target.IsInitOnly, info.IsInitOnly);
            Assert.AreEqual(target.IsStatic, info.IsStatic);
        }

        public static void AreEquivalent(PropertyInfo target, IPropertyInfo info)
        {
            Assert.AreEqual(CodeElementKind.Property, info.Kind);

            AreMembersEquivalent(target, info);
            AreEqual(target, info.Resolve(true));

            AreEqualWhenResolved(target.PropertyType, info.ValueType);
            Assert.AreEqual(0, info.Position);

            Assert.AreEqual(target.Attributes, info.PropertyAttributes);

            AreEqualWhenResolved(target.GetGetMethod(), info.GetMethod);
            AreEqualWhenResolved(target.GetSetMethod(), info.SetMethod);
        }

        public static void AreEquivalent(Type target, ITypeInfo info)
        {
            Assert.AreEqual(CodeElementKind.Type, info.Kind);

            AreTypesEquivalent(target, info);
            Assert.AreEqual(target, info.Resolve(true));
        }

        public static void AreEquivalent(Type target, IGenericParameterInfo info)
        {
            Assert.AreEqual(CodeElementKind.GenericParameter, info.Kind);

            AreTypesEquivalent(target, info);
            Assert.AreEqual(target, info.Resolve(true));

            AreEqualWhenResolved(typeof(Type), info.ValueType);
            Assert.IsTrue(info.ContainsGenericParameters);
            Assert.AreEqual(target.GenericParameterPosition, info.Position);
            Assert.AreEqual(target.GenericParameterAttributes, info.GenericParameterAttributes);

            Assert.IsNull(info.FullName);
            Assert.IsNull(info.AssemblyQualifiedName);
        }

        public static void AreEquivalent(ParameterInfo target, IParameterInfo info)
        {
            Assert.AreEqual(CodeElementKind.Parameter, info.Kind);

            AreEqual(target, info.Resolve(true));

            Assert.AreEqual(CodeReference.CreateFromParameter(target), info.CodeReference);
            Assert.AreEqual(target.Name, info.Name);
            Assert.IsNull(info.GetXmlDocumentation());

            AreEqualWhenResolved(target.ParameterType, info.ValueType);
            Assert.AreEqual(target.Position, info.Position);

            Assert.AreEqual(target.Attributes, info.ParameterAttributes);
            AreEqualWhenResolved(target.Member, info.Member);

            AreAttributeProvidersEquivalent(target, info);
        }

        private static void AreFunctionsEquivalent(MethodBase target, IFunctionInfo info)
        {
            AreMembersEquivalent(target, info);
            AreEqual(target, info.Resolve(true));

            Assert.AreEqual(target.IsAbstract, info.IsAbstract);
            Assert.AreEqual(target.IsPublic, info.IsPublic);
            Assert.AreEqual(target.IsStatic, info.IsStatic);
            Assert.AreEqual(target.Attributes, info.MethodAttributes);
            AreElementsEqualWhenResolved(target.GetParameters(), info.Parameters);

            // The source location may not be exactly the same with some wrappers.
            // What's important is that it is in the same file at least.
            CodeLocation targetLocation = DebugSymbolUtils.GetSourceLocation(target)
                ?? DebugSymbolUtils.GetSourceLocation(target.DeclaringType);

            if (targetLocation != null)
                StringAssert.AreEqualIgnoreCase(targetLocation.Path, info.GetCodeLocation().Path);
        }

        private static void AreTypesEquivalent(Type target, ITypeInfo info)
        {
            AreMembersEquivalent(target, info);
            AreEqual(target, info.Resolve(true));

            Assert.AreEqual(target.FullName, info.ToString());

            Assert.AreEqual(target.Assembly, info.Assembly.Resolve());
            Assert.AreEqual(target.Namespace, info.Namespace.Name);
            AreEqualWhenResolved(target.BaseType, info.BaseType);
            Assert.AreEqual(target.AssemblyQualifiedName, info.AssemblyQualifiedName);
            Assert.AreEqual(target.FullName, info.FullName);
            Assert.AreEqual(target.Attributes, info.TypeAttributes);
            AreEqualWhenResolved(target.HasElementType ? target.GetElementType() : null, info.ElementType);
            Assert.AreEqual(target.IsArray, info.IsArray);
            Assert.AreEqual(target.IsPointer, info.IsPointer);
            Assert.AreEqual(target.IsByRef, info.IsByRef);
            Assert.AreEqual(target.IsGenericParameter, info.IsGenericParameter);
            Assert.AreEqual(Type.GetTypeCode(target), info.TypeCode);

            Assert.AreEqual(target.ContainsGenericParameters, info.ContainsGenericParameters);
            Assert.AreEqual(target.IsGenericType, info.IsGenericType);
            Assert.AreEqual(target.IsGenericTypeDefinition, info.IsGenericTypeDefinition);
            AreElementsEqualWhenResolved(target.GetGenericArguments(), info.GenericArguments);
            AreEqualWhenResolved(target.IsGenericType ? target.GetGenericTypeDefinition() : null, info.GenericTypeDefinition);

            if (target.IsArray)
                Assert.AreEqual(target.GetArrayRank(), info.ArrayRank);
            else
                InterimAssert.Throws<InvalidOperationException>(delegate { GC.KeepAlive(info.ArrayRank); });

            AreElementsEqualWhenResolved(target.GetInterfaces(), info.Interfaces);
            AreElementsEqualWhenResolved(target.GetConstructors(All), info.GetConstructors(All));
            AreElementsEqualWhenResolved(target.GetEvents(All), info.GetEvents(All));
            AreElementsEqualWhenResolved(target.GetFields(All), info.GetFields(All));
            AreElementsEqualWhenResolved(target.GetMethods(All), info.GetMethods(All));
            AreElementsEqualWhenResolved(target.GetProperties(All), info.GetProperties(All));

            foreach (MethodInfo method in target.GetMethods(All))
            {
                try
                {
                    target.GetMethod(method.Name, All);
                    InterimAssert.DoesNotThrow(delegate { AreEqualWhenResolved(method, info.GetMethod(method.Name, All)); });
                }
                catch (AmbiguousMatchException)
                {
                    InterimAssert.Throws<AmbiguousMatchException>(delegate { info.GetMethod(method.Name, All); });
                }
            }
        }

        private static void AreMembersEquivalent(MemberInfo target, IMemberInfo info)
        {
            AreEqual(target, info.Resolve(true));

            Assert.AreEqual(target.Name, info.Name);
            Assert.AreEqual(CodeReference.CreateFromMember(target), info.CodeReference);
            Assert.AreEqual(GetCompoundName(target), info.CompoundName);
            AreEqualWhenResolved(target.DeclaringType, info.DeclaringType);
            Assert.AreEqual(XmlDocumentationUtils.GetXmlDocumentation(target), info.GetXmlDocumentation());

            AreAttributeProvidersEquivalent(target, info);
        }

        private static string GetCompoundName(MemberInfo member)
        {
            return member.DeclaringType != null ? GetCompoundName(member.DeclaringType) + "." + member.Name : member.Name;
        }

        private static void AreEqual(Type expected, Type actual)
        {
            Assert.AreEqual(expected, actual);
        }

        private static void AreEqual(MemberInfo expected, MemberInfo actual)
        {
            Assert.AreEqual(expected.MetadataToken, actual.MetadataToken);
        }

        private static void AreEqual(ParameterInfo expected, ParameterInfo actual)
        {
            Assert.AreEqual(expected.MetadataToken, actual.MetadataToken);
        }

        private static void AreEqualWhenResolved(Type expected, ITypeInfo wrapper)
        {
            if (expected != null)
                AreEqual(expected, wrapper.Resolve(true));
            else
                Assert.IsNull(wrapper);
        }

        private static void AreEqualWhenResolved<TMember, TWrapper>(TMember expected, TWrapper wrapper)
            where TMember : MemberInfo
            where TWrapper : IMemberInfo
        {
            if (expected != null)
                AreEqual(expected, wrapper.Resolve(true));
            else
                Assert.IsNull(wrapper);
        }

        private static void AreEqualWhenResolved(ParameterInfo expected, IParameterInfo wrapper)
        {
            if (expected != null)
                AreEqual(expected, wrapper.Resolve(true));
            else
                Assert.IsNull(wrapper);
        }

        private static void AreElementsEqualWhenResolved<TWrapper>(IEnumerable<Type> expectedTypes, IEnumerable<TWrapper> actualTypes)
            where TWrapper : ITypeInfo
        {
            Dictionary<string, Type> keyedExpectedTypes = new Dictionary<string, Type>();
            foreach (Type expectedType in expectedTypes)
                keyedExpectedTypes.Add(expectedType.FullName ?? expectedType.Name, expectedType);

            Dictionary<string, ITypeInfo> keyedActualTypes = new Dictionary<string, ITypeInfo>();
            foreach (ITypeInfo actualType in actualTypes)
                keyedActualTypes.Add(actualType.FullName ?? actualType.Name, actualType);

            InterimAssert.WithKeyedPairs(keyedExpectedTypes, keyedActualTypes, AreEqualWhenResolved);
        }

        private static void AreElementsEqualWhenResolved<TMember, TWrapper>(IEnumerable<TMember> expectedMembers, IEnumerable<TWrapper> actualMembers)
            where TMember : MemberInfo
            where TWrapper : IMemberInfo
        {
            Dictionary<string, TMember> keyedExpectedMembers = new Dictionary<string, TMember>();
            foreach (TMember expectedMember in expectedMembers)
            {
                string key = expectedMember.Name;
                MethodBase method = expectedMember as MethodBase;
                if (method != null)
                    key += method.GetParameters().Length; // note: imprecise overload disambiguation, but ok for now

                keyedExpectedMembers.Add(key, expectedMember);
            }

            Dictionary<string, TWrapper> keyedActualMembers = new Dictionary<string, TWrapper>();
            foreach (TWrapper actualMember in actualMembers)
            {
                string key = actualMember.Name;
                IFunctionInfo method = actualMember as IFunctionInfo;
                if (method != null)
                    key += method.Parameters.Count; // note: imprecise overload disambiguation, but ok for now

                keyedActualMembers.Add(key, actualMember);
            }

            InterimAssert.WithKeyedPairs(keyedExpectedMembers, keyedActualMembers, AreEqualWhenResolved);
        }

        private static void AreElementsEqualWhenResolved(IEnumerable<ParameterInfo> expected, IEnumerable<IParameterInfo> actual)
        {
            InterimAssert.WithPairs(expected, actual, AreEqualWhenResolved);
        }

        private static void AreAttributeProvidersEquivalent<TTarget, TWrapper>(TTarget expectedTarget, TWrapper actualWrapper)
            where TTarget : ICustomAttributeProvider
            where TWrapper : ICodeElementInfo
        {
            object[] nonInheritedAttribs = expectedTarget.GetCustomAttributes(false);
            object[] inheritedAttribs = expectedTarget.GetCustomAttributes(true);

            Assert.AreEqual(nonInheritedAttribs.Length, new List<object>(actualWrapper.GetAttributes(false)).Count,
                "Number of non-inherited attributes should be equal.");
            Assert.AreEqual(inheritedAttribs.Length, new List<object>(actualWrapper.GetAttributes(true)).Count,
                "Number of inherited attributes should be equal.");
            Assert.AreEqual(nonInheritedAttribs.Length, new List<object>(actualWrapper.GetAttributes(typeof(Attribute), false)).Count,
                "Number of non-inherited attributes should be equal.");
            Assert.AreEqual(inheritedAttribs.Length, new List<object>(actualWrapper.GetAttributes(typeof(Attribute), true)).Count,
                "Number of inherited attributes should be equal.");
            Assert.AreEqual(nonInheritedAttribs.Length, new List<IAttributeInfo>(actualWrapper.GetAttributeInfos(false)).Count,
                "Number of wrapped non-inherited attributes should be equal.");
            Assert.AreEqual(inheritedAttribs.Length, new List<IAttributeInfo>(actualWrapper.GetAttributeInfos(true)).Count,
                "Number of wrapped inherited attributes should be equal.");
            Assert.AreEqual(nonInheritedAttribs.Length, new List<IAttributeInfo>(actualWrapper.GetAttributeInfos(typeof(Attribute), false)).Count,
                "Number of wrapped non-inherited attributes should be equal.");
            Assert.AreEqual(inheritedAttribs.Length, new List<IAttributeInfo>(actualWrapper.GetAttributeInfos(typeof(Attribute), true)).Count,
                "Number of wrapped inherited attributes should be equal.");

            foreach (object nonInheritedAttrib in inheritedAttribs)
            {
                Assert.IsTrue(actualWrapper.HasAttribute(nonInheritedAttrib.GetType(), false),
                    "Should contain non-inherited attributes of same type.");
            }

            foreach (object inheritedAttrib in inheritedAttribs)
            {
                Assert.IsTrue(actualWrapper.HasAttribute(inheritedAttrib.GetType(), true),
                    "Should contain inherited attributes of same type.");
            }
        }
    }
}
