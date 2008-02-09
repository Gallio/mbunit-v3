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
using Gallio.ReSharperRunner.Reflection.Impl;
using JetBrains.ReSharper.Psi;

namespace Gallio.ReSharperRunner.Reflection.Impl
{
    internal sealed class PsiGenericParameterWrapper : PsiConstructedTypeWrapper<IDeclaredType>, IGenericParameterInfo
    {
        public PsiGenericParameterWrapper(PsiReflector reflector, IDeclaredType target)
            : base(reflector, target)
        {
        }

        public override CodeElementKind Kind
        {
            get { return CodeElementKind.GenericParameter; }
        }

        public override ITypeInfo BaseType
        {
            get { return Gallio.Reflection.Reflector.Wrap(typeof(Object)); }
        }

        public override ITypeInfo EffectiveClassType
        {
            get { return Gallio.Reflection.Reflector.Wrap(typeof(Object)); }
        }

        public override bool IsGenericParameter
        {
            get { return true; }
        }

        public override bool ContainsGenericParameters
        {
            get { return true; }
        }

        public GenericParameterAttributes GenericParameterAttributes
        {
            get
            {
                GenericParameterAttributes flags = 0;
                ITypeParameter typeParameter = TypeParameter;

                if (typeParameter.IsValueType)
                    flags |= GenericParameterAttributes.NotNullableValueTypeConstraint;
                if (typeParameter.IsClassType)
                    flags |= GenericParameterAttributes.ReferenceTypeConstraint;
                if (typeParameter.HasDefaultConstructor)
                    flags |= GenericParameterAttributes.DefaultConstructorConstraint;

                return flags;
            }
        }

        public override IAssemblyInfo Assembly
        {
            get { return GetUltimateDeclaringType().Assembly; }
        }

        public override INamespaceInfo Namespace
        {
            get { return GetUltimateDeclaringType().Namespace; }
        }

        public override ITypeInfo DeclaringType
        {
            get { return Reflector.Wrap(TypeParameter.OwnerType); }
        }

        public IMethodInfo DeclaringMethod
        {
            get { return Reflector.Wrap(TypeParameter.OwnerMethod); }
        }

        public ITypeInfo[] Constraints
        {
            get { return Array.ConvertAll<IType, ITypeInfo>(TypeParameter.TypeConstraints, Reflector.Wrap); }
        }

        public ITypeInfo ValueType
        {
            get { return Gallio.Reflection.Reflector.Wrap(typeof(Type)); }
        }

        public int Position
        {
            get { return TypeParameter.Index; }
        }

        public bool Equals(ISlotInfo other)
        {
            return Equals((object) other);
        }

        public override IList<IConstructorInfo> GetConstructors(BindingFlags bindingFlags)
        {
            return EmptyArray<IConstructorInfo>.Instance;
        }

        public override IEnumerable<IAttributeInfo> GetAttributeInfos(ITypeInfo attributeType, bool inherit)
        {
            return EnumerateAttributesForElement(TypeParameter, attributeType, inherit);
        }

        protected override string SimpleName
        {
            get { return TypeParameter.ShortName; }
        }

        private ITypeParameter TypeParameter
        {
            get { return (ITypeParameter) Target.GetTypeElement(); }
        }

        private ITypeInfo GetUltimateDeclaringType()
        {
            return DeclaringType ?? DeclaringMethod.DeclaringType;
        }
    }
}