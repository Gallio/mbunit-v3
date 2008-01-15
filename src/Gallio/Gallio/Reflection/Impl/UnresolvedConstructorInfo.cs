using System;
using System.Globalization;
using System.Reflection;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// Represents a <see cref="ConstructorInfo" /> whose native definition could not be resolved
    /// so we fall back on the <see cref="IConstructorInfo"/> wrapper.
    /// </summary>
    public partial class UnresolvedConstructorInfo : ConstructorInfo
    {
        private readonly IConstructorInfo adapter;

        /// <summary>
        /// Creates a reflection object backed by the specified adapter.
        /// </summary>
        /// <param name="adapter">The adapter</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="adapter"/> is null</exception>
        public UnresolvedConstructorInfo(IConstructorInfo adapter)
        {
            if (adapter == null)
                throw new ArgumentNullException("adapter");

            this.adapter = adapter;
        }

        /// <inheritdoc />
        public override MemberTypes MemberType
        {
            get { return MemberTypes.Constructor; }
        }

        /// <inheritdoc />
        public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}