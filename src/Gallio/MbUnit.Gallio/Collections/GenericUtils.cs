// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Text;

namespace MbUnit.Collections
{
    /// <summary>
    /// Utility functions for manipulating generic collections.
    /// </summary>
    public static class GenericUtils
    {
        /// <summary>
        /// Converts each element of the input collection and stores the result in the
        /// output list using the same index.  The output list must be at least as
        /// large as the input list.
        /// </summary>
        /// <typeparam name="TInput">The input type</typeparam>
        /// <typeparam name="TOutput">The output type</typeparam>
        /// <param name="input">The input list</param>
        /// <param name="output">The output list</param>
        /// <param name="converter">The conversion function to apply to each element</param>
        public static void ConvertAndCopyAll<TInput, TOutput>(ICollection<TInput> input, IList<TOutput> output,
            Converter<TInput, TOutput> converter)
        {
            int i = 0;
            foreach (TInput value in input)
                output[i++] = converter(value);
        }

        /// <summary>
        /// Converts each element of the input collection and adds the result to the
        /// output collection succession in the same order.
        /// </summary>
        /// <typeparam name="TInput">The input type</typeparam>
        /// <typeparam name="TOutput">The output type</typeparam>
        /// <param name="input">The input list</param>
        /// <param name="output">The output list</param>
        /// <param name="converter">The conversion function to apply to each element</param>
        public static void ConvertAndAddAll<TInput, TOutput>(ICollection<TInput> input, ICollection<TOutput> output,
            Converter<TInput, TOutput> converter)
        {
            foreach (TInput value in input)
                output.Add(converter(value));
        }

        /// <summary>
        /// Converts each element of the input collection and returns the collected results as an array
        /// of the same size.
        /// </summary>
        /// <typeparam name="TInput">The input type</typeparam>
        /// <typeparam name="TOutput">The output type</typeparam>
        /// <param name="input">The input collection</param>
        /// <param name="converter">The conversion function to apply to each element</param>
        /// <returns>The output array</returns>
        public static TOutput[] ConvertAllToArray<TInput, TOutput>(ICollection<TInput> input,
            Converter<TInput, TOutput> converter)
        {
            TOutput[] output = new TOutput[input.Count];
            ConvertAndCopyAll(input, output, converter);
            return output;
        }

        /// <summary>
        /// Copies all of the elements of the input collection to an array.
        /// </summary>
        /// <typeparam name="T">The element type</typeparam>
        /// <param name="collection">The input collection</param>
        /// <returns>The output array</returns>
        public static T[] ToArray<T>(ICollection<T> collection)
        {
            T[] output = new T[collection.Count];

            int i = 0;
            foreach (T value in collection)
                output[i++] = value;

            return output;
        }

        /// <summary>
        /// Returns the first element of the input enumeration for which the specified
        /// predicate returns true.
        /// </summary>
        /// <typeparam name="T">The element type</typeparam>
        /// <param name="enumeration">The input enumeration</param>
        /// <param name="predicate">The predicate</param>
        /// <returns>The first matching value or the default for the type if not found</returns>
        public static T Find<T>(IEnumerable<T> enumeration, Predicate<T> predicate)
        {
            foreach (T value in enumeration)
                if (predicate(value))
                    return value;

            return default(T);
        }
    }
}