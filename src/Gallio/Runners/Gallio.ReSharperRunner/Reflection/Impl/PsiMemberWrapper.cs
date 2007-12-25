// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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

using System.Collections.Generic;
using System.Reflection;
using Gallio.Reflection;
using Gallio.ReSharperRunner.Reflection.Impl;
using JetBrains.ReSharper.Psi;

namespace Gallio.ReSharperRunner.Reflection.Impl
{
    internal abstract class PsiMemberWrapper<TTarget> : PsiCodeElementWrapper<TTarget>, IMemberInfo
        where TTarget : class, ITypeMember
    {
        public PsiMemberWrapper(PsiReflector reflector, TTarget target)
            : base(reflector, target)
        {
        }

        public override IDeclaredElement DeclaredElement
        {
            get { return Target; }
        }

        public override string Name
        {
            get { return Target.ShortName; }
        }

        public virtual string CompoundName
        {
            get
            {
                ITypeInfo declaringType = DeclaringType;
                return declaringType != null ? declaringType.CompoundName + @"." + Name : Name;
            }
        }

        public override CodeReference CodeReference
        {
            get
            {
                CodeReference reference = DeclaringType.CodeReference;
                reference.MemberName = Name;
                return reference;
            }
        }

        public ITypeInfo DeclaringType
        {
            get { return Reflector.Wrap(Target.GetContainingType()); }
        }

        public override IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit)
        {
            return EnumerateAttributesForElement(Target, inherit);
        }

        MemberInfo IMemberInfo.Resolve()
        {
            return ResolveMemberInfo();
        }

        public abstract MemberInfo ResolveMemberInfo();

        public bool Equals(IMemberInfo other)
        {
            return Equals((object)other);
        }

        public override string ToString()
        {
            return CompoundName;
        }
    }
}