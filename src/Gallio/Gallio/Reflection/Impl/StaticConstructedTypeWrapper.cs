using System;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> constructed type wrapper.
    /// </summary>
    /// <seealso cref="StaticArrayTypeWrapper"/>
    /// <seealso cref="StaticByRefTypeWrapper"/>
    /// <seealso cref="StaticPointerTypeWrapper"/>
    public abstract class StaticConstructedTypeWrapper : StaticDelegatingTypeWrapper
    {
        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy</param>
        /// <param name="elementType">The element type</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/> or <paramref name="elementType" /> is null</exception>
        public StaticConstructedTypeWrapper(StaticReflectionPolicy policy, StaticTypeWrapper elementType)
            : base(policy, elementType, null)
        {
        }

        /// <inheritdoc />
        public override StaticTypeWrapper ElementType
        {
            get { return (StaticTypeWrapper)Handle; }
        }

        /// <inheritdoc />
        public override bool ContainsGenericParameters
        {
            get { return ElementType.ContainsGenericParameters; }
        }

        /// <inheritdoc />
        public override IAssemblyInfo Assembly
        {
            get { return ElementType.Assembly; }
        }

        /// <inheritdoc />
        public override INamespaceInfo Namespace
        {
            get { return ElementType.Namespace; }
        }

        /// <inheritdoc />
        public override string Name
        {
            get { return ElementType.Name + NameSuffix; }
        }

        /// <inheritdoc />
        public override string FullName
        {
            get { return ElementType.FullName + NameSuffix; }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ElementType + NameSuffix;
        }

        /// <summary>
        /// Gets the suffix to append to the name of the constructed type.
        /// </summary>
        protected abstract string NameSuffix { get; }
    }
}
