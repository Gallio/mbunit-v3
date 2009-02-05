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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// <para>
    /// Contains a collection of distinct object instances. 
    /// All the elements of the collection should be referentially different.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of object instances in the collection.</typeparam>
    public class DistinctInstanceCollection<T> : IEnumerable<T>
    {
        private readonly List<T> distinctInstances;

        /// <summary>
        /// <para>
        /// Constructs a collection of distinct object instances.
        /// All the elements of the collection should be referentially different.
        /// </para>
        /// </summary>
        public DistinctInstanceCollection()
        {
            distinctInstances = new List<T>();
        }

        /// <summary>
        /// Adds the specified instance to the collection.
        /// </summary>
        /// <param name="instance"></param>
        public void Add(T instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            foreach (T item in distinctInstances)
            {
                if (item.Equals(instance))
                {
                    throw new ArgumentException("The collection already contains an similar instance.", "instance");
                }
            }

            distinctInstances.Add(instance);
        }

        /// <summary>
        /// Gets the collection of equivalent instances.
        /// </summary>
        public IList<T> Instances
        {
            get
            {
                return new ReadOnlyCollection<T>(distinctInstances);
            }
        }

        /// <summary>
        /// Returns a strongly-typed enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A strongly-typed enumerator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return distinctInstances.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
