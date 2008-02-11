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
using System.Reflection;
using System.Text;
using Gallio.Utilities;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> field wrapper.
    /// </summary>
    public class StaticFieldWrapper : StaticMemberWrapper, IFieldInfo
    {
        private readonly Memoizer<FieldAttributes> fieldAttributesMemoizer = new Memoizer<FieldAttributes>();

        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy</param>
        /// <param name="handle">The underlying reflection object</param>
        /// <param name="declaringType">The declaring type</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/>, <paramref name="handle"/>,
        /// or <paramref name="declaringType"/> is null</exception>
        public StaticFieldWrapper(StaticReflectionPolicy policy, object handle, StaticDeclaredTypeWrapper declaringType)
            : base(policy, handle, declaringType)
        {
            if (declaringType == null)
                throw new ArgumentNullException("declaringType");
        }

        /// <inheritdoc />
        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Field; }
        }

        /// <inheritdoc />
        public FieldAttributes FieldAttributes
        {
            get
            {
                return fieldAttributesMemoizer.Memoize(delegate
                {
                    return Policy.GetFieldAttributes(this); 
                });
            }
        }

        /// <inheritdoc />
        public bool IsLiteral
        {
            get { return (FieldAttributes & FieldAttributes.Literal) != 0; }
        }

        /// <inheritdoc />
        public bool IsPublic
        {
            get { return (FieldAttributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Public; }
        }

        /// <inheritdoc />
        public bool IsInitOnly
        {
            get { return (FieldAttributes & FieldAttributes.InitOnly) != 0; }
        }

        /// <inheritdoc />
        public bool IsStatic
        {
            get { return (FieldAttributes & FieldAttributes.Static) != 0; }
        }

        /// <inheritdoc />
        public ITypeInfo ValueType
        {
            get { return Substitution.Apply(Policy.GetFieldType(this)); }
        }

        /// <inheritdoc />
        public int Position
        {
            get { return 0; }
        }

        /// <inheritdoc />
        public bool Equals(ISlotInfo other)
        {
            return Equals((object)other);
        }

        /// <inheritdoc />
        public bool Equals(IFieldInfo other)
        {
            return Equals((object)other);
        }

        /// <inheritdoc />
        public FieldInfo Resolve(bool throwOnError)
        {
            return ReflectorResolveUtils.ResolveField(this, throwOnError);
        }

        /// <inheritdoc />
        protected override MemberInfo ResolveMemberInfo(bool throwOnError)
        {
            return Resolve(throwOnError);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            StringBuilder sig = new StringBuilder();

            sig.Append(GetTypeNameForSignature(ValueType));
            sig.Append(' ');
            sig.Append(Name);

            return sig.ToString();
        }
    }
}
