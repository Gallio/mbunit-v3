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
using Gallio.Common.Collections;
using Gallio.Common;

namespace Gallio.Common.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> generic parameter wrapper.
    /// </summary>
    public sealed class StaticGenericParameterWrapper : StaticSpecialTypeWrapper, IGenericParameterInfo
    {
        private Memoizer<GenericParameterAttributes> genericParameterAttributesMemoizer = new Memoizer<GenericParameterAttributes>();
        private Memoizer<IList<ITypeInfo>> constraintsMemoizer = new Memoizer<IList<ITypeInfo>>();
        private Memoizer<int> positionMemoizer = new Memoizer<int>();

        private readonly StaticMethodWrapper declaringMethod;

        private StaticGenericParameterWrapper(StaticReflectionPolicy policy, object handle,
            StaticDeclaredTypeWrapper declaringType, StaticMethodWrapper declaringMethod)
            : base(policy, handle, declaringType)
        {
            this.declaringMethod = declaringMethod;
        }

        /// <summary>
        /// Creates a wrapper for a generic type parameter.
        /// </summary>
        /// <param name="policy">The reflection policy.</param>
        /// <param name="handle">The underlying reflection object.</param>
        /// <param name="declaringType">The declaring type, which must be a generic type definition.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/>, <paramref name="handle"/>
        /// or <paramref name="declaringType"/> is null.</exception>
        public static StaticGenericParameterWrapper CreateGenericTypeParameter(StaticReflectionPolicy policy, object handle,
            StaticDeclaredTypeWrapper declaringType)
        {
            if (declaringType == null)
                throw new ArgumentNullException("declaringType");

            return new StaticGenericParameterWrapper(policy, handle, declaringType, null);
        }

        /// <summary>
        /// Creates a wrapper for a generic method parameter.
        /// </summary>
        /// <param name="policy">The reflection policy.</param>
        /// <param name="handle">The underlying reflection object.</param>
        /// <param name="declaringMethod">The declaring method, which must be a generic method definition.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/>, <paramref name="handle"/>
        /// or <paramref name="declaringMethod"/> is null.</exception>
        public static StaticGenericParameterWrapper CreateGenericMethodParameter(StaticReflectionPolicy policy, object handle,
            StaticMethodWrapper declaringMethod)
        {
            if (declaringMethod == null)
                throw new ArgumentNullException("declaringMethod");

            return new StaticGenericParameterWrapper(policy, handle, declaringMethod.DeclaringType, declaringMethod);
        }

        /// <inheritdoc />
        public override StaticDeclaredTypeWrapper DeclaringType
        {
            get
            {
                StaticDeclaredTypeWrapper declaringType = base.DeclaringType;
                return declaringType.GenericTypeDefinition ?? declaringType;
            }
        }

        /// <summary>
        /// Gets the declaring method, or null if the generic parameter belongs to a type.
        /// </summary>
        public StaticMethodWrapper DeclaringMethod
        {
            get
            {
                if (declaringMethod == null)
                    return null;
                return declaringMethod.GenericMethodDefinition;
            }
        }
        IMethodInfo IGenericParameterInfo.DeclaringMethod
        {
            get { return DeclaringMethod; }
        }

        /// <inheritdoc />
        public override CodeElementKind Kind
        {
            get { return CodeElementKind.GenericParameter; }
        }

        /// <excludedoc />
        protected override ITypeInfo BaseTypeInternal
        {
            get { return Reflector.Wrap(typeof(Object)); }
        }

        /// <inheritdoc />
        public override bool IsGenericParameter
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override bool ContainsGenericParameters
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override string FullName
        {
            get { return null; }
        }

        /// <inheritdoc />
        public override string AssemblyQualifiedName
        {
            get { return null; }
        }

        /// <inheritdoc />
        public override IAssemblyInfo Assembly
        {
            get { return DeclaringType.Assembly; }
        }

        /// <inheritdoc />
        public override string NamespaceName
        {
            get { return DeclaringType.NamespaceName; }
        }

        /// <inheritdoc />
        public override TypeAttributes TypeAttributes
        {
            get { return TypeAttributes.Public; }
        }

        /// <inheritdoc />
        public GenericParameterAttributes GenericParameterAttributes
        {
            get
            {
                return genericParameterAttributesMemoizer.Memoize(delegate
                {
                    return ReflectionPolicy.GetGenericParameterAttributes(this);
                });
            }
        }

        /// <inheritdoc />
        public IList<ITypeInfo> Constraints
        {
            get
            {
                return constraintsMemoizer.Memoize(delegate
                {
                    return new CovariantList<StaticTypeWrapper, ITypeInfo>(ReflectionPolicy.GetGenericParameterConstraints(this));
                });
            }
        }

        /// <inheritdoc />
        public ITypeInfo ValueType
        {
            get { return Reflector.Wrap(typeof(Type)); }
        }

        /// <inheritdoc />
        public int Position
        {
            get { return positionMemoizer.Memoize(() => ReflectionPolicy.GetGenericParameterPosition(this)); }
        }

        /// <inheritdoc />
        public override IList<IConstructorInfo> GetConstructors(BindingFlags bindingFlags)
        {
            return EmptyArray<IConstructorInfo>.Instance;
        }

        /// <excludedoc />
        protected internal override ITypeInfo ApplySubstitution(StaticTypeSubstitution substitution)
        {
            return substitution.Apply(this);
        }

        /// <inheritdoc />
        public bool Equals(ISlotInfo other)
        {
            return Equals((object)other);
        }

        /// <inheritdoc />
        public bool Equals(IGenericParameterInfo other)
        {
            return Equals((object)other);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Name;
        }
    }
}
