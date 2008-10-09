// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

        /// <summary>
        /// <para>
        /// Purges the entire contents of the container.  (Use with caution!)
        /// </para>
        /// </summary>
        void Purge();
    }
}
