// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using Gallio.Utilities;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> type wrapper.
    /// </summary>
    public abstract class StaticTypeWrapper : StaticMemberWrapper, IResolvableTypeInfo
    {
        private readonly KeyedMemoizer<bool, Type> resolveMemoizer = new KeyedMemoizer<bool, Type>();

        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy</param>
        /// <param name="handle">The underlying reflection object</param>
        /// <param name="declaringType">The declaring type, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/> or <paramref name="handle"/> is null</exception>
        protected StaticTypeWrapper(StaticReflectionPolicy policy, object handle, StaticDeclaredTypeWrapper declaringType)
            : base(policy, handle, declaringType)
        {
        }

        /// <inheritdoc />
        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Type; }
        }

        /// <inheritdoc />
        public override CodeReference CodeReference
        {
            get
            {
                CodeReference reference = Assembly.CodeReference;

                ITypeInfo simpleType = GenericTypeDefinition ?? this;
                return new CodeReference(reference.AssemblyName,
                    simpleType.NamespaceName ?? "",
                    simpleType.FullName ?? simpleType.Name,
                    null, null);
            }
        }

        /// <inheritdoc />
        public abstract IAssemblyInfo Assembly { get; }

        /// <inheritdoc />
        public INamespaceInfo Namespace
        {
            get { return Reflector.WrapNamespace(NamespaceName); }
        }

        /// <inheritdoc />
        public abstract string NamespaceName { get; }

        /// <inheritdoc />
        public ITypeInfo BaseType
        {
            get { return BaseTypeInternal; }
        }

        /// <summary>
        /// Internal implementation of <see cref="BaseType"/>.
        /// </summary>
        protected abstract ITypeInfo BaseTypeInternal { get; }

        /// <inheritdoc />
        public virtual string AssemblyQualifiedName
        {
            get
            {
                string fullName = FullName;
                if (fullName == null)
                    return null;

                return fullName + @", " + Assembly.FullName;
            }
        }

        /// <inheritdoc />
        public abstract string FullName { get; }

        /// <inheritdoc />
        public abstract TypeAttributes TypeAttributes { get; }

        /// <inheritdoc />
        public bool IsAbstract
        {
            get { return (TypeAttributes & TypeAttributes.Abstract) != 0; }
        }

        /// <inheritdoc />
        public bool IsSealed
        {
            get { return (TypeAttributes & TypeAttributes.Sealed) != 0; }
        }

        /// <inheritdoc />
        public bool IsClass
        {
            get { return ! IsInterface && ! IsValueType; }
        }

        /// <inheritdoc />
        public bool IsInterface
        {
            get { return (TypeAttributes & TypeAttributes.Interface) != 0; }
        }

        /// <inheritdoc />
        public bool IsEnum
        {
            get { return ! IsAbstract && ! IsInterface && IsSubclassOf(Reflector.Wrap(typeof(Enum))); }
        }

        /// <inheritdoc />
        public bool IsValueType
        {
            get
            {
                return ! IsAbstract && ! IsInterface && IsSubclassOf(Reflector.Wrap(typeof(ValueType)));
            }
        }

        /// <inheritdoc />
        public bool IsNested
        {
            get { return DeclaringType != null; }
        }

        /// <inheritdoc />
        public bool IsNestedAssembly
        {
            get { return (TypeAttributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedAssembly; }
        }

        /// <inheritdoc />
        public bool IsNestedFamilyAndAssembly
        {
            get { return (TypeAttributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedFamANDAssem; }
        }

        /// <inheritdoc />
        public bool IsNestedFamily
        {
            get { return (TypeAttributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedFamily; }
        }

        /// <inheritdoc />
        public bool IsNestedFamilyOrAssembly
        {
            get { return (TypeAttributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedFamORAssem; }
        }

        /// <inheritdoc />
        public bool IsNestedPrivate
        {
            get { return (TypeAttributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedPrivate; }
        }

        /// <inheritdoc />
        public bool IsNestedPublic
        {
            get { return (TypeAttributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedPublic; }
        }

        /// <inheritdoc />
        public bool IsPublic
        {
            get { return (TypeAttributes & TypeAttributes.VisibilityMask) == TypeAttributes.Public; }
        }

        /// <inheritdoc />
        public bool IsNotPublic
        {
            get { return (TypeAttributes & TypeAttributes.VisibilityMask) == TypeAttributes.NotPublic; }
        }

        /// <summary>
        /// Gets the element type, or null if none.
        /// </summary>
        public virtual StaticTypeWrapper ElementType
        {
            get { return null; }
        }

        ITypeInfo ITypeInfo.ElementType
        {
            get { return ElementType; }
        }

        /// <inheritdoc />
        public virtual int ArrayRank
        {
            get { throw new InvalidOperationException("The type is not an array type."); }
        }

        /// <inheritdoc />
        public virtual bool IsArray
        {
            get { return false; }
        }

        /// <inheritdoc />
        public virtual bool IsPointer
        {
            get { return false; }
        }

        /// <inheritdoc />
        public virtual bool IsByRef
        {
            get { return false; }
        }

        /// <inheritdoc />
        public virtual bool IsGenericParameter
        {
            get { return false; }
        }

        /// <inheritdoc />
        public virtual bool IsGenericType
        {
            get { return false; }
        }

        /// <inheritdoc />
        public virtual bool IsGenericTypeDefinition
        {
            get { return false; }
        }

        /// <inheritdoc />
        public virtual bool ContainsGenericParameters
        {
            get { return false; }
        }

        /// <inheritdoc />
        public virtual IList<ITypeInfo> GenericArguments
        {
            get { return EmptyArray<ITypeInfo>.Instance; }
        }

        /// <inheritdoc />
        public virtual StaticDeclaredTypeWrapper GenericTypeDefinition
        {
            get { return null; }
        }
        ITypeInfo ITypeInfo.GenericTypeDefinition
        {
            get { return GenericTypeDefinition; }
        }

        /// <inheritdoc />
        public virtual TypeCode TypeCode
        {
            get { return ReflectorTypeUtils.GetTypeCode(this); }
        }

        /// <inheritdoc />
        public abstract IList<ITypeInfo> Interfaces { get; }

        /// <inheritdoc />
        public abstract IList<IConstructorInfo> GetConstructors(BindingFlags bindingFlags);

        /// <inheritdoc />
        public abstract IMethodInfo GetMethod(string methodName, BindingFlags bindingFlags);

        /// <inheritdoc />
        public abstract IList<IMethodInfo> GetMethods(BindingFlags bindingFlags);

        /// <inheritdoc />
        public abstract IPropertyInfo GetProperty(string propertyName, BindingFlags bindingFlags);

        /// <inheritdoc />
        public abstract IList<IPropertyInfo> GetProperties(BindingFlags bindingFlags);

        /// <inheritdoc />
        public abstract IFieldInfo GetField(string fieldName, BindingFlags bindingFlags);

        /// <inheritdoc />
        public abstract IList<IFieldInfo> GetFields(BindingFlags bindingFlags);

        /// <inheritdoc />
        public abstract IEventInfo GetEvent(string eventName, BindingFlags bindingFlags);

        /// <inheritdoc />
        public abstract IList<IEventInfo> GetEvents(BindingFlags bindingFlags);

        /// <inheritdoc />
        public abstract ITypeInfo GetNestedType(string nestedTypeName, BindingFlags bindingFlags);

        /// <inheritdoc />
        public abstract IList<ITypeInfo> GetNestedTypes(BindingFlags bindingFlags);

        /// <inheritdoc />
        public bool IsAssignableFrom(ITypeInfo type)
        {
            if (type == null)
                return false;

            throw new NotImplementedException("IsAssignableFrom not implemented for static types yet.");
        }

        /// <inheritdoc />
        public bool IsSubclassOf(ITypeInfo type)
        {
            if (type == null)
                return false;

            for (ITypeInfo baseType = BaseType; baseType != null; baseType = baseType.BaseType)
            {
                if (baseType.Equals(type) || baseType.AssemblyQualifiedName == type.AssemblyQualifiedName)
                    return true;
            }

            return false;
        }

        /// <inheritdoc />
        public StaticArrayTypeWrapper MakeArrayType(int arrayRank)
        {
            return new StaticArrayTypeWrapper(Policy, this, arrayRank);
        }
        ITypeInfo ITypeInfo.MakeArrayType(int arrayRank)
        {
            return MakeArrayType(arrayRank);
        }

        /// <inheritdoc />
        public StaticPointerTypeWrapper MakePointerType()
        {
            return new StaticPointerTypeWrapper(Policy, this);
        }
        ITypeInfo ITypeInfo.MakePointerType()
        {
            return MakePointerType();
        }

        /// <inheritdoc />
        public StaticByRefTypeWrapper MakeByRefType()
        {
            return new StaticByRefTypeWrapper(Policy, this);
        }
        ITypeInfo ITypeInfo.MakeByRefType()
        {
            return MakeByRefType();
        }

        /// <inheritdoc />
        public virtual StaticDeclaredTypeWrapper MakeGenericType(IList<ITypeInfo> genericArguments)
        {
            throw new InvalidOperationException("The type is not a generic type definition.");
        }
        ITypeInfo ITypeInfo.MakeGenericType(IList<ITypeInfo> genericArguments)
        {
            return MakeGenericType(genericArguments);
        }

        /// <summary>
        /// Applies a type substitution and returns the resulting type.
        /// </summary>
        /// <param name="substitution">The substitution</param>
        /// <returns>The type after substitution has been performed</returns>
        protected internal virtual ITypeInfo ApplySubstitution(StaticTypeSubstitution substitution)
        {
            return this;
        }

        /// <inheritdoc />
        public bool Equals(ITypeInfo other)
        {
            return Equals((object)other);
        }

        /// <inheritdoc />
        public Type Resolve(bool throwOnError)
        {
            return resolveMemoizer.Memoize(throwOnError, () => Resolve(null, throwOnError));
        }

        /// <inheritdoc />
        public Type Resolve(MethodInfo methodContext, bool throwOnError)
        {
            return ReflectorResolveUtils.ResolveType(this, methodContext, throwOnError);
        }

        /// <inheritdoc />
        protected override MemberInfo ResolveMemberInfo(bool throwOnError)
        {
            return Resolve(null, throwOnError);
        }

        /// <inheritdoc />
        protected override IEnumerable<ICodeElementInfo> GetInheritedElements()
        {
            for (ITypeInfo baseType = BaseType; baseType != null; baseType = baseType.BaseType)
                yield return baseType;
        }
    }
}
