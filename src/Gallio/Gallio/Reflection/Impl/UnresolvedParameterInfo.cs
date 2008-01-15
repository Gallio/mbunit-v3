using System;
using System.Reflection;
using Gallio.Collections;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// Represents a <see cref="ParameterInfo" /> whose native definition could not be resolved
    /// so we fall back on the <see cref="IParameterInfo"/> wrapper.
    /// </summary>
    public class UnresolvedParameterInfo : ParameterInfo
    {
        private readonly IParameterInfo adapter;

        /// <summary>
        /// Creates a reflection object backed by the specified adapter.
        /// </summary>
        /// <param name="adapter">The adapter</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="adapter"/> is null</exception>
        public UnresolvedParameterInfo(IParameterInfo adapter)
        {
            if (adapter == null)
                throw new ArgumentNullException("adapter");

            this.adapter = adapter;
        }

        /// <inheritdoc />
        public override ParameterAttributes Attributes
        {
            get { return adapter.ParameterAttributes; }
        }

        /// <inheritdoc />
        public override object DefaultValue
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override MemberInfo Member
        {
            get { return adapter.Member.Resolve(false); }
        }

        /// <inheritdoc />
        public override string Name
        {
            get { return adapter.Name; }
        }

        /// <inheritdoc />
        public override Type ParameterType
        {
            get { return adapter.ValueType.Resolve(false); }
        }

        /// <inheritdoc />
        public override int Position
        {
            get { return adapter.Position; }
        }

        /// <inheritdoc />
        public override object RawDefaultValue
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override bool Equals(object o)
        {
            UnresolvedParameterInfo other = o as UnresolvedParameterInfo;
            return other != null && adapter.Equals(other.adapter);
        }

        /// <inheritdoc />
        public override object[] GetCustomAttributes(bool inherit)
        {
            return GenericUtils.ToArray(adapter.GetAttributes(inherit));
        }

        /// <inheritdoc />
        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return GenericUtils.ToArray(adapter.GetAttributes(attributeType, inherit));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return adapter.GetHashCode();
        }

        /// <inheritdoc />
        public override Type[] GetOptionalCustomModifiers()
        {
            return EmptyArray<Type>.Instance;
        }

        /// <inheritdoc />
        public override Type[] GetRequiredCustomModifiers()
        {
            return EmptyArray<Type>.Instance;
        }

        /// <inheritdoc />
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return adapter.HasAttribute(attributeType, inherit);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return adapter.ToString();
        }
    }
}