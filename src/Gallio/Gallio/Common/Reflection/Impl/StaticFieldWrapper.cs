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
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Gallio.Common;

namespace Gallio.Common.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> field wrapper.
    /// </summary>
    public sealed class StaticFieldWrapper : StaticReflectedMemberWrapper, IFieldInfo
    {
        private Memoizer<FieldAttributes> fieldAttributesMemoizer = new Memoizer<FieldAttributes>();
        private KeyedMemoizer<bool, FieldInfo> resolveMemoizer = new KeyedMemoizer<bool, FieldInfo>();

        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy</param>
        /// <param name="handle">The underlying reflection object</param>
        /// <param name="declaringType">The declaring type</param>
        /// <param name="reflectedType">The reflected type</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/>, <paramref name="handle"/>,
        /// <paramref name="declaringType"/>, or <paramref name="reflectedType"/> is null</exception>
        public StaticFieldWrapper(StaticReflectionPolicy policy, object handle, StaticDeclaredTypeWrapper declaringType,
            StaticDeclaredTypeWrapper reflectedType)
            : base(policy, handle, declaringType, reflectedType)
        {
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
        public bool IsAssembly
        {
            get { return (FieldAttributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Assembly; }
        }

        /// <inheritdoc />
        public bool IsFamily
        {
            get { return (FieldAttributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Family; }
        }

        /// <inheritdoc />
        public bool IsFamilyAndAssembly
        {
            get { return (FieldAttributes & FieldAttributes.FieldAccessMask) == FieldAttributes.FamANDAssem; }
        }

        /// <inheritdoc />
        public bool IsFamilyOrAssembly
        {
            get { return (FieldAttributes & FieldAttributes.FieldAccessMask) == FieldAttributes.FamORAssem; }
        }

        /// <inheritdoc />
        public bool IsPrivate
        {
            get { return (FieldAttributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Private; }
        }

        /// <inheritdoc />
        public bool IsPublic
        {
            get { return (FieldAttributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Public; }
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
            return resolveMemoizer.Memoize(throwOnError,
                () => ReflectorResolveUtils.ResolveField(this, throwOnError));
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

        /// <inheritdoc />
        protected override IEnumerable<Attribute> GetPseudoCustomAttributes()
        {
            if ((FieldAttributes & FieldAttributes.NotSerialized) != 0)
                yield return new NonSerializedAttribute();

            // TODO: Handle MarshalAs and FieldOffset.
        }
    }
}
