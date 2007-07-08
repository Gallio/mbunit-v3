using System.Collections;
using MbUnit.Framework.Core.Attributes;

namespace MbUnit.Framework.Core.Attributes
{
    /// <summary>
    /// Sorts decorator pattern attributes in ascending order by <see cref="DecoratorPatternAttribute.Order" />.
    /// </summary>
    public sealed class DecoratorOrderComparer : IComparer
    {
        /// <summary>
        /// Gets the singleton instance of the comparer.
        /// </summary>
        public static readonly DecoratorOrderComparer Instance = new DecoratorOrderComparer();

        private DecoratorOrderComparer()
        {
        }

        /// <inheritdoc />
        public int Compare(object x, object y)
        {
            DecoratorPatternAttribute xAttrib = (DecoratorPatternAttribute)x;
            DecoratorPatternAttribute yAttrib = (DecoratorPatternAttribute)y;

            return xAttrib.Order.CompareTo(yAttrib.Order);
        }
    }
}