// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Reflection.Impl;

namespace Gallio.Reflection.Impl
{
    internal sealed class NativeMethodWrapper : NativeFunctionWrapper<MethodInfo>, IMethodInfo
    {
        public NativeMethodWrapper(MethodInfo target)
            : base(target)
        {
        }

        public bool IsGenericMethod
        {
            get { return Target.IsGenericMethod; }
        }

        public bool IsGenericMethodDefinition
        {
            get { return Target.IsGenericMethodDefinition; }
        }

        public bool ContainsGenericParameters
        {
            get { return Target.ContainsGenericParameters; }
        }

        public IList<ITypeInfo> GenericArguments
        {
            get { return Array.ConvertAll<Type, ITypeInfo>(Target.GetGenericArguments(), Reflector.Wrap); }
        }

        public IMethodInfo GenericMethodDefinition
        {
            get { return IsGenericMethod ? Reflector.Wrap(Target.GetGenericMethodDefinition()) : null; }
        }

        public ITypeInfo ReturnType
        {
            get { return Reflector.Wrap(Target.ReturnType); }
        }

        public IParameterInfo ReturnParameter
        {
            get { return Reflector.Wrap(Target.ReturnParameter); }
        }

        public IMethodInfo MakeGenericMethod(IList<ITypeInfo> genericArguments)
        {
            Type[] resolvedGenericArguments = GenericUtils.ConvertAllToArray<ITypeInfo, Type>(genericArguments,
                delegate(ITypeInfo genericArgument) { return genericArgument.Resolve(true); });
            return Reflector.Wrap(Target.MakeGenericMethod(resolvedGenericArguments));
        }

        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Method; }
        }

        new public MethodInfo Resolve(bool throwOnError)
        {
            return Target;
        }

        public override bool Equals(object obj)
        {
            // Note: We must also compare generic arguments to determine exact method equality.
            return base.Equals(obj)
                && GenericUtils.ElementsEqual(GenericArguments, ((NativeMethodWrapper)obj).GenericArguments);
        }

        public override int GetHashCode()
        {
            return Target.MetadataToken;
        }
    }
}