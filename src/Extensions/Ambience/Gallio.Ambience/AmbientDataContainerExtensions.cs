using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Db4objects.Db4o.Linq;
using Gallio.Ambience.Impl;

namespace Gallio.Ambience
{
    /// <summary>
    /// <para>
    /// Extension methods for LINQ syntax over Ambient data containers.
    /// </para>
    /// </summary>
    public static class AmbientDataContainerExtensions
    {
        /// <summary>
        /// <para>
        /// Obtains a query over a data container.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Client code will not call this method directly.  However, it turns out that
        /// the C# compiler will add an implicit call to this method when it attempts to
        /// select a value from the container using LINQ syntax.
        /// </para>
        /// </remarks>
        /// <typeparam name="T">The result type</typeparam>
        /// <param name="container">The container</param>
        /// <returns>The query object</returns>
        public static IAmbientDataQuery<T> Cast<T>(this IAmbientDataContainer container)
        {
            var db4oContainer = (Db4oAmbientDataContainer)container;
            return new Db4oAmbientDataQuery<T>(db4oContainer.Inner.Cast<T>());
        }
    }
}
