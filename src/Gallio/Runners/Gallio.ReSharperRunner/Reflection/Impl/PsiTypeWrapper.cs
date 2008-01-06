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
using Gallio.Reflection;
using Gallio.Reflection.Impl;
using Gallio.ReSharperRunner.Reflection.Impl;
using JetBrains.ReSharper.Psi;

namespace Gallio.ReSharperRunner.Reflection.Impl
{
    internal abstract class PsiTypeWrapper<TTarget> : PsiCodeElementWrapper<TTarget>, ITypeInfo, IPsiTypeWrapper
        where TTarget : class, IType
    {
        public PsiTypeWrapper(PsiReflector reflector, TTarget target)
            : base(reflector, target)
        {
        }

        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Type; }
        }

        IType IPsiTypeWrapper.Target
        {
            get { return Target; }
        }

        public string AssemblyQualifiedName
        {
            get { return FullName + ", " + Assembly.FullName; }
        }

        public override CodeReference CodeReference
        {
            get { return new CodeReference(Assembly.FullName, Namespace.Name, FullName, null, null); }
        }

        public bool IsAssignableFrom(ITypeInfo type)
        {
            IPsiTypeWrapper typeWrapper = type as IPsiTypeWrapper;
            return typeWrapper != null && typeWrapper.Target.IsImplicitlyConvertibleTo(Target, PsiLanguageType.UNKNOWN);
        }

        public virtual ITypeInfo ElementType
        {
            get { return null; }
        }

        public virtual int ArrayRank
        {
            get { throw new InvalidOperationException("Not an array type."); }
        }

        public TypeCode TypeCode
        {
            get { return ReflectorTypeUtils.GetTypeCode(this); }
        }

        public virtual bool IsArray
        {
            get { return false; }
        }

        public virtual bool IsPointer
        {
            get { return false; }
        }

        public virtual bool IsByRef
        {
            get { return false; }
        }

        public virtual bool IsGenericParameter
        {
            get { return false; }
        }

        public abstract string CompoundName { get; }
        public abstract ITypeInfo DeclaringType { get; }
        public abstract IAssemblyInfo Assembly { get; }
        public abstract INamespaceInfo Namespace { get; }
        public abstract ITypeInfo BaseType { get; }
        public abstract string FullName { get; }
        public abstract TypeAttributes TypeAttributes { get; }
        public abstract IList<ITypeInfo> Interfaces { get; }
        public abstract IList<IConstructorInfo> GetConstructors(BindingFlags bindingFlags);
        public abstract IMethodInfo GetMethod(string methodName, BindingFlags bindingFlags);
        public abstract IList<IMethodInfo> GetMethods(BindingFlags bindingFlags);
        public abstract IList<IPropertyInfo> GetProperties(BindingFlags bindingFlags);
        public abstract IList<IFieldInfo> GetFields(BindingFlags bindingFlags);
        public abstract IList<IEventInfo> GetEvents(BindingFlags bindingFlags);
        public abstract IList<IGenericParameterInfo> GenericParameters { get; }

        public Type Resolve()
        {
            return ReflectorResolveUtils.ResolveType(this);
        }

        MemberInfo IMemberInfo.Resolve()
        {
            return Resolve();
        }

        public bool Equals(IMemberInfo other)
        {
            return Equals((object)other);
        }

        public bool Equals(ITypeInfo other)
        {
            return Equals((object)other);
        }
    }
}