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
using System.Reflection;
using Gallio.Reflection;
using Gallio.ReSharperRunner.Reflection.Impl;
using JetBrains.ReSharper.Psi;

namespace Gallio.ReSharperRunner.Reflection.Impl
{
    internal sealed class PsiGenericParameterWrapper : PsiDeclaredTypeWrapper, IGenericParameterInfo
    {
        public PsiGenericParameterWrapper(PsiReflector reflector, IDeclaredType target)
            : base(reflector, target)
        {
        }

        public override CodeElementKind Kind
        {
            get { return CodeElementKind.GenericParameter; }
        }

        public override bool IsGenericParameter
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

        private ITypeParameter TypeParameter
        {
            get { return (ITypeParameter) TypeElement; }
        }
    }
}