using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gallio.Ambience
{
    /// <summary>
    /// <para>
    /// Represents a container of Ambient data and providers operations to
    /// query, store and update its contents.
    /// </para>
    /// </summary>
    public interface IAmbientDataContainer
    {
        /// <summary>
        /// Gets all objects of a particular type in the container.
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <returns>The data set</returns>
        IList<T> Query<T>();

        /// <summary>
        /// Gets all objects of a particular type in the container that match a particular filtering criteria.
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="predicate">The filtering criteria</param>
        /// <returns>The data set</returns>
        IList<T> Query<T>(Predicate<T> predicate);

        /// <summary>
        /// <para>
        /// Deletes the object from the container.
        /// </para>
        /// </summary>
        /// <param name="obj">The object to delete</param>
        void Delete(object obj);

        /// <summary>
        /// <para>
        /// Stores or updates an object in the container.
        /// </para>
        /// </summary>
        /// <param name="obj">The object to store</param>
        void Store(object obj);
    }
}
