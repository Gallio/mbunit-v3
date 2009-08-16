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
using System.Globalization;
using System.Reflection;
using Gallio.Common.Collections;

#if DOTNET40
namespace Gallio.Common.Reflection.Impl.DotNet40
#else
namespace Gallio.Common.Reflection.Impl.DotNet20
#endif
{
    internal sealed partial class UnresolvedFieldInfo : FieldInfo, IUnresolvedCodeElement
    {
        private readonly IFieldInfo adapter;

        internal UnresolvedFieldInfo(IFieldInfo adapter)
        {
            if (adapter == null)
                throw new ArgumentNullException("adapter");

            this.adapter = adapter;
        }

        public IFieldInfo Adapter
        {
            get { return adapter; }
        }

        ICodeElementInfo IUnresolvedCodeElement.Adapter
        {
            get { return adapter; }
        }

        public override FieldAttributes Attributes
        {
            get { return adapter.FieldAttributes; }
        }

        public override RuntimeFieldHandle FieldHandle
        {
            get { throw new NotSupportedException("Cannot get field handle of unresolved field."); }
        }

        public override Type FieldType
        {
            get { return adapter.ValueType.Resolve(false); }
        }

        public override MemberTypes MemberType
        {
            get { return MemberTypes.Field; }
        }

        public override Type[] GetOptionalCustomModifiers()
        {
            return EmptyArray<Type>.Instance;
        }

        public override object GetRawConstantValue()
        {
            throw new NotSupportedException("Cannot get constant value of unresolved field.");
        }

        public override Type[] GetRequiredCustomModifiers()
        {
            return EmptyArray<Type>.Instance;
        }

        public override object GetValue(object obj)
        {
            throw new NotSupportedException("Cannot get value of unresolved field.");
        }

        public override object GetValueDirect(TypedReference obj)
        {
            throw new NotSupportedException("Cannot get value of unresolved field.");
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder,
            CultureInfo culture)
        {
            throw new NotSupportedException("Cannot set value of unresolved field.");
        }

        public override void SetValueDirect(TypedReference obj, object value)
        {
            throw new NotSupportedException("Cannot set value of unresolved field.");
        }

        #region .Net 4.0 Only
#if DOTNET40
        public override bool IsSecurityCritical
        {
            get { return false; }
        }

        public override bool IsSecuritySafeCritical
        {
            get { return false; }
        }

        public override bool IsSecurityTransparent
        {
            get { return false; }
        }
#endif
        #endregion
    }
}