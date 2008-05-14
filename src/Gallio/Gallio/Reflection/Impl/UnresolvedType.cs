// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using Gallio.Collections;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// Represents a <see cref="Type" /> whose native definition could not be resolved
    /// so we fall back on the <see cref="ITypeInfo"/> wrapper.
    /// </summary>
    /// <todo author="jeff">
    /// FIXME: Several of these methods are not implemented because they have not yet
    /// been required for operation.
    /// </todo>
    public partial class UnresolvedType : Type
    {
        private readonly ITypeInfo adapter;

        /// <summary>
        /// Creates a reflection object backed by the specified adapter.
        /// </summary>
        /// <param name="adapter">The adapter</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="adapter"/> is null</exception>
        public UnresolvedType(ITypeInfo adapter)
        {
            if (adapter == null)
                throw new ArgumentNullException("adapter");

            this.adapter = adapter;
        }

        /// <summary>
        /// Gets the underlying reflection adapter.
        /// </summary>
        public ITypeInfo Adapter
        {
            get { return adapter; }
        }

        /// <inheritdoc />
        public override Assembly Assembly
        {
            get { return adapter.Assembly.Resolve(); }
        }

        /// <inheritdoc />
        public override string AssemblyQualifiedName
        {
            get { return adapter.AssemblyQualifiedName; }
        }

        /// <inheritdoc />
        public override Type BaseType
        {
            get { return UnresolvedMemberInfo.ResolveType(adapter.BaseType); }
        }

        /// <inheritdoc />
        public override bool ContainsGenericParameters
        {
            get { return adapter.ContainsGenericParameters; }
        }

        /// <inheritdoc />
        public override MethodBase DeclaringMethod
        {
            get { throw new NotImplementedException(); }
        }

        /// <inheritdoc />
        public override string FullName
        {
            get { return adapter.FullName; }
        }

        /// <inheritdoc />
        public override GenericParameterAttributes GenericParameterAttributes
        {
            get
            {
                IGenericParameterInfo parameter = adapter as IGenericParameterInfo;
                if (parameter == null)
                    throw new InvalidOperationException("Not a generic parameter.");
                return parameter.GenericParameterAttributes;
            }
        }

        /// <inheritdoc />
        public override int GenericParameterPosition
        {
            get
            {
                IGenericParameterInfo parameter = adapter as IGenericParameterInfo;
                if (parameter == null)
                    throw new InvalidOperationException("Not a generic parameter.");
                return parameter.Position;
            }
        }

        /// <inheritdoc />
        public override Guid GUID
        {
            get { throw new NotImplementedException(); }
        }

        /// <inheritdoc />
        public override bool IsGenericParameter
        {
            get { return adapter.Kind == CodeElementKind.GenericParameter; }
        }

        /// <inheritdoc />
        public override bool IsGenericType
        {
            get { return adapter.IsGenericType; }
        }

        /// <inheritdoc />
        public override bool IsGenericTypeDefinition
        {
            get { return adapter.IsGenericTypeDefinition; }
        }

        /// <inheritdoc />
        public override MemberTypes MemberType
        {
            get { return adapter.DeclaringType != null ? MemberTypes.NestedType : MemberTypes.TypeInfo; }
        }

        /// <inheritdoc />
        public override string Namespace
        {
            get
            {
                string namespaceName = adapter.NamespaceName;
                return namespaceName.Length == 0 ? null : namespaceName;
            }
        }

        /// <inheritdoc />
        public override StructLayoutAttribute StructLayoutAttribute
        {
            get { throw new NotImplementedException(); }
        }

        /// <inheritdoc />
        public override RuntimeTypeHandle TypeHandle
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override Type UnderlyingSystemType
        {
            get { return this; }
        }

        /// <inheritdoc />
        public override int GetArrayRank()
        {
            return adapter.ArrayRank;
        }

        /// <inheritdoc />
        protected override TypeAttributes GetAttributeFlagsImpl()
        {
            return adapter.TypeAttributes;
        }

        /// <inheritdoc />
        protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder,
            CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
        {
            return
                GenericUtils.ConvertAllToArray<IConstructorInfo, ConstructorInfo>(adapter.GetConstructors(bindingAttr),
                    delegate(IConstructorInfo constructor) { return constructor.Resolve(false); });
        }

        /// <inheritdoc />
        public override MemberInfo[] GetDefaultMembers()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override Type GetElementType()
        {
            ITypeInfo elementType = adapter.ElementType;
            if (elementType == null)
                throw new InvalidOperationException("Type does not have an element type.");
            return elementType.Resolve(false);
        }

        /// <inheritdoc />
        public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override EventInfo[] GetEvents(BindingFlags bindingAttr)
        {
            return GenericUtils.ConvertAllToArray<IEventInfo, EventInfo>(adapter.GetEvents(bindingAttr),
                delegate(IEventInfo @event) { return @event.Resolve(false); });
        }

        /// <inheritdoc />
        public override FieldInfo GetField(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override FieldInfo[] GetFields(BindingFlags bindingAttr)
        {
            return GenericUtils.ConvertAllToArray<IFieldInfo, FieldInfo>(adapter.GetFields(bindingAttr),
                delegate(IFieldInfo field) { return field.Resolve(false); });
        }

        /// <inheritdoc />
        public override Type[] GetGenericArguments()
        {
            return
                GenericUtils.ConvertAllToArray<ITypeInfo, Type>(adapter.GenericArguments,
                    delegate(ITypeInfo parameter) { return parameter.Resolve(false); });
        }

        /// <inheritdoc />
        public override Type[] GetGenericParameterConstraints()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override Type GetGenericTypeDefinition()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override Type GetInterface(string name, bool ignoreCase)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override InterfaceMapping GetInterfaceMap(Type interfaceType)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override Type[] GetInterfaces()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder,
            CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
        {
            return GenericUtils.ConvertAllToArray<IMethodInfo, MethodInfo>(adapter.GetMethods(bindingAttr),
                delegate(IMethodInfo method) { return method.Resolve(false); });
        }

        /// <inheritdoc />
        public override Type GetNestedType(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override Type[] GetNestedTypes(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            return GenericUtils.ConvertAllToArray<IPropertyInfo, PropertyInfo>(adapter.GetProperties(bindingAttr),
                delegate(IPropertyInfo property) { return property.Resolve(false); });
        }

        /// <inheritdoc />
        protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder,
            Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override bool HasElementTypeImpl()
        {
            return adapter.ElementType != null;
        }

        /// <inheritdoc />
        public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target,
            object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        protected override bool IsArrayImpl()
        {
            return adapter.IsArray;
        }

        /// <inheritdoc />
        public override bool IsAssignableFrom(Type c)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override bool IsByRefImpl()
        {
            return adapter.IsByRef;
        }

        /// <inheritdoc />
        protected override bool IsCOMObjectImpl()
        {
            return false;
        }

        /// <inheritdoc />
        public override bool IsInstanceOfType(object o)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override bool IsPointerImpl()
        {
            return adapter.IsPointer;
        }

        /// <inheritdoc />
        protected override bool IsPrimitiveImpl()
        {
            return adapter.TypeCode != TypeCode.Object;
        }

        /// <inheritdoc />
        public override bool IsSubclassOf(Type c)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override Type MakeArrayType()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override Type MakeArrayType(int rank)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override Type MakeByRefType()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override Type MakeGenericType(params Type[] typeArguments)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override Type MakePointerType()
        {
            throw new NotImplementedException();
        }
    }
}