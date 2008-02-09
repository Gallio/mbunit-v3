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
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Resolve;

namespace Gallio.ReSharperRunner.Reflection.Impl
{
    internal class PsiDeclaredTypeWrapper : PsiTypeWrapper<IDeclaredType>
    {
        public PsiDeclaredTypeWrapper(PsiReflector reflector, IDeclaredType target)
            : base(reflector, target)
        {
        }

        public override IDeclaredElement DeclaredElement
        {
            get { return TypeElement; }
        }

        public override IAssemblyInfo Assembly
        {
            get
            {
                IModule module = TypeElement.Module;
                return Reflector.Wrap(module);
            }
        }

        public override ITypeInfo DeclaringType
        {
            get { return Reflector.Wrap(TypeElement.GetContainingType()); }
        }

        public override INamespaceInfo Namespace
        {
            get { return Reflector.WrapNamespace(NamespaceName); }
        }

        public override ITypeInfo BaseType
        {
            get
            {
                foreach (IDeclaredType superType in Target.GetSuperTypes())
                {
                    IClass @class = superType.GetTypeElement() as IClass;
                    if (@class != null)
                        return Reflector.Wrap(@class);
                }

                return null;
            }
        }

        public override bool IsGenericParameter
        {
            get { return false; }
        }

        public override bool IsGenericType
        {
            get { return TypeElement.TypeParameters.Length != 0; }
        }

        public override bool IsGenericTypeDefinition
        {
            get
            {
                ITypeParameter[] typeParameters = TypeElement.TypeParameters;
                if (typeParameters.Length == 0)
                    return false;

                ISubstitution substitution = Target.GetSubstitution();
                foreach (ITypeParameter typeParameter in typeParameters)
                    if (substitution.HasInDomain(typeParameter))
                        return false;

                return true;
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
            get
            {
                ISubstitution substitution = Target.GetSubstitution();
                ITypeParameter[] parameters = TypeElement.TypeParameters;
                return Array.ConvertAll<ITypeParameter, ITypeInfo>(parameters,
                    delegate(ITypeParameter parameter) { return Reflector.Wrap(substitution.Apply(parameter)); });
            }
        }

        public override ITypeInfo GenericTypeDefinition
        {
            get
            {
                if (!IsGenericType)
                    return null;

                return Reflector.Wrap(TypeElement);
            }
        }

        public override TypeAttributes TypeAttributes
        {
            get
            {
                TypeAttributes flags = 0;
                ITypeElement typeElement = TypeElement;

                if (typeElement is IClass)
                    flags |= TypeAttributes.Class;
                else if (typeElement is IInterface)
                    flags |= TypeAttributes.Interface;

                IModifiersOwner modifiers = typeElement as IModifiersOwner;
                if (modifiers != null)
                {
                    if (modifiers.IsAbstract)
                        flags |= TypeAttributes.Abstract;
                    if (modifiers.IsSealed)
                        flags |= TypeAttributes.Sealed;

                    bool isNested = typeElement.GetContainingType() != null;

                    switch (modifiers.GetAccessRights())
                    {
                        case AccessRights.PUBLIC:
                            flags |= isNested ? TypeAttributes.NestedPublic : TypeAttributes.Public;
                            break;
                        case AccessRights.PRIVATE:
                            flags |= isNested ? TypeAttributes.NestedPrivate : TypeAttributes.NotPublic;
                            break;
                        case AccessRights.NONE:
                        case AccessRights.INTERNAL:
                            flags |= TypeAttributes.NestedAssembly;
                            break;
                        case AccessRights.PROTECTED:
                            flags |= TypeAttributes.NestedFamily;
                            break;
                        case AccessRights.PROTECTED_AND_INTERNAL:
                            flags |= TypeAttributes.NestedFamANDAssem;
                            break;
                        case AccessRights.PROTECTED_OR_INTERNAL:
                            flags |= TypeAttributes.NestedFamORAssem;
                            break;
                    }
                }

                return flags;
            }
        }

        public override IList<ITypeInfo> Interfaces
        {
            get
            {
                List<ITypeInfo> interfaces = new List<ITypeInfo>();

                foreach (IDeclaredType superType in Target.GetSuperTypes())
                {
                    IInterface @interface = superType.GetTypeElement() as IInterface;
                    if (@interface != null)
                        interfaces.Add(Reflector.Wrap(@interface));
                }

                return interfaces;
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
            ITypeElement typeElement = ((PsiDeclaredTypeWrapper)type).TypeElement;

            foreach (IConstructor constructor in typeElement.Constructors)
                yield return Reflector.Wrap(constructor);
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
            ITypeElement typeElement = ((PsiDeclaredTypeWrapper)type).TypeElement;

            foreach (IMethod method in typeElement.Methods)
                yield return Reflector.Wrap(method);
            foreach (IOperator method in typeElement.Operators)
                yield return Reflector.Wrap(method);
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
            ITypeElement typeElement = ((PsiDeclaredTypeWrapper)type).TypeElement;

            foreach (IProperty property in typeElement.Properties)
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
            ITypeElement typeElement = ((PsiDeclaredTypeWrapper)type).TypeElement;

            IClass @class = typeElement as IClass;
            if (@class != null)
            {
                foreach (IField field in @class.Fields)
                    yield return Reflector.Wrap(field);
                foreach (IField field in @class.Constants)
                    yield return Reflector.Wrap(field);
            }
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
            ITypeElement typeElement = ((PsiDeclaredTypeWrapper)type).TypeElement;

            foreach (IEvent @event in typeElement.Events)
                yield return Reflector.Wrap(@event);
        }

        public override IEnumerable<IAttributeInfo> GetAttributeInfos(ITypeInfo attributeType, bool inherit)
        {
            return EnumerateAttributesForElement(TypeElement, attributeType, inherit);
        }

        protected override string SimpleName
        {
            get { return TypeElement.ShortName; }
        }

        private string NamespaceName
        {
            get { return CLRTypeName.NamespaceName; }
        }

        private CLRTypeName CLRTypeName
        {
            get { return new CLRTypeName(Target.GetCLRName()); }
        }

        protected ITypeElement TypeElement
        {
            get { return Target.GetTypeElement(); }
        }
    }
}