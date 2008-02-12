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
using System.Text;
using Gallio.Collections;
using Gallio.Utilities;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> property wrapper.
    /// </summary>
    public sealed class StaticPropertyWrapper : StaticMemberWrapper, IPropertyInfo
    {
        private readonly Memoizer<PropertyAttributes> propertyAttributesMemoizer = new Memoizer<PropertyAttributes>();
        private readonly Memoizer<IList<IParameterInfo>> indexParametersMemoizer = new Memoizer<IList<IParameterInfo>>();

        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy</param>
        /// <param name="handle">The underlying reflection object</param>
        /// <param name="declaringType">The declaring type</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/>, <paramref name="handle"/>,
        /// or <paramref name="declaringType"/> is null</exception>
        public StaticPropertyWrapper(StaticReflectionPolicy policy, object handle, StaticDeclaredTypeWrapper declaringType)
            : base(policy, handle, declaringType)
        {
            if (declaringType == null)
                throw new ArgumentNullException("declaringType");
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
            get { return Policy.GetPropertyGetMethod(this); }
        }
        IMethodInfo IPropertyInfo.GetMethod
        {
            get { return GetMethod; }
        }

        /// <inheritdoc />
        public StaticMethodWrapper SetMethod
        {
            get { return Policy.GetPropertySetMethod(this); }
        }
        IMethodInfo IPropertyInfo.SetMethod
        {
            get { return SetMethod; }
        }

        /// <inheritdoc />
        public IList<IParameterInfo> IndexParameters
        {
            get
            {
                return indexParametersMemoizer.Memoize(delegate
                {
                    IMethodInfo getMethod = GetMethod;
                    if (getMethod != null)
                        return getMethod.Parameters;

                    IList<IParameterInfo> setterParameters = SetMethod.Parameters;
                    if (setterParameters.Count == 1)
                        return EmptyArray<IParameterInfo>.Instance;

                    IParameterInfo[] parameters = new IParameterInfo[setterParameters.Count - 1];
                    for (int i = 0; i < parameters.Length; i++)
                        parameters[i] = setterParameters[i];
                    return parameters;
                });
            }
        }

        /// <inheritdoc />
        public ITypeInfo ValueType
        {
            get { return Policy.GetPropertyType(this); }
        }

        /// <inheritdoc />
        public int Position
        {
            get { return 0; }
        }

        /// <summary>
        /// Gets the methods that this one overrides.
        /// Only includes overrides that appear on class types, not interfaces.
        /// </summary>
        public IEnumerable<StaticPropertyWrapper> GetOverrides()
        {
            StaticMethodWrapper discriminator = GetDiscriminatorMethod(this);
            if (!discriminator.IsOverride)
                yield break;

            foreach (StaticDeclaredTypeWrapper baseType in DeclaringType.GetAllBaseTypes())
            {
                foreach (StaticPropertyWrapper other in Policy.GetTypeProperties(baseType))
                {
                    StaticMethodWrapper otherDiscriminator = GetDiscriminatorMethod(other);
                    if (otherDiscriminator == null)
                        yield break;

                    if (discriminator.OverridesMethod(otherDiscriminator))
                    {
                        yield return other;

                        if (!otherDiscriminator.IsOverride)
                            yield break;
                        break;
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

            IList<IParameterInfo> indexParameters = IndexParameters;
            if (indexParameters.Count != 0)
            {
                sig.Append(' ');
                AppendParameterListToSignature(sig, indexParameters);
            }

            return sig.ToString();
        }

        /// <inheritdoc />
        protected override IEnumerable<ICodeElementInfo> GetInheritedElements()
        {
            foreach (StaticPropertyWrapper element in GetOverrides())
                yield return element;
        }
    }
}
