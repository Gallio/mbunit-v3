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

namespace Gallio.Common.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> by-ref type wrapper.
    /// </summary>
    public sealed class StaticByRefTypeWrapper : StaticConstructedTypeWrapper
    {
        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy.</param>
        /// <param name="elementType">The element type.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/> or <paramref name="elementType" /> is null.</exception>
        public StaticByRefTypeWrapper(StaticReflectionPolicy policy, StaticTypeWrapper elementType)
            : base(policy, elementType)
        {
        }

        /// <inheritdoc />
        public override bool IsByRef
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override TypeAttributes TypeAttributes
        {
            get { return TypeAttributes.Class; }
        }

        /// <excludedoc />
        protected internal override ITypeInfo ApplySubstitution(StaticTypeSubstitution substitution)
        {
            return ElementType.ApplySubstitution(substitution).MakeByRefType();
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            StaticByRefTypeWrapper other = obj as StaticByRefTypeWrapper;
            return other != null && ElementType.Equals(other.ElementType);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return ElementType.GetHashCode() ^ GetType().GetHashCode();
        }

        /// <excludedoc />
        protected override string NameSuffix
        {
            get { return @"&"; }
        }
    }
}
