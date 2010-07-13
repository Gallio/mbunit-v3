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
using System.Reflection;
using System.Text;

namespace Gallio.Common.Collections
{
    /// <summary>
    /// Encapsulates various strategies for counting the number of elements in a collection, an array, or
    /// a simple enumerable type.
    /// </summary>
    public class EnumerableCounter
    {
        private readonly IEnumerable values;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="values">The collection, the array, or the simple enumerable object to count elements.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="values"/> is null.</exception>
        public EnumerableCounter(IEnumerable values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            this.values = values;
        }

        /// <summary>
        /// Counts the number of elements in a collection, an array, or any other enumerable type, by
        /// using the most appropriate strategies.
        /// </summary>
        /// <returns>An enumeration of counting strategies applicable to the specified enumerable values.</returns>
        public IEnumerable<ICountingStrategy> Count()
        {
            var methods = new Func<IEnumerable<ICountingStrategy>>[]
            {
                ForArray,
                ForGenericCollection,
                ForNonGenericCollection,
                ForSimpleEnumerable,
            };

            foreach (var method in methods)
            {
                bool found = false;

                foreach (var strategy in method())
                {
                    found = true;
                    yield return strategy;
                }

                if (found)
                    break;
            }
        }

        private IEnumerable<ICountingStrategy> ForArray()
        {
            if (values is Array)
            {
                yield return CountingStrategy.ByLengthGetter((Array)values);
            }
        }

        private IEnumerable<ICountingStrategy> ForGenericCollection()
        {
            var collection = values as ICollection;

            if (!ReferenceEquals(null, collection))
            {
                yield return CountingStrategy.ByCountGetter(() => collection.Count);
                yield return CountingStrategy.ByEnumeratingElements(values);
            }
        }

        private IEnumerable<ICountingStrategy> ForNonGenericCollection()
        {
            foreach (Type type in values.GetType().GetInterfaces())
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>))
                {
                    yield return CountingStrategy.ByCountGetter(() => (int)type.GetProperty("Count").GetValue(values, null));
                    yield return CountingStrategy.ByEnumeratingElements(values);
                }
            }
        }

        private IEnumerable<ICountingStrategy> ForSimpleEnumerable()
        {
            var type = values.GetType();
            var property = type.GetProperty("Count", BindingFlags.Instance | BindingFlags.Public, null, typeof(int), EmptyArray<Type>.Instance, null);

            if (property != null)
                yield return CountingStrategy.ByReflectedCountGetter(() => (int)property.GetValue(values, null));

            yield return CountingStrategy.ByEnumeratingElements(values);
        }
    }

    /// <summary>
    /// Represents a strategy for counting the number of elements in a collection, an array, or a simple enumerable type.
    /// </summary>
    public interface ICountingStrategy
    {
        /// <summary>
        /// Gets the name of the counting strategy.
        /// </summary>
        CountingStrategyName Name
        {
            get;
        }

        /// <summary>
        /// Gets a displayable description of the counting strategy.
        /// </summary>
        string Description
        {
            get;
        }

        /// <summary>
        /// Returns the number of elements found by applying the current counting strategy.
        /// </summary>
        /// <returns>The number of elements found.</returns>
        int Count
        {
            get;
        }
    }

    /// <summary>
    /// Name of the existing counting strategies.
    /// </summary>
    public enum CountingStrategyName
    {
        /// <summary>
        /// Counts by getting the value returned by the <see cref="Array.Length"/> property getter.
        /// </summary>
        ByLengthGetter,

        /// <summary>
        /// Counts by getting the value returned by the <see cref="ICollection.Count"/> or <see cref="ICollection{T}.Count"/> property getter.
        /// </summary>
        ByCountGetter,

        /// <summary>
        /// Counts by enumerating the elements returned by <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        ByEnumeratingElements,

        /// <summary>
        /// Counts by getting the value returned by an existing public "Count" property getter.
        /// </summary>
        ByReflectedCountGetter,
    }

    internal class CountingStrategy : ICountingStrategy
    {
        private readonly string description;
        private readonly CountingStrategyName name; 
        private readonly Lazy<int> lazyCount;

        private CountingStrategy(CountingStrategyName name, string description, Func<int> counting)
        {
            this.name = name;
            this.description = description;
            lazyCount = new Lazy<int>(counting);
        }

        public CountingStrategyName Name
        {
            get
            {
                return name;
            }
        }

        public int Count
        {
            get
            {
                return lazyCount.Value;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }
        }

        public static ICountingStrategy ByLengthGetter(Array array)
        {
            return new CountingStrategy(CountingStrategyName.ByLengthGetter, "By Length Property", () => array.Length);
        }

        public static ICountingStrategy ByCountGetter(Func<int> countReader)
        {
            return new CountingStrategy(CountingStrategyName.ByCountGetter, "By Count Property", countReader);
        }

        public static ICountingStrategy ByEnumeratingElements(IEnumerable values)
        {
            return new CountingStrategy(CountingStrategyName.ByEnumeratingElements, "By Enumerating Elements", () =>
            {
                int count = 0;
                var enumerator = values.GetEnumerator();

                while (enumerator.MoveNext())
                    count++;

                return count;
            });
        }

        public static ICountingStrategy ByReflectedCountGetter(Func<int> countReader)
        {
            return new CountingStrategy(CountingStrategyName.ByReflectedCountGetter, "By Reflected Count Property", countReader);
        }
    }
}
