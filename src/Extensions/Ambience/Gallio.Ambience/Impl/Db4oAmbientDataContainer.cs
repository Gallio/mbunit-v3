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
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;

namespace Gallio.Ambience.Impl
{
    /// <summary>
    /// Facade over <see cref="IObjectContainer" />.
    /// </summary>
    internal sealed class Db4oAmbientDataContainer : IAmbientDataContainer
    {
        private readonly IObjectContainer inner;

        /// <summary>
        /// Creates a wrapper for a Db4o object container.
        /// </summary>
        /// <param name="inner">The inner container</param>
        public Db4oAmbientDataContainer(IObjectContainer inner)
        {
            if (inner == null)
                throw new ArgumentNullException("inner");

            this.inner = inner;
        }

        /// <summary>
        /// Gets the Db4o object container.
        /// </summary>
        public IObjectContainer Inner
        {
            get { return inner; }
        }

        /// <inheritdoc />
        public IAmbientDataSet<T> Query<T>()
        {
            try
            {
                return new Db4oListWrapper<T>(inner.Query<T>());
            }
            catch (Db4oException ex)
            {
                throw new AmbienceException("An error occurred while executing a database query.", ex);
            }
        }

        /// <inheritdoc />
        public IAmbientDataSet<T> Query<T>(Predicate<T> predicate)
        {
            try
            {
                return new Db4oListWrapper<T>(inner.Query<T>(predicate));
            }
            catch (Db4oException ex)
            {
                throw new AmbienceException("An error occurred while executing a database query.", ex);
            }
        }

        /// <inheritdoc />
        public void Delete(object obj)
        {
            try
            {
                inner.Delete(obj);
            }
            catch (Db4oException ex)
            {
                throw new AmbienceException("An error occurred while deleting an object from the database.", ex);
            }
        }

        /// <inheritdoc />
        public void Store(object obj)
        {
            try
            {
                inner.Store(obj);
            }
            catch (Db4oException ex)
            {
                throw new AmbienceException("An error occurred while storing an object in the database.", ex);
            }
        }

        /// <inheritdoc />
        public void DeleteAll()
        {
            try
            {
                foreach (object obj in inner.Query<object>())
                    inner.Delete(obj);
            }
            catch (Db4oException ex)
            {
                throw new AmbienceException("An error occurred while purging the database.", ex);
            }
        }
    }
}
