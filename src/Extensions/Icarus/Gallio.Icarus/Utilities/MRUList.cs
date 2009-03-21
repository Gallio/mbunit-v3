using System.Collections.Generic;

namespace Gallio.Icarus.Utilities
{
    public class MRUList
    {
        private readonly List<string> items;

        public IList<string> Items
        {
            get { return items; }
        }

        public int MaxItems { get; set; }

        public MRUList(List<string> items, int maxItems)
        {
            this.items = items;
            MaxItems = maxItems;
        }

        public void Add(string item)
        {
            if (items.Count == MaxItems)
                items.RemoveAt(MaxItems - 1);
            items.Insert(0, item);
        }
    }
}
