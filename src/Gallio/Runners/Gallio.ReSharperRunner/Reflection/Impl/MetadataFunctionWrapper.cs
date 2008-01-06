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
using Gallio.Reflection;
using Gallio.Reflection.Impl;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;

namespace Gallio.ReSharperRunner.Reflection.Impl
{
    internal abstract class MetadataFunctionWrapper : MetadataMemberWrapper<IMetadataMethod>, IFunctionInfo
    {
        public MetadataFunctionWrapper(MetadataReflector reflector, IMetadataMethod target)
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

        public MethodAttributes MethodAttributes
        {
            get
            {
                MethodAttributes flags = 0;
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Abstract, Target.IsAbstract);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Assembly, Target.IsAssembly);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Family, Target.IsFamily);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.FamANDAssem, Target.IsFamilyAndAssembly);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.FamORAssem, Target.IsFamilyOrAssembly);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Final, Target.IsFinal);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.HideBySig, Target.IsHideBySig);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.NewSlot, Target.IsNewSlot);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Private, Target.IsPrivate);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Public, Target.IsPublic);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.SpecialName, Target.IsSpecialName);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Static, Target.IsStatic);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Virtual, Target.IsVirtual);
                return flags;
            }
        }

        public bool IsAbstract
        {
            get { return Target.IsAbstract; }
        }

        public bool IsPublic
        {
            get { return Target.IsPublic; }
        }

        public bool IsStatic
        {
            get { return Target.IsStatic; }
        }

        public IList<IParameterInfo> Parameters
        {
            get
            {
                IMetadataParameter[] parameters = Target.Parameters;
                return Array.ConvertAll<IMetadataParameter, IParameterInfo>(parameters, Reflector.Wrap);
            }
        }

        public IList<IGenericParameterInfo> GenericParameters
        {
            get
            {
                IMetadataGenericArgument[] parameters = Target.GenericArguments;
                return Array.ConvertAll<IMetadataGenericArgument, IGenericParameterInfo>(parameters, Reflector.Wrap);
            }
        }

        public override MemberInfo ResolveMemberInfo()
        {
            return ResolveMethodBase();
        }

        MethodBase IFunctionInfo.Resolve()
        {
            return ResolveMethodBase();
        }

        public abstract MethodBase ResolveMethodBase();

        public bool Equals(IFunctionInfo other)
        {
            return Equals((object)other);
        }
    }
}