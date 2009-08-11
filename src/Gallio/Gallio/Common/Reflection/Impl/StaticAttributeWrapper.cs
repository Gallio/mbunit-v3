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
using Gallio.Common;

namespace Gallio.Common.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> attribute wrapper.
    /// </summary>
    public sealed class StaticAttributeWrapper : StaticWrapper, IAttributeInfo
    {
        private Memoizer<IConstructorInfo> constructorMemoizer = new Memoizer<IConstructorInfo>();
        private KeyedMemoizer<bool, object> resolveMemoizer = new KeyedMemoizer<bool, object>();
        private Memoizer<IEnumerable<KeyValuePair<StaticFieldWrapper, ConstantValue>>> fieldArgumentsMemoizer
            = new Memoizer<IEnumerable<KeyValuePair<StaticFieldWrapper, ConstantValue>>>();
        private Memoizer<IEnumerable<KeyValuePair<StaticPropertyWrapper, ConstantValue>>> propertyArgumentsMemoizer
            = new Memoizer<IEnumerable<KeyValuePair<StaticPropertyWrapper, ConstantValue>>>();

        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy.</param>
        /// <param name="handle">The underlying reflection object.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/> or <paramref name="handle"/> is null.</exception>
        public StaticAttributeWrapper(StaticReflectionPolicy policy, object handle)
            : base(policy, handle)
        {
        }

        /// <inheritdoc />
        public ITypeInfo Type
        {
            get { return Constructor.DeclaringType; }
        }

        /// <inheritdoc />
        public IConstructorInfo Constructor
        {
            get
            {
                return constructorMemoizer.Memoize(() => ReflectionPolicy.GetAttributeConstructor(this));
            }
        }

        /// <inheritdoc />
        public ConstantValue GetFieldValue(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            foreach (KeyValuePair<StaticFieldWrapper, ConstantValue> entry in FieldArguments)
                if (entry.Key.Name == name)
                    return entry.Value;

            IFieldInfo field = Type.GetField(name, BindingFlags.Public | BindingFlags.Instance);
            if (field != null && ReflectorAttributeUtils.IsAttributeField(field))
                return ConstantValue.FromNative(ReflectionUtils.GetDefaultValue(field.ValueType.TypeCode));

            throw new ArgumentException(String.Format("The attribute does not have a writable instance field named '{0}'.", name));
        }

        /// <inheritdoc />
        public ConstantValue GetPropertyValue(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            foreach (KeyValuePair<StaticPropertyWrapper, ConstantValue> entry in PropertyArguments)
                if (entry.Key.Name == name)
                    return entry.Value;

            IPropertyInfo property = Type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (property != null && ReflectorAttributeUtils.IsAttributeProperty(property))
                return ConstantValue.FromNative(ReflectionUtils.GetDefaultValue(property.ValueType.TypeCode));

            throw new ArgumentException(String.Format("The attribute does not have a writable instance property named '{0}'.", name));
        }

        /// <inheritdoc />
        public ConstantValue[] InitializedArgumentValues
        {
            get { return ReflectionPolicy.GetAttributeConstructorArguments(this); }
        }

        /// <inheritdoc />
        public IEnumerable<KeyValuePair<IFieldInfo, ConstantValue>> InitializedFieldValues
        {
            get
            {
                foreach (KeyValuePair<StaticFieldWrapper, ConstantValue> entry in FieldArguments)
                    yield return new KeyValuePair<IFieldInfo, ConstantValue>(entry.Key, entry.Value);
            }
        }

        /// <inheritdoc />
        public IEnumerable<KeyValuePair<IPropertyInfo, ConstantValue>> InitializedPropertyValues
        {
            get
            {
                foreach (KeyValuePair<StaticPropertyWrapper, ConstantValue> entry in PropertyArguments)
                    yield return new KeyValuePair<IPropertyInfo, ConstantValue>(entry.Key, entry.Value);
            }
        }

        private IEnumerable<KeyValuePair<StaticFieldWrapper, ConstantValue>> FieldArguments
        {
            get
            {
                return fieldArgumentsMemoizer.Memoize(() =>
                    new List<KeyValuePair<StaticFieldWrapper, ConstantValue>>(ReflectionPolicy.GetAttributeFieldArguments(this)));
            }
        }

        private IEnumerable<KeyValuePair<StaticPropertyWrapper, ConstantValue>> PropertyArguments
        {
            get
            {
                return propertyArgumentsMemoizer.Memoize(() =>
                    new List<KeyValuePair<StaticPropertyWrapper, ConstantValue>>(ReflectionPolicy.GetAttributePropertyArguments(this)));
            }
        }

        /// <inheritdoc />
        public object Resolve(bool throwOnError)
        {
            return resolveMemoizer.Memoize(throwOnError, () =>
                ReflectorAttributeUtils.CreateAttribute(this, throwOnError));
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return String.Format("Attribute of type '{0}'", Type);
        }
    }
}
