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
using Gallio.Collections;
using Gallio.Reflection;
using Gallio.Reflection.Impl;
using JetBrains.ReSharper.Psi;

namespace Gallio.ReSharperRunner.Reflection.Impl
{
    internal abstract class PsiFunctionWrapper<TTarget> : PsiMemberWrapper<TTarget>, IFunctionInfo
        where TTarget : class, IFunction
    {
        public PsiFunctionWrapper(PsiReflector reflector, TTarget target)
            : base(reflector, target)
        {
        }

        public MethodAttributes MethodAttributes
        {
            get
            {
                MethodAttributes flags = 0;
                IModifiersOwner modifiers = Target;

                switch (modifiers.GetAccessRights())
                {
                    case AccessRights.PUBLIC:
                        flags |= MethodAttributes.Public;
                        break;
                    case AccessRights.PRIVATE:
                        flags |= MethodAttributes.Private;
                        break;
                    case AccessRights.NONE:
                    case AccessRights.INTERNAL:
                        flags |= MethodAttributes.Assembly;
                        break;
                    case AccessRights.PROTECTED:
                        flags |= MethodAttributes.Family;
                        break;
                    case AccessRights.PROTECTED_AND_INTERNAL:
                        flags |= MethodAttributes.FamANDAssem;
                        break;
                    case AccessRights.PROTECTED_OR_INTERNAL:
                        flags |= MethodAttributes.FamORAssem;
                        break;
                }

                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Abstract, modifiers.IsAbstract);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Final, modifiers.IsSealed);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Static, modifiers.IsStatic);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Virtual, modifiers.IsVirtual);
                return flags;
            }
        }

        public bool IsAbstract
        {
            get { return Target.IsAbstract; }
        }

        public bool IsPublic
        {
            get { return Target.AccessibilityDomain.DomainType == AccessibilityDomain.AccessibilityDomainType.PUBLIC; }
        }

        public bool IsStatic
        {
            get { return Target.IsStatic; }
        }

        public IList<IParameterInfo> Parameters
        {
            get
            {
                IList<IParameter> parameters = Target.Parameters;
                return GenericUtils.ConvertAllToArray<IParameter, IParameterInfo>(parameters, Reflector.Wrap);
            }
        }

        public override MemberInfo ResolveMemberInfo(bool throwOnError)
        {
            return ResolveMethodBase(throwOnError);
        }

        MethodBase IFunctionInfo.Resolve(bool throwOnError)
        {
            return ResolveMethodBase(throwOnError);
        }

        public abstract MethodBase ResolveMethodBase(bool throwOnError);

        public bool Equals(IFunctionInfo other)
        {
            return Equals((object)other);
        }
    }
}