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
using Gallio.Framework.Data.Conversions;

namespace Gallio.Framework.Data.Binders
{
    /// <summary>
    /// A <see cref="IDataBindingAccessor" /> that converts a value before returning it.
    /// </summary>
    public sealed class ConvertedDataBindingAccessor : IDataBindingAccessor
    {
        private readonly IConverter converter;
        private readonly DataBinding binding;

        /// <summary>
        /// Creates a converted data binding accessor.
        /// </summary>
        /// <param name="converter">The converter for converting the raw value in the row
        /// to the requested type when needed</param>
        /// <param name="binding">The binding to query</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="converter"/> or
        /// <paramref name="binding"/> is null</exception>
        public ConvertedDataBindingAccessor(IConverter converter, DataBinding binding)
        {
            if (converter == null)
                throw new ArgumentNullException("converter");
            if (binding == null)
                throw new ArgumentNullException("binding");

            this.converter = converter;
            this.binding = binding;
        }

        /// <inheritdoc />
        public object GetValue(DataBindingItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            object value = item.GetRow().GetValue(binding);
            return converter.Convert(value, binding.Type);
        }
    }
}
