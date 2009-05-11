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
    /// Generator of sequential <see cref="Int32"/> values.
    /// </summary>
    public class SequenceInt32Generator : SequenceGenerator<int>
    {
        /// <summary>
        /// Constructs a generator of sequential <see cref="Int32"/> numbers.
        /// </summary>
        /// <param name="from">The starting point of the sequence.</param>
        /// <param name="step">The increment between each value of the sequence.</param>
        /// <param name="count">The length of the sequence of values that the generator must create.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="count"/> is negative, 
        /// or if <paramref name="from"/> or <paramref name="step"/> are one of the following:        
        /// <list type="bullet">
        /// <item><see cref="Int32.MinValue"/></item>
        /// <item><see cref="Int32.MinValue"/></item>
        /// </list>
        /// </exception>
        public SequenceInt32Generator(int from, int step, int count)
            : base(from, step, count)
        {
            CheckValidInt32(from, "from");
            CheckValidInt32(step, "step");
        }

        /// <inheritdoc/>
        protected sealed override int GetValue(int index)
        {
            return From + index * Step;
        }
    }
}
