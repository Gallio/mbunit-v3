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
using MbUnit.Model;

namespace MbUnit.Model.Filters
{
    /// <summary>
    /// A filter that matches objects whose <see cref="IModelComponent.Id" />
    /// equals the specified value.
    /// </summary>
    [Serializable]
    public class IdFilter<T> : Filter<T> where T : IModelComponent
    {
        private string id;

        /// <summary>
        /// Creates an identity filter.
        /// </summary>
        /// <param name="id">The id to look for</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> is null</exception>
        public IdFilter(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            
            this.id = id;
        }

        /// <inheritdoc />
        public override bool IsMatch(T value)
        {
            return value.Id == id;
        }
    }
}