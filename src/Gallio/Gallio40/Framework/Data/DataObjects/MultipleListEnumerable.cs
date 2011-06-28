// Copyright 2005-2011 Gallio Project - http://www.gallio.org/
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
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;

namespace Gallio.Framework.Data.DataObjects
{
    /// <summary>
    /// Abstracts the 
    /// </summary>
    public class MultipleListEnumerable
    {
        /// <summary>
        /// Returns interface to enumerator of all possible ordered combinations of the Lists of XElements
        /// aka the Cartesian Product.
        /// </summary>
        /// <param name="ListOfNodeLists">List of lists of XElements</param>
        /// <returns>Interface to enumerator of XElement arrays. The array will has as many cells as
        /// there are lists of XElements.</returns>
        public static IEnumerable<T[]> CartesianYield<T>(List<List<T>> ListOfNodeLists)
        {
            T[] OutputArray = new T[ListOfNodeLists.Count];

            foreach (T[] TestRootElementArray in CatersianYieldInner<T>(ListOfNodeLists, OutputArray, 0))
            {
                yield return TestRootElementArray;
            }
        }

        /// <summary>
        /// Helper method will incremental traverse the List of Node Lists until the Current Index reaches the 
        /// end of the "outer" list.  Recursively calls self to generate all possible combinations of a single
        /// element from each list.  Output Array contains the current combination.
        /// </summary>
        private static IEnumerable<T[]> CatersianYieldInner<T>(List<List<T>> ListOfNodeLists, T[] OutputArray, int CurrentIndex)
        {
            if (CurrentIndex == ListOfNodeLists.Count)
            {
                // We've reached the end of the recursion depth - start yielding results
                yield return OutputArray;
            }
            else
            {
                // Get the current list which we are processing
                List<T> CurrentList = ListOfNodeLists[CurrentIndex];

                // This loop ensures that we process every element in the current list
                for (int Counter = 0; Counter < CurrentList.Count; ++Counter)
                {
                    // Set the corresponding array cell to the current lists' value
                    OutputArray[CurrentIndex] = CurrentList[Counter];

                    // Recursive process the next list
                    foreach (T[] Element in 
                        CatersianYieldInner<T>(ListOfNodeLists, OutputArray, CurrentIndex + 1))
                    {
                        yield return Element;
                    }
                }
            }
        }

    }
}
