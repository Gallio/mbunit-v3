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
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;

namespace Gallio.ReSharperRunner.Reflection.Impl
{
    internal sealed class MetadataFieldWrapper : MetadataMemberWrapper<IMetadataField>, IFieldInfo
    {
        public MetadataFieldWrapper(MetadataReflector reflector, IMetadataField target)
            : base(reflector, target)
        {
        }

        protected override IDeclaredElement GetDeclaredElementWithLock()
        {
            return Reflector.GetDeclaredElementWithLock(Target);
        }

        public ITypeInfo ValueType
        {
            get { return Reflector.Wrap(Target.Type); }
        }

        public int Position
        {
            get { return 0; }
        }

        public override ITypeInfo DeclaringType
        {
            get { return Reflector.WrapOpenType(Target.DeclaringType); }
        }

        public override string Name
        {
            get { return Target.Name; }
        }

        public override MemberInfo ResolveMemberInfo(bool throwOnError)
        {
            return Resolve(throwOnError);
        }

        public FieldAttributes FieldAttributes
        {
            get
            {
                FieldAttributes flags = 0;
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, FieldAttributes.Assembly, Target.IsAssembly);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, FieldAttributes.Family, Target.IsFamily);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, FieldAttributes.FamANDAssem, Target.IsFamilyAndAssembly);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, FieldAttributes.FamORAssem, Target.IsFamilyOrAssembly);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, FieldAttributes.Private, Target.IsPrivate);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, FieldAttributes.Public, Target.IsPublic);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, FieldAttributes.SpecialName, Target.IsSpecialName);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, FieldAttributes.Static, Target.IsStatic);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, FieldAttributes.Literal, Target.IsLiteral);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, FieldAttributes.InitOnly, Target.IsInitOnly);
                return flags;
            }
        }

        public bool IsLiteral
        {
            get { return Target.IsLiteral; }
        }

        public bool IsPublic
        {
            get { return Target.IsPublic; }
        }

        public bool IsInitOnly
        {
            get { return Target.IsInitOnly; }
        }

        public bool IsStatic
        {
            get { return Target.IsStatic; }
        }

        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Field; }
        }

        public FieldInfo Resolve(bool throwOnError)
        {
            return ReflectorResolveUtils.ResolveField(this, throwOnError);
        }

        public bool Equals(ISlotInfo other)
        {
            return Equals((object)other);
        }

        public bool Equals(IFieldInfo other)
        {
            return Equals((object)other);
        }

        public override IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit)
        {
            return ReflectorAttributeUtils.EnumerateFieldAttributes(this, inherit, delegate(IFieldInfo member)
            {
                return EnumerateAttributesForEntity(((MetadataFieldWrapper)member).Target);
            });
        }
    }
}