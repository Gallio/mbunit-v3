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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Gallio.Hosting;
using Gallio.Reflection;
using Gallio.Reflection.Impl;
using MbUnit.Framework;
using System.Runtime.CompilerServices;

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

        private const TypeAttributes TypeAttributesMask =
            TypeAttributes.ClassSemanticsMask | TypeAttributes.VisibilityMask
            | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.SpecialName;

        private const GenericParameterAttributes GenericParameterAttributesMask =
            GenericParameterAttributes.SpecialConstraintMask | GenericParameterAttributes.VarianceMask;

        private const MethodAttributes MethodAttributesMask =
            MethodAttributes.MemberAccessMask | MethodAttributes.VtableLayoutMask
            | MethodAttributes.Abstract | MethodAttributes.Final
            | MethodAttributes.PrivateScope | MethodAttributes.SpecialName
            | MethodAttributes.Static | MethodAttributes.Virtual;

        private const ParameterAttributes ParameterAttributesMask =
            ParameterAttributes.HasDefault | ParameterAttributes.Optional
            | ParameterAttributes.In | ParameterAttributes.Out | ParameterAttributes.Retval;

        private const FieldAttributes FieldAttributesMask =
            FieldAttributes.FieldAccessMask | FieldAttributes.HasDefault | FieldAttributes.InitOnly
            | FieldAttributes.Literal | FieldAttributes.SpecialName | FieldAttributes.Static;

        private const PropertyAttributes PropertyAttributesMask =
            PropertyAttributes.HasDefault | PropertyAttributes.SpecialName;

        private const EventAttributes EventAttributesMask =
            EventAttributes.SpecialName;

        public static void AreEquivalent(string namespaceName, INamespaceInfo info)
        {
            Assert.AreEqual(CodeElementKind.Namespace, info.Kind);

            Assert.AreEqual(CodeReference.CreateFromNamespace(namespaceName), info.CodeReference);
            Assert.AreEqual(namespaceName, info.Name);

            Assert.IsFalse(info.GetAttributeInfos(null, false).GetEnumerator().MoveNext());
            Assert.IsFalse(info.GetAttributes(null, false).GetEnumerator().MoveNext());
            Assert.IsFalse(info.HasAttribute(null, false));

            Assert.IsNull(info.GetCodeLocation());
            Assert.IsNull(info.GetXmlDocumentation());
        }

        public static void AreEquivalent(Assembly target, IAssemblyInfo info, bool recursive)
        {
            Assert.AreEqual(CodeElementKind.Assembly, info.Kind);

            string targetDisplayName = target.GetName().Name;
            Assert.AreEqual(targetDisplayName, info.Name);

            if (info.Name != info.FullName)
            {
                Assert.AreEqual(target.FullName, info.ToString());
                Assert.AreEqual(target.FullName, info.FullName);
                Assert.AreEqual(target.GetName().FullName, info.GetName().FullName);
            }
            else
            {
                // We might not always be able to get the full name in the reflection wrapper.
                // A partial name will suffice.

                Assert.AreEqual(targetDisplayName, info.ToString());
                Assert.AreEqual(targetDisplayName, info.FullName);
                Assert.AreEqual(targetDisplayName, info.GetName().Name);
            }

            AreEqualUpToAssemblyDisplayName(CodeReference.CreateFromAssembly(target), info.CodeReference);
            StringAssert.AreEqualIgnoreCase(Path.GetFileName(Loader.GetAssemblyLocalPath(target)), Path.GetFileName(info.Path));

            CodeLocation infoLocation = info.GetCodeLocation();
            StringAssert.AreEqualIgnoreCase(Path.GetFileName(Loader.GetAssemblyLocalPath(target)), Path.GetFileName(infoLocation.Path));
            Assert.AreEqual(0, infoLocation.Line);
            Assert.AreEqual(0, infoLocation.Column);

            Assert.IsNull(info.GetXmlDocumentation());

            InterimAssert.AreElementsEqualIgnoringOrder(
                target.GetReferencedAssemblies(),
                info.GetReferencedAssemblies(),
                delegate(AssemblyName expected, AssemblyName actual) { return expected.FullName == actual.FullName; });

            IList<ITypeInfo> privateTypes = info.GetTypes();
            IList<ITypeInfo> publicTypes = info.GetExportedTypes();

            foreach (Type type in target.GetTypes())
            {
                if (type.IsPublic)
                {
                    ITypeInfo foundType = info.GetType(type.FullName);
                    AreEqualWhenResolved(type, foundType);
                    Assert.IsTrue(publicTypes.Contains(foundType), "Should appear in public types: {0}", type);
                }
            }

            foreach (ITypeInfo type in privateTypes)
            {
                if (type.IsPublic || type.IsNestedPublic)
                    Assert.IsTrue(publicTypes.Contains(type), "Should appear in public types: {0}", type);
                else
                    Assert.IsFalse(publicTypes.Contains(type), "Should not appear in public types: {0}", type);

                AreEqualWhenResolved(Type.GetType(type.AssemblyQualifiedName), type);
            }

            foreach (ITypeInfo type in publicTypes)
            {
                Assert.IsTrue(type.IsPublic || type.IsNestedPublic);
                Assert.IsTrue(privateTypes.Contains(type), "Should appear in private types: {0}", type);
            }

            Assert.AreEqual(target, info.Resolve());

            AreAttributeProvidersEquivalent(target, info);

            if (recursive)
            {
                foreach (Type type in target.GetExportedTypes())
                    AreEquivalent(type, info.GetType(type.AssemblyQualifiedName), true);
            }
        }

        public static void AreEquivalent(Attribute target, IAttributeInfo info, bool recursive)
        {
            AreEqualWhenResolved(target.GetType(), info.Type);
            AreEqual(target.GetType(), info.Resolve().GetType());

            foreach (FieldInfo field in target.GetType().GetFields())
                Assert.AreEqual(field.GetValue(target), info.GetFieldValue(field.Name));

            foreach (PropertyInfo property in target.GetType().GetProperties())
                if (property.CanRead && property.CanWrite)
                    Assert.AreEqual(property.GetValue(target, null), info.GetPropertyValue(property.Name));

            if (recursive)
            {
                AreEquivalent(target.GetType(), info.Type, true);
            }
        }

        public static void AreEquivalent(ConstructorInfo target, IConstructorInfo info, bool recursive)
        {
            Assert.AreEqual(CodeElementKind.Constructor, info.Kind);

            AreFunctionsEquivalent(target, info, recursive);
            AreEqual(target, info.Resolve(true));
        }

        public static void AreEquivalent(MethodInfo target, IMethodInfo info, bool recursive)
        {
            Assert.AreEqual(CodeElementKind.Method, info.Kind);

            AreFunctionsEquivalent(target, info, recursive);
            AreEqual(target, info.Resolve(true));

            Assert.AreEqual(target.ContainsGenericParameters, info.ContainsGenericParameters);
            Assert.AreEqual(target.IsGenericMethod, info.IsGenericMethod);
            Assert.AreEqual(target.IsGenericMethodDefinition, info.IsGenericMethodDefinition);
            AreElementsEqualWhenResolved(target.GetGenericArguments(), info.GenericArguments);
            AreEqualWhenResolved(target.IsGenericMethod ? target.GetGenericMethodDefinition() : null, info.GenericMethodDefinition);

            AreEqualWhenResolved(target.ReturnType, info.ReturnType);
            AreEqualWhenResolved(target.ReturnParameter, info.ReturnParameter);

            if (recursive)
            {
                AreEquivalent(target.GetGenericArguments(), info.GenericArguments, false);
                AreEquivalent(target.ReturnType, info.ReturnType, false);
            }
        }

        public static void AreEquivalent(EventInfo target, IEventInfo info, bool recursive)
        {
            Assert.AreEqual(CodeElementKind.Event, info.Kind);

            AreMembersEquivalent(target, info, recursive);
            AreEqual(target, info.Resolve(true));

            Assert.AreEqual(target.Attributes & EventAttributesMask, info.EventAttributes & EventAttributesMask);
            AreEqualWhenResolved(target.GetAddMethod(true), info.AddMethod);
            AreEqualWhenResolved(target.GetRemoveMethod(true), info.RemoveMethod);
            AreEqualWhenResolved(target.GetRaiseMethod(true), info.RaiseMethod);
            AreEqualWhenResolved(target.EventHandlerType, info.EventHandlerType);

            if (recursive)
            {
                AreEquivalent(target.EventHandlerType, info.EventHandlerType, false);
            }
        }

        public static void AreEquivalent(FieldInfo target, IFieldInfo info, bool recursive)
        {
            Assert.AreEqual(CodeElementKind.Field, info.Kind);

            AreMembersEquivalent(target, info, recursive);
            AreEqual(target, info.Resolve(true));

            AreEqualWhenResolved(target.FieldType, info.ValueType);
            Assert.AreEqual(0, info.Position);

            Assert.AreEqual(target.Attributes & FieldAttributesMask, info.FieldAttributes & FieldAttributesMask);

            Assert.AreEqual(target.IsLiteral, info.IsLiteral);
            Assert.AreEqual(target.IsInitOnly, info.IsInitOnly);
            Assert.AreEqual(target.IsStatic, info.IsStatic);

            Assert.AreEqual(target.IsAssembly, info.IsAssembly);
            Assert.AreEqual(target.IsFamily, info.IsFamily);
            Assert.AreEqual(target.IsFamilyAndAssembly, info.IsFamilyAndAssembly);
            Assert.AreEqual(target.IsFamilyOrAssembly, info.IsFamilyOrAssembly);
            Assert.AreEqual(target.IsPrivate, info.IsPrivate);
            Assert.AreEqual(target.IsPublic, info.IsPublic);

            if (recursive)
            {
                AreEquivalent(target.FieldType, info.ValueType, false);
            }
        }

        public static void AreEquivalent(PropertyInfo target, IPropertyInfo info, bool recursive)
        {
            Assert.AreEqual(CodeElementKind.Property, info.Kind);

            AreMembersEquivalent(target, info, recursive);
            AreEqual(target, info.Resolve(true));

            AreEqualWhenResolved(target.PropertyType, info.ValueType);
            Assert.AreEqual(0, info.Position);

            Assert.AreEqual(target.Attributes & PropertyAttributesMask, info.PropertyAttributes & PropertyAttributesMask);

            Assert.AreEqual(target.CanRead, info.CanRead);
            Assert.AreEqual(target.CanWrite, info.CanWrite);

            AreEqualWhenResolved(target.GetGetMethod(true), info.GetMethod);
            AreEqualWhenResolved(target.GetSetMethod(true), info.SetMethod);
            AreElementsEqualWhenResolved(target.GetIndexParameters(), info.IndexParameters);

            if (recursive)
            {
                AreEquivalent(target.PropertyType, info.ValueType, false);
                AreEquivalent(target.GetIndexParameters(), info.IndexParameters, true);
            }
        }

        public static void AreEquivalent(Type target, ITypeInfo info, bool recursive)
        {
            if (target.IsGenericParameter)
            {
                AreEquivalent(target, (IGenericParameterInfo)info, recursive);
                return;
            }

            Assert.AreEqual(CodeElementKind.Type, info.Kind);

            AreTypesEquivalent(target, info, recursive);
            Assert.AreEqual(target, info.Resolve(true));

            if (recursive)
            {
                if (target.HasElementType)
                    AreEquivalent(target.GetElementType(), info.ElementType, false);

                if (target.BaseType != null)
                    AreEquivalent(target.BaseType, info.BaseType, false);

                AreEquivalent(target.GetGenericArguments(), info.GenericArguments, false);
            }
        }

        public static void AreEquivalent(Type target, IGenericParameterInfo info, bool recursive)
        {
            Assert.AreEqual(CodeElementKind.GenericParameter, info.Kind);

            AreTypesEquivalent(target, info, recursive);
            Assert.AreEqual(target, info.Resolve(true));

            AreEqualWhenResolved(typeof(Type), info.ValueType);
            Assert.IsTrue(info.ContainsGenericParameters);
            Assert.AreEqual(target.GenericParameterPosition, info.Position);
            Assert.AreEqual(target.GenericParameterAttributes & GenericParameterAttributesMask, info.GenericParameterAttributes & GenericParameterAttributesMask);

            Assert.IsNull(info.FullName);
            Assert.IsNull(info.AssemblyQualifiedName);
        }

        public static void AreEquivalent(ParameterInfo target, IParameterInfo info, bool recursive)
        {
            Assert.AreEqual(CodeElementKind.Parameter, info.Kind);

            AreEqual(target, info.Resolve(true));

            AreEqualUpToAssemblyDisplayName(CodeReference.CreateFromParameter(target), info.CodeReference);
            Assert.AreEqual(target.Name, info.Name);
            Assert.IsNull(info.GetXmlDocumentation());

            AreEqualWhenResolved(target.ParameterType, info.ValueType);
            Assert.AreEqual(target.Position, info.Position);

            Assert.AreEqual(target.Attributes & ParameterAttributesMask, info.ParameterAttributes & ParameterAttributesMask);
            AreEqualWhenResolved(target.Member, info.Member);

            AreAttributeProvidersEquivalent(target, info);

            if (recursive)
            {
                AreEquivalent(target.ParameterType, info.ValueType, false);
            }
        }

        private static void AreFunctionsEquivalent(MethodBase target, IFunctionInfo info, bool recursive)
        {
            AreMembersEquivalent(target, info, recursive);
            AreEqual(target, info.Resolve(true));

            Assert.AreEqual(target.IsAbstract, info.IsAbstract);
            Assert.AreEqual(target.IsFinal, info.IsFinal);
            Assert.AreEqual(target.IsStatic, info.IsStatic);
            Assert.AreEqual(target.IsVirtual, info.IsVirtual);

            Assert.AreEqual(target.IsAssembly, info.IsAssembly);
            Assert.AreEqual(target.IsFamily, info.IsFamily);
            Assert.AreEqual(target.IsFamilyAndAssembly, info.IsFamilyAndAssembly);
            Assert.AreEqual(target.IsFamilyOrAssembly, info.IsFamilyOrAssembly);
            Assert.AreEqual(target.IsPrivate, info.IsPrivate);
            Assert.AreEqual(target.IsPublic, info.IsPublic);

            Assert.AreEqual(target.IsHideBySig, info.IsHideBySig);

            Assert.AreEqual(target.Attributes & MethodAttributesMask, info.MethodAttributes & MethodAttributesMask);
            Assert.AreEqual(target.CallingConvention, info.CallingConvention);
            AreElementsEqualWhenResolved(target.GetParameters(), info.Parameters);

            // The source location may not be exactly the same with some wrappers.
            // What's important is that it is in the same file at least when it is available at all.
            CodeLocation targetLocation = DebugSymbolUtils.GetSourceLocation(target)
                ?? DebugSymbolUtils.GetSourceLocation(target.DeclaringType);
            CodeLocation infoLocation = info.GetCodeLocation();

            if (targetLocation != null && infoLocation != null)
                StringAssert.AreEqualIgnoreCase(targetLocation.Path, infoLocation.Path);

            if (recursive)
            {
                AreEquivalent(target.GetParameters(), info.Parameters, true);
            }
        }

        private static void AreTypesEquivalent(Type target, ITypeInfo info, bool recursive)
        {
            AreMembersEquivalent(target, info, recursive);
            AreEqual(target, info.Resolve(true));

            Assert.AreEqual(target.Attributes & TypeAttributesMask, info.TypeAttributes & TypeAttributesMask, target.ToString());
            Assert.AreEqual(target.Assembly, info.Assembly.Resolve(), target.ToString());
            Assert.AreEqual(target.Namespace, info.Namespace.Name, target.ToString());
            AreEqualWhenResolved(target.BaseType, info.BaseType);
            Assert.AreEqual(target.AssemblyQualifiedName, info.AssemblyQualifiedName, target.ToString());
            Assert.AreEqual(target.FullName, info.FullName, target.ToString());
            AreEqualWhenResolved(target.HasElementType ? target.GetElementType() : null, info.ElementType);

            Assert.AreEqual(target.IsAbstract, info.IsAbstract);
            Assert.AreEqual(target.IsClass, info.IsClass);
            Assert.AreEqual(target.IsEnum, info.IsEnum);
            Assert.AreEqual(target.IsInterface, info.IsInterface);
            Assert.AreEqual(target.IsValueType, info.IsValueType);

            Assert.AreEqual(target.IsNested, info.IsNested);
            Assert.AreEqual(target.IsNestedAssembly, info.IsNestedAssembly);
            Assert.AreEqual(target.IsNestedFamANDAssem, info.IsNestedFamilyAndAssembly);
            Assert.AreEqual(target.IsNestedFamily, info.IsNestedFamily);
            Assert.AreEqual(target.IsNestedFamORAssem, info.IsNestedFamilyOrAssembly);
            Assert.AreEqual(target.IsNestedPrivate, info.IsNestedPrivate);
            Assert.AreEqual(target.IsNestedPublic, info.IsNestedPublic);
            Assert.AreEqual(target.IsPublic, info.IsPublic);
            Assert.AreEqual(target.IsNotPublic, info.IsNotPublic);

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

            BindingFlags memberBindingFlags = All;
            AreElementsEqualWhenResolved(target.GetInterfaces(), info.Interfaces);
            AreElementsEqualWhenResolved(target.GetConstructors(memberBindingFlags), info.GetConstructors(memberBindingFlags));
            AreElementsEqualWhenResolved(target.GetEvents(memberBindingFlags), info.GetEvents(memberBindingFlags));
            AreElementsEqualWhenResolved(target.GetFields(memberBindingFlags), info.GetFields(memberBindingFlags));
            AreElementsEqualWhenResolved(target.GetMethods(memberBindingFlags), info.GetMethods(memberBindingFlags));
            AreElementsEqualWhenResolved(target.GetProperties(memberBindingFlags), info.GetProperties(memberBindingFlags));

            foreach (MethodInfo method in target.GetMethods(memberBindingFlags))
            {
                try
                {
                    MethodInfo methodByName = target.GetMethod(method.Name, memberBindingFlags);
                    Assert.IsNotNull(methodByName);

                    // In the case of hidden methods, GetMethod might return a different method than we
                    // expected because the method was hidden by not overridden.  In this case, we need
                    // to perform the comparison on the basis of the named method we obtained rather
                    // than the one we were originally looking for.
                    InterimAssert.DoesNotThrow(delegate
                    {
                        AreEqualWhenResolved(methodByName, info.GetMethod(methodByName.Name, memberBindingFlags));
                    });
                }
                catch (AmbiguousMatchException)
                {
                    InterimAssert.Throws<AmbiguousMatchException>(delegate
                    {
                        Assert.IsNotNull(info.GetMethod(method.Name, memberBindingFlags));
                    });
                }
            }

            foreach (PropertyInfo property in target.GetProperties(memberBindingFlags))
            {
                try
                {
                    PropertyInfo propertyByName = target.GetProperty(property.Name, memberBindingFlags);
                    Assert.IsNotNull(propertyByName);

                    InterimAssert.DoesNotThrow(delegate
                    {
                        AreEqualWhenResolved(propertyByName, info.GetProperty(propertyByName.Name, memberBindingFlags));
                    });
                }
                catch (AmbiguousMatchException)
                {
                    InterimAssert.Throws<AmbiguousMatchException>(delegate
                    {
                        Assert.IsNotNull(info.GetProperty(property.Name, memberBindingFlags));
                    });
                }
            }

            foreach (FieldInfo field in target.GetFields(memberBindingFlags))
            {
                try
                {
                    FieldInfo fieldByName = target.GetField(field.Name, memberBindingFlags);
                    Assert.IsNotNull(fieldByName);

                    InterimAssert.DoesNotThrow(delegate
                    {
                        AreEqualWhenResolved(fieldByName, info.GetField(fieldByName.Name, memberBindingFlags));
                    });
                }
                catch (AmbiguousMatchException)
                {
                    InterimAssert.Throws<AmbiguousMatchException>(delegate
                    {
                        Assert.IsNotNull(info.GetField(field.Name, memberBindingFlags));
                    });
                }
            }

            foreach (EventInfo @event in target.GetEvents(memberBindingFlags))
            {
                try
                {
                    EventInfo eventByName = target.GetEvent(@event.Name, memberBindingFlags);
                    Assert.IsNotNull(eventByName);

                    InterimAssert.DoesNotThrow(delegate
                    {
                        AreEqualWhenResolved(eventByName, info.GetEvent(eventByName.Name, memberBindingFlags));
                    });
                }
                catch (AmbiguousMatchException)
                {
                    InterimAssert.Throws<AmbiguousMatchException>(delegate
                    {
                        Assert.IsNotNull(info.GetEvent(@event.Name, memberBindingFlags));
                    });
                }
            }

            if (recursive)
            {
                if (target.IsGenericType)
                {
                    Type[] genericArguments = target.GetGenericArguments();
                    IList<ITypeInfo> genericArgumentInfos = info.GenericArguments;
                    for (int i = 0; i < genericArguments.Length; i++)
                        AreEquivalent(genericArguments[i], genericArgumentInfos[i], false);
                }

                foreach (FieldInfo field in target.GetFields(memberBindingFlags))
                {
                    IFieldInfo fieldInfo = GetMemberThatIsEqualWhenResolved(field, info.GetFields(memberBindingFlags));
                    AreEquivalent(field, fieldInfo, true);
                }

                foreach (EventInfo @event in target.GetEvents(memberBindingFlags))
                {
                    IEventInfo eventInfo = GetMemberThatIsEqualWhenResolved(@event, info.GetEvents(memberBindingFlags));
                    AreEquivalent(@event, eventInfo, true);
                }

                foreach (PropertyInfo property in target.GetProperties(memberBindingFlags))
                {
                    IPropertyInfo propertyInfo = GetMemberThatIsEqualWhenResolved(property, info.GetProperties(memberBindingFlags));
                    AreEquivalent(property, propertyInfo, true);
                }

                foreach (ConstructorInfo constructor in target.GetConstructors(memberBindingFlags))
                {
                    IConstructorInfo constructorInfo = GetMemberThatIsEqualWhenResolved(constructor, info.GetConstructors(memberBindingFlags));
                    AreEquivalent(constructor, constructorInfo, true);
                }

                foreach (MethodInfo method in target.GetMethods(memberBindingFlags))
                {
                    IMethodInfo methodInfo = GetMemberThatIsEqualWhenResolved(method, info.GetMethods(memberBindingFlags));
                    AreEquivalent(method, methodInfo, true);
                }
            }
        }

        private static void AreMembersEquivalent(MemberInfo target, IMemberInfo info, bool recursive)
        {
            AreEqual(target, info.Resolve(true));

            Assert.AreEqual(target.ToString(), info.ToString());

            Assert.AreEqual(target.Name, info.Name);
            AreEqualUpToAssemblyDisplayName(CodeReference.CreateFromMember(target), info.CodeReference);
            AreEqualWhenResolved(target.DeclaringType, info.DeclaringType);

            string xmlDocumentation = info.GetXmlDocumentation();
            if (xmlDocumentation != null)
                Assert.AreEqual(XmlDocumentationUtils.GetXmlDocumentation(target), xmlDocumentation);

            AreAttributeProvidersEquivalent(target, info);
        }

        private static void AreEquivalent(ParameterInfo[] parameters, IList<IParameterInfo> parameterInfos, bool recursive)
        {
            for (int i = 0; i < parameters.Length; i++)
                AreEquivalent(parameters[i], parameterInfos[i], recursive);
        }

        private static void AreEquivalent(Type[] types, IList<ITypeInfo> typeInfos, bool recursive)
        {
            for (int i = 0; i < types.Length; i++)
                AreEquivalent(types[i], typeInfos[i], recursive);
        }

        private static T GetMemberThatIsEqualWhenResolved<T>(MemberInfo member, IEnumerable<T> memberInfos)
            where T : IMemberInfo
        {
            foreach (T memberInfo in memberInfos)
            {
                MemberInfo resolvedMember = memberInfo.Resolve(true);
                if (member.DeclaringType == resolvedMember.DeclaringType
                    && member.MetadataToken == resolvedMember.MetadataToken)
                    return memberInfo;
            }

            throw new AssertionException(String.Format("Did not find matching member: {0}", member));
        }

        private static void AreEqual(Type expected, Type actual)
        {
            Assert.AreEqual(expected, actual);
        }

        private static void AreEqual(MemberInfo expected, MemberInfo actual)
        {
            Assert.AreEqual(expected.DeclaringType, actual.DeclaringType);
            Assert.AreEqual(expected.MetadataToken, actual.MetadataToken);
        }

        private static void AreEqual(ParameterInfo expected, ParameterInfo actual)
        {
            Assert.AreEqual(expected.Member.DeclaringType, actual.Member.DeclaringType);
            Assert.AreEqual(expected.MetadataToken, actual.MetadataToken);
        }

        private static void AreEqualWhenResolved(Type expected, ITypeInfo wrapper)
        {
            if (expected != null)
                AreEqual(expected, wrapper != null ? wrapper.Resolve(true) : null);
            else
                Assert.IsNull(wrapper);
        }

        private static void AreEqualWhenResolved<TMember, TWrapper>(TMember expected, TWrapper wrapper)
            where TMember : MemberInfo
            where TWrapper : IMemberInfo
        {
            if (expected != null)
                AreEqual(expected, wrapper != null ? wrapper.Resolve(true) : null);
            else
                Assert.IsNull(wrapper);
        }

        private static void AreEqualWhenResolved(ParameterInfo expected, IParameterInfo wrapper)
        {
            if (expected != null)
                AreEqual(expected, wrapper != null ? wrapper.Resolve(true) : null);
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
                string key = expectedMember.DeclaringType + " -> " + expectedMember;
                keyedExpectedMembers.Add(key, expectedMember);
            }

            Dictionary<string, TWrapper> keyedActualMembers = new Dictionary<string, TWrapper>();
            foreach (TWrapper actualMember in actualMembers)
            {
                string key = actualMember.DeclaringType + " -> " + actualMember;

                if (keyedActualMembers.ContainsKey(key))
                    Assert.Fail("Found duplicate member: {0}", key);
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
            // There are some weird cases with attributes of object.
            // For example, the runtime reutrns ComVisibleAttributer and ClassInterfaceAttribute
            // when the "inherit" flag is false, but not if the flag is true!
            if (expectedTarget is Type && expectedTarget.Equals(typeof(object)))
                return;

            IList<object> nonInheritedAttribs = expectedTarget.GetCustomAttributes(false);
            IList<object> inheritedAttribs = expectedTarget.GetCustomAttributes(true);
            ITypeInfo attributeType = Reflector.Wrap(typeof(Attribute));

            AreAttributesOfSameTypesExcludingSpecialAttributes(nonInheritedAttribs,
                actualWrapper.GetAttributes(null, false));
            AreAttributesOfSameTypesExcludingSpecialAttributes(inheritedAttribs,
                actualWrapper.GetAttributes(null, true));
            AreAttributesOfSameTypesExcludingSpecialAttributes(nonInheritedAttribs,
                actualWrapper.GetAttributes(attributeType, false));
            AreAttributesOfSameTypesExcludingSpecialAttributes(inheritedAttribs,
                actualWrapper.GetAttributes(attributeType, true));

            AreAttributesOfSameTypesExcludingSpecialAttributes(nonInheritedAttribs,
                actualWrapper.GetAttributeInfos(null, false));
            AreAttributesOfSameTypesExcludingSpecialAttributes(inheritedAttribs,
                actualWrapper.GetAttributeInfos(null, true));
            AreAttributesOfSameTypesExcludingSpecialAttributes(nonInheritedAttribs,
                actualWrapper.GetAttributeInfos(attributeType, false));
            AreAttributesOfSameTypesExcludingSpecialAttributes(inheritedAttribs,
                actualWrapper.GetAttributeInfos(attributeType, true));

            foreach (object nonInheritedAttrib in nonInheritedAttribs)
            {
                if (!IsSpecialAttribute(nonInheritedAttrib))
                    Assert.IsTrue(AttributeUtils.HasAttribute(actualWrapper, nonInheritedAttrib.GetType(), false),
                        "Should contain non-inherited attributes of same type.");
            }

            foreach (object inheritedAttrib in inheritedAttribs)
            {
                if (!IsSpecialAttribute(inheritedAttrib))
                    Assert.IsTrue(AttributeUtils.HasAttribute(actualWrapper, inheritedAttrib.GetType(), true),
                        "Should contain inherited attributes of same type.");
            }
        }

        private static void AreAttributesOfSameTypesExcludingSpecialAttributes(IEnumerable<object> expected,
            IEnumerable<object> actual)
        {
            Dictionary<Type, int> expectedCounts = new Dictionary<Type, int>();
            Dictionary<Type, int> actualCounts = new Dictionary<Type, int>();

            foreach (object expectedAttrib in expected)
                if (!IsSpecialAttribute(expectedAttrib))
                    IncrementCount(expectedCounts, expectedAttrib.GetType());
            foreach (object actualAttrib in actual)
                if (!IsSpecialAttribute(actualAttrib))
                    IncrementCount(actualCounts, actualAttrib.GetType());

            InterimAssert.WithKeyedPairs(expectedCounts, actualCounts, delegate(Type type, int expectedCount, int actualCount)
            {
                Assert.AreEqual(expectedCount, actualCount, "Number of {0} attributes should be equal.", type);
            });
        }

        private static void AreAttributesOfSameTypesExcludingSpecialAttributes(IEnumerable<object> expected,
            IEnumerable<IAttributeInfo> actual)
        {
            List<object> actualResolvedObjects = new List<object>();
            foreach (IAttributeInfo attrib in actual)
                actualResolvedObjects.Add(attrib.Resolve());

            AreAttributesOfSameTypesExcludingSpecialAttributes(expected, actualResolvedObjects);
        }

        private static void IncrementCount(IDictionary<Type, int> counts, Type type)
        {
            int count;
            counts.TryGetValue(type, out count);
            counts[type] = count + 1;
        }

        private static bool IsSpecialAttribute(object attrib)
        {
            return attrib is ComVisibleAttribute
                || attrib is ClassInterfaceAttribute
                || attrib is DefaultMemberAttribute
                || attrib is SerializableAttribute
                || attrib is DebuggableAttribute
                || attrib is CompilationRelaxationsAttribute
                || attrib is RuntimeCompatibilityAttribute
                || attrib is AssemblyCultureAttribute
                || attrib is AssemblyVersionAttribute;
        }

        private static void AreEqualUpToAssemblyDisplayName(CodeReference expected, CodeReference actual)
        {
            Assert.AreEqual(
                new CodeReference(new AssemblyName(expected.AssemblyName).Name,
                    expected.NamespaceName, expected.TypeName, expected.MemberName, expected.ParameterName),
                new CodeReference(new AssemblyName(actual.AssemblyName).Name,
                    actual.NamespaceName, actual.TypeName, actual.MemberName, actual.ParameterName));
        }
    }
}
