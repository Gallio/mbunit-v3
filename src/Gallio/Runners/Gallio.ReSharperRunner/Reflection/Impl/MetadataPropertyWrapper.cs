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

using System.Collections.Generic;
using System.Reflection;
using Gallio.Reflection;
using Gallio.Reflection.Impl;
using Gallio.ReSharperRunner.Reflection.Impl;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;

namespace Gallio.ReSharperRunner.Reflection.Impl
{
    internal sealed class MetadataPropertyWrapper : MetadataMemberWrapper<IMetadataProperty>, IPropertyInfo
    {
        public MetadataPropertyWrapper(MetadataReflector reflector, IMetadataProperty target)
            : base(reflector, target)
        {
        }

        protected override IDeclaredElement GetDeclaredElementWithLock()
        {
            return Reflector.GetDeclaredElementWithLock(Target);
        }

        public override string Name
        {
            get { return Target.Name; }
        }

        public override ITypeInfo DeclaringType
        {
            get { return Reflector.WrapOpenType(Target.DeclaringType); }
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
            get { return Reflector.WrapMethod(Target.Getter); }
        }

        public IMethodInfo SetMethod
        {
            get { return Reflector.WrapMethod(Target.Setter); }
        }

        public override MemberInfo ResolveMemberInfo()
        {
            return Resolve();
        }

        public PropertyInfo Resolve()
        {
            return ReflectorResolveUtils.ResolveProperty(this);
        }

        public bool Equals(ISlotInfo other)
        {
            return Equals((object)other);
        }

        public bool Equals(IPropertyInfo other)
        {
            return Equals((object)other);
        }

        public override IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit)
        {
            return ReflectorAttributeUtils.EnumeratePropertyAttributes(this, inherit, delegate(IPropertyInfo member)
            {
                return EnumerateAttributesForEntity(((MetadataPropertyWrapper)member).Target);
            });
        }
    }
}