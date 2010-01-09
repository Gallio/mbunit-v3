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
using System.ComponentModel;

namespace Gallio.Icarus.Utilities
{
    /// <summary>
    /// Most Recently Used items list.
    /// </summary>
    public class MRUList : INotifyPropertyChanged
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
        public int MaxItems { get; private set; }

        public MRUList(List<string> items, int maxItems)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            if (items.Count > maxItems)
                throw new Exception("Input list contains too many items!");

            MaxItems = maxItems;
            this.items = items;

            OnPropertyChanged(new PropertyChangedEventArgs("Items"));
        }

        /// <summary>
        /// Add an item to the list (e.g. MRU projects).
        /// Always added to the front, once the list reaches 
        /// capacity (MaxItems) items fall of the end.
        /// If the item is already in the list, it is removed
        /// and still added to the front.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        public void Add(string item)
        {
            if (items.Contains(item))
                items.Remove(item);

            if (items.Count == MaxItems)
                items.RemoveAt(MaxItems - 1);

            items.Insert(0, item);

            OnPropertyChanged(new PropertyChangedEventArgs("Items"));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
    }
}
