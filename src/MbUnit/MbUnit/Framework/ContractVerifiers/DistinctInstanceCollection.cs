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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// A collection of distinct object instances. Every element represents a valid instance
    /// which is different from all the other elements in the collection (object equality).
    /// </summary>
    /// <remarks>
    /// <para>
    /// Distinct instances are used by some contract verifiers such as 
    /// <see cref="CollectionContract{TCollection, TItem}"/> to check for the correct 
    /// implementation of the collection interface.
    /// </para>
    /// <para>
    /// Use the default constructor followed by a list initializer.
    /// <example>
    /// <code><![CDATA[
    /// var collection = new DistinctInstanceCollection<Foo>
    /// {
    ///     new Foo(1)
    ///     new Foo(2),
    ///     new Foo(3),
    /// };
    /// ]]></code>
    /// </example>
    /// </para>
    /// <para>
    /// Or, use the single-parameter constructor to create a collection from an pre-existing enumeration.
    /// <example>
    /// <code><![CDATA[
    /// var collection = new DistinctInstanceCollection<Foo>(Foo.GetThemAll());
    /// ]]></code>
    /// </example>
    /// </para>
    /// </remarks>
    /// <typeparam name="T">The type of the object instances in the collection.</typeparam>
    public class DistinctInstanceCollection<T> : IEnumerable<T>
    {
        private readonly List<T> distinctInstances;

        /// <summary>
        /// Constructs an empty collection of distinct object instances.
        /// </summary>
        /// <remarks>
        /// <para>
        /// All the elements of the collection should be different (object equality).
        /// </para>
        /// </remarks>
        public DistinctInstanceCollection()
        {
            distinctInstances = new List<T>();
        }

        /// <summary>
        /// Constructs acollection of distinct object instances from the specified
        /// enumeration of elements.
        /// </summary>
        /// <param name="distinctInstances">An enumeration of distinct object instances.</param>
        public DistinctInstanceCollection(IEnumerable<T> distinctInstances)
        {
            if (distinctInstances == null)
            {
                throw new ArgumentNullException("distinctInstances");
            }

            foreach (T instance in distinctInstances)
            {
                if (instance == null)
                {
                    throw new ArgumentException(String.Format("An collection of distinct object instance of type " +
                        "'{0}' cannot contain a null reference instance.", typeof(T)), "distinctInstances");
                }
            }

            this.distinctInstances = new List<T>(distinctInstances);
        }

        /// <summary>
        /// Adds the specified instance to the collection.
        /// </summary>
        /// <param name="instance">A valid and non-null object instance.</param>
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
        /// Gets all the instances.
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
        /// <returns>A strongly-typed enumerator.</returns>
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
