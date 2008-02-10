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
using JetBrains.ReSharper.Psi.Impl.Special;

namespace Gallio.ReSharperRunner.Reflection.Impl
{
    internal sealed class PsiMethodWrapper : PsiFunctionWrapper<IFunction>, IMethodInfo
    {
        public PsiMethodWrapper(PsiReflector reflector, IFunction target)
            : base(reflector, target)
        {
        }

        public bool IsGenericMethod
        {
            get { return Target.GetSignature(Target.IdSubstitution).GetTypeParameters().Length != 0; }
        }

        public bool IsGenericMethodDefinition
        {
            get { return IsGenericMethod; }
        }

        public bool ContainsGenericParameters
        {
            get { return IsGenericMethod; }
        }

        public IList<ITypeInfo> GenericArguments
        {
            get
            {
                ITypeParameter[] parameter = Target.GetSignature(Target.IdSubstitution).GetTypeParameters();
                return Array.ConvertAll<ITypeParameter, IGenericParameterInfo>(parameter, Reflector.Wrap);
            }
        }

        public IMethodInfo GenericMethodDefinition
        {
            get { return IsGenericMethod ? this : null; }
        }

        public ITypeInfo ReturnType
        {
            get { return Reflector.Wrap(Target.ReturnType); }
        }

        public IParameterInfo ReturnParameter
        {
            get
            {
                // TODO: This won't provide access to any parameter attributes.
                //       How should we retrieve them?
                IType type = Target.ReturnType;
                return type != null ? Reflector.Wrap(new Parameter(Target, type, null)) : null;
            }
        }

        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Method; }
        }

        public override MethodBase ResolveMethodBase(bool throwOnError)
        {
            return Resolve(throwOnError);
        }

        public MethodInfo Resolve(bool throwOnError)
        {
            return ReflectorResolveUtils.ResolveMethod(this, throwOnError);
        }

        public override string ToString()
        {
            return ReflectorNameUtils.GetMethodSignature(this);
        }
    }
}