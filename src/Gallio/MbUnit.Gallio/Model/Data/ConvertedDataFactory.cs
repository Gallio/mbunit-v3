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
    /// <summary>
    /// A data factory that applies a converter to the value it obtains.
    /// The implementation assumes that the converter supports the desired
    /// conversion.
    /// </summary>
    public class ConvertedDataFactory : IDataFactory
    {
        private readonly IConverter converter;
        private readonly IDataFactory inner;
        private readonly Type targetType;

        /// <summary>
        /// Creates a converted data factory.
        /// </summary>
        /// <param name="inner">The inner factory</param>
        /// <param name="converter">The converter</param>
        /// <param name="targetType">The target type to which the value produced by
        /// the inner factory should be converted</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="inner"/>,
        /// <paramref name="converter"/> or <paramref name="targetType"/> is null</exception>
        public ConvertedDataFactory(IDataFactory inner, IConverter converter, Type targetType)
        {
            if (inner == null)
                throw new ArgumentNullException("inner");
            if (converter == null)
                throw new ArgumentNullException("converter");
            if (targetType == null)
                throw new ArgumentNullException("targetType");

            this.inner = inner;
            this.converter = converter;
            this.targetType = targetType;
        }

        /// <inheritdoc />
        public object GetValue(IDataBindingContext context)
        {
            object sourceValue = inner.GetValue(context);
            return converter.Convert(sourceValue, targetType);
        }
    }
}
