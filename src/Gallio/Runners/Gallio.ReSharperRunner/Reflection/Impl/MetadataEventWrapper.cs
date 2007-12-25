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
using Gallio.Reflection.Impl;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;

namespace Gallio.ReSharperRunner.Reflection.Impl
{
    internal sealed class MetadataEventWrapper : MetadataMemberWrapper<IMetadataEvent>, IEventInfo
    {
        public MetadataEventWrapper(MetadataReflector reflector, IMetadataEvent target)
            : base(reflector, target)
        {
        }

        protected override IDeclaredElement GetDeclaredElementWithLock()
        {
            return Reflector.GetDeclaredElementWithLock(Target);
        }

        public override ITypeInfo DeclaringType
        {
            get { return Reflector.WrapOpenType(Target.DeclaringType); }
        }

        public override string Name
        {
            get { return Target.Name; }
        }

        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Event; }
        }

        public override MemberInfo ResolveMemberInfo()
        {
            return Resolve();
        }

        public IMethodInfo GetAddMethod()
        {
            return Reflector.WrapMethod(Target.Adder);
        }

        public IMethodInfo GetRaiseMethod()
        {
            return Reflector.WrapMethod(Target.Raiser);
        }

        public IMethodInfo GetRemoveMethod()
        {
            return Reflector.WrapMethod(Target.Remover);
        }

        public EventInfo Resolve()
        {
            return ReflectorResolveUtils.ResolveEvent(this);
        }

        public bool Equals(IEventInfo other)
        {
            return Equals((object)other);
        }

        public override IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit)
        {
            return ReflectorAttributeUtils.EnumerateEventAttributes(this, inherit, delegate(IEventInfo member)
            {
                return EnumerateAttributesForEntity(((MetadataEventWrapper)member).Target);
            });
        }
    }
}