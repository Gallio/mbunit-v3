using System;
using System.Collections.Generic;
using System.Reflection;
using Gallio.Utilities;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> attribute wrapper.
    /// </summary>
    public sealed class StaticAttributeWrapper : StaticWrapper, IAttributeInfo
    {
        private readonly Memoizer<IConstructorInfo> constructorMemoizer = new Memoizer<IConstructorInfo>();
        private readonly Memoizer<object> resolveMemoizer = new Memoizer<object>();

        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy</param>
        /// <param name="handle">The underlying reflection object</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/> or <paramref name="handle"/> is null</exception>
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
                return constructorMemoizer.Memoize(delegate
                {
                    return Policy.GetAttributeConstructor(this);
                });
            }
        }

        /// <inheritdoc />
        public object GetFieldValue(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            foreach (KeyValuePair<StaticFieldWrapper, object> entry in Policy.GetAttributeFieldArguments(this))
                if (entry.Key.Name == name)
                    return entry.Value;

            IFieldInfo field = Type.GetField(name, BindingFlags.Public | BindingFlags.Instance);
            if (field != null && ReflectorAttributeUtils.IsAttributeField(field))
                return ReflectionUtils.GetDefaultValue(field.ValueType.TypeCode);

            throw new ArgumentException(String.Format("The attribute does not have a writable instance field named '{0}'.", name));
        }

        /// <inheritdoc />
        public object GetPropertyValue(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            foreach (KeyValuePair<StaticPropertyWrapper, object> entry in Policy.GetAttributePropertyArguments(this))
                if (entry.Key.Name == name)
                    return entry.Value;

            IPropertyInfo property = Type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (property != null && ReflectorAttributeUtils.IsAttributeProperty(property))
                return ReflectionUtils.GetDefaultValue(property.ValueType.TypeCode);

            throw new ArgumentException(String.Format("The attribute does not have a writable instance property named '{0}'.", name));
        }

        /// <inheritdoc />
        public object[] InitializedArgumentValues
        {
            get { return Policy.GetAttributeConstructorArguments(this); }
        }

        /// <inheritdoc />
        public IDictionary<IFieldInfo, object> InitializedFieldValues
        {
            get
            {
                Dictionary<IFieldInfo, object> result = new Dictionary<IFieldInfo, object>();
                foreach (KeyValuePair<StaticFieldWrapper, object> entry in Policy.GetAttributeFieldArguments(this))
                    result.Add(entry.Key, entry.Value);
                return result;
            }
        }

        /// <inheritdoc />
        public IDictionary<IPropertyInfo, object> InitializedPropertyValues
        {
            get
            {
                Dictionary<IPropertyInfo, object> result = new Dictionary<IPropertyInfo, object>();
                foreach (KeyValuePair<StaticPropertyWrapper, object> entry in Policy.GetAttributePropertyArguments(this))
                    result.Add(entry.Key, entry.Value);
                return result;
            }
        }

        /// <inheritdoc />
        public object Resolve()
        {
            return resolveMemoizer.Memoize(delegate
            {
                return ReflectorAttributeUtils.CreateAttribute(this);
            });
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return String.Format("Attribute of type '{0}'", Type);
        }
    }
}
