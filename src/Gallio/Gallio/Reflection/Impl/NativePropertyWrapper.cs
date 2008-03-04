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
using Gallio.Reflection.Impl;

namespace Gallio.Reflection.Impl
{
    internal sealed class NativePropertyWrapper : NativeMemberWrapper<PropertyInfo>, IPropertyInfo
    {
        public NativePropertyWrapper(PropertyInfo target)
            : base(target)
        {
        }

        public ITypeInfo ValueType
        {
            get { return Reflector.Wrap(Target.PropertyType); }
        }

        public int Position
        {
            get { return 0; }
        }

        public PropertyAttributes PropertyAttributes
        {
            get { return Target.Attributes; }
        }

        public bool CanRead
        {
            get { return Target.CanRead; }
        }

        public bool CanWrite
        {
            get { return Target.CanWrite; }
        }

        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Property; }
        }

        public IMethodInfo GetMethod
        {
            get { return Reflector.Wrap(Target.GetGetMethod(true)); }
        }

        public IMethodInfo SetMethod
        {
            get { return Reflector.Wrap(Target.GetSetMethod(true)); }
        }

        public IList<IParameterInfo> IndexParameters
        {
            get { return Array.ConvertAll<ParameterInfo, IParameterInfo>(Target.GetIndexParameters(), Reflector.Wrap); }
        }

        new public PropertyInfo Resolve(bool throwOnError)
        {
            return Target;
        }

        public bool Equals(ISlotInfo other)
        {
            return Equals((object)other);
        }

        public bool Equals(IPropertyInfo other)
        {
            return Equals((object)other);
        }
    }
}