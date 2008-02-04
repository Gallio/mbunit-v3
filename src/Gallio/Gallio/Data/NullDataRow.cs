// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Collections;
using Gallio.Reflection;

namespace Gallio.Data
{
    /// <summary>
    /// A null data row simply returns the default value for the type of
    /// any binding that is requested.  It has no metadata and it ignores
    /// disposal.
    /// </summary>
    public sealed class NullDataRow : IDataRow
    {
        /// <summary>
        /// Gets the singleton null data row instance.
        /// </summary>
        public static readonly NullDataRow Instance = new NullDataRow();

        private NullDataRow()
        {
        }

        /// <inheritdoc />
        public IEnumerable<KeyValuePair<string, string>> GetMetadata()
        {
            return EmptyArray<KeyValuePair<string, string>>.Instance;
        }

        /// <inheritdoc />
        public object GetValue(DataBinding binding)
        {
            if (binding == null)
                throw new ArgumentNullException("binding");

            return ReflectionUtils.GetDefaultValue(binding.Type);
        }
    }
}
