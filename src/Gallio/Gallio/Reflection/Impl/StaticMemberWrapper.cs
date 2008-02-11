// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using System.Reflection;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> member wrapper.
    /// </summary>
    public abstract class StaticMemberWrapper : StaticCodeElementWrapper, IMemberInfo
    {
        private readonly StaticDeclaredTypeWrapper declaringType;

        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy</param>
        /// <param name="handle">The underlying reflection object</param>
        /// <param name="declaringType">The declaring type, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/> or <paramref name="handle"/> is null</exception>
        public StaticMemberWrapper(StaticReflectionPolicy policy, object handle, StaticDeclaredTypeWrapper declaringType)
            : base(policy, handle)
        {
            this.declaringType = declaringType;
        }

        /// <summary>
        /// Gets the type substitution for generic parameters.
        /// </summary>
        public virtual StaticTypeSubstitution Substitution
        {
            get { return declaringType != null ? declaringType.Substitution : StaticTypeSubstitution.Empty; }
        }

        /// <inheritdoc />
        public override CodeReference CodeReference
        {
            get
            {
                CodeReference reference = DeclaringType.CodeReference;
                reference.MemberName = Name;
                return reference;
            }
        }

        /// <inheritdoc />
        public override string Name
        {
            get { return Policy.GetMemberName(this); }
        }

        /// <summary>
        /// Gets the declaring type.
        /// </summary>
        public StaticDeclaredTypeWrapper DeclaringType
        {
            get { return declaringType; }
        }
        ITypeInfo IMemberInfo.DeclaringType
        {
            get { return DeclaringType; }
        }

        /// <inheritdoc />
        public override CodeLocation GetCodeLocation()
        {
            CodeLocation location = Policy.GetMemberSourceLocation(this);
            if (location == null && declaringType != null)
            {
                location = DeclaringType.GetCodeLocation();
                if (location != null)
                {
                    location.Line = 0;
                    location.Column = 0;
                }
            }

            return location;
        }

        /// <inheritdoc />
        public bool Equals(IMemberInfo other)
        {
            return Equals((object)other);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            StaticMemberWrapper other = obj as StaticMemberWrapper;
            return EqualsByHandle(other) && Equals(declaringType, other.declaringType);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ (declaringType != null ? declaringType.GetHashCode() : 0);
        }

        /// <inheritdoc />
        protected override IEnumerable<StaticAttributeWrapper> GetCustomAttributes()
        {
            return Policy.GetMemberCustomAttributes(this);
        }

        /// <summary>
        /// Implementation of <see cref="IMemberInfo.Resolve" />
        /// </summary>
        protected abstract MemberInfo ResolveMemberInfo(bool throwOnError);

        MemberInfo IMemberInfo.Resolve(bool throwOnError)
        {
            return ResolveMemberInfo(throwOnError);
        }
    }
}
