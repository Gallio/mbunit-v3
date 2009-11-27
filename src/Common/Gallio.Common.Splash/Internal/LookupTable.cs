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
