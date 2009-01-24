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
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Gallio.Collections;
using Gallio.Utilities;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> property wrapper.
    /// </summary>
    public sealed class StaticPropertyWrapper : StaticReflectedMemberWrapper, IPropertyInfo
    {
        private readonly Memoizer<PropertyAttributes> propertyAttributesMemoizer = new Memoizer<PropertyAttributes>();
        private readonly Memoizer<IList<StaticParameterWrapper>> indexParametersMemoizer = new Memoizer<IList<StaticParameterWrapper>>();
        private readonly Memoizer<StaticMethodWrapper> getMethodMemoizer = new Memoizer<StaticMethodWrapper>();
        private readonly Memoizer<StaticMethodWrapper> setMethodMemoizer = new Memoizer<StaticMethodWrapper>();
        private readonly Memoizer<ITypeInfo> valueTypeMemoizer = new Memoizer<ITypeInfo>();

        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy</param>
        /// <param name="handle">The underlying reflection object</param>
        /// <param name="declaringType">The declaring type</param>
        /// <param name="reflectedType">The reflected type</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/>, <paramref name="handle"/>,
        /// <paramref name="declaringType"/>, or <paramref name="reflectedType"/> is null</exception>
        public StaticPropertyWrapper(StaticReflectionPolicy policy, object handle, StaticDeclaredTypeWrapper declaringType,
            StaticDeclaredTypeWrapper reflectedType)
            : base(policy, handle, declaringType, reflectedType)
        {
        }

        /// <inheritdoc />
        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Property; }
        }

        /// <inheritdoc />
        public PropertyAttributes PropertyAttributes
        {
            get
            {
                return propertyAttributesMemoizer.Memoize(delegate
                {
                    return Policy.GetPropertyAttributes(this);
                });
            }
        }

        /// <inheritdoc />
        public bool CanRead
        {
            get { return GetMethod != null; }
        }

        /// <inheritdoc />
        public bool CanWrite
        {
            get { return SetMethod != null; }
        }

        /// <inheritdoc />
        public StaticMethodWrapper GetMethod
        {
            get { return getMethodMemoizer.Memoize(() => Policy.GetPropertyGetMethod(this)); }
        }
        IMethodInfo IPropertyInfo.GetMethod
        {
            get { return GetMethod; }
        }

        /// <inheritdoc />
        public StaticMethodWrapper SetMethod
        {
            get { return setMethodMemoizer.Memoize(() => Policy.GetPropertySetMethod(this)); }
        }
        IMethodInfo IPropertyInfo.SetMethod
        {
            get { return SetMethod; }
        }

        /// <inheritdoc />
        public IList<StaticParameterWrapper> IndexParameters
        {
            get
            {
                return indexParametersMemoizer.Memoize(delegate
                {
                    IList<StaticParameterWrapper> parameters;
                    int indexParameterCount;

                    StaticMethodWrapper getMethod = GetMethod;
                    if (getMethod != null)
                    {
                        parameters = getMethod.Parameters;
                        indexParameterCount = parameters.Count;
                    }
                    else
                    {
                        parameters = SetMethod.Parameters;
                        indexParameterCount = parameters.Count - 1;
                    }

                    if (indexParameterCount == 0)
                        return EmptyArray<StaticParameterWrapper>.Instance;

                    StaticParameterWrapper[] indexParameters = new StaticParameterWrapper[indexParameterCount];
                    for (int i = 0; i < indexParameterCount; i++)
                    {
                        StaticParameterWrapper parameter = parameters[i];
                        indexParameters[i] = new StaticParameterWrapper(parameter.Policy, parameter.Handle, this);
                    }

                    return indexParameters;
                });
            }
        }

        IList<IParameterInfo> IPropertyInfo.IndexParameters
        {
            get { return new CovariantList<StaticParameterWrapper, IParameterInfo>(IndexParameters); }
        }

        /// <inheritdoc />
        public ITypeInfo ValueType
        {
            get { return valueTypeMemoizer.Memoize(() => Substitution.Apply(Policy.GetPropertyType(this))); }
        }

        /// <inheritdoc />
        public int Position
        {
            get { return 0; }
        }

        /// <summary>
        /// Gets the properties that this one overrides or hides.
        /// Only includes overrides that appear on class types, not interfaces.
        /// </summary>
        /// <param name="overridesOnly">If true, only returns overrides</param>
        public IEnumerable<StaticPropertyWrapper> GetOverridenOrHiddenProperties(bool overridesOnly)
        {
            StaticMethodWrapper discriminator = GetDiscriminatorMethod(this);
            if (overridesOnly && !discriminator.IsOverride)
                yield break;

            string propertyName = Name;
            foreach (StaticDeclaredTypeWrapper baseType in DeclaringType.GetAllBaseTypes())
            {
                foreach (StaticPropertyWrapper other in Policy.GetTypeProperties(baseType, ReflectedType))
                {
                    if (propertyName == other.Name)
                    {
                        if (overridesOnly)
                        {
                            StaticMethodWrapper otherDiscriminator = GetDiscriminatorMethod(other);
                            if (otherDiscriminator == null)
                                yield break;

                            if (discriminator.HidesMethod(otherDiscriminator))
                            {
                                yield return other;

                                if (!otherDiscriminator.IsOverride)
                                    yield break;
                                break;
                            }
                        }
                        else
                        {
                            yield return other;
                        }
                    }
                }
            }
        }

        private StaticMethodWrapper GetDiscriminatorMethod(StaticPropertyWrapper property)
        {
            if (GetMethod != null)
                return property.GetMethod;
            if (SetMethod != null)
                return property.SetMethod;
            return null;
        }

        /// <inheritdoc />
        public bool Equals(ISlotInfo other)
        {
            return Equals((object)other);
        }

        /// <inheritdoc />
        public bool Equals(IPropertyInfo other)
        {
            return Equals((object)other);
        }

        /// <inheritdoc />
        public PropertyInfo Resolve(bool throwOnError)
        {
            return ReflectorResolveUtils.ResolveProperty(this, throwOnError);
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

            IList<StaticParameterWrapper> indexParameters = IndexParameters;
            if (indexParameters.Count != 0)
            {
                sig.Append(' ');
                sig.Append('[');
                AppendParameterListToSignature(sig, indexParameters, false);
                sig.Append(']');
            }

            return sig.ToString();
        }

        /// <inheritdoc />
        protected override IEnumerable<Attribute> GetPseudoCustomAttributes()
        {
            return EmptyArray<Attribute>.Instance;
        }
    }
}
