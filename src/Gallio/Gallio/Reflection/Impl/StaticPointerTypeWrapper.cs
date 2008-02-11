using System;
using System.Reflection;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> pointer type wrapper.
    /// </summary>
    public sealed class StaticPointerTypeWrapper : StaticConstructedTypeWrapper
    {
        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy</param>
        /// <param name="elementType">The element type</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/> or <paramref name="elementType" /> is null</exception>
        public StaticPointerTypeWrapper(StaticReflectionPolicy policy, StaticTypeWrapper elementType)
            : base(policy, elementType)
        {
        }

        /// <inheritdoc />
        public override bool IsPointer
        {
            get { return true; }
        }

        /// <inheritdoc />
        protected internal override ITypeInfo ApplySubstitution(StaticTypeSubstitution substitution)
        {
            return ElementType.ApplySubstitution(substitution).MakePointerType();
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            StaticPointerTypeWrapper other = obj as StaticPointerTypeWrapper;
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
            get { return Reflector.Wrap(typeof(Pointer)); }
        }

        /// <inheritdoc />
        protected override string NameSuffix
        {
            get { return @"*"; }
        }
    }
}
