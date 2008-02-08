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

using System.Reflection;
using Gallio.Reflection;
using Gallio.Reflection.Impl;
using Gallio.ReSharperRunner.Reflection.Impl;
using JetBrains.ReSharper.Psi;

namespace Gallio.ReSharperRunner.Reflection.Impl
{
    internal sealed class PsiPropertyWrapper : PsiMemberWrapper<IProperty>, IPropertyInfo
    {
        public PsiPropertyWrapper(PsiReflector reflector, IProperty target)
            : base(reflector, target)
        {
        }

        public ITypeInfo ValueType
        {
            get { return Reflector.Wrap(Target.Type); }
        }

        public int Position
        {
            get { return 0; }
        }

        public PropertyAttributes PropertyAttributes
        {
            get
            {
                // Note: There don't seem to be any usable property attributes.
                return 0;
            }
        }

        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Property; }
        }

        public IMethodInfo GetMethod
        {
            get { return Reflector.Wrap(Target.Getter(false)); }
        }

        public IMethodInfo SetMethod
        {
            get { return Reflector.Wrap(Target.Setter(false)); }
        }

        public override MemberInfo ResolveMemberInfo(bool throwOnError)
        {
            return Resolve(throwOnError);
        }

        public PropertyInfo Resolve(bool throwOnError)
        {
            return ReflectorResolveUtils.ResolveProperty(this, throwOnError);
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