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
using JetBrains.ReSharper.Psi;

namespace Gallio.ReSharperRunner.Reflection.Impl
{
    internal sealed class PsiParameterWrapper : PsiCodeElementWrapper<IParameter>, IParameterInfo
    {
        public PsiParameterWrapper(PsiReflector reflector, IParameter target)
            : base(reflector, target)
        {
        }

        public override IDeclaredElement DeclaredElement
        {
            get { return Target; }
        }

        public override CodeReference CodeReference
        {
            get
            {
                CodeReference reference = Member.CodeReference;
                reference.ParameterName = Name;
                return reference;
            }
        }

        public ITypeInfo ValueType
        {
            get { return Reflector.Wrap(Target.Type, true); }
        }

        public int Position
        {
            get
            {
                return Target.ContainingParametersOwner.Parameters.IndexOf(Target);
            }
        }

        public IMemberInfo Member
        {
            get { return Reflector.Wrap((IFunction)Target.ContainingParametersOwner); }
        }

        public ParameterAttributes ParameterAttributes
        {
            get
            {
                ParameterAttributes flags = 0;
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, ParameterAttributes.HasDefault, ! Target.GetDefaultValue().IsBadValue());
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, ParameterAttributes.In, Target.Kind != ParameterKind.OUTPUT);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, ParameterAttributes.Out, Target.Kind != ParameterKind.VALUE);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, ParameterAttributes.Optional, Target.IsOptional);
                return flags;
            }
        }

        public override string Name
        {
            get { return Target.ShortName; }
        }

        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Parameter; }
        }

        public override IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit)
        {
            return EnumerateAttributesForElement(Target, inherit);
        }

        public ParameterInfo Resolve(bool throwOnError)
        {
            return ReflectorResolveUtils.ResolveParameter(this, throwOnError);
        }

        public bool Equals(ISlotInfo other)
        {
            return Equals((object)other);
        }

        public bool Equals(IParameterInfo other)
        {
            return Equals((object)other);
        }
    }
}