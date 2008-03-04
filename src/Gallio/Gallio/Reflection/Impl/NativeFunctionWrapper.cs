// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

namespace Gallio.Reflection.Impl
{
    internal abstract class NativeFunctionWrapper<TTarget> : NativeMemberWrapper<TTarget>, IFunctionInfo
        where TTarget : MethodBase
    {
        public NativeFunctionWrapper(TTarget target)
            : base(target)
        {
        }

        public MethodAttributes MethodAttributes
        {
            get { return Target.Attributes; }
        }

        public CallingConventions CallingConvention
        {
            get { return Target.CallingConvention; }
        }

        public bool IsAbstract
        {
            get { return Target.IsAbstract; }
        }

        public bool IsFinal
        {
            get { return Target.IsFinal; }
        }

        public bool IsStatic
        {
            get { return Target.IsStatic; }
        }

        public bool IsVirtual
        {
            get { return Target.IsVirtual; }
        }

        public bool IsAssembly
        {
            get { return Target.IsAssembly; }
        }

        public bool IsFamily
        {
            get { return Target.IsFamily; }
        }

        public bool IsFamilyAndAssembly
        {
            get { return Target.IsFamilyAndAssembly; }
        }

        public bool IsFamilyOrAssembly
        {
            get { return Target.IsFamilyOrAssembly; }
        }

        public bool IsPrivate
        {
            get { return Target.IsPrivate; }
        }

        public bool IsPublic
        {
            get { return Target.IsPublic; }
        }

        public bool IsHideBySig
        {
            get { return Target.IsHideBySig; }
        }

        public IList<IParameterInfo> Parameters
        {
            get
            {
                ParameterInfo[] parameters = Target.GetParameters();
                return Array.ConvertAll<ParameterInfo, IParameterInfo>(parameters, Reflector.Wrap);
            }
        }

        new public MethodBase Resolve(bool throwOnError)
        {
            return Target;
        }

        public override CodeLocation GetCodeLocation()
        {
            return DebugSymbolUtils.GetSourceLocation(Target)
                ?? base.GetCodeLocation();
        }

        public bool Equals(IFunctionInfo other)
        {
            return Equals((object)other);
        }
    }
}