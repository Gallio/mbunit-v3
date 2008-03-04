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
using Gallio.Collections;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> constructed type wrapper.
    /// </summary>
    /// <seealso cref="StaticArrayTypeWrapper"/>
    /// <seealso cref="StaticByRefTypeWrapper"/>
    /// <seealso cref="StaticPointerTypeWrapper"/>
    public abstract class StaticConstructedTypeWrapper : StaticSpecialTypeWrapper
    {
        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy</param>
        /// <param name="elementType">The element type</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/> or <paramref name="elementType" /> is null</exception>
        public StaticConstructedTypeWrapper(StaticReflectionPolicy policy, StaticTypeWrapper elementType)
            : base(policy, elementType, null)
        {
        }

        /// <inheritdoc />
        public override StaticTypeWrapper ElementType
        {
            get { return (StaticTypeWrapper)Handle; }
        }

        /// <inheritdoc />
        public override bool ContainsGenericParameters
        {
            get { return ElementType.ContainsGenericParameters; }
        }

        /// <inheritdoc />
        public override IAssemblyInfo Assembly
        {
            get { return ElementType.Assembly; }
        }

        /// <inheritdoc />
        public override INamespaceInfo Namespace
        {
            get { return ElementType.Namespace; }
        }

        /// <inheritdoc />
        public override string Name
        {
            get { return ElementType.Name + NameSuffix; }
        }

        /// <inheritdoc />
        public override string FullName
        {
            get
            {
                string fullName = ElementType.FullName;
                if (fullName == null)
                    return null;

                return fullName + NameSuffix;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ElementType + NameSuffix;
        }

        /// <inheritdoc />
        public override CodeLocation GetCodeLocation()
        {
            return null;
        }

        /// <inheritdoc />
        protected override IEnumerable<StaticAttributeWrapper> GetCustomAttributes()
        {
            return EmptyArray<StaticAttributeWrapper>.Instance;
        }

        /// <summary>
        /// Gets the suffix to append to the name of the constructed type.
        /// </summary>
        protected abstract string NameSuffix { get; }
    }
}
