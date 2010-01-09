// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Globalization;
using System.Reflection;

#if DOTNET40
namespace Gallio.Common.Reflection.Impl.DotNet40
#else
namespace Gallio.Common.Reflection.Impl.DotNet20
#endif
{
    internal sealed partial class UnresolvedConstructorInfo : ConstructorInfo, IUnresolvedCodeElement
    {
        private readonly IConstructorInfo adapter;

        internal UnresolvedConstructorInfo(IConstructorInfo adapter)
        {
            if (adapter == null)
                throw new ArgumentNullException("adapter");

            this.adapter = adapter;
        }

        public IConstructorInfo Adapter
        {
            get { return adapter; }
        }

        ICodeElementInfo IUnresolvedCodeElement.Adapter
        {
            get { return adapter; }
        }

        public override MemberTypes MemberType
        {
            get { return MemberTypes.Constructor; }
        }

        public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot invoke unresolved constructor.");
        }

        #region .Net 4.0 Only
#if DOTNET40
#endif
        #endregion
    }
}