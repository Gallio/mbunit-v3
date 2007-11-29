using System.Collections.Generic;

namespace Gallio.Model.Reflection
{
    /// <summary>
    /// Sorts an members that all belong to the same type
    /// such that the members declared by supertypes appear before those
    /// declared by subtypes.
    /// </summary>
    /// <example>
    /// If type A derives from types B and C then given methods
    /// A.Foo, A.Bar, B.Foo, C.Quux one possible sorted order will be:
    /// B.Foo, C.Quux, A.Bar, A.Foo.  The members are not sorted by name or
    /// by any other criterion except by relative specificity of the
    /// declaring types.
    /// </example>
    public sealed class DeclaringTypeComparer<T> : IComparer<T>
        where T : IMemberInfo
    {
        /// <summary>
        /// Gets the singleton instance of the comparer.
        /// </summary>
        public static readonly DeclaringTypeComparer<T> Instance = new DeclaringTypeComparer<T>();

        private DeclaringTypeComparer()
        {
        }

        /// <inheritdoc />
        public int Compare(T x, T y)
        {
            ITypeInfo tx = x.DeclaringType, ty = y.DeclaringType;
            if (tx.IsAssignableFrom(ty))
                return -1;
            if (ty.IsAssignableFrom(tx))
                return 1;

            return 0;
        }
    }
}
