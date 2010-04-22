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
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Gallio.Common;
using Gallio.Framework.Data;
using Gallio.Framework.Data.Generation;

namespace MbUnit.Framework
{
    /// <summary>
    /// Helper methods to quickly combine and generate random or constrained values for data-driven tests.
    /// </summary>
    public static partial class DataGenerators
    {
        private static readonly IDictionary<JoinStrategy, IJoinStrategy> map = new Dictionary<JoinStrategy, IJoinStrategy>
        {
            { JoinStrategy.Combinatorial, CombinatorialJoinStrategy.Instance },
            { JoinStrategy.Sequential, SequentialJoinStrategy.Instance },
            { JoinStrategy.Pairwise, PairwiseJoinStrategy.Instance },
        };

        /// <summary>
        /// Joins the two specified enumerations of values into a single enumeration of paired values 
        /// by applying the default combinatorial join strategy.
        /// </summary>
        /// <typeparam name="T1">The type of the values in the first enumeration.</typeparam>
        /// <typeparam name="T2">The type of the values in the first enumeration.</typeparam>
        /// <param name="values1">The first enumeration of values.</param>
        /// <param name="values2">The second enumeration of values.</param>
        /// <returns>The resulting enumeration of paired values.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="values1"/> or <paramref name="values2"/> is null.</exception>
        /// <seealso cref="Join{T1,T2}(IEnumerable{T1},IEnumerable{T2},JoinStrategy)"/>
        /// <seealso cref="CombinatorialJoinAttribute"/>
        public static IEnumerable<Pair<T1, T2>> Join<T1, T2>(IEnumerable<T1> values1, IEnumerable<T2> values2)
        {
            return Join(values1, values2, JoinStrategy.Combinatorial);
        }

        /// <summary>
        /// Joins the two specified enumerations of values into a single enumeration of paired values 
        /// by applying the specified join strategy.
        /// </summary>
        /// <typeparam name="T1">The type of the values in the first enumeration.</typeparam>
        /// <typeparam name="T2">The type of the values in the first enumeration.</typeparam>
        /// <param name="values1">The first enumeration of values.</param>
        /// <param name="values2">The second enumeration of values.</param>
        /// <param name="joinStrategy">The join strategy to use.</param>
        /// <returns>The resulting enumeration of paired values.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="values1"/> or <paramref name="values2"/> is null.</exception>
        /// <seealso cref="Join{T1,T2}(IEnumerable{T1},IEnumerable{T2})"/>
        /// <seealso cref="CombinatorialJoinAttribute"/>
        /// <seealso cref="SequentialJoinAttribute"/>
        /// <seealso cref="PairwiseJoinAttribute"/>
        public static IEnumerable<Pair<T1, T2>> Join<T1, T2>(IEnumerable<T1> values1, IEnumerable<T2> values2, JoinStrategy joinStrategy)
        {
            var providers = new List<IDataProvider>
            {
                new ValueSequenceDataSet(values1, null, false),
                new ValueSequenceDataSet(values2, null, false)
            };

            var binding = new DataBinding(0, null);
            var bindings = new[] { new[] { binding }, new[] { binding } };

            foreach (var items in map[joinStrategy].Join(providers, bindings, false))
            {
                yield return new Pair<T1, T2>(
                    (T1)items[0].GetValue(binding),
                    (T2)items[1].GetValue(binding));
            }
        }

        /// <summary>
        /// Joins the three specified enumerations of values into a single enumeration of triplet values 
        /// by applying the default combinatorial join strategy.
        /// </summary>
        /// <typeparam name="T1">The type of the values in the first enumeration.</typeparam>
        /// <typeparam name="T2">The type of the values in the second enumeration.</typeparam>
        /// <typeparam name="T3">The type of the values in the third enumeration.</typeparam>
        /// <param name="values1">The first enumeration of values.</param>
        /// <param name="values2">The second enumeration of values.</param>
        /// <param name="values3">The third enumeration of values.</param>
        /// <returns>The resulting enumeration of triplet values.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="values1"/>, <paramref name="values2"/>, or <paramref name="values3"/> is null.</exception>
        /// <seealso cref="Join{T1,T2,T3}(IEnumerable{T1},IEnumerable{T2},IEnumerable{T3},JoinStrategy)"/>
        /// <seealso cref="CombinatorialJoinAttribute"/>
        public static IEnumerable<Triple<T1, T2, T3>> Join<T1, T2, T3>(IEnumerable<T1> values1, IEnumerable<T2> values2, IEnumerable<T3> values3)
        {
            return Join(values1, values2, values3, JoinStrategy.Combinatorial);
        }

        /// <summary>
        /// Joins the two specified enumerations of values into a single enumeration of triplet values 
        /// by applying the specified join strategy.
        /// </summary>
        /// <typeparam name="T1">The type of the values in the first enumeration.</typeparam>
        /// <typeparam name="T2">The type of the values in the second enumeration.</typeparam>
        /// <typeparam name="T3">The type of the values in the third enumeration.</typeparam>
        /// <param name="values1">The first enumeration of values.</param>
        /// <param name="values2">The second enumeration of values.</param>
        /// <param name="values3">The third enumeration of values.</param>
        /// <param name="joinStrategy">The join strategy to use.</param>
        /// <returns>The resulting enumeration of triplet values.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="values1"/>, <paramref name="values2"/>, or <paramref name="values3"/> is null.</exception>
        /// <seealso cref="Join{T1,T2,T3}(IEnumerable{T1},IEnumerable{T2},IEnumerable{T3})"/>
        /// <seealso cref="CombinatorialJoinAttribute"/>
        /// <seealso cref="SequentialJoinAttribute"/>
        /// <seealso cref="PairwiseJoinAttribute"/>
        public static IEnumerable<Triple<T1, T2, T3>> Join<T1, T2, T3>(IEnumerable<T1> values1, IEnumerable<T2> values2, IEnumerable<T3> values3, JoinStrategy joinStrategy)
        {
            var providers = new List<IDataProvider>
            {
                new ValueSequenceDataSet(values1, null, false),
                new ValueSequenceDataSet(values2, null, false),
                new ValueSequenceDataSet(values3, null, false)
            };

            var binding = new DataBinding(0, null);
            var bindings = new[] { new[] { binding }, new[] { binding }, new[] { binding } };

            foreach (var items in map[joinStrategy].Join(providers, bindings, false))
            {
                yield return new Triple<T1, T2, T3>(
                    (T1)items[0].GetValue(binding), 
                    (T2)items[1].GetValue(binding), 
                    (T3)items[2].GetValue(binding));
            }
        }
    }
}
