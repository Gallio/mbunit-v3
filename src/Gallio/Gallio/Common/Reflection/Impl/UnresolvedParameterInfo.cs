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

namespace Gallio.Common.Reflection.Impl
{
    /// <summary>
    /// Represents a <see cref="ParameterInfo" /> whose native definition could not be resolved
    /// so we fall back on the <see cref="IParameterInfo"/> wrapper.
    /// </summary>
    public sealed partial class UnresolvedParameterInfo : ParameterInfo, IUnresolvedCodeElement
    {
        private readonly IParameterInfo adapter;

        /// <summary>
        /// Creates a reflection object backed by the specified adapter.
        /// </summary>
        /// <param name="adapter">The adapter</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="adapter"/> is null</exception>
        public UnresolvedParameterInfo(IParameterInfo adapter)
        {
            if (adapter == null)
                throw new ArgumentNullException("adapter");

            this.adapter = adapter;
        }

        /// <summary>
        /// Gets the underlying reflection adapter.
        /// </summary>
        public IParameterInfo Adapter
        {
            get { return adapter; }
        }

        ICodeElementInfo IUnresolvedCodeElement.Adapter
        {
            get { return adapter; }
        }

        /// <inheritdoc />
        public override ParameterAttributes Attributes
        {
            get { return adapter.ParameterAttributes; }
        }

        /// <inheritdoc />
        public override object DefaultValue
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override MemberInfo Member
        {
            get { return adapter.Member.Resolve(false); }
        }

        /// <inheritdoc />
        public override string Name
        {
            get { return adapter.Name; }
        }

        /// <inheritdoc />
        public override Type ParameterType
        {
            get { return adapter.ValueType.Resolve(false); }
        }

        /// <inheritdoc />
        public override int Position
        {
            get { return adapter.Position; }
        }

        /// <inheritdoc />
        public override object RawDefaultValue
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override bool Equals(object o)
        {
            UnresolvedParameterInfo other = o as UnresolvedParameterInfo;
            return other != null && adapter.Equals(other.adapter);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return adapter.GetHashCode();
        }

        /// <inheritdoc />
        public override Type[] GetOptionalCustomModifiers()
        {
            return EmptyArray<Type>.Instance;
        }

        /// <inheritdoc />
        public override Type[] GetRequiredCustomModifiers()
        {
            return EmptyArray<Type>.Instance;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return adapter.ToString();
        }
    }
}