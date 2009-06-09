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
    /// A collection of equivalence classes. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// Equivalent classes are used by some contract verifiers such as 
    /// <see cref="EqualityContract{TTarget}"/> and <see cref="ComparisonContract{TTarget}"/> 
    /// to check for the correct implementation of object equality or comparison.
    /// </para>
    /// <para>
    /// Use the default constructor followed by a list initializer to create a 
    /// collection of equivalence classes which contains a variable number of object instances.
    /// <code><![CDATA[
    /// var collection = new EquivalenceClassCollection<Foo>
    /// {
    ///     { new Foo(1), new Foo("One") },
    ///     { new Foo(2), new Foo("Two") },
    ///     { new Foo(3), new Foo("Three") }
    /// };
    /// ]]></code>
    /// </para>
    /// <para>
    /// Use the single-parameter constructor to create a collection of equivalence classes 
    /// which contains one single object instance each.
    /// <code><![CDATA[
    /// var collection = new EquivalenceClassCollection<Foo>(new Foo(1), new Foo(2), new Foo(3));
    /// ]]></code>
    /// </para>
    /// </remarks>
    /// <typeparam name="T">The type of equivalent object instances.</typeparam>
    public class EquivalenceClassCollection<T> : IEnumerable<EquivalenceClass<T>>
    {
        private readonly List<EquivalenceClass<T>> equivalenceClasses;

        /// <summary>
        /// Constructs an empty collection of equivalence classes.
        /// </summary>
        public EquivalenceClassCollection()
        {
            equivalenceClasses = new List<EquivalenceClass<T>>();
        }

        /// <summary>
        /// Constructs a collection of equivalence classes from the specified distinct object instances.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The resulting collection contains as many equivalence class as provided instances. Each equivalence
        /// class contains one single object. To construct a collection with equivalence classes containing
        /// several equivalent instances, use preferably the default constructor followed by a list initializer.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code><![CDATA[
        /// var collection = new EquivalenceClassCollection<Foo>
        /// {
        ///     { new Foo(1), new Foo("One") },
        ///     { new Foo(2), new Foo("Two") },
        ///     { new Foo(3), new Foo("Three") }
        /// };
        /// ]]></code>
        /// </example>
        /// <param name="distinctInstances">An enumeration of distinct instances.</param>
        public EquivalenceClassCollection(IEnumerable<T> distinctInstances)
            : this()
        {
            if (distinctInstances == null)
            {
                throw new ArgumentNullException("distinctInstances");
            }

            foreach (T instance in distinctInstances)
            {
                Add(instance);
            }
        }

        /// <summary>
        /// Adds to the collection a new equivalence class which contains the specified objects.
        /// </summary>
        /// <param name="equivalentInstances">An array of equivalent instances.</param>
        public void Add(params T[] equivalentInstances)
        {
            if (equivalentInstances == null)
            {
                throw new ArgumentNullException("equivalentInstances", "A collection of equivalence classes cannot contain a null reference class.");
            }

            foreach (var instance in equivalentInstances)
            {
                if (instance == null)
                {
                    throw new ArgumentException(String.Format("An equivalence class of type '{0}' cannot contain a null reference instance.", typeof(T)), "equivalentInstances");
                }
            }

            equivalenceClasses.Add(new EquivalenceClass<T>(equivalentInstances));
        }

        /// <summary>
        /// Gets the equivalence classes.
        /// </summary>
        public IList<EquivalenceClass<T>> EquivalenceClasses
        {
            get { return new ReadOnlyCollection<EquivalenceClass<T>>(equivalenceClasses); }
        }

        /// <summary>
        /// Returns a strongly-typed enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A strongly-typed enumerator.</returns>
        public IEnumerator<EquivalenceClass<T>> GetEnumerator()
        {
            return equivalenceClasses.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
