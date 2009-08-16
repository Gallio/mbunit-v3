// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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
using System.Reflection;

#if DOTNET40
namespace Gallio.Common.Reflection.Impl.DotNet40
#else
namespace Gallio.Common.Reflection.Impl.DotNet20
#endif
{
    internal sealed partial class UnresolvedMethodInfo : MethodInfo, IUnresolvedCodeElement
    {
        private readonly IMethodInfo adapter;

        internal UnresolvedMethodInfo(IMethodInfo adapter)
        {
            if (adapter == null)
                throw new ArgumentNullException("adapter");

            this.adapter = adapter;
        }

        public IMethodInfo Adapter
        {
            get { return adapter; }
        }

        ICodeElementInfo IUnresolvedCodeElement.Adapter
        {
            get { return adapter; }
        }

        public override MemberTypes MemberType
        {
            get { return MemberTypes.Method; }
        }

        public override ParameterInfo ReturnParameter
        {
            get { return adapter.ReturnParameter.Resolve(false); }
        }

        public override Type ReturnType
        {
            get { return adapter.ReturnType.Resolve(false); }
        }

        public override ICustomAttributeProvider ReturnTypeCustomAttributes
        {
            get { return ReturnParameter; }
        }

        public override MethodInfo GetBaseDefinition()
        {
            throw new NotImplementedException();
        }

        public override MethodInfo GetGenericMethodDefinition()
        {
            IMethodInfo genericMethodDefinition = adapter.GenericMethodDefinition;
            if (genericMethodDefinition == null)
                throw new InvalidOperationException("The method is not generic.");

            return genericMethodDefinition.Resolve(false);
        }

        public override MethodInfo MakeGenericMethod(params Type[] typeArguments)
        {
            return adapter.MakeGenericMethod(Reflector.Wrap(typeArguments)).Resolve(false);
        }

        #region .Net 4.0 Only
#if DOTNET40
#endif
        #endregion
    }
}