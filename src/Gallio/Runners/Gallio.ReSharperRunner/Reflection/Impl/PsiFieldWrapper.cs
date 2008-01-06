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
using JetBrains.ReSharper.Psi;

namespace Gallio.ReSharperRunner.Reflection.Impl
{
    internal sealed class PsiFieldWrapper : PsiMemberWrapper<IField>, IFieldInfo
    {
        public PsiFieldWrapper(PsiReflector reflector, IField target)
            : base(reflector, target)
        {
        }

        public ITypeInfo ValueType
        {
            get { return Reflector.Wrap(Target.Type, true); }
        }

        public int Position
        {
            get { return 0; }
        }

        public override MemberInfo ResolveMemberInfo()
        {
            return Resolve();
        }

        public FieldAttributes FieldAttributes
        {
            get
            {
                FieldAttributes flags = 0;
                IModifiersOwner modifiers = Target;

                switch (modifiers.GetAccessRights())
                {
                    case AccessRights.PUBLIC:
                        flags |= FieldAttributes.Public;
                        break;
                    case AccessRights.PRIVATE:
                        flags |= FieldAttributes.Private;
                        break;
                    case AccessRights.NONE:
                    case AccessRights.INTERNAL:
                        flags |= FieldAttributes.Assembly;
                        break;
                    case AccessRights.PROTECTED:
                        flags |= FieldAttributes.Family;
                        break;
                    case AccessRights.PROTECTED_AND_INTERNAL:
                        flags |= FieldAttributes.FamANDAssem;
                        break;
                    case AccessRights.PROTECTED_OR_INTERNAL:
                        flags |= FieldAttributes.FamORAssem;
                        break;
                }

                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, FieldAttributes.Static, modifiers.IsStatic);
                return flags;
            }
        }

        public bool IsLiteral
        {
            get { return Target.IsConstant; }
        }

        public bool IsPublic
        {
            get { return Target.AccessibilityDomain.DomainType == AccessibilityDomain.AccessibilityDomainType.PUBLIC; }
        }

        public bool IsInitOnly
        {
            get { return Target.IsReadonly; }
        }

        public bool IsStatic
        {
            get { return Target.IsStatic; }
        }

        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Field; }
        }

        public FieldInfo Resolve()
        {
            return ReflectorResolveUtils.ResolveField(this);
        }

        public bool Equals(ISlotInfo other)
        {
            return Equals((object)other);
        }

        public bool Equals(IFieldInfo other)
        {
            return Equals((object)other);
        }
    }
}