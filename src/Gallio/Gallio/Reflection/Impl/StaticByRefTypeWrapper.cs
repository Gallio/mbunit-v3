using System;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> by-ref type wrapper.
    /// </summary>
    public sealed class StaticByRefTypeWrapper : StaticConstructedTypeWrapper
    {
        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy</param>
        /// <param name="elementType">The element type</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/> or <paramref name="elementType" /> is null</exception>
        public StaticByRefTypeWrapper(StaticReflectionPolicy policy, StaticTypeWrapper elementType)
            : base(policy, elementType)
        {
        }

        /// <inheritdoc />
        public override bool IsByRef
        {
            get { return true; }
        }

        /// <inheritdoc />
        protected internal override ITypeInfo ApplySubstitution(StaticTypeSubstitution substitution)
        {
            return ElementType.ApplySubstitution(substitution).MakeByRefType();
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            StaticByRefTypeWrapper other = obj as StaticByRefTypeWrapper;
            return other != null && ElementType.Equals(other.ElementType);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return ElementType.GetHashCode() ^ GetType().GetHashCode();
        }

        /// <inheritdoc />
        protected override ITypeInfo EffectiveType
        {
            get { return Reflector.Wrap(typeof(TypedReference)); }
        }

        /// <inheritdoc />
        protected override string NameSuffix
        {
            get { return @"&"; }
        }
    }
}
