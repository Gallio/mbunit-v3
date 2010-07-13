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
using System.Linq;
using System.Text;
using Gallio.Common.Collections;
using Gallio.Framework;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Collections
{
    [TestFixture]
    [TestsOn(typeof(EnumerableCounter))]
    public class EnumerableCounterTest
    {
        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_values_should_throw_exception()
        {
            new EnumerableCounter(null);
        }

        private static void AssertStrategies(IEnumerable<ICountingStrategy> actualStrategies, params CountingStrategyName[] expectedNames)
        {
            Assert.AreElementsEqualIgnoringOrder(expectedNames, actualStrategies.Select(x => x.Name));
            Assert.ForAll(actualStrategies, o => o.Count == 3);
        }

        [Test]
        public void Counts_simple_enumerable()
        {
            var counter = new EnumerableCounter(GetSimpleEnumerable());
            AssertStrategies(counter.Count(), CountingStrategyName.ByEnumeratingElements);
        }

        [Test]
        public void Counts_custom_enumerable_with_count()
        {
            var counter = new EnumerableCounter(new CustomEnumerable());
            AssertStrategies(counter.Count(), CountingStrategyName.ByEnumeratingElements, CountingStrategyName.ByReflectedCountGetter);
        }

        [Test]
        public void Counts_array()
        {
            var counter = new EnumerableCounter(GetArray());
            AssertStrategies(counter.Count(), CountingStrategyName.ByLengthGetter);
        }


        [Test]
        public void Counts_mixed_generic_collection() // Implemented both ICollection and ICollection<T>
        {
            var counter = new EnumerableCounter(GetGenericList());
            AssertStrategies(counter.Count(), CountingStrategyName.ByCountGetter, CountingStrategyName.ByEnumeratingElements);
        }

        [Test]
        public void Counts_non_generic_collection() // Implemented ICollection only
        {
            var counter = new EnumerableCounter(GetArrayList());
            AssertStrategies(counter.Count(), CountingStrategyName.ByCountGetter, CountingStrategyName.ByEnumeratingElements);
        }

        [Test]
        public void Counts_pure_generic_collection() // Implemented ICollection<T> only
        {
            var counter = new EnumerableCounter(GetHashSet());
            AssertStrategies(counter.Count(), CountingStrategyName.ByCountGetter, CountingStrategyName.ByEnumeratingElements);
        }

        #region Samples

        private static IEnumerable GetSimpleEnumerable()
        {
            yield return "Alpha";
            yield return "Beta";
            yield return "Gamma";
        }

        private class CustomEnumerable : IEnumerable
        {
            public int Count
            {
                get
                {
                    return 3;
                }
            }

            public IEnumerator GetEnumerator()
            {
                yield return "Alpha";
                yield return "Beta";
                yield return "Gamma";
            }
        }

        private static IEnumerable GetArray()
        {
            return GetSimpleEnumerable().Cast<string>().ToArray();
        }

        private static IEnumerable GetGenericList()
        {
            return GetSimpleEnumerable().Cast<string>().ToList();
        }

        private static IEnumerable GetArrayList()
        {
            var collection = new ArrayList();

            foreach (var o in GetSimpleEnumerable())
                collection.Add(o);

            return collection;
        }

        private static IEnumerable GetHashSet() // Interesting case because it does not implement ICollection!
        {
            return new System.Collections.Generic.HashSet<string>(GetSimpleEnumerable().Cast<string>());
        }

        #endregion
    }
}
