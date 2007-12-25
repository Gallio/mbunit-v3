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

using System.Reflection;
using Gallio.Reflection;
using Gallio.Reflection.Impl;
using Gallio.ReSharperRunner.Reflection.Impl;
using JetBrains.ReSharper.Psi;

namespace Gallio.ReSharperRunner.Reflection.Impl
{
    internal sealed class PsiMethodWrapper : PsiFunctionWrapper<IFunction>, IMethodInfo
    {
        public PsiMethodWrapper(PsiReflector reflector, IFunction target)
            : base(reflector, target)
        {
        }

        public ITypeInfo ReturnType
        {
            get { return Reflector.Wrap(Target.ReturnType, true); }
        }

        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Method; }
        }

        public override MethodBase ResolveMethodBase()
        {
            return Resolve();
        }

        public MethodInfo Resolve()
        {
            return ReflectorResolveUtils.ResolveMethod(this);
        }
    }
}