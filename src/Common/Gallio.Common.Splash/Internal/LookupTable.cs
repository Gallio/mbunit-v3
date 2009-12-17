// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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

namespace Gallio.Common.Splash.Internal
{
    internal sealed class LookupTable<T>
    {
        private readonly int maxCount;
        private readonly string maxCountExceededMessageFormat;
        private readonly Dictionary<T, int> dict;
        private readonly List<T> list;

        public LookupTable(int maxCount, string maxCountExceededMessageFormat)
        {
            this.maxCount = maxCount;
            this.maxCountExceededMessageFormat = maxCountExceededMessageFormat;

            dict = new Dictionary<T, int>();
            list = new List<T>();
        }

        public int Count
        {
            get { return list.Count; }
        }

        public T this[int index]
        {
            get { return list[index]; }
        }

        public void Clear()
        {
            dict.Clear();
            list.Clear();
        }

        public int AssignIndex(T item)
        {
            int index;
            if (!dict.TryGetValue(item, out index))
            {
                index = dict.Count;

                if (index >= maxCount)
                    throw new InvalidOperationException(string.Format(maxCountExceededMessageFormat, maxCount));

                dict.Add(item, index);
                list.Add(item);
            }

            return index;
        }
    }
}
