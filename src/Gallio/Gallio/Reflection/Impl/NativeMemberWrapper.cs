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
using Gallio.Reflection.Impl;

namespace Gallio.Reflection.Impl
{
    internal abstract class NativeMemberWrapper<TTarget> : NativeCodeElementWrapper<TTarget>, IMemberInfo
        where TTarget : MemberInfo
    {
        public NativeMemberWrapper(TTarget target)
            : base(target)
        {
        }

        public override string Name
        {
            get { return Target.Name; }
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
            get { return CodeReference.CreateFromMember(Target); }
        }

        public override string GetXmlDocumentation()
        {
            return XmlDocumentationUtils.GetXmlDocumentation(Target);
        }

        public override CodeLocation GetCodeLocation()
        {
            return DeclaringType.GetCodeLocation();
        }

        public ITypeInfo DeclaringType
        {
            get { return Reflector.Wrap(Target.DeclaringType); }
        }

        public MemberInfo Resolve()
        {
            return Target;
        }

        public bool Equals(IMemberInfo other)
        {
            return Equals((object)other);
        }
    }
}