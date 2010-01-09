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
using System.Reflection;
using Gallio.Common.Collections;

#if DOTNET40
namespace Gallio.Common.Reflection.Impl.DotNet40
#else
namespace Gallio.Common.Reflection.Impl.DotNet20
#endif
{
    internal sealed partial class UnresolvedParameterInfo : ParameterInfo, IUnresolvedCodeElement
    {
        private readonly IParameterInfo adapter;

        internal UnresolvedParameterInfo(IParameterInfo adapter)
        {
            if (adapter == null)
                throw new ArgumentNullException("adapter");

            this.adapter = adapter;
        }

        public IParameterInfo Adapter
        {
            get { return adapter; }
        }

        ICodeElementInfo IUnresolvedCodeElement.Adapter
        {
            get { return adapter; }
        }

        public override ParameterAttributes Attributes
        {
            get { return adapter.ParameterAttributes; }
        }

        public override object DefaultValue
        {
            get { throw new NotSupportedException("Cannot get default value of unresolved parameter."); }
        }

        public override MemberInfo Member
        {
            get { return adapter.Member.Resolve(false); }
        }

        public override string Name
        {
            get { return adapter.Name; }
        }

        public override Type ParameterType
        {
            get { return adapter.ValueType.Resolve(false); }
        }

        public override int Position
        {
            get { return adapter.Position; }
        }

        public override object RawDefaultValue
        {
            get { throw new NotSupportedException("Cannot get default value of unresolved parameter."); }
        }

        public override bool Equals(object o)
        {
            UnresolvedParameterInfo other = o as UnresolvedParameterInfo;
            return other != null && adapter.Equals(other.adapter);
        }

        public override int GetHashCode()
        {
            return adapter.GetHashCode();
        }

        public override Type[] GetOptionalCustomModifiers()
        {
            return EmptyArray<Type>.Instance;
        }

        public override Type[] GetRequiredCustomModifiers()
        {
            return EmptyArray<Type>.Instance;
        }

        public override string ToString()
        {
            return adapter.ToString();
        }

        #region .Net 4.0 Only
#if DOTNET40
        public override int MetadataToken
        {
            get { throw new NotSupportedException("Cannot get metadata token of unresolved parameter."); }
        }
#endif
        #endregion
    }
}