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

namespace Gallio.Common.Xml
{
    /// <summary>
    /// A simple builder for the <see cref="DiffSet"/> object.
    /// </summary>
    public class DiffSetBuilder
    {
        private readonly List<Diff> items = new List<Diff>();

        /// <summary>
        /// Adds the specified diff item to the set.
        /// </summary>
        /// <param name="item">The diff item to be added.</param>
        /// <returns>A reference to the builder itself.</returns>
        public DiffSetBuilder Add(Diff item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
        
            items.Add(item);
            return this;
        }

        /// <summary>
        /// Adds the diff items contained into the specified set to the current set.
        /// </summary>
        /// <param name="items">The diff items to be added.</param>
        /// <returns>A reference to the builder itself.</returns>
        public DiffSetBuilder Add(DiffSet items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            foreach (var item in items)
            {
                Add(item);
            }

            return this;
        }

        /// <summary>
        /// Returns a new <see cref="DiffSet"/> based on the current content of the builder.
        /// </summary>
        /// <returns>The resulting diff set object.</returns>
        public DiffSet ToDiffSet()
        {
            return new DiffSet(items);
        }
    }
}
