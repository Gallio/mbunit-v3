// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Linq.Expressions;
using Db4objects.Db4o.Linq;
using Gallio.Ambience.Impl;

namespace Gallio.Ambience
{
    /// <summary>
    /// Extension methods for LINQ syntax over Ambient data queries.
    /// </summary>
    public static class AmbientDataQueryExtensions
    {
        /// <summary>
        /// Counts the number of objects produced by the query.
        /// </summary>
        /// <typeparam name="TSource">The type of object being queried.</typeparam>
        /// <param name="self">The query.</param>
        /// <returns>The number of objects.</returns>
        public static int Count<TSource>(this IAmbientDataQuery<TSource> self)
        {
            var query = (Db4oAmbientDataQuery<TSource>)self;
            return query.Inner.Count();
        }

        /// <summary>
        /// Produces a new query to select a projection of a component of another query.
        /// </summary>
        /// <typeparam name="TSource">The type of object being queried.</typeparam>
        /// <typeparam name="TRet">The projection result type.</typeparam>
        /// <param name="self">The query.</param>
        /// <param name="selector">The selection expression.</param>
        /// <returns>The projected query.</returns>
        public static IAmbientDataQuery<TRet> Select<TSource, TRet>(this IAmbientDataQuery<TSource> self, System.Func<TSource, TRet> selector)
        {
            var query = (Db4oAmbientDataQuery<TSource>)self;
            return new Db4oAmbientDataQuery<TRet>(query.Inner.Select(selector));
        }

        /// <summary>
        /// Produces a new query to filter another query by a criteria.
        /// </summary>
        /// <typeparam name="TSource">The type of object being queried.</typeparam>
        /// <param name="self">The query.</param>
        /// <param name="expression">The filter expression.</param>
        /// <returns>The filtered query.</returns>
        public static IAmbientDataQuery<TSource> Where<TSource>(this IAmbientDataQuery<TSource> self, Expression<System.Func<TSource, bool>> expression)
        {
            var query = (Db4oAmbientDataQuery<TSource>)self;
            return new Db4oAmbientDataQuery<TSource>(query.Inner.Where(expression));
        }

        /// <summary>
        /// Produces a new query ordered by a comparison expression in ascending order.
        /// </summary>
        /// <typeparam name="TSource">The type of object being queried.</typeparam>
        /// <typeparam name="TKey">The sort key type.</typeparam>
        /// <param name="self">The query.</param>
        /// <param name="expression">The sort comparison expression.</param>
        /// <returns>The ordered query.</returns>
        public static IAmbientDataQuery<TSource> OrderBy<TSource, TKey>(this IAmbientDataQuery<TSource> self, Expression<System.Func<TSource, TKey>> expression)
        {
            var query = (Db4oAmbientDataQuery<TSource>)self;
            return new Db4oAmbientDataQuery<TSource>(query.Inner.OrderBy(expression));
        }

        /// <summary>
        /// Produces a new query ordered by a comparison expression in descending order.
        /// </summary>
        /// <typeparam name="TSource">The type of object being queried.</typeparam>
        /// <typeparam name="TKey">The sort key type.</typeparam>
        /// <param name="self">The query.</param>
        /// <param name="expression">The sort comparison expression.</param>
        /// <returns>The ordered query.</returns>
        public static IAmbientDataQuery<TSource> OrderByDescending<TSource, TKey>(this IAmbientDataQuery<TSource> self, Expression<System.Func<TSource, TKey>> expression)
        {
            var query = (Db4oAmbientDataQuery<TSource>)self;
            return new Db4oAmbientDataQuery<TSource>(query.Inner.OrderByDescending(expression));
        }

        /// <summary>
        /// Produces a new query ordered by an additional comparison expression in ascending order.
        /// </summary>
        /// <typeparam name="TSource">The type of object being queried.</typeparam>
        /// <typeparam name="TKey">The sort key type.</typeparam>
        /// <param name="self">The query.</param>
        /// <param name="expression">The sort comparison expression.</param>
        /// <returns>The ordered query.</returns>
        public static IAmbientDataQuery<TSource> ThenBy<TSource, TKey>(this IAmbientDataQuery<TSource> self, Expression<System.Func<TSource, TKey>> expression)
        {
            var query = (Db4oAmbientDataQuery<TSource>)self;
            return new Db4oAmbientDataQuery<TSource>(query.Inner.ThenBy(expression));
        }

        /// <summary>
        /// Produces a new query ordered by an additional comparison expression in descending order.
        /// </summary>
        /// <typeparam name="TSource">The type of object being queried.</typeparam>
        /// <typeparam name="TKey">The sort key type.</typeparam>
        /// <param name="self">The query.</param>
        /// <param name="expression">The sort comparison expression.</param>
        /// <returns>The ordered query.</returns>
        public static IAmbientDataQuery<TSource> ThenByDescending<TSource, TKey>(this IAmbientDataQuery<TSource> self, Expression<System.Func<TSource, TKey>> expression)
        {
            var query = (Db4oAmbientDataQuery<TSource>)self;
            return new Db4oAmbientDataQuery<TSource>(query.Inner.ThenByDescending(expression));
        }
    }
}
