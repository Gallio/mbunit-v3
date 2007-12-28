using System;

namespace Gallio.Model.Filters
{
    /// <summary>
    /// Visits the structure of filters.
    /// </summary>
    public interface IFilterVisitor
    {
        /// <summary>
        /// Visits a <see cref="AnyFilter{T}" />.
        /// </summary>
        /// <param name="filter">The filter</param>
        void VisitAnyFilter<T>(AnyFilter<T> filter);

        /// <summary>
        /// Visits a <see cref="NoneFilter{T}" />.
        /// </summary>
        /// <param name="filter">The filter</param>
        void VisitNoneFilter<T>(NoneFilter<T> filter);

        /// <summary>
        /// Visits a <see cref="AndFilter{T}" />.
        /// </summary>
        /// <param name="filter">The filter</param>
        void VisitAndFilter<T>(AndFilter<T> filter);

        /// <summary>
        /// Visits a <see cref="OrFilter{T}" />.
        /// </summary>
        /// <param name="filter">The filter</param>
        void VisitOrFilter<T>(OrFilter<T> filter);

        /// <summary>
        /// Visits a <see cref="NotFilter{T}" />.
        /// </summary>
        /// <param name="filter">The filter</param>
        void VisitNotFilter<T>(NotFilter<T> filter);

        /// <summary>
        /// Visits a <see cref="PropertyFilter{T}" />.
        /// </summary>
        /// <param name="filter">The filter</param>
        void VisitPropertyFilter<T>(PropertyFilter<T> filter);

        /// <summary>
        /// Visits a <see cref="EqualityFilter{T}" />.
        /// </summary>
        /// <param name="filter">The filter</param>
        void VisitEqualityFilter<T>(EqualityFilter<T> filter)
            where T : class, IEquatable<T>;

        /// <summary>
        /// Visits a <see cref="RegexFilter" />.
        /// </summary>
        /// <param name="filter">The filter</param>
        void VisitRegexFilter(RegexFilter filter);
    }
}
