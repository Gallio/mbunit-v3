using System;
using System.Reflection;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// Represents a <see cref="MethodInfo" /> whose native definition could not be resolved
    /// so we fall back on the <see cref="IMethodInfo"/> wrapper.
    /// </summary>
    public partial class UnresolvedMethodInfo : MethodInfo
    {
        private readonly IMethodInfo adapter;

        /// <summary>
        /// Creates a reflection object backed by the specified adapter.
        /// </summary>
        /// <param name="adapter">The adapter</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="adapter"/> is null</exception>
        public UnresolvedMethodInfo(IMethodInfo adapter)
        {
            if (adapter == null)
                throw new ArgumentNullException("adapter");

            this.adapter = adapter;
        }

        /// <inheritdoc />
        public override MemberTypes MemberType
        {
            get { return MemberTypes.Method; }
        }

        /// <inheritdoc />
        public override ParameterInfo ReturnParameter
        {
            get { return adapter.ReturnParameter.Resolve(false); }
        }

        /// <inheritdoc />
        public override Type ReturnType
        {
            get { return adapter.ReturnType.Resolve(false); }
        }

        /// <inheritdoc />
        public override ICustomAttributeProvider ReturnTypeCustomAttributes
        {
            get { return ReturnParameter; }
        }

        /// <inheritdoc />
        public override MethodInfo GetBaseDefinition()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override MethodInfo GetGenericMethodDefinition()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override MethodInfo MakeGenericMethod(params Type[] typeArguments)
        {
            throw new NotImplementedException();
        }
    }
}