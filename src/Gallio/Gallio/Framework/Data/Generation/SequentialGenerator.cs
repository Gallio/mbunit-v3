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

namespace Gallio.Framework.Data.Generation
{
    /// <summary>
    /// Generic abstract generator of sequential values.
    /// </summary>
    /// <typeparam name="T">The type of the value to generate.</typeparam>
    public abstract class SequentialGenerator<T> : Generator<T>
        where T : IComparable<T>
    {
        private readonly T start;
        private readonly T step;

        /// <summary>
        /// Gets the starting point of the sequence.
        /// </summary>
        public T Start
        {
            get
            {
                return start;
            }
        }

        /// <summary>
        /// Gets the increment between each value of the sequence.
        /// </summary>
        public T Step
        {
            get
            {
                return step;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="start">The starting point of the sequence.</param>
        /// <param name="step">The increment between each value of the sequence.</param>
        /// <param name="count">The length of the sequence of values that the generator must create.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="count"/> is negative.</exception>
        protected SequentialGenerator(T start, T step, int count)
            : base(count)
        {
            this.start = start;
            this.step = step;
        }
    }
}
