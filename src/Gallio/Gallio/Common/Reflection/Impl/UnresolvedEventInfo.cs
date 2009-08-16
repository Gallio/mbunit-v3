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
using Gallio.Common.Collections;

#if DOTNET40
namespace Gallio.Common.Reflection.Impl.DotNet40
#else
namespace Gallio.Common.Reflection.Impl.DotNet20
#endif
{
    internal sealed partial class UnresolvedEventInfo : EventInfo, IUnresolvedCodeElement
    {
        private readonly IEventInfo adapter;

        internal UnresolvedEventInfo(IEventInfo adapter)
        {
            if (adapter == null)
                throw new ArgumentNullException("adapter");

            this.adapter = adapter;
        }

        public IEventInfo Adapter
        {
            get { return adapter; }
        }

        ICodeElementInfo IUnresolvedCodeElement.Adapter
        {
            get { return adapter; }
        }

        public override EventAttributes Attributes
        {
            get { return adapter.EventAttributes; }
        }

        public override MemberTypes MemberType
        {
            get { return MemberTypes.Event; }
        }

        public override MethodInfo GetAddMethod(bool nonPublic)
        {
            return UnresolvedMemberInfo.ResolveMethod(adapter.AddMethod, nonPublic);
        }

        public override MethodInfo[] GetOtherMethods(bool nonPublic)
        {
            return EmptyArray<MethodInfo>.Instance;
        }

        public override MethodInfo GetRaiseMethod(bool nonPublic)
        {
            return UnresolvedMemberInfo.ResolveMethod(adapter.RaiseMethod, nonPublic);
        }

        public override MethodInfo GetRemoveMethod(bool nonPublic)
        {
            return UnresolvedMemberInfo.ResolveMethod(adapter.RemoveMethod, nonPublic);
        }

        #region .Net 4.0 Only
#if DOTNET40
#endif
        #endregion
    }
}