using System;
using System.Collections.Generic;

namespace Gallio.Icarus.Utilities
{
    /// <summary>
    /// Most Recently Used items list.
    /// </summary>
    public class MRUList
    {
        private readonly List<string> items;

        public IList<string> Items
        {
            get { return items; }
        }

        public int Count
        {
            get { return items.Count; }
        }

        /// <summary>
        /// The maximum number of items that can be
        /// in the list. 
        /// </summary>
        public int MaxItems { get; set; }

        public MRUList(List<string> items, int maxItems)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            this.items = items;
            MaxItems = maxItems;
        }

        /// <summary>
        /// Add an item to the list (e.g. MRU projects).
        /// Always added to the front, once the list reaches 
        /// capacity (MaxItems) items fall of the end.
        /// If the item is already in the list, it is removed
        /// and still added to the front.
        /// </summary>
        /// <param name="item"></param>
        public void Add(string item)
        {
            if (items.Contains(item))
                items.Remove(item);

            if (items.Count == MaxItems)
                items.RemoveAt(MaxItems - 1);

            items.Insert(0, item);
        }
    }
}
