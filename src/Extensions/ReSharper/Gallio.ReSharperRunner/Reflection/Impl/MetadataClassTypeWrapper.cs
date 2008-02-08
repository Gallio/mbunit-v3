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
using Gallio.Reflection;
using Gallio.Reflection.Impl;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;

namespace Gallio.ReSharperRunner.Reflection.Impl
{
    internal sealed class MetadataClassTypeWrapper : MetadataTypeWrapper<IMetadataClassType>
    {
        public MetadataClassTypeWrapper(MetadataReflector reflector, IMetadataClassType target)
            : base(reflector, target)
        {
        }

        protected override IDeclaredElement GetDeclaredElementWithLock()
        {
            return Reflector.GetDeclaredElementWithLock(Target.Type);
        }

        public override string Name
        {
            get { return ReflectorTypeUtils.GetTypeName(this, ShortName); }
        }

        public override string FullName
        {
            get { return ReflectorTypeUtils.GetTypeFullName(this, ShortName); }
        }

        public override string CompoundName
        {
            get
            {
                ITypeInfo declaringType = DeclaringType;
                return declaringType != null ? declaringType.CompoundName + @"." + Name : Name;
            }
        }

        public override ITypeInfo DeclaringType
        {
            get { return Reflector.WrapOpenType(TypeInfo.DeclaringType); }
        }

        public override IAssemblyInfo Assembly
        {
            get
            {
                IMetadataAssembly assembly = MetadataReflector.GetMetadataAssemblyHack(TypeInfo);
                if (assembly != null)
                    return Reflector.Wrap(assembly);

                AssemblyName assemblyName = TypeInfo.DeclaringAssemblyName;
                if (assemblyName != null)
                {
                    assembly = Reflector.LoadMetadataAssembly(assemblyName, false);
                    if (assembly != null)
                        return Reflector.Wrap(assembly);
                }

                // Note: ReSharper can sometimes return null for built-in such as System.String.
                //       I don't know whether it will do this for other types though.
                //       So for now we assume System.
                string typeName = TypeInfo.FullyQualifiedName;
                Assembly systemAssembly = typeof(String).Assembly;
                if (systemAssembly.GetType(typeName, false) != null)
                    return Gallio.Reflection.Reflector.Wrap(systemAssembly);

                throw new NotImplementedException(String.Format(
                    "Cannot determine the assembly to which type '{0}' belongs.", typeName));
            }
        }

        public override INamespaceInfo Namespace
        {
            get { return Reflector.WrapNamespace(NamespaceName); }
        }

        public override ITypeInfo BaseType
        {
            get { return Reflector.Wrap(TypeInfo.Base); }
        }

        public override TypeAttributes TypeAttributes
        {
            get
            {
                IMetadataTypeInfo info = TypeInfo;
                TypeAttributes flags = 0;
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.Abstract, info.IsAbstract);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.Class, info.IsClass);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.Interface, info.IsInterface);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.NestedAssembly, info.IsNestedAssembly);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.NestedFamily, info.IsNestedFamily);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.NestedFamANDAssem, info.IsNestedFamilyAndAssembly);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.NestedFamORAssem, info.IsNestedFamilyOrAssembly);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.NestedPrivate, info.IsNestedPrivate);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.NestedPublic, info.IsNestedPublic);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.Public, info.IsPublic);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.NotPublic, info.IsNotPublic);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.Sealed, info.IsSealed);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.Serializable, info.IsSerializable);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.SpecialName, info.IsSpecialName);
                return flags;
            }
        }

        public override IList<ITypeInfo> Interfaces
        {
            get
            {
                IMetadataClassType[] interfaces = TypeInfo.Interfaces;
                return Array.ConvertAll<IMetadataClassType, ITypeInfo>(interfaces, Reflector.Wrap);
            }
        }

        public override IList<IConstructorInfo> GetConstructors(BindingFlags bindingFlags)
        {
            return new List<IConstructorInfo>(EnumerateConstructors(bindingFlags));
        }

        private IEnumerable<IConstructorInfo> EnumerateConstructors(BindingFlags bindingFlags)
        {
            return ReflectorMemberUtils.EnumerateConstructors(this, bindingFlags, GetConstructors);
        }

        private IEnumerable<IConstructorInfo> GetConstructors(ITypeInfo type)
        {
            IMetadataTypeInfo typeInfo = ((MetadataClassTypeWrapper)type).TypeInfo;

            foreach (IMetadataMethod method in typeInfo.GetMethods())
            {
                if (MetadataReflector.IsConstructor(method))
                    yield return Reflector.WrapConstructor(method);
            }
        }

        public override IMethodInfo GetMethod(string methodName, BindingFlags bindingFlags)
        {
            return ReflectorMemberUtils.GetMemberByName(EnumerateMethods(bindingFlags), methodName);
        }

        public override IList<IMethodInfo> GetMethods(BindingFlags bindingFlags)
        {
            return new List<IMethodInfo>(EnumerateMethods(bindingFlags));
        }

        private IEnumerable<IMethodInfo> EnumerateMethods(BindingFlags bindingFlags)
        {
            return ReflectorMemberUtils.EnumerateMethods(this, bindingFlags, GetMethods);
        }

        private IEnumerable<IMethodInfo> GetMethods(ITypeInfo type)
        {
            IMetadataTypeInfo typeInfo = ((MetadataClassTypeWrapper)type).TypeInfo;

            foreach (IMetadataMethod method in typeInfo.GetMethods())
            {
                if (!MetadataReflector.IsConstructor(method))
                    yield return Reflector.WrapMethod(method);
            }
        }

        public override IList<IPropertyInfo> GetProperties(BindingFlags bindingFlags)
        {
            return new List<IPropertyInfo>(EnumerateProperties(bindingFlags));
        }

        private IEnumerable<IPropertyInfo> EnumerateProperties(BindingFlags bindingFlags)
        {
            return ReflectorMemberUtils.EnumerateProperties(this, bindingFlags, GetProperties);
        }

        private IEnumerable<IPropertyInfo> GetProperties(ITypeInfo type)
        {
            IMetadataTypeInfo typeInfo = ((MetadataClassTypeWrapper)type).TypeInfo;

            foreach (IMetadataProperty property in typeInfo.GetProperties())
                yield return Reflector.Wrap(property);
        }

        public override IList<IFieldInfo> GetFields(BindingFlags bindingFlags)
        {
            return new List<IFieldInfo>(EnumerateFields(bindingFlags));
        }

        private IEnumerable<IFieldInfo> EnumerateFields(BindingFlags bindingFlags)
        {
            return ReflectorMemberUtils.EnumerateFields(this, bindingFlags, GetFields);
        }

        private IEnumerable<IFieldInfo> GetFields(ITypeInfo type)
        {
            foreach (IMetadataField field in TypeInfo.GetFields())
                yield return Reflector.Wrap(field);
        }

        public override IList<IEventInfo> GetEvents(BindingFlags bindingFlags)
        {
            return new List<IEventInfo>(EnumerateEvents(bindingFlags));
        }

        private IEnumerable<IEventInfo> EnumerateEvents(BindingFlags bindingFlags)
        {
            return ReflectorMemberUtils.EnumerateEvents(this, bindingFlags, GetEvents);
        }

        private IEnumerable<IEventInfo> GetEvents(ITypeInfo type)
        {
            IMetadataTypeInfo typeInfo = ((MetadataClassTypeWrapper)type).TypeInfo;

            foreach (IMetadataEvent @event in typeInfo.GetEvents())
                yield return Reflector.Wrap(@event);
        }

        public override bool IsAssignableFrom(ITypeInfo type)
        {
            // TODO
            throw new NotImplementedException();
        }

        public override bool IsGenericType
        {
            get { return Target.Type.GenericParameters.Length != 0; }
        }

        public override bool IsGenericTypeDefinition
        {
            get
            {
                if (Target.Arguments.Length == 0)
                    return false;

                return ! Array.Exists(Target.Arguments, delegate(IMetadataType argument)
                {
                    IMetadataGenericArgumentReferenceType argumentRef = argument as IMetadataGenericArgumentReferenceType;
                    return argumentRef == null || argumentRef.Argument.TypeOwner.Token != TypeInfo.Token;
                });
            }
        }

        public override bool ContainsGenericParameters
        {
            get
            {
                return GenericUtils.Find(GenericArguments,
                    delegate(ITypeInfo genericArgument) { return genericArgument.ContainsGenericParameters; }) != null;
            }
        }

        public override IList<ITypeInfo> GenericArguments
        {
            get { return Array.ConvertAll<IMetadataType, ITypeInfo>(Target.Arguments, Reflector.Wrap); }
        }

        public override ITypeInfo GenericTypeDefinition
        {
            get
            {
                if (! IsGenericType)
                    return null;

                return Reflector.WrapOpenType(TypeInfo);
            }
        }

        public override IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit)
        {
            return ReflectorAttributeUtils.EnumerateTypeAttributes(this, inherit, delegate(ITypeInfo member)
            {
                return EnumerateAttributesForEntity(((MetadataClassTypeWrapper)member).TypeInfo);
            });
        }

        private string NamespaceName
        {
            get { return new CLRTypeName(TypeInfo.FullyQualifiedName).NamespaceName; }
        }

        private string ShortName
        {
            get { return new CLRTypeName(TypeInfo.FullyQualifiedName).ShortName; }
        }

        private IMetadataTypeInfo TypeInfo
        {
            get { return Target.Type; }
        }
    }
}