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
    /// A collection of equivalence classes. 
    /// </para>
    /// <para>
    /// Equivalent classes are used by some contract verifiers such as 
    /// <see cref="EqualityContract{TTarget}"/>  to check for 
    /// the correct implementation of object equality or comparison.
    /// </para>
    /// <para>
    /// Use the list initializer to create a collection of equivalence classes which contain a variable 
    /// number of object instances.
    /// <example>
    /// <code><![CDATA[
    /// var collection = new EquivalenceClassCollection<Foo>
    /// {
    ///     { new Foo(7, 2) },
    ///     { new Foo(25, 2), new Foo(10, 5) },
    ///     { new Foo(3, 4), new Foo(2, 6), new Foo(1, 12) }
    /// };
    /// ]]></code>
    /// </example>
    /// </para>
    /// </summary>
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
        /// <returns>A strongly-typed enumerator</returns>
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
