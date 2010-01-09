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
using Gallio.Common;

namespace MbUnit.Framework
{
    // Helper class used to compare two sequences of unordered elements.
    internal class MatchTable<T>
    {
        private readonly EqualityComparison<T> comparer;
        private readonly List<KeyValuePair<T, Pair<int, int>>> items;
        private int nonEqualCount;

        public MatchTable(EqualityComparison<T> comparer)
        {
            this.comparer = comparer;
            items = new List<KeyValuePair<T, Pair<int, int>>>();
        }

        public int NonEqualCount
        {
            get
            {
                return nonEqualCount;
            }
        }

        public IEnumerable<KeyValuePair<T, Pair<int, int>>> Items
        {
            get
            {
                return items;
            }
        }

        public void AddLeftValue(T key)
        {
            Add(key, 1, 0);
        }

        public void AddRightValue(T key)
        {
            Add(key, 0, 1);
        }

        private void Add(T key, int expectedCount, int actualCount)
        {
            for (int i = 0; i < items.Count; i++)
            {
                KeyValuePair<T, Pair<int, int>> item = items[i];
                if (comparer(item.Key, key))
                {
                    Pair<int, int> oldCounters = items[i].Value;
                    Pair<int, int> newCounters = new Pair<int, int>(oldCounters.First + expectedCount, oldCounters.Second + actualCount);
                    items[i] = new KeyValuePair<T, Pair<int, int>>(item.Key, newCounters);

                    if (newCounters.First == newCounters.Second)
                        nonEqualCount -= 1;
                    else if (oldCounters.First == oldCounters.Second)
                        nonEqualCount += 1;

                    return;
                }
            }

            items.Add(new KeyValuePair<T, Pair<int, int>>(key, new Pair<int, int>(expectedCount, actualCount)));
            nonEqualCount += 1;
        }
    }
}
