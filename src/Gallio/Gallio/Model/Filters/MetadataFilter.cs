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
using Gallio.Model;

namespace Gallio.Model.Filters
{
    /// <summary>
    /// A filter that matches objects whose <see cref="ITestComponent.Metadata" />
    /// has a key with a value that matches the value filter.
    /// </summary>
    [Serializable]
    public class MetadataFilter<T> : PropertyFilter<T> where T : ITestComponent
    {
        private readonly string key;

        /// <summary>
        /// Creates a metadata filter.
        /// </summary>
        /// <param name="key">The metadata key to look for</param>
        /// <param name="valueFilter">A filter for the metadata value to match</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> or <paramref name="valueFilter"/> is null</exception>
        public MetadataFilter(string key, Filter<string> valueFilter)
            : base(valueFilter)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            
            this.key = key;
        }

        /// <inheritdoc />
        public override string Key
        {
            get { return key; }
        }

        /// <inheritdoc />
        public override bool IsMatch(T value)
        {
            foreach (string assocValue in value.Metadata[key])
                if (ValueFilter.IsMatch(assocValue))
                    return true;

            return false;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return @"Metadata('" + key + @"', " + ValueFilter + @")";
        }
    }
}
