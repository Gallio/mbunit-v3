using System.Collections.Generic;

namespace Gallio.Reflection
{
    /// <summary>
    /// Sorts code elements in various ways.
    /// </summary>
    public static class CodeElementSorter
    {
        /// <summary>
        /// Sorts members such that those declared by supertypes appear before those
        /// declared by subtypes.
        /// </summary>
        /// <typeparam name="T">The type of member</typeparam>
        /// <param name="members">The members to sort</param>
        /// <returns>The sorted members</returns>
        /// <seealso cref="DeclaringTypeComparer{T}"/>
        public static IList<T> SortMembersByDeclaringType<T>(IEnumerable<T> members)
            where T : IMemberInfo
        {
            List<T> sortedMembers = new List<T>(members);
            sortedMembers.Sort(DeclaringTypeComparer<T>.Instance);
            return sortedMembers;
        }
    }
}
