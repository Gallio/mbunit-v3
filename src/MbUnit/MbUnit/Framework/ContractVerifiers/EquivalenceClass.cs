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
    /// Contains a collection of equivalent object instances. 
    /// All the elements of the collection are supposed to be equal together.
    /// </para>
    /// <para>
    /// Equivalent classes are used by some contract verifiers such as 
    /// <see cref="VerifyEqualityContractAttribute"/> to check for correct
    /// implementation of object equality or comparison.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of equivalent instances.</typeparam>
    public class EquivalenceClass<T> : IEnumerable<T>
    {
        private readonly List<T> equivalentInstances;

        /// <summary>
        /// Constructs an class of equivalent instances.
        /// </summary>
        /// <param name="equivalentInstances">The type of equivalent instances.</param>
        public EquivalenceClass(params T[] equivalentInstances)
        {
            if (equivalentInstances == null)
            {
                if (default(T) != null)
                    throw new ArgumentNullException("equivalentInstances", "The instance cannot be null for a value type.");

                this.equivalentInstances = new List<T>(new T[] { default(T) });
            }
            else
            {
                if (equivalentInstances.Length == 0)
                {
                    throw new ArgumentException(String.Format("An equivalence class of type '{0}' must be initialized with at least one object.", 
                        typeof(T)), "equivalentInstances");
                }

                this.equivalentInstances = new List<T>(equivalentInstances);
            }
        }

        /// <summary>
        /// Gets the collection of equivalent instances.
        /// </summary>
        public IList<T> EquivalentInstances
        {
            get { return new ReadOnlyCollection<T>(equivalentInstances); }
        }

        /// <summary>
        /// Returns a strongly-typed enumerator through the collection.
        /// </summary>
        /// <returns>A strongly-typed enumerator.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return equivalentInstances.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
