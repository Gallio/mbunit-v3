// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

namespace Gallio.Framework.Data
{
    /// <summary>
    /// A <see cref="IDataAccessor" /> that simply accesses a value contained
    /// by a data item using a particular <see cref="DataBinding" />.
    /// </summary>
    public sealed class BoundDataAccessor : BaseDataAccessor
    {
        private readonly DataBinding binding;

        /// <summary>
        /// Creates a bound data accessor.
        /// </summary>
        /// <param name="binding">The binding to query</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="binding"/> is null</exception>
        public BoundDataAccessor(DataBinding binding)
        {
            if (binding == null)
                throw new ArgumentNullException("binding");

            this.binding = binding;
        }

        /// <inheritdoc />
        protected override object GetValueImpl(IDataItem item)
        {
            object value = item.GetValue(binding);
            return value;
        }
    }
}
