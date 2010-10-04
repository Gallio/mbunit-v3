// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
    /// Contains a collection of equivalent object instances. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// All the elements of the collection should be equal together, in
    /// the sense of the local implementation of an equality contract
    /// (<see cref="IEquatable{T}"/>) or a comparison contract 
    /// (<see cref="IComparable{T}"/>).
    /// </para>
    /// <para>
    /// Equivalence classes are used by some contract verifiers such as 
    /// <see cref="EqualityContract{TTarget}"/> to check for 
    /// the correct implementation of object equality or comparison.
    /// </para>
    /// </remarks>
    public class EquivalenceClass : IEnumerable
    {
        private readonly IList<object> equivalentInstances;

        /// <summary>
        /// Constructs a class of equivalent instances.
        /// </summary>
        /// <remarks>
        /// <para>
        /// All the elements of the collection should be equal together, in
        /// the sense of the local implementation of an equality contract
        /// (<see cref="IEquatable{T}"/>) or a comparison contract 
        /// (<see cref="IComparable{T}"/>).
        /// </para>
        /// </remarks>
        /// <param name="equivalentInstances">The type of equivalent object instances.</param>
        public EquivalenceClass(params object[] equivalentInstances)
        {
            if (equivalentInstances == null)
                throw new ArgumentNullException("equivalentInstances", "An equivalence class cannot be initialized with a null reference.");
            if (equivalentInstances.Length == 0)
                throw new ArgumentException("An equivalence class must be initialized with at least one instance.", "equivalentInstances");

            foreach (object item in equivalentInstances)
            {
                if (ReferenceEquals(item, null))
                    throw new ArgumentException("An equivalence class cannot be initialized with a collection of instances containing a null reference.", "equivalentInstances");
            }

            this.equivalentInstances = new List<object>(equivalentInstances);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An enumerator object that can be used to iterate through the collection.</returns>
        public IEnumerator GetEnumerator()
        {
            return equivalentInstances.GetEnumerator();
        }
    }
}
