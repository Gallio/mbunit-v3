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
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Model.Data
{
    public class DataBindingSet
    {
        private readonly DataBinding[] bindings;

        public DataBindingSet(DataBinding[] bindings)
        {
            this.bindings = bindings;
        }

        /// <summary>
        /// Groups the specified bindings together so that they are generated
        /// from data providers together as a set.  Ungrouped bindings are
        /// generated combinatorially.
        /// </summary>
        /// <remarks>
        /// Grouping is transitive.  If binding A is grouped with binding B and
        /// binding B is grouped with binding C then A, B, and C are grouped
        /// together.
        /// </remarks>
        /// <param name="bindings">The bindings to group</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="bindings"/>
        /// is null</exception>
        public void Group(IList<DataBinding> bindings)
        {
            throw new NotImplementedException();
        }
    }
}
