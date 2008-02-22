using System;

namespace Gallio.Model.Filters
{
    /// <summary>
    /// A filter that matches objects whose <see cref="ITestComponent.Name" />
    /// matches the specified name filter.
    /// </summary>
    [Serializable]
    public class NameFilter<T> : PropertyFilter<T> where T : ITestComponent
    {
        /// <summary>
        /// Creates a name filter.
        /// </summary>
        /// <param name="nameFilter">A filter for the name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="nameFilter"/> is null</exception>
        public NameFilter(Filter<string> nameFilter)
            : base(nameFilter)
        {
        }

        /// <inheritdoc />
        public override string Key
        {
            get { return @"Name"; }
        }

        /// <inheritdoc />
        public override bool IsMatch(T value)
        {
            return ValueFilter.IsMatch(value.Name);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return @"Name(" + ValueFilter + @")";
        }
    }
}