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
using System.Text;
using Gallio.Common.Reflection;
using System.Reflection;

namespace Gallio.MbUnitCppAdapter.Model.Bridge
{
    /// <summary>
    /// A fake type info that represents an MbUnitCpp test fixture.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Used to fool the Gallio filtering engine so that we can use filters on an MbUnitCpp test tree.
    /// </para>
    /// </remarks>
    /// <seealso cref="Gallio.Model.Filters.TypeFilter{T}"/>
    internal class FakeTypeInfo : ITypeInfo
    {
        private readonly string name;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the MbUnitCpp fixture.</param>
        public FakeTypeInfo(string name)
        {
            this.name = name;
        }

        /// <inheritdoc />
        public IAssemblyInfo Assembly
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public INamespaceInfo Namespace
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public string NamespaceName
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public ITypeInfo BaseType
        {
            get { return null; }
        }

        /// <inheritdoc />
        public string AssemblyQualifiedName
        {
            get { return String.Empty; }
        }

        /// <inheritdoc />
        public string FullName
        {
            get { return name; }
        }

        /// <inheritdoc />
        public TypeAttributes TypeAttributes
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public bool IsAbstract
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public bool IsSealed
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public bool IsClass
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public bool IsInterface
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public bool IsEnum
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public bool IsValueType
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public bool IsNested
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public bool IsNestedAssembly
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public bool IsNestedFamilyAndAssembly
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public bool IsNestedFamily
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public bool IsNestedFamilyOrAssembly
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public bool IsNestedPrivate
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public bool IsNestedPublic
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public bool IsPublic
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public bool IsNotPublic
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public bool IsArray
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public bool IsPointer
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public bool IsByRef
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public bool IsGenericParameter
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public bool IsGenericType
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public bool IsGenericTypeDefinition
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public bool ContainsGenericParameters
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public IList<ITypeInfo> GenericArguments
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public ITypeInfo GenericTypeDefinition
        {
            get { return null; }
        }

        /// <inheritdoc />
        public int ArrayRank
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public TypeCode TypeCode
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public IList<ITypeInfo> Interfaces
        {
            get { return new List<ITypeInfo>(); }
        }

        /// <inheritdoc />
        public ITypeInfo ElementType
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public IList<IConstructorInfo> GetConstructors(System.Reflection.BindingFlags bindingFlags)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public IMethodInfo GetMethod(string methodName, System.Reflection.BindingFlags bindingFlags)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public IList<IMethodInfo> GetMethods(System.Reflection.BindingFlags bindingFlags)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public IPropertyInfo GetProperty(string propertyName, System.Reflection.BindingFlags bindingFlags)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public IList<IPropertyInfo> GetProperties(System.Reflection.BindingFlags bindingFlags)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public IFieldInfo GetField(string fieldName, System.Reflection.BindingFlags bindingFlags)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public IList<IFieldInfo> GetFields(System.Reflection.BindingFlags bindingFlags)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public IEventInfo GetEvent(string eventName, System.Reflection.BindingFlags bindingFlags)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public IList<IEventInfo> GetEvents(System.Reflection.BindingFlags bindingFlags)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public ITypeInfo GetNestedType(string nestedTypeName, System.Reflection.BindingFlags bindingFlags)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public IList<ITypeInfo> GetNestedTypes(System.Reflection.BindingFlags bindingFlags)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public bool IsAssignableFrom(ITypeInfo type)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public bool IsSubclassOf(ITypeInfo type)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public ITypeInfo MakeArrayType(int arrayRank)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public ITypeInfo MakePointerType()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public ITypeInfo MakeByRefType()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public ITypeInfo MakeGenericType(IList<ITypeInfo> genericArguments)
        {
            throw new NotSupportedException();
        }

        public Type Resolve(bool throwOnError)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public ITypeInfo DeclaringType
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public ITypeInfo ReflectedType
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        MemberInfo IMemberInfo.Resolve(bool throwOnError)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public string Name
        {
            get { return name; }
        }

        /// <inheritdoc />
        public CodeElementKind Kind
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public CodeReference CodeReference
        {
            get { return CodeReference.Unknown; }
        }

        /// <inheritdoc />
        public IEnumerable<IAttributeInfo> GetAttributeInfos(ITypeInfo attributeType, bool inherit)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public bool HasAttribute(ITypeInfo attributeType, bool inherit)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public IEnumerable<object> GetAttributes(ITypeInfo attributeType, bool inherit)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public string GetXmlDocumentation()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public CodeLocation GetCodeLocation()
        {
            return CodeLocation.Unknown;
        }

        /// <inheritdoc />
        public IReflectionPolicy ReflectionPolicy
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public bool Equals(ICodeElementInfo other)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public bool Equals(IMemberInfo other)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public bool Equals(ITypeInfo other)
        {
            throw new NotSupportedException();
        }
    }
}
