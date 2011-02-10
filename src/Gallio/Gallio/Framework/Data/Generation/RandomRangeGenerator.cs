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
using System.Text;
using System.Collections;
using Gallio.Common.Security;

namespace Gallio.Framework.Data.Generation
{
    /// <summary>
    /// Generic abstract generator of random values within a given range.
    /// </summary>
    /// <typeparam name="T">The type of the value to generate.</typeparam>
    public abstract class RandomRangeGenerator<T> : RandomGenerator<T>
        where T : struct, IComparable<T>, IEquatable<T>
    {
        /// <summary>
        /// Gets the lower bound of the range.
        /// </summary>
        public T? Minimum
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the upper bound of the range.
        /// </summary>
        public T? Maximum
        {
            get;
            set;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected RandomRangeGenerator()
        {
        }

        /// <inheritdoc/>
        protected override void Verify()
        {
            base.Verify();

            if (!Minimum.HasValue)
                throw new GenerationException("The 'Minimum' property must be initialized.");
            if (!Maximum.HasValue)
                throw new GenerationException("The 'Maximum' property must be initialized.");
            if (Minimum.Value.CompareTo(Maximum.Value) > 0)
                throw new GenerationException("The 'Minimum' property must be less than or equal to the 'Maximum' property.");
        }
    }
}