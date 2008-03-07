// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Reflection.Impl;

namespace Gallio.Reflection.Impl
{
    internal class NativeTypeWrapper : NativeMemberWrapper<Type>, ITypeInfo
    {
        public NativeTypeWrapper(Type target)
            : base(target)
        {
        }

        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Type; }
        }

        public override CodeReference CodeReference
        {
            get { return CodeReference.CreateFromType(Target); }
        }

        public IAssemblyInfo Assembly
        {
            get { return Reflector.Wrap(Target.Assembly); }
        }

        public INamespaceInfo Namespace
        {
            get { return Reflector.WrapNamespace(Target.Namespace); }
        }

        public ITypeInfo BaseType
        {
            get { return Reflector.Wrap(Target.BaseType); }
        }

        public ITypeInfo ElementType
        {
            get { return Reflector.Wrap(Target.GetElementType()); }
        }

        public bool IsArray
        {
            get { return Target.IsArray; }
        }

        public bool IsPointer
        {
            get { return Target.IsPointer; }
        }

        public bool IsByRef
        {
            get { return Target.IsByRef; }
        }

        public bool IsGenericParameter
        {
            get { return Target.IsGenericParameter; }
        }

        public bool IsGenericType
        {
            get { return Target.IsGenericType; }
        }

        public bool IsGenericTypeDefinition
        {
            get { return Target.IsGenericTypeDefinition; }
        }

        public bool ContainsGenericParameters
        {
            get { return Target.ContainsGenericParameters; }
        }

        public ITypeInfo GenericTypeDefinition
        {
            get { return IsGenericType ? Reflector.Wrap(Target.GetGenericTypeDefinition()) : null; }
        }

        public int ArrayRank
        {
            get
            {
                if (! Target.IsArray)
                    throw new InvalidOperationException("Not an array type.");
                return Target.GetArrayRank();
            }
        }

        public TypeCode TypeCode
        {
            get { return Type.GetTypeCode(Target); }
        }

        public string AssemblyQualifiedName
        {
            get { return Target.AssemblyQualifiedName; }
        }

        public string FullName
        {
            get { return Target.FullName; }
        }

        public TypeAttributes TypeAttributes
        {
            get { return Target.Attributes; }
        }

        public bool IsAbstract
        {
            get { return Target.IsAbstract; }
        }

        public bool IsSealed
        {
            get { return Target.IsSealed; }
        }

        public bool IsClass
        {
            get { return Target.IsClass; }
        }

        public bool IsInterface
        {
            get { return Target.IsInterface; }
        }

        public bool IsEnum
        {
            get { return Target.IsEnum; }
        }

        public bool IsValueType
        {
            get { return Target.IsValueType; }
        }

        public bool IsNested
        {
            get { return Target.IsNested; }
        }

        public bool IsNestedAssembly
        {
            get { return Target.IsNestedAssembly; }
        }

        public bool IsNestedFamilyAndAssembly
        {
            get { return Target.IsNestedFamANDAssem; }
        }

        public bool IsNestedFamily
        {
            get { return Target.IsNestedFamily; }
        }

        public bool IsNestedFamilyOrAssembly
        {
            get { return Target.IsNestedFamORAssem; }
        }

        public bool IsNestedPrivate
        {
            get { return Target.IsNestedPrivate; }
        }

        public bool IsNestedPublic
        {
            get { return Target.IsNestedPublic; }
        }

        public bool IsPublic
        {
            get { return Target.IsPublic; }
        }

        public bool IsNotPublic
        {
            get { return Target.IsNotPublic; }
        }

        public IList<ITypeInfo> Interfaces
        {
            get
            {
                Type[] interfaces = Target.GetInterfaces();
                return Array.ConvertAll<Type, ITypeInfo>(interfaces, Reflector.Wrap);
            }
        }

        public IList<IConstructorInfo> GetConstructors(BindingFlags bindingFlags)
        {
            ConstructorInfo[] constructors = Target.GetConstructors(bindingFlags);
            return Array.ConvertAll<ConstructorInfo, IConstructorInfo>(constructors, Reflector.Wrap);
        }

        public IMethodInfo GetMethod(string methodName, BindingFlags bindingFlags)
        {
            return Reflector.Wrap(Target.GetMethod(methodName, bindingFlags));
        }

        public IList<IMethodInfo> GetMethods(BindingFlags bindingFlags)
        {
            MethodInfo[] methods = Target.GetMethods(bindingFlags);
            return Array.ConvertAll<MethodInfo, IMethodInfo>(methods, Reflector.Wrap);
        }

        public IPropertyInfo GetProperty(string propertyName, BindingFlags bindingFlags)
        {
            return Reflector.Wrap(Target.GetProperty(propertyName, bindingFlags));
        }

        public IList<IPropertyInfo> GetProperties(BindingFlags bindingFlags)
        {
            PropertyInfo[] properties = Target.GetProperties(bindingFlags);
            return Array.ConvertAll<PropertyInfo, IPropertyInfo>(properties, Reflector.Wrap);
        }

        public IFieldInfo GetField(string fieldName, BindingFlags bindingFlags)
        {
            return Reflector.Wrap(Target.GetField(fieldName, bindingFlags));
        }

        public IList<IFieldInfo> GetFields(BindingFlags bindingFlags)
        {
            FieldInfo[] fields = Target.GetFields(bindingFlags);
            return Array.ConvertAll<FieldInfo, IFieldInfo>(fields, Reflector.Wrap);
        }

        public IEventInfo GetEvent(string eventName, BindingFlags bindingFlags)
        {
            return Reflector.Wrap(Target.GetEvent(eventName, bindingFlags));
        }

        public IList<IEventInfo> GetEvents(BindingFlags bindingFlags)
        {
            EventInfo[] events = Target.GetEvents(bindingFlags);
            return Array.ConvertAll<EventInfo, IEventInfo>(events, Reflector.Wrap);
        }

        public ITypeInfo GetNestedType(string nestedTypeName, BindingFlags bindingFlags)
        {
            return Reflector.Wrap(Target.GetNestedType(nestedTypeName, bindingFlags));
        }

        public IList<ITypeInfo> GetNestedTypes(BindingFlags bindingFlags)
        {
            Type[] nestedTypes = Target.GetNestedTypes(bindingFlags);
            return Array.ConvertAll<Type, ITypeInfo>(nestedTypes, Reflector.Wrap);
        }

        public IList<ITypeInfo> GenericArguments
        {
            get { return Array.ConvertAll<Type, ITypeInfo>(Target.GetGenericArguments(), Reflector.Wrap); }
        }

        public bool IsAssignableFrom(ITypeInfo type)
        {
            return Target.IsAssignableFrom(type.Resolve(false));
        }

        public bool IsSubclassOf(ITypeInfo type)
        {
            return Target.IsSubclassOf(type.Resolve(false));
        }

        public ITypeInfo MakeArrayType(int arrayRank)
        {
            if (arrayRank == 1)
                return Reflector.Wrap(Target.MakeArrayType());
            return Reflector.Wrap(Target.MakeArrayType(arrayRank));
        }

        public ITypeInfo MakePointerType()
        {
            return Reflector.Wrap(Target.MakePointerType());
        }

        public ITypeInfo MakeByRefType()
        {
            return Reflector.Wrap(Target.MakeByRefType());
        }

        public ITypeInfo MakeGenericType(IList<ITypeInfo> genericArguments)
        {
            Type[] resolvedGenericArguments = GenericUtils.ConvertAllToArray<ITypeInfo, Type>(genericArguments,
                delegate(ITypeInfo genericArgument) { return genericArgument.Resolve(true); });
            return Reflector.Wrap(Target.MakeGenericType(resolvedGenericArguments));
        }

        new public Type Resolve(bool throwOnError)
        {
            return Target;
        }

        public override CodeLocation GetCodeLocation()
        {
            return DebugSymbolUtils.GetSourceLocation(Target)
                ?? Assembly.GetCodeLocation();
        }

        public override bool Equals(object obj)
        {
            NativeTypeWrapper other = obj as NativeTypeWrapper;
            return other != null && Target.Equals(other.Target);
        }

        public override int GetHashCode()
        {
            return Target.GetHashCode();
        }

        public bool Equals(ITypeInfo other)
        {
            return Equals((object)other);
        }
    }
}