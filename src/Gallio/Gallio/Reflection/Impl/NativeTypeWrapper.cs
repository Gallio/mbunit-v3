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
using Gallio.Reflection.Impl;

namespace Gallio.Reflection.Impl
{
    internal class NativeTypeWrapper : NativeMemberWrapper<Type>, ITypeInfo
    {
        public NativeTypeWrapper(Type target)
            : base(target)
        {
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

        public bool IsGenericTypeDefinition
        {
            get { return Target.IsGenericTypeDefinition; }
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

        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Type; }
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

        public IList<IPropertyInfo> GetProperties(BindingFlags bindingFlags)
        {
            PropertyInfo[] properties = Target.GetProperties(bindingFlags);
            return Array.ConvertAll<PropertyInfo, IPropertyInfo>(properties, Reflector.Wrap);
        }

        public IList<IFieldInfo> GetFields(BindingFlags bindingFlags)
        {
            FieldInfo[] fields = Target.GetFields(bindingFlags);
            return Array.ConvertAll<FieldInfo, IFieldInfo>(fields, Reflector.Wrap);
        }

        public IList<IEventInfo> GetEvents(BindingFlags bindingFlags)
        {
            EventInfo[] events = Target.GetEvents(bindingFlags);
            return Array.ConvertAll<EventInfo, IEventInfo>(events, Reflector.Wrap);
        }

        public IList<IGenericParameterInfo> GenericParameters
        {
            get
            {
                if (!Target.ContainsGenericParameters)
                    return EmptyArray<IGenericParameterInfo>.Instance;

                Type[] parameters = Target.GetGenericArguments();
                return Array.ConvertAll<Type, IGenericParameterInfo>(parameters, Reflector.WrapAsGenericParameter);
            }
        }

        public bool IsAssignableFrom(ITypeInfo type)
        {
            return Target.IsAssignableFrom(type.Resolve(true));
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

        public bool Equals(ITypeInfo other)
        {
            return Equals((object)other);
        }
    }
}