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

namespace Gallio.Common.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> member wrapper for members that are not types,
    /// generic parameters or nested types.  These members must be declared by types, so
    /// they all share the constraint that the declaring type and reflected type must not be null.
    /// In particular, the reflected type may be a subtype of the declaring type in the case
    /// of inherited members.
    /// </summary>
    public abstract class StaticReflectedMemberWrapper : StaticMemberWrapper
    {
        private readonly StaticDeclaredTypeWrapper reflectedType;

        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy.</param>
        /// <param name="handle">The underlying reflection object.</param>
        /// <param name="declaringType">The declaring type.</param>
        /// <param name="reflectedType">The reflected type.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/>, <paramref name="handle"/>,
        /// <paramref name="declaringType"/>, or <paramref name="reflectedType"/> is null.</exception>
        protected StaticReflectedMemberWrapper(StaticReflectionPolicy policy, object handle, StaticDeclaredTypeWrapper declaringType,
            StaticDeclaredTypeWrapper reflectedType)
            : base(policy, handle, declaringType)
        {
            if (declaringType == null)
                throw new ArgumentNullException("declaringType");
            if (reflectedType == null)
                throw new ArgumentNullException("reflectedType");

            this.reflectedType = reflectedType;
        }

        /// <inheritdoc />
        public override StaticDeclaredTypeWrapper ReflectedType
        {
            get { return reflectedType; }
        }
    }
}
