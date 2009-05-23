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
    /// A <see cref="StaticReflectionPolicy"/> array type wrapper.
    /// </summary>
    public sealed class StaticArrayTypeWrapper : StaticConstructedTypeWrapper
    {
        private readonly int arrayRank;

        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy</param>
        /// <param name="elementType">The element type</param>
        /// <param name="arrayRank">The array rank</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/> or <paramref name="elementType" /> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="arrayRank"/> is less than 1</exception>
        public StaticArrayTypeWrapper(StaticReflectionPolicy policy, StaticTypeWrapper elementType, int arrayRank)
            : base(policy, elementType)
        {
            if (arrayRank <= 0)
                throw new ArgumentOutOfRangeException("arrayRank", "The array rank must be at least 1.");

            this.arrayRank = arrayRank;
        }

        /// <inheritdoc />
        public override bool IsArray
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override int ArrayRank
        {
            get { return arrayRank; }
        }

        /// <excludedoc />
        protected override ITypeInfo BaseTypeInternal
        {
            get { return Reflector.Wrap(typeof(Array)); }
        }

        /// <inheritdoc />
        public override TypeAttributes TypeAttributes
        {
            get { return TypeAttributes.Public | TypeAttributes.Sealed; }
        }

        /// <excludedoc />
        protected internal override ITypeInfo ApplySubstitution(StaticTypeSubstitution substitution)
        {
            return ElementType.ApplySubstitution(substitution).MakeArrayType(arrayRank);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            StaticArrayTypeWrapper other = obj as StaticArrayTypeWrapper;
            return other != null && ElementType.Equals(other.ElementType) && arrayRank == other.arrayRank;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return ElementType.GetHashCode() ^ GetType().GetHashCode() ^ arrayRank;
        }

        /// <excludedoc />
        protected override string NameSuffix
        {
            get { return arrayRank == 1 ? @"[]" : (@"[" + new String(',', arrayRank - 1) + @"]"); }
        }
    }
}
