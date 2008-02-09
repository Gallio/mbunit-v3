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
using Gallio.ReSharperRunner.Reflection.Impl;
using JetBrains.Metadata.Reader.API;

namespace Gallio.ReSharperRunner.Reflection.Impl
{
    internal abstract class MetadataMemberWrapper<TTarget> : MetadataCodeElementWrapper<TTarget>, IMemberInfo
        where TTarget : class, IMetadataEntity
    {
        public MetadataMemberWrapper(MetadataReflector reflector, TTarget target)
            : base(reflector, target)
        {
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

        public abstract ITypeInfo DeclaringType { get; }

        MemberInfo IMemberInfo.Resolve(bool throwOnError)
        {
            return ResolveMemberInfo(throwOnError);
        }

        public abstract MemberInfo ResolveMemberInfo(bool throwOnError);

        public bool Equals(IMemberInfo other)
        {
            return Equals((object)other);
        }
    }
}