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

namespace Gallio.Reflection.Impl
{
    internal sealed class NativeGenericParameterWrapper : NativeTypeWrapper, IGenericParameterInfo
    {
        public NativeGenericParameterWrapper(Type target)
            : base(target)
        {
        }

        public override CodeElementKind Kind
        {
            get { return CodeElementKind.GenericParameter; }
        }

        public GenericParameterAttributes GenericParameterAttributes
        {
            get { return Target.GenericParameterAttributes; }
        }

        public IMethodInfo DeclaringMethod
        {
            get { return Reflector.Wrap((MethodInfo)Target.DeclaringMethod); }
        }

        public IList<ITypeInfo> Constraints
        {
            get { return Array.ConvertAll<Type, ITypeInfo>(Target.GetGenericParameterConstraints(), Reflector.Wrap); }
        }

        public ITypeInfo ValueType
        {
            get { return Reflector.Wrap(typeof(Type)); }
        }

        public int Position
        {
            get { return Target.GenericParameterPosition; }
        }

        public bool Equals(ISlotInfo other)
        {
            return Equals((object)other);
        }

        public bool Equals(IGenericParameterInfo other)
        {
            return Equals((object)other);
        }
    }
}