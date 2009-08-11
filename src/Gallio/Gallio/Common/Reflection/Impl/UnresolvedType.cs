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
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using Gallio.Common.Collections;

namespace Gallio.Common.Reflection.Impl
{
    /// <summary>
    /// Represents a <see cref="Type" /> whose native definition could not be resolved
    /// so we fall back on the <see cref="ITypeInfo"/> wrapper.
    /// </summary>
    /// <todo author="jeff">
    /// FIXME: Several of these methods are not implemented because they have not yet
    /// been required for operation.
    /// </todo>
    public sealed partial class UnresolvedType : Type, IUnresolvedCodeElement
    {
        private readonly ITypeInfo adapter;

        /// <summary>
        /// Creates a reflection object backed by the specified adapter.
        /// </summary>
        /// <param name="adapter">The adapter.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="adapter"/> is null.</exception>
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

        ICodeElementInfo IUnresolvedCodeElement.Adapter
        {
            get { return adapter; }
        }

        /// <inheritdoc />
        public override Assembly Assembly
        {
            get { return adapter.Assembly.Resolve(false); }
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
            get
            {
                IGenericParameterInfo parameter = adapter as IGenericParameterInfo;
                if (parameter == null)
                    throw new InvalidOperationException("Not a generic parameter.");
                return parameter.DeclaringMethod.Resolve(false);
            }
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
            get { throw new NotSupportedException("Cannot get type handle of unresolved type."); }
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

        /// <excludedoc />
        protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder,
            CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            foreach (var constructor in adapter.GetConstructors(bindingAttr))
            {
                if (ParameterListMatchesTypes(constructor.Parameters, types))
                    return constructor.Resolve(false);
            }

            return null;
        }

        /// <inheritdoc />
        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
        {
            return
                GenericCollectionUtils.ConvertAllToArray<IConstructorInfo, ConstructorInfo>(adapter.GetConstructors(bindingAttr),
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
            IEventInfo @event = adapter.GetEvent(name, bindingAttr);
            return @event != null ? @event.Resolve(false) : null;
        }

        /// <inheritdoc />
        public override EventInfo[] GetEvents(BindingFlags bindingAttr)
        {
            return GenericCollectionUtils.ConvertAllToArray<IEventInfo, EventInfo>(adapter.GetEvents(bindingAttr),
                delegate(IEventInfo @event) { return @event.Resolve(false); });
        }

        /// <inheritdoc />
        public override FieldInfo GetField(string name, BindingFlags bindingAttr)
        {
            IFieldInfo field = adapter.GetField(name, bindingAttr);
            return field != null ? field.Resolve(false) : null;
        }

        /// <inheritdoc />
        public override FieldInfo[] GetFields(BindingFlags bindingAttr)
        {
            return GenericCollectionUtils.ConvertAllToArray<IFieldInfo, FieldInfo>(adapter.GetFields(bindingAttr),
                delegate(IFieldInfo field) { return field.Resolve(false); });
        }

        /// <inheritdoc />
        public override Type[] GetGenericArguments()
        {
            return GenericCollectionUtils.ConvertAllToArray<ITypeInfo, Type>(adapter.GenericArguments,
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
            ITypeInfo genericTypeDefinition = adapter.GenericTypeDefinition;
            if (genericTypeDefinition == null)
                throw new InvalidOperationException("The type is not generic.");

            return genericTypeDefinition.Resolve(false);
        }

        /// <inheritdoc />
        public override Type GetInterface(string name, bool ignoreCase)
        {
            foreach (ITypeInfo @interface in adapter.Interfaces)
            {
                if (string.Compare(@interface.Name, name, ignoreCase) == 0
                    || string.Compare(@interface.FullName, name, ignoreCase) == 0)
                    return @interface.Resolve(false);
            }

            return null;
        }

        /// <inheritdoc />
        public override InterfaceMapping GetInterfaceMap(Type interfaceType)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override Type[] GetInterfaces()
        {
            return GenericCollectionUtils.ConvertAllToArray<ITypeInfo, Type>(adapter.Interfaces,
                    delegate(ITypeInfo @interface) { return @interface.Resolve(false); });
        }

        /// <inheritdoc />
        public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            return GetMembers(name, type, bindingAttr);
        }

        /// <inheritdoc />
        public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
        {
            return GetMembers(null, MemberTypes.All, bindingAttr);
        }

        private MemberInfo[] GetMembers(string name, MemberTypes type, BindingFlags bindingAttr)
        {
            var members = new List<MemberInfo>();

            if ((type & MemberTypes.Constructor) != 0)
                AddMatchingMembers(members, name, adapter.GetConstructors(bindingAttr));
            if ((type & MemberTypes.Event) != 0)
                AddMatchingMembers(members, name, adapter.GetEvents(bindingAttr));
            if ((type & MemberTypes.Field) != 0)
                AddMatchingMembers(members, name, adapter.GetFields(bindingAttr));
            if ((type & MemberTypes.Method) != 0)
                AddMatchingMembers(members, name, adapter.GetMethods(bindingAttr));
            if ((type & MemberTypes.Property) != 0)
                AddMatchingMembers(members, name, adapter.GetProperties(bindingAttr));
            if ((type & MemberTypes.NestedType) != 0)
                AddMatchingMembers(members, name, adapter.GetNestedTypes(bindingAttr));

            return members.ToArray();
        }

        private static void AddMatchingMembers<T>(List<MemberInfo> members, string name, IList<T> candidates)
            where T : IMemberInfo
        {
            foreach (T candidate in candidates)
            {
                if (name == null || candidate.Name == name)
                    members.Add(candidate.Resolve(false));
            }
        }

        /// <excludedoc />
        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder,
            CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            foreach (var method in adapter.GetMethods(bindingAttr))
            {
                if (method.Name == name && ParameterListMatchesTypes(method.Parameters, types))
                    return method.Resolve(false);
            }

            return null;
        }

        /// <inheritdoc />
        public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
        {
            return GenericCollectionUtils.ConvertAllToArray<IMethodInfo, MethodInfo>(adapter.GetMethods(bindingAttr),
                delegate(IMethodInfo method) { return method.Resolve(false); });
        }

        /// <inheritdoc />
        public override Type GetNestedType(string name, BindingFlags bindingAttr)
        {
            ITypeInfo nestedType = adapter.GetNestedType(name, bindingAttr);
            return nestedType != null ? nestedType.Resolve(false) : null;
        }

        /// <inheritdoc />
        public override Type[] GetNestedTypes(BindingFlags bindingAttr)
        {
            return GenericCollectionUtils.ConvertAllToArray<ITypeInfo, Type>(adapter.GetNestedTypes(bindingAttr),
                delegate(ITypeInfo type) { return type.Resolve(false); });
        }

        /// <inheritdoc />
        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            return GenericCollectionUtils.ConvertAllToArray<IPropertyInfo, PropertyInfo>(adapter.GetProperties(bindingAttr),
                delegate(IPropertyInfo property) { return property.Resolve(false); });
        }

        /// <excludedoc />
        protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder,
            Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            foreach (var property in adapter.GetProperties(bindingAttr))
            {
                if (property.Name == name && ParameterListMatchesTypes(property.IndexParameters, types))
                    return property.Resolve(false);
            }

            return null;
        }

        /// <excludedoc />
        protected override bool HasElementTypeImpl()
        {
            return adapter.ElementType != null;
        }

        /// <inheritdoc />
        public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target,
            object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
        {
            throw new NotSupportedException("Cannot invoke member of an unresolved type.");
        }

        /// <excludedoc />
        protected override bool IsArrayImpl()
        {
            return adapter.IsArray;
        }

        /// <inheritdoc />
        public override bool IsAssignableFrom(Type c)
        {
            return adapter.IsAssignableFrom(Reflector.Wrap(c));
        }

        /// <excludedoc />
        protected override bool IsByRefImpl()
        {
            return adapter.IsByRef;
        }

        /// <excludedoc />
        protected override bool IsCOMObjectImpl()
        {
            return false;
        }

        /// <inheritdoc />
        public override bool IsInstanceOfType(object o)
        {
            return false;
        }

        /// <excludedoc />
        protected override bool IsPointerImpl()
        {
            return adapter.IsPointer;
        }

        /// <excludedoc />
        protected override bool IsPrimitiveImpl()
        {
            return adapter.TypeCode != TypeCode.Object;
        }

        /// <inheritdoc />
        public override bool IsSubclassOf(Type c)
        {
            return adapter.IsSubclassOf(Reflector.Wrap(c));
        }

        /// <inheritdoc />
        public override Type MakeArrayType()
        {
            return adapter.MakeArrayType(1).Resolve(false);
        }

        /// <inheritdoc />
        public override Type MakeArrayType(int rank)
        {
            return adapter.MakeArrayType(rank).Resolve(false);
        }

        /// <inheritdoc />
        public override Type MakeByRefType()
        {
            return adapter.MakeByRefType().Resolve(false);
        }

        /// <inheritdoc />
        public override Type MakeGenericType(params Type[] typeArguments)
        {
            return adapter.MakeGenericType(Reflector.Wrap(typeArguments)).Resolve(false);
        }

        /// <inheritdoc />
        public override Type MakePointerType()
        {
            return adapter.MakePointerType().Resolve(false);
        }

        private static bool ParameterListMatchesTypes(IList<IParameterInfo> parameters, Type[] types)
        {
            if (types == null)
                return true;

            if (parameters.Count != types.Length)
                return false;

            for (int i = 0; i < types.Length; i++)
                if (parameters[i].ValueType.Name != types[i].Name)
                    return false;

            return true;
        }
    }
}