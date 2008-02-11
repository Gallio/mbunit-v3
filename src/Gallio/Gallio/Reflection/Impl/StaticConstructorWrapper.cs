using System;
using System.Reflection;
using Gallio.Collections;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> constructor wrapper.
    /// </summary>
    public sealed class StaticConstructorWrapper : StaticFunctionWrapper, IConstructorInfo
    {
        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy</param>
        /// <param name="handle">The underlying reflection object</param>
        /// <param name="declaringType">The declaring type</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/>, <paramref name="handle"/>,
        /// or <paramref name="declaringType"/> is null</exception>
        public StaticConstructorWrapper(StaticReflectionPolicy policy, object handle, StaticDeclaredTypeWrapper declaringType)
            : base(policy, handle, declaringType)
        {
            if (declaringType == null)
                throw new ArgumentNullException("declaringType");
        }

        /// <inheritdoc />
        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Constructor; }
        }

        /// <inheritdoc />
        public bool Equals(IConstructorInfo other)
        {
            return Equals((object)other);
        }

        /// <inheritdoc />
        public ConstructorInfo Resolve(bool throwOnError)
        {
            return ReflectorResolveUtils.ResolveConstructor(this, throwOnError);
        }

        /// <inheritdoc />
        protected override MethodBase ResolveMethodBase(bool throwOnError)
        {
            return Resolve(throwOnError);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ToString(Reflector.Wrap(typeof(void)), EmptyArray<ITypeInfo>.Instance);
        }
    }
}
