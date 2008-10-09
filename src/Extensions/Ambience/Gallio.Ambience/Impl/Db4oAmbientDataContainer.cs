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
using Db4objects.Db4o;

namespace Gallio.Ambience.Impl
{
    /// <summary>
    /// Facade over <see cref="IObjectContainer" />.
    /// </summary>
    internal class Db4oAmbientDataContainer : IAmbientDataContainer
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
        public IList<T> Query<T>()
        {
            return inner.Query<T>();
        }

        /// <inheritdoc />
        public IList<T> Query<T>(Predicate<T> predicate)
        {
            return inner.Query<T>(predicate);
        }

        /// <inheritdoc />
        public void Delete(object obj)
        {
            inner.Delete(obj);
        }

        /// <inheritdoc />
        public void Store(object obj)
        {
            inner.Store(obj);
        }

        /// <inheritdoc />
        public void Purge()
        {
            inner.Ext().Purge();
        }
    }
}
