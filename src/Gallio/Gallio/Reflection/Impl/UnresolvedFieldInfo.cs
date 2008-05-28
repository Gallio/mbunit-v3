// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Collections;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// Represents a <see cref="FieldInfo" /> whose native definition could not be resolved
    /// so we fall back on the <see cref="IFieldInfo"/> wrapper.
    /// </summary>
    public sealed partial class UnresolvedFieldInfo : FieldInfo, IUnresolvedCodeElement
    {
        private readonly IFieldInfo adapter;

        /// <summary>
        /// Creates a reflection object backed by the specified adapter.
        /// </summary>
        /// <param name="adapter">The adapter</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="adapter"/> is null</exception>
        public UnresolvedFieldInfo(IFieldInfo adapter)
        {
            if (adapter == null)
                throw new ArgumentNullException("adapter");

            this.adapter = adapter;
        }

        /// <summary>
        /// Gets the underlying reflection adapter.
        /// </summary>
        public IFieldInfo Adapter
        {
            get { return adapter; }
        }

        ICodeElementInfo IUnresolvedCodeElement.Adapter
        {
            get { return adapter; }
        }

        /// <inheritdoc />
        public override FieldAttributes Attributes
        {
            get { return adapter.FieldAttributes; }
        }

        /// <inheritdoc />
        public override RuntimeFieldHandle FieldHandle
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override Type FieldType
        {
            get { return adapter.ValueType.Resolve(false); }
        }

        /// <inheritdoc />
        public override MemberTypes MemberType
        {
            get { return MemberTypes.Field; }
        }

        /// <inheritdoc />
        public override Type[] GetOptionalCustomModifiers()
        {
            return EmptyArray<Type>.Instance;
        }

        /// <inheritdoc />
        public override object GetRawConstantValue()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override Type[] GetRequiredCustomModifiers()
        {
            return EmptyArray<Type>.Instance;
        }

        /// <inheritdoc />
        public override object GetValue(object obj)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        [CLSCompliant(false)]
        public override object GetValueDirect(TypedReference obj)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        [CLSCompliant(false)]
        public override void SetValueDirect(TypedReference obj, object value)
        {
            throw new NotSupportedException();
        }
    }
}