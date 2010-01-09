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
//#define STATISTICS

using System;

namespace Gallio.Common
{
    /// <summary>
    /// A structure that memoizes the result of some computation for later reuse.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Not thread safe.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">The value type.</typeparam>
    /// <example>
    /// <code><![CDATA[
    /// public class MyClass
    /// {
    ///     // Do NOT put the "readonly" keyword on this field.
    ///     // Otherwise we will not be able to modify the contents of the structure and memoization will not occur.
    ///     private Memoizer<int> valueMemoizer = new Memoizer<int>();
    ///     
    ///     public int GetValue()
    ///     {
    ///         return valueMemoizer.Memoize(() =>
    ///         {
    ///             // Expensive calculation here.
    ///             return 42;
    ///         });
    ///     }
    /// }
    /// ]]></code>
    /// </example>
    public struct Memoizer<T>
    {
        private T value;
        private bool isPopulated;

        /// <summary>
        /// Gets the memoized value if available, otherwise populates it
        /// using the specified populator function and stores it for later reuse.
        /// </summary>
        /// <param name="populator">The populator.</param>
        /// <returns>The value returned by the populator, possibly memoized.</returns>
        public T Memoize(Func<T> populator)
        {
            if (!isPopulated)
            {
#if STATISTICS
                OnMiss();
#endif
                value = populator();
                isPopulated = true;
            }
            else
            {
#if STATISTICS
                OnHit();
#endif
            }

#if STATISTICS
            OnReturn();
#endif
            return value;
        }

#if STATISTICS
        private static int hits;
        private static int misses;
        private static DateTime nextSample;

        private static void OnHit()
        {
            hits += 1;
        }

        private static void OnMiss()
        {
            misses += 1;
        }

        private static void OnReturn()
        {
            DateTime now = DateTime.Now;
            if (now > nextSample)
            {
                nextSample = now.AddSeconds(5);
                DiagnosticLog.WriteLine(
                    "Memoizer<{0}>: Hits = {1}, Misses = {2}",
                    typeof(T).Name, hits, misses);
            }
        }
#endif
    }
}